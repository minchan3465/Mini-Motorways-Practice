using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Motorways.Models;

namespace Motorways.UI {
	public class RestartBtn : MonoBehaviour {
		public void OnRestartGame() {
			// [사용자 요청] 매니저 없이 직접 씬 재시작 및 데이터 초기화 수행
			DOTween.KillAll();
			Time.timeScale = 1f;

			// 정적 데이터 리셋
			Vehicle.ResetId();
			Lane.ResetId();
			CityModel.LatestLaneChangeFrame = -1;
			CityModel.ChangedNodes.Clear();

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
