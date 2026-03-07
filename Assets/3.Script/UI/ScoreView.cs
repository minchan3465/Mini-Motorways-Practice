using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Motorways.Managers;

namespace Motorways.UI {
	public class ScoreView : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI Score_Text;

		private void Start() {
			if (ScoreManager.Instance != null) {
				ScoreManager.Instance.OnScoreChanged += SetScore;
				SetScore(ScoreManager.Instance.CurrentScore);
			}
		}

		private void OnDestroy() {
			if (ScoreManager.Instance != null) {
				ScoreManager.Instance.OnScoreChanged -= SetScore;
			}
		}

		public void SetScore(int totalScore) {
			Score_Text.text = totalScore.ToString();
		}
	}
}

