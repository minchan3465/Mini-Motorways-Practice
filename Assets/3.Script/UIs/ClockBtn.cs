using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Motorways.UI {
	public class ClockBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		[SerializeField] private RectTransform pauseBtn;
		[SerializeField] private RectTransform playBtn;
		[SerializeField] private RectTransform fasterBtn;

		[Header("Settings")]
		[SerializeField] private float moveDistance = 65f; // 내려갈 거리
		[SerializeField] private float btnDistance = 50f; // 버튼 간격
		[SerializeField] private float duration = 0.4f;     // 애니메이션 시간

		//펀치
		[SerializeField] private float targetScale = 1.15f;
		[SerializeField] private float punchAmount = 0.15f;
		[SerializeField] private float punchDuration = 0.2f;

		private bool isDown = false;      // 현재 내려가 있는 상태인지
		private bool isAnimating = false; // 이동 애니메이션 중인지

		public void ToggleTimerBtn() {
			transform.DOKill(true);
			transform.DOPunchScale(Vector3.one * -punchAmount, punchDuration / 2f, 1, 0f).SetLoops(2, LoopType.Restart);

			if (isAnimating) return;

			isAnimating = true;
			Sequence seq = DOTween.Sequence();

			Vector2 originPos = Vector2.zero;

			if (!isDown) {
				//시계 클릭으로 인하여, 버튼 나오는 ClockOpen
				SoundManager.Instance.PlaySFX(SoundEffect.ClockOpen);

				seq.Append(AnimateOutMove(pauseBtn, duration, originPos + Vector2.down * moveDistance));
				seq.Join(AnimateOutMove(playBtn, duration - 0.05f, originPos + Vector2.down * (moveDistance + btnDistance), 0.3f));
				seq.Join(AnimateOutMove(fasterBtn, duration - 0.1f, originPos + Vector2.down * (moveDistance + btnDistance * 2), 0.35f));
			} else {
				//시계 클릭으로 인하여, 버튼 사라지는 ClockClose
				SoundManager.Instance.PlaySFX(SoundEffect.ClockClose);

				seq.Append(AnimateInMove(pauseBtn, duration, originPos, 0.05f));
				seq.Join(AnimateInMove(playBtn,duration - 0.05f, originPos, 0.15f));
				seq.Join(AnimateInMove(fasterBtn, duration - 0.1f, originPos, 0.17f));
			}

			seq.OnComplete(() => {
				isDown = !isDown;
				isAnimating = false;
			});
		}

		private Tween AnimateOutMove(RectTransform target, float duration, Vector2 targetPos, float delay = 0) {
			return target.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad).SetDelay(delay);
		}

		private Tween AnimateInMove(RectTransform target, float duration, Vector2 targetPos, float delay = 0) {
			return target.DOAnchorPos(targetPos, duration).SetEase(Ease.InQuad).SetDelay(delay);
		}


		public void OnPointerEnter(PointerEventData eventData) {
			transform.DOKill(true);
			transform.DOScale(targetScale, 0.05f).SetEase(Ease.OutQuad);
			transform.DOPunchScale(Vector3.one * -punchAmount, punchDuration / 2f, 1, 0f).SetLoops(2, LoopType.Restart);
		}

		public void OnPointerExit(PointerEventData eventData) {
			transform.DOScale(1f, 0.05f).SetEase(Ease.OutQuad);
		}
	}
}

