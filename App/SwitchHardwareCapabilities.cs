using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Factory;
using JetBrains.Annotations;
using Rewired;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class SwitchHardwareCapabilities : IHardwareCapabilities, IReleasedFromScopeHandler
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x060003E7 RID: 999 RVA: 0x0000EA0E File Offset: 0x0000CC0E
	public RuntimePlatform Platform
	{
		get
		{
			return Application.platform;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060003E8 RID: 1000 RVA: 0x0000EE98 File Offset: 0x0000D098
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return UnityLocaleQuery.GetLocaleId(this._localeDatabase);
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x060003E9 RID: 1001 RVA: 0x0000EEA5 File Offset: 0x0000D0A5
	public string PersistentStoragePath
	{
		get
		{
			return "";
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x0000EEAC File Offset: 0x0000D0AC
	public string UniqueDeviceId
	{
		get
		{
			return "nx";
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060003EB RID: 1003 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsHapticFeedback
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x0000EEB4 File Offset: 0x0000D0B4
	public void GenerateHapticFeedback(HapticFeedbackType feedback)
	{
		if (feedback != HapticFeedbackType.Selection || this._inputState.CurrentDeviceInputType == DeviceInputType.Touch)
		{
			SwitchHardwareCapabilities.SwitchVibrationData vibrationData = this.GetVibration(feedback);
			this.SetVibration(vibrationData);
		}
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x0000EEE4 File Offset: 0x0000D0E4
	private SwitchHardwareCapabilities.SwitchVibrationData GetVibration(HapticFeedbackType feedback)
	{
		SwitchHardwareCapabilities.SwitchVibrationData vibrationData = default(SwitchHardwareCapabilities.SwitchVibrationData);
		vibrationData.frequencyLow = 160f;
		vibrationData.frequencyHigh = 320f;
		switch (feedback)
		{
		case HapticFeedbackType.LightImpact:
		case HapticFeedbackType.Selection:
		case HapticFeedbackType.Warning:
			vibrationData.amplitudeLow = 0.05f;
			vibrationData.amplitudeHigh = 0.025f;
			vibrationData.durationSeconds = 0.05f;
			break;
		case HapticFeedbackType.MediumImpact:
		case HapticFeedbackType.Error:
			vibrationData.amplitudeLow = 0.15f;
			vibrationData.amplitudeHigh = 0.1f;
			vibrationData.durationSeconds = 0.075f;
			break;
		case HapticFeedbackType.HeavyImpact:
		case HapticFeedbackType.Success:
			vibrationData.amplitudeLow = 0.5f;
			vibrationData.amplitudeHigh = 0.25f;
			vibrationData.durationSeconds = 0.1f;
			break;
		}
		return vibrationData;
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsManualExit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060003EF RID: 1007 RVA: 0x0000222C File Offset: 0x0000042C
	// (set) Token: 0x060003F0 RID: 1008 RVA: 0x000022F5 File Offset: 0x000004F5
	public bool IsPreventingSleep
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060003F1 RID: 1009 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsChangingResolution
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060003F2 RID: 1010 RVA: 0x0000ED50 File Offset: 0x0000CF50
	public Vector2Int DefaultMaximumResolution
	{
		get
		{
			return new Vector2Int(-1, -1);
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060003F3 RID: 1011 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsAntiAliasingOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060003F4 RID: 1012 RVA: 0x000020AA File Offset: 0x000002AA
	public int DefaultAntiAliasingLevel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x060003F5 RID: 1013 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleDisplays
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x060003F6 RID: 1014 RVA: 0x000020AA File Offset: 0x000002AA
	public int DisplayCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0000EFA6 File Offset: 0x0000D1A6
	public void ActivateControllerSelect()
	{
		this.ShowControllerSupportApplet();
		this._checkForControllerChange = true;
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x0000EFB5 File Offset: 0x0000D1B5
	public void Exit()
	{
		Diagnostics.FailAssert("Exit() not supported on Switch.", Array.Empty<object>());
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060003F9 RID: 1017 RVA: 0x0000EFC6 File Offset: 0x0000D1C6
	public DeviceInputType DefaultDeviceInputType
	{
		get
		{
			return DeviceInputType.Controller;
		}
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x060003FA RID: 1018 RVA: 0x0000EFCC File Offset: 0x0000D1CC
	// (remove) Token: 0x060003FB RID: 1019 RVA: 0x0000F004 File Offset: 0x0000D204
	public event Action<DeviceInputGamepadStyle> OnGamepadStyleChanged;

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060003FC RID: 1020 RVA: 0x0000F039 File Offset: 0x0000D239
	// (set) Token: 0x060003FD RID: 1021 RVA: 0x0000F058 File Offset: 0x0000D258
	public DeviceInputGamepadStyle CurrentGamepadStyle
	{
		get
		{
			if (this._currentGamepadStyle == DeviceInputGamepadStyle.None)
			{
				this.CurrentGamepadStyle = this.GetCurrentGamepadStyle();
			}
			return this._currentGamepadStyle;
		}
		private set
		{
			if (value == this._currentGamepadStyle)
			{
				return;
			}
			SwitchHardwareCapabilities.Log.Info("Changing gamepad style from {0} to {1}.", new object[]
			{
				this._currentGamepadStyle,
				value
			});
			this._currentGamepadStyle = value;
			Action<DeviceInputGamepadStyle> onGamepadStyleChanged = this.OnGamepadStyleChanged;
			if (onGamepadStyleChanged == null)
			{
				return;
			}
			onGamepadStyleChanged(value);
		}
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x0000F0B3 File Offset: 0x0000D2B3
	private DeviceInputGamepadStyle GetCurrentGamepadStyle()
	{
		DeviceInputGamepadStyle result = DeviceInputGamepadStyle.SwitchHandheld;
		this.GetLastActiveJoystick();
		return result;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x0000F0BD File Offset: 0x0000D2BD
	[CanBeNull]
	private Joystick GetLastActiveJoystick()
	{
		return this._rewiredPlayer.controllers.GetLastActiveController<Joystick>() as Joystick;
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0000F0D4 File Offset: 0x0000D2D4
	public virtual void OnAppStart()
	{
		this._inputState.ControllerConnected(this._scope.Get<ITouchScreenController>());
		this._inputState.ControllerConnected(this._scope.Get<IGamepadController>());
		this._tickRegistry.AppTicking += this.Tick;
		this._rewiredPlayer = ReInput.players.GetPlayer(0);
		this._rewiredPlayer.controllers.AddLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
		this._rumbleTaskCancellationSource = new CancellationTokenSource();
		CancellationToken token = this._rumbleTaskCancellationSource.Token;
		this._rumbleTask = Task.Run(new Action(this.RumbleThreadProc), token);
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x0000F180 File Offset: 0x0000D380
	private void RumbleThreadProc()
	{
		Stopwatch threadStopwatch = new Stopwatch();
		threadStopwatch.Start();
		long lastMs = threadStopwatch.ElapsedMilliseconds;
		for (;;)
		{
			long elapsedMilliseconds = threadStopwatch.ElapsedMilliseconds;
			float deltaTime = (float)(elapsedMilliseconds - lastMs) / 1000f;
			lastMs = elapsedMilliseconds;
			if (this._vibrationTimer > 0f)
			{
				this._vibrationTimer -= deltaTime;
				if (this._vibrationTimer <= 0f)
				{
					this.CancelVibration();
					this._vibrationTimer = 0f;
				}
			}
		}
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x0000F1EC File Offset: 0x0000D3EC
	private void OnLastActiveControllerChanged(Rewired.Player player, Controller controller)
	{
		if (player == this._rewiredPlayer && this._rewiredPlayer.controllers.joystickCount > 0)
		{
			this.CurrentGamepadStyle = this.GetCurrentGamepadStyle();
		}
		this.CancelVibration();
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x0000F21C File Offset: 0x0000D41C
	private void Tick(float deltaTime)
	{
		if (this._rewiredPlayer.controllers.joystickCount == 0)
		{
			SwitchHardwareCapabilities.Log.Info("No active joysticks! Showing controller selection applet.", Array.Empty<object>());
			this.ActivateControllerSelect();
		}
		if (this._checkForControllerChange && this._rewiredPlayer.controllers.joystickCount > 0)
		{
			SwitchHardwareCapabilities.Log.Info("Found a new active joystick.", Array.Empty<object>());
			this.CurrentGamepadStyle = this.GetCurrentGamepadStyle();
			this.CancelVibration();
			this._checkForControllerChange = false;
		}
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x0000F2A0 File Offset: 0x0000D4A0
	public void OnReleasedFromScope(IScope scope)
	{
		if (this._rumbleTask != null && !this._rumbleTask.IsCanceled && !this._rumbleTask.IsCompleted && this._rumbleTaskCancellationSource != null)
		{
			this._rumbleTaskCancellationSource.Cancel();
			this._rumbleTaskCancellationSource = null;
			this._rumbleTask = null;
		}
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x000022F5 File Offset: 0x000004F5
	private void ShowControllerSupportApplet()
	{
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x0000F2F0 File Offset: 0x0000D4F0
	private void SetVibration(SwitchHardwareCapabilities.SwitchVibrationData vibrationData)
	{
		this.GetLastActiveJoystick();
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0000F2FC File Offset: 0x0000D4FC
	private void CancelVibration()
	{
		SwitchHardwareCapabilities.Log.Info("Cancelling all vibration.", Array.Empty<object>());
		foreach (Joystick joystick in this._rewiredPlayer.controllers.Joysticks)
		{
		}
	}

	// Token: 0x04000193 RID: 403
	[Dependency]
	private IScope _scope;

	// Token: 0x04000194 RID: 404
	[Dependency]
	private LocaleDatabase _localeDatabase;

	// Token: 0x04000195 RID: 405
	[Dependency]
	private InputState _inputState;

	// Token: 0x04000196 RID: 406
	[Dependency]
	private TickRegistry _tickRegistry;

	// Token: 0x04000197 RID: 407
	private Rewired.Player _rewiredPlayer;

	// Token: 0x04000198 RID: 408
	private bool _checkForControllerChange;

	// Token: 0x04000199 RID: 409
	private float _vibrationTimer;

	// Token: 0x0400019A RID: 410
	private Task _rumbleTask;

	// Token: 0x0400019B RID: 411
	private CancellationTokenSource _rumbleTaskCancellationSource;

	// Token: 0x0400019C RID: 412
	private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("SwitchHardwareCapabilities");

	// Token: 0x0400019E RID: 414
	private DeviceInputGamepadStyle _currentGamepadStyle;

	// Token: 0x020000C9 RID: 201
	private struct SwitchVibrationData
	{
		// Token: 0x0400019F RID: 415
		public float amplitudeLow;

		// Token: 0x040001A0 RID: 416
		public float amplitudeHigh;

		// Token: 0x040001A1 RID: 417
		public float frequencyLow;

		// Token: 0x040001A2 RID: 418
		public float frequencyHigh;

		// Token: 0x040001A3 RID: 419
		public float durationSeconds;
	}
}
