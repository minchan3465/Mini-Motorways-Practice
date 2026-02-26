using System;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000375 RID: 885
	[Serializable]
	public class CityStartOffsetDefinition
	{
		// Token: 0x0400121D RID: 4637
		[Tooltip("The camera starting offset")]
		public Vector3Fixed fixedPosition;

		// Token: 0x0400121E RID: 4638
		[Tooltip("The variance from this position")]
		public Fix64 variance;
	}
}
