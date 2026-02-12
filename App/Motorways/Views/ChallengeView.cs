using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Leaderboards;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000576 RID: 1398
	public class ChallengeView : IView, IReusable
	{
		// Token: 0x06002649 RID: 9801 RVA: 0x000A26DF File Offset: 0x000A08DF
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			this.TickChallengeTimeRemainingDisplay(timeInterval.Delta);
			return TickResult.ContinueTicking;
		}

		// Token: 0x0600264A RID: 9802 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetGameobjectActive(bool isActive)
		{
		}

		// Token: 0x0600264B RID: 9803 RVA: 0x000A26F0 File Offset: 0x000A08F0
		private void TickChallengeTimeRemainingDisplay(float deltaTime)
		{
			ActiveChallengesModel activeChallengesModel = this._scope.Get<ActiveChallengesModel>();
			if (!activeChallengesModel.HasChallenges)
			{
				return;
			}
			if (!activeChallengesModel.HasEndTime)
			{
				return;
			}
			int secondsLeftWithGracePeriod = activeChallengesModel.SecondsLeftWithGracePeriod;
			if (secondsLeftWithGracePeriod <= 15)
			{
				if (!this._scoreSubmitted)
				{
					LeaderboardId leaderboardId = this._motorwaysGame.GetLeaderboardIdForGame();
					int currentScore = this._scope.Get<ScoreModel>().Score;
					this._leaderboardService.SubmitScore(leaderboardId, currentScore, LeaderboardScoreState.Locked);
					this._scoreSubmitted = true;
					return;
				}
			}
			else if (secondsLeftWithGracePeriod < 900)
			{
				ISimulation simulation = this._scope.Get<ISimulation>();
				if (!this._hasSeenMessage)
				{
					AnchoredMessageModel message = this._scope.Get<AnchoredMessageModel>();
					message.InitializeWithScreenAnchor(StringId.Leaderboard_ChallengeTimeRunningOut, ChallengeView.MessageAnchorOffset, CameraLayer.Overlay, null);
					simulation.AddModel(message);
					this._messageModal = message;
					this._messageDisplayTimeRemaining = 5f;
					this._hasSeenMessage = true;
				}
				if (this._messageDisplayTimeRemaining > 0f)
				{
					this._messageDisplayTimeRemaining -= deltaTime;
					if (this._messageDisplayTimeRemaining < 0f)
					{
						simulation.RemoveModel(this._messageModal);
						this._messageModal = null;
					}
				}
			}
		}

		// Token: 0x0600264C RID: 9804 RVA: 0x000A280F File Offset: 0x000A0A0F
		public void Reset()
		{
			this._hasSeenMessage = false;
			this._messageModal = null;
			this._messageDisplayTimeRemaining = 0f;
			this._scoreSubmitted = false;
		}

		// Token: 0x04002030 RID: 8240
		[Dependency]
		private IScope _scope;

		// Token: 0x04002031 RID: 8241
		[Dependency]
		private MotorwaysGame _motorwaysGame;

		// Token: 0x04002032 RID: 8242
		[Dependency]
		private LeaderboardService _leaderboardService;

		// Token: 0x04002033 RID: 8243
		private AnchoredMessageModel _messageModal;

		// Token: 0x04002034 RID: 8244
		private bool _hasSeenMessage;

		// Token: 0x04002035 RID: 8245
		private float _messageDisplayTimeRemaining;

		// Token: 0x04002036 RID: 8246
		private bool _scoreSubmitted;

		// Token: 0x04002037 RID: 8247
		private static readonly Vector2 MessageAnchorOffset = new Vector2(0f, 0.8f);

		// Token: 0x04002038 RID: 8248
		public const int TimeRemainingBeforeNotificationInSeconds = 900;

		// Token: 0x04002039 RID: 8249
		public const int SubmitScoreWhenSecondsRemaining = 15;

		// Token: 0x0400203A RID: 8250
		private const float MessageDisplayDurationInSeconds = 5f;
	}
}
