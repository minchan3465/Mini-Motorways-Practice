using UnityEngine;
using DG.Tweening;

namespace Motorways.Views {
    public class PinView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _colorRenderer;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.5f;

        private Vector3 _originalScale;
        private Tween _scaleTween;
        private bool _isShowing = false;

        private void Awake() {
            _originalScale = transform.localScale;

            _isShowing = gameObject.activeSelf;
            if (!_isShowing) {
                transform.localScale = Vector3.zero;
            }
        }

        public void SetColor(Color color) {
            if (_colorRenderer != null) {
                _colorRenderer.color = color;
            }
        }

        public void SetVisibility(bool visible) {
            if (_isShowing == visible) return;
            _isShowing = visible;

            _scaleTween?.Kill();

            if (visible) {
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;

                //Pivot이 Bottom일 때 OutBack을 쓰면,
                //아래는 고정된 채로 위로 부풀어 올랐다가 살짝 줄어드는 완벽한 풍선 효과가 납니다.
                _scaleTween = transform.DOScale(_originalScale, _animationDuration)
                    .SetEase(Ease.OutBack);
            } else {
                //사라질 때는 반대로 쪼그라들며 사라집니다.
                _scaleTween = transform.DOScale(Vector3.zero, _animationDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        gameObject.SetActive(false);
                    });
            }
        }
    }
}