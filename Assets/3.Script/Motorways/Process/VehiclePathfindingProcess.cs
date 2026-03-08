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

				if (v.NeedsPathfind) {
					ProcessPathfindForVehicle(v);
					v.LatestAttemptedPathfindFrame = Time.frameCount; 
				}
			}
		}

		private bool TryStitchPath(List<Lane> path, Vector2Int targetNode) {
			if (path == null) return false;
			if (path.Count == 0 && targetNode != null) {
				// 이미 도착지에 있는 경우 등 예외 처리
				return true;
			}

			Lane lastLane = path.Count > 0 ? path[path.Count - 1] : null;
			Vector2Int lastNode = lastLane != null ? lastLane.EndNode : targetNode; 

			if (lastNode == targetNode && path.Count > 0) return true;

			if (MapManager.Instance._grid.TryGetValue(lastNode, out TileData roadTile)) {
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
			switch (v.State) {
				case VehicleState.Ready:
					List<Lane> toDest = Pathfinder.FindPath(v.HomeNode, v.DestNode);
					if (toDest == null && v.HomeNode == v.DestNode) toDest = new List<Lane>();

					if (TryStitchPath(toDest, v.DestNode)) {
						v.AssignPath(toDest);
						v.State = VehicleState.Driving;
						
						List<Lane> toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode);
						if (toHome == null && v.HomeNode == v.DestNode) toHome = new List<Lane>();
						if (TryStitchPath(toHome, v.HomeNode)) v.AssignReturnPath(toHome);
					}
					return;

				case VehicleState.Driving:
				case VehicleState.Returning:
					v.NeedsPathfind = false;
					return;
			}
		}
	}
}
