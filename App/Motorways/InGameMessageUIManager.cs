using System;
using System.Collections.Generic;
using Factory;
using Motorways.Views;

namespace Motorways
{
	// Token: 0x0200044B RID: 1099
	public class InGameMessageUIManager : ICreatedInScopeHandler, MainMenuScreen.IObserver
	{
		// Token: 0x06001B44 RID: 6980 RVA: 0x00063E14 File Offset: 0x00062014
		public void DisplayMessage(StandaloneLocString localisedString)
		{
			this.queuedMessages.Add(localisedString);
			if (this._currentMessage == null)
			{
				InGameMessageUIManager.Log.Info("Displaying message " + ((localisedString != null) ? localisedString.ToString() : null), Array.Empty<object>());
				this.DisplayNextQueuedMessage();
				return;
			}
			InGameMessageUIManager.Log.Info("Queueing message " + ((localisedString != null) ? localisedString.ToString() : null), Array.Empty<object>());
			this._currentMessage.SetIcon(true);
			this._currentMessage.ShowDismissIcon();
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x00063EA8 File Offset: 0x000620A8
		private void DisplayNextQueuedMessage()
		{
			if (this._canShowMessages && Diagnostics.Verify(this._currentMessage == null, "The current message isn't null! This may mean we didn't finish the close animation before trying to show a new one.") && Diagnostics.Verify(this.queuedMessages.Count > 0, "We don't have any messages to show! Aborting."))
			{
				this._currentMessage = this._scope.Get<InGameMessage>();
				this._currentMessage.SetMessage(this.queuedMessages[0], new Action(this.RemoveCurrentMessage));
				this._currentMessage.MoveMessage(this._mainMenu.inGameMessageStackStartPosition.position);
				this.queuedMessages.RemoveAt(0);
				this._currentMessage.SetIcon(this.queuedMessages.Count > 0);
			}
		}

		// Token: 0x06001B46 RID: 6982 RVA: 0x00063F69 File Offset: 0x00062169
		public void OnMainMenuTransitionedIn()
		{
			this._canShowMessages = true;
			if (this.queuedMessages.Count > 0)
			{
				this.DisplayNextQueuedMessage();
			}
		}

		// Token: 0x06001B47 RID: 6983 RVA: 0x00063F86 File Offset: 0x00062186
		public void OnMainMenuTransitionOut()
		{
			this._canShowMessages = false;
			InGameMessage currentMessage = this._currentMessage;
			if (currentMessage == null)
			{
				return;
			}
			currentMessage.DismissMessage(this._player.IsSkipTransitionsEnabled);
		}

		// Token: 0x06001B48 RID: 6984 RVA: 0x00063FAA File Offset: 0x000621AA
		public void OnCreatedInScope(IScope scope)
		{
			this._mainMenu.Subscribe(this);
			scope.Get<InGameMessageService>();
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06001B49 RID: 6985 RVA: 0x00063FBF File Offset: 0x000621BF
		public bool HasMessage
		{
			get
			{
				return this._currentMessage != null;
			}
		}

		// Token: 0x06001B4A RID: 6986 RVA: 0x00063FCD File Offset: 0x000621CD
		public void DismissCurrentMessage()
		{
			InGameMessage currentMessage = this._currentMessage;
			if (currentMessage == null)
			{
				return;
			}
			currentMessage.DismissMessage(false);
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x00063FE0 File Offset: 0x000621E0
		private void RemoveCurrentMessage()
		{
			this._currentMessage = null;
			if (this.queuedMessages.Count > 0)
			{
				this.DisplayNextQueuedMessage();
			}
		}

		// Token: 0x040016CE RID: 5838
		[Dependency]
		private MainMenuScreen _mainMenu;

		// Token: 0x040016CF RID: 5839
		[Dependency]
		private Scope _scope;

		// Token: 0x040016D0 RID: 5840
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040016D1 RID: 5841
		private InGameMessage _currentMessage;

		// Token: 0x040016D2 RID: 5842
		private List<StandaloneLocString> queuedMessages = new List<StandaloneLocString>();

		// Token: 0x040016D3 RID: 5843
		private bool _canShowMessages;

		// Token: 0x040016D4 RID: 5844
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("InGameMessage");
	}
}
