using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x02000692 RID: 1682
	public class AudioModuleDefinition
	{
		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x06002EA1 RID: 11937 RVA: 0x000D8C13 File Offset: 0x000D6E13
		// (set) Token: 0x06002EA2 RID: 11938 RVA: 0x000D8C1B File Offset: 0x000D6E1B
		public string Id { get; private set; }

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06002EA3 RID: 11939 RVA: 0x000D8C24 File Offset: 0x000D6E24
		// (set) Token: 0x06002EA4 RID: 11940 RVA: 0x000D8C2C File Offset: 0x000D6E2C
		public int Order { get; private set; }

		// Token: 0x06002EA5 RID: 11941 RVA: 0x000D8C35 File Offset: 0x000D6E35
		public bool IsMute(AudioLoadout loadout)
		{
			return this.GetBool(loadout, "mute", false);
		}

		// Token: 0x06002EA6 RID: 11942 RVA: 0x000D8C44 File Offset: 0x000D6E44
		public bool IsSolo(AudioLoadout loadout)
		{
			return this.GetBool(loadout, "solo", false);
		}

		// Token: 0x06002EA7 RID: 11943 RVA: 0x000D8C54 File Offset: 0x000D6E54
		public IAudioModule CreateModule(AudioLoadout loadout)
		{
			IAudioModule module = null;
			Playback playback = null;
			switch (this.Type)
			{
			case AudioModuleType.DestinationInstancer:
				module = new DestinationInstancer(this.Filter);
				break;
			case AudioModuleType.VehicleInstancer:
				module = new VehicleInstancer(this.Filter);
				break;
			case AudioModuleType.SFX:
				module = new SFX();
				break;
			case AudioModuleType.Persistent:
				module = new Persistent();
				break;
			case AudioModuleType.Clock:
				playback = new Clock(this.Filter, this.Id);
				break;
			case AudioModuleType.House:
				playback = new House(this.Filter);
				break;
			case AudioModuleType.Road:
				playback = new Road(this.Filter);
				break;
			case AudioModuleType.TrafficLight:
				playback = new TrafficLight(this.Filter);
				break;
			case AudioModuleType.Experiment:
				playback = new Experiment(this.Filter);
				break;
			case AudioModuleType.DemandTimer:
				playback = new DemandTimer(this.Filter);
				break;
			case AudioModuleType.Motorway:
				playback = new Motorway(this.Filter);
				break;
			}
			if (module == null && playback != null)
			{
				module = PulsedAudioModule.CreateModule(this.Id, playback, null, this.GetInt(loadout, "pulse", 1));
			}
			return module;
		}

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x06002EA8 RID: 11944 RVA: 0x000D8D5C File Offset: 0x000D6F5C
		public AudioModuleType Type
		{
			get
			{
				Attribute typeAttribute = this.GetAttribute("type");
				if (typeAttribute == null)
				{
					return AudioModuleType.None;
				}
				return (AudioModuleType)Enum.Parse(typeof(AudioModuleType), typeAttribute.GetString(this.parentLoadout));
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x06002EA9 RID: 11945 RVA: 0x000D8D9C File Offset: 0x000D6F9C
		public AudioEventFilter Filter
		{
			get
			{
				if (this.filter.Type != AudioEventType.None)
				{
					return this.filter;
				}
				AudioModuleDefinition baseDefinition = this.BaseDefinition;
				if (baseDefinition != null)
				{
					return baseDefinition.Filter;
				}
				return this.filter;
			}
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x000D8DD4 File Offset: 0x000D6FD4
		public bool GetBool(AudioLoadout loadout, string name, bool defaultValue = false)
		{
			Attribute boolAttribute = this.GetAttribute(name);
			if (boolAttribute == null)
			{
				return defaultValue;
			}
			return boolAttribute.GetBool(loadout);
		}

		// Token: 0x06002EAB RID: 11947 RVA: 0x000D8DF8 File Offset: 0x000D6FF8
		public int GetInt(AudioLoadout loadout, string name, int defaultValue = 0)
		{
			Attribute intAttribute = this.GetAttribute(name);
			if (intAttribute == null)
			{
				return defaultValue;
			}
			return intAttribute.GetInt(loadout);
		}

		// Token: 0x06002EAC RID: 11948 RVA: 0x000D8E1C File Offset: 0x000D701C
		public int[] GetIntArray(AudioLoadout loadout, string name, int[] defaultValue = null)
		{
			Attribute intArrayAttribute = this.GetAttribute(name);
			if (intArrayAttribute == null)
			{
				return defaultValue;
			}
			return intArrayAttribute.GetIntArray(loadout);
		}

		// Token: 0x06002EAD RID: 11949 RVA: 0x000D8E40 File Offset: 0x000D7040
		public float GetFloat(AudioLoadout loadout, string name, float defaultValue = 0f)
		{
			Attribute floatAttribute = this.GetAttribute(name);
			if (floatAttribute == null)
			{
				return defaultValue;
			}
			return floatAttribute.GetFloat(loadout);
		}

		// Token: 0x06002EAE RID: 11950 RVA: 0x000D8E64 File Offset: 0x000D7064
		public float[] GetFloatArray(AudioLoadout loadout, string name, float[] defaultValue = null)
		{
			Attribute floatArrayAttribute = this.GetAttribute(name);
			if (floatArrayAttribute == null)
			{
				return defaultValue;
			}
			return floatArrayAttribute.GetFloatArray(loadout);
		}

		// Token: 0x06002EAF RID: 11951 RVA: 0x000D8E88 File Offset: 0x000D7088
		public string GetString(AudioLoadout loadout, string name, string defaultValue = null)
		{
			Attribute stringAttribute = this.GetAttribute(name);
			if (stringAttribute == null)
			{
				return defaultValue;
			}
			return stringAttribute.GetString(loadout);
		}

		// Token: 0x06002EB0 RID: 11952 RVA: 0x000D8EAC File Offset: 0x000D70AC
		public string[] GetStringArray(AudioLoadout loadout, string name, string[] defaultValue = null)
		{
			Attribute stringArrayAttribute = this.GetAttribute(name);
			if (stringArrayAttribute == null)
			{
				return defaultValue;
			}
			return stringArrayAttribute.GetStringArray(loadout);
		}

		// Token: 0x06002EB1 RID: 11953 RVA: 0x000D8ED0 File Offset: 0x000D70D0
		private Attribute GetAttribute(string name)
		{
			if (this.attributes.ContainsKey(name))
			{
				return this.attributes[name];
			}
			AudioModuleDefinition baseDefinition = this.BaseDefinition;
			if (baseDefinition != null)
			{
				return baseDefinition.GetAttribute(name);
			}
			return null;
		}

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x06002EB2 RID: 11954 RVA: 0x000D8F0C File Offset: 0x000D710C
		private AudioModuleDefinition BaseDefinition
		{
			get
			{
				if (!string.IsNullOrEmpty(this.Id) && this.Id.Length > 0)
				{
					AudioLoadout baseLoadout = this.parentLoadout.BaseLoadout;
					if (baseLoadout != null)
					{
						return baseLoadout.GetModuleDefinition(this.Id);
					}
				}
				return null;
			}
		}

		// Token: 0x06002EB3 RID: 11955 RVA: 0x000D8F54 File Offset: 0x000D7154
		public static AudioModuleDefinition FromJSON(AudioLoadout loadout, JSON.Dictionary jsonDictionary)
		{
			if (jsonDictionary == null)
			{
				return null;
			}
			AudioModuleDefinition moduleDefinition = new AudioModuleDefinition(loadout);
			foreach (string key in jsonDictionary.Keys)
			{
				if (key == "id")
				{
					moduleDefinition.Id = jsonDictionary.GetString("id");
				}
				else
				{
					if (key == "name" && string.IsNullOrEmpty(moduleDefinition.Id))
					{
						moduleDefinition.Id = jsonDictionary.GetString("name");
					}
					if (key == "filter")
					{
						moduleDefinition.filter = AudioEventFilter.FromJSON(jsonDictionary.GetDictionary("filter"));
					}
					else if (key == "order")
					{
						moduleDefinition.Order = jsonDictionary.GetInt("order", 0);
					}
					else
					{
						Attribute attribute = Attribute.FromJSON(jsonDictionary[key]);
						if (attribute != null)
						{
							moduleDefinition.attributes[key] = attribute;
						}
					}
				}
			}
			return moduleDefinition;
		}

		// Token: 0x06002EB4 RID: 11956 RVA: 0x000D9064 File Offset: 0x000D7264
		private AudioModuleDefinition(AudioLoadout loadout)
		{
			this.parentLoadout = loadout;
			this.Order = int.MaxValue;
		}

		// Token: 0x0400287D RID: 10365
		private AudioLoadout parentLoadout;

		// Token: 0x0400287E RID: 10366
		private AudioEventFilter filter;

		// Token: 0x0400287F RID: 10367
		private Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();
	}
}
