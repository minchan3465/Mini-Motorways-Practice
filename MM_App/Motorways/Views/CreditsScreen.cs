using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000533 RID: 1331
	public class CreditsScreen : MonoBehaviour
	{
		// Token: 0x06002323 RID: 8995 RVA: 0x0008FB7A File Offset: 0x0008DD7A
		public void OnInteractionToggled(bool toggle)
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (toggle)
				{
					this.TransitionIn();
					return;
				}
				this.TransitionOut();
			}
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x0008FB99 File Offset: 0x0008DD99
		private void TransitionIn()
		{
			this._scrollProgress = 0f;
			this._timerBeforeScrolling = this.PassiveScrollDelay;
			this._scrollRect.verticalNormalizedPosition = 1f;
			base.StopAllCoroutines();
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x0008FBC8 File Offset: 0x0008DDC8
		private void TransitionOut()
		{
			this.StopAutoScroll();
			this._scrollProgressAtTransitionOut = this._scrollRect.verticalNormalizedPosition;
			base.StartCoroutine(this.ScrollBackToStart());
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x0008FBEE File Offset: 0x0008DDEE
		private void StopAutoScroll()
		{
			this._scrollProgress = -1f;
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x0008FBFC File Offset: 0x0008DDFC
		public void Update()
		{
			if (this._scrollProgress < 1f && this._scrollProgress >= 0f)
			{
				if (this._timerBeforeScrolling > 0f)
				{
					this._timerBeforeScrolling -= Time.deltaTime;
					return;
				}
				this._scrollRect.verticalNormalizedPosition = 1f - this._scrollProgress;
				this._scrollProgress += Time.deltaTime * this.PassiveScrollSpeed;
			}
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x0008FC73 File Offset: 0x0008DE73
		private IEnumerator ScrollBackToStart()
		{
			for (float time = 0f; time < this.ScrollResetDuration; time += Time.deltaTime)
			{
				yield return new WaitForEndOfFrame();
				this._scrollRect.verticalNormalizedPosition = Mathf.Lerp(this._scrollProgressAtTransitionOut, 1f, time / this.ScrollResetDuration);
			}
			yield break;
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x0008FC82 File Offset: 0x0008DE82
		public void OnScrollStart()
		{
			this.StopAutoScroll();
		}

		// Token: 0x04001D33 RID: 7475
		[SerializeField]
		private ScrollRect _scrollRect;

		// Token: 0x04001D34 RID: 7476
		public Transform CreditSectionContainer;

		// Token: 0x04001D35 RID: 7477
		[SerializeField]
		private float PassiveScrollSpeed = 0.01f;

		// Token: 0x04001D36 RID: 7478
		[SerializeField]
		private float PassiveScrollDelay = 1f;

		// Token: 0x04001D37 RID: 7479
		[SerializeField]
		private float ScrollResetDuration = 0.1f;

		// Token: 0x04001D38 RID: 7480
		private float _scrollProgress;

		// Token: 0x04001D39 RID: 7481
		private float _timerBeforeScrolling;

		// Token: 0x04001D3A RID: 7482
		private float _scrollProgressAtTransitionOut;
	}
}
