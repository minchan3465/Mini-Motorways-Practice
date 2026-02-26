using System;
using Motorways.Models;
using Motorways.Views.Boats;

namespace Motorways.Audio
{
	// Token: 0x020006C2 RID: 1730
	public class Boat : Playback
	{
		// Token: 0x06002FA4 RID: 12196 RVA: 0x000DE3F6 File Offset: 0x000DC5F6
		public Boat(BoatView view)
		{
			this._boatView = view;
			this._boatMotor = new Boat.AudioBoatMotor(this._boatView);
		}

		// Token: 0x06002FA5 RID: 12197 RVA: 0x000DE418 File Offset: 0x000DC618
		public override void Update()
		{
			if (Get.State.HasAny(new StateType[]
			{
				StateType.GameOver
			}) || Get.Game.Simulation.IsPaused)
			{
				this._boatMotor.Stop();
				return;
			}
			BoatModel model = this._boatView.Model;
			Boat.AudioBoatMotor boatMotor = this._boatMotor;
			if (boatMotor != null)
			{
				boatMotor.OnGameTick();
			}
			if (this._lastState != model.state)
			{
				switch (model.state)
				{
				case BoatModel.BehaviorState.Sailing:
				{
					Boat.AudioBoatMotor boatMotor2 = this._boatMotor;
					if (boatMotor2 != null)
					{
						boatMotor2.Start();
					}
					break;
				}
				case BoatModel.BehaviorState.Stopping:
				{
					Boat.AudioBoatMotor boatMotor3 = this._boatMotor;
					if (boatMotor3 != null)
					{
						boatMotor3.Stop();
					}
					break;
				}
				case BoatModel.BehaviorState.Undocking:
				{
					string note = Note.SCALE[Get.Loadout.MusicData.CurrentScale.Key];
					double dspTime = Get.Pulse.QuantizedTime(0.25);
					AudioPlayer.Default.PlaySample("boat-horn-dry-" + note, this._boatView.Pan.x, 0.4f * this._boatView.Attenuation, 1f, 0.0, dspTime, false, null, false, false, 0f, false);
					AudioPlayer.Default.PlaySample("boat-horn-wet-" + note, 0.5f, 0.16f * this._boatView.Attenuation, 1f, 0.0, dspTime, false, null, true, false, 0f, false);
					break;
				}
				}
			}
			this._lastState = model.state;
		}

		// Token: 0x04002915 RID: 10517
		private readonly BoatView _boatView;

		// Token: 0x04002916 RID: 10518
		private BoatModel.BehaviorState _lastState;

		// Token: 0x04002917 RID: 10519
		private AudioSample _engineSample;

		// Token: 0x04002918 RID: 10520
		private readonly Boat.AudioBoatMotor _boatMotor;

		// Token: 0x020006C3 RID: 1731
		public class AudioBoatMotor : FX.Modulator
		{
			// Token: 0x170007FE RID: 2046
			// (get) Token: 0x06002FA6 RID: 12198 RVA: 0x000DE5AB File Offset: 0x000DC7AB
			// (set) Token: 0x06002FA7 RID: 12199 RVA: 0x000DE5B3 File Offset: 0x000DC7B3
			private BoatView BoatView { get; set; }

			// Token: 0x170007FF RID: 2047
			// (get) Token: 0x06002FA8 RID: 12200 RVA: 0x000DE5BC File Offset: 0x000DC7BC
			// (set) Token: 0x06002FA9 RID: 12201 RVA: 0x000DE5C4 File Offset: 0x000DC7C4
			private AudioSample Sample { get; set; }

			// Token: 0x06002FAA RID: 12202 RVA: 0x000DE5CD File Offset: 0x000DC7CD
			public AudioBoatMotor(BoatView v) : base(null, null, null)
			{
				this.BoatView = v;
				this.Start();
			}

			// Token: 0x06002FAB RID: 12203 RVA: 0x000DE5E5 File Offset: 0x000DC7E5
			public override void OnGameTick()
			{
				this._pan = this.BoatView.Pan.x;
				this._attenuation = this.BoatView.Attenuation;
			}

			// Token: 0x17000800 RID: 2048
			// (get) Token: 0x06002FAC RID: 12204 RVA: 0x000DE60E File Offset: 0x000DC80E
			public override float Pan
			{
				get
				{
					return this._pan;
				}
			}

			// Token: 0x17000801 RID: 2049
			// (get) Token: 0x06002FAD RID: 12205 RVA: 0x000DE616 File Offset: 0x000DC816
			public override float Gain
			{
				get
				{
					return 0.25f * this._attenuation;
				}
			}

			// Token: 0x06002FAE RID: 12206 RVA: 0x000DE624 File Offset: 0x000DC824
			public void Stop()
			{
				if (this.Sample == null)
				{
					return;
				}
				this.Sample.FadeOutAndStop((double)((Get.Game.GetTimeScale() == TimeScale.Single) ? 2.25f : 1.5f));
				this.Sample.DynamicMix = null;
				this.Sample = null;
			}

			// Token: 0x06002FAF RID: 12207 RVA: 0x000DE678 File Offset: 0x000DC878
			public void Start()
			{
				this.Sample = AudioPlayer.UI.PlaySample("boat-loop", this.BoatView.Pan.x, 0.5625f, 1f, (double)((Get.Game.GetTimeScale() == TimeScale.Single) ? 2.25f : 1.5f), -1.0, true, this, false, true, 0f, false);
			}

			// Token: 0x0400291B RID: 10523
			private float _pan;

			// Token: 0x0400291C RID: 10524
			private float _attenuation;
		}
	}
}
