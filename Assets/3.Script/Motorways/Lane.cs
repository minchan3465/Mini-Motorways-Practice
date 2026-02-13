using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Motorways {
	public enum RoadState {
		None,
		Planned,    //플레이어가 드래그 했을 때, 가상의 도로를 표시
		Pending,    //건설중
		Active,     //활성화
		Mothballed, //삭제 대기

		//--유틸--
		//ActiveOrPending = Active | Pending,	//집의 진입로를 정할 때, 사용한다고 하는데... 모르겠다.
		//VisiblyActive = Planned | Active,	//눈에 보이는 모든 도로
		//Any = 15	//전부 다
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
