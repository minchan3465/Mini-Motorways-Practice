using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Views;
using Server;
using Unity.Profiling;

namespace Motorways.Processes
{
	// Token: 0x02000482 RID: 1154
	public class AchievementCheckingProcess : IProcess, IReusable
	{
		// Token: 0x06001CA7 RID: 7335 RVA: 0x0006AC64 File Offset: 0x00068E64
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			if (!this._city.Rules.RecordsGameStatistics())
			{
				return;
			}
			if (this._gameContainer == null)
			{
				this._gameContainer = this._scope.Get<GameContainerScreen>();
			}
			if (this._trackedAchievements == null && this._gameContainer != null && this._gameContainer.CurrentCityName != null)
			{
				this._trackedAchievements = new List<MotorwaysAchievementDefinition>();
				for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
				{
					MotorwaysAchievementDefinition achievement = this._achievements[achievementIndex] as MotorwaysAchievementDefinition;
					if (Diagnostics.Verify(achievement != null, "The achievement {0} isn't a motorways achievement!"))
					{
						bool addAchievement = (achievement.Scale == AchievementScale.City && achievement.CityName == this._gameContainer.CurrentCityName && (achievement.ChallengeIndex == this._challenges.cityChallengeIndex || achievement.ChallengeIndex == -1)) || (achievement.Scale == AchievementScale.City && achievement.CityName == this._gameContainer.CurrentCityName && achievement.ChallengeIndex == -2 && this._challenges.cityChallengeIndex != -1) || achievement.Scale == AchievementScale.Game;
						addAchievement &= achievement.DoesGameModeMatch(this._cityModel.Mode);
						if (!this._challenges.IsCityChallenge && this._challenges.HasChallenges && achievement.Type == AchievementType.Score)
						{
							addAchievement = false;
						}
						if (addAchievement)
						{
							this._trackedAchievements.Add(achievement);
						}
					}
				}
			}
			if (this._trackedAchievements != null && this._trackedAchievements.Count > 0)
			{
				int achievementIndexToCheck = this._clock.FrameCount % this._trackedAchievements.Count;
				if (this._trackedAchievements[achievementIndexToCheck].IsGameAchievementSatisfied(this._gameContainer.GetActiveGame() as MotorwaysGame))
				{
					this._player.CompleteAchievement(this._trackedAchievements[achievementIndexToCheck], true);
					this._trackedAchievements.RemoveAt(achievementIndexToCheck);
				}
			}
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x0006AE58 File Offset: 0x00069058
		public void Reset()
		{
			this._trackedAchievements = null;
		}

		// Token: 0x0400189A RID: 6298
		[Dependency]
		private Clock _clock;

		// Token: 0x0400189B RID: 6299
		[Dependency]
		private IScope _scope;

		// Token: 0x0400189C RID: 6300
		[Dependency]
		private AchievementDatabase _achievements;

		// Token: 0x0400189D RID: 6301
		[Dependency]
		private City _city;

		// Token: 0x0400189E RID: 6302
		[Dependency]
		private CityModel _cityModel;

		// Token: 0x0400189F RID: 6303
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040018A0 RID: 6304
		[Dependency]
		private ActiveChallengesModel _challenges;

		// Token: 0x040018A1 RID: 6305
		[Serialize(false, null)]
		private List<MotorwaysAchievementDefinition> _trackedAchievements;

		// Token: 0x040018A2 RID: 6306
		[Serialize(false, null)]
		private GameContainerScreen _gameContainer;

		// Token: 0x040018A3 RID: 6307
		private static readonly ProfilerMarker Profiler_Step = new ProfilerMarker(ProfilerUtility.CategoryProcess, "AchievmentCheckingProcess.Step");
	}
}
