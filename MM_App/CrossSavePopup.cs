using System;
using Factory;
using Motorways;
using Popups;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C4 RID: 452
public class CrossSavePopup : BasePopup
{
	// Token: 0x06000AA3 RID: 2723 RVA: 0x00023388 File Offset: 0x00021588
	public void StartSteamSync()
	{
		this._mainPromptText.SetStringId(this._scope, StringId.CrossSave_Importer_Header);
		this.SetAdditionalInfo(StringId.CrossSave_Importer_Loading);
		this.SetButtonVisibility(false, false);
		this._spinner.SetActive(true);
		this._spinTimer = this._maxSpinTime;
		this._reachability.OpenManualConnection(delegate(InternetConnectionHandle handle)
		{
			if (!handle.IsAvailable)
			{
				handle.Close();
				this.PresentError(SteamCloudSyncError.NoConnection);
				return;
			}
			this._requestHandle = this._cloudSyncService.Authenticate(delegate(string token, SteamCloudSyncError error)
			{
				this._requestHandle = null;
				this._spinTimer = 0f;
				if (error != SteamCloudSyncError.None || string.IsNullOrEmpty(token))
				{
					this.PresentError(error);
					handle.Close();
					return;
				}
				this._requestHandle = this._cloudSyncService.DownloadProfiles(token, delegate(ILegacyUserProfile legacyUserProfile, IExtendedUserProfile extendedUserProfile, SteamCloudSyncError syncError)
				{
					this._requestHandle = null;
					if (error != SteamCloudSyncError.None)
					{
						this.PresentError(error);
						handle.Close();
						return;
					}
					if (legacyUserProfile == null && extendedUserProfile == null)
					{
						this.PresentError(StringId.CrossSave_Error_NoSteamData);
						handle.Close();
						return;
					}
					if (legacyUserProfile != null)
					{
						this._player.Player.MergeUserProfile(legacyUserProfile);
					}
					if (extendedUserProfile != null)
					{
						this._player.Player.MergeExtendedUserProfile(extendedUserProfile);
					}
					this.SetAdditionalInfo(StringId.CrossSave_ImportSuccessful);
					this.SetButtonVisibility(true, false);
					this._spinner.SetActive(false);
					handle.Close();
				});
			});
		});
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x000233F0 File Offset: 0x000215F0
	public void Update()
	{
		if (this._spinTimer > 0f)
		{
			this._spinTimer -= Time.deltaTime;
			if (this._spinTimer <= 0f)
			{
				this._spinTimer = 0f;
				this.SetButtonVisibility(false, true);
			}
		}
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0002343C File Offset: 0x0002163C
	private void SetAdditionalInfo(StringId additionalInfoStringId)
	{
		if (additionalInfoStringId != StringId.None)
		{
			this._additionalText.gameObject.SetActive(true);
			this._additionalText.SetStringId(this._scope, additionalInfoStringId);
			return;
		}
		this._additionalText.gameObject.SetActive(false);
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00023478 File Offset: 0x00021678
	private void PresentError(SteamCloudSyncError error)
	{
		StringId errorStringId;
		switch (error)
		{
		case SteamCloudSyncError.NoConnection:
			errorStringId = StringId.CrossSave_Error_NoConnection;
			goto IL_44;
		case SteamCloudSyncError.AuthorizationDenied:
			errorStringId = StringId.CrossSave_Error_SteamLinkCancel;
			goto IL_44;
		case SteamCloudSyncError.NotSupported:
			errorStringId = StringId.CrossSave_Error_SteamLinkFail;
			goto IL_44;
		case SteamCloudSyncError.InvalidData:
			errorStringId = StringId.CrossSave_Error_DataImportFail;
			goto IL_44;
		}
		errorStringId = StringId.CrossSave_Error_DataDownloadFail;
		IL_44:
		this.PresentError(errorStringId);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x000234D0 File Offset: 0x000216D0
	private void PresentError(StringId errorStringId)
	{
		this.SetAdditionalInfo(errorStringId);
		this.SetButtonVisibility(true, false);
		this._spinner.SetActive(false);
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000234ED File Offset: 0x000216ED
	public void NoPressed()
	{
		AsyncRequestHandle requestHandle = this._requestHandle;
		if (requestHandle != null)
		{
			requestHandle.Cancel();
		}
		this._popupStack.PopPopup(false);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0002350C File Offset: 0x0002170C
	public void YesPressed()
	{
		this._popupStack.PopPopup(false);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0002351C File Offset: 0x0002171C
	private void SetButtonVisibility(bool isYesVisible, bool isNoVisible)
	{
		this._yesButton.gameObject.SetActive(isYesVisible);
		this._noButton.gameObject.SetActive(isNoVisible);
		if (isYesVisible != isNoVisible && this.appScope.Get<InputState>().CurrentInputTypeRequiresFocus)
		{
			Selectable newFocus = isYesVisible ? this._yesButton : this._noButton;
			this.navigation.SetNewFocus(newFocus);
			newFocus.animator.SetTrigger(CrossSavePopup.Highlighted);
		}
	}

	// Token: 0x040005B5 RID: 1461
	private AsyncRequestHandle _requestHandle;

	// Token: 0x040005B6 RID: 1462
	private float _spinTimer;

	// Token: 0x040005B7 RID: 1463
	[SerializeField]
	private GameObject _spinner;

	// Token: 0x040005B8 RID: 1464
	[Min(1f)]
	[Tooltip("The maximum time in seconds the spinner will be shown before the cancel button will be revealed.")]
	[SerializeField]
	private float _maxSpinTime = 5f;

	// Token: 0x040005B9 RID: 1465
	[SerializeField]
	private TouchButton _yesButton;

	// Token: 0x040005BA RID: 1466
	[SerializeField]
	private TouchButton _noButton;

	// Token: 0x040005BB RID: 1467
	[SerializeField]
	private LocalizedTextUI _mainPromptText;

	// Token: 0x040005BC RID: 1468
	[SerializeField]
	private LocalizedTextUI _additionalText;

	// Token: 0x040005BD RID: 1469
	[Dependency]
	private IScope _scope;

	// Token: 0x040005BE RID: 1470
	[Dependency]
	private ISteamCloudSyncService _cloudSyncService;

	// Token: 0x040005BF RID: 1471
	[Dependency]
	private PopupStack _popupStack;

	// Token: 0x040005C0 RID: 1472
	[Dependency]
	private ActivePlayer _player;

	// Token: 0x040005C1 RID: 1473
	[Dependency]
	private IReachability _reachability;

	// Token: 0x040005C2 RID: 1474
	private static readonly int Highlighted = Animator.StringToHash("Highlighted");
}
