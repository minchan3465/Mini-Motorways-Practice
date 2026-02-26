using System;
using Client;
using Easing;
using Factory.Pools;
using FixMath;
using JetBrains.Annotations;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005C8 RID: 1480
	public class IndicatorAnimationView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x0600297B RID: 10619 RVA: 0x000B20C8 File Offset: 0x000B02C8
		public IndicatorAnimationView.AnimationType Animation
		{
			get
			{
				return this._animationType;
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x0600297C RID: 10620 RVA: 0x000B20D0 File Offset: 0x000B02D0
		public Fix64 Duration
		{
			get
			{
				if (this.Animation == IndicatorAnimationView.AnimationType.Tap)
				{
					return Fix64Consts.Two;
				}
				if (this.Animation == IndicatorAnimationView.AnimationType.Drag)
				{
					return (Fix64)3f + Fix64Consts.Two + Fix64Consts.Two;
				}
				return Fix64Consts.Two;
			}
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x000B2110 File Offset: 0x000B0310
		public void Reset()
		{
			this._animator = null;
			this._dragLerp = 0f;
			this._animationState = IndicatorAnimationView.AnimationState.Started;
			base.transform.position = Vector3.zero;
			this._animationType = IndicatorAnimationView.AnimationType.Tap;
			this._startingPoint = default(Vector3);
			this._endPoint = default(Vector3);
		}

		// Token: 0x0600297E RID: 10622 RVA: 0x000B2168 File Offset: 0x000B0368
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._animationType == IndicatorAnimationView.AnimationType.Drag)
			{
				if (this._animationState == IndicatorAnimationView.AnimationState.ExplicitlyControlled && this._dragLerp < 1f)
				{
					this._dragLerp += timeInterval.Delta * 0.33333334f;
					base.transform.position = Vector3.Lerp(this._startingPoint, this._endPoint, Easings.QuadraticEaseInOut(this._dragLerp));
					if (this._dragLerp >= 1f)
					{
						this._animator.SetTrigger(IndicatorAnimationView.EndDragTrigger);
					}
				}
				return TickResult.ContinueTicking;
			}
			if (this._animationState != IndicatorAnimationView.AnimationState.Finished)
			{
				return TickResult.ContinueTicking;
			}
			return TickResult.Destroy;
		}

		// Token: 0x0600297F RID: 10623 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002980 RID: 10624 RVA: 0x000B21FF File Offset: 0x000B03FF
		[UsedImplicitly]
		public void Animator_OnDragStartFinished()
		{
			this._animationState = IndicatorAnimationView.AnimationState.ExplicitlyControlled;
		}

		// Token: 0x06002981 RID: 10625 RVA: 0x000B2208 File Offset: 0x000B0408
		[UsedImplicitly]
		public void Animator_OnDragEndFinished()
		{
			this._animationState = IndicatorAnimationView.AnimationState.Finished;
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x000B2208 File Offset: 0x000B0408
		[UsedImplicitly]
		public void Animator_OnHighlightEndFinished()
		{
			this._animationState = IndicatorAnimationView.AnimationState.Finished;
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x000B2208 File Offset: 0x000B0408
		[UsedImplicitly]
		public void Animator_OnTapFinished()
		{
			this._animationState = IndicatorAnimationView.AnimationState.Finished;
		}

		// Token: 0x06002984 RID: 10628 RVA: 0x000B2208 File Offset: 0x000B0408
		[UsedImplicitly]
		public void Animator_OnAlertFinished()
		{
			this._animationState = IndicatorAnimationView.AnimationState.Finished;
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x000B2211 File Offset: 0x000B0411
		public void OnAnimationRelease()
		{
			this._dragLerp = 0f;
			if (this._animationType == IndicatorAnimationView.AnimationType.Highlight)
			{
				this._animator.SetTrigger(IndicatorAnimationView.EndHighlightTrigger);
				return;
			}
			if (this._animationType == IndicatorAnimationView.AnimationType.Alert)
			{
				this._animator.SetTrigger(IndicatorAnimationView.AlertEndTrigger);
			}
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x000B2251 File Offset: 0x000B0451
		public void SetAlertType(NotificationView.AlertIconType type)
		{
			this._icon.sprite = this._iconTypes[(int)type];
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x000B2268 File Offset: 0x000B0468
		public void Initialize(IndicatorAnimationView.AnimationType type, Vector3 start, Vector3? end = null)
		{
			this._animationType = type;
			this._startingPoint = start;
			this._endPoint = (end ?? Vector2.zero);
			this._animator = base.gameObject.GetComponent<Animator>();
			base.transform.position = start;
			if (this._animationType == IndicatorAnimationView.AnimationType.Drag)
			{
				this._animator.SetTrigger(IndicatorAnimationView.StartDragTrigger);
				return;
			}
			if (this._animationType == IndicatorAnimationView.AnimationType.Tap)
			{
				this._animator.SetTrigger(IndicatorAnimationView.TapTrigger);
				return;
			}
			if (this._animationType == IndicatorAnimationView.AnimationType.Highlight)
			{
				this._animator.SetTrigger(IndicatorAnimationView.HighlightTrigger);
				return;
			}
			if (this._animationType == IndicatorAnimationView.AnimationType.Alert)
			{
				this._animator.SetTrigger(IndicatorAnimationView.AlertStartTrigger);
			}
		}

		// Token: 0x0400231F RID: 8991
		private IndicatorAnimationView.AnimationType _animationType;

		// Token: 0x04002320 RID: 8992
		private Vector3 _startingPoint;

		// Token: 0x04002321 RID: 8993
		private Vector3 _endPoint;

		// Token: 0x04002322 RID: 8994
		private Animator _animator;

		// Token: 0x04002323 RID: 8995
		[SerializeField]
		private SpriteRenderer _icon;

		// Token: 0x04002324 RID: 8996
		[EnumTypedArray(typeof(NotificationView.AlertIconType))]
		[SerializeField]
		private Sprite[] _iconTypes = new Sprite[Enum.GetValues(typeof(NotificationView.AlertIconType)).Length];

		// Token: 0x04002325 RID: 8997
		private float _dragLerp;

		// Token: 0x04002326 RID: 8998
		private IndicatorAnimationView.AnimationState _animationState;

		// Token: 0x04002327 RID: 8999
		private const float DragDuration = 3f;

		// Token: 0x04002328 RID: 9000
		private static readonly int TapTrigger = Animator.StringToHash("tap");

		// Token: 0x04002329 RID: 9001
		private static readonly int StartDragTrigger = Animator.StringToHash("startDrag");

		// Token: 0x0400232A RID: 9002
		private static readonly int EndDragTrigger = Animator.StringToHash("endDrag");

		// Token: 0x0400232B RID: 9003
		private static readonly int HighlightTrigger = Animator.StringToHash("startHighlight");

		// Token: 0x0400232C RID: 9004
		private static readonly int EndHighlightTrigger = Animator.StringToHash("endHighlight");

		// Token: 0x0400232D RID: 9005
		private static readonly int AlertStartTrigger = Animator.StringToHash("startAlert");

		// Token: 0x0400232E RID: 9006
		private static readonly int AlertEndTrigger = Animator.StringToHash("endAlert");

		// Token: 0x020005C9 RID: 1481
		public enum AnimationType
		{
			// Token: 0x04002330 RID: 9008
			Tap,
			// Token: 0x04002331 RID: 9009
			Drag,
			// Token: 0x04002332 RID: 9010
			Highlight,
			// Token: 0x04002333 RID: 9011
			Alert
		}

		// Token: 0x020005CA RID: 1482
		private enum AnimationState
		{
			// Token: 0x04002335 RID: 9013
			Started,
			// Token: 0x04002336 RID: 9014
			ExplicitlyControlled,
			// Token: 0x04002337 RID: 9015
			Finished
		}
	}
}
