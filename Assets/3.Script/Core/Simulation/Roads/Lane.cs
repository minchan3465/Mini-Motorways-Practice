using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.Simulation.Roads {
	using Core.Systems;

	public enum RoadState {
		Active,     // 정상 작동 중
		Mothballed, // 폐쇄 예정 (차량이 비워지길 기다리는 중)
		Planned     // 건설 중 (건설 애니메이션 등)
	}

	[System.Serializable]
	public class Lane {
		public int Id { get; private set; }
		private static int _nextId = 0;

		// 연결 정보
		public Vector2Int StartNode;
		public Vector2Int EndNode;

		public RoadState State = RoadState.Active;

		public List<int> VehiclesOnLane = new List<int>();			//도로 위 차량
		public HashSet<int> InboundVehicles = new HashSet<int>();   //도로에 오겠다는 차량
		public List<Lane> OutboundLanes = new List<Lane>();

		public float Length;    //Cost 계산용
		public float BaseCost => Length;

		public Lane(Vector2Int start, Vector2Int end) {
			Id = _nextId++;
			StartNode = start;
			EndNode = end;
			Length = Vector2Int.Distance(start, end); // 대각선 고려
		}

		public float GetPathfindingCost() {
			if (State == RoadState.Mothballed) return 100000f;
			float costMultiplier = 1.0f;
			//추후 고속도로 추가시, 0.5f 같은걸로 가중치 낮추기.
			return BaseCost * costMultiplier;
		}

		public bool CanRelease() {
			return VehiclesOnLane.Count == 0 && InboundVehicles.Count == 0;
		}

		public void Reserve(int vehicleID) {
			if (!InboundVehicles.Contains(vehicleID)) InboundVehicles.Add(vehicleID);
		}

		public void Enter(int vehicleId) {
			if (InboundVehicles.Contains(vehicleId)) InboundVehicles.Remove(vehicleId);
			if (!VehiclesOnLane.Contains(vehicleId)) VehiclesOnLane.Add(vehicleId);
		}

		public void Exit(int vehicleId) {
			VehiclesOnLane.Remove(vehicleId);
		}
	}
}
