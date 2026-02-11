using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Simulation.Managers {
	using Core.Data;
	using Core.Systems;
	using Core.Simulation.Buildings;
	using Core.Simulation.Vehicles;
	using Core.Simulation.Roads;
	using Core.Simulation.Navigation;

	public class BuildingManager : MonoBehaviour {
		//partial 사용 가능하다는데, 한 클래스를 보기 편하게 구분시키는건듯. 
		//지금이야 혼자하니까 그렇다 치고, 협업할때 사용할수도. 근데 진짜 명확하게 구조를 나눠야할듯 (기능이 겹칠수도 있음)

		public static BuildingManager Instance;

		private List<House> _houses = new List<House>();
		private List<Destination> _destinations = new List<Destination>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Start() {
			//게임 시작 시, 초기 건물 배치.
		}

		private void Update() {
			AssignVehiclesToRequests();
		}


		//--- 건물 생성 ---
		public void SpawnHouse(Vector2Int coord, ColorType color) {
			if (!MapBootstrapper.Grid.TryGetValue(coord, out CellData cell)) return;
			if (!cell.IsBuildable) return;

			House newHouse = new House(coord, color);
			newHouse.InitializeFootprint();

			//입구 설정
			newHouse.EntranceCoordinate = FindBestEntrance(newHouse);

			//MapBootsrapper 데이터 갱신.
			UpdateCellData(newHouse);

			Vehicle v1 = new Vehicle();
			Vehicle v2 = new Vehicle();
			newHouse.OwnedVehicleIds.Add(v1.Id);
			newHouse.OwnedVehicleIds.Add(v2.Id);

			_houses.Add(newHouse);
		}
		public void SpawnDestination(Vector2Int coord, ColorType color) {
			Destination newDest = new Destination(coord, color);
			newDest.InitializeFootprint();
			newDest.EntranceCoordinate = FindBestEntrance(newDest);
			UpdateCellData(newDest);
			_destinations.Add(newDest);
		}

		//--- 구요 공급 매칭 ---
		private void AssignVehiclesToRequests() {
			foreach(var dest in _destinations) {
				if(dest.CurrentPin > 0 && dest.HasParkingSpace()) {
					TryDispatchVehicleTo(dest);
				}
			}
		}
		private void TryDispatchVehicleTo(Destination dest) {
			House house = FindBestHouseFor(dest);
			if (house == null) return;

			var path = Pathfinder.FindPath(
				house.EntranceCoordinate,
				dest.EntranceCoordinate,
				RoadNetworkManager.Instance.Grid
			);
			if (path == null || path.Count == 0) return;

			var returnPath = Pathfinder.FindPath(
				dest.EntranceCoordinate,
				house.EntranceCoordinate,
				RoadNetworkManager.Instance.Grid
			);
			if (returnPath == null || returnPath.Count == 0) return; //복귀 경로가 없을 가능성은 낮지만, 아무튼 예방 차원.

			//여기까지 왔으면, 사실상 배차 된거임~
			if(house.TryDispatchVehicle(out int vehicleId)) {
				Vehicle vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);
				if (vehicle != null) {
					vehicle.Dispatch(path, returnPath);
					dest.IncomingVehicles++;
					//목적지 주차장 예약 (도착 전이지만 미리 자리 확보 or 도착 후 확보는 기획 선택)
					//원작은 보통 도착 후 처리하지만, 여기선 배차 성공 시 핀이 바로 반응할지 여부에 따라 다름.
					//일단 여기서는 Dispatch만 수행.

					//디버그 : 배차 성공.
				}
			}
		}
		private House FindBestHouseFor(Destination dest) {
			House bestHouse = null;
			float minDistance = float.MaxValue;

			foreach(var house in _houses) {
				if (house.Color != dest.Color) continue;
				if (house.AvailableVehicles <= 0) continue;

				//임시
				float dist = Vector2Int.Distance(house.EntranceCoordinate, dest.EntranceCoordinate);
				if(dist < minDistance) {
					minDistance = dist;
					bestHouse = house;
				}
			}
			return bestHouse;
		}

		public void NotifyVehicleReturned(int vehicleId) {
			foreach(var house in _houses) {
				if(house.OwnedVehicleIds.Contains(vehicleId)) {
					house.VehicleReturned(vehicleId);
					return;
				}
			}
		}
		public void NotifyVehicleDeparted(int vehicleId) {
			foreach(var dest in _destinations) {
				if(dest.OccupiedSlots.Contains(vehicleId)) {
					dest.VehicleDeparted(vehicleId);
					dest.VehicleArrived(vehicleId);
					return;
				}
			}
		}

		//--- 유틸 ---
		private Vector2Int FindBestEntrance(Building b) {
			return b.RootCoordinate + Vector2Int.right;
		}

		private void UpdateCellData(Building b) {
			foreach (var coord in b.OccupiedCoordinates) {
				if (MapBootstrapper.Grid.TryGetValue(coord, out CellData cell)) {
					cell.Type = TileLogicType.Obstacle;
				}
			}

			if(MapBootstrapper.Grid.TryGetValue(b.EntranceCoordinate, out CellData entracneCell)) {
				entracneCell.Type = TileLogicType.Entrance;
			}
		}
	}
}


