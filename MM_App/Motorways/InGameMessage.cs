using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x0200044A RID: 1098
	public class InGameMessage : MonoBehaviour, IReusable
	{
		// Token: 0x06001B39 RID: 6969 RVA: 0x00063B80 File Offset: 0x00061D80
		public void SetMessage(StandaloneLocString standaloneLocString, Action onDismissed)
		{
			this._onDismissed = onDismissed;
			this._button.Initialize(this._scope);
			this._text.HandleParentAllocated(this._scope);
			this._text.LocString = standaloneLocString;
			base.GetComponentsInChildren<IThemeComponent>(true, this._messageThemeComponents);
			this._mainMenu.RegisterAdditionalThemeComponents(this._messageThemeComponents);
			base.transform.SetParent(this._mainMenu.transform, false);
			base.transform.position = this._mainMenu.inGameMessageStartingPosition.position;
			Canvas.ForceUpdateCanvases();
		}

		// Token: 0x06001B3A RID: 6970 RVA: 0x00063C17 File Offset: 0x00061E17
		public void MoveMessage(Vector3 position)
		{
			this._currentMovementTimer = 0f;
			this._startPosition = base.transform.position;
			this._desiredPosition = position;
		}

		// Token: 0x06001B3B RID: 6971 RVA: 0x00063C3C File Offset: 0x00061E3C
		public void SetIcon(bool hasNextMessage)
		{
			this._dismissIcon.sprite = (hasNextMessage ? this._constants.InGameMessageQueuedIcon : this._constants.InGameMessageDismissIcon);
		}

		// Token: 0x06001B3C RID: 6972 RVA: 0x00063C64 File Offset: 0x00061E64
		private void Update()
		{
			if (this._currentMovementTimer < this._constants.InGameMessageAppearEasingDuration && this._currentMovementTimer >= 0f)
			{
				this._currentMovementTimer += Time.deltaTime;
				base.transform.position = Vector2.Lerp(this._startPosition, this._desiredPosition, Easings.Interpolate(this._currentMovementTimer / this._constants.InGameMessageAppearEasingDuration, this._constants.InGameMessageAppearEasingFunction));
				if (this._currentMovementTimer >= this._constants.InGameMessageAppearEasingDuration)
				{
					base.transform.position = this._desiredPosition;
					this.ShowDismissIcon();
				}
			}
		}

		// Token: 0x06001B3D RID: 6973 RVA: 0x00063D20 File Offset: 0x00061F20
		public void Reset()
		{
			base.transform.localScale = Vector3.one;
			base.transform.localPosition = Vector3.zero;
			this._currentMovementTimer = -1f;
			this._desiredPosition = Vector3.zero;
			this._startPosition = Vector3.zero;
		}

		// Token: 0x06001B3E RID: 6974 RVA: 0x00063D6E File Offset: 0x00061F6E
		public void ShowDismissIcon()
		{
			this._animator.SetTrigger(InGameMessage.DismissAppear);
		}

		// Token: 0x06001B3F RID: 6975 RVA: 0x00063D80 File Offset: 0x00061F80
		public void DismissMessage(bool instantly = false)
		{
			if (instantly)
			{
				this.OnMessageFullyDismissed();
				return;
			}
			this._animator.SetTrigger(InGameMessage.Disappear);
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x00063D9C File Offset: 0x00061F9C
		public void OnMessageTapped()
		{
			this.DismissMessage(false);
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x00063DA5 File Offset: 0x00061FA5
		public void OnMessageFullyDismissed()
		{
			Action onDismissed = this._onDismissed;
			if (onDismissed != null)
			{
				onDismissed();
			}
			this._scope.Release(this);
			this._mainMenu.UnregisterAdditionalThemeComponents(this._messageThemeComponents);
		}

		// Token: 0x040016C0 RID: 5824
		private static readonly int Disappear = Animator.StringToHash("Disappear");

		// Token: 0x040016C1 RID: 5825
		private static readonly int DismissAppear = Animator.StringToHash("DismissAppear");

		// Token: 0x040016C2 RID: 5826
		[Dependency]
		private Scope _scope;

		// Token: 0x040016C3 RID: 5827
		[Dependency]
		private MainMenuScreen _mainMenu;

		// Token: 0x040016C4 RID: 5828
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x040016C5 RID: 5829
		[SerializeField]
		private LocalizedTextUI _text;

		// Token: 0x040016C6 RID: 5830
		[SerializeField]
		private Animator _animator;

		// Token: 0x040016C7 RID: 5831
		[SerializeField]
		private TouchButton _button;

		// Token: 0x040016C8 RID: 5832
		[SerializeField]
		private Image _dismissIcon;

		// Token: 0x040016C9 RID: 5833
		private Action _onDismissed;

		// Token: 0x040016CA RID: 5834
		private Vector3 _desiredPosition;

		// Token: 0x040016CB RID: 5835
		private Vector3 _startPosition;

		// Token: 0x040016CC RID: 5836
		private float _currentMovementTimer = -1f;

		// Token: 0x040016CD RID: 5837
		private List<IThemeComponent> _messageThemeComponents = new List<IThemeComponent>();
	}
}
