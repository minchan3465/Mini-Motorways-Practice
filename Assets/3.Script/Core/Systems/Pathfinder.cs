using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
    using Core.Data;
    using Core.Utils;

    public static class Pathfinder {
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
    }
}

