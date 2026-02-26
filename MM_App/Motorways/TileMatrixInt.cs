using System;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200038B RID: 907
	[Factory.Serializable(1)]
	public class TileMatrixInt : TileMatrix<int>
	{
		// Token: 0x060015DC RID: 5596 RVA: 0x0004AE84 File Offset: 0x00049084
		public static TileMatrixInt Create(IScope scope, RectInt dimensions, int defaultValue)
		{
			TileMatrixInt tileMatrixInt = scope.Get<TileMatrixInt>();
			tileMatrixInt.Initialize(dimensions, defaultValue);
			return tileMatrixInt;
		}
	}
}
