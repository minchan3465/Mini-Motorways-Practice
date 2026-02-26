using System;
using Motorways;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200018C RID: 396
[CreateAssetMenu(fileName = "New Achievement", menuName = "Motorways/Achievements/Achievement Data", order = 1)]
public class MotorwaysAchievementData : AchievementData
{
	// Token: 0x06000900 RID: 2304 RVA: 0x0001D6FC File Offset: 0x0001B8FC
	private bool ShouldShowUpgradeTypeField()
	{
		return this.type == AchievementType.UpgradesUsed || this.type == AchievementType.UpgradeLength || this.type == AchievementType.DeletedUpgrades;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x0001D71B File Offset: 0x0001B91B
	private bool ShouldHideCityField()
	{
		return this.scale == AchievementScale.Lifetime;
	}

	// Token: 0x04000476 RID: 1142
	[HideIf("ShouldHideCityField")]
	[EnumSearch(typeof(MapDefinition.CityNames), false, isString = true)]
	public string cityName;

	// Token: 0x04000477 RID: 1143
	[HideIf("ShouldHideCityField")]
	[Dropdown("_getChallengeIndexValues")]
	public int challengeIndex = -1;

	// Token: 0x04000478 RID: 1144
	public int intValue;

	// Token: 0x04000479 RID: 1145
	public AchievementType type;

	// Token: 0x0400047A RID: 1146
	public AchievementScale scale;

	// Token: 0x0400047B RID: 1147
	[HideIf("ShouldHideCityField")]
	public MotorwaysAchievementDefinition.AchievementGameMode gameMode = MotorwaysAchievementDefinition.AchievementGameMode.Everything;

	// Token: 0x0400047C RID: 1148
	[ShowIf("ShouldShowUpgradeTypeField")]
	public UpgradeType upgradeType;

	// Token: 0x0400047D RID: 1149
	[StringEnumSearch(typeof(StringId))]
	[Header("Description ID")]
	public string DescriptionId = StringId.None.ToString();

	// Token: 0x0400047E RID: 1150
	private DropdownList<int> _getChallengeIndexValues = new DropdownList<int>
	{
		{
			"No Challenge",
			-1
		},
		{
			"Any Challenge",
			-2
		},
		{
			"Challenge 0",
			0
		},
		{
			"Challenge 1",
			1
		},
		{
			"Challenge 2",
			2
		},
		{
			"Challenge 3",
			3
		},
		{
			"Challenge 4",
			4
		}
	};
}
