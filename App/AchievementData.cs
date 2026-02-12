using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class AchievementData : ScriptableObject
{
	// Token: 0x0600035B RID: 859 RVA: 0x0000E3A3 File Offset: 0x0000C5A3
	public virtual string GetId()
	{
		return base.name;
	}

	// Token: 0x04000163 RID: 355
	public Sprite achievementIcon;

	// Token: 0x04000164 RID: 356
	[SerializeField]
	public List<AchievementData.AchievementPlatformSpecificData> platformSpecificData = new List<AchievementData.AchievementPlatformSpecificData>();

	// Token: 0x020000B8 RID: 184
	public enum AchievementPlatform
	{
		// Token: 0x04000166 RID: 358
		None,
		// Token: 0x04000167 RID: 359
		GameCenter,
		// Token: 0x04000168 RID: 360
		Steamworks
	}

	// Token: 0x020000B9 RID: 185
	public enum AchievementDataType
	{
		// Token: 0x0400016A RID: 362
		None,
		// Token: 0x0400016B RID: 363
		PlatformId
	}

	// Token: 0x020000BA RID: 186
	[Serializable]
	public class AchievementPlatformSpecificData
	{
		// Token: 0x0600035D RID: 861 RVA: 0x0000E3C0 File Offset: 0x0000C5C0
		public virtual AchievementData.AchievementPlatformSpecificData Clone(AchievementData.AchievementPlatformSpecificData intoData = null)
		{
			AchievementData.AchievementPlatformSpecificData clonedData = intoData;
			if (clonedData == null)
			{
				clonedData = new AchievementData.AchievementPlatformSpecificData();
			}
			clonedData.forPlatform = this.forPlatform;
			clonedData.dataKey = this.dataKey;
			clonedData.intData = this.intData;
			clonedData.stringData = this.stringData;
			return clonedData;
		}

		// Token: 0x0400016C RID: 364
		public AchievementData.AchievementPlatform forPlatform;

		// Token: 0x0400016D RID: 365
		public AchievementData.AchievementDataType dataKey;

		// Token: 0x0400016E RID: 366
		public int intData;

		// Token: 0x0400016F RID: 367
		public string stringData;
	}
}
