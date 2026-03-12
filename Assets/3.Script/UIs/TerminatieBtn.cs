using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.UI {
	public class TerminatieBtn : MonoBehaviour {
		public void TermiinateGame() {
			// [사용자 요청] 게임 종료 및 에디터 정지 직접 수행
			SoundManager.Instance.PlaySFX(SoundEffect.ButtonMain);
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
