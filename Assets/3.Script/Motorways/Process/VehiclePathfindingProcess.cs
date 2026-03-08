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
				if (v.LatestAttemptedPathfindFrame < latestLaneChange) {
					v.RequestPathfind();
				}

				if (v.NeedsPathfind && v.State == VehicleState.Ready) {
					ProcessPathfindForVehicle(v);
					v.LatestAttemptedPathfindFrame = Time.frameCount; 
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
			// 현재 상태가 Ready일 때만 길을 찾습니다.
			Vector2Int start = v.IsReturning ? v.DestNode : v.HomeNode;
			Vector2Int target = v.IsReturning ? v.HomeNode : v.DestNode;

			List<Lane> path = Pathfinder.FindPath(start, target);
			if (path == null && start == target) path = new List<Lane>();

			if (TryStitchPath(path, target)) {
				v.AssignPath(path);
				v.State = v.IsReturning ? VehicleState.Returning : VehicleState.Driving;

				// [고립 방지 핵심] 목적지로 출발하는 순간, 귀환 경로도 미리 계산해서 예약해둡니다.
				if (!v.IsReturning) {
					List<Lane> toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode);
					if (toHome == null && v.DestNode == v.HomeNode) toHome = new List<Lane>();
					if (TryStitchPath(toHome, v.HomeNode)) {
						v.AssignReturnPath(toHome);
					}
				}
			}
		}
	}
}
