using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Navigation {
	public static class Pathfinder {
		private class Node {
			public Vector2Int Coord;
			public Node Parent;
			public Lane ConnectionLane; // 이 노드로 오기 위해 사용한 타일 간 차선

			public float G; // 시작점으로부터 현재까지의 실제 비용
			public float H; // 현재부터 목적지까지의 추정 비용 (Heuristic)
			public float F => G + H;

			public Node(Vector2Int coord, Node parent, Lane lane, float g, float h) {
				Coord = coord;
				Parent = parent;
				ConnectionLane = lane;
				G = g;
				H = h;
			}
		}

		public static float GetPathCost(Vector2Int start, Vector2Int target) {
			Node endNode = SolveAStar(start, target);
			if (endNode == null) return -1f;

			return endNode.G; // G값이 총 이동 비용
		}

		public static List<Lane> FindPath(Vector2Int start, Vector2Int target) {
			Node endNode = SolveAStar(start, target);
			if (endNode == null) return null;

			return RetracePath(endNode);
		}

		private static Node SolveAStar(Vector2Int start, Vector2Int target) {
			var grid = MapManager.Instance._grid;

			if (start == target) return null;
			if (!grid.ContainsKey(start) || !grid.ContainsKey(target)) return null;


			// 우선순위 큐 대용의 리스트 (최적화 시 SortedList나 PriorityQueue 권장)
			var openSet = new List<Node>();
			var closedSet = new HashSet<Vector2Int>();

			openSet.Add(new Node(start, null, null, 0, Vector2Int.Distance(start, target)));

			while (openSet.Count > 0) {
				// F값이 가장 낮은 노드 선택 (최적화: 힙/우선순위 큐 사용 권장)
				Node current = openSet.OrderBy(n => n.F).First();

				// 목적지에 도달했다면
				if (current.Coord == target) {
					return current;
				}

				openSet.Remove(current);
				closedSet.Add(current.Coord);

				// 아니면 인접 타일 탐색
				if (!grid.TryGetValue(current.Coord, out TileData currentTile)) continue;

				foreach (Lane outboundLane in currentTile.Lanes) {
					if (outboundLane == null) continue;

					Vector2Int neighborCoord = outboundLane.EndNode;

					if (closedSet.Contains(neighborCoord)) continue;

					// Lane의 현재 상태에 따른 가중치 적용 (Mothballed 상태는 매우 높은 비용을 부여하여 회피)
					float movementCostToNeighbor = outboundLane.GetPathfindingCost();
					float newG = current.G + movementCostToNeighbor;

					Node neighborInOpen = openSet.Find(n => n.Coord == neighborCoord);

					if(neighborInOpen == null || newG < neighborInOpen.G) {
						float h = Vector2.Distance(neighborCoord, target);	// 맨해튼 거리 또는 유클리드 거리
						
						if(neighborInOpen == null) {
							openSet.Add(new Node(neighborCoord, current, outboundLane, newG, h));
						} else {
							neighborInOpen.G = newG;
							neighborInOpen.Parent = current;
							neighborInOpen.ConnectionLane = outboundLane;
						}
					}
				}
			}
			return null; // 경로 탐색 실패
		}

		private static List<Lane> RetracePath(Node endNode) {
			List<Lane> path = new List<Lane>();
			Node current = endNode;

			while (current != null && current.ConnectionLane != null) {
				path.Add(current.ConnectionLane);
				current = current.Parent;
			}

			path.Reverse();
			return path;
		}
	}
}
