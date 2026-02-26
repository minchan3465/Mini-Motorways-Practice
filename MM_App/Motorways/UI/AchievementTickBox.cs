using System;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000719 RID: 1817
	public class AchievementTickBox : MonoBehaviour
	{
		// Token: 0x060031EF RID: 12783 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		// Token: 0x060031F0 RID: 12784 RVA: 0x000EC638 File Offset: 0x000EA838
		public void SetCompleted(bool completed)
		{
			this.tick.SetActive(completed);
		}

		// Token: 0x04002AC9 RID: 10953
		public GameObject tick;

		// Token: 0x04002ACA RID: 10954
		public LocalizedTextUI achievementDescription;
	}
}
