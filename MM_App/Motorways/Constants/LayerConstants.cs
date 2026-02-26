using System;
using UnityEngine;

namespace Motorways.Constants
{
	// Token: 0x02000519 RID: 1305
	public static class LayerConstants
	{
		// Token: 0x04001C6F RID: 7279
		public static readonly int DefaultLayerId = LayerMask.NameToLayer("Default");

		// Token: 0x04001C70 RID: 7280
		public static readonly int OverlayLayerId = LayerMask.NameToLayer("Overlay");

		// Token: 0x04001C71 RID: 7281
		public static readonly int UILayerId = LayerMask.NameToLayer("UI");

		// Token: 0x04001C72 RID: 7282
		public static readonly int ShadowMask = LayerMask.GetMask(new string[]
		{
			"Shadow"
		});

		// Token: 0x04001C73 RID: 7283
		public static readonly int MotorwayMask = LayerMask.GetMask(new string[]
		{
			"Motorway"
		});

		// Token: 0x04001C74 RID: 7284
		private static string HeadlightLayerName = "Headlight";

		// Token: 0x04001C75 RID: 7285
		public static readonly int HeadlightMask = LayerMask.GetMask(new string[]
		{
			LayerConstants.HeadlightLayerName
		});

		// Token: 0x04001C76 RID: 7286
		public static readonly int HeadlightLayerId = LayerMask.GetMask(new string[]
		{
			LayerConstants.HeadlightLayerName
		});

		// Token: 0x04001C77 RID: 7287
		private static string HeadlightOcclusionLayerName = "HeadlightOcclusion";

		// Token: 0x04001C78 RID: 7288
		public static readonly int HeadlightOcclusionLayerMask = LayerMask.GetMask(new string[]
		{
			LayerConstants.HeadlightOcclusionLayerName
		});

		// Token: 0x04001C79 RID: 7289
		public static readonly int HeadlightOcclusionLayerId = LayerMask.NameToLayer(LayerConstants.HeadlightOcclusionLayerName);

		// Token: 0x04001C7A RID: 7290
		private static string HeadlightOcclusionMountainLayerName = "HeadlightOcclusionMountain";

		// Token: 0x04001C7B RID: 7291
		public static readonly int HeadlightOcclusionMountainLayerNameLayerMask = LayerMask.GetMask(new string[]
		{
			LayerConstants.HeadlightOcclusionMountainLayerName
		});
	}
}
