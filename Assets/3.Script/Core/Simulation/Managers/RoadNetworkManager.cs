using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Simulation.Managers {
	using Core.Simulation.Roads;

	public class RoadNetworkManager : MonoBehaviour {
		public static RoadNetworkManager Instance;

		//전체 맵 데이터. (좌표 -> 타일 정보)
		public Dictionary<Vector2Int, RoadTile> Grid { get; private set; } = new Dictionary<Vector2Int, RoadTile>();
		private List<Lane> _mothballedLanes = new List<Lane>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Update() {
			ProcessMothballedLanes();
		}


		//--- 도로 설치 ---
		public void TryConnectNodes(Vector2Int from, Vector2Int to) {
			RoadTile fromTile = GetOrCreateTile(from);
			RoadTile toTile = GetOrCreateTile(to);

			Lane existingLane = fromTile.GetLaneTo(to);	//이미 연결된 도로가 있는지 확인.

			if(existingLane != null) {
				//만약 Mothballed 상태인 연결이라면.
				if(existingLane.State == RoadState.Mothballed) {
					existingLane.State = RoadState.Active;

					//삭제 대기 중인 도로에서 제거.
					if (_mothballedLanes.Contains(existingLane)) _mothballedLanes.Remove(existingLane);
				} 
			} else {
				//연결이 안되어있다면.
				CreateOneWayLane(fromTile, toTile);
				CreateOneWayLane(toTile, fromTile);

				//도로 연결 되었음.
			}
		}
		private void CreateOneWayLane(RoadTile start, RoadTile end) {
			Lane newLane = new Lane(start.Coordinates, end.Coordinates);
			start.AddLane(newLane);
		}


		//--- 도로 삭제 대기---
		public void TryRemoveConnection(Vector2Int from, Vector2Int to) {
			RoadTile tile = GetTile(from);
			if (tile == null) return;

			Lane targetLane = tile.GetLaneTo(to);
			if (targetLane == null) return;

			//위에 데이터 둘다 있으면, 도로를 삭제 대기 상태로 변경.
			SetLaneToMothballed(targetLane);

			//반대도 똑같이 삭제 대기로 해줍니다.
			RoadTile oppositeTile = GetTile(to);
			if(oppositeTile != null) {
				Lane oppositeLane = oppositeTile.GetLaneTo(from);
				if(oppositeLane != null) SetLaneToMothballed(oppositeLane);
			}
		}
		private void SetLaneToMothballed(Lane lane) {
			if(lane.State != RoadState.Mothballed) {
				lane.State = RoadState.Mothballed;
				_mothballedLanes.Add(lane);
			}
		}


		//--- 도로 삭제 ---
		private void ProcessMothballedLanes() {
			if (_mothballedLanes.Count == 0) return;

			List<Lane> lanesToRemove = new List<Lane>();

			foreach(var lane in _mothballedLanes) {
				if(lane.CanRelease()) {
					lanesToRemove.Add(lane);
				} else {
					//삭제 불가능하다면
					//해당 위치로 지나갈 차량들에게 경로 다시 찾으라고 말해줍니다.
				}
			}

			foreach(var lane in lanesToRemove) {
				FinalizeLaneRemoval(lane);
			}

		}
		private void FinalizeLaneRemoval(Lane lane) {
			_mothballedLanes.Remove(lane);

			RoadTile tile = GetTile(lane.StartNode);
			if(tile != null) {
				tile.RemoveLane(lane);
			}
			//도로 자원 반환 추가.
		}


		//--- 유틸리티 ---
		private RoadTile GetOrCreateTile(Vector2Int coord) {
			if(!_grid.ContainsKey(coord)) {
				_grid[coord] = new RoadTile(coord);
			}
			return _grid[coord];
		}
		private RoadTile GetTile(Vector2Int coord) {
			if(_grid.TryGetValue(coord, out RoadTile tile)) return tile;
			else return null;
		}
	}
}
