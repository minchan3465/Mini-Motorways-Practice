using UnityEngine;
using UnityEngine.InputSystem; // New Input System 사용
using System.Collections.Generic;

namespace Motorways.DebugTools {
    using Motorways.Managers;
    using Motorways.Models;
    using Motorways.Process;
    using Motorways.Navigation;
    using Motorways.Actions; // InteractionController 접근용

    public class VehicleTestSpawner : MonoBehaviour {
        [Header("Settings")]
        public GameObject VehiclePrefab;
        public InteractionController InputController;

        // 시작점 상태 저장을 위한 변수
        private bool _hasStartPos = false;
        private Vector2Int _savedStartPos;

        private void Update() {
            // New Input System을 이용한 K키 입력 처리
            if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame) {
                HandleInput();
            }
        }

        private void HandleInput() {
            if (InputController == null || !InputController.IsPointerValid) return;

            Vector2Int currentPos = InputController.CurrentGridPointer;
            TileData tile = MapManager.Instance.GetTileData(currentPos);

            // 1. 해당 타일에 도로가 있는지 확인
            if (tile == null) return;

            bool hasRoad = false;
            if (tile.Lanes != null) {
                foreach (var lane in tile.Lanes) {
                    if (lane != null) {
                        hasRoad = true;
                        break;
                    }
                }
            }

            if (!hasRoad) {
                Debug.LogWarning($"({currentPos}) 위치에는 도로가 없습니다.");
                return;
            }

            // 2. 입력 상태에 따른 분기 처리 (시작점 지정 -> 목적지 지정)
            if (!_hasStartPos) {
                // 첫 번째 K키 입력: 시작점 저장
                _savedStartPos = currentPos;
                _hasStartPos = true;
                Debug.Log($"[1/2] 시작점 설정됨: {_savedStartPos}. 이제 목적지에서 K를 누르세요.");
            } else {
                // 두 번째 K키 입력: 목적지 설정 및 생성
                if (currentPos == _savedStartPos) {
                    Debug.LogWarning("시작점과 목적지가 같습니다. 다른 곳을 찍어주세요.");
                    return;
                }

                SpawnTestVehicle(_savedStartPos, currentPos);

                // 다음 테스트를 위해 상태 초기화
                _hasStartPos = false;
            }
        }

        private void SpawnTestVehicle(Vector2Int startPos, Vector2Int targetPos) {
            // 3. 차량 생성 및 설정
            GameObject go;
            if (VehiclePrefab != null) go = Instantiate(VehiclePrefab);
            else {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.localScale = Vector3.one * 0.3f;
            }

            // 위치 초기화 (기존 코드의 targetPos.y 오타를 startPos.y로 수정)
            go.transform.position = new Vector3(startPos.x + 0.5f, 0, startPos.y + 0.5f);

            Vehicle newVehicle = go.GetComponent<Vehicle>();
            if (newVehicle == null) newVehicle = go.AddComponent<Vehicle>();

            // 4. 매니저 등록 및 배차 (좌표만 넘김)
            // 길찾기는 VehiclePathfindingProcess가 VehicleState.Ready 상태를 감지하여 알아서 수행합니다.
            VehicleMovementProcess.Instance.RegisterVehicle(newVehicle);
            newVehicle.Dispatch(startPos, targetPos);

            Debug.Log($"[2/2] 차량 {newVehicle.Id} 배치 완료! ({startPos} -> {targetPos}) 경로 탐색 시스템이 연산을 시작합니다.");
        }

        // 선택된 시작점을 Scene View에서 시각적으로 확인하기 위한 Gizmo
        private void OnDrawGizmos() {
            if (_hasStartPos) {
                Gizmos.color = Color.green;
                Vector3 pos = new Vector3(_savedStartPos.x + 0.5f, 0.5f, _savedStartPos.y + 0.5f);
                Gizmos.DrawWireSphere(pos, 0.4f);
                Gizmos.DrawLine(pos, pos + Vector3.up * 2);
            }
        }
    }
}