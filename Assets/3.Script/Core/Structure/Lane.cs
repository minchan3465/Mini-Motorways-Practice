using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.Structure {
	using Core.Systems;

	public enum LaneState {
		Active,
		Mothballed
	}

	public class Lane {
		public Vector2Int StartNode;
		public Vector2Int EndNode;

		public List<Lane> OutboundLands = new List<Lane>();

		public Vector3 StartWorldPos;
		public Vector3 EndWorldPos;
		public float Length;

		public LaneState State = LaneState.Active;

		public int Cost {
			get {
				int baseCost = Mathf.RoundToInt(Length * 10);

				if(State == LaneState.Mothballed) {
					return 100000;
				}

				return baseCost;
			}
		}

		public List<CarMovement> VehiclesOnLane = new List<CarMovement>();  //앞차 감지용

		public Lane(Vector2Int start, Vector2Int end, Vector3 startPos, Vector3 endPos) {
			StartNode = start;
			EndNode = end;
			StartWorldPos = startPos;
			EndWorldPos = endPos;
			Length = Vector3.Distance(startPos, endPos);
		}

		public void RegisterVehicle(CarMovement car) {
			if (!VehiclesOnLane.Contains(car)) VehiclesOnLane.Add(car);
		}
		public void UnregisterVehicle(CarMovement car) {
			VehiclesOnLane.Remove(car);
		}

		// 도로가 비었는지 확인 (삭제 가능 여부 체크용)
		public bool IsEmpty => VehiclesOnLane.Count == 0;
	}
}
