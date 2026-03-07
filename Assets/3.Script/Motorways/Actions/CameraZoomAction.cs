using UnityEngine;
using Motorways.Views;

namespace Motorways.Actions {
	public class CameraZoomAction : MotorwaysPlayerAction {
		private float _scrollDelta;
		private Camera _camera;

		public void SetScrollDelta(float delta) {
			_scrollDelta = delta;
		}

		public override void Initialize(InteractionController controller) {
			base.Initialize(controller);
			_camera = controller.MainCamera;
		}

		public override void OnActionBegin(float timestamp) {
			if (_camera == null || Mathf.Abs(_scrollDelta) < 0.01f) {
				OnActionComplete();
				return;
			}

			// 줌 시 그리드 뷰 노출
			if (GridView.Instance != null) {
				GridView.Instance.SetVisible(true, false);
			}

			if (_camera != null && _camera.orthographic) {
				float zoomStep = 1.0f;
				float minSize = 3.0f;
				float maxSize = 15.0f;

				float targetSize = _camera.orthographicSize - (Mathf.Sign(_scrollDelta) * zoomStep);
				_camera.orthographicSize = Mathf.Clamp(targetSize, minSize, maxSize);
			}

			OnActionComplete();
		}
	}
}
