using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Managers;

	public class LaneUpdateProcess : MonoBehaviour, ISimulationProcess {
		private int _lastProcessedFrame = -1;

		private void Start() {
			if (Managers.SimulationManager.Instance != null) {
				Managers.SimulationManager.Instance.RegisterProcess(this);
			}
		}

		private void OnDestroy() {
			if (Managers.SimulationManager.Instance != null) {
				Managers.SimulationManager.Instance.RemoveProcess(this);
			}
		}

		public void Tick(float dt) {
			if (CityModel.LatestLaneChangeFrame == _lastProcessedFrame) return;
			_lastProcessedFrame = CityModel.LatestLaneChangeFrame;

			if (VehicleMovementProcess.Instance == null || RoadNetworkManager.Instance == null) return;

			HashSet<int> affectedVehicleIds = new HashSet<int>();

			//1. [원작 방식 / 최적화] "타일에 있는 VehicleId를 찾아서"
			//AllLanes를 전부 순회하지 않고, 변경된 타일(ChangedNodes)에 등록된 Lane들만 확인합니다.
			foreach (Vector2Int coord in CityModel.ChangedNodes) {
				if (MapManager.Instance._grid.TryGetValue(coord, out TileData tile)) {
					//해당 타일의 8방향 Lane에 예약/주행 중인 차량 ID 수집
					for (int i = 0; i < 8; i++) {
						Lane lane = tile.Lanes[i];
						if (lane != null) {
							foreach (int vId in lane.InboundVehicles) affectedVehicleIds.Add(vId);
							foreach (int vId in lane.VehiclesOnLane) affectedVehicleIds.Add(vId);
						}
					}
				}
			}

			//2. "Mothballed 도로에서만 작용합니다"
			//삭제 대기 중인 도로들에 대해서도 해당 도로를 밟고 있거나 진입 예정인 차량들을 수집합니다.
			foreach (Lane mothballedLane in RoadNetworkManager.Instance.MothballedLanes) {
				foreach (int vId in mothballedLane.InboundVehicles) affectedVehicleIds.Add(vId);
				foreach (int vId in mothballedLane.VehiclesOnLane) affectedVehicleIds.Add(vId);
			}

			//3. 찾아낸 차량들에게만 핀포인트로 재탐색 요청
			foreach (int vId in affectedVehicleIds) {
				Vehicle v = VehicleMovementProcess.Instance.GetVehicle(vId);
				if (v != null) {
					v.RequestPathfind();
				}
			}

			//Removed CityModel.ChangedNodes.Clear(); to allow RoadNetworkManager.LateUpdate to render them first.
		}
	}
}
