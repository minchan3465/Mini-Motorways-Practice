using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Navigation;
	using Motorways.Managers;

	public class VehiclePathfindingProcess : MonoBehaviour, ISimulationProcess {

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		private void OnDestroy() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RemoveProcess(this);
			}
		}

		public void Tick(float dt) {
			if (VehicleMovementProcess.Instance == null) return;

			List<Vehicle> allVehicles = VehicleMovementProcess.Instance._activeVehicle;
			int latestLaneChange = CityModel.LatestLaneChangeFrame;

			foreach (Vehicle v in allVehicles) {
				// LaneUpdateProcess에 의해 NeedsPathfind가 true가 된 차량만 재탐색
				if (v.NeedsPathfind) {
					ProcessPathfindForVehicle(v);
					v.LatestAttemptedPathfindFrame = latestLaneChange; 
				}
			}
		}

		private bool TryStitchPath(List<Lane> path, Vector2Int targetNode) {
			if (path == null) return false;
			if (path.Count == 0 && targetNode != null) return true;

			Lane lastLane = path[path.Count - 1];
			if (lastLane.EndNode == targetNode) return true;

			if (MapManager.Instance._grid.TryGetValue(lastLane.EndNode, out TileData roadTile)) {
				foreach (var lane in roadTile.Lanes) {
					if (lane != null && lane.EndNode == targetNode) {
						path.Add(lane);
						return true;
					}
				}
			}
			return false;
		}

		private void ProcessPathfindForVehicle(Vehicle v) {
			// [수정] 원작 방식의 단계별 경로 탐색 및 예외 처리
			
			// 1. Ready 상태: 최초 배차 또는 집으로 귀환 결정 직후
			if (v.State == VehicleState.Ready) {
				Vector2Int start = v.IsReturning ? v.DestNode : v.HomeNode;
				Vector2Int target = v.IsReturning ? v.HomeNode : v.DestNode;

				// 일반 탐색
				List<Lane> path = Pathfinder.FindPath(start, target, allowMothballed: false);
				// 실패 시 Mothballed(삭제 중) 도로를 포함하여 재탐색 (Last Resort)
				if (path == null) path = Pathfinder.FindPath(start, target, allowMothballed: true);

				if (path == null && start == target) path = new List<Lane>();

				if (TryStitchPath(path, target)) {
					v.AssignPath(path);
					v.State = v.IsReturning ? VehicleState.Returning : VehicleState.Driving;

					if (!v.IsReturning) {
						List<Lane> toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode, allowMothballed: false);
						if (toHome == null) toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode, allowMothballed: true);
						if (toHome == null && v.DestNode == v.HomeNode) toHome = new List<Lane>();
						if (TryStitchPath(toHome, v.HomeNode)) v.AssignReturnPath(toHome);
					}
				} else {
					// 경로가 아예 없는 경우 원작처럼 집으로 리셋하여 고립 방지
					v.State = VehicleState.Ready;
					v.IsReturning = false;
					if (v.HomeObject != null) v.HomeObject.OnVehicleArrived(v.Id);
				}
			}
			// 2. Arrived 상태: 주차 중 도로 변화에 대응
			else if (v.State == VehicleState.Arrived) {
				List<Lane> newReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode, allowMothballed: false);
				if (newReturnPath == null) newReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode, allowMothballed: true);
				
				if (newReturnPath == null && v.DestNode == v.HomeNode) newReturnPath = new List<Lane>();
				
				if (TryStitchPath(newReturnPath, v.HomeNode)) {
					v.AssignReturnPath(newReturnPath);
				}
				v.NeedsPathfind = false;
			}
			// 3. 주행 중 재탐색 (실시간 도로 변화 대응 핵심)
			else {
				Lane currLane = v.GetCurrentLane();
				if (currLane != null) {
					// 현재 주행 중인 도로가 삭제 중(Mothballed)이라면 기존 경로를 유지하여 U턴 방지
					if (currLane.State == RoadState.Mothballed) {
						v.NeedsPathfind = false;
						return;
					}

					Vector2Int target = v.IsReturning ? v.HomeNode : v.DestNode;
					
					// 일반 탐색
					List<Lane> remainingPath = Pathfinder.FindPath(currLane.EndNode, target, allowMothballed: false, restrictUTurnLane: currLane);
					// 실패 시 Mothballed 포함 탐색
					if (remainingPath == null) remainingPath = Pathfinder.FindPath(currLane.EndNode, target, allowMothballed: true, restrictUTurnLane: currLane);

					if (remainingPath == null && currLane.EndNode == target) remainingPath = new List<Lane>();

					if (TryStitchPath(remainingPath, target)) {
						v.AssignPath(remainingPath);
					} else {
						// 주행 중 경로가 완전히 끊긴 경우 (Mothballed 도로마저 삭제된 경우)
						// 원작은 이런 경우 차량을 인근 유효 도로로 텔레포트시키거나 집으로 리셋합니다.
						v.ClearAllReservations();
						if (v.HomeObject != null) v.HomeObject.OnVehicleArrived(v.Id);
						// 차량 오브젝트의 물리적 위치도 집으로 이동시켜야 할 수 있음 (VehicleMovementProcess에서 처리 권장)
					}
				}
				v.NeedsPathfind = false;
			}
		}
	}
}
