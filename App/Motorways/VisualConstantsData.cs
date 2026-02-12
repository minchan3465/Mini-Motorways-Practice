using System;
using System.Collections.Generic;
using Easing;
using JetBrains.Annotations;
using Motorways.Themes;
using Motorways.Utility;
using NaughtyAttributes;
using Screens;
using UnityEngine;
using UnityEngine.Serialization;

namespace Motorways
{
	// Token: 0x0200045F RID: 1119
	[CreateAssetMenu(menuName = "Motorways/VisualConstants")]
	public class VisualConstantsData : ScriptableObject
	{
		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06001C02 RID: 7170 RVA: 0x000671F0 File Offset: 0x000653F0
		public int ProfileIconCount
		{
			get
			{
				return this._profileIconOptions.Length;
			}
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x000671FC File Offset: 0x000653FC
		public Sprite GetProfileIcon(int iconIndex)
		{
			if (this._profileIconOptions == null || this._profileIconOptions.Length == 0)
			{
				return null;
			}
			if (!Diagnostics.Verify(iconIndex >= 0, "Index {0} is invalid!", iconIndex) || !Diagnostics.Verify(iconIndex < this.ProfileIconCount, "Trying to get an icon for {0} when we only have {1}", iconIndex, this.ProfileIconCount))
			{
				iconIndex = 0;
			}
			return this._profileIconOptions[iconIndex];
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x00067268 File Offset: 0x00065468
		public Vector3 GetCameraPositionForTransitionToGame(ScreenTransition transition, float transitionInPercentage, CityDefinition city)
		{
			float oneOverPercentageToUseForFirstHalf = 1f / this.PercentageOfDurationToUseForInitialMovement;
			if (transitionInPercentage < this.PercentageOfDurationToUseForInitialMovement)
			{
				float lerp = this.FirstPartTransitionInEaseCurve.Evaluate(transitionInPercentage * oneOverPercentageToUseForFirstHalf);
				Vector2 inPoint = transition.spline.inPoint;
				Vector2 partOneEnd = inPoint + this.MapSelectScreenExitOffset;
				return Vector3.Lerp(inPoint, partOneEnd, lerp);
			}
			float oneOverPercentageToUseForSecondHalf = 1f / (1f - this.PercentageOfDurationToUseForInitialMovement);
			Vector2 end = transition.spline.outPoint;
			Vector2 start = end + city.cameraZoom.cameraEntryPosition;
			return Spline.EvaluateBezier(this.SecondPartTransitionInEaseCurve.Evaluate((transitionInPercentage - this.PercentageOfDurationToUseForInitialMovement) * oneOverPercentageToUseForSecondHalf), start, start, city.cameraZoom.cameraEntrySplineHandle + end, end);
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x0006733C File Offset: 0x0006553C
		public Vector3 GetCameraPositionForTransitionFromGame(ScreenTransition transition, float transitionInPercentage, Vector2 cameraPosition, Vector2 cameraHandle)
		{
			float invertedPercentage = 1f - this.PercentageOfDurationToUseForInitialMovement;
			if (transitionInPercentage > invertedPercentage)
			{
				float oneOverPercentage = 1f / this.PercentageOfDurationToUseForInitialMovement;
				Vector2 end = transition.spline.outPoint;
				Vector2 start = end + this.MapSelectScreenEntryOffset;
				float time = (transitionInPercentage - invertedPercentage) * oneOverPercentage;
				return Spline.EvaluateBezier(this.SecondPartTransitionOutEaseCurve.Evaluate(time), start, start + this.MapSelectScreenEntryStartHandle, this.MapSelectScreenEntryEndHandle + end, end);
			}
			float oneOverPercentage2 = 1f / invertedPercentage;
			Vector2 start2 = transition.spline.inPoint;
			Vector2 end2 = start2 + cameraPosition;
			return Spline.EvaluateBezier(this.FirstPartTransitionOutEaseCurve.Evaluate(transitionInPercentage * oneOverPercentage2), start2, start2 + cameraHandle, end2, end2);
		}

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001C06 RID: 7174 RVA: 0x00067404 File Offset: 0x00065604
		// (remove) Token: 0x06001C07 RID: 7175 RVA: 0x0006743C File Offset: 0x0006563C
		public event Action OnExpertPermanenceDebugZoneIndexChanged;

		// Token: 0x06001C08 RID: 7176 RVA: 0x00067471 File Offset: 0x00065671
		[UsedImplicitly]
		public void OnExpertPermanenceDebugZoneIndexChangedCallback()
		{
			Action onExpertPermanenceDebugZoneIndexChanged = this.OnExpertPermanenceDebugZoneIndexChanged;
			if (onExpertPermanenceDebugZoneIndexChanged == null)
			{
				return;
			}
			onExpertPermanenceDebugZoneIndexChanged();
		}

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06001C09 RID: 7177 RVA: 0x00067484 File Offset: 0x00065684
		// (remove) Token: 0x06001C0A RID: 7178 RVA: 0x000674BC File Offset: 0x000656BC
		public event Action OnExpertPermanenceDebugViewOpacityChanged;

		// Token: 0x06001C0B RID: 7179 RVA: 0x000674F1 File Offset: 0x000656F1
		[UsedImplicitly]
		private void OnExpertPermanenceDebugViewOpacityChangedCallback()
		{
			Action onExpertPermanenceDebugViewOpacityChanged = this.OnExpertPermanenceDebugViewOpacityChanged;
			if (onExpertPermanenceDebugViewOpacityChanged == null)
			{
				return;
			}
			onExpertPermanenceDebugViewOpacityChanged();
		}

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06001C0C RID: 7180 RVA: 0x00067504 File Offset: 0x00065704
		// (remove) Token: 0x06001C0D RID: 7181 RVA: 0x0006753C File Offset: 0x0006573C
		public event Action OnExpertRemoveHarshAnglesChanged;

		// Token: 0x06001C0E RID: 7182 RVA: 0x00067571 File Offset: 0x00065771
		[UsedImplicitly]
		private void OnExpertRemoveHarshAnglesChangedCallback()
		{
			Action onExpertRemoveHarshAnglesChanged = this.OnExpertRemoveHarshAnglesChanged;
			if (onExpertRemoveHarshAnglesChanged == null)
			{
				return;
			}
			onExpertRemoveHarshAnglesChanged();
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06001C0F RID: 7183 RVA: 0x00067584 File Offset: 0x00065784
		// (remove) Token: 0x06001C10 RID: 7184 RVA: 0x000675BC File Offset: 0x000657BC
		public event Action OnExpertPermanentRoadFadeLengthChanged;

		// Token: 0x06001C11 RID: 7185 RVA: 0x000675F1 File Offset: 0x000657F1
		[UsedImplicitly]
		private void OnExpertPermanentRoadFadeLengthChangedCallback()
		{
			Action onExpertPermanentRoadFadeLengthChanged = this.OnExpertPermanentRoadFadeLengthChanged;
			if (onExpertPermanentRoadFadeLengthChanged == null)
			{
				return;
			}
			onExpertPermanentRoadFadeLengthChanged();
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x00067603 File Offset: 0x00065803
		private void OnValidate()
		{
			if (this.ControllerSpeedSensitivityOptions.Length != 5)
			{
				Debug.LogWarning("ControllerSpeedSensitivityOptions supports exactly 5 options");
				Array.Resize<float>(ref this.ControllerSpeedSensitivityOptions, 5);
			}
		}

		// Token: 0x04001744 RID: 5956
		[SerializeField]
		[Tooltip("The icons players can pick to associate with profiles.")]
		private Sprite[] _profileIconOptions;

		// Token: 0x04001745 RID: 5957
		public const int MaxProfileColors = 6;

		// Token: 0x04001746 RID: 5958
		private const string InGameMessagesGroup = "In-game Messages";

		// Token: 0x04001747 RID: 5959
		[FoldoutGroup("In-game Messages")]
		public Easings.Functions InGameMessageAppearEasingFunction;

		// Token: 0x04001748 RID: 5960
		[FoldoutGroup("In-game Messages")]
		public float InGameMessageAppearEasingDuration = 0.6f;

		// Token: 0x04001749 RID: 5961
		[FoldoutGroup("In-game Messages")]
		public Sprite InGameMessageDismissIcon;

		// Token: 0x0400174A RID: 5962
		[FoldoutGroup("In-game Messages")]
		public Sprite InGameMessageQueuedIcon;

		// Token: 0x0400174B RID: 5963
		private const string InGameErrorMessagesGroup = "In-game Error Messages";

		// Token: 0x0400174C RID: 5964
		[FoldoutGroup("In-game Error Messages")]
		public float RepeatRecentErrorTimeWindow = 5f;

		// Token: 0x0400174D RID: 5965
		[FoldoutGroup("In-game Error Messages")]
		[Tooltip("If you repeat an error this meany times within the recent error time window, it will show a text message as well.")]
		public int RepeatRecentErrorCount = 3;

		// Token: 0x0400174E RID: 5966
		[FoldoutGroup("In-game Error Messages")]
		public float TimeAfterIconAppearsWhenNotificationAppears = 1f;

		// Token: 0x0400174F RID: 5967
		private const string AlertIndicators = "Alert Indicators";

		// Token: 0x04001750 RID: 5968
		[FoldoutGroup("Alert Indicators")]
		public ThemedMaterialType BuildingEchoAlertColor = ThemedMaterialType.Dark;

		// Token: 0x04001751 RID: 5969
		[FoldoutGroup("Alert Indicators")]
		public ThemedMaterialType UpgradeAlertColor = ThemedMaterialType.Dark;

		// Token: 0x04001752 RID: 5970
		private const string DestinationPinTiming = "Destination Pin Timing";

		// Token: 0x04001753 RID: 5971
		[FoldoutGroup("Destination Pin Timing")]
		[MinValue(0)]
		[Tooltip("Delay between visually adding new pins to the same destination. A value of 0 means a pin will always appear the moment the simulation adds a pin.")]
		public float CooldownTimeBetweenNewPins = 0.5f;

		// Token: 0x04001754 RID: 5972
		[FoldoutGroup("Destination Pin Timing")]
		[MinValue(0)]
		[Tooltip("Delay before we visibly add a pin after one is removed. Zero means it will appear instantly.")]
		public float PostponementForNewPinsAfterPinRemoved = 2f;

		// Token: 0x04001755 RID: 5973
		[Tooltip("Delay before we visibly add an overflow pin after big-pinning. Zero means it will appear instantly.")]
		[MinValue(0)]
		[FoldoutGroup("Destination Pin Timing")]
		public float PostponementForOverflowPinsAfterBigPin = 2f;

		// Token: 0x04001756 RID: 5974
		[FoldoutGroup("Destination Pin Timing")]
		[Tooltip("What value does the big pin timer have to be at before we show the big pin? The lower this is the more you risk the timer pin rapidly animating in and out if the destination is hovering around its maximum pin count.")]
		[MinValue(0)]
		public float MinOvercrowdingTimeBeforeTimerPin = 0.2f;

		// Token: 0x04001757 RID: 5975
		private const string TransitionIntoGame = "Transition Into Game";

		// Token: 0x04001758 RID: 5976
		[FoldoutGroup("Transition Into Game")]
		public AnimationCurve FirstPartTransitionInEaseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x04001759 RID: 5977
		[FoldoutGroup("Transition Into Game")]
		public AnimationCurve SecondPartTransitionInEaseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x0400175A RID: 5978
		[FoldoutGroup("Transition Into Game")]
		public AnimationCurve FirstPartTransitionOutEaseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x0400175B RID: 5979
		[FoldoutGroup("Transition Into Game")]
		public AnimationCurve SecondPartTransitionOutEaseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x0400175C RID: 5980
		[FoldoutGroup("Transition Into Game")]
		[Slider(0, 1)]
		public float PercentageOfDurationToUseForInitialMovement = 0.5f;

		// Token: 0x0400175D RID: 5981
		[FoldoutGroup("Transition Into Game")]
		public Vector2 MapSelectScreenExitOffset = new Vector2(100f, 0f);

		// Token: 0x0400175E RID: 5982
		[FoldoutGroup("Transition Into Game")]
		public Vector2 MapSelectScreenEntryOffset = new Vector2(0f, 100f);

		// Token: 0x0400175F RID: 5983
		[FoldoutGroup("Transition Into Game")]
		public Vector2 MapSelectScreenEntryStartHandle = new Vector2(0f, 0f);

		// Token: 0x04001760 RID: 5984
		[FoldoutGroup("Transition Into Game")]
		public Vector2 MapSelectScreenEntryEndHandle = new Vector2(0f, 0f);

		// Token: 0x04001761 RID: 5985
		private const string RoadAnimation = "Road Animation";

		// Token: 0x04001762 RID: 5986
		[FoldoutGroup("Road Animation")]
		public float AppearDuration = 1f;

		// Token: 0x04001763 RID: 5987
		[FoldoutGroup("Road Animation")]
		public AnimationCurve OutlineAppearCurve;

		// Token: 0x04001764 RID: 5988
		[FoldoutGroup("Road Animation")]
		public AnimationCurve InnerAppearCurve;

		// Token: 0x04001765 RID: 5989
		[FoldoutGroup("Road Animation")]
		public float DisappearDuration = 0.2f;

		// Token: 0x04001766 RID: 5990
		[FoldoutGroup("Road Animation")]
		public AnimationCurve DisappearCurve;

		// Token: 0x04001767 RID: 5991
		[FoldoutGroup("Road Animation")]
		[Tooltip("How long it takes a dead end to emerge from a connection that is mothballed or removed.")]
		public float DeadEndEmergeDuration = 0.2f;

		// Token: 0x04001768 RID: 5992
		[FoldoutGroup("Road Animation")]
		public Easings.Functions DeadEndEmergeEasingFunction;

		// Token: 0x04001769 RID: 5993
		[FoldoutGroup("Road Animation")]
		[Tooltip("How long it takes a dead end to collapse into the connection that replaces it.")]
		public float DeadEndCollapseDuration = 0.2f;

		// Token: 0x0400176A RID: 5994
		[FoldoutGroup("Road Animation")]
		public Easings.Functions DeadEndCollapseEasingFunction;

		// Token: 0x0400176B RID: 5995
		[Tooltip("How long it takes a dead end that is being edited to assume its manual distortion.")]
		[FoldoutGroup("Road Animation")]
		public float DeadEndEditDistortionStartDuration = 0.2f;

		// Token: 0x0400176C RID: 5996
		[FoldoutGroup("Road Animation")]
		public Easings.Functions DeadEndEditDistortionStartEasingFunction;

		// Token: 0x0400176D RID: 5997
		[FoldoutGroup("Road Animation")]
		[Tooltip("How long it takes a dead end that is being manually distorted to return to its natural state if the distortion is cancelled.")]
		public float DeadEndEditDistortionReturnDuration = 0.2f;

		// Token: 0x0400176E RID: 5998
		[FoldoutGroup("Road Animation")]
		public Easings.Functions DeadEndEditDistortionReturnEasingFunction;

		// Token: 0x0400176F RID: 5999
		[FoldoutGroup("Road Animation")]
		public float RoadDrawingStepDistance = 1.65f;

		// Token: 0x04001770 RID: 6000
		[FoldoutGroup("Road Animation")]
		public float DiagonalRoadDrawingStepDistance = 1.65f;

		// Token: 0x04001771 RID: 6001
		[FoldoutGroup("Road Animation")]
		public float InteractionCircleOffsetAdjustmentDuration = 0.1f;

		// Token: 0x04001772 RID: 6002
		[FoldoutGroup("Road Animation")]
		public float TrafficLightsOffsetAdjustmentDuration = 0.1f;

		// Token: 0x04001773 RID: 6003
		[FoldoutGroup("Road Animation")]
		public Easings.Functions InteractionCircleAndTrafficLightAdjustmentEasingFunction;

		// Token: 0x04001774 RID: 6004
		private const string RoadPermanence = "Road Permanence";

		// Token: 0x04001775 RID: 6005
		[FoldoutGroup("Road Permanence")]
		public AnimationCurve DryingRoadFalloff;

		// Token: 0x04001776 RID: 6006
		[FoldoutGroup("Road Permanence")]
		public AnimationCurve DryingMotorwayHazardStripesFalloff;

		// Token: 0x04001777 RID: 6007
		[FoldoutGroup("Road Permanence")]
		public AnimationCurve DryingTunnelFalloff;

		// Token: 0x04001778 RID: 6008
		[FoldoutGroup("Road Permanence")]
		public float MaxDryingTunnelOpacity;

		// Token: 0x04001779 RID: 6009
		[FoldoutGroup("Road Permanence")]
		public AnimationCurve DryingInteractionCircleFalloff;

		// Token: 0x0400177A RID: 6010
		private const string UpgradeBar = "Upgrade Bar";

		// Token: 0x0400177B RID: 6011
		[FoldoutGroup("Upgrade Bar")]
		[Tooltip("This is the space between each item in the upgrade list")]
		public float UpgradeBarSeparationPadding;

		// Token: 0x0400177C RID: 6012
		[Tooltip("This is the space between each item in the upgrade list with a count over 1")]
		[FoldoutGroup("Upgrade Bar")]
		public float UpgradeBarSeparationPaddingWithCount;

		// Token: 0x0400177D RID: 6013
		[Tooltip("this is an extra bit of padding right side of the concrete button")]
		[FoldoutGroup("Upgrade Bar")]
		public float UpgradeBarRightSeparationPadding;

		// Token: 0x0400177E RID: 6014
		[FoldoutGroup("Upgrade Bar")]
		[Tooltip("This is an extra bit of padding left side of the concrete button")]
		public float UpgradeBarLeftInactiveSeparationPadding;

		// Token: 0x0400177F RID: 6015
		private const string Themes = "Themes";

		// Token: 0x04001780 RID: 6016
		[FoldoutGroup("Themes")]
		public ColorGroup[] AvailableColorfulColorBlindColorGroups;

		// Token: 0x04001781 RID: 6017
		[FoldoutGroup("Themes")]
		public ColorGroup[] AvailableDarkColorBlindColorGroups;

		// Token: 0x04001782 RID: 6018
		private const string VehicleHeadlights = "Vehicle Headlights";

		// Token: 0x04001783 RID: 6019
		[FoldoutGroup("Vehicle Headlights")]
		[Min(0f)]
		[Tooltip("The time it will take most vehicles to turn their headlights on/off after a day/night mode change")]
		public float MeanVehicleHeadlightResponseTime = 5f;

		// Token: 0x04001784 RID: 6020
		[FoldoutGroup("Vehicle Headlights")]
		[Min(0f)]
		[Tooltip("How spread out times are from the mean. Low values mean most response times will be around the mean, higher values mean they will be more spread out.")]
		public float StandardDeviationVehicleHeadlightResponseTime = 1f;

		// Token: 0x04001785 RID: 6021
		private const string Trains = "Trains";

		// Token: 0x04001786 RID: 6022
		[Tooltip("How far away the train shadow is offset from the train")]
		[Min(0f)]
		[FoldoutGroup("Trains")]
		public float TrainShadowOffset = 0.5f;

		// Token: 0x04001787 RID: 6023
		[Tooltip("How long in game time the transition between the front/back headlights takes when a train switches direction")]
		[FoldoutGroup("Trains")]
		[Min(0f)]
		public float TrainHeadlightSwitchTransitionTime = 0.5f;

		// Token: 0x04001788 RID: 6024
		[FoldoutGroup("Trains")]
		public Easings.Functions TrainHeadlightSwitchTransitionEasingFunction;

		// Token: 0x04001789 RID: 6025
		[FoldoutGroup("Trains")]
		[Min(0f)]
		[Tooltip("How long in game time the train headlights take to turn on/off when toggling day/night mode")]
		public float TrainHeadlightDayNightTransitionTime = 0.5f;

		// Token: 0x0400178A RID: 6026
		[FoldoutGroup("Trains")]
		public Easings.Functions TrainHeadlightDayNightTransitionEasingFunction;

		// Token: 0x0400178B RID: 6027
		private const string ControllerSpeed = "Controller Speed";

		// Token: 0x0400178C RID: 6028
		[FoldoutGroup("Controller Speed")]
		public AnimationCurve ControllerAccelerationCurve;

		// Token: 0x0400178D RID: 6029
		[Tooltip("A 0->1 curve that determines how much to use the ControllerAcclerationCurve over camera zoom.")]
		[FoldoutGroup("Controller Speed")]
		public AnimationCurve BaseControllerSpeedOverZoom;

		// Token: 0x0400178E RID: 6030
		[FoldoutGroup("Controller Speed")]
		public float BaseControllerSpeed = 0.185f;

		// Token: 0x0400178F RID: 6031
		private const string ControllerCamera = "Controller Camera";

		// Token: 0x04001790 RID: 6032
		[FoldoutGroup("Controller Camera")]
		public bool ShowGridOnZoom = true;

		// Token: 0x04001791 RID: 6033
		[FoldoutGroup("Controller Camera")]
		public List<float> ZoomLevelsCameraMin = new List<float>();

		// Token: 0x04001792 RID: 6034
		[FoldoutGroup("Controller Camera")]
		[Tooltip("This needs to have the same number of elements as ZoomLevelsCameraMin as it will correspond with those indexes")]
		public List<float> PanningSpeedPerZoomLevel = new List<float>();

		// Token: 0x04001793 RID: 6035
		[FoldoutGroup("Controller Speed")]
		public float[] ControllerSpeedSensitivityOptions = new float[]
		{
			0.3f,
			0.5f,
			1f,
			1.5f,
			2f
		};

		// Token: 0x04001794 RID: 6036
		private const string ButtonColors = "Button Colours";

		// Token: 0x04001795 RID: 6037
		[FoldoutGroup("Button Colours")]
		public Color NormalTabButtonColor = new Color(0.5921569f, 0.7450981f, 0.7490196f, 1f);

		// Token: 0x04001796 RID: 6038
		[FoldoutGroup("Button Colours")]
		public Color EndlessTabButtonColor = new Color(0.9137256f, 0.7450981f, 0.7960785f, 1f);

		// Token: 0x04001797 RID: 6039
		[FoldoutGroup("Button Colours")]
		public Color ExpertTabButtonColor = new Color(0.937255f, 0.7254902f, 0.5372549f, 1f);

		// Token: 0x04001798 RID: 6040
		[FoldoutGroup("Button Colours")]
		public Color CreativeTabButtonColor = new Color(0.537255f, 0.7554902f, 0.4372549f, 1f);

		// Token: 0x04001799 RID: 6041
		private const string ExpertPermanentRoads = "Expert Permanent Roads";

		// Token: 0x0400179A RID: 6042
		[Slider(0.01f, 1f)]
		[OnValueChanged("OnExpertPermanentRoadFadeLengthChangedCallback")]
		[FoldoutGroup("Expert Permanent Roads")]
		public float ExpertPermanentRoadsFadeLength = 0.25f;

		// Token: 0x0400179B RID: 6043
		[FoldoutGroup("Expert Permanent Roads")]
		[Slider(0f, 1f)]
		public float ExpertPermanentRoadsFadeDuration = 0.25f;

		// Token: 0x0400179C RID: 6044
		[FoldoutGroup("Expert Permanent Roads")]
		[OnValueChanged("OnExpertRemoveHarshAnglesChangedCallback")]
		public bool RemoveHarshAngles = true;

		// Token: 0x0400179D RID: 6045
		[FoldoutGroup("Expert Permanent Roads")]
		[OnValueChanged("OnExpertPermanenceDebugViewOpacityChangedCallback")]
		[Slider(0f, 1f)]
		public float PermanenceDebugViewOpacity;

		// Token: 0x0400179E RID: 6046
		[FoldoutGroup("Expert Permanent Roads")]
		[OnValueChanged("OnExpertPermanenceDebugZoneIndexChangedCallback")]
		[Slider(0, 250)]
		public int PermanenceDebugViewZoneIndex;

		// Token: 0x0400179F RID: 6047
		private const string Rails = "Rails";

		// Token: 0x040017A0 RID: 6048
		[FoldoutGroup("Rails")]
		public float RailWidth = 0.1f;

		// Token: 0x040017A1 RID: 6049
		[FoldoutGroup("Rails")]
		public float RailNotchWidth = 0.2f;

		// Token: 0x040017A2 RID: 6050
		[FoldoutGroup("Rails")]
		public float RailNotchLength = 0.6f;

		// Token: 0x040017A3 RID: 6051
		[FoldoutGroup("Rails")]
		public float RailEndLength = 1.4f;

		// Token: 0x040017A4 RID: 6052
		[FoldoutGroup("Rails")]
		public float RailNotchCornerRadius = 0.02f;

		// Token: 0x040017A5 RID: 6053
		private const string BoatPaths = "BoatPaths";

		// Token: 0x040017A6 RID: 6054
		[FoldoutGroup("BoatPaths")]
		public float BoatPathWidth = 0.1f;

		// Token: 0x040017A7 RID: 6055
		[FoldoutGroup("BoatPaths")]
		public float BoatPathDashLength = 0.1f;

		// Token: 0x040017A8 RID: 6056
		[FoldoutGroup("BoatPaths")]
		public float BoatPathDashGapLength = 0.3f;

		// Token: 0x040017A9 RID: 6057
		[FoldoutGroup("BoatPaths")]
		public int BoatPathCardinalDegreesCorrection = 4;

		// Token: 0x040017AA RID: 6058
		private const string BoatVFX = "Boat VFX";

		// Token: 0x040017AB RID: 6059
		[FoldoutGroup("Boat VFX")]
		public float boatMaximumRippleEmission = 6f;

		// Token: 0x040017AC RID: 6060
		[FoldoutGroup("Boat VFX")]
		public float boatRippleEmissionStopFactor = 2f;

		// Token: 0x040017AD RID: 6061
		[FoldoutGroup("Boat VFX")]
		public float boatRippleSpeed = 35f;

		// Token: 0x040017AE RID: 6062
		[FoldoutGroup("Boat VFX")]
		public float boatLightBlinkTime = 1f;

		// Token: 0x040017AF RID: 6063
		[FoldoutGroup("Boat VFX")]
		public float boatShadowPivotDistance = 0.6f;

		// Token: 0x040017B0 RID: 6064
		[FormerlySerializedAs("boatTrailFadeInDelay")]
		[FoldoutGroup("Boat VFX")]
		public float boatTrailDistanceFromTargetFadeIn = 4f;

		// Token: 0x040017B1 RID: 6065
		[FoldoutGroup("Boat VFX")]
		public float boatTrailDistanceFromTargetVisible = 1f;

		// Token: 0x040017B2 RID: 6066
		[FoldoutGroup("Boat VFX")]
		public float boatNormalTrailRendererTime = 0.8f;

		// Token: 0x040017B3 RID: 6067
		[FoldoutGroup("Boat VFX")]
		public float boatDoubleSpeedTrailRendererTime = 0.6f;

		// Token: 0x040017B4 RID: 6068
		[FoldoutGroup("Boat VFX")]
		public float boatGifCaptureTrailRendererTime = 0.2f;

		// Token: 0x040017B5 RID: 6069
		[FoldoutGroup("Boat VFX")]
		public float boatMovementBobbingAmplitude = 0.2f;

		// Token: 0x040017B6 RID: 6070
		[FoldoutGroup("Boat VFX")]
		public float boatMovementBobbingPeriod = 1f;

		// Token: 0x040017B7 RID: 6071
		[FoldoutGroup("Boat VFX")]
		public float boatMovementBobbingForwardsAmplitude = 0.2f;

		// Token: 0x040017B8 RID: 6072
		[FoldoutGroup("Boat VFX")]
		public float boatMovementLookAheadDistance = 2f;

		// Token: 0x040017B9 RID: 6073
		private const string CinematicMode = "Cinematic Mode";

		// Token: 0x040017BA RID: 6074
		[FoldoutGroup("Cinematic Mode")]
		public float MaximumDurationOnIdleCar = 10f;

		// Token: 0x040017BB RID: 6075
		[FoldoutGroup("Cinematic Mode")]
		public float WaitDurationBetweenCompletedJourneyAndNewAgent = 4f;

		// Token: 0x040017BC RID: 6076
		[FoldoutGroup("Cinematic Mode")]
		public float MaximumDurationOnTrain = 120f;

		// Token: 0x040017BD RID: 6077
		[FoldoutGroup("Cinematic Mode")]
		public float MinimumDurationOnTrain = 60f;

		// Token: 0x040017BE RID: 6078
		[FoldoutGroup("Cinematic Mode")]
		public float ChanceToSelectTrain = 0.1f;

		// Token: 0x040017BF RID: 6079
		[FoldoutGroup("Cinematic Mode")]
		public float DistanceAtWhichToLowerChanceToSelectTrain = 30f;

		// Token: 0x040017C0 RID: 6080
		[FoldoutGroup("Cinematic Mode")]
		[Tooltip("How quickly does this accelerate towards the next agent?")]
		public float CinematicCameraAccelerationWhenChangingAgent = 0.05f;

		// Token: 0x040017C1 RID: 6081
		[FoldoutGroup("Cinematic Mode")]
		public float CinematicCameraMoveSpeed = 10f;

		// Token: 0x040017C2 RID: 6082
		[FoldoutGroup("Cinematic Mode")]
		public float CinematicCameraZoomSpeed = 6f;

		// Token: 0x040017C3 RID: 6083
		[FoldoutGroup("Cinematic Mode")]
		public float CinematicCameraSpringDuration = 0.45f;

		// Token: 0x040017C4 RID: 6084
		[FoldoutGroup("Cinematic Mode")]
		public Easings.Functions CinematicCameraEasingFunction = Easings.Functions.QuarticEaseOut;

		// Token: 0x040017C5 RID: 6085
		[FoldoutGroup("Cinematic Mode")]
		public float CinematicTransitionOutBlurSpeed = 0.5f;

		// Token: 0x040017C6 RID: 6086
		private const string iCloudFaqLink = "iCloud Faq Link";

		// Token: 0x040017C7 RID: 6087
		[FoldoutGroup("iCloud Faq Link")]
		public string iCloudLinkString;
	}
}
