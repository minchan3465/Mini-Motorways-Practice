using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using Motorways.Audio;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000594 RID: 1428
	public class DestinationPinAnimatorView : MonoBehaviour
	{
		// Token: 0x060027BD RID: 10173 RVA: 0x000A9594 File Offset: 0x000A7794
		public void Initialize(DestinationView destination, IScope scope)
		{
			this.Reset();
			this._destination = destination;
			this._audioSystem = scope.Get<IAudioSystem>();
			this._constants = scope.Get<VisualConstantsData>();
			this._timerPin.Initialize(scope);
			if (!this.supportsUpgrading)
			{
				this._animator.enabled = true;
			}
			this.IsUpgraded = this._destination.Model.IsUpgraded;
			this.IsBigPinVisible = this._destination.Model.IsOvercrowding;
			this.SetPinCount(this._destination.Model.TotalDemand);
			Theme theme = scope.Get<MotorwaysThemeDatabase>().GetTheme() as Theme;
			if (Diagnostics.Verify(theme != null))
			{
				this.SetPinColors(theme.GetBuildingColor(destination.groupIndex, ThemeComponentGroupTarget.BuildingTop));
			}
		}

		// Token: 0x060027BE RID: 10174 RVA: 0x000A965C File Offset: 0x000A785C
		public void Tick(TimeInterval timeInterval, float stepAlpha, float gracePeriod)
		{
			if (this._destination != null)
			{
				if (this._newPinPostponement > 0f)
				{
					this._newPinPostponement = Mathf.Max(0f, this._newPinPostponement - timeInterval.ScaledDelta);
				}
				if (this._newPinCooldown > 0f)
				{
					this._newPinCooldown = Mathf.Max(0f, this._newPinCooldown - timeInterval.ScaledDelta);
				}
				if (this._visiblePins < this._destination.Model.TotalDemand)
				{
					if (this._newPinPostponement <= 0f && this._newPinCooldown <= 0f)
					{
						this.SetPinCount(this._visiblePins + 1);
						this._newPinCooldown = this._constants.CooldownTimeBetweenNewPins;
					}
				}
				else if (this._visiblePins > this._destination.Model.TotalDemand)
				{
					this.SetPinCount(this._visiblePins - 1);
				}
				float overcrowdingTime = this._destination.Model.GetMidStepOvercrowdingTime(stepAlpha);
				if (!this.IsBigPinVisible)
				{
					if (overcrowdingTime > this._constants.MinOvercrowdingTimeBeforeTimerPin)
					{
						this.IsBigPinVisible = true;
						this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.DestinationOvercrowding, this._destination, true));
						return;
					}
				}
				else
				{
					if (overcrowdingTime <= 0f)
					{
						this.IsBigPinVisible = false;
						this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.DestinationOvercrowding, this._destination, false));
					}
					this._timerPin.SetTime(timeInterval.Delta, overcrowdingTime, this._destination.MaxOvercrowdingTime, gracePeriod, this._destination.Model.IsOvercrowding, TransitionStyle.Tween);
				}
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x060027BF RID: 10175 RVA: 0x000A97FA File Offset: 0x000A79FA
		public Vector2 BigPinAlertPosition
		{
			get
			{
				return this._timerPinCenter.position;
			}
		}

		// Token: 0x060027C0 RID: 10176 RVA: 0x000A980C File Offset: 0x000A7A0C
		public void Upgrade()
		{
			this.IsUpgraded = true;
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x060027C1 RID: 10177 RVA: 0x000A9815 File Offset: 0x000A7A15
		public int VisiblePinCount
		{
			get
			{
				return this._visiblePins;
			}
		}

		// Token: 0x060027C2 RID: 10178 RVA: 0x000A9820 File Offset: 0x000A7A20
		public void SetPinColors(Color color)
		{
			foreach (DestinationPinView destinationPinView in this.pins)
			{
				destinationPinView.SetPinColor(color);
			}
			foreach (DestinationPinView destinationPinView2 in this.overflowPins)
			{
				destinationPinView2.SetPinColor(color);
			}
		}

		// Token: 0x060027C3 RID: 10179 RVA: 0x000A98B4 File Offset: 0x000A7AB4
		public bool RemovePinForVehicleArrival()
		{
			int num = this.SetPinCount(this._visiblePins - 1);
			this._newPinPostponement = this._constants.PostponementForNewPinsAfterPinRemoved;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.VehicleFulfillsDemand, this._destination, true));
			this._timerPin.StartHoldAnimation();
			return num < 0;
		}

		// Token: 0x060027C4 RID: 10180 RVA: 0x000A990C File Offset: 0x000A7B0C
		public void Reset()
		{
			if (this.supportsUpgrading)
			{
				this._animator.SetBool(DestinationPinAnimatorView.IsUpgradedParameterId, false);
			}
			if (this.supportsPinCountParameter)
			{
				this._animator.SetInteger(DestinationPinAnimatorView.PinCountParameterId, 0);
			}
			this._animator.SetBool(DestinationPinAnimatorView.BigPinActiveParameterId, false);
			this.FlushAnimator();
			this._animator.enabled = false;
			this._newPinCooldown = 0f;
			this._visiblePins = -1;
			this._destination = null;
		}

		// Token: 0x060027C5 RID: 10181 RVA: 0x000A9988 File Offset: 0x000A7B88
		private void Awake()
		{
			this._animator.enabled = false;
			foreach (DestinationPinView destinationPinView in this.pins)
			{
				destinationPinView.Hidden += this.OnPinHidden;
			}
			foreach (DestinationPinView destinationPinView2 in this.overflowPins)
			{
				destinationPinView2.Hidden += this.OnPinHidden;
			}
		}

		// Token: 0x060027C6 RID: 10182 RVA: 0x000A9A3C File Offset: 0x000A7C3C
		private void OnEnable()
		{
			foreach (DestinationPinView destinationPinView in this.pins)
			{
				destinationPinView.Reset();
			}
			foreach (DestinationPinView destinationPinView2 in this.overflowPins)
			{
				destinationPinView2.Reset();
			}
			if (this._destination != null && this._destination.Model != null)
			{
				this.SetPinCount(this._destination.Model.TotalDemand);
				this.FlushAnimator();
				foreach (DestinationPinView destinationPinView3 in this.pins)
				{
					destinationPinView3.FlushAnimator();
				}
				foreach (DestinationPinView destinationPinView4 in this.overflowPins)
				{
					destinationPinView4.FlushAnimator();
				}
				this.IsUpgraded = this._destination.Model.IsUpgraded;
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x060027C7 RID: 10183 RVA: 0x000A9BA0 File Offset: 0x000A7DA0
		// (set) Token: 0x060027C8 RID: 10184 RVA: 0x000A9BB4 File Offset: 0x000A7DB4
		private bool IsBigPinVisible
		{
			get
			{
				return this._animator.GetBool(DestinationPinAnimatorView.BigPinActiveParameterId);
			}
			set
			{
				if (this.IsBigPinVisible != value)
				{
					this._animator.enabled = true;
					this._animator.SetBool(DestinationPinAnimatorView.BigPinActiveParameterId, value);
				}
				if (!value)
				{
					this._newPinPostponement = this._constants.PostponementForNewPinsAfterPinRemoved;
					this.SetPinCount(this._visiblePins);
					return;
				}
				this._newPinPostponement = this._constants.PostponementForOverflowPinsAfterBigPin;
			}
		}

		// Token: 0x060027C9 RID: 10185 RVA: 0x000A9C1A File Offset: 0x000A7E1A
		[UsedImplicitly]
		private void OnBigPinFinishAppearing()
		{
			this._destination.CreateBigPinAlert();
			this._newPinPostponement = this._constants.PostponementForOverflowPinsAfterBigPin;
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x060027CA RID: 10186 RVA: 0x000A9C38 File Offset: 0x000A7E38
		// (set) Token: 0x060027CB RID: 10187 RVA: 0x000A9C54 File Offset: 0x000A7E54
		private bool IsUpgraded
		{
			get
			{
				return this.supportsUpgrading && this._animator.GetBool(DestinationPinAnimatorView.IsUpgradedParameterId);
			}
			set
			{
				if (this.supportsUpgrading && this.IsUpgraded != value)
				{
					this._animator.enabled = true;
					this._animator.SetBool(DestinationPinAnimatorView.IsUpgradedParameterId, value);
				}
			}
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x000A9C84 File Offset: 0x000A7E84
		private int SetPinCount(int count)
		{
			int previousVisiblePinCount = this._visiblePins;
			if (this.supportsPinCountParameter)
			{
				this._animator.SetInteger(DestinationPinAnimatorView.PinCountParameterId, count);
			}
			this._visiblePins = count;
			int overflowCount = 0;
			if (count > this._destination.Model.MaximumDemandBeforeTimerStarts)
			{
				overflowCount = count - this._destination.Model.MaximumDemandBeforeTimerStarts;
				count = this._destination.Model.MaximumDemandBeforeTimerStarts;
				if (!this.IsBigPinVisible)
				{
					this._visiblePins = count;
				}
			}
			if (this._visiblePins > 0 && this._visiblePins > previousVisiblePinCount)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.DestinationDemanded, this._destination, true));
			}
			int changeInVisiblePins = 0;
			for (int defaultPinIndex = 0; defaultPinIndex < this.pins.Count; defaultPinIndex++)
			{
				DestinationPinView pin = this.pins[defaultPinIndex];
				bool isPinVisible = pin.IsVisible;
				if (defaultPinIndex < count && !this.IsBigPinVisible)
				{
					if (!isPinVisible)
					{
						changeInVisiblePins++;
					}
					pin.Show();
				}
				else
				{
					if (isPinVisible)
					{
						changeInVisiblePins--;
					}
					pin.Hide();
				}
			}
			for (int overflowPinIndex = 0; overflowPinIndex < this.overflowPins.Count; overflowPinIndex++)
			{
				DestinationPinView pin2 = this.overflowPins[overflowPinIndex];
				bool isPinVisible2 = pin2.IsVisible;
				if (overflowPinIndex < overflowCount && this.IsBigPinVisible)
				{
					if (!isPinVisible2)
					{
						changeInVisiblePins++;
					}
					pin2.Show();
				}
				else
				{
					if (isPinVisible2)
					{
						changeInVisiblePins--;
					}
					pin2.Hide();
				}
			}
			return changeInVisiblePins;
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x000A9DEB File Offset: 0x000A7FEB
		private void OnPinHidden(DestinationPinView pinView)
		{
			if (this._destination != null)
			{
				this._destination.OnPinHidden();
			}
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x000A9E06 File Offset: 0x000A8006
		[UsedImplicitly]
		private void OnSquareIdle()
		{
			if (!this.IsBigPinVisible && !this.IsUpgraded && this.supportsUpgrading)
			{
				this.DisableAnimator();
			}
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x000A9E26 File Offset: 0x000A8026
		[UsedImplicitly]
		private void OnSquareBigPinIdle()
		{
			if (this.IsBigPinVisible && !this.IsUpgraded && this.supportsUpgrading)
			{
				this.DisableAnimator();
			}
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x000A9E46 File Offset: 0x000A8046
		[UsedImplicitly]
		private void OnCircleIdle()
		{
			if (!this.IsBigPinVisible && this.IsUpgraded && this.supportsUpgrading)
			{
				this.DisableAnimator();
			}
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x000A9E66 File Offset: 0x000A8066
		[UsedImplicitly]
		private void OnCircleBigPinIdle()
		{
			if (this.IsBigPinVisible && this.IsUpgraded && this.supportsUpgrading)
			{
				this.DisableAnimator();
			}
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x000A9E86 File Offset: 0x000A8086
		private void DisableAnimator()
		{
			this._animator.enabled = false;
			this._animator.Update(float.Epsilon);
		}

		// Token: 0x060027D3 RID: 10195 RVA: 0x000A9EA4 File Offset: 0x000A80A4
		private void FlushAnimator()
		{
			for (int flushIteration = 0; flushIteration < 6; flushIteration++)
			{
				if (this._animator.gameObject.activeInHierarchy)
				{
					this._animator.Update(2f);
				}
			}
		}

		// Token: 0x0400219C RID: 8604
		public bool supportsUpgrading = true;

		// Token: 0x0400219D RID: 8605
		public bool supportsPinCountParameter;

		// Token: 0x0400219E RID: 8606
		public List<DestinationPinView> pins;

		// Token: 0x0400219F RID: 8607
		public List<DestinationPinView> overflowPins;

		// Token: 0x040021A0 RID: 8608
		[SerializeField]
		private Animator _animator;

		// Token: 0x040021A1 RID: 8609
		[SerializeField]
		private TimerPinView _timerPin;

		// Token: 0x040021A2 RID: 8610
		[SerializeField]
		private Transform _timerPinCenter;

		// Token: 0x040021A3 RID: 8611
		private IAudioSystem _audioSystem;

		// Token: 0x040021A4 RID: 8612
		private VisualConstantsData _constants;

		// Token: 0x040021A5 RID: 8613
		private static readonly int BigPinActiveParameterId = Animator.StringToHash("BigPinActive");

		// Token: 0x040021A6 RID: 8614
		private static readonly int IsUpgradedParameterId = Animator.StringToHash("IsUpgraded");

		// Token: 0x040021A7 RID: 8615
		private static readonly int PinCountParameterId = Animator.StringToHash("PinCount");

		// Token: 0x040021A8 RID: 8616
		private DestinationView _destination;

		// Token: 0x040021A9 RID: 8617
		private float _newPinCooldown;

		// Token: 0x040021AA RID: 8618
		private float _newPinPostponement;

		// Token: 0x040021AB RID: 8619
		private int _visiblePins = -1;

		// Token: 0x040021AC RID: 8620
		private const float MaxAnimationClipDuration = 2f;

		// Token: 0x040021AD RID: 8621
		private const int MaxTransitionsToReachIdleState = 6;
	}
}
