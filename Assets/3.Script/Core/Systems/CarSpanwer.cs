using UnityEngine;
using UnityEngine.InputSystem; // 필수
using System.Collections.Generic;

namespace Core.Systems {
    public class CarSpawner : MonoBehaviour {
        [Header("References")]
        [SerializeField] private GameObject _carPrefab;
        [SerializeField] private InteractionController _interaction;

        private PlayerInput _input; // C# Class
        private Vector2Int _startNode;
        private bool _isSelectingDestination = false;

        private void Awake() {
            _input = new PlayerInput();
        }

        private void OnEnable() {
            _input.Enable();
            // Input Actions에 'Test' 액션을 만들었다고 가정 (단축키 T)
            _input.Player.Test.performed += OnTestPerformed;
        }

        private void OnDisable() {
            _input.Disable();
            _input.Player.Test.performed -= OnTestPerformed;
        }

        private void OnTestPerformed(InputAction.CallbackContext context) {
            if (_interaction == null || !_interaction.IsPointerValid) return;

            Vector2Int currentPos = _interaction.CurrentGridPointer;

            if (!_isSelectingDestination) {
                // 1단계: 출발지 설정
                _startNode = currentPos;
                _isSelectingDestination = true;
                Debug.Log($"출발지 설정됨: {_startNode}. 목적지를 가리키고 T를 다시 누르세요.");
            } else {
                // 2단계: 목적지 설정 및 출발
                Debug.Log($"목적지 설정됨: {currentPos}. 경로 탐색 시작...");
                SpawnCar(_startNode, currentPos);
                _isSelectingDestination = false;
            }
        }

        private void SpawnCar(Vector2Int start, Vector2Int end) {
            List<Vector2Int> path = Pathfinder.FindPath(start, end);

            if (path == null || path.Count == 0) {
                Debug.LogError("경로를 찾을 수 없습니다! (도로 연결 확인 필요)");
                return;
            }

            Debug.Log($"경로 확인! 이동 거리: {path.Count}칸");

            Vector3 spawnWorldPos = new Vector3(start.x + 0.5f, 0, start.y + 0.5f);
            GameObject carObj = Instantiate(_carPrefab, spawnWorldPos, Quaternion.identity);

            CarMovement car = carObj.GetComponent<CarMovement>();
            if (car != null) {
                // 집 위치를 알려줌 (나중에 돌아오기 위해)
                car.Initialize(start);
                car.SetPath(path);
            }
        }
    }
}