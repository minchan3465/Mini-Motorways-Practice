using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Navigation;

	public class VehiclePathfindingProcess : MonoBehaviour {
		private void Update() {
			if (VehicleMovementProcess.Instance == null) return;

			List<Vehicle> allVehicles = VehicleMovementProcess.Instance._activeVehicle;

			foreach (Vehicle v in allVehicles) {
				if (v.RepathUrgency == PathfindUrgency.WhenPossible) {
					ProcessPathfindForVehicle(v);
				}
			}
		}

		private void ProcessPathfindForVehicle(Vehicle v) {
			switch (v.State) {
				case VehicleState.Ready:
					List<Lane> toDest = Pathfinder.FindPath(v.HomeNode, v.DestNode);
					List<Lane> toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode);

					if (toDest != null && toDest.Count > 0 && toHome != null && toHome.Count > 0) {
                        v.CurrentPath = new Queue<Lane>(toDest);
                        v.ReturnPath = new Queue<Lane>(toHome);
                        foreach (var lane in v.CurrentPath) lane.Reserve(v.Id);
						foreach (var lane in v.ReturnPath) lane.Reserve(v.Id);

						v.State = VehicleState.Driving;
					} else {
						// ½ÇÆÐ½Ã °æ·Î ¸øÃ£À½
					}
					v.RepathUrgency = PathfindUrgency.NotRequired;
					return;
				case VehicleState.Arrived:
                    List<Lane> newReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode);
                    if (newReturnPath != null && newReturnPath.Count > 0) {
						v.AssignReturnPath(newReturnPath);
					}
					v.RepathUrgency = PathfindUrgency.NotRequired;
					return;
				case VehicleState.Driving:
				case VehicleState.Returning:
					Lane committedLane = v.LastCommittedLane;
					if (committedLane == null) {
						v.RepathUrgency = PathfindUrgency.NotRequired;
						return;
					}

					Vector2Int startNode = committedLane.EndNode;

					if (v.State == VehicleState.Driving) {
						if (startNode != v.DestNode) {
							List<Lane> newPathRemaining = Pathfinder.FindPath(startNode, v.DestNode);
							if (newPathRemaining != null && newPathRemaining.Count > 0) {
								v.AssignPath(newPathRemaining);
							} else {
								Debug.LogError("[Crisis Error] Á¿µÊ. ¸ñÀûÁö ¸ø°¨.");
							}
						}
						List<Lane> updateReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode);
						if (updateReturnPath != null && updateReturnPath.Count > 0) {
							v.AssignReturnPath(updateReturnPath);
						}
					} else if (v.State == VehicleState.Returning) {
						if (startNode != v.HomeNode) {
							List<Lane> newPathRemaining = Pathfinder.FindPath(startNode, v.HomeNode);
							if (newPathRemaining != null && newPathRemaining.Count > 0) {
								v.AssignPath(newPathRemaining);
							} else {
								Debug.LogError("[Crisis Error] Á¿µÊ. Áý ¸ø°¨.");
							}
						}
					}
					v.RepathUrgency = PathfindUrgency.NotRequired;
					return;
			}
		}
	}
}
