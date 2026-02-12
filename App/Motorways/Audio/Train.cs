using System;
using System.Collections.Generic;
using Motorways.Models;
using Motorways.Views.Trains;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006D9 RID: 1753
	public class Train : Playback
	{
		// Token: 0x06003022 RID: 12322 RVA: 0x000E1F4C File Offset: 0x000E014C
		public Train(TrainView view)
		{
			this.Reseed();
		}

		// Token: 0x06003023 RID: 12323 RVA: 0x000E1FF8 File Offset: 0x000E01F8
		public void Reseed()
		{
			this.seed = Rando.Range(1, 9999, -1);
			this.patternLength = ((this.PatternLengthOverride > 0) ? this.PatternLengthOverride : Rando.Range(4, 13, this.seed));
			this.enginePulse = Rando.Range(6, 9, -1);
		}

		// Token: 0x06003024 RID: 12324 RVA: 0x000E204C File Offset: 0x000E024C
		protected override void OnPulse()
		{
			if (Get.State.HasAny(new StateType[]
			{
				StateType.GameOver
			}) || Get.Game.Simulation.IsPaused)
			{
				return;
			}
			foreach (TrainView train in Get.Environment.Trains)
			{
				this.Attenuation = Mathf.Pow(train.Attenuation, 1.5f);
				if (train._state != TrainModel.BehaviorState.Stopped)
				{
					this.counter++;
					if (this.counter > this.patternLength - 1)
					{
						this.counter = 0;
					}
					this.SpeedAlpha = train._speed / 3f;
					if (this.VariablePulseMode)
					{
						int subDiv = (int)(Mathf.Lerp(4f, (float)this.enginePulse, this.SpeedAlpha) / Get.Pulse.Scale.Scale);
						if (subDiv != this._subDiv)
						{
							this.Module.ChangePulse(subDiv);
							this._subDiv = subDiv;
						}
					}
					else
					{
						if (this.prevScale != Get.Pulse.Scale)
						{
							this.Module.ChangePulse((int)(4f / Get.Pulse.Scale.Scale));
						}
						this.prevScale = Get.Pulse.Scale;
						int num = this.seed;
						this.seed = num + 1;
						if (Rando.m(num) > this.SpeedAlpha)
						{
							break;
						}
					}
					if (this.Attenuation > 0f)
					{
						AudioPlayer.Default.PlaySample("PeepAppears_" + this.TrainEngines[Rando.Range(0, this.TrainEngines.Count - 1, this.seed * this.counter)], train.Pan.x, Mathf.Lerp(0.5f, 1f, Maf.VolCurve(this.SpeedAlpha)) * 0.5f * this.Attenuation, Rando.Range(0.5f, 2f, this.seed + this.counter) * Mathf.Lerp(0.5f, 1f, this.SpeedAlpha), 0.0, this.time, false, null, false, false, 0f, false);
						if (Rando.m(this.seed + this.counter) < this.KickDoublingProbability)
						{
							AudioPlayer.Default.PlaySample("perc_kick", train.Pan.x, Mathf.Lerp(0.5f, 1f, Maf.VolCurve(this.SpeedAlpha)) * 0.75f * this.Attenuation, Rando.Range(0.75f, 1.25f, this.seed + this.counter) * Mathf.Lerp(0.5f, 1f, this.SpeedAlpha), 0.0, this.time, false, null, false, false, 0f, false);
						}
					}
				}
				else if (this.trainArrived)
				{
					if (this.Attenuation > 0f)
					{
						AudioPlayer.Default.PlaySample("TrainArrives_" + Rando.Pick<string>(new string[]
						{
							"0",
							"1"
						}), train.Pan.x, 0.075f * this.Attenuation, Rando.Range(0.6f, 0.8f, -1), 0.0, this.time, false, null, false, false, 0f, false);
					}
					this.trainArrived = false;
				}
			}
		}

		// Token: 0x06003025 RID: 12325 RVA: 0x000E23E0 File Offset: 0x000E05E0
		public override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnTrainArrives), new AudioEventFilter(AudioEventType.TrainArrives));
		}

		// Token: 0x06003026 RID: 12326 RVA: 0x000E2407 File Offset: 0x000E0607
		private void OnTrainArrives(AudioEvent e)
		{
			this.trainArrived = true;
			Get.Loadout.MusicData.OnTrainArrived();
		}

		// Token: 0x06003027 RID: 12327 RVA: 0x000022F5 File Offset: 0x000004F5
		private void OnTrainDeparts(AudioEvent e)
		{
		}

		// Token: 0x0400298A RID: 10634
		public List<string> TrainEngines = new List<string>
		{
			"CIRCLE",
			"CROSS",
			"DIAMOND",
			"EGG",
			"PENTAGON",
			"SQUARE",
			"STAR",
			"TRIANGLE"
		};

		// Token: 0x0400298B RID: 10635
		private int seed = -1;

		// Token: 0x0400298C RID: 10636
		private int patternLength;

		// Token: 0x0400298D RID: 10637
		private int enginePulse = 8;

		// Token: 0x0400298E RID: 10638
		private int counter;

		// Token: 0x0400298F RID: 10639
		private bool trainArrived;

		// Token: 0x04002990 RID: 10640
		private int _subDiv = -1;

		// Token: 0x04002991 RID: 10641
		public bool VariablePulseMode = true;

		// Token: 0x04002992 RID: 10642
		public int PatternLengthOverride = -1;

		// Token: 0x04002993 RID: 10643
		public float KickDoublingProbability = 0.25f;

		// Token: 0x04002994 RID: 10644
		public float SpeedAlpha;

		// Token: 0x04002995 RID: 10645
		public float Attenuation;

		// Token: 0x04002996 RID: 10646
		private TimeScale prevScale;
	}
}
