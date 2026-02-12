using System;
using System.Collections.Generic;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000637 RID: 1591
	public class AudioDatabase
	{
		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06002C5A RID: 11354 RVA: 0x000CE14F File Offset: 0x000CC34F
		public GATActiveSampleBank MasterBank
		{
			get
			{
				return this.masterBank;
			}
		}

		// Token: 0x06002C5B RID: 11355 RVA: 0x000CE158 File Offset: 0x000CC358
		public AudioDatabase()
		{
			new GameObject("GATManager").AddComponent<GATManager>();
			this.CreateBanks();
			this.CreatePulseModules();
		}

		// Token: 0x06002C5C RID: 11356 RVA: 0x000CE1B3 File Offset: 0x000CC3B3
		public bool LoadBanks()
		{
			this.masterBank = this.LoadSampleBank();
			return !(this.masterBank == null);
		}

		// Token: 0x06002C5D RID: 11357 RVA: 0x000CE1D4 File Offset: 0x000CC3D4
		public bool LoadLoadouts()
		{
			foreach (string loadoutFilename in new List<string>
			{
				"Audio/Loadouts/sfx",
				"Audio/Loadouts/city",
				"Audio/Loadouts/menu",
				"Audio/Loadouts/beijing",
				"Audio/Loadouts/daressalaam",
				"Audio/Loadouts/dubai",
				"Audio/Loadouts/losangeles",
				"Audio/Loadouts/manila",
				"Audio/Loadouts/mexicocity",
				"Audio/Loadouts/moscow",
				"Audio/Loadouts/munich",
				"Audio/Loadouts/riodejaneiro",
				"Audio/Loadouts/tokyo",
				"Audio/Loadouts/tutorial",
				"Audio/Loadouts/wellington",
				"Audio/Loadouts/zurich",
				"Audio/Loadouts/warsaw",
				"Audio/Loadouts/chiangmai",
				"Audio/Loadouts/lisbon",
				"Audio/Loadouts/busan",
				"Audio/Loadouts/london",
				"Audio/Loadouts/mumbai",
				"Audio/Loadouts/newyorkcity",
				"Audio/Loadouts/reykjavik",
				"Audio/Loadouts/vancouver",
				"Audio/Loadouts/copenhagen",
				"Audio/Loadouts/cairns",
				"Audio/Loadouts/hongkong"
			})
			{
				object jsonLoadout = null;
				if (jsonLoadout == null)
				{
					jsonLoadout = JSON.Load(loadoutFilename, false);
				}
				if (jsonLoadout == null)
				{
					AudioSystem.Log.Error("AudioDatabase: Failed to load {0} as JSON.", new object[]
					{
						loadoutFilename
					});
				}
				else
				{
					AudioLoadout loadout = AudioLoadout.FromJSON(jsonLoadout as JSON.Dictionary);
					if (loadout == null)
					{
						AudioSystem.Log.Error("AudioDatabase: Failed to parse {0}.", new object[]
						{
							loadoutFilename
						});
					}
					else
					{
						this.loadouts[loadout.Id] = loadout;
					}
				}
			}
			return true;
		}

		// Token: 0x06002C5E RID: 11358 RVA: 0x000CE3BC File Offset: 0x000CC5BC
		public AudioDataBank CreateDataBank(string id, int frequency, bool isCompressed = false)
		{
			AudioDataBank newAudioDataBank = new AudioDataBank(id, frequency);
			this.audioDataBanks.Add(newAudioDataBank);
			return newAudioDataBank;
		}

		// Token: 0x06002C5F RID: 11359 RVA: 0x000CE3DE File Offset: 0x000CC5DE
		public bool LoadSample(string name)
		{
			return this.GetSampleData(name) != null;
		}

		// Token: 0x06002C60 RID: 11360 RVA: 0x000CE3EC File Offset: 0x000CC5EC
		public AudioSampleData GetSampleData(string name)
		{
			for (int i = 0; i < this.activeAudioDataBanks.Count; i++)
			{
				AudioSampleData sampleData = this.activeAudioDataBanks[i].GetSampleData(name);
				if (sampleData != null)
				{
					return sampleData;
				}
			}
			AudioSystem.Log.Error("AudioDatabase: Failed to find data for sample '{0}'.", new object[]
			{
				name
			});
			return null;
		}

		// Token: 0x06002C61 RID: 11361 RVA: 0x000CE441 File Offset: 0x000CC641
		public AudioLoadout GetLoadout(string id)
		{
			if (!this.loadouts.ContainsKey(id))
			{
				return null;
			}
			return this.loadouts[id];
		}

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06002C62 RID: 11362 RVA: 0x000CE14F File Offset: 0x000CC34F
		public GATActiveSampleBank DefaultSampleBank
		{
			get
			{
				return this.masterBank;
			}
		}

		// Token: 0x06002C63 RID: 11363 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public GATActiveSampleBank GetSampleBank(string bankId)
		{
			return null;
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06002C64 RID: 11364 RVA: 0x000CE45F File Offset: 0x000CC65F
		public MasterPulseModule MasterPulse
		{
			get
			{
				return this.masterPulse;
			}
		}

		// Token: 0x06002C65 RID: 11365 RVA: 0x000CE468 File Offset: 0x000CC668
		public SubPulseModule GetPulse(int stepCount, string key = "")
		{
			if (key == "")
			{
				key = stepCount.ToString();
			}
			SubPulseModule subPulse;
			if (this.subPulses.TryGetValue(key, out subPulse))
			{
				return subPulse;
			}
			if (stepCount <= 0)
			{
				return null;
			}
			subPulse = this.CreateSubPulseModule("Subpulse: 1/" + stepCount.ToString(), stepCount);
			this.subPulses.Add(stepCount.ToString(), subPulse);
			return subPulse;
		}

		// Token: 0x06002C66 RID: 11366 RVA: 0x000CE4D0 File Offset: 0x000CC6D0
		public SubPulseModule GetHyperPulse(Rhythm rhythm)
		{
			SubPulseModule subPulse;
			if (this.subPulses.TryGetValue(rhythm.Id, out subPulse))
			{
				return subPulse;
			}
			subPulse = this.CreateHyperPulseModule(rhythm);
			this.subPulses.Add(rhythm.Id, subPulse);
			return subPulse;
		}

		// Token: 0x06002C67 RID: 11367 RVA: 0x000CE510 File Offset: 0x000CC710
		private GATActiveSampleBank LoadSampleBank()
		{
			int sampleBankRate = this.SampleBankRate;
			if (sampleBankRate != AudioSettings.outputSampleRate)
			{
				AudioSystem.Log.Info("AudioDatabase: Resampling {0} Hz audio to {1} Hz.", new object[]
				{
					sampleBankRate,
					AudioSettings.outputSampleRate
				});
			}
			if (!this.LoadAudioBank("core", sampleBankRate, false))
			{
				return null;
			}
			return new GameObject("sampleBank").AddComponent<GATActiveSampleBank>();
		}

		// Token: 0x06002C68 RID: 11368 RVA: 0x000CE578 File Offset: 0x000CC778
		private void CreatePulseModules()
		{
			GameObject gameObject = new GameObject("Pulse: Master");
			this.masterPulse = gameObject.AddComponent<MasterPulseModule>();
			bool[] arr = new bool[12];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = true;
			}
			this.masterPulse.Steps = arr;
			this.masterPulse.Period = 0.8333333333333334;
			this.masterPulse.StartPulsing(0, 0.0);
		}

		// Token: 0x06002C69 RID: 11369 RVA: 0x000CE5EC File Offset: 0x000CC7EC
		private SubPulseModule CreateSubPulseModule(string name, int stepCount)
		{
			SubPulseModule subPulse = new GameObject(name).AddComponent<SubPulseModule>();
			subPulse.transform.parent = this.MasterPulse.transform;
			bool[] steps = new bool[stepCount];
			for (int i = 0; i < stepCount; i++)
			{
				steps[i] = true;
			}
			subPulse.Steps = steps;
			subPulse.SubPulseMode = SubPulseModule.PeriodMode.SubdivideParent;
			subPulse.ParentPulse = this.masterPulse;
			return subPulse;
		}

		// Token: 0x06002C6A RID: 11370 RVA: 0x000CE650 File Offset: 0x000CC850
		public SubPulseModule CreateHyperPulseModule(Rhythm rhythm)
		{
			SubPulseModule subPulse = new GameObject(rhythm.Id).AddComponent<SubPulseModule>();
			subPulse.transform.parent = this.MasterPulse.transform;
			bool[] steps = new bool[rhythm.Steps.Length];
			for (int i = 0; i < steps.Length; i++)
			{
				steps[i] = true;
			}
			subPulse.Steps = steps;
			subPulse.Ratios = rhythm.Steps;
			subPulse.RatioOffset = rhythm.Offset;
			subPulse.SubPulseMode = SubPulseModule.PeriodMode.Hyper;
			subPulse.ParentPulse = this.masterPulse;
			return subPulse;
		}

		// Token: 0x06002C6B RID: 11371 RVA: 0x000CE6D8 File Offset: 0x000CC8D8
		private bool LoadAudioBank(string id, int sampleRate, bool async)
		{
			int i = 0;
			while (i < this.audioDataBanks.Count)
			{
				AudioDataBank audioDataBank = this.audioDataBanks[i];
				if (audioDataBank.Id == id && audioDataBank.Frequency == sampleRate)
				{
					if (!audioDataBank.Load(async))
					{
						AudioSystem.Log.Warn("AudioDatabase: Failed to load audio bank '{0}' for {1} kHz.", new object[]
						{
							id,
							sampleRate
						});
						return false;
					}
					this.activeAudioDataBanks.Add(audioDataBank);
					return true;
				}
				else
				{
					i++;
				}
			}
			AudioSystem.Log.Warn("AudioDatabase: Failed to find audio bank '{0}' for {1} kHz.", new object[]
			{
				id,
				sampleRate
			});
			return false;
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06002C6C RID: 11372 RVA: 0x000CE780 File Offset: 0x000CC980
		private int SampleBankRate
		{
			get
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
				{
					return 24000;
				}
				int outputSampleRate = AudioSettings.outputSampleRate;
				int[] sampleRates = new int[]
				{
					24000,
					44100,
					48000
				};
				for (int i = 0; i < sampleRates.Length; i++)
				{
					if (sampleRates[i] >= outputSampleRate)
					{
						return sampleRates[i];
					}
				}
				return sampleRates[sampleRates.Length - 1];
			}
		}

		// Token: 0x04002691 RID: 9873
		private GATActiveSampleBank masterBank;

		// Token: 0x04002692 RID: 9874
		private MasterPulseModule masterPulse;

		// Token: 0x04002693 RID: 9875
		private Dictionary<string, SubPulseModule> subPulses = new Dictionary<string, SubPulseModule>();

		// Token: 0x04002694 RID: 9876
		private Dictionary<string, AudioLoadout> loadouts = new Dictionary<string, AudioLoadout>();

		// Token: 0x04002695 RID: 9877
		private List<AudioDataBank> audioDataBanks = new List<AudioDataBank>();

		// Token: 0x04002696 RID: 9878
		private List<AudioDataBank> activeAudioDataBanks = new List<AudioDataBank>();
	}
}
