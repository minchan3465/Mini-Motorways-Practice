using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Structure;

	public class RoadNetwork : MonoBehaviour {
		public static RoadNetwork Instance { get; private set; }

		//그래프 구조입니다.
		//인접 리스트의 구조로 연결되어있는 도로를 판단합니다.

		//Key : 출발 노드의 좌표 / Value: 여기서 시작하는 Lane들의 목록	(즉, 이어져 있는 길을 의미함)
		private Dictionary<Vector2Int, List<Lane>> _adjacencyList = new Dictionary<Vector2Int, List<Lane>>();

		//------------------

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		//--- Lane 등록, 해제 ---
		public void RegisterLane(Lane lane) {
			if(!_adjacencyList.ContainsKey(lane.StartNode)) {
				_adjacencyList.Add(lane.StartNode, new List<Lane>());
			}
			_adjacencyList[lane.StartNode].Add(lane);
		}

		public void UnRegisterLane(Lane lane) {
			if(_adjacencyList.ContainsKey(lane.StartNode)) {
				_adjacencyList[lane.StartNode].Remove(lane);
			}
			//빈 리스트를 지워주는 최적화가 필요할 수 있다.
		}

		//--- Lane 조회 (PathFinder가 사용할거)
		public List<Lane> GetOutboundLanes(Vector2Int node) {
			if(_adjacencyList.TryGetValue(node, out List<Lane> lanes)) {
				return lanes;
			}
			return null;
		}
	}
}

