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
				// [사용자님 원본 로직] 도로가 변했으면 즉시 재탐색 예약
				if (v.LatestAttemptedPathfindFrame < latestLaneChange) {
					v.RequestPathfind();
				}

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
			// [복구] 사용자님의 정교한 상태별 경로 갱신 로직
			
			// 1. Ready 상태: 최초 배차된 직후 길찾기
			if (v.State == VehicleState.Ready) {
				Vector2Int start = v.IsReturning ? v.DestNode : v.HomeNode;
				Vector2Int target = v.IsReturning ? v.HomeNode : v.DestNode;

				List<Lane> path = Pathfinder.FindPath(start, target);
				if (path == null && start == target) path = new List<Lane>();

				if (TryStitchPath(path, target)) {
					v.AssignPath(path);
					v.State = v.IsReturning ? VehicleState.Returning : VehicleState.Driving;

					// 목적지로 출발할 때, 고립 방지를 위해 귀환 경로도 미리 예약
					if (!v.IsReturning) {
						List<Lane> toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode);
						if (toHome == null && v.DestNode == v.HomeNode) toHome = new List<Lane>();
						if (TryStitchPath(toHome, v.HomeNode)) v.AssignReturnPath(toHome);
					}
				}
			}
			// 2. Arrived 상태: 주차 중 도로 변화에 대응 (Mothball 선점 핵심)
			else if (v.State == VehicleState.Arrived) {
				// 주차된 상태에서 집으로 가는 최신 경로를 다시 찾아 예약을 갱신합니다.
				// 이 과정이 있어야 주차 중 도로가 지워져도 Mothballed 상태가 유지됩니다.
				List<Lane> newReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode);
				if (newReturnPath == null && v.DestNode == v.HomeNode) newReturnPath = new List<Lane>();
				
				if (TryStitchPath(newReturnPath, v.HomeNode)) {
					v.AssignReturnPath(newReturnPath); // 실시간 예약 갱신
				}
				v.NeedsPathfind = false;
			}
			// 3. 주행 중 재탐색 (텔레포트 없이 경로만 부드럽게 갱신)
			else {
				Lane currLane = v.GetCurrentLane();
				if (currLane != null) {
					Vector2Int target = v.IsReturning ? v.HomeNode : v.DestNode;
					// 현재 있는 차선의 끝점부터 목표지까지의 경로를 새로 찾습니다.
					List<Lane> remainingPath = Pathfinder.FindPath(currLane.EndNode, target, currLane);
					if (remainingPath == null && currLane.EndNode == target) remainingPath = new List<Lane>();

					if (TryStitchPath(remainingPath, target)) {
						v.AssignPath(remainingPath); // Vehicle.cs의 안전한 갱신 로직 호출
					}
				}
				v.NeedsPathfind = false;
			}
		}
	}
}
