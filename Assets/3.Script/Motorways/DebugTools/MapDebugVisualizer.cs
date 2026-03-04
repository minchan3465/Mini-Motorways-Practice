using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Motorways.DebugTools {
    using Motorways.Actions;
    using Motorways.Process;
    using Motorways.Managers;

    public class MapDebugVisualizer : MonoBehaviour {
//       [Header("References")]
//       [SerializeField] private InteractionController _interactionController;

//       [Header("Visualization Settings")]
//       public bool ShowGrid = true;
//       public bool ShowLanes = true;
//       public bool ShowVehicles = true;
//       public bool ShowInteraction = true;

//       [Range(0.1f, 1f)] public float TileSize = 0.9f;

//       //바닥 겹침(Z-Fighting) 방지용 오프셋
//       private Vector3 _offset = new Vector3(0, 0.1f, 0);

//       private void OnDrawGizmos() {
//           if (!Application.isPlaying) return;
//           if (MapManager.Instance == null) return;

//           if (ShowGrid) DrawMapData();
//           if (ShowLanes) DrawLanes();
//           //if (ShowVehicles) DrawVehicles();
//           if (ShowInteraction) DrawInteractionVisuals();
//       }

//       private void DrawMapData() {
//           foreach (var kvp in MapManager.Instance._grid) {
//               TileData tile = kvp.Value;
//               Vector3 center = new Vector3(tile.coordinate.x + 0.5f, 0, tile.coordinate.y + 0.5f);

//               Gizmos.color = new Color(0, 0, 0, 0.2f);
//               if (tile.type != TileLogicType.Empty) {
//                   if (tile.type == TileLogicType.House) Gizmos.color = new Color(1, 1, 0, 0.3f);
//                   else if (tile.type == TileLogicType.Destination) Gizmos.color = new Color(1, 0, 0, 0.3f);
//               }

//               Gizmos.DrawWireCube(center, new Vector3(TileSize, 0, TileSize));

//               //--- 건물 입구(Entrance) 시각화 ---
//               if (tile.type == TileLogicType.House || tile.type == TileLogicType.Destination) {
//                   if (tile.Lanes == null) continue;

//                   //건물에 연결된 차선(EntranceLane) 찾기
//                   for (int i = 0; i < tile.Lanes.Length; i++) {
//                       Lane entranceLane = tile.Lanes[i];
//                       if (entranceLane != null) {
//                           Vector3 entranceEndPos = new Vector3(entranceLane.EndNode.x + 0.5f, 0, entranceLane.EndNode.y + 0.5f) + _offset;

//                           //입구 방향을 굵고 명확하게 표시 (시안색)
//                           Gizmos.color = Color.cyan;
//                           Gizmos.DrawLine(center + _offset, entranceEndPos);

//                           //도로와 연결되어야 하는 핵심 노드(타일 중앙)에 동그라미 마커 표시
//                           Gizmos.DrawWireSphere(entranceEndPos, 0.2f);
//                       }
//                   }
//               }
//           }
//       }

//       private void DrawLanes() {
//           if (RoadNetworkManager.Instance == null) return;

//           //타일 기준이 아닌 RoadNetworkManager의 전체 리스트를 순회합니다.
//           foreach (Lane lane in RoadNetworkManager.Instance.AllLanes) {
//               if (lane == null) continue;

//               Vector3 start = new Vector3(lane.StartNode.x + 0.5f, 0, lane.StartNode.y + 0.5f) + _offset;
//               Vector3 end = new Vector3(lane.EndNode.x + 0.5f, 0, lane.EndNode.y + 0.5f) + _offset;

//               //[핵심] InboundVehicles(예약자)가 있는지 확인합니다.
//               int resCount = lane.InboundVehicles.Count;
//               bool isReserved = resCount > 0;

//               if (isReserved) {
//                   Gizmos.color = Color.yellow; //예약된 도로는 노란색 테두리
//                   Vector3 cross = Vector3.Cross((end - start).normalized, Vector3.up) * 0.15f;
//                   Gizmos.DrawLine(start - cross, end - cross);
//                   Gizmos.DrawLine(start + cross, end + cross);
//               }

//               //Mothballed 상태면 빨간색, 아니면 흰색
//               Gizmos.color = (lane.State == RoadState.Mothballed) ? Color.red : Color.white;
//               Gizmos.DrawLine(start, end);

//#if UNITY_EDITOR
//               //예약자 수를 라벨로 표시 (InboundVehicles 수)
//               if (resCount > 0 || lane.State == RoadState.Mothballed) {
//                   Vector3 midPoint = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 0.5f;
//                   Handles.Label(midPoint, $"Res: {resCount}", new GUIStyle {
//                       normal = { textColor = Color.yellow },
//                       fontStyle = FontStyle.Bold
//                   });
//               }
//#endif
//           }
//       }

//       //private void DrawVehicles() {
//       //   if (VehicleMovementProcess.Instance == null) return;

//       //   //_activeVehicle 리스트 접근
//       //   foreach (var v in VehicleMovementProcess.Instance._activeVehicle) {
//       //       if (v == null) continue;
//       //       Gizmos.color = Color.blue;
//       //       Vector3 pos = v.transform.position;
//       //       pos.y = _offset.y + 0.2f;
//       //       Gizmos.DrawSphere(pos, 0.25f);
//       //   }
//       //}

//       private void DrawInteractionVisuals() {
//           if (_interactionController == null || !_interactionController.IsPointerValid) return;

//           //현재 커서
//           Vector2Int currentGrid = _interactionController.CurrentGridPointer;
//           Vector3 cursorPos = new Vector3(currentGrid.x + 0.5f, 0, currentGrid.y + 0.5f) + _offset;
//           Gizmos.color = Color.magenta;
//           Gizmos.DrawWireCube(cursorPos, Vector3.one * 0.9f);

//           //드래그 시각화
//           if (_interactionController.IsDraggingBuild) {
//               Vector2Int lastPtr = _interactionController.LastGridPointer;
//               Vector3 startPos = new Vector3(lastPtr.x + 0.5f, 0, lastPtr.y + 0.5f) + _offset;

//               if (!_interactionController.HasPassedDeadzone) {
//                   Gizmos.color = Color.yellow;
//                   Gizmos.DrawWireSphere(_interactionController.ClickOriginWorldPos + _offset, _interactionController.InitialDragDeadzone);
//               } else {
//                   float size = _interactionController.ConnectionDistanceThreshold * 2;
//                   Gizmos.color = new Color(0, 1, 0, 0.5f); //연결 가능 범위 (초록)
//                   Gizmos.DrawWireCube(startPos, new Vector3(size, 0.1f, size));
//               }
//           }
//       }

//       private void DrawArrow(Vector3 pos, Vector3 direction) {
//           Gizmos.color = Color.green;
//           Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
//           Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
//           Gizmos.DrawRay(pos, right * 0.2f);
//           Gizmos.DrawRay(pos, left * 0.2f);
//       }

//#if UNITY_EDITOR
//       //[수정 2] 라벨에 예약 수(Res)를 출력하도록 매개변수 추가
//       private void DrawLaneInfoLabel(Lane lane, Vector3 start, Vector3 end, int reservationCount) {
//           if (reservationCount > 0 || lane.State == RoadState.Mothballed) {
//               Vector3 midPoint = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 0.5f;
//               GUIStyle style = new GUIStyle();
//               style.normal.textColor = Color.cyan;
//               style.fontSize = 12;
//               style.fontStyle = FontStyle.Bold;

//               //화면에 'Res: 숫자' 형태로 표시
//               UnityEditor.Handles.Label(midPoint, $"Res: {reservationCount}", style);
//           }
//       }
//#endif
    }
}