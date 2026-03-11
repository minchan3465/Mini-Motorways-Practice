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
        [SerializeField] private RectTransform yellowBarRect;

        private RectTransform myRect;
        private Vector3 originalScale;

        private void Awake() {
            TryGetComponent(out myRect);
            originalScale = transform.localScale;

            SetPivotToLeftAndKeepPosition(myRect);

            if (yellowBarRect != null) {
                SetPivotToLeftAndKeepPosition(yellowBarRect);
                yellowBarRect.localScale = new Vector3(0f, 1f, 1f);
            }
        }

        private void SetPivotToLeftAndKeepPosition(RectTransform targetRect) {
            if (targetRect == null) return;

            Vector2 currentPivot = targetRect.pivot;
            Vector2 targetPivot = new Vector2(0f, currentPivot.y);

            Vector2 pivotDifference = targetPivot - currentPivot;

            targetRect.anchoredPosition += new Vector2(
                pivotDifference.x * targetRect.rect.width,
                pivotDifference.y * targetRect.rect.height
            );

            targetRect.pivot = targetPivot;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOKill();
            if (yellowBarRect != null) yellowBarRect.DOKill();

            transform.DOScale(hoverScale, duration).SetEase(Ease.OutQuad).SetUpdate(true);

            if (yellowBarRect != null) {
                yellowBarRect.DOScaleX(1f, duration).SetEase(Ease.OutQuad).SetUpdate(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            if (yellowBarRect != null) yellowBarRect.DOKill();

            transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(true);

            if (yellowBarRect != null) {
                yellowBarRect.DOScaleX(0f, duration).SetEase(Ease.OutQuad).SetUpdate(true);
            }
        }

        // 씬 전환이나 오브젝트 삭제 시 DOTween 에러를 방지하는 부분
        private void OnDestroy() {
            transform.DOKill();
            if (yellowBarRect != null) yellowBarRect.DOKill();
        }
    }
}