using UnityEngine;

namespace Motorways.Views {
    public class PinView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _colorRenderer;

        public void SetColor(Color color) {
            if (_colorRenderer != null) {
                _colorRenderer.color = color;
            }
        }

        public void SetVisibility(bool visible) {
            gameObject.SetActive(visible);
        }
    }
}
