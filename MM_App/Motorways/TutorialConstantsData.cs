using System;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000443 RID: 1091
	[CreateAssetMenu(menuName = "Motorways/TutorialConstants")]
	public class TutorialConstantsData : ScriptableObject
	{
		// Token: 0x04001681 RID: 5761
		private const string GeneralGroup = "General";

		// Token: 0x04001682 RID: 5762
		[FoldoutGroup("General")]
		public Vector2 UnanchoredMessageOffset = new Vector2(0f, 0.7f);

		// Token: 0x04001683 RID: 5763
		[FoldoutGroup("General")]
		public Vector2 UpgradeScreenMessageOffset = new Vector2(0f, 0.7f);

		// Token: 0x04001684 RID: 5764
		private const string DrawDeleteGroup = "Draw Delete Stage";

		// Token: 0x04001685 RID: 5765
		[FoldoutGroup("Draw Delete Stage")]
		public Vector2Int LockedEditModePosition = new Vector2Int(0, 0);

		// Token: 0x04001686 RID: 5766
		[FoldoutGroup("Draw Delete Stage")]
		public Vector2 DrawRoadIdleHintStartPosition = new Vector2(-25f, -6f);

		// Token: 0x04001687 RID: 5767
		[FoldoutGroup("Draw Delete Stage")]
		public Vector2 DrawRoadIdleHintEndPosition = new Vector2(-10f, -6f);

		// Token: 0x04001688 RID: 5768
		public int DefaultConcreteForUpgradePair = 15;

		// Token: 0x04001689 RID: 5769
		private const string TutorialEndStage = "Tutorial End Stage";

		// Token: 0x0400168A RID: 5770
		[FoldoutGroup("Tutorial End Stage")]
		public int AdditionalScoreToGet = 50;

		// Token: 0x0400168B RID: 5771
		[FoldoutGroup("Tutorial End Stage")]
		public int AdditionalScoreToGetRounding = 50;
	}
}
