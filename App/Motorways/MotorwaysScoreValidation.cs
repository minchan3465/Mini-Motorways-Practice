using System;

namespace Motorways
{
	// Token: 0x020003B8 RID: 952
	public static class MotorwaysScoreValidation
	{
		// Token: 0x060016B6 RID: 5814 RVA: 0x00052450 File Offset: 0x00050650
		public static bool ShouldRecordScore(bool isScoreLocked, int currentScore, int newScore)
		{
			if (isScoreLocked)
			{
				MotorwaysScoreValidation.Log.Info("Not recording score. Score is locked.", new object[]
				{
					newScore
				});
				return false;
			}
			if (newScore < currentScore)
			{
				MotorwaysScoreValidation.Log.Info("Not recording score. New score of {0} is less than current score of {1}.", new object[]
				{
					newScore,
					currentScore
				});
				return false;
			}
			return true;
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x000524AE File Offset: 0x000506AE
		public static bool ShouldLockScoreWhenGameEnds(MapChallenge.ChallengeType challengeType, GameEndReason gameEndReason)
		{
			return challengeType == MapChallenge.ChallengeType.Daily && (gameEndReason == GameEndReason.GameOver || gameEndReason == GameEndReason.Restart);
		}

		// Token: 0x04001350 RID: 4944
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MotorwaysScoreValidation");
	}
}
