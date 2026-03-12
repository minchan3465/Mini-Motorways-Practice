using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Motorways.Managers;
using DG.Tweening;

namespace Motorways.UI {

	public class RewardBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		[SerializeField] private float fadeDuration = 0.2f; // 인스펙터에서 전환 속도 조절 가능
		[SerializeField] private Color yellow;


		private Dictionary<RawImage, Color> originalColors = new Dictionary<RawImage, Color>();

		private void Awake() {
			RawImage[] childImages = GetComponentsInChildren<RawImage>();

			foreach (RawImage img in childImages) {
				originalColors.Add(img, img.color);
			}
		}

		public void SelectReward(int option) {
			WeeklyRewardManager.Instance.SelectReward(option);
		}

		public void OnPointerEnter(PointerEventData eventData) {
			SoundEffect SelectRnd = Random.Range(0, 3) + SoundEffect.SelecteSE1;
			SoundManager.Instance.PlaySFX(SelectRnd);
			foreach (RawImage img in originalColors.Keys) {
				if (img != null) {
					// 애니메이션 충돌 방지를 위해 기존 진행 중인 트윈을 먼저 종료
					img.DOKill();
					// 지정한 시간 동안 노란색으로 부드럽게 전환
					img.DOColor(yellow, fadeDuration);
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData) {
			foreach (KeyValuePair<RawImage, Color> item in originalColors) {
				if (item.Key != null) {
					item.Key.DOKill();
					// 원래 색상으로 부드럽게 복구
					item.Key.DOColor(item.Value, fadeDuration);
				}
			}
		}

	}
}