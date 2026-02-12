using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using Motorways.Themes;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200037F RID: 895
	[CreateAssetMenu(fileName = "New Map", menuName = "Motorways/Map Definition", order = 1)]
	public class MapDefinition : ScriptableObject
	{
		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x060015A7 RID: 5543 RVA: 0x0004A5BA File Offset: 0x000487BA
		public MapDefinition.CityNames CityNameEnum
		{
			get
			{
				return (MapDefinition.CityNames)Enum.Parse(typeof(MapDefinition.CityNames), this.cityName, true);
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x060015A8 RID: 5544 RVA: 0x0004A5D8 File Offset: 0x000487D8
		public StringId HowToUnlockDescription
		{
			get
			{
				StringId description;
				if (Enum.TryParse<StringId>(this.howToUnlockDescription, out description))
				{
					return description;
				}
				return StringId.None;
			}
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x0004A5F8 File Offset: 0x000487F8
		public bool IsLocked(IScope scope)
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks))
			{
				return false;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo) && this._achievementsThatUnlockMap != null)
			{
				return true;
			}
			ActivePlayer activePlayer = scope.Get<ActivePlayer>();
			AchievementDatabase achievementDatabase = scope.Get<AchievementDatabase>();
			MotorwaysCityStatistics existingScoreOnMap = activePlayer.GetCityStatisticsForCity(this.cityName, GameMode.Normal, false);
			if (existingScoreOnMap != null && existingScoreOnMap.MaxTrips > 0)
			{
				return false;
			}
			if (this._achievementsThatUnlockMap == null || this._achievementsThatUnlockMap.Count == 0)
			{
				return false;
			}
			if (this._achievementsThatUnlockMap.Count == 1 && this._achievementsThatUnlockMap[0] == null)
			{
				return false;
			}
			foreach (AchievementData requiredAchievement in this._achievementsThatUnlockMap)
			{
				if (!(requiredAchievement == null) && activePlayer.IsAchievementCompleted(achievementDatabase[requiredAchievement.GetId()]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x0004A6F4 File Offset: 0x000488F4
		public bool IsExpertModeUnlocked(IScope scope)
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks))
			{
				return true;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.ExpertLock))
			{
				return false;
			}
			ActivePlayer activePlayer = scope.Get<ActivePlayer>();
			AchievementDatabase achievementDatabase = scope.Get<AchievementDatabase>();
			return this._expertRequiredAchievement == null || activePlayer.IsAchievementCompleted(achievementDatabase[this._expertRequiredAchievement.GetId()]);
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0004A74C File Offset: 0x0004894C
		public bool IsCityChallengeLocked(IScope scope)
		{
			return !FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks) && !FeatureToggle.IsFeatureDisabled(Feature.CityChallenges) && (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo) || scope.Get<ActivePlayer>().GetCityStatisticsForCity(this.cityName, GameMode.Normal, true).MaxTrips < this.challengeModeTargetScore);
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x0004A79C File Offset: 0x0004899C
		public bool HasUpgradeType(UpgradeType upgradeType)
		{
			UpgradeType[] array = this.availableUpgrades;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == upgradeType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001252 RID: 4690
		[EnumSearch(typeof(MapDefinition.CityNames), false, isString = true)]
		public string cityName;

		// Token: 0x04001253 RID: 4691
		[EnumSearch(typeof(StringId), false, isString = true)]
		public string mapName;

		// Token: 0x04001254 RID: 4692
		[EnumSearch(typeof(StringId), false, isString = true)]
		public string mapDescription;

		// Token: 0x04001255 RID: 4693
		public string mapAssetBundle;

		// Token: 0x04001256 RID: 4694
		public string mapPrefabName;

		// Token: 0x04001257 RID: 4695
		[EnumTypedArray(typeof(MotorwaysThemePreference))]
		[NonReorderable]
		[Space(20f)]
		public Theme[] themes = new Theme[5];

		// Token: 0x04001258 RID: 4696
		[EnumTypedArray(typeof(MotorwaysThemePreference))]
		[NonReorderable]
		public Sprite[] themePreviewSprites = new Sprite[5];

		// Token: 0x04001259 RID: 4697
		[Tooltip("What upgrades does this map provide?")]
		public UpgradeType[] availableUpgrades;

		// Token: 0x0400125A RID: 4698
		[Tooltip("What score does the player need to unlock challenge mode?")]
		public int challengeModeTargetScore;

		// Token: 0x0400125B RID: 4699
		public CityChallengeData[] cityChallenges;

		// Token: 0x0400125C RID: 4700
		[SerializeField]
		[CanBeNull]
		private AchievementData _expertRequiredAchievement;

		// Token: 0x0400125D RID: 4701
		[InfoBox("Completing any of the achievements in this list will unlock this map.", InfoBoxType.Normal, null)]
		[CanBeNull]
		[SerializeField]
		public List<AchievementData> _achievementsThatUnlockMap;

		// Token: 0x0400125E RID: 4702
		[EnumSearch(typeof(StringId), false, isString = true)]
		public string howToUnlockDescription;

		// Token: 0x0400125F RID: 4703
		public bool isTrainMap;

		// Token: 0x04001260 RID: 4704
		public bool isBoatMap;

		// Token: 0x02000380 RID: 896
		public enum CityNames
		{
			// Token: 0x04001262 RID: 4706
			None,
			// Token: 0x04001263 RID: 4707
			LosAngeles,
			// Token: 0x04001264 RID: 4708
			Beijing,
			// Token: 0x04001265 RID: 4709
			MexicoCity,
			// Token: 0x04001266 RID: 4710
			DarEsSalaam,
			// Token: 0x04001267 RID: 4711
			Moscow,
			// Token: 0x04001268 RID: 4712
			Tokyo,
			// Token: 0x04001269 RID: 4713
			Munich,
			// Token: 0x0400126A RID: 4714
			Manila,
			// Token: 0x0400126B RID: 4715
			Zurich,
			// Token: 0x0400126C RID: 4716
			RioDeJaneiro,
			// Token: 0x0400126D RID: 4717
			Dubai,
			// Token: 0x0400126E RID: 4718
			Wellington,
			// Token: 0x0400126F RID: 4719
			Warsaw,
			// Token: 0x04001270 RID: 4720
			ChiangMai,
			// Token: 0x04001271 RID: 4721
			Lisbon,
			// Token: 0x04001272 RID: 4722
			Busan,
			// Token: 0x04001273 RID: 4723
			London,
			// Token: 0x04001274 RID: 4724
			Mumbai,
			// Token: 0x04001275 RID: 4725
			NewYorkCity,
			// Token: 0x04001276 RID: 4726
			Reykjavik,
			// Token: 0x04001277 RID: 4727
			Vancouver,
			// Token: 0x04001278 RID: 4728
			Cairns,
			// Token: 0x04001279 RID: 4729
			Copenhagen,
			// Token: 0x0400127A RID: 4730
			HongKong,
			// Token: 0x0400127B RID: 4731
			CapeTown
		}
	}
}
