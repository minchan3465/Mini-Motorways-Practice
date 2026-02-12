using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200045D RID: 1117
	public class TweenRadians : Tween<float>
	{
		// Token: 0x06001BFE RID: 7166 RVA: 0x000671B7 File Offset: 0x000653B7
		protected override float LerpValue(float startValue, float endValue, float alpha)
		{
			return Mathf.LerpAngle(startValue * 57.29578f, endValue * 57.29578f, alpha) * 0.017453292f;
		}
	}
}
