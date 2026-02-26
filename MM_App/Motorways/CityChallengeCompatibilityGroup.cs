using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000352 RID: 850
	[CreateAssetMenu(fileName = "New Challenge", menuName = "Motorways/Challenges/CityGroup", order = 3)]
	public class CityChallengeCompatibilityGroup : ScriptableObject
	{
		// Token: 0x060014EE RID: 5358 RVA: 0x00045AE8 File Offset: 0x00043CE8
		public bool IsMapCompatible(MapDefinition.CityNames cityName)
		{
			bool containedInList = this.cities.Contains(cityName);
			return this.isWhiteList == containedInList;
		}

		// Token: 0x04001163 RID: 4451
		[TextArea]
		public string description;

		// Token: 0x04001164 RID: 4452
		[InfoBox("If a map is in this list, is it compatible or incompatible with the group?", InfoBoxType.Normal, null)]
		public bool isWhiteList;

		// Token: 0x04001165 RID: 4453
		public MapDefinition.CityNames[] cities = new MapDefinition.CityNames[0];
	}
}
