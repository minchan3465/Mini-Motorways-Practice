using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;

	public static class Pathfinder {
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

        public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos) {
            // 맵에 없는 좌표면 취소
            if (!MapBootstrapper.Grid.ContainsKey(startPos) || !MapBootstrapper.Grid.ContainsKey(targetPos)) return null;

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
                foreach (Vector2Int neighborPos in GetConnectedNeighbors(currentNode.Position)) {
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

        // [핵심] 현재 위치에서 '도로적으로 연결된' 이웃만 반환
        private static List<Vector2Int> GetConnectedNeighbors(Vector2Int current) {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            if (!MapBootstrapper.Grid.TryGetValue(current, out CellData currentData))
                return neighbors;

            // 내 타일의 ConnectionMask를 확인해서 갈 수 있는 방향인지 검사
            foreach (Vector2Int dir in _directions) {
                RoadDirection dirEnum = GetDirectionEnum(dir);

                //현재 타일이랑 연결된 타일이 있는가? (도로 뚫린거)
                if (currentData.ConnectionMask.HasFlag(dirEnum)) {
                    Vector2Int neighborPos = current + dir;

                    //해당 연결된 곳에 타일이 있는지. (예상치 못한 오류 계산, 실시간 계산용)
                    if (MapBootstrapper.Grid.ContainsKey(neighborPos)) {
                        // (옵션) 상대방도 나를 향해 뚫려있는지 검사할 수 있지만,
                        // RoadSystem에서 양방향 연결을 보장하므로 생략 가능.
                        neighbors.Add(neighborPos);
                    }
                }
            }
            return neighbors;
        }

        //-----------유틸 :  벡터 -> Enum 변환
        private static RoadDirection GetDirectionEnum(Vector2Int dir) {
            if (dir.x == 0 && dir.y == 1) return RoadDirection.North;
            if (dir.x == 0 && dir.y == -1) return RoadDirection.South;
            if (dir.x == 1 && dir.y == 0) return RoadDirection.East;
            if (dir.x == -1 && dir.y == 0) return RoadDirection.West;
            if (dir.x == 1 && dir.y == 1) return RoadDirection.NorthEast;
            if (dir.x == 1 && dir.y == -1) return RoadDirection.SouthEast;
            if (dir.x == -1 && dir.y == -1) return RoadDirection.SouthWest;
            if (dir.x == -1 && dir.y == 1) return RoadDirection.NorthWest;
            return RoadDirection.None;
        }
    }
}

