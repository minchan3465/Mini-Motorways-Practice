using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Navigation {
	public static class Pathfinder {
		private class Node {
			public Vector2Int Coord;
			public Node Parent;
			public Lane ConnectionLane; //이 노드로 오기 위해 타고 온 도로

			public float G; //시작점에서 지금까지의 비용
			public float H; //목적지까지의 예상 비용 (Heuristic)
			public float F => G + H;

			public Node(Vector2Int coord, Node parent, Lane lane, float g, float h) {
				Coord = coord;
				Parent = parent;
				ConnectionLane = lane;
				G = g;
				H = h;
			}
		}

		public static List<Lane> FindPath(Vector2Int start, Vector2Int target) {
			var grid = MapManager.Instance._grid;

			if (start == target) return new List<Lane>();
			if (!grid.ContainsKey(start) || !grid.ContainsKey(target)) return null;


			//우선순위 큐 역할을 할 리스트 (간결함을 위해 SortedList나 PriorityQueue 대용)
			var openSet = new List<Node>();
			var closedSet = new HashSet<Vector2Int>();

			openSet.Add(new Node(start, null, null, 0, Vector2Int.Distance(start, target)));

			while (openSet.Count > 0) {
				// F값이 가장 낮은 노드 선택 (최적화: 힙/우선순위 큐 사용 가능)
				Node current = openSet.OrderBy(n => n.F).First();

				if (current.Coord == target) {
					return RetracePath(current);
				}

				openSet.Remove(current);
				closedSet.Add(current.Coord);

				//만약 목적지에 도달했다면.
				if (current.Coord == target) {
					return RetracePath(current);
				}

				//아니라면 인접 타일 탐색합니다.
				if (!grid.TryGetValue(current.Coord, out TileData currentTile)) continue;

				foreach (Lane outboundLane in currentTile.Lanes) {
					if (outboundLane == null) continue;

					Vector2Int neighborCoord = outboundLane.EndNode;

					if (closedSet.Contains(neighborCoord)) continue;

					//Lane의 현재 상태에 따른 비용 계산. (Mothballed 상태면 100,000 이라 가능하면 회피.)
					float movementCostToNeighbor = outboundLane.GetPathfindingCost();
					float newG = current.G + movementCostToNeighbor;

					Node neighborInOpen = openSet.Find(n => n.Coord == neighborCoord);

					if(neighborInOpen == null || newG < neighborInOpen.G) {
						float h = Vector2.Distance(neighborCoord, target);	//맨해튼 거리 or 유클리드.
						
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
			return null; //경로 탐색 실패
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
		/* Lane으로 계산 (old)
		private class PathNode {
			public Vector2Int Position; //현재 노드 좌표.
			public PathNode Parent;     //어디서 왔는지. (위치)
			public Lane IncomingLane;   //어떤 도로부터 왔는지.

			public int GCost;   //시작으로부터 현재까지 누적 비용.
			public int HCost;   //휴리스틱 알고리즘.
			public int FCost => GCost + HCost;

			public PathNode(Vector2Int pos) {
				Position = pos;
				GCost = int.MaxValue; // 초기값은 무한대 (아직 방문 안 함)
				HCost = 0;
				Parent = null;
				IncomingLane = null;
			}
		}

		//--- Lane 기반 길찾기 ---
		public static List<Lane> FindLanePath(Vector2Int startNodePos, Vector2Int targetNodePos) {
			if (RoadNetwork.Instance == null) return null;
			if (RoadNetwork.Instance.GetOutboundLanes(startNodePos) == null) return null;

			List<PathNode> openList = new List<PathNode>();
			HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
			Dictionary<Vector2Int, PathNode> allNodes = new Dictionary<Vector2Int, PathNode>();

			PathNode startNode = new PathNode(startNodePos);
			openList.Add(startNode);
			allNodes.Add(startNodePos, startNode);

			while(openList.Count > 0) {
				//F값이 가장 낮은 노드를 꺼냅니다. (최적화 가능 : 힙 사용. 근데 리스트도 뭐...)
				PathNode currentNode = GetLowestFCostNode(openList);
				for(int i = 1; i<openList.Count; i++) {
					if (openList[i].FCost < currentNode.FCost ||
						(openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
						currentNode = openList[i];
				}

				openList.Remove(currentNode);
				closedSet.Add(currentNode.Position);

				//목적지 도착했는지.
				if(currentNode.Position == targetNodePos) {
					return RetraceLanePath(startNode, currentNode);
				}

				//이웃 탐색합니다.
				List<Lane> outboundLanes = RoadNetwork.Instance.GetOutboundLanes(currentNode.Position);
				if(outboundLanes != null) {
					foreach(Lane lane in outboundLanes) {
						Vector2Int neighborPos = lane.EndNode;  //Lane의 끝이 곧 이웃노드.
						if (closedSet.Contains(neighborPos)) continue;

						//이동 비용 계산.
						//근데 Lane.Cost가 Mothballed 상태면 큰 숫자이므로, 그쪽으로 안갈것.
						int newMovementCost = currentNode.GCost + lane.Cost;

						PathNode neighborNode;
						if(!allNodes.TryGetValue(neighborPos, out neighborNode)) {
							neighborNode = new PathNode(neighborPos);
							allNodes.Add(neighborPos, neighborNode);
							openList.Add(neighborNode); //처음보면 추가.
						}

						//더 적은 비용의 경로 or 아직 방문 안한 경로라면 갱신
						if(newMovementCost < neighborNode.GCost || !openList.Contains(neighborNode)) {
							neighborNode.GCost = newMovementCost;
							neighborNode.HCost = GetHeuristic(neighborPos, targetNodePos);
							neighborNode.Parent = currentNode;
							neighborNode.IncomingLane = lane;   //어떤 Lane을 탔는지 기록합니다. 이는 곧 차량의 경로가 됩니다.
						}

						if(!openList.Contains(neighborNode)) {
							openList.Add(neighborNode);
						}
					}
				}
			}
			return null;
		}

		private static PathNode GetLowestFCostNode(List<PathNode> pathNodes) {
			PathNode lowest = pathNodes[0];
			for (int i = 1; i < pathNodes.Count; i++) {
				if (pathNodes[i].FCost < lowest.FCost ||
				   (pathNodes[i].FCost == lowest.FCost && pathNodes[i].HCost < lowest.HCost)) {
					lowest = pathNodes[i];
				}
			}
			return lowest;
		}

		private static List<Lane> RetraceLanePath(PathNode startNode, PathNode endNode) {
			List<Lane> path = new List<Lane>();
			PathNode currentNode = endNode;

			while(currentNode != startNode) {
				//이 노드를 들어올 때 탔던 Lane을 추가.
				if(currentNode.IncomingLane != null) {
					path.Add(currentNode.IncomingLane);
				}
				currentNode = currentNode.Parent;
			}

			path.Reverse();
			return path;
		}

		private static int GetHeuristic(Vector2Int a, Vector2Int b) {
			return Mathf.RoundToInt(Vector2Int.Distance(a, b) * 10);
		}
		*/
		/* 그리드(타일) 기반 비트마스크로 경로 계산하는 것. (구식)
		//A* 알고리즘용 노드 클래스
		private class Node {
			public Vector2Int Position;
			public Node Parent;
			public float GCost; // 시작점부터 비용
			public float HCost; // 목적지까지 추정 비용
			public float FCost => GCost + HCost;

			public Node(Vector2Int pos) { Position = pos; }
		}

		// 8방향 오프셋
		private static readonly Vector2Int[] _directions = new Vector2Int[]
		{
			new Vector2Int(0, 1),  // North
			new Vector2Int(0, -1), // South
			new Vector2Int(1, 0),  // East
			new Vector2Int(-1, 0), // West
			new Vector2Int(1, 1),  // NorthEast
			new Vector2Int(1, -1), // SouthEast
			new Vector2Int(-1, -1),// SouthWest
			new Vector2Int(-1, 1)  // NorthWest
		};

		//--- 길찾기 알고리즘 ---
		public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos, bool allowMothballed = false) {
			// 맵에 없는 좌표면 취소
			if (MapBootstrapper.Grid == null ||
				!MapBootstrapper.Grid.ContainsKey(startPos) ||
				!MapBootstrapper.Grid.ContainsKey(targetPos)) return null;

			List<Node> openList = new List<Node>();
			HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

			Node startNode = new Node(startPos);
			Node targetNode = new Node(targetPos);

			openList.Add(startNode);

			while (openList.Count > 0) {
				//F값이 가장 낮은 노드 선택
				Node currentNode = openList[0];
				for (int i = 1; i < openList.Count; i++) {
					if (openList[i].FCost < currentNode.FCost ||
					   (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)) {
						currentNode = openList[i];
					}
				}

				openList.Remove(currentNode);
				closedSet.Add(currentNode.Position);

				//목적지 도착하면 그냥 끝내기.
				if (currentNode.Position == targetNode.Position) {
					return RetracePath(startNode, currentNode);
				}

				//이웃 탐색...
				foreach (Vector2Int neighborPos in GetConnectedNeighbors(currentNode.Position, allowMothballed)) {
					if (closedSet.Contains(neighborPos)) continue;

					// 이동 비용 (직선: 1 / 대각선: 1.414)
					float moveCost = Vector2Int.Distance(currentNode.Position, neighborPos);
					float newMovementCost = currentNode.GCost + moveCost;

					Node neighborNode = openList.Find(n => n.Position == neighborPos);

					if (neighborNode == null || newMovementCost < neighborNode.GCost) {
						if (neighborNode == null) {
							neighborNode = new Node(neighborPos);
							openList.Add(neighborNode);
						}

						neighborNode.GCost = newMovementCost;
						neighborNode.HCost = GetHeuristic(neighborPos, targetNode.Position);
						neighborNode.Parent = currentNode;
					}
				}
			}

			return null;
		}

		// 경로 역추적
		private static List<Vector2Int> RetracePath(Node startNode, Node endNode) {
			List<Vector2Int> path = new List<Vector2Int>();
			Node currentNode = endNode;

			while (currentNode != startNode) {
				path.Add(currentNode.Position);
				currentNode = currentNode.Parent;
			}
			// path.Add(startNode.Position); // 시작점 포함 여부는 선택 (차량 이동 시 제외하는 게 보통)
			path.Add(startNode.Position);

			path.Reverse();
			return path;
		}

		// 휴리스틱 (유클리드 거리)
		private static float GetHeuristic(Vector2Int a, Vector2Int b) {
			return Vector2.Distance(a, b);
		}

		// [핵심] 현재 위치에서 '도로적으로 연결된' 이웃만 반환.
		private static List<Vector2Int> GetConnectedNeighbors(Vector2Int current, bool allowMothballed) {
			List<Vector2Int> neighbors = new List<Vector2Int>();
			if (!MapBootstrapper.Grid.TryGetValue(current, out CellData currentData)) {
				return neighbors;
			}

			foreach (Vector2Int dir in _directions) {
				RoadDirection dirEnum = DirUtiles.GetDirectionFromVector(dir);

				bool isOutgoingValid = false;

				if (allowMothballed) {
					//2차 탐색 - 물리적 연결만 잇으면 ok (ConnectionMask 확인합니다)
					if (currentData.HasConnection(dirEnum)) isOutgoingValid = true;
				} else {
					//1차 탐색 - 활성 연결만 OK (ActiveMask 확인합니다)
					if (currentData.IsActiveConnection(dirEnum)) isOutgoingValid = true;
				}

				if (isOutgoingValid) {
					Vector2Int neighborPos = current + dir;

					//아무튼 경로가 있다면
					if (MapBootstrapper.Grid.TryGetValue(neighborPos, out CellData neighborData)) {
						if (neighborData.IsDriveable) {
							//혹시 모를 확인.
							neighbors.Add(neighborPos);
						}
					}
				}
			}
			return neighbors;
		}
		*/

