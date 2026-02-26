using System;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200038A RID: 906
	[Factory.Serializable(1)]
	[System.Serializable]
	public class TileMatrixBool : TileMatrix<bool>
	{
		// Token: 0x060015DA RID: 5594 RVA: 0x0004AE6D File Offset: 0x0004906D
		public static TileMatrixBool CreateUnscoped(RectInt dimensions, bool defaultValue)
		{
			TileMatrixBool tileMatrixBool = new TileMatrixBool();
			tileMatrixBool.Initialize(dimensions, defaultValue);
			return tileMatrixBool;
		}
	}
}
