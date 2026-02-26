using System;
using System.Collections.Generic;
using System.Globalization;
using Client;
using Factory;
using Helpers.GameCenter;
using JetBrains.Annotations;
using Motorways;
using Notifications;
using NotificationService.Events;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class App : IApp, IControllerConnectionObserver
{
	// Token: 0x060000B8 RID: 184 RVA: 0x00003A04 File Offset: 0x00001C04
	public void Start()
	{
		this.PreventCodeStripping();
		this._hardwareCapabilities.OnAppStart();
		this._softwareCapabilities.OnAppStart();
		this._achievementHandler.OnAppStart();
		this._gameCenterAuthentication.Authenticate();
		this._systemNotificationService.Setup();
		if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest))
		{
			this._hardwareCapabilities.IsPreventingSleep = true;
		}
		this._storageAuditTrail.IsRecordingEvents = FeatureToggle.IsFeatureEnabled(Feature.RecordStorageAuditTrail);
		this._inputState.SubscribeToControllerConnectionMessages(this);
		this._inputState.Start();
		Diagnostics.Verify(this._localeDatabase.Load(), "Failed to load the locale database!");
		Diagnostics.Verify(this._achievementDatabase.Load(), "Failed to load the achievement database!");
		bool isAudioRunning = AudioSettings.dspTime > 0.0;
		this._audioSystem.Start(isAudioRunning);
		this._themeDatabase.Start();
		this._screenStack.Start();
		this._activePlayer.PlayerChanged += this._notificationScheduler.OnPlayerChanged;
		this._activePlayer.DataChanged += this._notificationScheduler.OnPlayerDataChanged;
		this._activePlayer.PlayerChanged += this.OnPlayerChanged;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00003B3B File Offset: 0x00001D3B
	public void GameOpenedNotificationSetup()
	{
		this._notificationEventSystem.RecordEvent(new OpenedMiniMotorways(), true);
		this._systemNotificationService.RemoveAllDeliveredNotifications();
		this._systemNotificationService.ApplicationBadge = 0;
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00003B65 File Offset: 0x00001D65
	private void OnPlayerChanged(Player oldPlayer, Player newPlayer)
	{
		this.GameOpenedNotificationSetup();
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00003B70 File Offset: 0x00001D70
	public void Tick(float absoluteAppTime, float deltaTime)
	{
		this._tickRegistry.Tick(deltaTime);
		this._audioSystem.Tick();
		this._inputState.Tick(absoluteAppTime);
		this._playerActionController.Tick(deltaTime);
		this._screenStack.Tick(deltaTime);
		this._themeDatabase.Tick(deltaTime);
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00003BC4 File Offset: 0x00001DC4
	public void OnControllerConnected(IController controller)
	{
		controller.RegisterInputActionsForApp(this.Scope);
		controller.EnsureActionsAreRegistered(this.Scope);
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00003BDE File Offset: 0x00001DDE
	public void OnControllerDisconnected(IController controller)
	{
		if (typeof(IScopeObserver).IsAssignableFrom(controller.GetType()))
		{
			this.Scope.Unsubscribe((IScopeObserver)controller);
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000BE RID: 190 RVA: 0x00003C08 File Offset: 0x00001E08
	// (set) Token: 0x060000BF RID: 191 RVA: 0x00003C10 File Offset: 0x00001E10
	[Dependency]
	public IScope Scope { get; private set; }

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000C0 RID: 192 RVA: 0x00003C19 File Offset: 0x00001E19
	public Game Game
	{
		get
		{
			return this._screenStack.GetGameIfInGame();
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060000C1 RID: 193 RVA: 0x00003C26 File Offset: 0x00001E26
	public IInputState InputState
	{
		get
		{
			return this._inputState;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060000C2 RID: 194 RVA: 0x00003C2E File Offset: 0x00001E2E
	public PlayerActionController PlayerActionController
	{
		get
		{
			return this._playerActionController;
		}
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00003C38 File Offset: 0x00001E38
	private void PreventCodeStripping()
	{
		if (!this._preventCodeStripping)
		{
			return;
		}
		this._calendars = new List<Calendar>();
		this._calendars.Add(new ChineseLunisolarCalendar());
		this._calendars.Add(new JapaneseLunisolarCalendar());
		this._calendars.Add(new KoreanLunisolarCalendar());
		this._calendars.Add(new TaiwanLunisolarCalendar());
		this._calendars.Add(new GregorianCalendar());
		this._calendars.Add(new HebrewCalendar());
		this._calendars.Add(new HijriCalendar());
		this._calendars.Add(new JapaneseCalendar());
		this._calendars.Add(new JulianCalendar());
		this._calendars.Add(new KoreanCalendar());
		this._calendars.Add(new PersianCalendar());
		this._calendars.Add(new TaiwanCalendar());
		this._calendars.Add(new ThaiBuddhistCalendar());
		this._calendars.Add(new UmAlQuraCalendar());
	}

	// Token: 0x04000034 RID: 52
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x04000035 RID: 53
	[Dependency]
	private ISoftwareCapabilities _softwareCapabilities;

	// Token: 0x04000036 RID: 54
	[Dependency]
	private IAchievementHandler _achievementHandler;

	// Token: 0x04000037 RID: 55
	[Dependency]
	private IGameCenterAuthentication _gameCenterAuthentication;

	// Token: 0x04000038 RID: 56
	[Dependency]
	private ISystemNotificationService _systemNotificationService;

	// Token: 0x04000039 RID: 57
	[Dependency]
	private NotificationScheduler _notificationScheduler;

	// Token: 0x0400003A RID: 58
	[Dependency]
	private INotificationEventSystem _notificationEventSystem;

	// Token: 0x0400003B RID: 59
	[Dependency]
	private IAudioSystem _audioSystem;

	// Token: 0x0400003C RID: 60
	[Dependency]
	private IInputState _inputState;

	// Token: 0x0400003D RID: 61
	[Dependency]
	private PlayerActionController _playerActionController;

	// Token: 0x0400003E RID: 62
	[Dependency]
	private LocaleDatabase _localeDatabase;

	// Token: 0x0400003F RID: 63
	[Dependency]
	private AchievementDatabase _achievementDatabase;

	// Token: 0x04000040 RID: 64
	[Dependency]
	private ScreenStack _screenStack;

	// Token: 0x04000041 RID: 65
	[Dependency]
	private IThemeDatabase _themeDatabase;

	// Token: 0x04000042 RID: 66
	[Dependency]
	private ActivePlayer _activePlayer;

	// Token: 0x04000043 RID: 67
	[Dependency]
	private Diagnostics.StorageAuditTrail _storageAuditTrail;

	// Token: 0x04000044 RID: 68
	[Dependency]
	private TickRegistry _tickRegistry;

	// Token: 0x04000046 RID: 70
	private volatile bool _preventCodeStripping = true;

	// Token: 0x04000047 RID: 71
	[UsedImplicitly]
	private volatile List<Calendar> _calendars;
}
