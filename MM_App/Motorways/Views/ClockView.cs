using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Themes;
using Motorways.UI;
using Server;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200058D RID: 1421
	public class ClockView : MonoBehaviour, IView, IThemeComponent, ICreatedInScopeHandler, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x0600274B RID: 10059 RVA: 0x000A7E90 File Offset: 0x000A6090
		// (set) Token: 0x0600274C RID: 10060 RVA: 0x000A7E98 File Offset: 0x000A6098
		public ClockModel ClockModel
		{
			get
			{
				return this._clockModel;
			}
			set
			{
				this._clockModel = value;
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x0600274D RID: 10061 RVA: 0x000A7EA1 File Offset: 0x000A60A1
		public TouchButton ScoreButton
		{
			get
			{
				return this._scoreButton;
			}
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x0600274E RID: 10062 RVA: 0x000A7EA9 File Offset: 0x000A60A9
		public Transform VcrInactiveAnchor
		{
			get
			{
				return this._vcrInactiveAnchor;
			}
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x0600274F RID: 10063 RVA: 0x000A7EB1 File Offset: 0x000A60B1
		// (set) Token: 0x06002750 RID: 10064 RVA: 0x000A7EB9 File Offset: 0x000A60B9
		public bool IsVisuallyPaused
		{
			get
			{
				return this._isVisuallyPaused;
			}
			set
			{
				this._isVisuallyPaused = value;
				this.UpdateColors();
				ClockView.OnVisuallyPausedChanged visuallyPausedChanged = this.VisuallyPausedChanged;
				if (visuallyPausedChanged == null)
				{
					return;
				}
				visuallyPausedChanged(this._isVisuallyPaused);
			}
		}

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06002751 RID: 10065 RVA: 0x000A7EDE File Offset: 0x000A60DE
		public ScoreView ScoreView
		{
			get
			{
				return this._scoreView;
			}
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06002752 RID: 10066 RVA: 0x000A7EE8 File Offset: 0x000A60E8
		// (remove) Token: 0x06002753 RID: 10067 RVA: 0x000A7F20 File Offset: 0x000A6120
		public event ClockView.OnVisuallyPausedChanged VisuallyPausedChanged;

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06002754 RID: 10068 RVA: 0x000A7F58 File Offset: 0x000A6158
		// (remove) Token: 0x06002755 RID: 10069 RVA: 0x000A7F90 File Offset: 0x000A6190
		public event Action OnClockToggled;

		// Token: 0x06002756 RID: 10070 RVA: 0x000A7FC5 File Offset: 0x000A61C5
		public void OnCreatedInScope(IScope scope)
		{
			this._scoreView = this._scope.Get<ScoreView>();
			this._scoreView.transform.SetParent(this._scoreViewParent, false);
			this._scoreButton = this._scoreView.scoreButton;
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x000A8000 File Offset: 0x000A6200
		public void Initialize(ClockModel clockModel, GameObject clockAnchorActive, Transform clockAnchorInactive, GameObject dayAnchorActive, Transform dayAnchorInactive, GameObject scoreAnchorActive, Transform scoreAnchorInactive)
		{
			this._clockModel = clockModel;
			this._clockFloatingElement.baseElement = clockAnchorActive;
			this._clockFloatingElement.SetInactiveAnchor(clockAnchorInactive);
			this._dayFloatingElement.baseElement = dayAnchorActive;
			this._dayFloatingElement.SetInactiveAnchor(dayAnchorInactive);
			this._scoreView.FloatingElement.baseElement = scoreAnchorActive;
			this._scoreView.FloatingElement.SetInactiveAnchor(scoreAnchorInactive);
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x000A806C File Offset: 0x000A626C
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			float targetRotation = this.clockHandRotationOrigin - 36f * this._clockModel.GetInterpolatedTime(stepAlpha);
			this.clockHandRectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetRotation));
			Locale.DaysOfTheWeek newDay = this._localeDatabase.CurrentLocale.GetDayLabel(this._clockModel.Day % 7);
			if (this._currentDay != newDay)
			{
				this._currentDay = newDay;
				StringId dayKey;
				if (Diagnostics.Verify(Enum.TryParse<StringId>(this._currentDay.ToString(), out dayKey)))
				{
					this.dayText.LocString = StandaloneLocString.CreateString(this._scope, dayKey);
				}
				AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.DayStart, 0.5f, -1f, true, null));
			}
			if (this._dayTime && !this.IsDayTime())
			{
				this.UpdateColors();
				this._dayTime = false;
			}
			else if (!this._dayTime && this.IsDayTime())
			{
				this.UpdateColors();
				this._dayTime = true;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000A8184 File Offset: 0x000A6384
		private void UpdateColors()
		{
			bool paused = this._isVisuallyPaused;
			if (FeatureToggle.IsFeatureDisabled(Feature.ClockPauseColor))
			{
				paused = false;
			}
			if (this._clockModel != null)
			{
				Image[] array;
				if (!this.IsDayTime())
				{
					this.clockHand.CrossFadeColor(this._lightColor, 0.1f, false, false);
					array = this.clockPips;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].CrossFadeColor(this._lightColor, 0.1f, false, false);
					}
					this.clockFace.CrossFadeColor(paused ? this.pauseColor : this._darkColor, 0.1f, false, false);
					return;
				}
				this.clockHand.CrossFadeColor(this._darkColor, 0.1f, false, false);
				array = this.clockPips;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].CrossFadeColor(this._darkColor, 0.1f, false, false);
				}
				this.clockFace.CrossFadeColor(paused ? this.pauseColor : this._lightColor, 0.1f, false, false);
			}
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x000A8281 File Offset: 0x000A6481
		public void Pulse()
		{
			this.animator.SetTrigger(ClockView.PulseTrigger);
		}

		// Token: 0x0600275C RID: 10076 RVA: 0x000A8293 File Offset: 0x000A6493
		private bool IsDayTime()
		{
			return this._clockModel.Hour % 24 >= 6 && this._clockModel.Hour % 24 < 18;
		}

		// Token: 0x0600275D RID: 10077 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x0600275E RID: 10078 RVA: 0x000A82BC File Offset: 0x000A64BC
		public void ApplyTheme(ITheme newTheme)
		{
			Theme theme = (Theme)newTheme;
			this._darkColor = theme.GetColor(this._darkThemeColor, "_Color");
			this._lightColor = theme.GetColor(this._lightThemeColor, "_Color");
			this.UpdateColors();
		}

		// Token: 0x0600275F RID: 10079 RVA: 0x000A8304 File Offset: 0x000A6504
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Theme theme = (Theme)newTheme;
			this._darkColor = theme.GetColor(this._darkThemeColor, "_Color");
			this._lightColor = theme.GetColor(this._lightThemeColor, "_Color");
			this.UpdateColors();
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06002760 RID: 10080 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06002761 RID: 10081 RVA: 0x000A834D File Offset: 0x000A654D
		public void OnReleasedFromScope(IScope scope)
		{
			this._isVisuallyPaused = false;
		}

		// Token: 0x06002762 RID: 10082 RVA: 0x000A8356 File Offset: 0x000A6556
		public void ClockToggled()
		{
			Action onClockToggled = this.OnClockToggled;
			if (onClockToggled == null)
			{
				return;
			}
			onClockToggled();
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x000A8368 File Offset: 0x000A6568
		public void Reset()
		{
			this._clockModel = null;
			this._darkColor = Color.black;
			this._lightColor = Color.white;
			this._currentDay = Locale.DaysOfTheWeek.Monday;
			this._dayTime = true;
			this._isVisuallyPaused = false;
		}

		// Token: 0x04002152 RID: 8530
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04002153 RID: 8531
		[Dependency]
		private IScope _scope;

		// Token: 0x04002154 RID: 8532
		private ClockModel _clockModel;

		// Token: 0x04002155 RID: 8533
		public RectTransform clockHandRectTransform;

		// Token: 0x04002156 RID: 8534
		public RectTransform clockFaceTransform;

		// Token: 0x04002157 RID: 8535
		public Animator animator;

		// Token: 0x04002158 RID: 8536
		private static readonly int PulseTrigger = Animator.StringToHash("Pulse");

		// Token: 0x04002159 RID: 8537
		public float clockHandRotationOrigin = 90f;

		// Token: 0x0400215A RID: 8538
		private const float ClockHandAnglePerSecond = 36f;

		// Token: 0x0400215B RID: 8539
		public LocalizedTextUI dayText;

		// Token: 0x0400215C RID: 8540
		private Locale.DaysOfTheWeek _currentDay;

		// Token: 0x0400215D RID: 8541
		public Image clockFace;

		// Token: 0x0400215E RID: 8542
		public Image[] clockPips;

		// Token: 0x0400215F RID: 8543
		public Image clockHand;

		// Token: 0x04002160 RID: 8544
		[SerializeField]
		private ThemedMaterialType _darkThemeColor = ThemedMaterialType.Dark;

		// Token: 0x04002161 RID: 8545
		[SerializeField]
		private ThemedMaterialType _lightThemeColor;

		// Token: 0x04002162 RID: 8546
		[SerializeField]
		private FloatingElement _clockFloatingElement;

		// Token: 0x04002163 RID: 8547
		[SerializeField]
		private FloatingElement _dayFloatingElement;

		// Token: 0x04002164 RID: 8548
		[SerializeField]
		private Transform _scoreViewParent;

		// Token: 0x04002165 RID: 8549
		[SerializeField]
		private Transform _vcrInactiveAnchor;

		// Token: 0x04002166 RID: 8550
		private TouchButton _scoreButton;

		// Token: 0x04002167 RID: 8551
		public Color pauseColor = Color.red;

		// Token: 0x04002168 RID: 8552
		private Color _darkColor = Color.black;

		// Token: 0x04002169 RID: 8553
		private Color _lightColor = Color.white;

		// Token: 0x0400216A RID: 8554
		private bool _dayTime = true;

		// Token: 0x0400216B RID: 8555
		private const float ColorChangeDuration = 0.1f;

		// Token: 0x0400216C RID: 8556
		private bool _isVisuallyPaused;

		// Token: 0x0400216D RID: 8557
		private ScoreView _scoreView;

		// Token: 0x0200058E RID: 1422
		// (Invoke) Token: 0x06002767 RID: 10087
		public delegate void OnVisuallyPausedChanged(bool isVisuallyPaused);

		// Token: 0x0200058F RID: 1423
		public class Builder : IViewBuilder
		{
			// Token: 0x0600276A RID: 10090 RVA: 0x000A83FC File Offset: 0x000A65FC
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				ClockView clockView = client.Scope.Get<ClockView>();
				clockView._clockModel = (model as ClockModel);
				clockView.UpdateColors();
				client.AddView(clockView);
			}
		}
	}
}
