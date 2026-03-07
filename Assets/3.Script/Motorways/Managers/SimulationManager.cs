using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	public class SimulationManager : MonoBehaviour {
        public static SimulationManager Instance;

        [Range(0, 5)]
		public float TimeScale { get; private set; } = 1.0f;
        public bool IsPaused = false;

		public event System.Action OnSimulationStateChanged;

		private List<ISimulationProcess> _processes = new List<ISimulationProcess>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		public void RegisterProcess(ISimulationProcess process) {
			if (!_processes.Contains(process)) {
				_processes.Add(process);
			}
		}

		public void RemoveProcess(ISimulationProcess process) {
			if (_processes.Contains(process)) {
				_processes.Remove(process);
			}
		}

		private float _savedTimeScale = 1.0f;

		public void TogglePause() {
			if (IsPaused) {
				// 일시정지 해제: 이전에 저장해둔 배속으로 복구
				TimeScale = _savedTimeScale;
				IsPaused = false;
			} else {
				// 일시정지 시작: 현재 배속을 저장하고 배속을 0으로 (논리적 정지)
				_savedTimeScale = TimeScale;
				IsPaused = true;
			}
			//Debug.Log(IsPaused ? "Simulation Paused" : "Simulation Resumed (Speed: " + TimeScale + ")");
			OnSimulationStateChanged?.Invoke();
		}

		public void changeTimeScale(float timeSacle) {
			TimeScale = timeSacle;
			if (TimeScale > 0) {
				_savedTimeScale = TimeScale;
				IsPaused = false;
			} else {
				IsPaused = true;
			}
			OnSimulationStateChanged?.Invoke();
		}

		private void Update() {
			if (IsPaused) return;

			float dt = Time.deltaTime * TimeScale;
			
			//옵저버 패턴!
			for(int i = 0; i < _processes.Count; i++) {
				_processes[i].Tick(dt);
			}
		}
	}
}
