using System;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000397 RID: 919
	[Serializable]
	public class ZoomParameters
	{
		// Token: 0x040012B2 RID: 4786
		public AnimationCurve velocity = new AnimationCurve();

		// Token: 0x040012B3 RID: 4787
		public Fix64 startSize = (Fix64)8L;

		// Token: 0x040012B4 RID: 4788
		public Fix64 endSize = (Fix64)20L;

		// Token: 0x040012B5 RID: 4789
		public Fix64 delayInDays = (Fix64)1L;

		// Token: 0x040012B6 RID: 4790
		public Fix64 durationInDays = (Fix64)69L;

		// Token: 0x040012B7 RID: 4791
		public Vector2 cameraEntryPosition = new Vector2(-100f, 0f);

		// Token: 0x040012B8 RID: 4792
		public Vector2 cameraEntrySplineHandle = new Vector2(-10f, 0f);
	}
}
