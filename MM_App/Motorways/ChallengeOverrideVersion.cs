using System;
using System.Collections.Generic;

namespace Motorways
{
	// Token: 0x02000348 RID: 840
	public class ChallengeOverrideVersion
	{
		// Token: 0x060014B4 RID: 5300 RVA: 0x00044970 File Offset: 0x00042B70
		public string Serialize()
		{
			return Json.Serialize(new Dictionary<string, object>
			{
				{
					"Timestamp",
					this.Timestamp
				}
			}, false);
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x00044994 File Offset: 0x00042B94
		public bool Deserialize(string json)
		{
			JSON.Dictionary dictionary = JSON.ToDictionary(JSON.LoadFromString(json));
			if (dictionary == null)
			{
				ChallengeOverrides.Log.Error("Failed to parse JSON string to Dictionary.\n" + json, Array.Empty<object>());
				return false;
			}
			this.Timestamp = dictionary.GetInt("Timestamp", -1);
			if (this.Timestamp == -1)
			{
				ChallengeOverrides.Log.Error(string.Format("Failed to Deserialize Timestamp.\nTimestamp: {0}\n\nSource:\n{1}", this.Timestamp, json), Array.Empty<object>());
				return false;
			}
			return true;
		}

		// Token: 0x04001131 RID: 4401
		public int Timestamp;
	}
}
