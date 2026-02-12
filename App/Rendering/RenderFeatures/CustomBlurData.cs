using System;
using UnityEngine;

namespace Rendering.RenderFeatures
{
	// Token: 0x02000299 RID: 665
	public class CustomBlurData : MonoBehaviour
	{
		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x0003815B File Offset: 0x0003635B
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x00038163 File Offset: 0x00036363
		public float Strength { get; set; }

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x0003816C File Offset: 0x0003636C
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x00038174 File Offset: 0x00036374
		public float LevelsRange { get; set; }

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x0003817D File Offset: 0x0003637D
		// (set) Token: 0x0600108D RID: 4237 RVA: 0x00038185 File Offset: 0x00036385
		public float LevelsOffset { get; set; }
	}
}
