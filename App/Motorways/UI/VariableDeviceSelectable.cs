using System;
using System.Collections.Generic;
using Factory;
using Motorways.Audio;
using Motorways.UI.NewContentIndicators;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200074D RID: 1869
	public class VariableDeviceSelectable : Selectable, InputState.IObserver, ISubmitHandler, IEventSystemHandler
	{
		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x0600342C RID: 13356 RVA: 0x000F5EA4 File Offset: 0x000F40A4
		public virtual string NewContentId
		{
			get
			{
				return this._newContentId;
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x0600342D RID: 13357 RVA: 0x000F5EAC File Offset: 0x000F40AC
		// (set) Token: 0x0600342E RID: 13358 RVA: 0x000F5EB4 File Offset: 0x000F40B4
		private protected virtual bool BypassNewContentData { protected get; private set; }

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x0600342F RID: 13359 RVA: 0x000F5EBD File Offset: 0x000F40BD
		// (set) Token: 0x06003430 RID: 13360 RVA: 0x000F5EC5 File Offset: 0x000F40C5
		public virtual bool IsManuallyTriggered { get; private set; }

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x06003431 RID: 13361 RVA: 0x000F5ECE File Offset: 0x000F40CE
		public List<string> ContainedNewContentIds
		{
			get
			{
				return this._containedNewContentIds;
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x06003432 RID: 13362 RVA: 0x000F5ED6 File Offset: 0x000F40D6
		// (set) Token: 0x06003433 RID: 13363 RVA: 0x000F5EDE File Offset: 0x000F40DE
		public DeviceInputType DeviceInputType { get; private set; }

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x06003434 RID: 13364 RVA: 0x000F5EE7 File Offset: 0x000F40E7
		// (set) Token: 0x06003435 RID: 13365 RVA: 0x000F5EEF File Offset: 0x000F40EF
		public bool IsInitialized { get; private set; }

		// Token: 0x06003436 RID: 13366 RVA: 0x000F5EF8 File Offset: 0x000F40F8
		public void SetNewContentID(string newContentId, bool bypassNewContent = false, bool isManuallyTriggered = false)
		{
			this._newContentId = newContentId;
			this.BypassNewContentData = bypassNewContent;
			this.IsManuallyTriggered = isManuallyTriggered;
		}

		// Token: 0x06003437 RID: 13367 RVA: 0x000F5F10 File Offset: 0x000F4110
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (this._newContentIndicator != null && !string.IsNullOrWhiteSpace(this.NewContentId) && !this._dontUpdateContentIDOnPointer)
			{
				this.SetNewContentSeen(this._scope);
				if (!this.IsNewContent(this._scope))
				{
					this.PlayNewContentIndicatorExit();
				}
			}
		}

		// Token: 0x06003438 RID: 13368 RVA: 0x000F5F68 File Offset: 0x000F4168
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (this._newContentIndicator != null && !string.IsNullOrWhiteSpace(this.NewContentId))
			{
				this.SetNewContentSeen(this._scope);
				if (!this.IsNewContent(this._scope))
				{
					this.PlayNewContentIndicatorExit();
				}
			}
		}

		// Token: 0x06003439 RID: 13369 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void DoPressedAnimation()
		{
		}

		// Token: 0x0600343A RID: 13370 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnActivate()
		{
		}

		// Token: 0x0600343B RID: 13371 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnSubmit(BaseEventData eventData)
		{
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x000F5FB8 File Offset: 0x000F41B8
		public void Initialize(IScope scope)
		{
			this._scope = scope;
			this.menuNavigation = scope.Get<MenuNavigation>();
			this.IsInitialized = true;
			this._inputState = scope.Get<InputState>();
			this._inputState.Subscribe(this);
			this.RegisterWithDeviceInputType(this._inputState.CurrentDeviceInputType);
			this._audioSystem = scope.Get<IAudioSystem>();
			this._feedbackGenerator = scope.Get<HapticFeedbackGenerator>();
		}

		// Token: 0x0600343D RID: 13373 RVA: 0x000F6020 File Offset: 0x000F4220
		public void RegisterWithDeviceInputType(DeviceInputType newInputType)
		{
			this.DeviceInputType = newInputType;
		}

		// Token: 0x0600343E RID: 13374 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Unregister()
		{
		}

		// Token: 0x0600343F RID: 13375 RVA: 0x000F6029 File Offset: 0x000F4229
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._newContentIndicator != null && !this.IsNewContent(this._scope))
			{
				UnityEngine.Object.Destroy(this._newContentIndicator.gameObject);
				this._newContentIndicator = null;
			}
		}

		// Token: 0x06003440 RID: 13376 RVA: 0x000F6064 File Offset: 0x000F4264
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this._inputState != null)
			{
				this._inputState.Unsubscribe(this);
			}
		}

		// Token: 0x06003441 RID: 13377 RVA: 0x000F6081 File Offset: 0x000F4281
		public void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			this.Unregister();
			this.RegisterWithDeviceInputType(newInputType);
		}

		// Token: 0x06003442 RID: 13378 RVA: 0x000F6090 File Offset: 0x000F4290
		public bool ShowNewContentIndicatorIfNeeded(bool playIntro)
		{
			return this.IsNewContent(this._scope) && this.ShowNewContentIndicator(playIntro);
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x000F60A9 File Offset: 0x000F42A9
		protected void SetNewContentSeen(IScope appScope)
		{
			if (!string.IsNullOrWhiteSpace(this.NewContentId))
			{
				appScope.Get<NewContentData>().SetNewContentSeen(this.NewContentId);
			}
		}

		// Token: 0x06003444 RID: 13380 RVA: 0x000F60CC File Offset: 0x000F42CC
		public virtual bool IsNewContentItem(IScope appScope)
		{
			if (string.IsNullOrWhiteSpace(this.NewContentId))
			{
				return false;
			}
			NewContentData newContentData = appScope.Get<NewContentData>();
			if (!string.IsNullOrEmpty(this.NewContentId) && newContentData.IsNewContent(this.NewContentId, this.BypassNewContentData))
			{
				return true;
			}
			this._newContentId = null;
			return false;
		}

		// Token: 0x06003445 RID: 13381 RVA: 0x000F611C File Offset: 0x000F431C
		public bool IsNewContentContainer(IScope appScope)
		{
			NewContentData newContentData = appScope.Get<NewContentData>();
			foreach (string containedNewContentId in this._containedNewContentIds)
			{
				if (!string.IsNullOrWhiteSpace(containedNewContentId) && newContentData.IsNewContent(containedNewContentId, false))
				{
					if (!this._hasSubscribedToNewContentSeenEvent)
					{
						newContentData.onNewContentSeen += this.OnNewContentSeen;
						this._hasSubscribedToNewContentSeenEvent = true;
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003446 RID: 13382 RVA: 0x000F61AC File Offset: 0x000F43AC
		protected bool IsNewContent(IScope appScope)
		{
			return this.IsNewContentItem(appScope) || this.IsNewContentContainer(appScope);
		}

		// Token: 0x06003447 RID: 13383 RVA: 0x000F61C0 File Offset: 0x000F43C0
		private void OnNewContentSeen(string newContentId)
		{
			if (!this.IsNewContent(this._scope))
			{
				this.PlayNewContentIndicatorExit();
				this._scope.Get<NewContentData>().onNewContentSeen -= this.OnNewContentSeen;
				this._hasSubscribedToNewContentSeenEvent = false;
			}
		}

		// Token: 0x06003448 RID: 13384 RVA: 0x000F61FC File Offset: 0x000F43FC
		private bool InitNewContentIndicatorIfNeeded()
		{
			if (this._newContentIndicator == null)
			{
				this._newContentIndicator = this._scope.Get<NewContentIndicator>();
				this._newContentIndicator.transform.SetParent(this._newContentIndicatorParent, false);
				return true;
			}
			return this._newContentIndicator.IsHidden;
		}

		// Token: 0x06003449 RID: 13385 RVA: 0x000F624C File Offset: 0x000F444C
		private bool ShowNewContentIndicator(bool playIntro)
		{
			if (this.InitNewContentIndicatorIfNeeded() && playIntro)
			{
				this._newContentIndicator.PlayIntro();
				return true;
			}
			this._newContentIndicator.PlayIdle();
			return false;
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x000F6271 File Offset: 0x000F4471
		protected void PlayNewContentIndicatorExit()
		{
			if (this._newContentIndicator != null)
			{
				this._newContentIndicator.PlayExit();
			}
		}

		// Token: 0x0600344B RID: 13387 RVA: 0x000F628C File Offset: 0x000F448C
		public new Selectable FindSelectable(Vector3 desiredDirection)
		{
			desiredDirection = desiredDirection.normalized;
			Vector2 ourNavigationPoint = base.transform.position;
			float bestNavigationMetric = float.NegativeInfinity;
			Selectable bestSelectable = null;
			for (int selectableIndex = 0; selectableIndex < Selectable.allSelectablesArray.Length; selectableIndex++)
			{
				Selectable otherSelectable = Selectable.allSelectablesArray[selectableIndex];
				if (otherSelectable != this && otherSelectable != null && otherSelectable.IsInteractable() && otherSelectable.navigation.mode != Navigation.Mode.None)
				{
					Vector2 otherNavigationPoint = otherSelectable.transform.position;
					Vector2 directionToOtherSelectable = otherNavigationPoint - ourNavigationPoint;
					float dotToOtherSelectable = Vector3.Dot(desiredDirection, directionToOtherSelectable);
					Debug.DrawLine(ourNavigationPoint, otherNavigationPoint, Color.blue, 1f);
					if (dotToOtherSelectable > 0f)
					{
						float navigationMetric = dotToOtherSelectable / directionToOtherSelectable.sqrMagnitude;
						if (navigationMetric > bestNavigationMetric)
						{
							bestNavigationMetric = navigationMetric;
							bestSelectable = otherSelectable;
						}
					}
				}
			}
			return bestSelectable;
		}

		// Token: 0x0600344C RID: 13388 RVA: 0x000F637C File Offset: 0x000F457C
		public override Selectable FindSelectableOnLeft()
		{
			if (base.navigation.mode == Navigation.Mode.Explicit)
			{
				return base.navigation.selectOnLeft;
			}
			if ((base.navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
			{
				return this.FindSelectable(base.transform.rotation * Vector3.left);
			}
			return null;
		}

		// Token: 0x0600344D RID: 13389 RVA: 0x000F63D8 File Offset: 0x000F45D8
		public override Selectable FindSelectableOnRight()
		{
			if (base.navigation.mode == Navigation.Mode.Explicit)
			{
				return base.navigation.selectOnRight;
			}
			if ((base.navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
			{
				return this.FindSelectable(base.transform.rotation * Vector3.right);
			}
			return null;
		}

		// Token: 0x0600344E RID: 13390 RVA: 0x000F6434 File Offset: 0x000F4634
		public override Selectable FindSelectableOnUp()
		{
			if (base.navigation.mode == Navigation.Mode.Explicit)
			{
				return base.navigation.selectOnUp;
			}
			if ((base.navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
			{
				return this.FindSelectable(base.transform.rotation * Vector3.up);
			}
			return null;
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x000F6490 File Offset: 0x000F4690
		public override Selectable FindSelectableOnDown()
		{
			if (base.navigation.mode == Navigation.Mode.Explicit)
			{
				return base.navigation.selectOnDown;
			}
			if ((base.navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
			{
				return this.FindSelectable(base.transform.rotation * Vector3.down);
			}
			return null;
		}

		// Token: 0x04002C88 RID: 11400
		protected IAudioSystem _audioSystem;

		// Token: 0x04002C89 RID: 11401
		[SerializeField]
		protected UIAudioProfile _audioProfile = UIAudioProfile.Generic;

		// Token: 0x04002C8A RID: 11402
		[SerializeField]
		private Transform _newContentIndicatorParent;

		// Token: 0x04002C8B RID: 11403
		[SerializeField]
		private string _newContentId;

		// Token: 0x04002C8C RID: 11404
		[SerializeField]
		private List<string> _containedNewContentIds;

		// Token: 0x04002C8D RID: 11405
		[SerializeField]
		protected bool _dontUpdateContentIDOnPointer;

		// Token: 0x04002C90 RID: 11408
		protected InputState _inputState;

		// Token: 0x04002C91 RID: 11409
		private NewContentIndicator _newContentIndicator;

		// Token: 0x04002C92 RID: 11410
		private bool _hasSubscribedToNewContentSeenEvent;

		// Token: 0x04002C95 RID: 11413
		protected MenuNavigation menuNavigation;

		// Token: 0x04002C96 RID: 11414
		protected HapticFeedbackGenerator _feedbackGenerator;

		// Token: 0x04002C97 RID: 11415
		private IScope _scope;
	}
}
