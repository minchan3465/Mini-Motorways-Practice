using UnityEngine;

namespace Motorways.Actions {
	public class CameraPanAction : MotorwaysPlayerAction {
		private Vector2 _lastMouseScreenPos;
		private Camera _camera;

		public override void Initialize(InteractionController controller) {
			base.Initialize(controller);
			_camera = controller.MainCamera;
		}

		public override void OnActionBegin(float timestamp) {
			if (_camera == null) return;

			// 축소 상태(Size 15)일 때는 화면 이동을 막습니다.
			if (_camera.orthographicSize > 14.9f) {
				OnActionCancel();
				return;
			}

			_lastMouseScreenPos = _controller.MouseScreenPos;
		}

		public override void Tick(float frameTime) {
			if (_camera == null || _isComplete) return;

			Vector2 currentMousePos = _controller.MouseScreenPos;
			Vector2 mouseDelta = currentMousePos - _lastMouseScreenPos;

			if (mouseDelta != Vector2.zero) {
				float screenToWorldScale = _camera.orthographicSize * 2.0f / Screen.height;
				Vector3 worldDelta = new Vector3(mouseDelta.x, 0, mouseDelta.y) * screenToWorldScale;

				Vector3 targetPos = _camera.transform.position - worldDelta;

				// [범위 제한 강화] 가동 범위를 조금 더 좁게 설정 (80% 수준)
				if (MapManager.Instance != null) {
					RectInt area = MapManager.Instance.PlayableArea;
					float tileSize = MapSettings.TILE_SIZE;
					float clampBuffer = 0.8f; 

					float minX = area.xMin * tileSize * clampBuffer;
					float maxX = area.xMax * tileSize * clampBuffer;
					float minZ = area.yMin * tileSize * clampBuffer;
					float maxZ = area.yMax * tileSize * clampBuffer;

					targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
					targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);
				}

				_camera.transform.position = targetPos;
			}

			_lastMouseScreenPos = currentMousePos;
		}
	}
}
