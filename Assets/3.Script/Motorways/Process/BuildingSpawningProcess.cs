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
			ProcessScheduledBuildings();
			ProcessDynamicHouseSpawning(dt);
		}

		private void ProcessScheduledBuildings() {
			var scheduleList = BuildingManager.Instance.ScheduleList;

			if (scheduleList.Count > 0) {
				ScheduledBuilding ticket = scheduleList[0];

				float currentTime = 0f;
				if (ClockProcess.Instance != null && ClockProcess.Instance.Model != null) {
					currentTime = ClockProcess.Instance.Model.ExpansionTime;
				} else {
					currentTime = Time.time;
				}

				if (ticket.SpawnTime <= currentTime) {
					if (TrySpawnBuilding(ticket)) {
						scheduleList.RemoveAt(0);
					} else {
						ticket.SpawnAttempts++;
						ticket.SpawnTime = currentTime + 5.0f;
						scheduleList.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
					}
				}
			}
		}

		private float _houseCheckTimer = 0f;
		private void ProcessDynamicHouseSpawning(float dt) {
			_houseCheckTimer += dt;
			if (_houseCheckTimer < 1.0f) return; // [최적화] 집 체크 주기를 3초에서 1초로 줄여 더 민감하게 반응
			_houseCheckTimer = 0f;

			var activeDestinations = BuildingManager.Instance.ActiveDestinations;
			var activeHouses = BuildingManager.Instance.ActiveHouses;

			// 1. 현재 게임의 난이도 배율 (핀 생성 가속도) 가져오기
			float spawnScale = 1.0f;
			if (DemandProcess.Instance != null) {
				spawnScale = DemandProcess.Instance.CurrentSpawnScale;
			}

			Dictionary<int, float> demandPerGroup = new Dictionary<int, float>();
			
			// 2. 목적지별 '수요량(Demand Power)' 계산
			foreach (var dest in activeDestinations) {
				if (!dest.isActive) continue;
				if (!demandPerGroup.ContainsKey(dest.GroupIndex)) demandPerGroup[dest.GroupIndex] = 0f;

				// [수정: 난이도 조절] 집이 너무 많은 것을 방지하기 위해 기본 요구량을 1.5로 낮춤 (원래 2.0)
				float baseDemand = 1.5f; 
				
				// 난이도가 올라가 핀 생성 속도(spawnScale)가 빨라지면, 요구하는 집의 수도 그에 비례해 증가함
				// [수정: 난이도 조절] spawnScale의 영향을 절반으로 줄임
				float scaledDemand = baseDemand * (1.0f + (spawnScale - 1.0f) * 0.5f);

				// 목적지에 핀이 쌓이기 시작했다면 (도로가 막혔거나 거리가 멀어 공급이 딸리는 상태) 긴급 추가 수요 발생
				// [수정: 난이도 조절] 긴급 배율도 낮춤
				if (dest.TotalDemand >= 4) {
					scaledDemand *= 1.3f; 
				} else if (dest.TotalDemand >= 2) {
					scaledDemand *= 1.1f;
				}

				demandPerGroup[dest.GroupIndex] += scaledDemand;
			}

			// 3. 현재 맵과 스케줄에 있는 '공급량(Supply Power)' 계산
			Dictionary<int, int> supplyPerColor = new Dictionary<int, int>();
			foreach (var house in activeHouses) {
				if (!supplyPerColor.ContainsKey(house.GroupIndex)) supplyPerColor[house.GroupIndex] = 0;
				supplyPerColor[house.GroupIndex]++;
			}

			foreach (var ticket in BuildingManager.Instance.ScheduleList) {
				if (ticket.Type == BuildingType.House) {
					if (!supplyPerColor.ContainsKey(ticket.GroupIndex)) supplyPerColor[ticket.GroupIndex] = 0;
					supplyPerColor[ticket.GroupIndex]++;
				}
			}

			// 4. 수요(Demand)가 공급(Supply)을 앞지르면 즉시 집 스폰 예약
			foreach (var kvp in demandPerGroup) {
				int color = kvp.Key;
				// 요구량은 올림(Ceil) 처리하여 소수점 수요라도 무조건 집 한 채를 더 주도록 보수적으로 잡음
				int requiredHouses = Mathf.CeilToInt(kvp.Value); 
				int currentHouses = supplyPerColor.ContainsKey(color) ? supplyPerColor[color] : 0;

				if (currentHouses < requiredHouses) {
					// 얼마나 부족한지 계산
					int shortage = requiredHouses - currentHouses;

					// 부족한 만큼 집을 한 번에 스케줄링 (단, 한 프레임에 너무 많이 쏟아지지 않게 최대 2채까지만)
					int spawnCount = Mathf.Min(shortage, 2); 

					for (int i = 0; i < spawnCount; i++) {
						// 핀이 밀려서 다급한 상황(shortage가 큼)일수록 딜레이를 0에 가깝게 줄임
						float randomDelay = Random.Range(0.1f, Mathf.Max(0.5f, 3.0f / shortage));
						BuildingManager.Instance.ScheduleBuilding(BuildingType.House, color, randomDelay);
						
						// 스케줄에 넣었으니 가상 공급량 증가
						if (!supplyPerColor.ContainsKey(color)) supplyPerColor[color] = 0;
						supplyPerColor[color]++;
					}
				}
			}
		}

		private bool TrySpawnBuilding(ScheduledBuilding ticket) {
			bool isHouse = ticket.Type == BuildingType.House;
			var layouts = isHouse ? BuildingManager.Instance.HouseLayouts : BuildingManager.Instance.DestinationLayouts;

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
			var playableArea = MapManager.Instance.PlayableArea;

			// [수정: 맵 가장자리 갇힘 방지] 건물이 맵 경계에서 1칸 안쪽에만 생성되도록 축소된 범위 사용
			RectInt innerPlayableArea = new RectInt(
				playableArea.x + 1, 
				playableArea.y + 1, 
				playableArea.width - 2, 
				playableArea.height - 2
			);

			// 입구 타일 지형 검사: 물 타일에는 설치 불가
			if (grid.TryGetValue(entrance, out TileData entranceTile)) {
				if (entranceTile.type == TileLogicType.Water) return false;
			}

			Vector2Int bottomLeft = entrance - layout.LocalEntrance;
			for (int x = 0; x < layout.Footprint.x; x++) {
				for (int y = 0; y < layout.Footprint.y; y++) {
					Vector2Int pos = bottomLeft + new Vector2Int(x, y);
					
					// 건물의 모든 칸이 innerPlayableArea 안에 있어야 함
					if (!innerPlayableArea.Contains(pos)) return false;
					
					if (!grid.TryGetValue(pos, out TileData tile) || !tile.IsBuildable()) return false;
				}
			}

			// [수정: 입구 충돌 방지] 도로 연결 타일(roadTarget) 검사
			Vector2Int roadTarget = entrance + TileUtils.GetDirectionVector(layout.Driveways[0]);
			
			// 입구 앞 도로는 플레이어 영역(playableArea) 안이면 됨 (가장자리에 닿아도 무방)
			if (!playableArea.Contains(roadTarget)) return false;

			if (grid.TryGetValue(roadTarget, out TileData roadTile)) {
				// 건물이 이미 있으면 무조건 불가 (House, Destination 등)
				if (roadTile.type == TileLogicType.House || roadTile.type == TileLogicType.Destination || roadTile.Building != null) return false;

				// 물이나 산 등은 제외하고, 건설 가능(Empty)하거나 이미 도로가 있는 타일이어야 함
				bool isAllowedType = (roadTile.type == TileLogicType.Empty || roadTile.type == TileLogicType.None);
				return (isAllowedType && roadTile.IsBuildable()) || roadTile.HasAnyRoad || roadTile.type == TileLogicType.Motorway;
			}
			return false;
		}

		private void SpawnBuilding(ScheduledBuilding ticket, Vector2Int entrance, BuildingLayout finalLayout, int rotIndex) {
			BuildingBase building = (ticket.Type == BuildingType.House) ? new House() : new Destination();
			building.Initialize(ticket.GroupIndex, entrance, finalLayout);
			
			var grid = MapManager.Instance._grid;
			foreach(var pos in building.OccupiedCoordinates) {
				if(grid.TryGetValue(pos, out TileData tile)) {
					tile.type = (building.Type == BuildingType.House) ? TileLogicType.House : TileLogicType.Destination;
					tile.Building = building;
				}
			}

			Vector2Int bottomLeft = entrance - finalLayout.LocalEntrance;
			Vector3 centerPos = new Vector3(
				(bottomLeft.x + (finalLayout.Footprint.x * 0.5f)) * MapSettings.TILE_SIZE, 
				0, 
				(bottomLeft.y + (finalLayout.Footprint.y * 0.5f)) * MapSettings.TILE_SIZE
			);
			
			Quaternion rotation;
			if (ticket.Type == BuildingType.House) {
				rotation = Quaternion.identity;
			} else {
				bool isVertical = finalLayout.Footprint.y > finalLayout.Footprint.x;
				rotation = isVertical ? Quaternion.Euler(0, -90f, 0) : Quaternion.identity;
			}
			
			if (building is House house) {
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterHouse(house);
				SpawnVehiclesForHouse(house, centerPos, rotation);
				BuildingManager.Instance.ActiveHouses.Add(house);
			} else if (building is Destination dest) {
				if (DemandProcess.Instance != null)	DemandProcess.Instance.RegisterDestination(dest);
				//if (ParkVehicleProcess.Instance != null) ParkVehicleProcess.Instance.RegisterCarpark(dest._CarPark);
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterDestination(dest);
				BuildingManager.Instance.ActiveDestinations.Add(dest);
			}

			GameObject instance = Instantiate((ticket.Type == BuildingType.House) ? BuildingManager.Instance.HousePrefab : BuildingManager.Instance.DestinationPrefab, centerPos, rotation);

			if(instance.TryGetComponent(out HouseView houseView)) houseView.UpdateColor(ticket.GroupIndex);
			if (instance.TryGetComponent(out DestinationView destView)) {
				bool isHorizontal = finalLayout.Footprint.x > finalLayout.Footprint.y;
				TileDirection doorDir = finalLayout.Driveways[0];
				bool isPositive = (doorDir == TileDirection.South || doorDir == TileDirection.East);
				destView.Initialize(building as Destination); 
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
					vehicle.SetHome(house); 
					if (VehicleMovementProcess.Instance != null) VehicleMovementProcess.Instance.RegisterVehicle(vehicle);
					if (vehicleObj.TryGetComponent(out VehicleView vehicleView)) {
						vehicleView.Initialize(vehicle);
						vehicleView.UpdateColor(house.GroupIndex);
					}
				}
				house.RegisterVehicle(vehicle.Id);
			}
		}
	}
}
