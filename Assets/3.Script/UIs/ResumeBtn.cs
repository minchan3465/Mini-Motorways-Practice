using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Motorways.Managers;

namespace Motorways.UI {
	public class ResumeBtn : MonoBehaviour {
		public void TogglePause() {
			if(GameMenuManager.Instance != null) {
				GameMenuManager.Instance.ToggleMenu();
			}
		}
	}
}
