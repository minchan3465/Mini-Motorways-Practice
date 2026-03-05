using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Managers;
	using Motorways.Models;
	using Motorways.Views;
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

			// 후보 좌표 선택 (이 좌표는 '입구 타일'의 그리드 좌표입니다)
			if (!TryPickRandomCoordinate(ticket.Type, out Vector2Int entranceCoord)) return false;

			if (isHouse) {
				BuildingLayout baseLayout = layouts[0];
				List<int> rotations = new List<int> { 0, 1, 2, 3 };
				rotations.Shuffle();

				foreach (int rotIndex in rotations) {
					BuildingLayout rotatedLayout = new BuildingLayout {
						Footprint = baseLayout.Footprint,
						LocalEntrance = baseLayout.LocalEntrance,
						Driveways = new List<TileDirection> { RotateUtils.RotateDirection(baseLayout.Driveways[0], rotIndex) }
					};

					if (IsValidPlacement(entranceCoord, rotatedLayout)) {
						SpawnBuilding(ticket, entranceCoord, rotatedLayout, rotIndex);
						return true;
					}
				}
			} else {
				// 목적지: 정의된 4개 패턴 중 적절한 위치 탐색
				List<BuildingLayout> shuffledLayouts = new List<BuildingLayout>(layouts);
				shuffledLayouts.Shuffle();

				foreach (var layout in shuffledLayouts) {
					if (IsValidPlacement(entranceCoord, layout)) {
						SpawnBuilding(ticket, entranceCoord, layout, 0);
						return true;
					}
				}
			}

			return false;
		}
private bool IsValidPlacement(Vector2Int entrance, BuildingLayout layout) {
	var grid = MapManager.Instance._grid;

	// 1. 입구 좌표로부터 건물의 하단 좌측 시작점 역산
	Vector2Int bottomLeft = entrance - layout.LocalEntrance;

	// 2. 건물이 차지할 모든 타일이 맵 범위(PlayableArea) 내에 있고 건설 가능한지 검사
	for (int x = 0; x < layout.Footprint.x; x++) {
		for (int y = 0; y < layout.Footprint.y; y++) {
			Vector2Int pos = bottomLeft + new Vector2Int(x, y);

			// 맵 범위를 벗어나거나, 타일 데이터가 없거나, 건설 불가능한 지형이면 탈락
			if (!MapManager.Instance.IsInPlayableArea(pos)) return false;
			if (!grid.TryGetValue(pos, out TileData tile) || !tile.IsBuildable()) return false;
		}
	}

	// 3. 입구 바로 앞 방향에 도로가 생길 공간이 있는지 검사 (역시 맵 범위 내여야 함)
	Vector2Int roadTarget = entrance + TileUtils.GetDirectionVector(layout.Driveways[0]);
	if (!MapManager.Instance.IsInPlayableArea(roadTarget)) return false;

	if (grid.TryGetValue(roadTarget, out TileData roadTile)) {
		return roadTile.IsBuildable() || roadTile.HasAnyRoad || roadTile.type == TileLogicType.Motorway;
	}
	return false;
}

		private void SpawnBuilding(ScheduledBuilding ticket, Vector2Int entrance, BuildingLayout finalLayout, int rotIndex) {
			BuildingBase building = (ticket.Type == BuildingType.House) ? new House() : new Destination();
			GameObject prefab = (ticket.Type == BuildingType.House) ? BuildingManager.Instance.HousePrefab : BuildingManager.Instance.DestinationPrefab;

			// 데이터 초기화 (입구 좌표 전달)
			building.Initialize(ticket.GroupIndex, entrance, finalLayout);
			
			// 그리드 맵 데이터 점유 상태 업데이트
			var grid = MapManager.Instance._grid;
			foreach(var pos in building.OccupiedCoordinates) {
				if(grid.TryGetValue(pos, out TileData tile)) {
					tile.type = (building.Type == BuildingType.House) ? TileLogicType.House : TileLogicType.Destination;
					tile.Building = building;
				}
			}

			// [핵심] 시각적 중심점 계산 (타일 스케일 2배수 적용)
			// bottomLeft 좌표를 기준으로 Footprint의 절반을 더해 정확한 '도형의 중심' 월드 좌표 산출
			Vector2Int bottomLeft = entrance - finalLayout.LocalEntrance;
			Vector3 centerPos = new Vector3(
				(bottomLeft.x + (finalLayout.Footprint.x * 0.5f)) * MapSettings.TILE_SIZE, 
				0, 
				(bottomLeft.y + (finalLayout.Footprint.y * 0.5f)) * MapSettings.TILE_SIZE
			);
			
			// House는 회전 X, Destination은 메쉬 원본 방향 유지
			Quaternion rotation;
			if (ticket.Type == BuildingType.House) {
				rotation = Quaternion.identity;
			} else {
				// 가로형(3x2)은 0도, 세로형(2x3)은 90도 회전
				bool isVertical = finalLayout.Footprint.y > finalLayout.Footprint.x;
				rotation = isVertical ? Quaternion.Euler(0, -90f, 0) : Quaternion.identity;
			}
			
			if (building is House house) {
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterHouse(house);
				SpawnVehiclesForHouse(house, centerPos, rotation);
			} else if (building is Destination dest) {
				if (DemandProcess.Instance != null)	DemandProcess.Instance.RegisterDestination(dest);
				if (ParkVehicleProcess.Instance != null) ParkVehicleProcess.Instance.RegisterCarpark(dest._CarPark);
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterDestination(dest);
			}

			GameObject instance = Instantiate(prefab, centerPos, rotation);

			// 뷰 업데이트 (형태, 방향, 색상 전달)
			if(instance.TryGetComponent(out HouseView houseView)) {
				houseView.UpdateColor(ticket.GroupIndex);
			}

			if (instance.TryGetComponent(out DestinationView destView)) {
				bool isHorizontal = finalLayout.Footprint.x > finalLayout.Footprint.y;
				TileDirection doorDir = finalLayout.Driveways[0];
				bool isPositive = (doorDir == TileDirection.South || doorDir == TileDirection.West);
				
				destView.UpdateVisuals(isHorizontal, isPositive);
				destView.UpdateColor(ticket.GroupIndex);
			}
		}

		private bool TryPickRandomCoordinate(BuildingType type, out Vector2Int bestCoord) {
			bestCoord = Vector2Int.zero;
			var grid = MapManager.Instance._grid;
			List<Vector2Int> candidates = new List<Vector2Int>();
			float totalWeight = 0f;
			bool isHouse = type == BuildingType.House;

			foreach (var kvp in grid) {
				TileData tile = kvp.Value;
				if (!tile.IsBuildable()) continue;

				// 현재 맵 확장 범위(PlayableArea) 내에 있는 타일만 스폰 후보로 선정
				if (!MapManager.Instance.IsInPlayableArea(tile.coordinate)) continue;

				float weight = isHouse ? tile.WeightHouseSpawn : tile.WeightDestinationSpawn;
				if (weight <= 0.01f) continue;

				candidates.Add(tile.coordinate);
				totalWeight += weight;
			}

			if (candidates.Count == 0) return false;

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
				if(vehicleObj.TryGetComponent(out VehicleView vehicleView)) {
					vehicleView.UpdateColor(house.GroupIndex);
				}
			}
		}
	}
}
