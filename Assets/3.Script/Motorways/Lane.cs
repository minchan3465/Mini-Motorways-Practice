using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {

	//도로의 상태를 나타내는 열거형

	public enum RoadState {
		None,
		Planned,    //플레이어가 드래그 중인 유령 도로
		Pending,    //건설 대기 중
		Active,     //활성화된 도로
		Mothballed, //삭제 대기 중 (차량이 지나갈 때까지 유지)
	}


	//두 노드 사이의 단방향 연결을 나타내는 클래스

	[System.Serializable]
	public class Lane {
		public int Id { get; private set; }
		private static int _nextId = 0;

		//연결 정보
		public Vector2Int StartNode;
		public Vector2Int EndNode;

		public RoadState State = RoadState.Active;

		//차량 관리
		public List<int> VehiclesOnLane = new List<int>();          //현재 도로 위에 있는 차량들
		public HashSet<int> InboundVehicles = new HashSet<int>();   //이 도로로 진입 예정인 차량들

		public Vector2? ControlPoint;   //곡선을 위한 제어점. (null이라면 직선)
		public bool IsCurved => ControlPoint.HasValue;

		public float Length;    //도로의 길이 (비용 계산용)
		public float BaseCost => Length;

		public Lane(Vector2Int start, Vector2Int end, Vector2? controlPoint = null) {
			Id = _nextId++;
			StartNode = start;
			EndNode = end;
			ControlPoint = controlPoint;

			if (IsCurved) {
				// 2차 베지어 곡선 길이를 수치적으로 대략 계산 (간단하게 10개 세그먼트 합산)
				Length = CalculateBezierLength(
					new Vector3(start.x + 0.5f, 0, start.y + 0.5f),
					new Vector3(controlPoint.Value.x, 0, controlPoint.Value.y),
					new Vector3(end.x + 0.5f, 0, end.y + 0.5f),
					10
				);
			} else {
				Length = Vector2Int.Distance(start, end);
			}
		}

		private float CalculateBezierLength(Vector3 p0, Vector3 p1, Vector3 p2, int segments) {
			float length = 0;
			Vector3 lastPos = p0;
			for (int i = 1; i <= segments; i++) {
				float t = (float)i / segments;
				Vector3 currentPos = Utils.BezierUtils.GetPoint(p0, p1, p2, t);
				length += Vector3.Distance(lastPos, currentPos);
				lastPos = currentPos;
			}
			return length;
		}


		//경로 탐색 시 가중치 계산

		public float GetPathfindingCost() {
			if (State == RoadState.Mothballed) return 100000f; //삭제 중인 도로는 가급적 피함
			return BaseCost;
		}


		//도로를 완전히 삭제해도 되는지 확인 (올라와 있거나 진입 예정인 차가 없어야 함)

		public bool CanRelease() {
			return VehiclesOnLane.Count == 0 && InboundVehicles.Count == 0;
		}

		public void Reserve(int vehicleID) {
			if (!InboundVehicles.Contains(vehicleID)) InboundVehicles.Add(vehicleID);
		}

		public void CancelReservation(int vehicleId) {
			if (InboundVehicles.Contains(vehicleId)) InboundVehicles.Remove(vehicleId);
		}

		public void Enter(int vehicleId) {
			CancelReservation(vehicleId);
			if (!VehiclesOnLane.Contains(vehicleId)) VehiclesOnLane.Add(vehicleId);
		}

		public void Exit(int vehicleId) {
			VehiclesOnLane.Remove(vehicleId);
		}
	}
}
