using System;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000693 RID: 1683
	public class PulsedAudioModule : AGATPulseClient, IAudioModule
	{
		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x06002EB5 RID: 11957 RVA: 0x000D9089 File Offset: 0x000D7289
		// (set) Token: 0x06002EB6 RID: 11958 RVA: 0x000D9091 File Offset: 0x000D7291
		public Playback Playback { get; set; }

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x06002EB7 RID: 11959 RVA: 0x000D909A File Offset: 0x000D729A
		public double NextPulseTime
		{
			get
			{
				return base.Pulse.PulseInfo.PulseDspTime + base.Pulse.PulseInfo.PulseDuration;
			}
		}

		// Token: 0x06002EB8 RID: 11960 RVA: 0x000D90C0 File Offset: 0x000D72C0
		public void Activate(AudioEnvironment environment)
		{
			if (!Diagnostics.Verify(!this.isActive, "Cannot reactivate {0}.", this))
			{
				return;
			}
			this.isActive = true;
			base.SubscribeToPulseIfNeeded();
			Playback playback = this.Playback;
			if (playback != null)
			{
				playback.Activate(environment);
			}
			Playback playback2 = this.Playback;
			if (playback2 == null)
			{
				return;
			}
			playback2.OnActivate();
		}

		// Token: 0x06002EB9 RID: 11961 RVA: 0x000D9114 File Offset: 0x000D7314
		public void Deactivate()
		{
			if (!Diagnostics.Verify(this.isActive, "Cannot deactivate {0}.", this))
			{
				return;
			}
			this.isActive = false;
			base.UnsubscribeToPulse();
			Playback playback = this.Playback;
			if (playback != null)
			{
				playback.Deactivate();
			}
			Playback playback2 = this.Playback;
			if (playback2 == null)
			{
				return;
			}
			playback2.OnDeactivate();
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x000D9163 File Offset: 0x000D7363
		public void Release()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06002EBB RID: 11963 RVA: 0x000D9170 File Offset: 0x000D7370
		public void UpdateModule()
		{
			Playback playback = this.Playback;
			if (playback == null)
			{
				return;
			}
			playback.Update();
		}

		// Token: 0x06002EBC RID: 11964 RVA: 0x000D9182 File Offset: 0x000D7382
		public override void OnPulse(IGATPulseInfo pulseInfo)
		{
			if (!this._subscribedSteps[pulseInfo.StepIndex])
			{
				return;
			}
			Playback playback = this.Playback;
			if (playback != null)
			{
				playback.OnGATPulse(pulseInfo, this.lastPulseTime);
			}
			this.lastPulseTime = AudioSystem.Instance.DspTime;
		}

		// Token: 0x06002EBD RID: 11965 RVA: 0x000D91BC File Offset: 0x000D73BC
		protected override bool CanSubscribeToPulse()
		{
			return base.CanSubscribeToPulse() && this.isActive;
		}

		// Token: 0x06002EBE RID: 11966 RVA: 0x000D91D0 File Offset: 0x000D73D0
		public void ChangePulse(Rhythm newRhythm)
		{
			if (this.Rhythm != null && newRhythm.Id == this.Rhythm.Id)
			{
				return;
			}
			base.Pulse = AudioSystem.Instance.Database.GetHyperPulse(newRhythm);
			this.Rhythm = newRhythm;
			base.gameObject.name = base.gameObject.name.Split('|', StringSplitOptions.None)[0] + " " + this.Rhythm.Id;
			base.SubscribeToPulseIfNeeded();
			((SubPulseModule)base.Pulse).PrepOffset(false);
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x000D9267 File Offset: 0x000D7467
		public void ChangePulse(int pulseStep)
		{
			base.Pulse = AudioSystem.Instance.Database.GetPulse(pulseStep, "");
			base.SubscribeToPulseIfNeeded();
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x000D928C File Offset: 0x000D748C
		public static IAudioModule CreateModule(string id, Playback playback, Rhythm rhythm = null, int pulseStep = -1)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = Get.Loadout.GameObject.transform;
			PulsedAudioModule module = gameObject.AddComponent<PulsedAudioModule>();
			module.Playback = playback;
			gameObject.name = "Playback: " + ((!string.IsNullOrEmpty(id)) ? id : "");
			if (rhythm != null)
			{
				GameObject gameObject2 = gameObject;
				gameObject2.name = gameObject2.name + " | " + rhythm.Id;
				module.Pulse = AudioSystem.Instance.Database.GetHyperPulse(rhythm);
			}
			else
			{
				if (pulseStep <= 0)
				{
					return null;
				}
				module.Pulse = AudioSystem.Instance.Database.GetPulse(pulseStep, "");
			}
			module.Rhythm = rhythm;
			module.Playback.Module = module;
			module.Playback.OnBeginPulse();
			return module;
		}

		// Token: 0x04002882 RID: 10370
		public bool isActive;

		// Token: 0x04002884 RID: 10372
		private double lastPulseTime;

		// Token: 0x04002885 RID: 10373
		public Rhythm Rhythm;
	}
}
