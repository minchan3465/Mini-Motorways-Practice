using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace Motorways.UI {

    public class UI_ButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [Header("Animation Settings")]
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.05f);

        [Header("UI References")]
        [SerializeField] private RectTransform yellowBarRect; // 노란색 Bar의 RectTransform

        private Vector3 originalScale;

        private void Awake() {
            // 원본 스케일 저장
            originalScale = transform.localScale;

            // 시작할 때 노란색 Bar의 가로 스케일을 0으로 초기화
            if (yellowBarRect != null) {
                yellowBarRect.localScale = new Vector3(0f, 1f, 1f);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            // 진행 중인 트윈을 취소하여 애니메이션 충돌 방지
            transform.DOKill();
            if (yellowBarRect != null) yellowBarRect.DOKill();

            // UI 스케일 확대
            transform.DOScale(hoverScale, duration).SetEase(Ease.OutQuad);

            // 노란색 Bar 채우기 (가로 스케일을 1로)
            if (yellowBarRect != null) {
                yellowBarRect.DOScaleX(1f, duration).SetEase(Ease.OutQuad);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            // 진행 중인 트윈 취소
            transform.DOKill();
            if (yellowBarRect != null) yellowBarRect.DOKill();

            // UI 스케일 원상태로 복구
            transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);

            // 노란색 Bar 가라앉기 (가로 스케일을 0으로)
            if (yellowBarRect != null) {
                yellowBarRect.DOScaleX(0f, duration).SetEase(Ease.OutQuad);
            }
        }
    }
}