using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using Factory;
using UnityEngine;

namespace Popups
{
	// Token: 0x020002DC RID: 732
	public class PopupStack
	{
		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x0003BCC5 File Offset: 0x00039EC5
		public bool HasActivePopups
		{
			get
			{
				return this._popupStack.Count > 0;
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001208 RID: 4616 RVA: 0x0003BCD5 File Offset: 0x00039ED5
		// (set) Token: 0x06001209 RID: 4617 RVA: 0x0003BCDD File Offset: 0x00039EDD
		public bool HasVisiblePopups { get; private set; }

		// Token: 0x0600120A RID: 4618 RVA: 0x0003BCE6 File Offset: 0x00039EE6
		public T PushConfirmationPopup<T>(StringId mainPromptStringId, Action onClosed, StringId additionalInfoStringId) where T : AbstractConfirmationPopup
		{
			T t = this.PushPopup<T>(0f, false);
			t.Initialise(this._appScope, mainPromptStringId, onClosed, additionalInfoStringId);
			return t;
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x0003BD08 File Offset: 0x00039F08
		public T PushConfirmationPopup<T>(StringId mainPromptStringId, Action onNoPressed, Action onYesPressed, StringId additionalInfoStringId) where T : AbstractConfirmationPopup
		{
			T t = this.PushPopup<T>(0f, false);
			t.Initialise(this._appScope, mainPromptStringId, onNoPressed, onYesPressed, additionalInfoStringId);
			return t;
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x0003BD2C File Offset: 0x00039F2C
		public T PushPopup<T>(float delay = 0f, bool ignoreScreen = false) where T : BasePopup
		{
			this.HasVisiblePopups = true;
			T popup = this._appScope.Get<T>();
			float popupDelay = delay;
			if (this._popupStack.Count > 0)
			{
				this._popupStack[this._popupStack.Count - 1].OnLostFocus();
			}
			else
			{
				this._occludedScreen = null;
				if (!ignoreScreen)
				{
					this._occludedScreen = this._screenStack.GetTopVisibleScreen();
				}
				IScreen occludedScreen = this._occludedScreen;
				if (occludedScreen != null)
				{
					occludedScreen.OnLostFocus();
				}
				popupDelay += this._popupParent.FirstPopupDelay;
				this.TweenInOut(true, null, popupDelay);
			}
			List<IThemeComponent> popupThemeComponents = new List<IThemeComponent>();
			popup.GetComponentsInChildren<IThemeComponent>(true, popupThemeComponents);
			IScreen occludedScreen2 = this._occludedScreen;
			if (occludedScreen2 != null)
			{
				occludedScreen2.RegisterAdditionalThemeComponents(popupThemeComponents);
			}
			this._popupStack.Add(popup);
			popup.OnOpened(popupDelay);
			return popup;
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0003BE01 File Offset: 0x0003A001
		public BasePopup GetTopPopup()
		{
			if (Diagnostics.Verify(this.HasActivePopups, "No active popups currently."))
			{
				return this._popupStack[this._popupStack.Count - 1];
			}
			return null;
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x0003BE30 File Offset: 0x0003A030
		public void PopPopup(bool skipTransition = false)
		{
			if (this._popupStack.Count > 0)
			{
				BasePopup poppedPopup = this._popupStack[this._popupStack.Count - 1];
				this._popupStack.RemoveAt(this._popupStack.Count - 1);
				poppedPopup.OnClosed(delegate
				{
					this._appScope.Release(poppedPopup);
					if (this._popupStack.Count > 0)
					{
						this._popupStack[this._popupStack.Count - 1].OnReceivedFocus();
						return;
					}
					IScreen occludedScreen2 = this._occludedScreen;
					if (occludedScreen2 != null)
					{
						occludedScreen2.OnGainedFocus();
					}
					this.HasVisiblePopups = false;
					this.TweenInOut(false, null, 0f);
				}, skipTransition);
				List<IThemeComponent> popupThemeComponents = new List<IThemeComponent>();
				poppedPopup.GetComponentsInChildren<IThemeComponent>(true, popupThemeComponents);
				IScreen occludedScreen = this._occludedScreen;
				if (occludedScreen == null)
				{
					return;
				}
				occludedScreen.UnregisterAdditionalThemeComponents(popupThemeComponents);
			}
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x0003BECA File Offset: 0x0003A0CA
		private void TweenInOut(bool isIn, Action thenExecute = null, float delay = 0f)
		{
			if (this._tweenCoroutine != null)
			{
				this._gameCamera.StopCoroutine(this._tweenCoroutine);
			}
			this._tweenCoroutine = this._gameCamera.StartCoroutine(this.TweenInOutCoroutine(isIn, thenExecute, delay));
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x0003BEFF File Offset: 0x0003A0FF
		private void ResetCachedValues()
		{
			this._blurStrengthBefore = -1f;
			this._blurRangeBefore = -1f;
			this._blurOffsetBefore = -1f;
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x0003BF22 File Offset: 0x0003A122
		public void ResetReturnBlur()
		{
			this._blurStrengthBefore = 0f;
			this._blurRangeBefore = 0f;
			this._blurOffsetBefore = 0f;
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x0003BF45 File Offset: 0x0003A145
		private IEnumerator TweenInOutCoroutine(bool isIn, Action thenExecute, float delay = 0f)
		{
			yield return new WaitForSeconds(delay);
			float time = 0f;
			if (isIn)
			{
				this._blurStrengthBefore = ((this._blurStrengthBefore < 0f) ? this._gameCamera.customBlur.Strength : this._blurStrengthBefore);
				this._blurRangeBefore = ((this._blurRangeBefore < 0f) ? this._gameCamera.customBlur.LevelsRange : this._blurRangeBefore);
				this._blurOffsetBefore = ((this._blurOffsetBefore < 0f) ? this._gameCamera.customBlur.LevelsOffset : this._blurOffsetBefore);
			}
			float startBlurStrength = this._gameCamera.customBlur.Strength;
			float startBlurRange = this._gameCamera.customBlur.LevelsRange;
			float startBlurOffset = this._gameCamera.customBlur.LevelsOffset;
			float targetBlurStrength = isIn ? this._popupParent.FullBlurStrength : this._blurStrengthBefore;
			float targetBlurRange = isIn ? this._popupParent.FullBlurRange : this._blurRangeBefore;
			float targetBlurOffset = isIn ? this._popupParent.FullBlurOffset() : this._blurOffsetBefore;
			while (time < this._popupParent.TweenDuration)
			{
				float progress = time / this._popupParent.TweenDuration;
				float strength = Mathf.Lerp(startBlurStrength, targetBlurStrength, progress);
				float range = Mathf.Lerp(startBlurRange, targetBlurRange, progress);
				float offset = Mathf.Lerp(startBlurOffset, targetBlurOffset, progress);
				this._gameCamera.customBlur.Strength = strength;
				this._gameCamera.customBlur.LevelsRange = range;
				this._gameCamera.customBlur.LevelsOffset = offset;
				time += Time.deltaTime;
				yield return null;
			}
			this._gameCamera.customBlur.Strength = targetBlurStrength;
			this._gameCamera.customBlur.LevelsRange = targetBlurRange;
			this._gameCamera.customBlur.LevelsOffset = targetBlurOffset;
			if (thenExecute != null)
			{
				thenExecute();
			}
			if (!isIn)
			{
				this.ResetCachedValues();
			}
			yield break;
		}

		// Token: 0x04000F97 RID: 3991
		[Dependency]
		private readonly IScope _appScope;

		// Token: 0x04000F98 RID: 3992
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04000F99 RID: 3993
		[Dependency]
		private PopupParent _popupParent;

		// Token: 0x04000F9A RID: 3994
		[Dependency]
		private ScreenStack _screenStack;

		// Token: 0x04000F9B RID: 3995
		private readonly List<BasePopup> _popupStack = new List<BasePopup>();

		// Token: 0x04000F9C RID: 3996
		private IScreen _occludedScreen;

		// Token: 0x04000F9D RID: 3997
		private Coroutine _tweenCoroutine;

		// Token: 0x04000F9E RID: 3998
		private float _blurStrengthBefore = -1f;

		// Token: 0x04000F9F RID: 3999
		private float _blurRangeBefore = -1f;

		// Token: 0x04000FA0 RID: 4000
		private float _blurOffsetBefore = -1f;
	}
}
