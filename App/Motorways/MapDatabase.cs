using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200037E RID: 894
	[CreateAssetMenu(fileName = "New Map Database", menuName = "Motorways/Map Database", order = 2)]
	public class MapDatabase : ScriptableObject
	{
		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x060015A5 RID: 5541 RVA: 0x0004A5B2 File Offset: 0x000487B2
		public MapLibrary MapLibrary
		{
			get
			{
				return this._mapLibrary;
			}
		}

		// Token: 0x04001251 RID: 4689
		[SerializeField]
		private MapLibrary _mapLibrary;
	}
}
