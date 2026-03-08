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
			if (_houseCheckTimer < 3.0f) return;
			_houseCheckTimer = 0f;

			var activeDestinations = BuildingManager.Instance.ActiveDestinations;
			var activeHouses = BuildingManager.Instance.ActiveHouses;

			Dictionary<int, int> minimumHousesPerGroup = new Dictionary<int, int>();
			foreach (var dest in activeDestinations) {
				if (!dest.isActive) continue;
				if (!minimumHousesPerGroup.ContainsKey(dest.GroupIndex)) minimumHousesPerGroup[dest.GroupIndex] = 0;
				minimumHousesPerGroup[dest.GroupIndex]++;
				if (dest.TotalDemand >= 4) minimumHousesPerGroup[dest.GroupIndex]++;
			}

			List<int> activeGroups = new List<int>(minimumHousesPerGroup.Keys);
			foreach (int group in activeGroups) minimumHousesPerGroup[group] += 1;

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

			foreach (var kvp in minimumHousesPerGroup) {
				int color = kvp.Key;
				int requiredDemand = kvp.Value;
				int currentSupply = supplyPerColor.ContainsKey(color) ? supplyPerColor[color] : 0;

				if (currentSupply < requiredDemand) {
					float randomDelay = Random.Range(1.0f, 3.0f);
					BuildingManager.Instance.ScheduleBuilding(BuildingType.House, color, randomDelay);
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
			Vector2Int bottomLeft = entrance - layout.LocalEntrance;
			for (int x = 0; x < layout.Footprint.x; x++) {
				for (int y = 0; y < layout.Footprint.y; y++) {
					Vector2Int pos = bottomLeft + new Vector2Int(x, y);
					if (!MapManager.Instance.IsInPlayableArea(pos)) return false;
					if (!grid.TryGetValue(pos, out TileData tile) || !tile.IsBuildable()) return false;
				}
			}
			Vector2Int roadTarget = entrance + TileUtils.GetDirectionVector(layout.Driveways[0]);
			if (!MapManager.Instance.IsInPlayableArea(roadTarget)) return false;
			if (grid.TryGetValue(roadTarget, out TileData roadTile)) {
				return roadTile.IsBuildable() || roadTile.HasAnyRoad || roadTile.type == TileLogicType.Motorway;
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
