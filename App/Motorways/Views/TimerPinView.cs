using System;
using Easing;
using Factory;
using Motorways.Audio;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000607 RID: 1543
	public class TimerPinView : MonoBehaviour
	{
		// Token: 0x06002B1E RID: 11038 RVA: 0x000BDB60 File Offset: 0x000BBD60
		public void Initialize(IScope scope)
		{
			this._simulation = scope.Get<ISimulation>();
			this._audioSystem = scope.Get<IAudioSystem>();
			this._gameCamera = scope.Get<GameCamera>();
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000BDB88 File Offset: 0x000BBD88
		public void Reset()
		{
			this._innerPinAnimationState = TimerPinView.InnerPinState.None;
			this._innerPinColorTween.Stop();
			this._innerPinScaleTween.Stop();
			this._lastTimerProgress = 0f;
			this._timeSinceAlert = 0f;
			this._isNextAlertInitial = false;
			this._isHoldingProgress = false;
			this._holdProgress = 0f;
			this._holdTimer = 0f;
			this._holdProgressAtCollapse = 0f;
			this._holdCollapseTween.Reset();
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x000BDC04 File Offset: 0x000BBE04
		public void StartHoldAnimation()
		{
			this._isHoldingProgress = true;
			this._holdProgress = Mathf.Max(this._holdProgress, this._lastTimerProgress);
			if (this._holdProgress > 0f)
			{
				this._holdTimer = this.HoldDuration;
			}
			this._holdCollapseTween.Reset();
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x000BDC54 File Offset: 0x000BBE54
		public void SetTime(float tickTime, float time, float maxTime, float graceTime, bool isIncreasing, TransitionStyle transitionStyle)
		{
			float rawTimerProgress = time / (maxTime - graceTime);
			float timerProgress = this._timerCurve.Evaluate(Mathf.Clamp01(rawTimerProgress));
			bool isTimerFull = time > maxTime - graceTime;
			if (isTimerFull)
			{
				this._timerRenderer.material.SetFloat(TimerPinView.InnerProgressPropertyId, rawTimerProgress);
			}
			else
			{
				this._timerRenderer.material.SetFloat(TimerPinView.InnerProgressPropertyId, timerProgress);
			}
			float visibleHoldProgress;
			if (this._isHoldingProgress)
			{
				if (this._holdProgress > timerProgress)
				{
					if (this._holdCollapseTween.IsActive)
					{
						this._holdCollapseTween.Tick(tickTime);
						this._holdProgress = Mathf.Lerp(this._holdProgressAtCollapse, timerProgress, Easings.Interpolate(this._holdCollapseTween.Value, this._holdCollapseEasing));
						if (!this._holdCollapseTween.IsActive)
						{
							this._isHoldingProgress = false;
						}
					}
					else
					{
						this._holdTimer -= tickTime;
						if (this._holdTimer <= 0f)
						{
							this._holdTimer = 0f;
							this._holdCollapseTween.Start(0f, 1f, (this._holdProgress - timerProgress) / this.HoldCollapseSpeed, Easings.Functions.Linear, 0f);
							this._holdProgressAtCollapse = this._holdProgress;
						}
					}
				}
				else
				{
					this._isHoldingProgress = false;
					this._holdTimer = 0f;
					this._holdProgress = timerProgress;
					this._holdCollapseTween.Reset();
				}
				visibleHoldProgress = this._holdProgress;
			}
			else
			{
				this._holdProgress = timerProgress;
				visibleHoldProgress = (isTimerFull ? rawTimerProgress : this._holdProgress);
			}
			this._timerRenderer.material.SetFloat(TimerPinView.OuterProgressPropertyId, visibleHoldProgress);
			float innerPinScale = isIncreasing ? 0.69f : 1f;
			Color increasingColor = this.InnerPinIncreasingColor.Evaluate(timerProgress);
			Color innerPinColor = isIncreasing ? increasingColor : this.InnerPinDecreasingColor;
			Color increasingReductionColor = this.InnerPinIncreasingHoldColor.Evaluate(timerProgress);
			Color decreasingReductionColor = this.InnerPinDecreasingHoldColor.Evaluate(timerProgress);
			Color reductionColor = isIncreasing ? increasingReductionColor : decreasingReductionColor;
			TimerPinView.InnerPinState newInnerPinAnimationState = isIncreasing ? TimerPinView.InnerPinState.IncreasingScale : TimerPinView.InnerPinState.DecreasingScale;
			if (newInnerPinAnimationState != this._innerPinAnimationState)
			{
				this._innerPinAnimationState = newInnerPinAnimationState;
				if (transitionStyle == TransitionStyle.Tween && this._lastTimerProgress > 0f)
				{
					this._innerPinScaleTween.Start(this._timerPinInterior.localScale.x, innerPinScale, 0.6f, Easings.Functions.SineEaseInOut, 0f);
					this._innerPinColorTween.Start(this._innerPinColorTween.Value, isIncreasing ? 0f : 1f, 0.6f, Easings.Functions.SineEaseInOut, 0f);
				}
				else
				{
					this._innerPinScaleTween.Stop();
					this._innerPinColorTween.Stop();
				}
			}
			if (this._innerPinScaleTween.IsActive)
			{
				this._innerPinScaleTween.Tick(tickTime);
				this._innerPinColorTween.Tick(tickTime);
				innerPinScale = this._innerPinScaleTween.Value;
				innerPinColor = Color.Lerp(increasingColor, this.InnerPinDecreasingColor, this._innerPinColorTween.Value);
				reductionColor = Color.Lerp(increasingReductionColor, decreasingReductionColor, this._innerPinColorTween.Value);
			}
			this._timerPinInterior.localScale = new Vector3(innerPinScale, innerPinScale, 1f);
			this._timerRenderer.material.SetColor(TimerPinView.InnerColorPropertyId, innerPinColor);
			this._timerRenderer.material.SetColor(TimerPinView.OuterColorPropertyId, reductionColor);
			if (!this._simulation.IsPaused)
			{
				this.UpdateAlertTimer(tickTime, timerProgress, isIncreasing);
			}
			this._lastTimerProgress = timerProgress;
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x000BDFA4 File Offset: 0x000BC1A4
		private void UpdateAlertTimer(float tickTime, float timerProgress, bool isTimerIncreasing)
		{
			if (!this._simulation.IsPaused)
			{
				this._timeSinceAlert += tickTime;
			}
			if (timerProgress < this.MinimumProgressForAlert || !isTimerIncreasing)
			{
				this._isNextAlertInitial = true;
				return;
			}
			float timeBetweenAlerts = Mathf.Lerp(this.MaximumTimeBetweenAlerts, this.MinimumTimeBetweenAlerts, (timerProgress - this.MinimumProgressForAlert) / (1f - this.MinimumProgressForAlert));
			if (this._timeSinceAlert > timeBetweenAlerts)
			{
				this._destinationView.CreateImminentFailAlert(this._isNextAlertInitial);
				this._isNextAlertInitial = false;
				this._audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.RippleAlert, this._destinationView, true));
				this._timeSinceAlert = 0f;
			}
		}

		// Token: 0x04002521 RID: 9505
		private const float InnerPinTransitionDuration = 0.6f;

		// Token: 0x04002522 RID: 9506
		private const float InnerPinIncreasingScale = 0.69f;

		// Token: 0x04002523 RID: 9507
		private const float InnerPinDecreasingScale = 1f;

		// Token: 0x04002524 RID: 9508
		[Tooltip("The time a large, instant reduction in the timer will be highlighted and held. This does not include the collapse duration.")]
		[SerializeField]
		private float HoldDuration = 0.5f;

		// Token: 0x04002525 RID: 9509
		[Tooltip("How quickly, in units / second, a held portion of the timer will collapse once the hold duration has ended.")]
		[SerializeField]
		private float HoldCollapseSpeed = 0.05f;

		// Token: 0x04002526 RID: 9510
		[SerializeField]
		private Easings.Functions _holdCollapseEasing;

		// Token: 0x04002527 RID: 9511
		[Tooltip("How far through the timer must be before it will send an alerts. This should be a value from 0 to 1.")]
		[SerializeField]
		private float MinimumProgressForAlert = 0.5f;

		// Token: 0x04002528 RID: 9512
		[Tooltip("The time between alerts when the timer is at the minimum for alerts.")]
		[SerializeField]
		private float MaximumTimeBetweenAlerts = 10f;

		// Token: 0x04002529 RID: 9513
		[SerializeField]
		[Tooltip("The time between alerts when the timer is full.")]
		private float MinimumTimeBetweenAlerts = 2f;

		// Token: 0x0400252A RID: 9514
		[SerializeField]
		[Tooltip("The colour of the timer while it is ticking up.")]
		private Gradient InnerPinIncreasingColor;

		// Token: 0x0400252B RID: 9515
		[SerializeField]
		[Tooltip("The colour of the timer while it is ticking down.")]
		private Color InnerPinDecreasingColor = Color.white;

		// Token: 0x0400252C RID: 9516
		[SerializeField]
		[Tooltip("The colour of the timer section that is removed when a vehicle picks a pin up from the destination, while the timer is ticking up.")]
		private Gradient InnerPinIncreasingHoldColor;

		// Token: 0x0400252D RID: 9517
		[SerializeField]
		[Tooltip("The colour of the timer section that is removed when a vehicle picks a pin up from the destination, while the timer is ticking down.")]
		private Gradient InnerPinDecreasingHoldColor;

		// Token: 0x0400252E RID: 9518
		[SerializeField]
		private AnimationCurve _timerCurve = new AnimationCurve();

		// Token: 0x0400252F RID: 9519
		private readonly TweenFloat _innerPinScaleTween = new TweenFloat();

		// Token: 0x04002530 RID: 9520
		private readonly TweenFloat _innerPinColorTween = new TweenFloat();

		// Token: 0x04002531 RID: 9521
		private float _lastTimerProgress;

		// Token: 0x04002532 RID: 9522
		private float _timeSinceAlert;

		// Token: 0x04002533 RID: 9523
		private bool _isNextAlertInitial;

		// Token: 0x04002534 RID: 9524
		private bool _isHoldingProgress;

		// Token: 0x04002535 RID: 9525
		private float _holdProgress;

		// Token: 0x04002536 RID: 9526
		private float _holdTimer;

		// Token: 0x04002537 RID: 9527
		private float _holdProgressAtCollapse;

		// Token: 0x04002538 RID: 9528
		private readonly TweenFloat _holdCollapseTween = new TweenFloat();

		// Token: 0x04002539 RID: 9529
		[SerializeField]
		private DestinationView _destinationView;

		// Token: 0x0400253A RID: 9530
		[SerializeField]
		private MeshRenderer _timerRenderer;

		// Token: 0x0400253B RID: 9531
		[SerializeField]
		private Transform _timerPinInterior;

		// Token: 0x0400253C RID: 9532
		private ISimulation _simulation;

		// Token: 0x0400253D RID: 9533
		private IAudioSystem _audioSystem;

		// Token: 0x0400253E RID: 9534
		private GameCamera _gameCamera;

		// Token: 0x0400253F RID: 9535
		private static readonly int InnerProgressPropertyId = Shader.PropertyToID("_InnerProgress");

		// Token: 0x04002540 RID: 9536
		private static readonly int InnerColorPropertyId = Shader.PropertyToID("_InnerColor");

		// Token: 0x04002541 RID: 9537
		private static readonly int OuterProgressPropertyId = Shader.PropertyToID("_OuterProgress");

		// Token: 0x04002542 RID: 9538
		private static readonly int OuterColorPropertyId = Shader.PropertyToID("_OuterColor");

		// Token: 0x04002543 RID: 9539
		private TimerPinView.InnerPinState _innerPinAnimationState;

		// Token: 0x02000608 RID: 1544
		private enum InnerPinState
		{
			// Token: 0x04002545 RID: 9541
			None,
			// Token: 0x04002546 RID: 9542
			DecreasingScale,
			// Token: 0x04002547 RID: 9543
			IncreasingScale
		}
	}
}
