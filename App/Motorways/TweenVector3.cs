using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200045E RID: 1118
	public class TweenVector3 : Tween<Vector3>
	{
		// Token: 0x06001C00 RID: 7168 RVA: 0x000671D3 File Offset: 0x000653D3
		protected override Vector3 LerpValue(Vector3 startValue, Vector3 endValue, float alpha)
		{
			return startValue + (endValue - startValue) * alpha;
		}
	}
}
