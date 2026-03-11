using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Motorways.UI {
	public class Inv_Bridge : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI Bridge_Text;
		[SerializeField] private RawImage Img;
		[SerializeField] private RawImage ImgBg;
		[SerializeField] private RawImage TextBg;

		[SerializeField] private Color black;
		[SerializeField] private Color white;

		// 애니메이션 설정값
		private float tweenDuration = 0.2f;

		// 상태 추적용 변수
		private int previousCount = 0;
		private Vector3 originalImgScale;
		private Vector3 originalTextBgScale;

		private void Awake() {
			// 인스펙터에 설정된 원래 크기를 저장해둡니다.
			originalImgScale = Img.rectTransform.localScale;
			originalTextBgScale = TextBg.rectTransform.localScale;
		}

		public void ChangeRoadCount() {
			int currentCount = ResourceManager.Instance.GetCount(ItemType.Bridge);

			// 텍스트는 항상 최신화
			Bridge_Text.text = currentCount.ToString();

			// 0에서 1 이상으로 늘어났을 때
			if (previousCount == 0 && currentCount > 0) {
				// 빠르게 연속 호출될 경우를 대비해 진행 중인 트윈 취소
				Img.DOKill();
				TextBg.DOKill();

				// 텍스트 활성화
				Bridge_Text.gameObject.SetActive(true);

				// 색상 변경 (black -> white)
				Img.DOColor(white, tweenDuration);

				// 크기 0에서 기존 크기로 전환 (살짝 튕기는 연출을 원하시면 SetEase(Ease.OutBack) 추가)
				ImgBg.rectTransform.localScale = Vector3.zero;
				ImgBg.rectTransform.DOScale(originalImgScale, tweenDuration);

				TextBg.rectTransform.localScale = Vector3.zero;
				TextBg.rectTransform.DOScale(originalTextBgScale, tweenDuration);
			}
			// 1 이상이었다가 0으로 줄어들었을 때
			else if (previousCount > 0 && currentCount == 0) {
				Img.DOKill();
				TextBg.DOKill();

				// 색상 변경 (white -> black)
				Img.DOColor(black, tweenDuration);

				// 기존 크기에서 0으로 전환
				ImgBg.rectTransform.DOScale(Vector3.zero, tweenDuration);
				TextBg.rectTransform.DOScale(Vector3.zero, tweenDuration);

				// 텍스트 비활성화 (보이지 않게 처리)
				Bridge_Text.gameObject.SetActive(false);
			}

			// 현재 갯수를 이전 갯수로 갱신
			previousCount = currentCount;
		}
	}
}