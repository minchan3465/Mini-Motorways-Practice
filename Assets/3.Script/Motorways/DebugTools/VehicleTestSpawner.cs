using UnityEngine;
using UnityEngine.InputSystem; //New Input System ���
using System.Collections.Generic;

namespace Motorways.DebugTools {
    using Motorways.Managers;
    using Motorways.Models;
    using Motorways.Process;
    using Motorways.Navigation;
    using Motorways.Actions; //InteractionController ���ٿ�

    public class VehicleTestSpawner : MonoBehaviour {
        //[Header("Settings")]
        //public GameObject VehiclePrefab;
        //public InteractionController InputController;

        ////������ ���� ������ ���� ����
        //private bool _hasStartPos = false;
        //private Vector2Int _savedStartPos;

        //private void Update() {
        //   //New Input System�� �̿��� KŰ �Է� ó��
        //   if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame) {
        //       HandleInput();
        //   }
        //}

        //private void HandleInput() {
        //   if (InputController == null || !InputController.IsPointerValid) return;

        //   Vector2Int currentPos = InputController.CurrentGridPointer;
        //   TileData tile = MapManager.Instance.GetTileData(currentPos);

        //   //1. �ش� Ÿ�Ͽ� ���ΰ� �ִ��� Ȯ��
        //   if (tile == null) return;

        //   bool hasRoad = false;
        //   if (tile.Lanes != null) {
        //       foreach (var lane in tile.Lanes) {
        //           if (lane != null) {
        //               hasRoad = true;
        //               break;
        //           }
        //       }
        //   }

        //   if (!hasRoad) {
        //       Debug.LogWarning($"({currentPos}) ��ġ���� ���ΰ� �����ϴ�.");
        //       return;
        //   }

        //   //2. �Է� ���¿� ���� �б� ó�� (������ ���� -> ������ ����)
        //   if (!_hasStartPos) {
        //       //ù ��° KŰ �Է�: ������ ����
        //       _savedStartPos = currentPos;
        //       _hasStartPos = true;
        //       Debug.Log($"[1/2] ������ ������: {_savedStartPos}. ���� ���������� K�� ��������.");
        //   } else {
        //       //�� ��° KŰ �Է�: ������ ���� �� ����
        //       if (currentPos == _savedStartPos) {
        //           Debug.LogWarning("�������� �������� �����ϴ�. �ٸ� ���� ����ּ���.");
        //           return;
        //       }

        //       SpawnTestVehicle(_savedStartPos, currentPos);

        //       //���� �׽�Ʈ�� ���� ���� �ʱ�ȭ
        //       _hasStartPos = false;
        //   }
        //}

        //private void SpawnTestVehicle(Vector2Int startPos, Vector2Int targetPos) {
        //   //3. ���� ���� �� ����
        //   GameObject go;
        //   if (VehiclePrefab != null) go = Instantiate(VehiclePrefab);
        //   else {
        //       go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //       go.transform.localScale = Vector3.one * 0.3f;
        //   }

        //   //��ġ �ʱ�ȭ (���� �ڵ��� targetPos.y ��Ÿ�� startPos.y�� ����)
        //   go.transform.position = new Vector3(startPos.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, startPos.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);

        //   Vehicle newVehicle = go.GetComponent<Vehicle>();
        //   if (newVehicle == null) newVehicle = go.AddComponent<Vehicle>();

        //   //4. �Ŵ��� ��� �� ���� (��ǥ�� �ѱ�)
        //   //��ã��� VehiclePathfindingProcess�� VehicleState.Ready ���¸� �����Ͽ� �˾Ƽ� �����մϴ�.
        //   VehicleMovementProcess.Instance.RegisterVehicle(newVehicle);
        //   newVehicle.Dispatch(startPos, targetPos);

        //   Debug.Log($"[2/2] ���� {newVehicle.Id} ��ġ �Ϸ�! ({startPos} -> {targetPos}) ��� Ž�� �ý����� ������ �����մϴ�.");
        //}

        ////���õ� �������� Scene View���� �ð������� Ȯ���ϱ� ���� Gizmo
        //private void OnDrawGizmos() {
        //   if (_hasStartPos) {
        //       Gizmos.color = Color.green;
        //       Vector3 pos = new Vector3(_savedStartPos.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0.5f, _savedStartPos.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
        //       Gizmos.DrawWireSphere(pos, 0.4f);
        //       Gizmos.DrawLine(pos, pos + Vector3.up * 2);
        //   }
        //}
    }
}