using UnityEngine;
using System.Collections.Generic;

namespace Motorways.DebugTools {
    using Motorways.Managers;
    using Motorways.Models;
    using Motorways.Process;
    using Motorways.Navigation;
    using Motorways.Actions; // InteractionController 접근용

    public class VehicleTestSpawner : MonoBehaviour {
        [Header("Settings")]
        public GameObject VehiclePrefab; // 차량 프리팹 (없으면 Sphere라도 넣으세요)
        public InteractionController InputController; // 마우스 좌표 얻기용

        private void Update() {
            // K키를 누르면 테스트 차량 소환
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) {
                SpawnTestVehicle();
            }
        }

        private void SpawnTestVehicle() {
            if (InputController == null || !InputController.IsPointerValid) return;

            Vector2Int spawnPos = InputController.CurrentGridPointer;
            TileData startTile = MapManager.Instance.GetTileData(spawnPos);

            // 1. 해당 타일에 도로가 있는지 확인
            if (startTile == null) return;

            // 아무 연결된 도로 하나 찾기
            Lane startLane = null;
            foreach (var lane in startTile.Lanes) {
                if (lane != null) {
                    startLane = lane;
                    break;
                }
            }

            if (startLane == null) {
                Debug.Log("여기엔 도로가 없어서 차를 못 뽑습니다.");
                return;
            }

            // 2. 목적지 찾기 (그냥 랜덤한 다른 도로 타일)
            // (실제 게임에선 집->회사겠지만, 지금은 테스트니까)
            Vector2Int targetPos = spawnPos + new Vector2Int(3, 0); // 동쪽으로 3칸 떨어진 곳 가정
            if (MapManager.Instance.GetTileData(targetPos) == null) {
                Debug.Log("목적지가 유효하지 않아 임시로 (0,0)을 목적지로 잡습니다.");
                targetPos = new Vector2Int(0, 0);
            }

            // 3. 경로 탐색
            List<Lane> path = Pathfinder.FindPath(spawnPos, targetPos);
            if (path == null || path.Count == 0) {
                Debug.Log("길을 찾을 수 없습니다!");
                return;
            }

            // 4. 차량 생성 및 설정
            GameObject go = null;
            if (VehiclePrefab != null) {
                go = Instantiate(VehiclePrefab);
            } else {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere); // 프리팹 없으면 공 생성
                go.transform.localScale = Vector3.one * 0.5f;
            }

            Vehicle newVehicle = go.AddComponent<Vehicle>(); // 또는 이미 붙어있다면 GetComponent
            if (newVehicle == null) newVehicle = go.GetComponent<Vehicle>();

            // 초기 위치 설정
            go.transform.position = new Vector3(spawnPos.x, 0, spawnPos.y);

            // 매니저 등록
            VehicleMovementProcess.Instance.RegisterVehicle(newVehicle);

            // 출발!
            // (집으로 돌아오는 경로는 일단 비워둠)
            newVehicle.Dispatch(path, new List<Lane>());

            Debug.Log($"차량 {newVehicle.Id} 생성 완료! 출발합니다.");
        }
    }
}