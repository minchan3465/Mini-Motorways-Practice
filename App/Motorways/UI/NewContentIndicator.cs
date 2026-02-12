using System;
using Factory.Pools;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000737 RID: 1847
	public class NewContentIndicator : MonoBehaviour, IReusable
	{
		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x06003394 RID: 13204 RVA: 0x000F3C20 File Offset: 0x000F1E20
		public bool IsHidden
		{
			get
			{
				return this._animator.GetCurrentAnimatorStateInfo(0).IsName("Exit");
			}
		}

		// Token: 0x06003395 RID: 13205 RVA: 0x000F3C46 File Offset: 0x000F1E46
		public void PlayIntro()
		{
			this._animator.SetTrigger(NewContentIndicator.Intro);
		}

		// Token: 0x06003396 RID: 13206 RVA: 0x000F3C58 File Offset: 0x000F1E58
		public void PlayIdle()
		{
			this._animator.SetTrigger(NewContentIndicator.Idle);
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x000F3C6A File Offset: 0x000F1E6A
		public void PlayExit()
		{
			this._animator.SetTrigger(NewContentIndicator.Exit);
		}

		// Token: 0x06003398 RID: 13208 RVA: 0x000F3C7C File Offset: 0x000F1E7C
		public void Reset()
		{
			this._animator.ResetTrigger(NewContentIndicator.Intro);
			this._animator.ResetTrigger(NewContentIndicator.Idle);
			this._animator.ResetTrigger(NewContentIndicator.Exit);
		}

		// Token: 0x04002BF5 RID: 11253
		private static readonly int Intro = Animator.StringToHash("Intro");

		// Token: 0x04002BF6 RID: 11254
		private static readonly int Idle = Animator.StringToHash("Idle");

		// Token: 0x04002BF7 RID: 11255
		private static readonly int Exit = Animator.StringToHash("Exit");

		// Token: 0x04002BF8 RID: 11256
		[SerializeField]
		private Animator _animator;
	}
}
