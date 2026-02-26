using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000596 RID: 1430
	public class DestinationPinView : MonoBehaviour
	{
		// Token: 0x14000044 RID: 68
		// (add) Token: 0x060027D7 RID: 10199 RVA: 0x000A9F24 File Offset: 0x000A8124
		// (remove) Token: 0x060027D8 RID: 10200 RVA: 0x000A9F5C File Offset: 0x000A815C
		public event Action<DestinationPinView> Hidden;

		// Token: 0x060027D9 RID: 10201 RVA: 0x000A9F94 File Offset: 0x000A8194
		private void Awake()
		{
			foreach (SpriteRenderer spriteRenderer in base.gameObject.GetComponentsInChildren<SpriteRenderer>())
			{
				SpriteRendererState state = new SpriteRendererState();
				state.spriteRenderer = spriteRenderer;
				state.color = spriteRenderer.color;
				Transform spriteTransform = spriteRenderer.transform;
				state.position = spriteTransform.localPosition;
				state.scale = spriteTransform.localScale;
				this._spriteRendererStates.Add(state);
			}
			this._animator.SetBool(DestinationPinView.VisibleParameterId, false);
			this._animator.enabled = false;
			this.FlushAnimator();
			this.SetSpriteVisibility(false);
		}

		// Token: 0x060027DA RID: 10202 RVA: 0x000AA030 File Offset: 0x000A8230
		public void Show()
		{
			if (!this.IsVisible)
			{
				this.SetSpriteVisibility(true);
				this._animator.enabled = true;
				this._animator.SetBool(DestinationPinView.VisibleParameterId, true);
			}
		}

		// Token: 0x060027DB RID: 10203 RVA: 0x000AA05E File Offset: 0x000A825E
		public void Hide()
		{
			if (this.IsVisible)
			{
				this._animator.enabled = true;
				this._animator.SetBool(DestinationPinView.VisibleParameterId, false);
			}
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x060027DC RID: 10204 RVA: 0x000AA085 File Offset: 0x000A8285
		public bool IsVisible
		{
			get
			{
				return this._animator.GetBool(DestinationPinView.VisibleParameterId);
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x060027DD RID: 10205 RVA: 0x000AA097 File Offset: 0x000A8297
		public Vector3 PinCenterPosition
		{
			get
			{
				return this._pinCenter.transform.position;
			}
		}

		// Token: 0x060027DE RID: 10206 RVA: 0x000AA0A9 File Offset: 0x000A82A9
		public void SetPinColor(Color color)
		{
			this._pinCenter.color = color;
		}

		// Token: 0x060027DF RID: 10207 RVA: 0x000AA0B8 File Offset: 0x000A82B8
		public void FlushAnimator()
		{
			for (int flushIteration = 0; flushIteration < 3; flushIteration++)
			{
				this._animator.Update(0.5f);
			}
		}

		// Token: 0x060027E0 RID: 10208 RVA: 0x000AA0E4 File Offset: 0x000A82E4
		public void Reset()
		{
			this._animator.SetBool(DestinationPinView.VisibleParameterId, false);
			this.FlushAnimator();
			this._animator.enabled = false;
			foreach (SpriteRendererState spriteState in this._spriteRendererStates)
			{
				SpriteRenderer spriteRenderer = spriteState.spriteRenderer;
				spriteRenderer.color = spriteState.color;
				Transform transform = spriteRenderer.transform;
				transform.localPosition = spriteState.position;
				transform.localScale = spriteState.scale;
			}
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x000AA184 File Offset: 0x000A8384
		private void SetSpriteVisibility(bool spriteVisibility)
		{
			foreach (SpriteRendererState spriteRendererState in this._spriteRendererStates)
			{
				spriteRendererState.spriteRenderer.gameObject.SetActive(spriteVisibility);
			}
		}

		// Token: 0x060027E2 RID: 10210 RVA: 0x000AA1E0 File Offset: 0x000A83E0
		[UsedImplicitly]
		private void OnPinHidden()
		{
			Action<DestinationPinView> hidden = this.Hidden;
			if (hidden == null)
			{
				return;
			}
			hidden(this);
		}

		// Token: 0x060027E3 RID: 10211 RVA: 0x000AA1F3 File Offset: 0x000A83F3
		[UsedImplicitly]
		private void OnIdleShown()
		{
			if (this.IsVisible)
			{
				this._animator.enabled = false;
			}
		}

		// Token: 0x060027E4 RID: 10212 RVA: 0x000AA209 File Offset: 0x000A8409
		[UsedImplicitly]
		private void OnIdleHidden()
		{
			if (!this.IsVisible)
			{
				this._animator.enabled = false;
				this.SetSpriteVisibility(false);
			}
		}

		// Token: 0x040021B2 RID: 8626
		[SerializeField]
		private SpriteRenderer _pinCenter;

		// Token: 0x040021B3 RID: 8627
		public const ThemeComponentGroupTarget BuildingComponentTargetColor = ThemeComponentGroupTarget.BuildingTop;

		// Token: 0x040021B4 RID: 8628
		private static readonly int VisibleParameterId = Animator.StringToHash("Visible");

		// Token: 0x040021B5 RID: 8629
		[SerializeField]
		private Animator _animator;

		// Token: 0x040021B7 RID: 8631
		private readonly List<SpriteRendererState> _spriteRendererStates = new List<SpriteRendererState>();

		// Token: 0x040021B8 RID: 8632
		private const float MaxAnimationClipDuration = 0.5f;

		// Token: 0x040021B9 RID: 8633
		private const int MaxTransitionsToReachIdleState = 3;
	}
}
