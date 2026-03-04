using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;

	public class LaneUpdateProcess : MonoBehaviour {
		private int _lastProcessedFrame = -1;

		private void Update() {
			if (CityModel.LatestLaneChangeFrame == _lastProcessedFrame) return;

			//만약 시간이 어긋나면, 맞춰줍니다.
			_lastProcessedFrame = CityModel.LatestLaneChangeFrame;

			//전체 차량 검사...
			if (VehicleMovementProcess.Instance == null) return;
			List<Vehicle> allVehicles = VehicleMovementProcess.Instance._activeVehicle;

			foreach(Vehicle v in allVehicles) {
				bool isAffected = false;

				//경로 교차 여부를 검사합니다.
				if(v.CurrentPath != null) {
					foreach (Lane lane in v.CurrentPath) {
						//if (CityModel.ChangedLanes.Contains(laneInPath) || laneInPath.State == RoadState.Mothballed) {
						//	isAffected = true;
						//	break;
						//}
						if (CityModel.ChangedNodes.Contains(lane.StartNode) ||
							CityModel.ChangedNodes.Contains(lane.EndNode) ||
							lane.State == RoadState.Mothballed) {
							isAffected = true;
							break;
						}
					}
				}

				//복귀 경로도 검사합니다. 아직 영향을 안받은 차량일수도 있으니.
				if(!isAffected && v.ReturnPath != null) {
					foreach(Lane lane in v.ReturnPath) {
                        //if (CityModel.ChangedLanes.Contains(laneInPath) || laneInPath.State == RoadState.Mothballed) {
                        //   isAffected = true;
                        //   break;
                        //}
                        if (CityModel.ChangedNodes.Contains(lane.StartNode) ||
                            CityModel.ChangedNodes.Contains(lane.EndNode) ||
                            lane.State == RoadState.Mothballed) {
                            isAffected = true;
                            break;
                        }
                    }
				}

				//차량에게 우회해야한다고 알림.
				if(isAffected) {
					v.RequestPathfind();
				}
			}

			CityModel.ChangedNodes.Clear();	//처리 끝!
		}
	}
}

