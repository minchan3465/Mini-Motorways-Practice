using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Systems {
    using Core.Data;
    using Core.Structure;
    using Core.Utils;

    public class MapDebugVisualizer : MonoBehaviour {
        [Header("References")]
        [SerializeField] private InteractionController _interactionController;

        // 바닥 겹침(Z-Fighting) 방지용 오프셋
        private Vector3 _offset = new Vector3(0, 0.1f, 0);

        private void OnDrawGizmos() {
            DrawMapData();
            DrawInteractionVisuals();
        }

        // ========================================================================
        // 1. 맵 데이터 시각화
        // ========================================================================
        private void DrawMapData() {
            if (MapBootstrapper.Grid == null) return;

            // A. 타일 타입 그리기
            foreach (var kvp in MapBootstrapper.Grid) {
                CellData cell = kvp.Value;
                if (cell.Type == TileLogicType.Empty) continue;

                Vector3 center = new Vector3(cell.Coordinate.x + 0.5f, 0, cell.Coordinate.y + 0.5f) + _offset;

                switch (cell.Type) {
                    case TileLogicType.Road: Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); break;
                    case TileLogicType.Supply: Gizmos.color = Color.blue; break;
                    case TileLogicType.Demand: Gizmos.color = Color.yellow; break;
                    case TileLogicType.Entrance: Gizmos.color = Color.cyan; break;
                    default: Gizmos.color = Color.white; break;
                }
                Gizmos.DrawWireCube(center, Vector3.one * 0.9f);

                // B. Lane (도로망) 그리기
                if (Application.isPlaying && RoadNetwork.Instance != null) {
                    List<Lane> lanes = RoadNetwork.Instance.GetOutboundLanes(cell.Coordinate);
                    if (lanes != null) {
                        foreach (Lane lane in lanes) {
                            DrawLane(lane);
                        }
                    }
                }
            }
        }

        private void DrawLane(Lane lane) {
            Vector3 s = lane.StartWorldPos + _offset + new Vector3(0, 0.05f, 0);
            Vector3 e = lane.EndWorldPos + _offset + new Vector3(0, 0.05f, 0);

            // 1. 선 그리기
            if (lane.State == LaneState.Mothballed) {
                Gizmos.color = Color.red;
            } else {
                Gizmos.color = Color.white;
            }
            Gizmos.DrawLine(s, e);

            // [추가] 3. 예약된 차량 수 텍스트 표시 (Editor 전용)
#if UNITY_EDITOR
            int count = lane.VehiclesOnLane.Count;

            // 차량이 있거나, 삭제 대기 상태라면 숫자를 표시해서 디버깅 도움
            if (count > 0 || lane.State == LaneState.Mothballed) {
                Vector3 midPoint = Vector3.Lerp(s, e, 0.5f) + Vector3.up * 0.3f; // 선보다 살짝 위

                GUIStyle style = new GUIStyle();
                style.fontSize = 15;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;

                if (lane.State == LaneState.Mothballed) {
                    // 삭제 대기 중인데 차량이 남았다면 빨간 글씨
                    style.normal.textColor = Color.red;
                    Handles.Label(midPoint, $"{count}", style);
                } else {
                    // 일반 도로 위의 차량 수는 흰색 글씨
                    style.normal.textColor = Color.white;
                    Handles.Label(midPoint, $"{count}", style);
                }
            }
#endif
        }

        // ========================================================================
        // 2. 인터랙션 시각화
        // ========================================================================
        private void DrawInteractionVisuals() {
            if (!Application.isPlaying || _interactionController == null) return;
            if (!_interactionController.IsPointerValid) return;

            Vector2Int currentGrid = _interactionController.CurrentGridPointer;

            // A. 마우스 커서
            Vector3 drawPos = new Vector3(currentGrid.x + 0.5f, 0, currentGrid.y + 0.5f) + _offset;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(drawPos, Vector3.one * 0.95f);

            // B. 드래그 라인
            if (_interactionController.IsDraggingBuild) {
                Vector3 startPos;
                Color dragColor;

                if (_interactionController.DragStartHouse != null) {
                    startPos = _interactionController.DragStartHouse.transform.position + _offset;
                    dragColor = Color.green;
                } else {
                    Vector2Int lastPtr = _interactionController.LastGridPointer;
                    startPos = new Vector3(lastPtr.x + 0.5f, 0, lastPtr.y + 0.5f) + _offset;
                    dragColor = Color.yellow;
                }

                Gizmos.color = dragColor;

                if (!_interactionController.HasPassedDeadzone) {
                    Gizmos.DrawWireSphere(_interactionController.ClickOriginWorldPos + _offset, _interactionController.InitialDragDeadzone);
                } else {
                    float size = _interactionController.ConnectionDistanceThreshold * 2;
                    Gizmos.DrawWireCube(startPos, new Vector3(size, 0.1f, size));

                    Vector3 mouseWorldPos = GetWorldPositionFromMouse();
                    if (mouseWorldPos != Vector3.zero) {
                        Gizmos.DrawLine(startPos, mouseWorldPos + _offset);
                    }
                }
            }
        }

        private Vector3 GetWorldPositionFromMouse() {
            Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            if (ground.Raycast(ray, out float enter)) {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }
    }
}