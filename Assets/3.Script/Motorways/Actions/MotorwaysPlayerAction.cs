using UnityEngine;
using Motorways.Managers;

namespace Motorways.Actions {
    public abstract class MotorwaysPlayerAction {
        protected InteractionController _controller;
        protected bool _isComplete = false;

        public bool IsComplete => _isComplete;

        public virtual void Initialize(InteractionController controller) {
            _controller = controller;
        }

        public virtual void OnActionBegin(float timestamp) { }
        public virtual void Tick(float frameTime) { }
        public virtual void OnActionComplete() {
            _isComplete = true;
        }
        public virtual void OnActionCancel() {
            _isComplete = true;
        }

        protected Vector2Int GetPointerGridPosition() => _controller.CurrentGridPointer;
    }
}
