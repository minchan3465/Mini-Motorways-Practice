using UnityEngine;
using Motorways.Actions;
using Motorways.Managers;
using Motorways.Models;

namespace Motorways.Process {
	public class ExpansionProcess : MonoBehaviour, ISimulationProcess {
		public static ExpansionProcess Instance;

		public ExpansionModel Model { get; private set; }

		private Camera _mainCamera;

		private void Awake() {
			if (Instance == null) {
				Instance = this;
				Model = new ExpansionModel();
				Model.Reset();
			} else {
				Destroy(gameObject);
			}
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
			_mainCamera = Camera.main;
		}

		private void OnDestroy() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RemoveProcess(this);
			}
		}

		public void Tick(float dt) {
			if (ClockProcess.Instance == null || MapManager.Instance == null) return;
			
			// 게임 오버 연출 중에는 카메라 동기화를 멈춰 DOTween과 충돌(축소 현상)하는 것을 방지합니다.
			if (GameOverManager.Instance != null && GameOverManager.Instance.IsSequenceActive) return;

			ClockModel clockModel = ClockProcess.Instance.Model;
			float currentDays = clockModel.ExpansionTime / ClockModel.SecondsPerDay;

			// 1. 카메라 줌 크기 계산 (원작의 City.GetCameraSizeAtTime 방식)
			float targetZoom = CalculateZoomAtTime(currentDays);
			Model.CurrentTargetZoom = targetZoom;

			// 2. 플레이 영역 확장 업데이트 (Zoom에 비례)
			UpdatePlayableArea(targetZoom);

			// 3. 카메라 실시간 동기화 (줌 아웃 상태일 때만)
			SyncCameraWithExpansion(targetZoom);
		}

		private float CalculateZoomAtTime(float days) {
			if (days <= Model.DelayDays) return Model.StartSize;
			if (days >= Model.DelayDays + Model.DurationDays) return Model.EndSize;

			// 선형 보간 (원작은 AnimationCurve를 쓰지만 여기선 기본적인 선형 보간 사용)
			float t = (days - Model.DelayDays) / Model.DurationDays;
			return Mathf.Lerp(Model.StartSize, Model.EndSize, t);
		}

		private void SyncCameraWithExpansion(float zoom) {
			// 기준값은 항상 업데이트
			MapManager.Instance.orthographicSizeValue = zoom;

			if (_mainCamera == null) return;

			var controller = InteractionController.Instance;
			if (controller == null) return;

			// 플레이어가 확대해서 보고 있지 않고, 수동 줌/팬 애니메이션 중이 아닐 때만 자동 추적
			if (!controller.IsPlayerZoomedIn && !controller.IsZoomingActionActive()) {
				// 줌 크기 부드럽게 동기화
				_mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, zoom, 0.05f);

				// 맵 중앙점 부드럽게 추적 (중앙값 변경 대응)
				Vector3 targetCenter = GetSmoothMapCenter();
				_mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, targetCenter, 0.05f);
			}
		}

		public Vector3 GetSmoothMapCenter() {
			float progress = Mathf.InverseLerp(Model.StartSize, Model.EndSize, Model.CurrentTargetZoom);

			RectInt initial = Model.InitialPlayableArea;
			RectInt max = Model.MaxPlayableArea;

			float x = Mathf.Lerp(initial.x, max.x, progress);
			float y = Mathf.Lerp(initial.y, max.y, progress);
			float width = Mathf.Lerp(initial.width, max.width, progress);
			float height = Mathf.Lerp(initial.height, max.height, progress);

			float centerX = (x + width * 0.5f) * MapSettings.TILE_SIZE;
			float centerZ = (y + height * 0.5f) * MapSettings.TILE_SIZE;
			
			float currentY = (_mainCamera != null) ? _mainCamera.transform.position.y : 10f;
			return new Vector3(centerX, currentY, centerZ);
		}

		private void UpdatePlayableArea(float zoom) {
			// 줌 진행도 (0~1)
			float progress = Mathf.InverseLerp(Model.StartSize, Model.EndSize, zoom);

			// 진행도에 맞춰 RectInt 확장
			RectInt initial = Model.InitialPlayableArea;
			RectInt max = Model.MaxPlayableArea;

			int x = (int)Mathf.Lerp(initial.x, max.x, progress);
			int y = (int)Mathf.Lerp(initial.y, max.y, progress);
			int width = (int)Mathf.Lerp(initial.width, max.width, progress);
			int height = (int)Mathf.Lerp(initial.height, max.height, progress);

			MapManager.Instance.UpdatePlayableArea(new RectInt(x, y, width, height));
		}
	}
}
