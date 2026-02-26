using System;
using Client;
using Easing;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000601 RID: 1537
	public class TileSelectedView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x06002AD9 RID: 10969 RVA: 0x000BB8EC File Offset: 0x000B9AEC
		public void Reset()
		{
			this._animationState = TileSelectedView.AnimationState.None;
			this._transitionTween.Stop();
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x000BB93A File Offset: 0x000B9B3A
		public void Appear()
		{
			this._animationState = TileSelectedView.AnimationState.Appearing;
			this._transitionTween.Start(0f, TileSelectedView.Size, TileSelectedView.TransitionInDuration, Easings.Functions.ElasticEaseOut, 0f);
		}

		// Token: 0x06002ADB RID: 10971 RVA: 0x000BB964 File Offset: 0x000B9B64
		public void Disappear()
		{
			this._animationState = TileSelectedView.AnimationState.Disappearing;
			this._transitionTween.Start(base.transform.localScale.x, 0f, TileSelectedView.TransitionOutDuration, Easings.Functions.CubicEaseIn, 0f);
		}

		// Token: 0x06002ADC RID: 10972 RVA: 0x000BB998 File Offset: 0x000B9B98
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._transitionTween.IsActive)
			{
				float scale = this._transitionTween.Tick(timeInterval.Delta);
				base.transform.localScale = new Vector3(scale, scale, 1f);
				if (!this._transitionTween.IsActive)
				{
					if (this._animationState == TileSelectedView.AnimationState.Appearing)
					{
						this._animationState = TileSelectedView.AnimationState.None;
					}
					else if (this._animationState == TileSelectedView.AnimationState.Disappearing)
					{
						this._animationState = TileSelectedView.AnimationState.None;
						return TickResult.Destroy;
					}
				}
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002ADD RID: 10973 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002ADE RID: 10974 RVA: 0x000BBA10 File Offset: 0x000B9C10
		public static TileSelectedView Create(ViewClient client, TileView owningTile)
		{
			TileSelectedView newSelectedView = client.Scope.Get<TileSelectedView>();
			newSelectedView.transform.position = owningTile.transform.position;
			newSelectedView.transform.localScale = new Vector3(0f, 0f, 1f);
			client.AddView(newSelectedView);
			return newSelectedView;
		}

		// Token: 0x040024E9 RID: 9449
		private static float Size = 1f;

		// Token: 0x040024EA RID: 9450
		private static float TransitionInDuration = 0.4f;

		// Token: 0x040024EB RID: 9451
		private static float TransitionOutDuration = 0.2f;

		// Token: 0x040024EC RID: 9452
		private TweenFloat _transitionTween = new TweenFloat();

		// Token: 0x040024ED RID: 9453
		private TileSelectedView.AnimationState _animationState;

		// Token: 0x02000602 RID: 1538
		private enum AnimationState
		{
			// Token: 0x040024EF RID: 9455
			None,
			// Token: 0x040024F0 RID: 9456
			Appearing,
			// Token: 0x040024F1 RID: 9457
			Disappearing
		}
	}
}
