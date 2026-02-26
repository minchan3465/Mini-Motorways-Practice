using System;
using Factory;
using Motorways.Themes;
using Popups;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000567 RID: 1383
	public class ProfileCreationScreen : BaseScalingScreen
	{
		// Token: 0x06002591 RID: 9617 RVA: 0x0009F123 File Offset: 0x0009D323
		public void OnNextIcon()
		{
			this._currentIconIndex = (this._currentIconIndex + 1) % this._visualConstants.ProfileIconCount;
			this._icon.sprite = this._visualConstants.GetProfileIcon(this._currentIconIndex);
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x0009F15B File Offset: 0x0009D35B
		public void OnSetIconIndex(int index)
		{
			this._currentIconIndex = index;
			this._icon.sprite = this._visualConstants.GetProfileIcon(this._currentIconIndex);
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x0009F180 File Offset: 0x0009D380
		public void OnNextColor()
		{
			this._currentBackgroundIndex = (this._currentBackgroundIndex + 1) % 6;
			Color nextColor = this._themeDatabase.GetGlobalColor(this.CurrentColorType);
			this._backgroundColor.color = nextColor;
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x0009F1BC File Offset: 0x0009D3BC
		public void OnSetColorIndex(int index)
		{
			this._currentBackgroundIndex = index;
			Color nextColor = this._themeDatabase.GetGlobalColor(this.CurrentColorType);
			this._backgroundColor.color = nextColor;
		}

		// Token: 0x06002595 RID: 9621 RVA: 0x0009F1EE File Offset: 0x0009D3EE
		public void OnDeleteProfileButton()
		{
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.DeleteProfile, new Action(this.OnDeleteProfileCancel), new Action(this.OnDeleteProfileFirstPromptConfirmed), StringId.DeleteProfileDescription);
		}

		// Token: 0x06002596 RID: 9622 RVA: 0x0009F218 File Offset: 0x0009D418
		private void OnDeleteProfileFirstPromptConfirmed()
		{
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.DeleteProfile, new Action(this.OnDeleteProfileReconsiderWaived), new Action(this.OnDeleteProfileCancel), StringId.DeleteProfileDescription2);
		}

		// Token: 0x06002597 RID: 9623 RVA: 0x0009F242 File Offset: 0x0009D442
		private void OnDeleteProfileReconsiderWaived()
		{
			this.popupStack.PushConfirmationPopup<ConfirmationPopup>(StringId.DeleteProfile, new Action(this.OnDeleteProfileCancel), new Action(this.OnDeleteProfileFinalConfirmation), StringId.DeleteProfileDescription3);
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x0009F26C File Offset: 0x0009D46C
		private void OnDeleteProfileFinalConfirmation()
		{
			this._playerDatabase.DeletePlayer(this._playerToEdit);
			this._profileSelectScreen.Enable(true);
			this._profileSelectScreen.PrepareScreen();
			this._screenStack.PopOneScreen();
		}

		// Token: 0x06002599 RID: 9625 RVA: 0x000022F5 File Offset: 0x000004F5
		private void OnDeleteProfileCancel()
		{
		}

		// Token: 0x0600259A RID: 9626 RVA: 0x0009F2A1 File Offset: 0x0009D4A1
		public void PrepareScreen(Player player)
		{
			this._playerToEdit = player;
			this._backgroundSelector.SetOption(this._playerToEdit.AvatarColorIndex, true);
			this._iconSelector.SetOption(this._playerToEdit.AvatarIconIndex, true);
		}

		// Token: 0x0600259B RID: 9627 RVA: 0x0009F2D8 File Offset: 0x0009D4D8
		public static ThemedMaterialType GetProfileColorEnumForIndex(int index)
		{
			ThemedMaterialType result;
			if (Diagnostics.Verify(Enum.TryParse<ThemedMaterialType>(string.Format("{0}{1}", "ProfileColor", index + 1), out result), "No profile color for index {0}", index))
			{
				return result;
			}
			return ThemedMaterialType.ProfileColor1;
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x0600259C RID: 9628 RVA: 0x0009F319 File Offset: 0x0009D519
		public ThemedMaterialType CurrentColorType
		{
			get
			{
				return ProfileCreationScreen.GetProfileColorEnumForIndex(this._currentBackgroundIndex);
			}
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x0009F326 File Offset: 0x0009D526
		public void OnBack()
		{
			this._playerToEdit.AvatarColorIndex = this._currentBackgroundIndex;
			this._playerToEdit.AvatarIconIndex = this._currentIconIndex;
			this._screenStack.PopOneScreen();
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x0009F355 File Offset: 0x0009D555
		public override void Reset()
		{
			base.Reset();
			this._currentIconIndex = 0;
			this._currentBackgroundIndex = 0;
		}

		// Token: 0x04001FB6 RID: 8118
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04001FB7 RID: 8119
		[Dependency]
		private ProfileSelectScreen _profileSelectScreen;

		// Token: 0x04001FB8 RID: 8120
		[SerializeField]
		private Image _backgroundColor;

		// Token: 0x04001FB9 RID: 8121
		[SerializeField]
		private Image _icon;

		// Token: 0x04001FBA RID: 8122
		[SerializeField]
		private TouchOptionButton _backgroundSelector;

		// Token: 0x04001FBB RID: 8123
		[SerializeField]
		private TouchOptionButton _iconSelector;

		// Token: 0x04001FBC RID: 8124
		private Player _playerToEdit;

		// Token: 0x04001FBD RID: 8125
		private int _currentBackgroundIndex;

		// Token: 0x04001FBE RID: 8126
		private int _currentIconIndex;

		// Token: 0x04001FBF RID: 8127
		[Dependency]
		private PlayerDatabase _playerDatabase;

		// Token: 0x04001FC0 RID: 8128
		private const string ProfileColorEnumId = "ProfileColor";
	}
}
