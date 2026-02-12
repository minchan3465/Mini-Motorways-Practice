using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Utility
{
	// Token: 0x02000460 RID: 1120
	[CreateAssetMenu(menuName = "Motorways/RoadTileOverride")]
	[Serializable]
	public class RoadTileMeshOverride : ScriptableObject
	{
		// Token: 0x040017CC RID: 6092
		[SerializeField]
		public List<RoadTileMeshOverrideDefinition> meshOverrides;
	}
}
