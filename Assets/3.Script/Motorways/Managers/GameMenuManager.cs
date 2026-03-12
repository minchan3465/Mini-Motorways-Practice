using UnityEngine;
using Motorways.Managers;
using DG.Tweening;

namespace Motorways.Managers {
	/// <summary>
	/// 게임 메뉴 UI와 관련된 상태 및 참조를 관리하는 매니저입니다.
	/// DOTween을 사용하여 페이드 인/아웃 효과를 제공합니다.
	/// </summary>
	public class GameMenuManager : MonoBehaviour {
		public static GameMenuManager Instance { get; private set; }

		[Header("Menu UI References")]
		public GameObject PausePanel;
		public GameObject OtherUI1;
		public GameObject OtherUI2;
		public GameObject OtherUI3;

		[Header("Fade Settings")]
		[SerializeField] private float _fadeDuration = 0.3f;

		private CanvasGroup _pauseCanvasGroup;
		public bool IsMenuOpen => PausePanel != null && PausePanel.activeSelf;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			if (PausePanel != null) {
				PausePanel.TryGetComponent(out _pauseCanvasGroup);
				if (_pauseCanvasGroup == null) _pauseCanvasGroup = PausePanel.AddComponent<CanvasGroup>();

				PausePanel.SetActive(false);
				_pauseCanvasGroup.alpha = 0f;
			}
		}

		// 메뉴 UI를 토글하고 게임 일시정지 상태를 제어합니다.
		public void ToggleMenu() {
			if (PausePanel == null) return;

			bool isOpening = !PausePanel.activeSelf;

			if (isOpening) {
				SoundManager.Instance.PlaySFX(SoundEffect.ButtonMain);

				WhiteoutController.Instance.OnWhiteOut();

				// 메뉴 열기
				PausePanel.SetActive(true);
				_pauseCanvasGroup.DOKill();
				_pauseCanvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true);

				// 다른 UI들 숨기기
				SetOtherUIsActive(false);

				//전체 시간 정지
				Time.timeScale = 0f;

			} else {
				SoundManager.Instance.PlaySFX(SoundEffect.ButtonSub);
				WhiteoutController.Instance.OffWhiteOut();

				// 메뉴 닫기
				_pauseCanvasGroup.DOKill();
				_pauseCanvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).OnComplete(() => {
					PausePanel.SetActive(false);
					Time.timeScale = 1f;
				});


				// 다른 UI들 다시 표시
				SetOtherUIsActive(true);
			}
		}

		private void SetOtherUIsActive(bool active) {
			if (OtherUI1 != null) OtherUI1.SetActive(active);
			if (OtherUI2 != null) OtherUI2.SetActive(active);
			if (OtherUI3 != null) OtherUI3.SetActive(active);
		}
	}
}
