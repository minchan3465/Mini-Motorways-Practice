using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.UI {
	using Managers;
	public class TimerBtn : MonoBehaviour {
		public void changeTimeScale(float timeSacle) {
			SimulationManager.Instance.changeTimeScale(timeSacle);
		}
	}
}
