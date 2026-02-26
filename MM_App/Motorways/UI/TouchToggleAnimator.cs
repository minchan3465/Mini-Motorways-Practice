using System;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x0200074A RID: 1866
	[RequireComponent(typeof(Animator))]
	public class TouchToggleAnimator : MonoBehaviour
	{
		// Token: 0x0600341E RID: 13342 RVA: 0x000F5C80 File Offset: 0x000F3E80
		private void Awake()
		{
			this._animator = base.GetComponent<Animator>();
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x000F5C90 File Offset: 0x000F3E90
		private void Update()
		{
			if (this.toggle.IsOn && !this._isOnVisually)
			{
				this._animator.SetTrigger(this.IsOnAnimationTrigger);
				this._isOnVisually = true;
				return;
			}
			if (!this.toggle.IsOn && this._isOnVisually)
			{
				this._animator.SetTrigger(this.IsOffAnimationTrigger);
				this._isOnVisually = false;
			}
		}

		// Token: 0x04002C75 RID: 11381
		public TouchToggle toggle;

		// Token: 0x04002C76 RID: 11382
		public string IsOnAnimationTrigger;

		// Token: 0x04002C77 RID: 11383
		public string IsOffAnimationTrigger;

		// Token: 0x04002C78 RID: 11384
		private bool _isOnVisually;

		// Token: 0x04002C79 RID: 11385
		private Animator _animator;
	}
}
