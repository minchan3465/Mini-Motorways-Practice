using UnityEngine;
using Motorways.Managers;

namespace Motorways.Actions {
    public class RemoveRoadAction : MotorwaysPlayerAction {
        private Vector2Int _lastGridPointer;    // 마지막으로 삭제가 일어난 타일 좌표

        public override void OnActionBegin(float timestamp) {
            _lastGridPointer = _controller.CurrentGridPointer;
            
            // 시작 지점 도로 삭제 시도
            RoadNetworkManager.Instance.TryRemoveRoad(_lastGridPointer);
        }

        public override void Tick(float frameTime) {
            if (_isComplete) return;

            Vector2Int currentPos = _controller.CurrentGridPointer;
            
            // 드래그하는 동안 마우스가 위치한 타일이 바뀌면 삭제 시도
            if (currentPos != _lastGridPointer) {
                RoadNetworkManager.Instance.TryRemoveRoad(currentPos);
                _lastGridPointer = currentPos;
            }
        }
    }
}
