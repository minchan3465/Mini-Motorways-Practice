using System;
using Easing;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200045C RID: 1116
	public class TweenFloat : Tween<float>
	{
		// Token: 0x06001BFB RID: 7163 RVA: 0x00067168 File Offset: 0x00065368
		public void Start(float start, float rangeBegin, float rangeEnd, float rangeDuration)
		{
			if (!Mathf.Approximately(rangeBegin, 0f))
			{
				float t = (start - rangeBegin) / (rangeEnd - rangeBegin);
				rangeDuration *= 1f - t;
			}
			base.Start(start, rangeEnd, rangeDuration, Easings.Functions.Linear, 0f);
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x000671A6 File Offset: 0x000653A6
		protected override float LerpValue(float startValue, float endValue, float alpha)
		{
			return startValue + (endValue - startValue) * alpha;
		}
	}
}
