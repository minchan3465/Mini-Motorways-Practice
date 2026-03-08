using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Actions {
	using Managers;

	/// <summary>
	/// ESC 메뉴 토글을 처리하는 액션입니다.
	/// </summary>
	public class GamePauseAction : MotorwaysPlayerAction {
		
		public override void OnActionBegin(float timestamp) {
			base.OnActionBegin(timestamp);

			if (GameMenuManager.Instance != null) {
				GameMenuManager.Instance.ToggleMenu();
			}

			OnActionComplete(); 
		}
	}
}
