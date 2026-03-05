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

//       //�ٴ� ��ħ(Z-Fighting) ������ ������
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
//               Vector3 center = new Vector3(tile.coordinate.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, tile.coordinate.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);

//               Gizmos.color = new Color(0, 0, 0, 0.2f);
//               if (tile.type != TileLogicType.Empty) {
//                   if (tile.type == TileLogicType.House) Gizmos.color = new Color(1, 1, 0, 0.3f);
//                   else if (tile.type == TileLogicType.Destination) Gizmos.color = new Color(1, 0, 0, 0.3f);
//               }

//               Gizmos.DrawWireCube(center, new Vector3(TileSize, 0, TileSize));

//               //--- �ǹ� �Ա�(Entrance) �ð�ȭ ---
//               if (tile.type == TileLogicType.House || tile.type == TileLogicType.Destination) {
//                   if (tile.Lanes == null) continue;

//                   //�ǹ��� ����� ����(EntranceLane) ã��
//                   for (int i = 0; i < tile.Lanes.Length; i++) {
//                       Lane entranceLane = tile.Lanes[i];
//                       if (entranceLane != null) {
//                           Vector3 entranceEndPos = new Vector3(entranceLane.EndNode.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, entranceLane.EndNode.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE) + _offset;

//                           //�Ա� ������ ���� ��Ȯ�ϰ� ǥ�� (�þȻ�)
//                           Gizmos.color = Color.cyan;
//                           Gizmos.DrawLine(center + _offset, entranceEndPos);

//                           //���ο� ����Ǿ�� �ϴ� �ٽ� ���(Ÿ�� �߾�)�� ���׶�� ��Ŀ ǥ��
//                           Gizmos.DrawWireSphere(entranceEndPos, 0.2f);
//                       }
//                   }
//               }
//           }
//       }

//       private void DrawLanes() {
//           if (RoadNetworkManager.Instance == null) return;

//           //Ÿ�� ������ �ƴ� RoadNetworkManager�� ��ü ����Ʈ�� ��ȸ�մϴ�.
//           foreach (Lane lane in RoadNetworkManager.Instance.AllLanes) {
//               if (lane == null) continue;

//               Vector3 start = new Vector3(lane.StartNode.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, lane.StartNode.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE) + _offset;
//               Vector3 end = new Vector3(lane.EndNode.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, lane.EndNode.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE) + _offset;

//               //[�ٽ�] InboundVehicles(������)�� �ִ��� Ȯ���մϴ�.
//               int resCount = lane.InboundVehicles.Count;
//               bool isReserved = resCount > 0;

//               if (isReserved) {
//                   Gizmos.color = Color.yellow; //����� ���δ� ����� �׵θ�
//                   Vector3 cross = Vector3.Cross((end - start).normalized, Vector3.up) * 0.15f;
//                   Gizmos.DrawLine(start - cross, end - cross);
//                   Gizmos.DrawLine(start + cross, end + cross);
//               }

//               //Mothballed ���¸� ������, �ƴϸ� ���
//               Gizmos.color = (lane.State == RoadState.Mothballed) ? Color.red : Color.white;
//               Gizmos.DrawLine(start, end);

//#if UNITY_EDITOR
//               //������ ���� �󺧷� ǥ�� (InboundVehicles ��)
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

//       //   //_activeVehicle ����Ʈ ����
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

//           //���� Ŀ��
//           Vector2Int currentGrid = _interactionController.CurrentGridPointer;
//           Vector3 cursorPos = new Vector3(currentGrid.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, currentGrid.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE) + _offset;
//           Gizmos.color = Color.magenta;
//           Gizmos.DrawWireCube(cursorPos, Vector3.one * 0.9f);

//           //�巡�� �ð�ȭ
//           if (_interactionController.IsDraggingBuild) {
//               Vector2Int lastPtr = _interactionController.LastGridPointer;
//               Vector3 startPos = new Vector3(lastPtr.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, lastPtr.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE) + _offset;

//               if (!_interactionController.HasPassedDeadzone) {
//                   Gizmos.color = Color.yellow;
//                   Gizmos.DrawWireSphere(_interactionController.ClickOriginWorldPos + _offset, _interactionController.InitialDragDeadzone);
//               } else {
//                   float size = _interactionController.ConnectionDistanceThreshold * 2;
//                   Gizmos.color = new Color(0, 1, 0, 0.5f); //���� ���� ���� (�ʷ�)
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
//       //[���� 2] �󺧿� ���� ��(Res)�� ����ϵ��� �Ű����� �߰�
//       private void DrawLaneInfoLabel(Lane lane, Vector3 start, Vector3 end, int reservationCount) {
//           if (reservationCount > 0 || lane.State == RoadState.Mothballed) {
//               Vector3 midPoint = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 0.5f;
//               GUIStyle style = new GUIStyle();
//               style.normal.textColor = Color.cyan;
//               style.fontSize = 12;
//               style.fontStyle = FontStyle.Bold;

//               //ȭ�鿡 'Res: ����' ���·� ǥ��
//               UnityEditor.Handles.Label(midPoint, $"Res: {reservationCount}", style);
//           }
//       }
//#endif
    }
}