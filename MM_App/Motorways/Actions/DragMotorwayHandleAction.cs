using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000707 RID: 1799
	public class DragMotorwayHandleAction : MotorwaysPlayerAction
	{
		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x0600315B RID: 12635 RVA: 0x000E94EE File Offset: 0x000E76EE
		public float Attenuation
		{
			get
			{
				return this._gameCamera.GetAttenuationFromWorld(this._motorwayView.RawHandlePosition, true, 5f);
			}
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x0600315C RID: 12636 RVA: 0x000E9511 File Offset: 0x000E7711
		public float Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(this._motorwayView.RawHandlePosition).x;
			}
		}

		// Token: 0x0600315D RID: 12637 RVA: 0x000E9534 File Offset: 0x000E7734
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			this._motorwayView = this._tilemapView.GetMotorwayView(this._editedMotorwayId);
			this._motorwayView.IsDraggingHandle = true;
			this._offset = this._motorwayView.RawHandlePosition - base.GetPointerWorldPosition();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateMotorwayEvent(AudioEventType.MotorwayHandlePulled, this._motorwayView, this.Pan, this.Attenuation, 0f));
		}

		// Token: 0x0600315E RID: 12638 RVA: 0x000E95C0 File Offset: 0x000E77C0
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			if (this._editedMotorwayId != -1 && this._motorwayView != null)
			{
				Vector2 pointerWorldCoordinates = base.GetPointerWorldPosition();
				this._motorwayView.RawHandlePosition = pointerWorldCoordinates + this._offset;
			}
		}

		// Token: 0x0600315F RID: 12639 RVA: 0x000E960C File Offset: 0x000E780C
		public override void OnActionComplete()
		{
			base.OnActionComplete();
			this._motorwayView.IsDraggingHandle = false;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateMotorwayEvent(AudioEventType.MotorwayHandleReleased, this._motorwayView, this.Pan, this.Attenuation, this._motorwayView.HandleTension));
		}

		// Token: 0x06003160 RID: 12640 RVA: 0x000E9664 File Offset: 0x000E7864
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			this._motorwayView.IsDraggingHandle = false;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateMotorwayEvent(AudioEventType.MotorwayHandleReleased, this._motorwayView, this.Pan, this.Attenuation, this._motorwayView.HandleTension));
		}

		// Token: 0x06003161 RID: 12641 RVA: 0x000E96B9 File Offset: 0x000E78B9
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			base.ObserveInput(timestamp, inputEvent, overUI);
			this.OnActionComplete();
		}

		// Token: 0x06003162 RID: 12642 RVA: 0x000E96CA File Offset: 0x000E78CA
		public override void Reset()
		{
			base.Reset();
			this._editedMotorwayId = -1;
			this._motorwayView = null;
			this._offset = default(Vector2);
		}

		// Token: 0x06003163 RID: 12643 RVA: 0x000E96EC File Offset: 0x000E78EC
		public static DragMotorwayHandleAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragMotorwayHandleAction newAction = scope.Get<DragMotorwayHandleAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			MotorwaysUIInputEvent uiInputEvent = owningGroup.InstigatingInputEvent as MotorwaysUIInputEvent;
			newAction._editedMotorwayId = uiInputEvent.UIButtonIndex;
			if (uiInputEvent.Source == InputEventSource.Mouse)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			else if (uiInputEvent.Source == InputEventSource.Touch)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.BlockNewTouchUpgradeActions();
			}
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x04002A6D RID: 10861
		protected int _editedMotorwayId = -1;

		// Token: 0x04002A6E RID: 10862
		protected MotorwayView _motorwayView;

		// Token: 0x04002A6F RID: 10863
		protected Vector2 _offset;

		// Token: 0x04002A70 RID: 10864
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A71 RID: 10865
		[Dependency]
		private GameCamera _gameCamera;
	}
}
