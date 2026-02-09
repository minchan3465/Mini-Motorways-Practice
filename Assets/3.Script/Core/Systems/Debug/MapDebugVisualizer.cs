using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // 핸들(Label) 사용을 위해
#endif

namespace Core.Systems {
    using Core.Data;

    public class MapDebugVisualizer : MonoBehaviour {
        [Header("References")]
        [SerializeField] private InteractionController _interactionController;

        private void OnDrawGizmos() {
            // 1. 맵 데이터 그리기 (도로 및 삭제 대기 상태)
            DrawGridData();

            // 2. 인터랙션 그리기 (드래그, 커서 등)
            DrawInteractionVisuals();
        }

        // ========================================================================
        // 1. 맵 데이터 시각화 (Active vs Mothballed)
        // ========================================================================
        private void DrawGridData() {
            if (MapBootstrapper.Grid == null) return;

            foreach (var kvp in MapBootstrapper.Grid) {
                CellData cell = kvp.Value;
                if (cell.Type == TileLogicType.Empty) continue;

                Vector3 center = new Vector3(cell.Coordinate.x + 0.5f, 0, cell.Coordinate.y + 0.5f);

                // A. 타일 타입별 기본 색상 박스
                switch (cell.Type) {
                    case TileLogicType.Obstacle: Gizmos.color = Color.red; break;
                    case TileLogicType.Road: Gizmos.color = Color.gray; break;
                    case TileLogicType.Supply: Gizmos.color = Color.blue; break;
                    case TileLogicType.Demand: Gizmos.color = Color.yellow; break;
                    case TileLogicType.Entrance: Gizmos.color = Color.cyan; break;
                    case TileLogicType.Restricted: Gizmos.color = new Color(0.5f, 0, 0, 0.5f); break;
                }
                Gizmos.DrawWireCube(center, Vector3.one * 0.9f);

                // B. 도로 연결 상태 그리기 (핵심)
                if (cell.ConnectionMask != RoadDirection.None) {
                    DrawConnections(center, cell);
                }

                // C. (옵션) 예약자 수 표시 - 씬 뷰에서 텍스트로 보임
#if UNITY_EDITOR
                if (cell.ReservationCount > 0) {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.fontSize = 15;
                    style.fontStyle = FontStyle.Bold;
                    Handles.Label(center + Vector3.up * 0.5f, $"{cell.ReservationCount}", style);
                }
#endif
            }
        }

        private void DrawConnections(Vector3 center, CellData cell) {
            // 8방향 벡터 정의
            Vector2Int[] offsets = {
                new Vector2Int(0, 1),   // North
                new Vector2Int(0, -1),  // South
                new Vector2Int(1, 0),   // East
                new Vector2Int(-1, 0),  // West
                new Vector2Int(1, 1),   // NE
                new Vector2Int(1, -1),  // SE
                new Vector2Int(-1, -1), // SW
                new Vector2Int(-1, 1)   // NW
            };

            // Enum 배열 정의 (위 오프셋 순서와 일치해야 함)
            RoadDirection[] dirs = {
                RoadDirection.North, RoadDirection.South, RoadDirection.East, RoadDirection.West,
                RoadDirection.NorthEast, RoadDirection.SouthEast, RoadDirection.SouthWest, RoadDirection.NorthWest
            };

            for (int i = 0; i < 8; i++) {
                RoadDirection dir = dirs[i];
                Vector3 offsetVec = new Vector3(offsets[i].x, 0, offsets[i].y) * 0.5f;

                // 1. 삭제 대기 중인 연결 (Mothballed) -> 빨간색
                if ((cell.MothballedMask & dir) != 0) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(center, center + offsetVec);
                    // 끝에 작은 구체로 "끊김 예정" 표시
                    Gizmos.DrawWireSphere(center + (offsetVec * 0.8f), 0.1f);
                }
                // 2. 정상 활성 연결 (Active) -> 자주색
                else if ((cell.ConnectionMask & dir) != 0) {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(center, center + offsetVec);
                }
            }
        }

        // ========================================================================
        // 2. 인터랙션 시각화 (통합된 부분)
        // ========================================================================
        private void DrawInteractionVisuals() {
            if (!Application.isPlaying || _interactionController == null) return;
            if (!_interactionController.IsPointerValid) return;

            Vector2Int currentGrid = _interactionController.CurrentGridPointer;

            // A. 현재 마우스 커서 위치 (항상 표시)
            Gizmos.color = Color.cyan;
            Vector3 drawPos = new Vector3(currentGrid.x + 0.5f, 0, currentGrid.y + 0.5f);
            Gizmos.DrawWireCube(drawPos, Vector3.one * 0.95f);

            // B. 건설 드래그 중일 때
            if (_interactionController.IsDraggingBuild) {
                Vector3 centerPos;

                // 집 회전 드래그 vs 일반 도로 드래그 색상 구분
                if (_interactionController.DragStartHouse != null) {
                    centerPos = _interactionController.DragStartHouse.transform.position;
                    Gizmos.color = Color.green;
                } else {
                    Vector2Int lastPtr = _interactionController.LastGridPointer;
                    centerPos = new Vector3(lastPtr.x + 0.5f, 0, lastPtr.y + 0.5f);
                    Gizmos.color = Color.yellow;
                }

                if (!_interactionController.HasPassedDeadzone) {
                    // 데드존 표시
                    Gizmos.DrawWireSphere(_interactionController.ClickOriginWorldPos, _interactionController.InitialDragDeadzone);
                } else {
                    // 드래그 연결 범위 표시
                    float size = _interactionController.ConnectionDistanceThreshold * 2;
                    Gizmos.DrawWireCube(centerPos, new Vector3(size, 0.1f, size));

                    // 현재 마우스까지의 선
                    Vector3 mouseWorldPos = GetWorldPositionFromMouse();
                    if (mouseWorldPos != Vector3.zero) {
                        Gizmos.DrawLine(centerPos, mouseWorldPos);
                    }
                }
            }
        }

        // 마우스 위치 다시 계산 (InteractionController의 private 메서드를 쓸 수 없으므로 간단히 재구현)
        // 또는 InteractionController에 public 프로퍼티를 만들어서 가져와도 됨.
        // 여기서는 Gizmo용으로 간단히 Raycast 수행
        private Vector3 GetWorldPositionFromMouse() {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            if (ground.Raycast(ray, out float enter)) {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }
    }
}