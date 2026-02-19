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
			foreach(var dest in _destinations) {
				if(dest.UnassignedPins > 0) {
					House bestHouse = FindNearestConnectionHouse(dest);

					if(bestHouse != null) {
						Vector2Int targetNode = dest._CarPark.EntranceLane.StartNode;
						if(bestHouse.TryDispatcVehicle(targetNode)) {
							dest.ReserverPin();
						}
					}
				}
			}
		}

		public House FindNearestConnectionHouse(Destination dest) {
			House bestHouse = null;
			float minCost = int.MaxValue;

			Vector2Int destNode = dest._CarPark.EntranceLane.StartNode;
			foreach(var house in _houses) {
				if (house.GroupIndex == dest.GroupIndex && house.WaitingVehicles.Count > 0) {
					Vector2Int startNode = house.EntranceLane.EndNode;
					float pathCost = Pathfinder.GetPathCost(startNode, destNode);
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

