using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Motorways.UI {
	public class TimerBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
		private TimerVisualManager manager;
		private RawImage buttonImage;

		[Header("Alpha Settings")]
		[Range(0f, 1f)] public float normalAlpha = 1f;
		[Range(0f, 1f)] public float selectedAlpha = 0.5f;

		[Header("Alpha Settings")]
		[Range(0, 2)] public float timeScale;

		private Vector3 originalScale;
		public bool isSelected = false;

		private Sequence clickSequence;

		private void Awake() {
			TryGetComponent(out buttonImage);
			originalScale = transform.localScale;

		}

		public void Setup(TimerVisualManager manager) {
			this.manager = manager;
		}

		public void OnPointerClick(PointerEventData eventData) {
			if (isSelected) return;

			SoundManager.Instance.PlaySFX(SoundEffect.ButtonMain);
			// 매니저에게 자신이 클릭되었음을 알림
			manager.OnButtonClicked(this, timeScale);

		}

		public void OnPointerEnter(PointerEventData eventData) {
			if (isSelected) return;
			// 마우스 올렸을 때 살짝 커짐
			transform.DOScale(originalScale * 1.2f, 0.3f).SetEase(Ease.OutQuad);
		}

		public void OnPointerExit(PointerEventData eventData) {
			if (isSelected) return;

			// 마우스가 나가면 원래 크기로
			transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutQuad);
		}

		public void SelectButton() {
			isSelected = true;

			transform.DOKill();
			buttonImage.DOKill();
			if (clickSequence != null && clickSequence.IsActive()) clickSequence.Kill();

			clickSequence = DOTween.Sequence();
			clickSequence.Append(transform.DOScale(originalScale * 1.1f, 0.1f).SetEase(Ease.OutQuad))
						 .Append(transform.DOScale(originalScale * 0.9f, 0.15f).SetEase(Ease.InOutQuad));

			// 색상 변경
			buttonImage.DOFade(selectedAlpha, 0.2f);
		}

		public void DeselectButton() {
			isSelected = false;

			transform.DOKill();
			buttonImage.DOKill();
			if (clickSequence != null && clickSequence.IsActive()) clickSequence.Kill();

			transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutQuad);
			buttonImage.DOFade(normalAlpha, 0.2f);
		}
	}
}
