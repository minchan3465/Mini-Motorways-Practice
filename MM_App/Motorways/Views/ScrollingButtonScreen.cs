using System;
using System.Collections.Generic;
using Easing;
using Factory;
using Motorways.Audio;
using Motorways.UI;
using Screens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200056E RID: 1390
	public abstract class ScrollingButtonScreen : BaseScalingScreen
	{
		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x060025E7 RID: 9703 RVA: 0x000A0570 File Offset: 0x0009E770
		protected AnimatedCard CurrentlySelectedButton
		{
			get
			{
				if (Diagnostics.Verify(this._currentlySelectedButtonIndex < this.ButtonCount, "Only have {0}, but trying to get index {1}!", this.ButtonCount, this._currentlySelectedButtonIndex))
				{
					return this.buttons[this._currentlySelectedButtonIndex];
				}
				return this.buttons[0];
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x060025E8 RID: 9704 RVA: 0x000A05CB File Offset: 0x0009E7CB
		private bool IsScrolling
		{
			get
			{
				return this._desiredScrollPosition >= 0f;
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x060025E9 RID: 9705 RVA: 0x000A05DD File Offset: 0x0009E7DD
		public int ButtonCount
		{
			get
			{
				return this.buttons.Count;
			}
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x000A05EA File Offset: 0x0009E7EA
		public void CancelButtonScrolling()
		{
			this._desiredScrollPosition = -1f;
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x000A05F8 File Offset: 0x0009E7F8
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this.IsScrolling)
			{
				float time = Mathf.Min(deltaTime * 5f, 1f);
				this.scrollRect.horizontalNormalizedPosition += (this._desiredScrollPosition - this.scrollRect.horizontalNormalizedPosition) * time;
				if (Mathf.Abs(this._desiredScrollPosition - this.scrollRect.horizontalNormalizedPosition) < 0.0001f)
				{
					this.scrollRect.horizontalNormalizedPosition = this._desiredScrollPosition;
					this.CancelButtonScrolling();
				}
			}
			else if (this.ButtonCount > 0)
			{
				if (this.scrollRect.velocity.magnitude < this._snapSpeedThreshold && this._shouldSnapDrag)
				{
					this.ScrollToNearestButton();
					this._shouldSnapDrag = false;
				}
				if (this._wasDraggedByScrollWheel)
				{
					this._shouldSnapDrag = true;
					this._wasDraggedByScrollWheel = false;
				}
			}
			if (base.IsVisible())
			{
				this._gameCamera.transform.position = this.GetCameraPosition();
			}
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000A06F0 File Offset: 0x0009E8F0
		protected int GetNearestButtonIndex()
		{
			float buttonDistance = 1f / (float)(this.buttons.Count - 1);
			return Mathf.RoundToInt(Mathf.Clamp(this.scrollRect.horizontalNormalizedPosition / buttonDistance, 0f, (float)(this.buttons.Count - 1)));
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x000A073C File Offset: 0x0009E93C
		public bool HasValidCameraPosition()
		{
			return this._originPosition != null;
		}

		// Token: 0x060025EE RID: 9710 RVA: 0x000A074C File Offset: 0x0009E94C
		public Vector3 GetCameraPosition()
		{
			if (!Diagnostics.Verify(this._originPosition != null, "Somehow trying to get the camera position when we haven't been initialised!"))
			{
				return Vector3.right * this.scrollRect.horizontalNormalizedPosition * this.buttonParent.sizeDelta.x * base.transform.localScale.x;
			}
			if (this.ButtonCount > 1)
			{
				float paralaxMultiplier = 1f - this.scrollRect.GetComponent<RectTransform>().sizeDelta.x / this.buttonParent.sizeDelta.x;
				this._rectTransform.anchoredPosition = this._originPosition.Value + Vector3.right * (this.scrollRect.horizontalNormalizedPosition * this.buttonParent.sizeDelta.x * base.transform.localScale.x * paralaxMultiplier);
				Vector3 newPosition = base.transform.position;
				newPosition.z = this._gameCamera.transform.position.z;
				return newPosition;
			}
			Vector3 newPosition2 = this._originPosition.Value;
			newPosition2.z = this._gameCamera.transform.position.z;
			return newPosition2;
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x000A0898 File Offset: 0x0009EA98
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			if (Diagnostics.Verify(this._safeArea != null, this, "{0} has not been set for {1}", "_safeArea", base.gameObject.name))
			{
				float rectScale = this._safeArea.GetComponent<RectTransform>().rect.height / BaseScalingScreen.referenceResolution.y;
				this.scrollRect.transform.localScale = new Vector3(rectScale, rectScale, rectScale);
			}
			this.AssignOriginPosition();
			if (this.buttons.Count != 0)
			{
				this.ScrollToButton(this.CurrentlySelectedButton, true);
			}
			this._shouldSnapDrag = true;
		}

		// Token: 0x060025F0 RID: 9712 RVA: 0x000A0938 File Offset: 0x0009EB38
		protected void AssignOriginPosition()
		{
			if (this._originPosition == null)
			{
				Vector3 newPosition = this._screenStack.GetPositionFor(base.ScreenType);
				float paralaxMultiplier = 1f - this.scrollRect.GetComponent<RectTransform>().sizeDelta.x / Mathf.Max(this.buttonParent.sizeDelta.x, 1f);
				newPosition.z = -0.25f;
				newPosition.x -= this.scrollRect.horizontalNormalizedPosition * this.buttonParent.sizeDelta.x * base.transform.localScale.x * paralaxMultiplier;
				this._originPosition = new Vector3?(newPosition);
				base.transform.position = newPosition;
			}
		}

		// Token: 0x060025F1 RID: 9713 RVA: 0x000A09FC File Offset: 0x0009EBFC
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._scaleToCamera = false;
		}

		// Token: 0x060025F2 RID: 9714 RVA: 0x000A0A0B File Offset: 0x0009EC0B
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.SetMapButtonValues));
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x000A0A30 File Offset: 0x0009EC30
		public void ScrollToNearestButton()
		{
			if (this.buttons.Count == 1)
			{
				this._desiredScrollPosition = 0f;
				return;
			}
			int buttonIndex = this.GetNearestButtonIndex();
			if (Diagnostics.Verify(buttonIndex >= 0 && buttonIndex < this.buttons.Count))
			{
				this.ScrollToButton(this.buttons[buttonIndex], false);
			}
		}

		// Token: 0x060025F4 RID: 9716 RVA: 0x000A0A8D File Offset: 0x0009EC8D
		public void OnEndDrag()
		{
			this._shouldSnapDrag = true;
		}

		// Token: 0x060025F5 RID: 9717 RVA: 0x000A0A96 File Offset: 0x0009EC96
		public void OnStartDrag()
		{
			this._shouldSnapDrag = false;
		}

		// Token: 0x060025F6 RID: 9718 RVA: 0x000A0AA0 File Offset: 0x0009ECA0
		public void OnScroll(Vector2 scrollDelta)
		{
			this._shouldSnapDrag = false;
			this._wasDraggedByScrollWheel = true;
			if (scrollDelta.y > 0f)
			{
				if (this._currentlySelectedButtonIndex < this.buttons.Count - 1)
				{
					this.ScrollToButton(this.buttons[this._currentlySelectedButtonIndex + 1], false);
					return;
				}
			}
			else if (scrollDelta.y < 0f && this._currentlySelectedButtonIndex > 0)
			{
				this.ScrollToButton(this.buttons[this._currentlySelectedButtonIndex - 1], false);
			}
		}

		// Token: 0x060025F7 RID: 9719 RVA: 0x000A0B28 File Offset: 0x0009ED28
		protected int IndexOf(AnimatedCard button)
		{
			for (int buttonIndex = 0; buttonIndex < this.buttons.Count; buttonIndex++)
			{
				if (this.buttons[buttonIndex] == button)
				{
					return buttonIndex;
				}
			}
			Diagnostics.FailAssert("We haven't stored {0} in mapButtons! Defaulting to the first button.", new object[]
			{
				button
			});
			return 0;
		}

		// Token: 0x060025F8 RID: 9720 RVA: 0x000A0B78 File Offset: 0x0009ED78
		public virtual void ScrollToButton(AnimatedCard button, bool instantly = false)
		{
			if (this.buttons != null && this.buttons.Count > 1)
			{
				if (this.CurrentlySelectedButton != button)
				{
					this._currentlySelectedButtonIndex = this.IndexOf(button);
				}
				if (instantly)
				{
					this.scrollRect.horizontalNormalizedPosition = (float)this._currentlySelectedButtonIndex / (float)(this.ButtonCount - 1);
					this._desiredScrollPosition = this.scrollRect.horizontalNormalizedPosition;
				}
				else
				{
					this._desiredScrollPosition = (float)this._currentlySelectedButtonIndex / (float)(this.ButtonCount - 1);
					float duration = Mathf.Abs(this._desiredScrollPosition - this.scrollRect.horizontalNormalizedPosition);
					if (duration > 0.01f)
					{
						SFX.PointerTargetDelta = this._desiredScrollPosition - this.scrollRect.horizontalNormalizedPosition;
						this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Transition, UIAudioProfile.None, duration, true, null, ScreenStack.MotorwaysScreen.MapSelect, ScreenStack.MotorwaysScreen.None));
						this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.MouseOver, UIAudioProfile.Theme, -1f, true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
					}
				}
			}
			this.OnSelectButton();
		}

		// Token: 0x060025F9 RID: 9721 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnSelectButton()
		{
		}

		// Token: 0x060025FA RID: 9722 RVA: 0x000A0C80 File Offset: 0x0009EE80
		protected void SetNewButtons(List<AnimatedCard> newButtons)
		{
			this.DestroyButtons();
			this.buttons.AddRange(newButtons);
			foreach (AnimatedCard animatedCard in this.buttons)
			{
				animatedCard.transform.SetParent(this.buttonParent, false);
			}
		}

		// Token: 0x060025FB RID: 9723 RVA: 0x000A0CF0 File Offset: 0x0009EEF0
		protected void AddNewButtonToExistingSet(AnimatedCard newButton)
		{
			this.buttons.Add(newButton);
			Canvas.ForceUpdateCanvases();
			base.RegisterAllLocalizedTextChildren();
			base.RegisterButtons();
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x000A0D20 File Offset: 0x0009EF20
		protected void SetMapButtonValues(Vector2 position)
		{
			if (this.ButtonCount > 0)
			{
				if (this.ButtonCount == 1)
				{
					this.buttons[0].SetHighlightAnimation(0f);
					return;
				}
				float distanceBetweenButtons = 1f / (float)(this.buttons.Count - 1);
				float focusPosition = position.x;
				for (int buttonIndex = 0; buttonIndex < this.buttons.Count; buttonIndex++)
				{
					float buttonPosition = distanceBetweenButtons * (float)buttonIndex;
					float clampedDistanceFromFocus = Mathf.Min(Mathf.Abs(focusPosition - buttonPosition), distanceBetweenButtons);
					float transitionAmount = Easings.Interpolate(1f - clampedDistanceFromFocus / distanceBetweenButtons, Easings.Functions.SineEaseInOut);
					this.buttons[buttonIndex].SetHighlightAnimation(transitionAmount);
				}
			}
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x000A0DC8 File Offset: 0x0009EFC8
		protected void DestroyButtons()
		{
			if (this.buttons != null)
			{
				for (int mapButtonIndex = 0; mapButtonIndex < this.buttons.Count; mapButtonIndex++)
				{
					this.buttons[mapButtonIndex].gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(this.buttons[mapButtonIndex].gameObject);
				}
				this.buttons.Clear();
			}
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x000A0E30 File Offset: 0x0009F030
		public override void OnMoveCursor(Selectable currentFocus, MoveDirection direction)
		{
			if (currentFocus == this.firstFocus)
			{
				if (direction == MoveDirection.Right)
				{
					if (this._currentlySelectedButtonIndex < this.buttons.Count - 1)
					{
						this.ScrollToButton(this.buttons[this._currentlySelectedButtonIndex + 1], false);
						return;
					}
				}
				else if (direction == MoveDirection.Left && this._currentlySelectedButtonIndex > 0)
				{
					this.ScrollToButton(this.buttons[this._currentlySelectedButtonIndex - 1], false);
				}
			}
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x000A0EA5 File Offset: 0x0009F0A5
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			this.DestroyButtons();
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x000A0EB4 File Offset: 0x0009F0B4
		public override void Reset()
		{
			base.Reset();
			this.scrollRect.horizontalNormalizedPosition = 0f;
			this._desiredScrollPosition = 0f;
			this._originPosition = null;
			this._scaleToCamera = true;
			this._currentlySelectedButtonIndex = 0;
			this._shouldSnapDrag = true;
			this._wasDraggedByScrollWheel = false;
			base.transform.position = Vector3.zero;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x04001FE3 RID: 8163
		public RectTransform buttonParent;

		// Token: 0x04001FE4 RID: 8164
		public ScrollRect scrollRect;

		// Token: 0x04001FE5 RID: 8165
		[SerializeField]
		private float _snapSpeedThreshold = 250f;

		// Token: 0x04001FE6 RID: 8166
		private float _desiredScrollPosition;

		// Token: 0x04001FE7 RID: 8167
		protected readonly List<AnimatedCard> buttons = new List<AnimatedCard>();

		// Token: 0x04001FE8 RID: 8168
		protected int _currentlySelectedButtonIndex;

		// Token: 0x04001FE9 RID: 8169
		private bool _shouldSnapDrag = true;

		// Token: 0x04001FEA RID: 8170
		private bool _wasDraggedByScrollWheel;

		// Token: 0x04001FEB RID: 8171
		private Vector3? _originPosition;

		// Token: 0x04001FEC RID: 8172
		[SerializeField]
		private SafeArea _safeArea;

		// Token: 0x04001FED RID: 8173
		private const Easings.Functions ButtonScrollEasing = Easings.Functions.SineEaseInOut;
	}
}
