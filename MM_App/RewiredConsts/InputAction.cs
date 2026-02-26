using System;
using Rewired.Dev;

namespace RewiredConsts
{
	// Token: 0x020002DF RID: 735
	public static class InputAction
	{
		// Token: 0x04000FB1 RID: 4017
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Horizontal")]
		public const int MoveHorizontal = 0;

		// Token: 0x04000FB2 RID: 4018
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Vertical")]
		public const int MoveVertical = 1;

		// Token: 0x04000FB3 RID: 4019
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Confirm or context specific action in game")]
		public const int Confirm = 2;

		// Token: 0x04000FB4 RID: 4020
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateUp")]
		public const int NavigateUp = 3;

		// Token: 0x04000FB5 RID: 4021
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateRight")]
		public const int NavigateRight = 4;

		// Token: 0x04000FB6 RID: 4022
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateDown")]
		public const int NavigateDown = 5;

		// Token: 0x04000FB7 RID: 4023
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateLeft")]
		public const int NavigateLeft = 6;

		// Token: 0x04000FB8 RID: 4024
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Navigate back or cancel in game")]
		public const int Back = 7;

		// Token: 0x04000FB9 RID: 4025
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "To access pause menu from game")]
		public const int Menu = 8;

		// Token: 0x04000FBA RID: 4026
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "ActivateControllerSelect")]
		public const int ActivateControllerSelect = 12;

		// Token: 0x04000FBB RID: 4027
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Left Mouse Button Pressed")]
		public const int LeftMouse = 19;

		// Token: 0x04000FBC RID: 4028
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Right Mouse Button Pressed")]
		public const int RightMouse = 20;

		// Token: 0x04000FBD RID: 4029
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "X Position of Mouse")]
		public const int MousePosition = 23;

		// Token: 0x04000FBE RID: 4030
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Y Position of Mouse")]
		public const int MouseY = 24;

		// Token: 0x04000FBF RID: 4031
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateUpSiri2")]
		public const int NavigateUpSiri2 = 26;

		// Token: 0x04000FC0 RID: 4032
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateRightSiri2")]
		public const int NavigateRightSiri2 = 27;

		// Token: 0x04000FC1 RID: 4033
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action0")]
		public const int NavigateDownSiri2 = 28;

		// Token: 0x04000FC2 RID: 4034
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "NavigateLeftSiri2")]
		public const int NavigateLeftSiri2 = 29;

		// Token: 0x04000FC3 RID: 4035
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Middle mouse button pressed")]
		public const int MiddleMouse = 30;

		// Token: 0x04000FC4 RID: 4036
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Left Stick Press")]
		public const int LeftStickPress = 36;

		// Token: 0x04000FC5 RID: 4037
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Right Stick Press")]
		public const int RightStickPress = 37;

		// Token: 0x04000FC6 RID: 4038
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Go one page left")]
		public const int PageLeft = 42;

		// Token: 0x04000FC7 RID: 4039
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Go one page right")]
		public const int PageRight = 43;

		// Token: 0x04000FC8 RID: 4040
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Draw Mode Toggle")]
		public const int DrawModeToggle = 9;

		// Token: 0x04000FC9 RID: 4041
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Increase Game Speed")]
		public const int IncreaseSpeed = 10;

		// Token: 0x04000FCA RID: 4042
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Decrease Game Speed")]
		public const int DecreaseSpeed = 11;

		// Token: 0x04000FCB RID: 4043
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Pauses the game")]
		public const int Pause = 13;

		// Token: 0x04000FCC RID: 4044
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Sets to normal speed")]
		public const int NormalSpeed = 14;

		// Token: 0x04000FCD RID: 4045
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Sets fast forward")]
		public const int FastForward = 15;

		// Token: 0x04000FCE RID: 4046
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Toggles paused or not")]
		public const int TogglePause = 16;

		// Token: 0x04000FCF RID: 4047
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Start drawing roads")]
		public const int DrawRoads = 17;

		// Token: 0x04000FD0 RID: 4048
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Start Deleting Roads")]
		public const int DeleteRoads = 18;

		// Token: 0x04000FD1 RID: 4049
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Raise or Lock the upgrade toolbar")]
		public const int RaiseToolbar = 21;

		// Token: 0x04000FD2 RID: 4050
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Unlock or lower the upgrade toolbar")]
		public const int LowerToolbar = 22;

		// Token: 0x04000FD3 RID: 4051
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "When held, DrawRoads will actually delete.")]
		public const int DeleteModeModifier = 25;

		// Token: 0x04000FD4 RID: 4052
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Zooms in or out")]
		public const int ToggleZoomAction = 31;

		// Token: 0x04000FD5 RID: 4053
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Toggles the game UI visible")]
		public const int ToggleGameUI = 32;

		// Token: 0x04000FD6 RID: 4054
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Pan Camera vertical")]
		public const int PanVertical = 33;

		// Token: 0x04000FD7 RID: 4055
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Pan camera horizontal")]
		public const int PanHorizontal = 34;

		// Token: 0x04000FD8 RID: 4056
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Zooms in")]
		public const int ZoomInAction = 40;

		// Token: 0x04000FD9 RID: 4057
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Zooms out")]
		public const int ZoomOutAction = 41;

		// Token: 0x04000FDA RID: 4058
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "OpenElectiveUpgradeScreen")]
		public const int OpenElectiveUpgradeScreen = 44;

		// Token: 0x04000FDB RID: 4059
		[ActionIdFieldInfo(categoryName = "In Game Actions", friendlyName = "Sets extra fast forward")]
		public const int ExtraFastForward = 45;
	}
}
