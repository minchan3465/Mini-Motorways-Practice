using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Managers;
	using Motorways.Navigation;

	public class DispatchProcess : MonoBehaviour, ISimulationProcess {
		public static DispatchProcess Instance;

		private List<House> _houses = new List<House>();
		private List<Destination> _destinations = new List<Destination>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		public void RegisterHouse(House house) {
			if (!_houses.Contains(house)) _houses.Add(house);
		}

		public void RegisterDestination(Destination dest) {
			if (!_destinations.Contains(dest)) _destinations.Add(dest);
		}

		public void Tick(float dt) {
			//매 틱마다 모든 목적지를 돌며 배정이 안 된 핀(수요)을 처리합니다.
			foreach(var dest in _destinations) {
				for (int i = 0; i < dest.UnassignedPins; i++) {
					House bestHouse = FindNearestConnectionHouse(dest);

					if(bestHouse != null) {
						//가장 가까운 집에게 배차 명령
						if(bestHouse.TryDispatchVehicle(dest)) {
							dest.ReserverPin();
						} else {
							//이 목적지에 대한 배차 시도가 이번 틱에 실패했다면(쿨다운 등) 다음 목적지로
							break;
						}
					}
				}
			}
		}

		public House FindNearestConnectionHouse(Destination dest) {
			House bestHouse = null;
			float minCost = float.MaxValue;

			Vector2Int destNode = dest.EntranceCoordinate;
			
			//모든 집을 전수 조사하여 같은 색상 중 가장 가까운(경로 비용이 낮은) 집을 찾습니다.
			foreach(var house in _houses) {
				if (house.GroupIndex == dest.GroupIndex && house.WaitingVehicles.Count > 0) {
					//집의 입구에서 목적지 입구까지의 실제 도로 거리를 가져옵니다.
					Vector2Int startNode = house.EntranceCoordinate;
					float pathCost = Pathfinder.GetPathCost(startNode, destNode);
					
					//-1은 경로가 없음을 의미합니다.
					if(pathCost != -1 && pathCost < minCost) {
						minCost = pathCost;
						bestHouse = house;
					}
				}
			}
			return bestHouse;
		}
	}
}
