using System;
using System.Collections.Generic;
using Motorways.Models;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006DB RID: 1755
	public class Vehicle : Playback
	{
		// Token: 0x0600302A RID: 12330 RVA: 0x000E247F File Offset: 0x000E067F
		public Vehicle(VehicleView view)
		{
			this.View = view;
			this.View.AudioVehicle = this;
		}

		// Token: 0x0600302B RID: 12331 RVA: 0x000E24A6 File Offset: 0x000E06A6
		public override void OnDeactivate()
		{
			this.View.AudioVehicle = null;
		}

		// Token: 0x0600302C RID: 12332 RVA: 0x000E24B4 File Offset: 0x000E06B4
		public override void Update()
		{
			Vehicle audioVehicle = this.View.AudioVehicle;
			if (audioVehicle == null)
			{
				return;
			}
			Vehicle.AudioMotor motor = audioVehicle.Motor;
			if (motor == null)
			{
				return;
			}
			motor.OnGameTick();
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x000E24D5 File Offset: 0x000E06D5
		protected override void OnPulse()
		{
			this.PrepHorn();
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x000E24DD File Offset: 0x000E06DD
		public float DistanceAlpha()
		{
			return 1f - Mathf.Clamp(this.View.DistanceToGoal / this.LegDistance, 0f, 1f);
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x000E2508 File Offset: 0x000E0708
		public void PrepHorn()
		{
			float speedThreshold = 0.01f;
			float minDistanceFromHome = 0.02f;
			float maxDistanceFromHome = 0.95f;
			int waitThreshold = 8;
			float baseGenerosity = 0.8f;
			bool flag = this.Location == LocationType.Home || this.Location == LocationType.Carpark;
			bool tooFast = this.View.Speed > speedThreshold;
			float distanceAlpha = this.DistanceAlpha();
			bool tooCloseToEndpoint = distanceAlpha > maxDistanceFromHome || distanceAlpha < minDistanceFromHome;
			bool gamePaused = Get.Game.Simulation.IsPaused;
			bool inTunnel = this.View.IsInTunnel;
			VehicleModel.Frame vehicleFrame = null;
			if (this.View != null && this.View.Model != null)
			{
				vehicleFrame = this.View.Model.CurrentFrame;
			}
			bool noLeadingVehicle = ((vehicleFrame != null) ? vehicleFrame.leadingVehicle : null) == null;
			bool tailGating = !noLeadingVehicle && (float)vehicleFrame.distanceToLeadingVehicle < 0.6f;
			object obj;
			if (vehicleFrame == null)
			{
				obj = null;
			}
			else
			{
				LaneModel blockingLane = vehicleFrame.blockingLane;
				if (blockingLane == null)
				{
					obj = null;
				}
				else
				{
					RoadChunkModel roadChunk = blockingLane.roadChunk;
					obj = ((roadChunk != null) ? roadChunk.TrafficLight : null);
				}
			}
			this.timeAtRedLight = ((obj != null) ? (this.timeAtRedLight + this.Module.Pulse.PulseInfo.PulseDuration) : 0.0);
			bool generous = this.timeAtRedLight < (double)waitThreshold && !tailGating && Rando.FlipCoin(baseGenerosity);
			if (flag || tooFast || tooCloseToEndpoint || noLeadingVehicle || generous || gamePaused || inTunnel)
			{
				return;
			}
			if (this.time > this._lastHonkTime + (double)waitThreshold)
			{
				this.Honk();
			}
		}

		// Token: 0x06003030 RID: 12336 RVA: 0x000E268C File Offset: 0x000E088C
		public void Honk()
		{
			if (this.View != null && this.View.groupIndex >= 0 && this.View.groupIndex < Get.Loadout.DestinationGroups.Count)
			{
				List<string> notes = Get.Loadout.DestinationGroups[this.View.groupIndex].Notes;
				if (Diagnostics.Verify(notes.Count > 0, string.Format("No notes available for honk on Group {0}. Destination Groups: {1}", this.View.groupIndex, Get.Loadout.DestinationGroups.Count)))
				{
					string note = notes[UnityEngine.Random.Range(0, notes.Count - 1)];
					note = note.Substring(0, note.Length - 1);
					AudioPlayer @default = AudioPlayer.Default;
					string sampleName = "Horn-" + note + "-" + (Rando.FlipCoin(0.995f) ? Rando.Pick<string>(new string[]
					{
						"01",
						"02",
						"03",
						"04",
						"05"
					}) : Get.Loadout.MusicData.EasterEggHorn);
					float pitch = Tune.centsToFreqRatio(UnityEngine.Random.Range(-50, 50));
					@default.PlaySample(sampleName, this.View.Pan.x, 0.11f * this.View.Attenuation, pitch, 0.0, this.time, false, null, false, false, 0f, false);
					this._lastHonkTime = this.time;
				}
			}
		}

		// Token: 0x06003031 RID: 12337 RVA: 0x000E2824 File Offset: 0x000E0A24
		public override void AddEventListeners()
		{
			AudioEventFilter f = new AudioEventFilter(this.EventTypes);
			f.Vehicle = this.View;
			this.EventListener.Add(new Action<AudioEvent>(this.OnVehicleEvents), f);
			this.EventListener.Add(new Action<AudioEvent>(this.OnGameOver), AudioEventType.GameOver, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnAudioMinimized), AudioEventType.AudioMinimized, -1);
		}

		// Token: 0x06003032 RID: 12338 RVA: 0x000E28A4 File Offset: 0x000E0AA4
		private void OnGameOver(AudioEvent e)
		{
			foreach (List<VehicleView> list in this.Environment.Vehicles)
			{
				foreach (VehicleView vehicleView in list)
				{
					if (vehicleView != null)
					{
						Vehicle audioVehicle = vehicleView.AudioVehicle;
						if (audioVehicle != null)
						{
							Vehicle.AudioMotor motor = audioVehicle.Motor;
							if (motor != null)
							{
								motor.FadeOutAndStop(2.0);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003033 RID: 12339 RVA: 0x000E2954 File Offset: 0x000E0B54
		private void OnAudioMinimized(AudioEvent e)
		{
			foreach (List<VehicleView> list in this.Environment.Vehicles)
			{
				foreach (VehicleView vehicleView in list)
				{
					if (vehicleView != null)
					{
						Vehicle audioVehicle = vehicleView.AudioVehicle;
						if (audioVehicle != null)
						{
							Vehicle.AudioMotor motor = audioVehicle.Motor;
							if (motor != null)
							{
								motor.FadeOutAndStop(2.0);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003034 RID: 12340 RVA: 0x000E2A04 File Offset: 0x000E0C04
		private void OnVehicleEvents(AudioEvent e)
		{
			AudioEventType type = e.Type;
			if (type <= AudioEventType.VehicleDepartedHouse)
			{
				if (type <= AudioEventType.VehicleArrivedAtHouse)
				{
					if (type != AudioEventType.VehicleArrivedAtDestination)
					{
						if (type != AudioEventType.VehicleArrivedAtHouse)
						{
							return;
						}
						this.Location = LocationType.Home;
						Vehicle.AudioMotor motor = this.Motor;
						if (motor == null)
						{
							return;
						}
						motor.FadeOutAndStop(1.0);
						return;
					}
					else
					{
						Vehicle.AudioMotor motor2 = this.Motor;
						if (motor2 == null)
						{
							return;
						}
						motor2.FadeOutAndStop(1.0);
						return;
					}
				}
				else if (type != AudioEventType.VehicleDepartedDestination)
				{
					if (type != AudioEventType.VehicleDepartedHouse)
					{
						return;
					}
					this.Location = LocationType.Road;
					this.LegDistance = Mathf.Max(0f, (float)this.View.Model.pathLength);
					Vehicle.AudioMotor motor3 = this.Motor;
					if (motor3 != null)
					{
						motor3.FadeOutAndStop(1.0);
					}
					if (this.Motor == null)
					{
						this.Motor = new Vehicle.AudioMotor(e.Vehicle);
						return;
					}
					this.Motor.PlayEngineLoop();
					return;
				}
				else
				{
					this.Location = LocationType.Road;
					Vehicle.AudioMotor motor4 = this.Motor;
					if (motor4 != null)
					{
						motor4.FadeOutAndStop(1.0);
					}
					if (this.Motor == null)
					{
						this.Motor = new Vehicle.AudioMotor(e.Vehicle);
						return;
					}
					this.Motor.PlayEngineLoop();
					return;
				}
			}
			else if (type <= AudioEventType.VehicleLeftMotorway)
			{
				if (type == AudioEventType.VehicleEnteredMotorway)
				{
					this.Location = LocationType.Motorway;
					return;
				}
				if (type != AudioEventType.VehicleLeftMotorway)
				{
					return;
				}
				this.Location = LocationType.Road;
				return;
			}
			else
			{
				if (type == AudioEventType.VehicleEnteredCarpark)
				{
					this.Location = LocationType.Carpark;
					return;
				}
				if (type != AudioEventType.VehicleReceivesPin)
				{
					return;
				}
				if (e.GroupIndex > Get.Loadout.DestinationGroups.Count - 1)
				{
					Dbug.Log.Warn("VehicleReceivesPin: event group index is greater than our DestGroup count. Skipping ...", Array.Empty<object>());
					return;
				}
				DestinationGroup d = Get.Loadout.GetDestinationGroup(e.GroupIndex);
				if (d.Notes.Count < 1)
				{
					Dbug.Log.Warn("VehicleReceivesPin: Notes have not yet been generated for this DestinationGroup.", Array.Empty<object>());
					return;
				}
				string note = d.Notes.SafeGet(d.Note_i);
				AudioPlayer @default = AudioPlayer.Default;
				string sampleName = "PeepEmbarks_" + note;
				float x = e.Vehicle.Pan.x;
				double dspTime = Get.Pulse.HybridTime(d.Module);
				@default.PlaySample(sampleName, x, Note.GainFactor(note) * 0.18f * e.Vehicle.Attenuation, 4f, 0.0, dspTime, false, null, false, false, 0f, false);
				AudioPlayer.Default.PlaySample("PeepEmbarks_" + note, e.Vehicle.Pan.x, Note.GainFactor(note) * 0.01f * e.Vehicle.Attenuation, -4f, 0.1, -1.0, false, null, false, false, 0.94f, false);
				return;
			}
		}

		// Token: 0x04002997 RID: 10647
		public VehicleView View;

		// Token: 0x04002998 RID: 10648
		public Vehicle.AudioMotor Motor;

		// Token: 0x04002999 RID: 10649
		public LocationType Location;

		// Token: 0x0400299A RID: 10650
		public float LegDistance;

		// Token: 0x0400299B RID: 10651
		private double _lastHonkTime;

		// Token: 0x0400299C RID: 10652
		private double timeAtRedLight;

		// Token: 0x0400299D RID: 10653
		public AudioEventType EventTypes = (AudioEventType)((ulong)-2146172676);

		// Token: 0x020006DC RID: 1756
		public class AudioMotor : FX.Modulator
		{
			// Token: 0x17000810 RID: 2064
			// (get) Token: 0x06003035 RID: 12341 RVA: 0x000E2CB6 File Offset: 0x000E0EB6
			// (set) Token: 0x06003036 RID: 12342 RVA: 0x000E2CBE File Offset: 0x000E0EBE
			public VehicleView Vehicle { get; set; }

			// Token: 0x17000811 RID: 2065
			// (get) Token: 0x06003037 RID: 12343 RVA: 0x000E2CC7 File Offset: 0x000E0EC7
			// (set) Token: 0x06003038 RID: 12344 RVA: 0x000E2CCF File Offset: 0x000E0ECF
			public HouseView House { get; private set; }

			// Token: 0x17000812 RID: 2066
			// (get) Token: 0x06003039 RID: 12345 RVA: 0x000E2CD8 File Offset: 0x000E0ED8
			// (set) Token: 0x0600303A RID: 12346 RVA: 0x000E2CE0 File Offset: 0x000E0EE0
			public DestinationView Destination { get; private set; }

			// Token: 0x17000813 RID: 2067
			// (get) Token: 0x0600303B RID: 12347 RVA: 0x000E2CE9 File Offset: 0x000E0EE9
			// (set) Token: 0x0600303C RID: 12348 RVA: 0x000E2CF1 File Offset: 0x000E0EF1
			public AudioSample Sample { get; private set; }

			// Token: 0x0600303D RID: 12349 RVA: 0x000E2CFC File Offset: 0x000E0EFC
			public AudioMotor(VehicleView v) : base(null, null, null)
			{
				this.Vehicle = v;
				this.House = v.House;
				this.Destination = v.Destination;
				List<MusicData.EngineData> groupEngines = Get.Loadout.MusicData.GroupEngines;
				this._engineData = groupEngines[Mathf.Clamp(this.Vehicle.groupIndex, 0, groupEngines.Count - 1)];
				this.PitchAtFullSpeed = (double)((float)Rando.Pick<int>(new int[]
				{
					-1,
					1
				}) * UnityEngine.Random.Range(this._engineData.PitchRange.x, this._engineData.PitchRange.y));
				this.Trem = new FX.Modulator.Tremolo(Rando.Range(1.0, 20.0, -1), UnityEngine.Random.Range(0f, 0.2f), 0.0);
				this.PlayEngineLoop();
			}

			// Token: 0x0600303E RID: 12350 RVA: 0x000E2E38 File Offset: 0x000E1038
			public override void OnGameTick()
			{
				AudioSample sample = this.Sample;
				if (((sample != null) ? sample.DynamicMix : null) != null)
				{
					this.Trem.Frequency = Maf.Lerp(this.Trem.FrequencyAtStart, 20.0, this.NormSpeed(2.0));
					this.Trem.Amplitude = Maf.Lerp(this.Trem.AmplitudeAtStart, 1.0, (1.0 - this.NormSpeed(3.0)) * 0.5);
				}
				this._gainPause.Target = (Get.Game.Simulation.IsPaused ? this._gainPause.Min : this._gainPause.Max);
				this._gainDeleteMode.Target = (Get.State.Contains(StateType.ModeDelete) ? this._gainDeleteMode.Min : this._gainDeleteMode.Max);
				this._gainInTunnel.Target = (this.Vehicle.IsInTunnel ? this._gainInTunnel.Min : this._gainInTunnel.Max);
			}

			// Token: 0x0600303F RID: 12351 RVA: 0x000E2F6B File Offset: 0x000E116B
			public override void Update(double deltaDspTime)
			{
				this._gainPause.Interp(deltaDspTime);
				this._gainDeleteMode.Interp(deltaDspTime);
				this._gainInTunnel.Interp(deltaDspTime);
			}

			// Token: 0x17000814 RID: 2068
			// (get) Token: 0x06003040 RID: 12352 RVA: 0x000E2F91 File Offset: 0x000E1191
			public override double Pitch
			{
				get
				{
					return Maf.Lerp(this.PitchAtFullSpeed * 0.1, this.PitchAtFullSpeed, this.NormSpeed(3.0));
				}
			}

			// Token: 0x17000815 RID: 2069
			// (get) Token: 0x06003041 RID: 12353 RVA: 0x000E2FC0 File Offset: 0x000E11C0
			public override float Gain
			{
				get
				{
					return base.Gain * this._engineData.Gain * this._gainPause.Value * this._gainDeleteMode.Value * this._gainInTunnel.Value * this.Vehicle.Attenuation * Twerp.Ease.In(Mathf.Clamp((float)this.NormSpeed(3.0), 0f, 0.6666667f), 2);
				}
			}

			// Token: 0x17000816 RID: 2070
			// (get) Token: 0x06003042 RID: 12354 RVA: 0x000E3038 File Offset: 0x000E1238
			public override float Pan
			{
				get
				{
					return this.Vehicle.Pan[0];
				}
			}

			// Token: 0x06003043 RID: 12355 RVA: 0x000E3059 File Offset: 0x000E1259
			public void FadeOutAndStop(double duration = 1.0)
			{
				if (this.Sample != null)
				{
					this.Sample.FadeOutAndStop(duration);
					this.Sample.DynamicMix = null;
					this.Sample = null;
				}
			}

			// Token: 0x06003044 RID: 12356 RVA: 0x000E3082 File Offset: 0x000E1282
			public double NormSpeed(double maxSpeed = 3.0)
			{
				return Math.Min((double)this.Vehicle.Speed / maxSpeed, 1.0);
			}

			// Token: 0x06003045 RID: 12357 RVA: 0x000E30A0 File Offset: 0x000E12A0
			public void PlayEngineLoop()
			{
				this.Sample = AudioPlayer.UI.PlaySample(this._engineData.Sample, this.Vehicle.Pan.x, 0.2f, (float)this.PitchAtFullSpeed, 1.5, -1.0, true, this, false, true, 0f, false);
			}

			// Token: 0x040029A2 RID: 10658
			private MusicData.EngineData _engineData;

			// Token: 0x040029A3 RID: 10659
			public double PitchAtFullSpeed;

			// Token: 0x040029A4 RID: 10660
			private Vehicle.AudioMotor.Interpolator _gainPause = new Vehicle.AudioMotor.Interpolator(0f, 1f, 2f);

			// Token: 0x040029A5 RID: 10661
			private Vehicle.AudioMotor.Interpolator _gainDeleteMode = new Vehicle.AudioMotor.Interpolator(0.25f, 1f, 1f);

			// Token: 0x040029A6 RID: 10662
			private Vehicle.AudioMotor.Interpolator _gainInTunnel = new Vehicle.AudioMotor.Interpolator(0.1f, 1f, 0.33f);

			// Token: 0x020006DD RID: 1757
			private struct Interpolator
			{
				// Token: 0x06003046 RID: 12358 RVA: 0x000E3100 File Offset: 0x000E1300
				public Interpolator(float min, float max, float duration)
				{
					this.Min = min;
					this.Max = max;
					this.Duration = duration;
					this.Value = (this.Target = 1f);
				}

				// Token: 0x06003047 RID: 12359 RVA: 0x000E3136 File Offset: 0x000E1336
				public void Interp(double deltaTime)
				{
					this.Value = Mathf.MoveTowards(this.Value, this.Target, (this.Max - this.Min) / (this.Duration / (float)deltaTime));
				}

				// Token: 0x040029A7 RID: 10663
				public float Value;

				// Token: 0x040029A8 RID: 10664
				public float Target;

				// Token: 0x040029A9 RID: 10665
				public float Min;

				// Token: 0x040029AA RID: 10666
				public float Max;

				// Token: 0x040029AB RID: 10667
				public float Duration;
			}
		}
	}
}
