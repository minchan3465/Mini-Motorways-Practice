using System;
using Factory;

namespace Motorways.Actions
{
	// Token: 0x02000713 RID: 1811
	public class RemoteEditMenuNavigateAction : EditMenuNavigateAction
	{
		// Token: 0x060031CB RID: 12747 RVA: 0x000EBC7D File Offset: 0x000E9E7D
		public static RemoteEditMenuNavigateAction Create(PlayerActionGroup playerActionGroup, IScope scope, float timestamp)
		{
			RemoteEditMenuNavigateAction remoteEditMenuNavigateAction = scope.Get<RemoteEditMenuNavigateAction>();
			remoteEditMenuNavigateAction.InitializeAction(playerActionGroup, timestamp);
			remoteEditMenuNavigateAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Remote, 2, InputEventButtonState.DoubleTapDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			remoteEditMenuNavigateAction.OnActionBegin(timestamp);
			return remoteEditMenuNavigateAction;
		}
	}
}
