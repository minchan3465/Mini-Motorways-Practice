using System;
using UnityEngine;

namespace Motorways.Managers {
	public class ScoreManager : MonoBehaviour {
		public static ScoreManager Instance;

		public int CurrentScore { get; private set; }
		public event Action<int> OnScoreChanged;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		public void AddScore(int amount = 1) {
			CurrentScore += amount;
			OnScoreChanged?.Invoke(CurrentScore);
			SoundEffect pinCollectRnd = UnityEngine.Random.Range(0, 2) + SoundEffect.PinCollect1;
			SoundManager.Instance.PlaySFX(pinCollectRnd);
		}

		public void ResetScore() {
			CurrentScore = 0;
			OnScoreChanged?.Invoke(CurrentScore);
		}
	}
}
