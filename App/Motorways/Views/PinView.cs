using System;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways.Audio;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005F4 RID: 1524
	public class PinView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x06002A6D RID: 10861 RVA: 0x000B9C7B File Offset: 0x000B7E7B
		public void Reset()
		{
			this._transitionTween.Stop();
			this._animationState = PinView.AnimationState.None;
			this._pickup = null;
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06002A6E RID: 10862 RVA: 0x000B9CB8 File Offset: 0x000B7EB8
		public void AppearAtVehicle(VehicleView vehicle, DestinationView fromDestination)
		{
			this._animationState = PinView.AnimationState.AppearingOnVehicle;
			this._transitionTween.Start(0f, 0.4f, 0.4f, Easings.Functions.ElasticEaseOut, 0f);
			this._pickup = vehicle;
			base.transform.localPosition = this._pickup.transform.position;
			base.transform.localScale = Vector3.zero;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateVehicleEvent(AudioEventType.VehicleReceivesPin, vehicle, null, fromDestination, null));
			Theme theme = this._themeDatabase.GetTheme() as Theme;
			this._pinCenter.color = theme.GetBuildingColor(vehicle.groupIndex, ThemeComponentGroupTarget.BuildingTop);
		}

		// Token: 0x06002A6F RID: 10863 RVA: 0x000B9D64 File Offset: 0x000B7F64
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._transitionTween.IsActive)
			{
				float newScale = this._transitionTween.Tick(timeInterval.Delta);
				base.transform.localScale = new Vector3(newScale, newScale, 1f);
				if (!this._transitionTween.IsActive)
				{
					PinView.AnimationState animationState = this._animationState;
					if (animationState != PinView.AnimationState.AppearingOnVehicle)
					{
						if (animationState == PinView.AnimationState.DisappearingFromVehicle)
						{
							return TickResult.Destroy;
						}
					}
					else
					{
						this._animationState = PinView.AnimationState.DisappearingFromVehicle;
						this._transitionTween.Start(base.transform.localScale.x, 0f, 0.4f, Easings.Functions.BackEaseIn, 0.2f);
					}
				}
			}
			if (this._animationState == PinView.AnimationState.AppearingOnVehicle || this._animationState == PinView.AnimationState.DisappearingFromVehicle)
			{
				base.transform.localPosition = this._pickup.transform.position;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x04002476 RID: 9334
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002477 RID: 9335
		[Dependency]
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04002478 RID: 9336
		[SerializeField]
		private SpriteRenderer _pinCenter;

		// Token: 0x04002479 RID: 9337
		private const float SizeOnVehicle = 0.4f;

		// Token: 0x0400247A RID: 9338
		private const float ScaleInDuration = 0.6f;

		// Token: 0x0400247B RID: 9339
		private const float ScaleOutDuration = 0.4f;

		// Token: 0x0400247C RID: 9340
		private const float DisappearFromVehicleDelay = 0.2f;

		// Token: 0x0400247D RID: 9341
		private PinView.AnimationState _animationState;

		// Token: 0x0400247E RID: 9342
		private TweenFloat _transitionTween = new TweenFloat();

		// Token: 0x0400247F RID: 9343
		private VehicleView _pickup;

		// Token: 0x020005F5 RID: 1525
		private enum AnimationState
		{
			// Token: 0x04002481 RID: 9345
			None,
			// Token: 0x04002482 RID: 9346
			AppearingOnVehicle,
			// Token: 0x04002483 RID: 9347
			DisappearingFromVehicle
		}
	}
}
