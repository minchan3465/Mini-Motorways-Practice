using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Motorways.Models;
	using Motorways.Utils;

	public class RoadNetworkManager : MonoBehaviour {
		public static RoadNetworkManager Instance;

		//논리적 도로만 관리.
		public List<Lane> AllLanes { get; private set; } = new List<Lane>();
		private List<Lane> _mothballedLanes = new List<Lane>();
		private HashSet<Lane> _systemLanes = new HashSet<Lane>();	//건물이나 목적지용 도로.

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Update() {
			ProcessMothballedLanes();
			if (CityModel.ChangedNodes.Count > 0) {
				RoadVisualManager.Instance.UpdateTileVisuals(CityModel.ChangedNodes);
				CityModel.ChangedNodes.Clear();
			}
		}

		//--- 외부 연결 로직 ---
		public void TryBuildRoad(Vector2Int from, Vector2Int to) {
			if (Vector2Int.Distance(from, to) > 1.5f) return;

			Lane existingLane = GetLane(from, to);

			//null이라면 도로가 없는 상태.
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

		//건물용 메서드
		public void BuildSystemRoad(Vector2Int from, Vector2Int to, out Lane outLane, out Lane inLane) {
			outLane = new Lane(from, to);
			inLane = new Lane(to, from);
			
			AllLanes.Add(outLane);
			AllLanes.Add(inLane);
			_systemLanes.Add(outLane); //시스템 도로로 등록
			_systemLanes.Add(inLane);

			MapManager.Instance.ConnectLaneToMap(outLane);
			MapManager.Instance.ConnectLaneToMap(inLane);

			//CityModel.LatestLaneChangeFrame = Time.frameCount;
			CityModel.ChangedNodes.Add(from);
			CityModel.ChangedNodes.Add(to);
		}


		public void TryRemoveRoad(Vector2Int targetTile) {
			if (MapManager.Instance._grid.TryGetValue(targetTile, out TileData tile)) {
				if (tile.Building != null) return; // 건물 위면 삭제 불가!
			}

			List<Lane> connectedLanes = AllLanes.FindAll(lane => lane.StartNode == targetTile || lane.EndNode == targetTile);
			if (connectedLanes.Count == 0) return; //안전장치

			foreach (Lane lane in connectedLanes) {
				if (_systemLanes.Contains(lane)) continue;
				SetLaneToMothballed(lane);
			}
		}
		public void MothballSystemRoad(Lane outLane, Lane inLane) {
			if (outLane != null) {
				SetLaneToMothballed(outLane);
				MapManager.Instance.DisconnectLaneFromMap(outLane);
			}

			if (inLane != null) {
				SetLaneToMothballed(inLane);
				MapManager.Instance.DisconnectLaneFromMap(inLane);
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
			//입구 도로 Mothballed인데, 우리가 지은건 아니잖슴.
			//즉, AllLanes에 없는 도로이므로 예외처리. (없으면 False가 도출되고, False면 도로 반환 x)
			bool wasPlayerBuilt = AllLanes.Remove(lane);
			bool isSystem = _systemLanes.Remove(lane); // 시스템 장부에서도 삭제

			//맵 데이터 갱신.
			MapManager.Instance.DisconnectLaneFromMap(lane);

			//도로 자원 반환 추가.
			if (wasPlayerBuilt && !isSystem) {
				bool isCanonical = (lane.StartNode.x < lane.EndNode.x) ||
								   (lane.StartNode.x == lane.EndNode.x && lane.StartNode.y < lane.EndNode.y);

				if (isCanonical) ResourceManager.Instance.AddResource(ItemType.Road, 1);
			}
		}
		private Lane GetLane(Vector2Int start, Vector2Int end) {
			return AllLanes.Find(l => l.StartNode == start && l.EndNode == end);
		}
	}
}
