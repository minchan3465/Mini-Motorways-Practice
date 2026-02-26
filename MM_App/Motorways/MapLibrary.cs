using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000381 RID: 897
	[CreateAssetMenu(fileName = "New Map Library", menuName = "Motorways/Map Library", order = 2)]
	public class MapLibrary : ScriptableObject
	{
		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x060015AE RID: 5550 RVA: 0x0004A7E7 File Offset: 0x000489E7
		public IEnumerable<MapDefinition> Maps
		{
			get
			{
				return this._maps;
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060015AF RID: 5551 RVA: 0x0004A7EF File Offset: 0x000489EF
		public int MapCount
		{
			get
			{
				return this._maps.Length;
			}
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x0004A7FC File Offset: 0x000489FC
		public MapDefinition GetMapByName(string cityName)
		{
			foreach (MapDefinition map in this._maps)
			{
				if (map.cityName == cityName)
				{
					return map;
				}
			}
			return null;
		}

		// Token: 0x0400127C RID: 4732
		[SerializeField]
		private MapDefinition[] _maps;
	}
}
