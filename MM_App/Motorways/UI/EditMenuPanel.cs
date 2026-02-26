using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Client;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.UI.EditMenu;
using Motorways.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000723 RID: 1827
	public class EditMenuPanel : MonoBehaviour, IView, ICreatedInScopeHandler, IReleasedFromScopeHandler
	{
		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x06003242 RID: 12866 RVA: 0x000ED5CC File Offset: 0x000EB7CC
		// (set) Token: 0x06003243 RID: 12867 RVA: 0x000ED5D4 File Offset: 0x000EB7D4
		public ICreativeModeEditableObject EditableObject { get; private set; }

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x06003244 RID: 12868 RVA: 0x000ED5DD File Offset: 0x000EB7DD
		public bool IsOpen
		{
			get
			{
				return base.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x06003246 RID: 12870 RVA: 0x000ED5F3 File Offset: 0x000EB7F3
		// (set) Token: 0x06003245 RID: 12869 RVA: 0x000ED5EA File Offset: 0x000EB7EA
		public bool IsPlayingOpenEditMenuSequence { get; private set; }

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x06003248 RID: 12872 RVA: 0x000ED604 File Offset: 0x000EB804
		// (set) Token: 0x06003247 RID: 12871 RVA: 0x000ED5FB File Offset: 0x000EB7FB
		public bool IsPlayingCloseEditMenuSequence { get; private set; }

		// Token: 0x06003249 RID: 12873 RVA: 0x000ED60C File Offset: 0x000EB80C
		public Task OpenEditMenu(ICreativeModeEditableObject editableObject)
		{
			EditMenuPanel.<OpenEditMenu>d__40 <OpenEditMenu>d__;
			<OpenEditMenu>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OpenEditMenu>d__.<>4__this = this;
			<OpenEditMenu>d__.editableObject = editableObject;
			<OpenEditMenu>d__.<>1__state = -1;
			<OpenEditMenu>d__.<>t__builder.Start<EditMenuPanel.<OpenEditMenu>d__40>(ref <OpenEditMenu>d__);
			return <OpenEditMenu>d__.<>t__builder.Task;
		}

		// Token: 0x0600324A RID: 12874 RVA: 0x000ED658 File Offset: 0x000EB858
		private void InitControllerNavigation()
		{
			if (!Diagnostics.Verify(this._inputState.CurrentDeviceInputType == DeviceInputType.Controller, "Call this method only when the input type is Controller"))
			{
				return;
			}
			Selectable firstButton = this.GetFirstActiveButton();
			this._inputState.BlockGameInput = false;
			this._editMenuControllerWidget.Open();
			this._editMenuControllerWidget.TurnToFace(firstButton.transform.position, false);
			this._navigation.SetNewFocus(firstButton);
			this._gameUI.SetFocusPointActive(false, false);
		}

		// Token: 0x0600324B RID: 12875 RVA: 0x000ED6CE File Offset: 0x000EB8CE
		private void InitRemoteNavigation()
		{
			if (!Diagnostics.Verify(this._inputState.CurrentDeviceInputType == DeviceInputType.Remote, "Call this method only when the input type is Remote"))
			{
				return;
			}
			this._gameUI.SetFocusPointActive(true, false);
			this._editMenuControllerWidget.Close();
		}

		// Token: 0x0600324C RID: 12876 RVA: 0x000ED704 File Offset: 0x000EB904
		private void EmitEditMenuOpenedEvent()
		{
			InputEventSource source = InputEventSource.Any;
			if (this._inputState.CurrentDeviceInputType == DeviceInputType.Controller)
			{
				source = InputEventSource.Generic;
			}
			else if (this._inputState.CurrentDeviceInputType == DeviceInputType.Remote)
			{
				source = InputEventSource.Remote;
			}
			if (source != InputEventSource.Any)
			{
				InputEvent inputEvent = MotorwaysUIInputEvent.CreateGenericUIEvent(this._scope, 2, source, InputEventButtonState.JustDown, GameUIButtonType.EditMenuOpened, 0);
				this._scope.Get<PlayerActionController>().OnInputEvent((float)this._scope.Get<ClockModel>().Time, inputEvent);
			}
		}

		// Token: 0x0600324D RID: 12877 RVA: 0x000ED774 File Offset: 0x000EB974
		private Task PlayOpeningSequence()
		{
			EditMenuPanel.<PlayOpeningSequence>d__44 <PlayOpeningSequence>d__;
			<PlayOpeningSequence>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<PlayOpeningSequence>d__.<>4__this = this;
			<PlayOpeningSequence>d__.<>1__state = -1;
			<PlayOpeningSequence>d__.<>t__builder.Start<EditMenuPanel.<PlayOpeningSequence>d__44>(ref <PlayOpeningSequence>d__);
			return <PlayOpeningSequence>d__.<>t__builder.Task;
		}

		// Token: 0x0600324E RID: 12878 RVA: 0x000ED7B8 File Offset: 0x000EB9B8
		public void RefreshView(bool instantly = false)
		{
			foreach (EditMenuButton editMenuButton in this._editMenuButtons)
			{
				if (this.EditableObject != null && this.EditableObject.GetEditOptions().HasFlag(editMenuButton.ButtonType))
				{
					EditMenuButtonType buttonType = editMenuButton.ButtonType;
					if (buttonType <= EditMenuButtonType.UpgradeDowngrade)
					{
						if (buttonType != EditMenuButtonType.Flip)
						{
							if (buttonType == EditMenuButtonType.UpgradeDowngrade)
							{
								ICreativeModeEditableObject editableObject = this.EditableObject;
								CreativeModeEditableDestination creativeModeEditableDestination2 = editableObject as CreativeModeEditableDestination;
								if (creativeModeEditableDestination2 != null)
								{
									if (!creativeModeEditableDestination2.IsTrainStation)
									{
										goto IL_12C;
									}
									goto IL_127;
								}
								else
								{
									DraftDestination draftDestination2 = editableObject as DraftDestination;
									if (draftDestination2 != null && draftDestination2.IsTrainStation)
									{
										goto IL_127;
									}
									goto IL_12C;
								}
								IL_12F:
								bool flag;
								if (flag)
								{
									editMenuButton.SetButtonToState(EditMenuButton.ButtonState.Hidden);
									continue;
								}
								editMenuButton.SetButtonToState(EditMenuButton.ButtonState.Normal);
								DraftDestination draftDestination = this.EditableObject as DraftDestination;
								if (draftDestination == null || draftDestination.viewModel.activeBuilding.upgradeLevel != 0)
								{
									CreativeModeEditableDestination creativeModeEditableDestination = this.EditableObject as CreativeModeEditableDestination;
									if (creativeModeEditableDestination == null || creativeModeEditableDestination.view.Model.IsUpgraded)
									{
										editMenuButton.IconImage.sprite = this._downgradeSprite;
										continue;
									}
								}
								editMenuButton.IconImage.sprite = this._upgradeSprite;
								continue;
								IL_12C:
								flag = false;
								goto IL_12F;
								IL_127:
								flag = true;
								goto IL_12F;
							}
						}
						else
						{
							if (this._rotateFlipButtonCoroutine != null)
							{
								base.StopCoroutine(this._rotateFlipButtonCoroutine);
							}
							Quaternion flipButtonRotation = this.GetFlipButtonRotation();
							if (instantly)
							{
								editMenuButton.transform.rotation = flipButtonRotation;
							}
							else
							{
								base.StartCoroutine(this.RotateFlipButton(editMenuButton.transform.rotation, flipButtonRotation, editMenuButton));
							}
						}
					}
					else if (buttonType != EditMenuButtonType.Confirm)
					{
						if (buttonType == EditMenuButtonType.Delete)
						{
							editMenuButton.SetButtonToState(EditMenuButton.ButtonState.Normal);
						}
					}
					else if (this.EditableObject == null || !this.EditableObject.IsConfirmable())
					{
						editMenuButton.SetButtonToState(EditMenuButton.ButtonState.Disabled);
					}
					else
					{
						editMenuButton.SetButtonToState(EditMenuButton.ButtonState.Normal);
					}
				}
			}
		}

		// Token: 0x0600324F RID: 12879 RVA: 0x000ED9B8 File Offset: 0x000EBBB8
		private Quaternion GetFlipButtonRotation()
		{
			if (this.EditableObject != null && this.EditableObject.GetBuildingLayout() == BuildingLayout.BuildingToSide)
			{
				return Quaternion.Euler(0f, 0f, -90f);
			}
			return Quaternion.identity;
		}

		// Token: 0x06003250 RID: 12880 RVA: 0x000ED9EA File Offset: 0x000EBBEA
		private IEnumerator RotateFlipButton(Quaternion startRotation, Quaternion endRotation, EditMenuButton flipButton)
		{
			float startTime = Time.time;
			if (startRotation == endRotation)
			{
				yield break;
			}
			while (Time.time < startTime + this._flipButtonRotationSeconds)
			{
				float originalLerpFactor = (Time.time - startTime) / this._flipButtonRotationSeconds;
				float lerpFactor = (float)Math.Pow((double)originalLerpFactor, 3.0) * (originalLerpFactor * (6f * originalLerpFactor - 15f) + 10f);
				flipButton.transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpFactor);
				yield return new WaitForSeconds(0.001f);
			}
			flipButton.transform.rotation = endRotation;
			yield break;
		}

		// Token: 0x06003251 RID: 12881 RVA: 0x000EDA10 File Offset: 0x000EBC10
		private TouchButton GetFirstActiveButton()
		{
			EditMenuButtonType editOptions = Diagnostics.Verify(this.EditableObject != null) ? this.EditableObject.GetEditOptions() : EditMenuButtonType.Decline;
			foreach (EditMenuButton button in this._editMenuButtons)
			{
				if (button.ButtonType != (EditMenuButtonType)0 && editOptions.HasFlag(button.ButtonType) && button.gameObject.activeInHierarchy && button.interactable)
				{
					return button;
				}
			}
			EditMenuPanel.Log.Error("No active button found in EditMenuPanel!", Array.Empty<object>());
			return null;
		}

		// Token: 0x06003252 RID: 12882 RVA: 0x000EDACC File Offset: 0x000EBCCC
		public void ShowHideEditMenu(bool show)
		{
			this.CloseEditMenu();
		}

		// Token: 0x06003253 RID: 12883 RVA: 0x000EDAD8 File Offset: 0x000EBCD8
		private Task CloseEditMenu()
		{
			EditMenuPanel.<CloseEditMenu>d__50 <CloseEditMenu>d__;
			<CloseEditMenu>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CloseEditMenu>d__.<>4__this = this;
			<CloseEditMenu>d__.<>1__state = -1;
			<CloseEditMenu>d__.<>t__builder.Start<EditMenuPanel.<CloseEditMenu>d__50>(ref <CloseEditMenu>d__);
			return <CloseEditMenu>d__.<>t__builder.Task;
		}

		// Token: 0x06003254 RID: 12884 RVA: 0x000EDB1C File Offset: 0x000EBD1C
		private Task PlayCloseSequence()
		{
			EditMenuPanel.<PlayCloseSequence>d__51 <PlayCloseSequence>d__;
			<PlayCloseSequence>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<PlayCloseSequence>d__.<>4__this = this;
			<PlayCloseSequence>d__.<>1__state = -1;
			<PlayCloseSequence>d__.<>t__builder.Start<EditMenuPanel.<PlayCloseSequence>d__51>(ref <PlayCloseSequence>d__);
			return <PlayCloseSequence>d__.<>t__builder.Task;
		}

		// Token: 0x06003255 RID: 12885 RVA: 0x000EDB60 File Offset: 0x000EBD60
		public void DeleteButton()
		{
			if (!Diagnostics.Verify(this.EditableObject != null, "EditableObjects shouldn't be null at time of deletion!"))
			{
				return;
			}
			bool isOriginalDeleted;
			this.OpenGhostPreview(out isOriginalDeleted);
			this.EditableObject.Delete(isOriginalDeleted);
			this.EditableObject = null;
			this.CloseEditMenu();
		}

		// Token: 0x06003256 RID: 12886 RVA: 0x000EDBA8 File Offset: 0x000EBDA8
		public void ConfirmEdit()
		{
			EditMenuPanel.Log.Info("Confirming edit at position {0}", new object[]
			{
				base.transform.position
			});
			if (this.EditableObject != null)
			{
				if (this.EditableObject.IsConfirmable())
				{
					this.EditableObject.Confirm();
				}
				else
				{
					this.EditableObject.Cancel();
				}
			}
			this.EditableObject = null;
			this.CloseEditMenu();
		}

		// Token: 0x06003257 RID: 12887 RVA: 0x000EDC18 File Offset: 0x000EBE18
		public void CancelEdit()
		{
			if (this.EditableObject != null)
			{
				this.EditableObject.Cancel();
				this.EditableObject = null;
			}
			this.CloseEditMenu();
		}

		// Token: 0x06003258 RID: 12888 RVA: 0x000EDC3C File Offset: 0x000EBE3C
		public void FlipButton()
		{
			bool isOriginalDeleted;
			this.OpenGhostPreview(out isOriginalDeleted);
			this.EditableObject.Flip(isOriginalDeleted);
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x000EDC60 File Offset: 0x000EBE60
		public void UpgradeDowngradeButton()
		{
			bool isOriginalDeleted;
			this.OpenGhostPreview(out isOriginalDeleted);
			this.EditableObject.UpgradeOrDowngrade(isOriginalDeleted);
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x000EDC84 File Offset: 0x000EBE84
		public void RotateButton()
		{
			bool isOriginalDeleted;
			this.OpenGhostPreview(out isOriginalDeleted);
			this.EditableObject.Rotate(isOriginalDeleted);
		}

		// Token: 0x0600325B RID: 12891 RVA: 0x000EDCA6 File Offset: 0x000EBEA6
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			this.UpdatePanelPosition();
			return TickResult.ContinueTicking;
		}

		// Token: 0x0600325C RID: 12892 RVA: 0x000EDCB0 File Offset: 0x000EBEB0
		private void UpdatePanelPosition()
		{
			if (this.EditableObject == null)
			{
				return;
			}
			Vector2 worldPos = this.EditableObject.GetCenterForEditMenuPosition();
			Vector2 screenPos = this._gameCamera.UICamera.WorldToScreenPoint(worldPos);
			Vector2 scaledPos = this._gameUI.NormalizePositionToScaledScreenSize(screenPos);
			base.gameObject.GetComponent<RectTransform>().anchoredPosition = scaledPos;
		}

		// Token: 0x0600325D RID: 12893 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetGameobjectActive(bool isActive)
		{
		}

		// Token: 0x0600325E RID: 12894 RVA: 0x000EDD0C File Offset: 0x000EBF0C
		public void OnCreatedInScope(IScope newScope)
		{
			this._scope = newScope;
			foreach (TouchButton touchButton in this._buttonGroup.buttons)
			{
				touchButton.Initialize(this._scope);
				EditMenuButton editMenuButton = touchButton as EditMenuButton;
				if (editMenuButton != null)
				{
					this._editMenuButtons.Add(editMenuButton);
					EditMenuButton editMenuButton2 = editMenuButton;
					editMenuButton2.onPointerEnter = (EditMenuButton.OnFocusPointerEnter)Delegate.Combine(editMenuButton2.onPointerEnter, new EditMenuButton.OnFocusPointerEnter(this.OnAssetButtonPointerEnter));
					EditMenuButton editMenuButton3 = editMenuButton;
					editMenuButton3.onPointerExit = (EditMenuButton.OnFocusPointerExit)Delegate.Combine(editMenuButton3.onPointerExit, new EditMenuButton.OnFocusPointerExit(this.OnAssetButtonPointerExit));
					editMenuButton.AddOnSelectedEvent(new UnityAction(this.OnAssetButtonSelected));
				}
			}
			this._cameraView.OnCameraZoomLevelChanged += this.HandleCameraZoom;
		}

		// Token: 0x0600325F RID: 12895 RVA: 0x000EDDF4 File Offset: 0x000EBFF4
		private void OnAssetButtonPointerEnter(EditMenuButton button)
		{
			if (this._inputState.CurrentDeviceInputType == DeviceInputType.Remote)
			{
				this._navigation.SetNewFocus(button);
			}
		}

		// Token: 0x06003260 RID: 12896 RVA: 0x000EDE10 File Offset: 0x000EC010
		private void OnAssetButtonPointerExit(EditMenuButton button)
		{
			if (this._inputState.CurrentDeviceInputType == DeviceInputType.Remote)
			{
				this._navigation.ReleaseUIFocus();
			}
		}

		// Token: 0x06003261 RID: 12897 RVA: 0x000EDE2C File Offset: 0x000EC02C
		public void SelectButtonAtDirection(Vector2 direction)
		{
			float minDiff = float.MaxValue;
			TouchButton closetButton = null;
			foreach (TouchButton touchButton in this._buttonGroup.buttons)
			{
				if (touchButton.gameObject.activeInHierarchy && touchButton.interactable)
				{
					Vector2 buttonPos = touchButton.transform.localPosition;
					float current = Mathf.Atan2(buttonPos.y, buttonPos.x) * 57.29578f;
					float directionAngle = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
					float diff = Mathf.Abs(Mathf.DeltaAngle(current, directionAngle));
					if (diff < minDiff)
					{
						minDiff = diff;
						closetButton = touchButton;
					}
				}
			}
			if (closetButton != null)
			{
				this._navigation.SetNewFocus(closetButton);
			}
		}

		// Token: 0x06003262 RID: 12898 RVA: 0x000EDF10 File Offset: 0x000EC110
		private void OnAssetButtonSelected()
		{
			EditMenuButton selectedButton = this._navigation.GetCurrentFocus() as EditMenuButton;
			if (selectedButton == null)
			{
				return;
			}
			if (!Diagnostics.Verify(this._editMenuControllerWidget != null, "EditMenuControllerWidget is null, set it in the prefab."))
			{
				return;
			}
			this._editMenuControllerWidget.TurnToFace(selectedButton.transform.position, true);
		}

		// Token: 0x06003263 RID: 12899 RVA: 0x000EDF68 File Offset: 0x000EC168
		public void MoveButton()
		{
			EditMenuPanel.Log.Info("MoveButton pressed, from device with input type ", new object[]
			{
				this._inputState.CurrentDeviceInputType
			});
			if (Diagnostics.Verify(this._scope != null))
			{
				DeviceInputType currentDeviceInputType = this._inputState.CurrentDeviceInputType;
				InputEvent assetButtonPressEvent;
				if (currentDeviceInputType != DeviceInputType.Touch)
				{
					if (currentDeviceInputType != DeviceInputType.Mouse)
					{
						assetButtonPressEvent = MotorwaysUIInputEvent.CreateGenericUIEvent(this._scope, 2, InputEventSource.Generic, InputEventButtonState.JustDown, GameUIButtonType.MoveCreativeModeObject, 0);
					}
					else
					{
						assetButtonPressEvent = MotorwaysUIInputEvent.CreateMouseUIEvent(this._scope, InputEventMouseButtonType.LeftMouse, InputEventButtonState.JustDown, GameUIButtonType.MoveCreativeModeObject, 0);
					}
				}
				else
				{
					assetButtonPressEvent = MotorwaysUIInputEvent.CreateTouchUIEvent(this._scope, 0, InputEventButtonState.JustDown, GameUIButtonType.MoveCreativeModeObject, 0);
				}
				this._scope.Get<PlayerActionController>().OnInputEvent((float)this._scope.Get<ClockModel>().Time, assetButtonPressEvent);
			}
		}

		// Token: 0x06003264 RID: 12900 RVA: 0x000EE024 File Offset: 0x000EC224
		public void LayoutButtons()
		{
			if (this._buttonGroup == null || this._buttonGroup.transform.childCount == 0)
			{
				return;
			}
			float offsetAngle = 360f / (float)this._buttonGroup.transform.childCount;
			float angle = this.Offset;
			for (int i = 0; i < this._buttonGroup.transform.childCount; i++)
			{
				RectTransform child = (RectTransform)this._buttonGroup.transform.GetChild(i);
				if (child != null)
				{
					Vector3 pos = new Vector3(Mathf.Sin(angle * 0.017453292f), Mathf.Cos(angle * 0.017453292f), 0f);
					child.localPosition = pos * this.Radius;
					angle += offsetAngle;
				}
			}
		}

		// Token: 0x06003265 RID: 12901 RVA: 0x000EE0E8 File Offset: 0x000EC2E8
		public void OnReleasedFromScope(IScope scope)
		{
			this.CleanUpColourManagement();
			if (this.EditableObject != null)
			{
				scope.Release(this.EditableObject);
				this.EditableObject = null;
			}
			this._editMenuButtons.Clear();
			this._cameraView.OnCameraZoomLevelChanged -= this.HandleCameraZoom;
		}

		// Token: 0x06003266 RID: 12902 RVA: 0x000EE139 File Offset: 0x000EC339
		private ICreativeModeEditableObject OpenGhostPreview(out bool isOriginalDeleted)
		{
			this.EditableObject = this.EditableObject.GetGhostPreview(out isOriginalDeleted);
			return this.EditableObject;
		}

		// Token: 0x06003267 RID: 12903 RVA: 0x000EE154 File Offset: 0x000EC354
		public void ColourButton()
		{
			bool isOriginalDeleted;
			this.OpenGhostPreview(out isOriginalDeleted);
			int nextGroupIndex = this.GetNextGroupIndex();
			EditMenuPanel.Log.Info("CreativeModeEditableDestination: changed {0} to {1}", new object[]
			{
				this.EditableObject.GetGroupIndex(),
				nextGroupIndex
			});
			this.EditableObject.SetGroupIndex(nextGroupIndex, isOriginalDeleted);
		}

		// Token: 0x06003268 RID: 12904 RVA: 0x000EE1AF File Offset: 0x000EC3AF
		private int GetMaxGroupIndices()
		{
			return this._scope.Get<City>().Definition.schedulePlanner.demandOscillationData.Count;
		}

		// Token: 0x06003269 RID: 12905 RVA: 0x000EE1D0 File Offset: 0x000EC3D0
		private void HandleCameraZoom()
		{
			if (!this.IsOpen)
			{
				return;
			}
			if (this.EditableObject != null)
			{
				if (this.EditableObject.IsConfirmable())
				{
					this.EditableObject.Confirm();
				}
				else
				{
					this.EditableObject.Cancel();
				}
			}
			this.EditableObject = null;
			this.CloseEditMenu();
		}

		// Token: 0x0600326A RID: 12906 RVA: 0x000EE224 File Offset: 0x000EC424
		private void ApplyCameraOffset()
		{
			RectFixed clientPlayableAreaAtTime = this._city.GetClientPlayableAreaAtTime(Fix64.MaxValue, City.PlayableAreaRoundingType.AllowPartialTiles);
			Vector2Int editableTilePos = this.EditableObject.GetTilePosition();
			int absX = Mathf.Abs(editableTilePos.x);
			int absY = Mathf.Abs(editableTilePos.y);
			float widthBoundary = Mathf.Abs((float)clientPlayableAreaAtTime.x / 2f);
			float heightBoundary = Mathf.Abs((float)clientPlayableAreaAtTime.y / 2f);
			if ((float)absX > widthBoundary - (float)this._panArea || (float)absY > heightBoundary - (float)this._panArea)
			{
				Vector2Fixed worldPos = TilemapModel.GetWorldPositionForCoordinates(editableTilePos);
				Vector3 focusPoint = new Vector3((float)Mathf.RoundToInt((float)worldPos.x * this._horizontalOffsetScalar), (float)Mathf.RoundToInt((float)worldPos.y * this._verticalOffsetScalar));
				this._cameraView.SetEditMenuFocusPoint(focusPoint);
			}
		}

		// Token: 0x0600326B RID: 12907 RVA: 0x000EE303 File Offset: 0x000EC503
		private int GetNextGroupIndex()
		{
			if (this._maxGroupIndex < 0)
			{
				this._maxGroupIndex = this.GetMaxGroupIndices();
				if (this._maxGroupIndex <= 0)
				{
					return this.EditableObject.GetGroupIndex();
				}
			}
			return (this.EditableObject.GetGroupIndex() + 1) % this._maxGroupIndex;
		}

		// Token: 0x0600326C RID: 12908 RVA: 0x000EE343 File Offset: 0x000EC543
		private void CleanUpColourManagement()
		{
			this._maxGroupIndex = -1;
		}

		// Token: 0x04002B16 RID: 11030
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("EditMenuPanel");

		// Token: 0x04002B17 RID: 11031
		[Dependency]
		private MenuNavigation _navigation;

		// Token: 0x04002B18 RID: 11032
		[Dependency]
		private InputState _inputState;

		// Token: 0x04002B19 RID: 11033
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002B1A RID: 11034
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002B1B RID: 11035
		[Dependency]
		private GameUIScreen _gameUI;

		// Token: 0x04002B1C RID: 11036
		[Dependency]
		protected City _city;

		// Token: 0x04002B1D RID: 11037
		[Dependency]
		private IScope _scope;

		// Token: 0x04002B1E RID: 11038
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x04002B1F RID: 11039
		[SerializeField]
		private ButtonGroup _buttonGroup;

		// Token: 0x04002B20 RID: 11040
		[SerializeField]
		private CanvasGroup _canvasGroup;

		// Token: 0x04002B21 RID: 11041
		[SerializeField]
		private EditMenuControllerWidget _editMenuControllerWidget;

		// Token: 0x04002B22 RID: 11042
		[Tooltip("How many tiles from the edge of the map will result in a camera pan.")]
		[SerializeField]
		private int _panArea = 5;

		// Token: 0x04002B23 RID: 11043
		[SerializeField]
		[Tooltip("Horizontal multiplier for camera panning.")]
		private float _horizontalOffsetScalar = 0.3f;

		// Token: 0x04002B24 RID: 11044
		[Tooltip("Vertical multiplier for camera panning.")]
		[SerializeField]
		private float _verticalOffsetScalar = 0.8f;

		// Token: 0x04002B25 RID: 11045
		private bool _cancelCloseSequence;

		// Token: 0x04002B26 RID: 11046
		private readonly List<EditMenuButton> _editMenuButtons = new List<EditMenuButton>();

		// Token: 0x04002B27 RID: 11047
		private int _maxGroupIndex = -1;

		// Token: 0x04002B29 RID: 11049
		[SerializeField]
		private float ButtonShowDelay = 0.5f;

		// Token: 0x04002B2A RID: 11050
		[SerializeField]
		private float PanelOutroTime = 0.2f;

		// Token: 0x04002B2B RID: 11051
		[SerializeField]
		private float Radius = 10f;

		// Token: 0x04002B2C RID: 11052
		[SerializeField]
		private float Offset = 10f;

		// Token: 0x04002B2D RID: 11053
		private Coroutine _rotateFlipButtonCoroutine;

		// Token: 0x04002B2E RID: 11054
		[SerializeField]
		private float _flipButtonRotationSeconds = 0.15f;

		// Token: 0x04002B2F RID: 11055
		[SerializeField]
		private Sprite _upgradeSprite;

		// Token: 0x04002B30 RID: 11056
		[SerializeField]
		private Sprite _downgradeSprite;
	}
}
