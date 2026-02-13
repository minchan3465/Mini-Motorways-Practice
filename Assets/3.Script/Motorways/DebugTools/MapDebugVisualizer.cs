using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Motorways.DebugTools {
    using Motorways.Actions;
    using Motorways.Process;
    using Motorways.Managers;

    public class MapDebugVisualizer : MonoBehaviour {
        [Header("References")]
        [SerializeField] private InteractionController _interactionController;

        [Header("Visualization Settings")]
        public bool ShowGrid = true;
        public bool ShowLanes = true;
        public bool ShowVehicles = true;
        public bool ShowInteraction = true;

        [Range(0.1f, 1f)] public float TileSize = 0.9f;

        // 바닥 겹침(Z-Fighting) 방지용 오프셋
        private Vector3 _offset = new Vector3(0, 0.1f, 0);

        private void OnDrawGizmos() {
            if (!Application.isPlaying) return;
            if (MapManager.Instance == null) return;

            if (ShowGrid) DrawMapData();
            if (ShowLanes) DrawLanes();
            //if (ShowVehicles) DrawVehicles();
            if (ShowInteraction) DrawInteractionVisuals();
        }

        private void DrawMapData() {
            // MapManager._grid가 public Dictionary이므로 바로 접근
            foreach (var kvp in MapManager.Instance._grid) {
                TileData tile = kvp.Value;
                Vector3 center = new Vector3(tile.coordinate.x + 0.5f, 0, tile.coordinate.y + 0.5f); // 오프셋 없이 바닥에

                // [수정됨] 눈이 편한 어두운 그리드
                Gizmos.color = new Color(0, 0, 0, 0.2f);
                if (tile.type != TileLogicType.Empty) {
                    // 타일이 뭔가 있다면 노란색 틴트
                    if (tile.type == TileLogicType.House) Gizmos.color = new Color(1, 1, 0, 0.3f);
                    else if (tile.type == TileLogicType.Destination) Gizmos.color = new Color(1, 0, 0, 0.3f);
                }

                Gizmos.DrawWireCube(center, new Vector3(TileSize, 0, TileSize));
            }
        }

        private void DrawLanes() {
            foreach (var kvp in MapManager.Instance._grid) {
                TileData tile = kvp.Value;

                // TileData에 Lanes 배열이 없으면 패스
                if (tile.Lanes == null) continue;

                for (int i = 0; i < tile.Lanes.Length; i++) {
                    Lane lane = tile.Lanes[i];
                    if (lane == null) continue;

                    Vector3 start = new Vector3(lane.StartNode.x + 0.5f, 0, lane.StartNode.y + 0.5f) + _offset;
                    Vector3 end = new Vector3(lane.EndNode.x + 0.5f, 0, lane.EndNode.y + 0.5f) + _offset;

                    // 상태별 색상
                    Gizmos.color = (lane.State == RoadState.Mothballed) ? Color.red : Color.white;
                    Gizmos.DrawLine(start, end);

                    // 방향 화살표
                    Vector3 dir = (end - start).normalized;
                    Vector3 arrowPos = Vector3.Lerp(start, end, 0.6f);
                    DrawArrow(arrowPos, dir);

#if UNITY_EDITOR
                    DrawLaneInfoLabel(lane, start, end);
#endif
                }
            }
        }

        //private void DrawVehicles() {
        //    if (VehicleMovementProcess.Instance == null) return;

        //    // _activeVehicle 리스트 접근
        //    foreach (var v in VehicleMovementProcess.Instance._activeVehicle) {
        //        if (v == null) continue;
        //        Gizmos.color = Color.blue;
        //        Vector3 pos = v.transform.position;
        //        pos.y = _offset.y + 0.2f;
        //        Gizmos.DrawSphere(pos, 0.25f);
        //    }
        //}

        private void DrawInteractionVisuals() {
            if (_interactionController == null || !_interactionController.IsPointerValid) return;

            // 현재 커서
            Vector2Int currentGrid = _interactionController.CurrentGridPointer;
            Vector3 cursorPos = new Vector3(currentGrid.x + 0.5f, 0, currentGrid.y + 0.5f) + _offset;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(cursorPos, Vector3.one * 0.9f);

            // 드래그 시각화
            if (_interactionController.IsDraggingBuild) {
                Vector2Int lastPtr = _interactionController.LastGridPointer;
                Vector3 startPos = new Vector3(lastPtr.x + 0.5f, 0, lastPtr.y + 0.5f) + _offset;

                if (!_interactionController.HasPassedDeadzone) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(_interactionController.ClickOriginWorldPos + _offset, _interactionController.InitialDragDeadzone);
                } else {
                    float size = _interactionController.ConnectionDistanceThreshold * 2;
                    Gizmos.color = new Color(0, 1, 0, 0.5f); // 연결 가능 범위 (초록)
                    Gizmos.DrawWireCube(startPos, new Vector3(size, 0.1f, size));
                }
            }
        }

        private void DrawArrow(Vector3 pos, Vector3 direction) {
            Gizmos.color = Color.green;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
            Gizmos.DrawRay(pos, right * 0.2f);
            Gizmos.DrawRay(pos, left * 0.2f);
        }

#if UNITY_EDITOR
        private void DrawLaneInfoLabel(Lane lane, Vector3 start, Vector3 end) {
            int count = lane.VehiclesOnLane.Count;
            if (count > 0 || lane.State == RoadState.Mothballed) {
                Vector3 midPoint = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 0.5f;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.cyan;
                style.fontSize = 12;
                style.fontStyle = FontStyle.Bold;
                Handles.Label(midPoint, count.ToString(), style);
            }
        }
#endif
    }
}