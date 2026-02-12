using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000731 RID: 1841
	public class MapButtonChallengeCardLockScreen : MonoBehaviour
	{
		// Token: 0x06003327 RID: 13095 RVA: 0x000F28A5 File Offset: 0x000F0AA5
		[UsedImplicitly]
		public void UnlockAnimationComplete()
		{
			this._challengeCard.UnlockAnimationComplete();
		}

		// Token: 0x06003328 RID: 13096 RVA: 0x000F28B2 File Offset: 0x000F0AB2
		[UsedImplicitly]
		public void FadeOutAnimationComplete()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x04002BA9 RID: 11177
		[SerializeField]
		private MapButtonChallengeCard _challengeCard;
	}
}
