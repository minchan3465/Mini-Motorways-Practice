using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Actions {
	
	public class GamePauseAction : MotorwaysPlayerAction {
		public override void OnActionBegin(float timestamp) {
			//메뉴 화면 열기.
			OnActionComplete(); // 즉시 완료 처리
		}
	}
}
