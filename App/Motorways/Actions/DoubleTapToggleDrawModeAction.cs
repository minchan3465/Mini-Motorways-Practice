using System;
using Factory;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006FF RID: 1791
	public class DoubleTapToggleDrawModeAction : MotorwaysPlayerAction
	{
		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x06003105 RID: 12549 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool IsInterruptible
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003106 RID: 12550 RVA: 0x000E68BC File Offset: 0x000E4ABC
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			this._firstTapTimestamp = Time.time;
			this._firstTapPosition = this.GetMoveFocusJoystickInputValue();
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x000E68E3 File Offset: 0x000E4AE3
		public override void OnActionComplete()
		{
			base.OnActionComplete();
			this._gameUI.ToggleDrawMode();
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x000E68F8 File Offset: 0x000E4AF8
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			Vector2 tapPosition = this.GetMoveFocusJoystickInputValue();
			if (this._secondTapTimestamp <= -3.4028235E+38f)
			{
				if (tapPosition != Vector2.zero && !this.WithinRadius(tapPosition, this._firstTapPosition, 0.040000003f))
				{
					this.OnActionCancel();
					return;
				}
				if (Time.time - this._firstTapTimestamp >= 0.5f)
				{
					this.OnActionCancel();
					return;
				}
			}
			else
			{
				if (tapPosition == Vector2.zero)
				{
					this.OnActionComplete();
					return;
				}
				if (Time.time - this._secondTapTimestamp < 0.1f)
				{
					if (tapPosition != Vector2.zero && !this.WithinRadius(tapPosition, this._secondTapPosition, 0.040000003f))
					{
						this.OnActionCancel();
						return;
					}
				}
				else
				{
					this.OnActionComplete();
				}
			}
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x000E69B8 File Offset: 0x000E4BB8
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			base.ObserveInput(timestamp, inputEvent, overUI);
			if (this._secondTapTimestamp <= -3.4028235E+38f && Time.time - this._firstTapTimestamp < 0.5f)
			{
				this._secondTapTimestamp = Time.time;
				this._secondTapPosition = this.GetMoveFocusJoystickInputValue();
			}
		}

		// Token: 0x0600310A RID: 12554 RVA: 0x000E6A05 File Offset: 0x000E4C05
		public override void Reset()
		{
			base.Reset();
			this._firstTapPosition = Vector2.zero;
			this._firstTapTimestamp = float.MinValue;
			this._secondTapPosition = Vector2.zero;
			this._secondTapTimestamp = float.MinValue;
		}

		// Token: 0x0600310B RID: 12555 RVA: 0x000E6A3C File Offset: 0x000E4C3C
		private bool WithinRadius(Vector2 a, Vector2 b, float radiusSquared)
		{
			return (a - b).sqrMagnitude <= radiusSquared;
		}

		// Token: 0x0600310C RID: 12556 RVA: 0x000E6A5E File Offset: 0x000E4C5E
		public static DoubleTapToggleDrawModeAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DoubleTapToggleDrawModeAction doubleTapToggleDrawModeAction = scope.Get<DoubleTapToggleDrawModeAction>();
			doubleTapToggleDrawModeAction.InitializeAction(owningGroup, timestamp);
			doubleTapToggleDrawModeAction.RegisterObserveInputEvent(InputEventFilter.CreateRemoteEventFilter(1, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			doubleTapToggleDrawModeAction.RegisterObserveInputEvent(InputEventFilter.CreateRemoteEventFilter(0, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			doubleTapToggleDrawModeAction.OnActionBegin(timestamp);
			return doubleTapToggleDrawModeAction;
		}

		// Token: 0x04002A13 RID: 10771
		private const float MaxTimeBetweenTapsInSeconds = 0.5f;

		// Token: 0x04002A14 RID: 10772
		private const float NoMovementTimeAfterSecondTap = 0.1f;

		// Token: 0x04002A15 RID: 10773
		private const float MaxDistanceBetweenTaps = 0.2f;

		// Token: 0x04002A16 RID: 10774
		private const float MaxDistanceBetweenTapsSquared = 0.040000003f;

		// Token: 0x04002A17 RID: 10775
		private Vector2 _firstTapPosition = Vector2.zero;

		// Token: 0x04002A18 RID: 10776
		private float _firstTapTimestamp = float.MinValue;

		// Token: 0x04002A19 RID: 10777
		private Vector2 _secondTapPosition = Vector2.zero;

		// Token: 0x04002A1A RID: 10778
		private float _secondTapTimestamp = float.MinValue;
	}
}
