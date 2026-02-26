using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000733 RID: 1843
	public class MapButtonLockedCard : MapButtonCard
	{
		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06003337 RID: 13111 RVA: 0x000F2944 File Offset: 0x000F0B44
		// (remove) Token: 0x06003338 RID: 13112 RVA: 0x000F297C File Offset: 0x000F0B7C
		public event Action OnNavButtonClicked;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06003339 RID: 13113 RVA: 0x000F29B4 File Offset: 0x000F0BB4
		// (remove) Token: 0x0600333A RID: 13114 RVA: 0x000F29EC File Offset: 0x000F0BEC
		private event Action _onUnlockAnimationComplete;

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x0600333B RID: 13115 RVA: 0x000F2A21 File Offset: 0x000F0C21
		public LocalizedTextUI Header
		{
			get
			{
				return this.header;
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x0600333C RID: 13116 RVA: 0x000F2A29 File Offset: 0x000F0C29
		public LocalizedTextUI DescriptionHeader
		{
			get
			{
				return this.descriptionHeader;
			}
		}

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x0600333D RID: 13117 RVA: 0x000F2A31 File Offset: 0x000F0C31
		public LocalizedTextUI Description
		{
			get
			{
				return this.description;
			}
		}

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x0600333E RID: 13118 RVA: 0x000F2A39 File Offset: 0x000F0C39
		public TouchButton TouchButton
		{
			get
			{
				return this.touchButton;
			}
		}

		// Token: 0x0600333F RID: 13119 RVA: 0x000F2A41 File Offset: 0x000F0C41
		public void NavButtonClicked()
		{
			Action onNavButtonClicked = this.OnNavButtonClicked;
			if (onNavButtonClicked == null)
			{
				return;
			}
			onNavButtonClicked();
		}

		// Token: 0x06003340 RID: 13120 RVA: 0x000F2A53 File Offset: 0x000F0C53
		public void PlayUnlockAnimation(Action onComplete)
		{
			this._onUnlockAnimationComplete += onComplete;
			this._animator.SetTrigger(MapButtonLockedCard.AnimationTriggerUnlockMap);
		}

		// Token: 0x06003341 RID: 13121 RVA: 0x000F2A6C File Offset: 0x000F0C6C
		[UsedImplicitly]
		public void UnlockAnimationComplete()
		{
			Action onUnlockAnimationComplete = this._onUnlockAnimationComplete;
			if (onUnlockAnimationComplete == null)
			{
				return;
			}
			onUnlockAnimationComplete();
		}

		// Token: 0x04002BB3 RID: 11187
		[SerializeField]
		private LocalizedTextUI header;

		// Token: 0x04002BB4 RID: 11188
		[SerializeField]
		private LocalizedTextUI descriptionHeader;

		// Token: 0x04002BB5 RID: 11189
		[SerializeField]
		private LocalizedTextUI description;

		// Token: 0x04002BB6 RID: 11190
		[SerializeField]
		private TouchButton touchButton;

		// Token: 0x04002BB7 RID: 11191
		[SerializeField]
		private Animator _animator;

		// Token: 0x04002BB8 RID: 11192
		private static readonly int AnimationTriggerUnlockMap = Animator.StringToHash("Unlock");
	}
}
