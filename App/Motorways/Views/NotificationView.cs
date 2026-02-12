using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Processes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000577 RID: 1399
	public class NotificationView : IView, IReusable
	{
		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x0600264F RID: 9807 RVA: 0x000A2847 File Offset: 0x000A0A47
		// (set) Token: 0x06002650 RID: 9808 RVA: 0x000A284F File Offset: 0x000A0A4F
		public bool NotificationsEnabled { get; set; } = true;

		// Token: 0x06002651 RID: 9809 RVA: 0x000A2858 File Offset: 0x000A0A58
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._tutorialProcess != null && this._tutorialProcess.HasVisibleMessage)
			{
				this._pendingNotificationMessage = null;
				this.DismissNotification();
			}
			if (this._notification != null)
			{
				if (this._hideCondition != null)
				{
					if (this._hideCondition())
					{
						this.DismissNotification();
					}
				}
				else
				{
					this._notificationTime -= timeInterval.Delta;
					if (this._notificationTime <= 0f || ((this._pendingNotificationMessage != null || this._notificationHideScheduled) && 4f - this._notificationTime > 1.5f))
					{
						this.DismissNotification();
					}
				}
			}
			else
			{
				this._notificationCooldown = Mathf.Max(0f, this._notificationCooldown - timeInterval.Delta);
				if (this._notificationCooldown <= 0f)
				{
					this._notificationDelay = Mathf.Max(0f, this._notificationDelay - timeInterval.Delta);
				}
				if (this._notificationCooldown <= 0f && this._notificationDelay <= 0f && this._pendingNotificationMessage != null)
				{
					this.ShowNotification(this._pendingNotificationMessage);
					this._pendingNotificationMessage = null;
					this._recentErrors.Clear();
				}
			}
			for (int errorIndex = 0; errorIndex < this._recentErrors.Count; errorIndex++)
			{
				if (this._recentErrors[errorIndex].age > this._visualConstants.RepeatRecentErrorTimeWindow)
				{
					this._recentErrors.RemoveAt(errorIndex);
					errorIndex--;
				}
				else
				{
					this._recentErrors[errorIndex].age += timeInterval.Delta;
				}
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002652 RID: 9810 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetGameobjectActive(bool isActive)
		{
		}

		// Token: 0x06002653 RID: 9811 RVA: 0x000A2A04 File Offset: 0x000A0C04
		public bool AddNotification(TileEditResultCode tileEditErrorCode, Vector2Int position)
		{
			if (tileEditErrorCode == TileEditResultCode.CannotCreateBridge)
			{
				this._gameUI.UpgradeBar.BounceUpgrade(UpgradeType.Bridge);
			}
			else if (tileEditErrorCode == TileEditResultCode.CannotCreateTunnel)
			{
				this._gameUI.UpgradeBar.BounceUpgrade(UpgradeType.Tunnel);
			}
			else if (tileEditErrorCode == TileEditResultCode.NotEnoughConcrete)
			{
				this._gameUI.UpgradeBar.BounceUpgrade(UpgradeType.Concrete);
			}
			if (!this.NotificationsEnabled)
			{
				return false;
			}
			bool sendAlert = false;
			NotificationView.AlertIconType alertIconType = NotificationView.AlertIconType.Cross;
			StringId errorStringId = StringId.None;
			switch (tileEditErrorCode)
			{
			case TileEditResultCode.CannotConnectToCarpark:
				if (this._city.Rules.ShowCannotConnectToCarparkErrorNotification())
				{
					errorStringId = StringId.Error_CannotConnectToCarpark;
				}
				sendAlert = true;
				break;
			case TileEditResultCode.CannotConnectHouseToBridge:
				errorStringId = StringId.Error_CannotConnectHouseToBridge;
				sendAlert = true;
				break;
			case TileEditResultCode.NotEnoughConcrete:
				if (this._city.Rules.ShowNoConcreteErrorNotification())
				{
					errorStringId = this._city.Rules.GetNoConcreteErrorMessage(this._inputState.CurrentDeviceInputType);
					alertIconType = NotificationView.AlertIconType.Exclaimation;
					sendAlert = true;
				}
				break;
			case TileEditResultCode.NotEnoughConcreteForMotorway:
				errorStringId = StringId.Error_NotEnoughConcreteMotorway;
				break;
			case TileEditResultCode.MotorwayBlockedByMountain:
				errorStringId = StringId.Error_MotorwayCollidesWithMountain;
				break;
			case TileEditResultCode.CannotConnectHouseToTunnel:
				errorStringId = StringId.Error_CannotConnectHouseToTunnel;
				sendAlert = true;
				break;
			case TileEditResultCode.CannotCreateBridge:
			case TileEditResultCode.CannotCreateTunnel:
				sendAlert = true;
				alertIconType = NotificationView.AlertIconType.Exclaimation;
				break;
			case TileEditResultCode.NoDeletableRoads:
				sendAlert = true;
				errorStringId = StringId.Error_NoDeletableRoads;
				break;
			case TileEditResultCode.NoDeletableUpgrade:
				sendAlert = true;
				errorStringId = StringId.Error_NoDeletableUpgrades;
				break;
			case TileEditResultCode.CannotConnectHouseToRail:
				sendAlert = true;
				errorStringId = StringId.Error_CannotConnectHouseToRail;
				break;
			}
			this._recentErrors.Add(new NotificationView.RecentError(tileEditErrorCode));
			if (sendAlert)
			{
				this.ShowIconNotification(alertIconType, position, tileEditErrorCode);
			}
			return errorStringId != StringId.None && this.AddNotification(errorStringId, this.GetDelayForError(sendAlert, tileEditErrorCode), null);
		}

		// Token: 0x06002654 RID: 9812 RVA: 0x000A2B8C File Offset: 0x000A0D8C
		private float GetDelayForError(bool isSendingAlert, TileEditResultCode reason)
		{
			float delay = isSendingAlert ? this._visualConstants.TimeAfterIconAppearsWhenNotificationAppears : 0f;
			int repeatErrorCount = 0;
			using (List<NotificationView.RecentError>.Enumerator enumerator = this._recentErrors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.code == reason)
					{
						repeatErrorCount++;
					}
				}
			}
			if (repeatErrorCount >= this._visualConstants.RepeatRecentErrorCount)
			{
				delay = 0f;
			}
			return delay;
		}

		// Token: 0x06002655 RID: 9813 RVA: 0x000A2C10 File Offset: 0x000A0E10
		public bool ShowIconNotification(NotificationView.AlertIconType type, Vector2Int position, TileEditResultCode reason)
		{
			if (reason == this._alertReason)
			{
				return false;
			}
			if (this._alertIcon != null)
			{
				this.HideAlertIcon();
			}
			IndicatorAnimationView animation = this._scope.Get<IndicatorAnimationView>();
			animation.Initialize(IndicatorAnimationView.AnimationType.Alert, position.ToVector3() * 2f, null);
			animation.SetAlertType(type);
			this._viewClient.AddView(animation);
			this._alertIcon = animation;
			this._alertReason = reason;
			return true;
		}

		// Token: 0x06002656 RID: 9814 RVA: 0x000A2C8C File Offset: 0x000A0E8C
		public bool AddNotification(MotorwaysStringKey newNotificationMessage, float delay = 0f, Func<bool> hideCondition = null)
		{
			if (hideCondition != null)
			{
				if (hideCondition())
				{
					return false;
				}
				this._hideCondition = hideCondition;
			}
			if (this._notification != null && this._notificationMessage.Equals(newNotificationMessage))
			{
				this._notificationTime = 4f;
				this._pendingNotificationMessage = null;
				this._notificationHideScheduled = false;
			}
			else
			{
				if (this._notification == null && this._pendingNotificationMessage != newNotificationMessage)
				{
					this._notificationDelay = delay;
				}
				else
				{
					this._nextNotificationDelay = delay;
				}
				this._pendingNotificationMessage = newNotificationMessage;
			}
			return true;
		}

		// Token: 0x06002657 RID: 9815 RVA: 0x000A2D18 File Offset: 0x000A0F18
		public void HideAlertIcon()
		{
			this._alertReason = TileEditResultCode.Success;
			if (this._alertIcon != null)
			{
				this._alertIcon.OnAnimationRelease();
				this._alertIcon = null;
			}
		}

		// Token: 0x06002658 RID: 9816 RVA: 0x000A2D41 File Offset: 0x000A0F41
		public void HideNotification()
		{
			this._notificationHideScheduled = true;
			this._pendingNotificationMessage = null;
			this._notificationTime = 1.5f;
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x000A2D5C File Offset: 0x000A0F5C
		public void CancelNotification()
		{
			this._notificationDelay = 0f;
			this._nextNotificationDelay = 0f;
			this._pendingNotificationMessage = null;
		}

		// Token: 0x0600265A RID: 9818 RVA: 0x000A2D7C File Offset: 0x000A0F7C
		public void Reset()
		{
			this._notification = null;
			this._notificationMessage = null;
			this._notificationTime = 0f;
			this._notificationCooldown = 0f;
			this._nextNotificationDelay = 0f;
			this._notificationDelay = 0f;
			this._notificationHideScheduled = false;
			this._pendingNotificationMessage = null;
			this._isControllerNotificationUp = false;
			this.NotificationsEnabled = true;
			this._recentErrors.Clear();
		}

		// Token: 0x0600265B RID: 9819 RVA: 0x000A2DEC File Offset: 0x000A0FEC
		private void ShowNotification(MotorwaysStringKey notificationMessage)
		{
			this._notification = this._scope.Get<AnchoredMessageView>();
			this._notification.InitializeWithScreenAnchor(StandaloneLocString.CreateString(this._scope, notificationMessage), new Vector2(0f, 0.7f), CameraLayer.Default);
			this._viewClient.AddView(this._notification);
			this._notificationTime = (this._isControllerNotificationUp ? float.MaxValue : 4f);
			this._notificationHideScheduled = false;
			this._notificationMessage = notificationMessage;
		}

		// Token: 0x0600265C RID: 9820 RVA: 0x000A2E6C File Offset: 0x000A106C
		private void DismissNotification()
		{
			if (this._notification != null)
			{
				this._notification.OnAnimationRelease();
				this._notification = null;
				this._notificationCooldown = 1f;
			}
			this._notificationDelay = this._nextNotificationDelay;
			this._isControllerNotificationUp = false;
			this._hideCondition = null;
		}

		// Token: 0x0600265D RID: 9821 RVA: 0x000A2EC0 File Offset: 0x000A10C0
		public void KillNotification()
		{
			if (this._notification != null)
			{
				this._notification.Kill();
				this._notification = null;
				this._notificationCooldown = 1f;
			}
			this._notificationDelay = this._nextNotificationDelay;
			this._isControllerNotificationUp = false;
			this._hideCondition = null;
		}

		// Token: 0x0400203B RID: 8251
		private const float MessageViewPosition = 0.7f;

		// Token: 0x0400203C RID: 8252
		private const float MinMessageDuration = 1.5f;

		// Token: 0x0400203D RID: 8253
		private const float MaxMessageDuration = 4f;

		// Token: 0x0400203E RID: 8254
		private const float MessageCooldownDuration = 1f;

		// Token: 0x0400203F RID: 8255
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("NotificationView");

		// Token: 0x04002040 RID: 8256
		[Dependency]
		private IScope _scope;

		// Token: 0x04002041 RID: 8257
		[Dependency]
		private City _city;

		// Token: 0x04002042 RID: 8258
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002043 RID: 8259
		[Dependency]
		private InputState _inputState;

		// Token: 0x04002044 RID: 8260
		[Dependency]
		private GameUIScreen _gameUI;

		// Token: 0x04002045 RID: 8261
		[Dependency]
		private TutorialProgressionProcess _tutorialProcess;

		// Token: 0x04002046 RID: 8262
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04002048 RID: 8264
		private AnchoredMessageView _notification;

		// Token: 0x04002049 RID: 8265
		private MotorwaysStringKey _notificationMessage;

		// Token: 0x0400204A RID: 8266
		private float _notificationTime;

		// Token: 0x0400204B RID: 8267
		private float _notificationCooldown;

		// Token: 0x0400204C RID: 8268
		private float _nextNotificationDelay;

		// Token: 0x0400204D RID: 8269
		private float _notificationDelay;

		// Token: 0x0400204E RID: 8270
		private IndicatorAnimationView _alertIcon;

		// Token: 0x0400204F RID: 8271
		private TileEditResultCode _alertReason;

		// Token: 0x04002050 RID: 8272
		private bool _notificationHideScheduled;

		// Token: 0x04002051 RID: 8273
		private MotorwaysStringKey _pendingNotificationMessage;

		// Token: 0x04002052 RID: 8274
		private Func<bool> _hideCondition;

		// Token: 0x04002053 RID: 8275
		private bool _isControllerNotificationUp;

		// Token: 0x04002054 RID: 8276
		private readonly List<NotificationView.RecentError> _recentErrors = new List<NotificationView.RecentError>();

		// Token: 0x02000578 RID: 1400
		private class RecentError
		{
			// Token: 0x06002660 RID: 9824 RVA: 0x000A2F3D File Offset: 0x000A113D
			public RecentError(TileEditResultCode resultCode)
			{
				this.code = resultCode;
				this.age = 0f;
			}

			// Token: 0x04002055 RID: 8277
			public TileEditResultCode code;

			// Token: 0x04002056 RID: 8278
			public float age;
		}

		// Token: 0x02000579 RID: 1401
		public enum AlertIconType
		{
			// Token: 0x04002058 RID: 8280
			Cross,
			// Token: 0x04002059 RID: 8281
			Exclaimation
		}
	}
}
