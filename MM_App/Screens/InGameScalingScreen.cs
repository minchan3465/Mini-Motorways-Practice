using System;
using Factory;
using Motorways;
using Server;

namespace Screens
{
	// Token: 0x02000298 RID: 664
	public class InGameScalingScreen : BaseScalingScreen
	{
		// Token: 0x06001081 RID: 4225 RVA: 0x0003806E File Offset: 0x0003626E
		protected virtual MapDefinition GetMapDefinition()
		{
			return this._game.MapDefinition;
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0003807B File Offset: 0x0003627B
		public virtual void InitScreen(IScope gameScope, bool blocksGameInput)
		{
			this._gameScope = gameScope;
			this._game = (this._gameScope.Get<Game>() as MotorwaysGame);
			this._simulation = this._gameScope.Get<ISimulation>();
			this._blocksGameInput = blocksGameInput;
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x000380B2 File Offset: 0x000362B2
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._appScope.Get<InputState>().BlockGameInput = this._blocksGameInput;
			if (this._blocksGameInput)
			{
				this._playerActionController.CancelAllActions();
			}
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x000380E3 File Offset: 0x000362E3
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			this._game.SetPaused(true);
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x000380F8 File Offset: 0x000362F8
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			if (this._canvas != null && base.gameObject.layer == this._gameCamera.OverlayLayerIndex)
			{
				this._gameCamera.AttachCameraToCanvas(this._canvas, CameraLayer.Overlay);
			}
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x00038144 File Offset: 0x00036344
		public override void Reset()
		{
			base.Reset();
			this._blocksGameInput = false;
		}

		// Token: 0x04000E86 RID: 3718
		[Dependency]
		protected PlayerActionController _playerActionController;

		// Token: 0x04000E87 RID: 3719
		protected IScope _gameScope;

		// Token: 0x04000E88 RID: 3720
		protected MotorwaysGame _game;

		// Token: 0x04000E89 RID: 3721
		protected ISimulation _simulation;

		// Token: 0x04000E8A RID: 3722
		protected bool _blocksGameInput;
	}
}
