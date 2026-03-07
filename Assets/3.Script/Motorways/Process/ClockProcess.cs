using UnityEngine;
using Motorways.Managers;
using Motorways.Models;

namespace Motorways.Process {
	// 원작의 ClockProcess 역할을 하며, 매 프레임 시간을 누적합니다.
	public class ClockProcess : MonoBehaviour, ISimulationProcess {
		public static ClockProcess Instance;

		public ClockModel Model { get; private set; }

		private void Awake() {
			if (Instance == null) {
				Instance = this;
				Model = new ClockModel();
				Model.Reset();
			} else {
				Destroy(gameObject);
			}
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		private void OnDestroy() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RemoveProcess(this);
			}
		}

		public void Tick(float dt) {
			if (Model == null || Model.IsPaused) return;

			// SimulationManager에서 이미 TimeScale이 곱해진 dt가 넘어오므로 그대로 사용합니다.
			float scaledDelta = dt;

			// 기본 시간 누적
			Model.Time += scaledDelta;

			// 확장에 관여하는 시간 누적 (게임 진행 상황, 주간 보상 등)
			if (!Model.ExpansionTimeManuallyPaused) {
				Model.ExpansionTime += scaledDelta;
			}
		}
	}
}
