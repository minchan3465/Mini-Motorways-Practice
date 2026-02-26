using System;
using System.Collections.Generic;
using Motorways.Views;
using Motorways.Views.Boats;
using Motorways.Views.Trains;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000664 RID: 1636
	public class AudioLoadout
	{
		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06002D6A RID: 11626 RVA: 0x000D16F0 File Offset: 0x000CF8F0
		// (set) Token: 0x06002D6B RID: 11627 RVA: 0x000D16F8 File Offset: 0x000CF8F8
		public GameObject GameObject { get; private set; }

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06002D6C RID: 11628 RVA: 0x000D1701 File Offset: 0x000CF901
		// (set) Token: 0x06002D6D RID: 11629 RVA: 0x000D1709 File Offset: 0x000CF909
		public string Id { get; private set; }

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06002D6E RID: 11630 RVA: 0x000D1714 File Offset: 0x000CF914
		public List<Rhythm> DestinationGroupRhythms
		{
			get
			{
				List<Rhythm> dgRhythms = new List<Rhythm>();
				foreach (DestinationGroup dg in Get.Loadout.DestinationGroups)
				{
					dgRhythms.Add(dg.Module.Rhythm);
				}
				return dgRhythms;
			}
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06002D6F RID: 11631 RVA: 0x000D177C File Offset: 0x000CF97C
		public AudioLoadout BaseLoadout
		{
			get
			{
				return this._baseLoadout;
			}
		}

		// Token: 0x06002D70 RID: 11632 RVA: 0x000D1784 File Offset: 0x000CF984
		private AudioLoadout()
		{
		}

		// Token: 0x06002D71 RID: 11633 RVA: 0x000D17B0 File Offset: 0x000CF9B0
		private void CreateDestinationGroups()
		{
			int activeGroups = 0;
			foreach (List<DestinationView> list in Get.Environment.Destinations)
			{
				foreach (DestinationView v in list)
				{
					activeGroups = Mathf.Max(activeGroups, v.groupIndex + 1);
				}
			}
			this.DestinationGroups.Clear();
			for (int i = 0; i < activeGroups; i++)
			{
				this.DestinationGroups.Add(this.CreateDestinationGroup(i));
			}
		}

		// Token: 0x06002D72 RID: 11634 RVA: 0x000D1874 File Offset: 0x000CFA74
		private DestinationGroup CreateDestinationGroup(int groupIndex)
		{
			Dbug.Log.Info("AudioLoadout.CreateDestinationGroup(): New Destination Group: {0}", new object[]
			{
				groupIndex
			});
			AudioEventFilter f = new AudioEventFilter
			{
				GroupIndex = groupIndex
			};
			DestinationGroup destinationGroup = new DestinationGroup(f);
			IAudioModule i = PulsedAudioModule.CreateModule("Destination Group " + f.GroupIndex.ToString(), destinationGroup, this.MusicData.PickInitRhythm(f.GroupIndex), -1);
			this.AddDynamicModule(i);
			return destinationGroup;
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x000D18F4 File Offset: 0x000CFAF4
		public DestinationGroup GetDestinationGroup(int groupIndex)
		{
			if (groupIndex < this.DestinationGroups.Count)
			{
				return this.DestinationGroups[groupIndex];
			}
			while (groupIndex >= this.DestinationGroups.Count)
			{
				this.DestinationGroups.Add(this.CreateDestinationGroup(this.DestinationGroups.Count));
			}
			return this.DestinationGroups[groupIndex];
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x000D1954 File Offset: 0x000CFB54
		private void CreateBoatModule()
		{
			this.Boat = null;
			if (this.Environment.Boats.Count <= 0)
			{
				return;
			}
			BoatView view = this.Environment.Boats[0];
			this.Boat = new Boat(view);
			this.AddDynamicModule(PulsedAudioModule.CreateModule("Boat " + view.name, this.Boat, null, 4));
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x000D19C0 File Offset: 0x000CFBC0
		private void CreateTrainModule()
		{
			this.Train = null;
			if (this.Environment.Trains.Count <= 0)
			{
				return;
			}
			TrainView view = this.Environment.Trains[0];
			this.Train = new Train(view);
			this.AddDynamicModule(PulsedAudioModule.CreateModule("Train " + view.name, this.Train, null, 4));
		}

		// Token: 0x06002D76 RID: 11638 RVA: 0x000D1A2C File Offset: 0x000CFC2C
		private void CreateVehicleModules()
		{
			if (this.Environment.Vehicles[0].Count == 0)
			{
				Dbug.Log.Warn("CreateVehicleModules(): No vehicles in group index 0.", Array.Empty<object>());
				return;
			}
			foreach (List<VehicleView> list in this.Environment.Vehicles)
			{
				foreach (VehicleView v in list)
				{
					if (v.AudioVehicle == null)
					{
						default(AudioEventFilter).Vehicle = v;
						Playback playback = new Vehicle(v);
						IAudioModule newVehicleModule = PulsedAudioModule.CreateModule("Vehicle " + v.Id.ToString(), playback, null, 4);
						this.AddDynamicModule(newVehicleModule);
					}
				}
			}
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x000D1B30 File Offset: 0x000CFD30
		private void CreateDrumSequencer()
		{
			this.DrumSequencer = new DrumSequencer();
			IAudioModule newModule = PulsedAudioModule.CreateModule("DrumSequencer " + this.Environment.City.Definition.name, this.DrumSequencer, null, 1);
			this.AddDynamicModule(newModule);
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x000D1B7C File Offset: 0x000CFD7C
		public void Activate(AudioEnvironment environment = null)
		{
			AudioSystem.Log.Info("AudioLoadout: Activating loadout {0}. isActive == {1}", new object[]
			{
				this.Id,
				this.isActive
			});
			this.Environment = (environment ?? Get.Environment);
			if (!this.isActive)
			{
				if (this.Id != "sfx")
				{
					string id = this.Id;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
					MusicData musicData;
					if (num <= 1503674346U)
					{
						if (num <= 796856760U)
						{
							if (num <= 539102529U)
							{
								if (num != 331822766U)
								{
									if (num != 456984969U)
									{
										if (num == 539102529U)
										{
											if (id == "cairns")
											{
												musicData = new Cairns();
												goto IL_515;
											}
										}
									}
									else if (id == "moscow")
									{
										musicData = new Moscow();
										goto IL_515;
									}
								}
								else if (id == "zurich")
								{
									musicData = new Zurich();
									goto IL_515;
								}
							}
							else if (num != 644625860U)
							{
								if (num != 672799717U)
								{
									if (num == 796856760U)
									{
										if (id == "chiangmai")
										{
											musicData = new ChiangMai();
											goto IL_515;
										}
									}
								}
								else if (id == "copenhagen")
								{
									musicData = new Copenhagen();
									goto IL_515;
								}
							}
							else if (id == "wellington")
							{
								musicData = new Wellington();
								goto IL_515;
							}
						}
						else if (num <= 951428446U)
						{
							if (num != 908156657U)
							{
								if (num != 912332847U)
								{
									if (num == 951428446U)
									{
										if (id == "dubai")
										{
											musicData = new Dubai();
											goto IL_515;
										}
									}
								}
								else if (id == "tutorial")
								{
									musicData = new Tutorial();
									goto IL_515;
								}
							}
							else if (id == "mexicocity")
							{
								musicData = new MexicoCity();
								goto IL_515;
							}
						}
						else if (num <= 1307848146U)
						{
							if (num != 1153259090U)
							{
								if (num == 1307848146U)
								{
									if (id == "busan")
									{
										musicData = new Busan();
										goto IL_515;
									}
								}
							}
							else if (id == "lisbon")
							{
								musicData = new Lisbon();
								goto IL_515;
							}
						}
						else if (num != 1357522533U)
						{
							if (num == 1503674346U)
							{
								if (id == "riodejaneiro")
								{
									musicData = new RioDeJaneiro();
									goto IL_515;
								}
							}
						}
						else if (id == "munich")
						{
							musicData = new Munich();
							goto IL_515;
						}
					}
					else if (num <= 2353801589U)
					{
						if (num <= 2054585666U)
						{
							if (num != 1710814996U)
							{
								if (num != 1882950431U)
								{
									if (num == 2054585666U)
									{
										if (id == "losangeles")
										{
											musicData = new LosAngeles();
											goto IL_515;
										}
									}
								}
								else if (id == "london")
								{
									musicData = new London();
									goto IL_515;
								}
							}
							else if (id == "vancouver")
							{
								musicData = new Vancouver();
								goto IL_515;
							}
						}
						else if (num != 2125789206U)
						{
							if (num != 2265167113U)
							{
								if (num == 2353801589U)
								{
									if (id == "daressalaam")
									{
										musicData = new DarEsSalaam();
										goto IL_515;
									}
								}
							}
							else if (id == "reykjavik")
							{
								musicData = new Reykjavik();
								goto IL_515;
							}
						}
						else if (id == "hongkong")
						{
							musicData = new HongKong();
							goto IL_515;
						}
					}
					else if (num <= 2639563685U)
					{
						if (num != 2504543993U)
						{
							if (num != 2581912890U)
							{
								if (num == 2639563685U)
								{
									if (id == "beijing")
									{
										musicData = new Beijing();
										goto IL_515;
									}
								}
							}
							else if (id == "menu")
							{
								musicData = new Menu();
								goto IL_515;
							}
						}
						else if (id == "tokyo")
						{
							musicData = new Tokyo();
							goto IL_515;
						}
					}
					else if (num <= 2922434365U)
					{
						if (num != 2795893313U)
						{
							if (num == 2922434365U)
							{
								if (id == "newyorkcity")
								{
									musicData = new NewYorkCity();
									goto IL_515;
								}
							}
						}
						else if (id == "manila")
						{
							musicData = new Manila();
							goto IL_515;
						}
					}
					else if (num != 3917494402U)
					{
						if (num == 3962314282U)
						{
							if (id == "warsaw")
							{
								musicData = new Warsaw();
								goto IL_515;
							}
						}
					}
					else if (id == "mumbai")
					{
						musicData = new Mumbai();
						goto IL_515;
					}
					musicData = new MusicData();
					IL_515:
					this.MusicData = musicData;
					this.MusicData.Injections();
					this.MusicData.Initialize();
					this.CreateModules();
					this.CreateDestinationGroups();
					this.CreateVehicleModules();
					this.CreateDrumSequencer();
					this.CreateTrainModule();
					this.CreateBoatModule();
					this.MusicData.PostLoad();
				}
				else
				{
					this.CreateModules();
					AudioLoadout.PersistentLoadout = this;
				}
				this.isActive = true;
			}
			for (int moduleIndex = 0; moduleIndex < this.modules.Count; moduleIndex++)
			{
				this.modules[moduleIndex].Activate(environment);
			}
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x000D2128 File Offset: 0x000D0328
		public void Deactivate()
		{
			if (!this.isActive)
			{
				return;
			}
			AudioSystem.Log.Info("AudioLoadout: Deactivating, then Resetting loadout {0}.", new object[]
			{
				this.Id
			});
			this.isActive = false;
			for (int moduleIndex = 0; moduleIndex < this.modules.Count; moduleIndex++)
			{
				this.modules[moduleIndex].Deactivate();
			}
			this.Reset();
			for (int dynamicModuleIndex = 0; dynamicModuleIndex < this.dynamicModules.Count; dynamicModuleIndex++)
			{
				this.modules.Remove(this.dynamicModules[dynamicModuleIndex]);
				this.dynamicModules[dynamicModuleIndex].Release();
			}
			this.dynamicModules.Clear();
			this.Environment = null;
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06002D7A RID: 11642 RVA: 0x000D21E1 File Offset: 0x000D03E1
		public bool IsActive
		{
			get
			{
				return this.isActive;
			}
		}

		// Token: 0x06002D7B RID: 11643 RVA: 0x000D21E9 File Offset: 0x000D03E9
		private void Reset()
		{
			this.DestinationGroups.Clear();
		}

		// Token: 0x06002D7C RID: 11644 RVA: 0x000D21F8 File Offset: 0x000D03F8
		public void Update()
		{
			for (int moduleIndex = 0; moduleIndex < this.modules.Count; moduleIndex++)
			{
				this.modules[moduleIndex].UpdateModule();
			}
		}

		// Token: 0x06002D7D RID: 11645 RVA: 0x000D222C File Offset: 0x000D042C
		public AudioModuleDefinition GetModuleDefinition(string moduleId)
		{
			for (int moduleIndex = 0; moduleIndex < this.moduleDefinitions.Count; moduleIndex++)
			{
				if (this.moduleDefinitions[moduleIndex].Id == moduleId)
				{
					return this.moduleDefinitions[moduleIndex];
				}
			}
			if (this._baseLoadout == null)
			{
				return null;
			}
			return this._baseLoadout.GetModuleDefinition(moduleId);
		}

		// Token: 0x06002D7E RID: 11646 RVA: 0x000D228B File Offset: 0x000D048B
		public Attribute GetConstant(string name)
		{
			if (this.constants != null && this.constants.ContainsKey(name))
			{
				return this.constants[name];
			}
			if (this._baseLoadout != null)
			{
				return this._baseLoadout.GetConstant(name);
			}
			return null;
		}

		// Token: 0x06002D7F RID: 11647 RVA: 0x000D22C6 File Offset: 0x000D04C6
		public void AddDynamicModule(IAudioModule dynamicModule)
		{
			this.modules.Add(dynamicModule);
			this.dynamicModules.Add(dynamicModule);
			if (this.isActive)
			{
				dynamicModule.Activate(this.Environment);
			}
		}

		// Token: 0x06002D80 RID: 11648 RVA: 0x000D22F4 File Offset: 0x000D04F4
		public static AudioLoadout FromJSON(JSON.Dictionary jsonDictionary)
		{
			if (jsonDictionary == null)
			{
				return null;
			}
			string id = jsonDictionary.GetString("id");
			if (id == null)
			{
				return null;
			}
			AudioLoadout loadout = new AudioLoadout();
			loadout.Id = id;
			loadout.GameObject = new GameObject();
			string baseId = jsonDictionary.GetString("base");
			if (baseId != null)
			{
				loadout._baseLoadout = AudioSystem.Instance.Database.GetLoadout(baseId);
			}
			JSON.Dictionary jsonConstants = jsonDictionary.GetDictionary("constants");
			if (jsonConstants != null)
			{
				loadout.constants = new Dictionary<string, Attribute>();
				foreach (string key in jsonConstants.Keys)
				{
					Attribute constantAttribute = Attribute.FromJSON(jsonConstants[key]);
					if (constantAttribute != null)
					{
						loadout.constants[key] = constantAttribute;
					}
				}
			}
			JSON.Array jsonModules = jsonDictionary.GetArray("modules");
			if (jsonModules != null)
			{
				for (int moduleIndex = 0; moduleIndex < jsonModules.Count; moduleIndex++)
				{
					JSON.Dictionary jsonModule = jsonModules.GetDictionary(moduleIndex);
					if (jsonModule != null)
					{
						if (jsonModule.ContainsKey("template") && jsonModule.ContainsKey("instances"))
						{
							JSON.Dictionary jsonTemplate = jsonModule.GetDictionary("template");
							JSON.Array jsonInstances = jsonModule.GetArray("instances");
							for (int instanceIndex = 0; instanceIndex < jsonInstances.Count; instanceIndex++)
							{
								JSON.Dictionary jsonInstance = jsonInstances.GetDictionary(instanceIndex);
								AudioModuleDefinition moduleDefinition = AudioModuleDefinition.FromJSON(loadout, JSON.Dictionary.Merge(jsonTemplate, jsonInstance));
								if (moduleDefinition != null)
								{
									loadout.moduleDefinitions.Add(moduleDefinition);
								}
							}
						}
						else
						{
							AudioModuleDefinition moduleDefinition2 = AudioModuleDefinition.FromJSON(loadout, jsonModule);
							if (moduleDefinition2 != null)
							{
								loadout.moduleDefinitions.Add(moduleDefinition2);
							}
						}
					}
				}
			}
			if (jsonDictionary.GetBool("activate", false))
			{
				loadout.Activate(null);
			}
			return loadout;
		}

		// Token: 0x06002D81 RID: 11649 RVA: 0x000D24C4 File Offset: 0x000D06C4
		private void CreateModules()
		{
			List<AudioModuleDefinition> allModuleDefinitions;
			if (this._baseLoadout == null)
			{
				allModuleDefinitions = this.moduleDefinitions;
			}
			else
			{
				allModuleDefinitions = new List<AudioModuleDefinition>();
				for (AudioLoadout loadout = this; loadout != null; loadout = loadout._baseLoadout)
				{
					if (loadout.moduleDefinitions != null)
					{
						for (int moduleIndex = 0; moduleIndex < loadout.moduleDefinitions.Count; moduleIndex++)
						{
							AudioModuleDefinition moduleDefinition = loadout.moduleDefinitions[moduleIndex];
							bool isDefinitionOverridden = false;
							if (moduleDefinition.Id != null)
							{
								for (int allModuleIndex = 0; allModuleIndex < allModuleDefinitions.Count; allModuleIndex++)
								{
									if (allModuleDefinitions[allModuleIndex].Id == moduleDefinition.Id)
									{
										isDefinitionOverridden = true;
										break;
									}
								}
							}
							if (!isDefinitionOverridden)
							{
								allModuleDefinitions.Add(moduleDefinition);
							}
						}
					}
				}
			}
			if (allModuleDefinitions == null)
			{
				return;
			}
			allModuleDefinitions.Sort((AudioModuleDefinition x, AudioModuleDefinition y) => x.Order - y.Order);
			bool anySoloModules = false;
			int moduleIndex2 = 0;
			while (!anySoloModules && moduleIndex2 < allModuleDefinitions.Count)
			{
				anySoloModules = allModuleDefinitions[moduleIndex2].IsSolo(this);
				moduleIndex2++;
			}
			this.modules = new List<IAudioModule>();
			for (int moduleIndex3 = 0; moduleIndex3 < allModuleDefinitions.Count; moduleIndex3++)
			{
				AudioModuleDefinition moduleDefinition2 = allModuleDefinitions[moduleIndex3];
				if ((!anySoloModules || moduleDefinition2.IsSolo(this)) && !moduleDefinition2.IsMute(this))
				{
					IAudioModule module = moduleDefinition2.CreateModule(this);
					if (module != null)
					{
						this.modules.Add(module);
					}
				}
			}
		}

		// Token: 0x0400278C RID: 10124
		public List<DestinationGroup> DestinationGroups = new List<DestinationGroup>();

		// Token: 0x0400278D RID: 10125
		public MusicData MusicData;

		// Token: 0x0400278E RID: 10126
		public DrumSequencer DrumSequencer;

		// Token: 0x0400278F RID: 10127
		public Train Train;

		// Token: 0x04002790 RID: 10128
		public Boat Boat;

		// Token: 0x04002791 RID: 10129
		private bool isActive;

		// Token: 0x04002792 RID: 10130
		private AudioEnvironment Environment;

		// Token: 0x04002793 RID: 10131
		private Dictionary<string, Attribute> constants;

		// Token: 0x04002794 RID: 10132
		private List<AudioModuleDefinition> moduleDefinitions = new List<AudioModuleDefinition>();

		// Token: 0x04002795 RID: 10133
		private List<IAudioModule> modules;

		// Token: 0x04002796 RID: 10134
		private List<IAudioModule> dynamicModules = new List<IAudioModule>();

		// Token: 0x04002797 RID: 10135
		public static AudioLoadout PersistentLoadout;

		// Token: 0x04002798 RID: 10136
		private AudioLoadout _baseLoadout;
	}
}
