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
						ticket.SpawnTime = currentTime + 5.0f; // 다음 시도 시간도 시뮬레이션 기반으로 갱신
						scheduleList.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
					}
				}
			}
		}

		private float _houseCheckTimer = 0f;
		private void ProcessDynamicHouseSpawning(float dt) {
			_houseCheckTimer += dt;
			if (_houseCheckTimer < 3.0f) return; // 3초마다 수요/공급 점검
			_houseCheckTimer = 0f;

			var activeDestinations = BuildingManager.Instance.ActiveDestinations;
			var activeHouses = BuildingManager.Instance.ActiveHouses;

			// [수정] 원작의 'ScheduleHousesFromCityHouseCurve' 로직 100% 동일 적용
			// 목적지 1개당 기본 1개의 집이 필요하고, 그룹(색상)이 활성화되면 추가로 1개의 보너스 집(AdditionalHousesPerGroup)을 요구합니다.
			Dictionary<int, int> minimumHousesPerGroup = new Dictionary<int, int>();
			
			foreach (var dest in activeDestinations) {
				if (!dest.isActive) continue;

				if (!minimumHousesPerGroup.ContainsKey(dest.GroupIndex)) {
					minimumHousesPerGroup[dest.GroupIndex] = 0;
				}
				
				// 목적지 1개당 집 1개 기본 할당
				minimumHousesPerGroup[dest.GroupIndex]++;

				// 핀이 심하게 밀리면 추가 집을 동적으로 요구 (원작의 !IsSupplySufficient 흉내)
				if (dest.TotalDemand >= 4) {
					minimumHousesPerGroup[dest.GroupIndex]++;
				}
			}

			// 그룹별 추가 기본 집 할당 (원작의 AdditionalHousesPerGroup 역할)
			// 그룹이 맵에 존재하기만 하면 보너스 집을 더 얹어줍니다.
			List<int> activeGroups = new List<int>(minimumHousesPerGroup.Keys);
			foreach (int group in activeGroups) {
				minimumHousesPerGroup[group] += 1; // 기본적으로 여유분 1개 추가
			}

			// 현재 색상별 공급(Supply) 계산
			Dictionary<int, int> supplyPerColor = new Dictionary<int, int>();
			foreach (var house in activeHouses) {
				if (!supplyPerColor.ContainsKey(house.GroupIndex)) supplyPerColor[house.GroupIndex] = 0;
				supplyPerColor[house.GroupIndex]++;
			}

			// 아직 스폰되지 않고 대기 중인 예약표(Ticket)도 미래의 공급으로 간주
			foreach (var ticket in BuildingManager.Instance.ScheduleList) {
				if (ticket.Type == BuildingType.House) {
					if (!supplyPerColor.ContainsKey(ticket.GroupIndex)) supplyPerColor[ticket.GroupIndex] = 0;
					supplyPerColor[ticket.GroupIndex]++;
				}
			}

			// 수요 대비 공급이 부족하다면 집을 동적으로 예약
			foreach (var kvp in minimumHousesPerGroup) {
				int color = kvp.Key;
				int requiredDemand = kvp.Value;
				int currentSupply = supplyPerColor.ContainsKey(color) ? supplyPerColor[color] : 0;

				if (currentSupply < requiredDemand) {
					// 1~2초 내에 긴급 스폰되도록 스케줄표 발행
					float randomDelay = Random.Range(1.0f, 3.0f);
					BuildingManager.Instance.ScheduleBuilding(BuildingType.House, color, randomDelay);
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
				BuildingManager.Instance.ActiveHouses.Add(house); // 동적 수요 계산을 위한 등록
			} else if (building is Destination dest) {
				if (DemandProcess.Instance != null)	DemandProcess.Instance.RegisterDestination(dest);
				if (ParkVehicleProcess.Instance != null) ParkVehicleProcess.Instance.RegisterCarpark(dest._CarPark);
				if (DispatchProcess.Instance != null) DispatchProcess.Instance.RegisterDestination(dest);
				BuildingManager.Instance.ActiveDestinations.Add(dest); // 동적 수요 계산을 위한 등록
			}

			GameObject instance = Instantiate(prefab, centerPos, rotation);

			// 뷰 업데이트 (형태, 방향, 색상 전달)
			if(instance.TryGetComponent(out HouseView houseView)) {
				houseView.UpdateColor(ticket.GroupIndex);
			}

			if (instance.TryGetComponent(out DestinationView destView)) {
				bool isHorizontal = finalLayout.Footprint.x > finalLayout.Footprint.y;
				TileDirection doorDir = finalLayout.Driveways[0];
				bool isPositive = (doorDir == TileDirection.South || doorDir == TileDirection.East);

				destView.Initialize(building as Destination); // 모델 초기화 연결
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
					vehicle.SetHome(house.OriginCoordinate); // [추가] 집 위치 영구 설정
					if (VehicleMovementProcess.Instance != null) {
						VehicleMovementProcess.Instance.RegisterVehicle(vehicle);
					}
				}
				house.RegisterVehicle(vehicle.Id);
				if(vehicleObj.TryGetComponent(out VehicleView vehicleView)) {
					vehicleView.Initialize(vehicle); // [추가] 뷰에 모델 연결
					vehicleView.UpdateColor(house.GroupIndex);
				}
			}
		}
	}
}
