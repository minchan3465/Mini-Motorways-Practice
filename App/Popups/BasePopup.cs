using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Factory;
using Factory.Pools;
using Motorways.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Popups
{
	// Token: 0x020002D3 RID: 723
	[RequireComponent(typeof(DelegateCanvasGroup))]
	public class BasePopup : MonoBehaviour, IReusable, ICreatedInScopeHandler, IReleasedFromScopeHandler, MenuNavigation.IObserver, InputState.IObserver
	{
		// Token: 0x1700038E RID: 910
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0003B38C File Offset: 0x0003958C
		public bool IsFullyVisible
		{
			get
			{
				return this.isFullyVisible;
			}
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x0003B394 File Offset: 0x00039594
		private void Awake()
		{
			this._delegateCanvasGroup = base.GetComponent<DelegateCanvasGroup>();
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x0003B3A4 File Offset: 0x000395A4
		public void OnCreatedInScope(IScope scope)
		{
			base.transform.SetParent(this._popupParent.transform, false);
			this._delegateCanvasGroup.SetInteractable(false);
			this._delegateCanvasGroup.SetBlocksRaycasts(false);
			this._delegateCanvasGroup.Alpha = 0f;
			this.isFullyVisible = false;
			this.RegisterAllLocalizedTextChildren();
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x0003B3FD File Offset: 0x000395FD
		public void OnReleasedFromScope(IScope scope)
		{
			this.UnregisterLocalizedTextChildren();
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool CanBeDismissed()
		{
			return true;
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x0003B405 File Offset: 0x00039605
		public virtual void OnOpened(float delay)
		{
			this.appScope.Get<IInputState>().BlockAllInput = true;
			this.OnReceivedFocus();
			this.TweenInOut(true, new Action(this.<OnOpened>g__OnTweenComplete|18_0), delay, false);
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x0003B433 File Offset: 0x00039633
		public virtual void OnClosed(Action onComplete = null, bool skipTransition = false)
		{
			this.OnLostFocus();
			this.isFullyVisible = false;
			this.TweenInOut(false, onComplete, 0f, skipTransition);
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x0003B450 File Offset: 0x00039650
		public void OnReceivedFocus()
		{
			this.RegisterButtons();
			this._delegateCanvasGroup.SetInteractable(true);
			this._delegateCanvasGroup.SetBlocksRaycasts(true);
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x0003B470 File Offset: 0x00039670
		public void OnLostFocus()
		{
			this.UnregisterButtons();
			this._delegateCanvasGroup.SetInteractable(false);
			this._delegateCanvasGroup.SetBlocksRaycasts(false);
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnPopupClosed()
		{
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x0003B490 File Offset: 0x00039690
		private void TweenInOut(bool isIn, Action onTweenComplete, float delay = 0f, bool skipTransition = false)
		{
			this._tweenCoroutine = base.StartCoroutine(this.TweenInOutCoroutine(isIn, onTweenComplete, delay, skipTransition));
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x0003B4A9 File Offset: 0x000396A9
		private IEnumerator TweenInOutCoroutine(bool isIn, Action onTweenComplete, float delay, bool skipTransition)
		{
			if (!skipTransition)
			{
				yield return new WaitForSeconds(delay);
			}
			float time = 0f;
			float startAlpha = this._delegateCanvasGroup.Alpha;
			float targetAlpha = (float)(isIn ? 1 : 0);
			if (!skipTransition)
			{
				while (time < this._tweenDuration)
				{
					float progress = time / this._tweenDuration;
					this._delegateCanvasGroup.Alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
					time += Time.deltaTime;
					yield return null;
				}
			}
			this._delegateCanvasGroup.Alpha = targetAlpha;
			if (!isIn)
			{
				this.OnPopupClosed();
			}
			if (onTweenComplete != null)
			{
				onTweenComplete();
			}
			this._tweenCoroutine = null;
			yield break;
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x0003B4D8 File Offset: 0x000396D8
		protected virtual void RegisterAllLocalizedTextChildren()
		{
			this.UnregisterLocalizedTextChildren();
			base.GetComponentsInChildren<LocalizedTextUI>(true, this._allLocalizedText);
			for (int newIndex = 0; newIndex < this._allLocalizedText.Count; newIndex++)
			{
				if (!this._allLocalizedText[newIndex].isInitialized)
				{
					this._allLocalizedText[newIndex].HandleParentAllocated(this.appScope);
				}
				this._localeDatabase.AddLocalizedObject(this._allLocalizedText[newIndex]);
			}
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x0003B550 File Offset: 0x00039750
		protected virtual void UnregisterLocalizedTextChildren()
		{
			for (int oldIndex = 0; oldIndex < this._allLocalizedText.Count; oldIndex++)
			{
				this._allLocalizedText[oldIndex].Unregister();
				this._localeDatabase.RemoveLocalizedObject(this._allLocalizedText[oldIndex]);
			}
			this._allLocalizedText.Clear();
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x0003B5A8 File Offset: 0x000397A8
		private void RegisterButtons()
		{
			base.GetComponentsInChildren<VariableDeviceSelectable>(true, this._allButtons);
			for (int buttonIndex = 0; buttonIndex < this._allButtons.Count; buttonIndex++)
			{
				if (!this._allButtons[buttonIndex].IsInitialized)
				{
					this._allButtons[buttonIndex].Initialize(this.appScope);
				}
			}
			if (Diagnostics.Verify(this._firstFocus != null) && this.appScope.Get<InputState>().CurrentInputTypeRequiresFocus)
			{
				this.navigation.SetNewFocus(this._firstFocus);
			}
			this.navigation.Subscribe(this);
			this.inputState.Subscribe(this);
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x0003B650 File Offset: 0x00039850
		public virtual void UnregisterButtons()
		{
			foreach (VariableDeviceSelectable variableDeviceSelectable in this._allButtons)
			{
				variableDeviceSelectable.Unregister();
			}
			this._allButtons.Clear();
			this.navigation.Unsubscribe(this);
			this.inputState.Unsubscribe(this);
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x0003B6C8 File Offset: 0x000398C8
		public virtual void Reset()
		{
			this._allButtons.Clear();
			this.isFullyVisible = false;
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x0003B6DC File Offset: 0x000398DC
		public void OnMoveCursorWithNullFocus()
		{
			this.navigation.SetNewFocus(this._firstFocus);
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnMoveCursor(Selectable currentFocus, MoveDirection direction)
		{
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0003B6EF File Offset: 0x000398EF
		public void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			if (InputState.DeviceInputTypeRequiresFocus(newInputType))
			{
				this.navigation.SetNewFocus(this._firstFocus);
				return;
			}
			this.navigation.ClearFocus(false);
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x0003B735 File Offset: 0x00039935
		[CompilerGenerated]
		private void <OnOpened>g__OnTweenComplete|18_0()
		{
			this.appScope.Get<IInputState>().BlockAllInput = false;
			this.isFullyVisible = true;
		}

		// Token: 0x04000F52 RID: 3922
		[Dependency]
		protected IScope appScope;

		// Token: 0x04000F53 RID: 3923
		[Dependency]
		protected readonly PopupParent _popupParent;

		// Token: 0x04000F54 RID: 3924
		[Dependency]
		protected MenuNavigation navigation;

		// Token: 0x04000F55 RID: 3925
		[Dependency]
		protected InputState inputState;

		// Token: 0x04000F56 RID: 3926
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04000F57 RID: 3927
		private readonly List<VariableDeviceSelectable> _allButtons = new List<VariableDeviceSelectable>();

		// Token: 0x04000F58 RID: 3928
		[SerializeField]
		private float _tweenDuration;

		// Token: 0x04000F59 RID: 3929
		[SerializeField]
		private VariableDeviceSelectable _firstFocus;

		// Token: 0x04000F5A RID: 3930
		protected DelegateCanvasGroup _delegateCanvasGroup;

		// Token: 0x04000F5B RID: 3931
		private Coroutine _tweenCoroutine;

		// Token: 0x04000F5C RID: 3932
		private readonly List<LocalizedTextUI> _allLocalizedText = new List<LocalizedTextUI>();

		// Token: 0x04000F5D RID: 3933
		protected bool isFullyVisible;
	}
}
