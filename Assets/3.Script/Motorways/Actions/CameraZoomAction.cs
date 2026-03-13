using UnityEngine;
using Motorways.Views;
using DG.Tweening;

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

			if (_camera.orthographic) {
				const float zoomInSize = 7.5f;
				// MapManager.Instance.orthographicSizeValue는 ExpansionProcess에 의해 시간이 지남에 따라 커집니다.
				float zoomOutSize = MapManager.Instance.orthographicSizeValue;
				bool isZoomingIn = _scrollDelta > 0;
				float targetSize = isZoomingIn ? zoomInSize : zoomOutSize;

				_camera.DOKill();
				_camera.transform.DOKill();

				float duration = 0.1f;

				//1. 부드러운 줌 연출
				_camera.DOOrthoSize(targetSize, duration).SetEase(Ease.OutQuad);
				_controller.IsPlayerZoomedIn = isZoomingIn;

				//2. 줌 위치 보정
				if (isZoomingIn) {
					//[마우스 중심 줌] 마우스 월드 좌표 방향으로 카메라를 살짝 이동
					Vector3 mouseWorldPos = _controller.GetWorldPositionFromMouse();
					Vector3 currentCamPos = _camera.transform.position;
					
					//마우스 지점과 카메라 사이의 차이를 보정 (새로운 위치 = 마우스 - (마우스 - 카메라) * (새사이즈 / 기존사이즈))
					Vector3 targetPos = mouseWorldPos - (mouseWorldPos - currentCamPos) * (targetSize / _camera.orthographicSize);
					targetPos.y = currentCamPos.y; //높이는 고정

					_camera.transform.DOMove(targetPos, duration).SetEase(Ease.OutQuad);
				} else {
					//[축소 시 중앙 복귀]
					Vector3 center = _controller.GetMapCenter();
					_camera.transform.DOMove(center, duration).SetEase(Ease.OutQuad);
				}

				//줌 상태에 따른 그리드 노출 제어
				if (GridView.Instance != null) {
					GridView.Instance.SetVisible(isZoomingIn, false);
				}
			}

			OnActionComplete();
		}
	}
}
