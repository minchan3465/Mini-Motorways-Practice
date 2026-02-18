using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Motorways.Models;

	public class RoadNetworkManager : MonoBehaviour {
		public static RoadNetworkManager Instance;

		//논리적 도로만 관리.
		public List<Lane> AllLanes { get; private set; } = new List<Lane>();
		private List<Lane> _mothballedLanes = new List<Lane>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Update() {
			ProcessMothballedLanes();
		}

		//--- 외부 연결 로직 ---
		public void TryBuildRoad(Vector2Int from, Vector2Int to) {
			if (Vector2Int.Distance(from, to) > 1.5f) return;

			Lane existingLane = GetLane(from, to);

			if (existingLane != null) {
				//만약 Mothballed 상태인 연결이라면.
				if (existingLane.State == RoadState.Mothballed) {
					RestoreMothballedLane(existingLane);
					//반대편도 복구.
					Lane opposite = GetLane(to, from);
					if (opposite != null) RestoreMothballedLane(opposite);
				}
				return; //이미 활성 도로면 무시.
			}

			//연결이 안되어있다면.
			//자원 차감.
			if (!ResourceManager.Instance.TryConsumeResource(ItemType.Road, 1)) return;

			CreateLane(from, to);
			CreateLane(to, from);

			//도로 연결 되었음.
		}
		public void TryRemoveRoad(Vector2Int targetTile) {
			List<Lane> connectedLanes = AllLanes.FindAll(lane => lane.StartNode == targetTile || lane.EndNode == targetTile);

			if (connectedLanes.Count == 0) return;

			foreach (Lane lane in connectedLanes) {
				SetLaneToMothballed(lane);
			}
		}

		//---내부 로직---
		private void CreateLane(Vector2Int start, Vector2Int end) {
			Lane newLane = new Lane(start, end);
			AllLanes.Add(newLane);

			MapManager.Instance.ConnectLaneToMap(newLane);
			CityModel.LatestLaneChangeFrame = Time.frameCount;
			//CityModel.ChangedLanes.Add(newLane);
			CityModel.ChangedNodes.Add(start);
			CityModel.ChangedNodes.Add(end);
		}

		//---삭제 프로세스---
		private void SetLaneToMothballed(Lane lane) {
			if (lane.State == RoadState.Mothballed) return;

			lane.State = RoadState.Mothballed;
			_mothballedLanes.Add(lane);

			CityModel.LatestLaneChangeFrame = Time.frameCount;
            //CityModel.ChangedLanes.Add(lane);
            CityModel.ChangedNodes.Add(lane.StartNode);
            CityModel.ChangedNodes.Add(lane.EndNode);
            //TODO : 시각적으로 삭제 대기 상태 표시가 필요할 때, 여기서 해줍니다!!
        }
		private void RestoreMothballedLane(Lane lane) {
			if (lane.State == RoadState.Mothballed) {
				lane.State = RoadState.Active;
				_mothballedLanes.Remove(lane);

				CityModel.LatestLaneChangeFrame = Time.frameCount;
                //CityModel.ChangedLanes.Add(lane);
                CityModel.ChangedNodes.Add(lane.StartNode);
                CityModel.ChangedNodes.Add(lane.EndNode);
            }

			//여기도 위와 동일.
		}

		private void ProcessMothballedLanes() {
			if (_mothballedLanes.Count == 0) return;

			for (int i = _mothballedLanes.Count - 1; i >= 0; i--) {
				Lane lane = _mothballedLanes[i];

				if (lane.CanRelease()) {
					FinalizeLaneRemoval(lane);
					_mothballedLanes.RemoveAt(i);
				} else {
					//삭제 불가능 시, 기존에 있던 차량들에게 Hotswap(우회 요청)
					//그럼에도 불가능하면 그냥 가야죠 머...
				}
			}
		}

		//---진짜 삭제---
		private void FinalizeLaneRemoval(Lane lane) {
			AllLanes.Remove(lane);

			//맵 데이터 갱신.
			MapManager.Instance.DisconnectLaneFromMap(lane);

			//도로 자원 반환 추가.
			bool isCanonical = (lane.StartNode.x < lane.EndNode.x) ||
							   (lane.StartNode.x == lane.EndNode.x && lane.StartNode.y < lane.EndNode.y);

			if (isCanonical) ResourceManager.Instance.AddResource(ItemType.Road, 1);

		}
		private Lane GetLane(Vector2Int start, Vector2Int end) {
			return AllLanes.Find(l => l.StartNode == start && l.EndNode == end);
		}
	}
}
