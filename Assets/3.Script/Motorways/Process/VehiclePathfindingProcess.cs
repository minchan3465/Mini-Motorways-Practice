using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Navigation;
	using Motorways.Managers;

	public class VehiclePathfindingProcess : MonoBehaviour {
		private void Update() {
			if (VehicleMovementProcess.Instance == null) return;

			List<Vehicle> allVehicles = VehicleMovementProcess.Instance._activeVehicle;
			int latestLaneChange = CityModel.LatestLaneChangeFrame;

			foreach (Vehicle v in allVehicles) {
				// 원작과 동일하게 도로망이 변했으면(latestLaneChangeFrame이 더 높으면) 재탐색 요청
				if (v.LatestAttemptedPathfindFrame < latestLaneChange) {
					v.RequestPathfind();
				}

				if (v.RepathUrgency == PathfindUrgency.WhenPossible) {
					ProcessPathfindForVehicle(v);
					v.LatestAttemptedPathfindFrame = Time.frameCount; // 현재 프레임 기록
				}
			}
		}

		private bool TryStitchPath(List<Lane> path, Vector2Int targetNode) {
			if (path == null || path.Count == 0) return false;

			Lane lastLane = path[path.Count - 1];
			if (lastLane.EndNode == targetNode) return true;

			// ������ ��尡 ��ǥ�� �ٷ� ��(�Ÿ� 1)���� Ȯ��
			if (Vector2Int.Distance(lastLane.EndNode, targetNode) <= 1.5f) {
				if (MapManager.Instance._grid.TryGetValue(lastLane.EndNode, out TileData roadTile)) {
					foreach (var lane in roadTile.Lanes) {
						if (lane != null && lane.EndNode == targetNode) {
							//ã�Ҵ�! ��ο� �߰�
							path.Add(lane);
							return true;
						}
					}
				}
			}
			return false;
		}



		private void ProcessPathfindForVehicle(Vehicle v) {
			switch (v.State) {
				case VehicleState.Ready:
					List<Lane> toDest = Pathfinder.FindPath(v.HomeNode, v.DestNode);
					List<Lane> toHome = Pathfinder.FindPath(v.DestNode, v.HomeNode);

					bool destOk = TryStitchPath(toDest, v.DestNode);
					bool homeOk = TryStitchPath(toHome, v.HomeNode);

					if (destOk && homeOk) {
						v.CurrentPath = new Queue<Lane>(toDest);
						v.ReturnPath = new Queue<Lane>(toHome);

						foreach (var lane in v.CurrentPath) lane.Reserve(v.Id);
						foreach (var lane in v.ReturnPath) lane.Reserve(v.Id);

						v.State = VehicleState.Driving;
					}
					v.RepathUrgency = PathfindUrgency.NotRequired;
					return;
				case VehicleState.Arrived:
                    List<Lane> newReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode);
					if (TryStitchPath(newReturnPath, v.HomeNode)) {
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
							if (TryStitchPath(newPathRemaining, v.DestNode)) {
								v.AssignPath(newPathRemaining);
							}
						}
						List<Lane> updateReturnPath = Pathfinder.FindPath(v.DestNode, v.HomeNode);
						if (TryStitchPath(updateReturnPath, v.HomeNode)) {
							v.AssignReturnPath(updateReturnPath);
						}
					} else if (v.State == VehicleState.Returning) {
						if (startNode != v.HomeNode) {
							List<Lane> newPathRemaining = Pathfinder.FindPath(startNode, v.HomeNode);
							if (TryStitchPath(newPathRemaining, v.HomeNode)) {
								v.AssignPath(newPathRemaining);
							}
						}
					}
					v.RepathUrgency = PathfindUrgency.NotRequired;
					return;
			}
		}
	}
}
