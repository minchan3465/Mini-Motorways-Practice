using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Managers;
	using Motorways.Models;
	using Utils;

	public class BuildingSpawningProcess : MonoBehaviour, ISimulationProcess {
		public static BuildingSpawningProcess Instance;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		public void Tick(float dt) {
			var scheduleList = BuildingManager.Instance.ScheduleList;

			if (scheduleList.Count > 0) {
				ScheduledBuilding ticket = scheduleList[0];

				if (ticket.SpawnTime <= Time.time) {
					if (TrySpawnBuilding(ticket)) {
						scheduleList.RemoveAt(0);
					} else {
						ticket.SpawnAttempts++;
						ticket.SpawnTime += 5.0f;
						scheduleList.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
					}
				}
			}
		}

		private bool TrySpawnBuilding(ScheduledBuilding ticket) {
			bool isHouse = ticket.Type == BuildingType.House;
			var layouts = isHouse ? BuildingManager.Instance.HouseLayouts : BuildingManager.Instance.DestinationLayouts;

			//좌표 선택 (랜덤으로, 룰렛 휠을 사용)
			if (!TryPickRandomCoordinate(ticket.Type, out Vector2Int rootCoord)) return false;

			//4방향 회전 모두 시도. (집은 회전 안함)
			List<int> rotations = new List<int> { 0, 1, 2, 3 };
			rotations.Shuffle();

			foreach (var baseLayout in layouts) {
				foreach (int rotIndex in rotations) {
					//1. 회전된 크기 계산
					//2. 회전된 입구 위치(Local) 계산
					//3. 회전된 입구 방향 계산
					BuildingLayout rotatedLayout = new BuildingLayout {
						Footprint = RotateUtils.RotateSize(baseLayout.Footprint, rotIndex),
						LocalEntrance = RotateUtils.RotatePoint(baseLayout.LocalEntrance, baseLayout.Footprint, rotIndex),
						Driveways = new List<TileDirection> { RotateUtils.RotateDirection(baseLayout.Driveways[0], rotIndex) }
					};

					if (IsValidPlacement(rootCoord, rotatedLayout)) {
						//성공 시, 배치 실행합니다.
						SpawnBuilding(ticket, rootCoord, rotatedLayout, rotIndex);
						return true;
					}
				}
			}

			return false;
		}

		//배치 유효성 검사
		private bool IsValidPlacement(Vector2Int root, BuildingLayout layout) {
			var grid = MapManager.Instance._grid;

			//1. 설치할 부분의 타일이 모두 비어있나요?
			for (int x = 0; x < layout.Footprint.x; x++) {
				for (int y = 0; y < layout.Footprint.y; y++) {
					Vector2Int pos = root + new Vector2Int(x, y);
					if (!grid.TryGetValue(pos, out TileData tile) || !tile.IsBuildable()) return false;
				}
			}

			//2. 입구 앞에 도로를 연결 가능한 상태인가요? (주위에 건물로 둘러 쌓여 설치가 불가능하다면 실패)
			Vector2Int entranceWorld = root + layout.LocalEntrance;
			Vector2Int roadTarget = entranceWorld + TileUtils.GetDirectionVector(layout.Driveways[0]);

			if (grid.TryGetValue(roadTarget, out TileData roadTile)) {
				//비어있거나 도로면 설치 가능합니다. (도로면 바로 이어주면 되니까.)
				return roadTile.IsBuildable() || roadTile.HasAnyRoad || roadTile.type == TileLogicType.Motorway;
			}
			return false;   //맵 밖이면 안됨.
		}

		//--- 실제 생성. (리팩토링 하면서, 집과 목적지의 생성을 통합) ---
		private void SpawnBuilding(ScheduledBuilding ticket, Vector2Int root, BuildingLayout finalLayout, int rotIndex) {
			BuildingBase building;
			GameObject prefab;
			TileLogicType type;

			if(ticket.Type == BuildingType.House) {
				building = new House();
				prefab = BuildingManager.Instance.HousePrefab;
				type = TileLogicType.House;
			} else {
				building = new Destination();
				prefab = BuildingManager.Instance.DestinationPrefab;
				type = TileLogicType.Destination;
			}

			for (int x = 0; x < finalLayout.Footprint.x; x++) {
				for (int y = 0; y < finalLayout.Footprint.y; y++) {
					Vector2Int pos = root + new Vector2Int(x, y);
					if(MapManager.Instance._grid.TryGetValue(pos, out TileData tile)) {
						tile.type = type;
					}
				}
			}

			building.Initialize(ticket.GroupIndex, root, finalLayout);
			UpdateMapData(building);

			Vector3 centerPos = new Vector3(root.x + finalLayout.Footprint.x / 2.0f, 0, root.y + finalLayout.Footprint.y / 2.0f);
			Quaternion rotation = Quaternion.Euler(0, rotIndex * 90f, 0);
			
			if (building is House house) {
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterHouse(house);
				SpawnVehiclesForHouse(house, centerPos, rotation);
			} else if (building is Destination dest) {
				if (DemandProcess.Instance != null)	DemandProcess.Instance.RegisterDestination(dest);
				if (ParkVehicleProcess.Instance != null) ParkVehicleProcess.Instance.RegisterCarpark(dest._CarPark);
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterDestination(dest);
			}

			Instantiate(prefab, centerPos, rotation);	
		}


		//---유틸---
		private void UpdateMapData(BuildingBase building) {
			var grid = MapManager.Instance._grid;
			foreach(var pos in building.OccupiedCoordinates) {
				if(grid.TryGetValue(pos, out TileData tile)) {
					tile.type = (building.Type == BuildingType.House)
								? TileLogicType.House : TileLogicType.Destination;
					tile.Building = building;
				}
			}
		}

		private bool TryPickRandomCoordinate(BuildingType type, out Vector2Int bestCoord) {
			bestCoord = Vector2Int.zero;

			var grid = MapManager.Instance._grid;
			List<Vector2Int> candidates = new List<Vector2Int>();
			float totalWeight = 0f;
			bool isHouse = type == BuildingType.House;

			//음... 그 뭐냐
			//룰렛 휠 알고리즘으로 위치 찾기.
			//1. 가중치가 있는 타일들 넣기.
			foreach (var kvp in grid) {
				TileData tile = kvp.Value;

				if (!tile.IsBuildable()) continue;  //비어있는 땅이 아니면 넘겨.

				float weight = isHouse ? tile.WeightHouseSpawn : tile.WeightDestinationSpawn;
				if (weight <= 0.01f) continue; //가중치가 너무 적어도 넘기기.

				candidates.Add(tile.coordinate);
				totalWeight += weight;
			}

			if (candidates.Count == 0) return false;

			//2. 룰렛 휠 돌리기
			float randomPoint = Random.value * totalWeight;
			float currentSum = 0f;

			foreach (var pos in candidates) {
				TileData c = grid[pos];
				float w = isHouse ? c.WeightHouseSpawn : c.WeightDestinationSpawn;

				currentSum += w;
				if (currentSum >= randomPoint) {
					bestCoord = pos;
					return true;
				}
			}

			//안전장치.
			bestCoord = candidates[0];
			return true;
		}

		private void SpawnVehiclesForHouse(House house, Vector3 pos, Quaternion rot) {
			int vehiclesPerHouse = 2;
			for (int i = 0; i < vehiclesPerHouse; i++) {
				GameObject vehicleObj = Instantiate(BuildingManager.Instance.VehiclePrefab, pos, rot);
				if (vehicleObj.TryGetComponent(out Vehicle vehicle)) {
					if (VehicleMovementProcess.Instance != null) {
						VehicleMovementProcess.Instance.RegisterVehicle(vehicle);
					}
				}
				house.RegisterVehicle(vehicle.Id);
			}
		}
	}
}
