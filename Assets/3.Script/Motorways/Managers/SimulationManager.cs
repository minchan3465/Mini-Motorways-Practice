using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	public class SimulationManager : MonoBehaviour {
        public static SimulationManager Instance;

        [Range(0, 5)]
		public float TimeScale { get; private set; } = 1.0f;
        public bool IsPaused = false;

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

		public void changeTimeScale(float timeSacle) {
			TimeScale = timeSacle;
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
