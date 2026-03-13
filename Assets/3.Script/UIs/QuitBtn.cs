using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Motorways.Models;

namespace Motorways.UI {
	public class QuitBtn : MonoBehaviour {
		[SerializeField] private RuntimeController rc;

		public void QuitRuntime() {
			DOTween.KillAll();
			Time.timeScale = 1f;

			//정적 데이터 리셋
			Vehicle.ResetId();
			Lane.ResetId();
			CityModel.LatestLaneChangeFrame = -1;
			CityModel.ChangedNodes.Clear();

			//WhiteoutController.Instance.OffWhiteOut();
			SoundManager.Instance.PlaySFX(SoundEffect.ButtonMain);

			rc.MovementToEndPoint();
		}
	}
}
