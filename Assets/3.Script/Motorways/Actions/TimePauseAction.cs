using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Actions {
	using Managers;

	public class TimePauseAction : MotorwaysPlayerAction {
		public override void OnActionBegin(float timestamp) {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.TogglePause();
			}
			OnActionComplete(); // 즉시 완료 처리
		}
	}
}
