using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;

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

			if (VehicleMovementProcess.Instance == null) return;
			List<Vehicle> allVehicles = VehicleMovementProcess.Instance._activeVehicle;

			foreach(Vehicle v in allVehicles) {
				bool isAffected = false;

				if(v.CurrentPath != null) {
					foreach (Lane lane in v.CurrentPath) {
						if (CityModel.ChangedNodes.Contains(lane.StartNode) ||
							CityModel.ChangedNodes.Contains(lane.EndNode) ||
							lane.State == RoadState.Mothballed) {
							isAffected = true;
							break;
						}
					}
				}

				if(!isAffected && v.ReturnPath != null) {
					foreach(Lane lane in v.ReturnPath) {
                        if (CityModel.ChangedNodes.Contains(lane.StartNode) ||
                            CityModel.ChangedNodes.Contains(lane.EndNode) ||
                            lane.State == RoadState.Mothballed) {
                            isAffected = true;
                            break;
                        }
                    }
				}

				if(isAffected) {
					v.RequestPathfind();
				}
			}

			// Removed CityModel.ChangedNodes.Clear(); to allow RoadNetworkManager.LateUpdate to render them first.
		}
	}
}
