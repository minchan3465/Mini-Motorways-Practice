using System;
using Motorways.Audio;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005B7 RID: 1463
	public class TileMatrixView : MonoBehaviour
	{
		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x060028B9 RID: 10425 RVA: 0x000ADBAE File Offset: 0x000ABDAE
		// (set) Token: 0x060028BA RID: 10426 RVA: 0x000ADBB6 File Offset: 0x000ABDB6
		public TileMatrixInt SourceMatrix
		{
			get
			{
				return this._sourceMatrix;
			}
			set
			{
				this._sourceMatrix = value;
			}
		}

		// Token: 0x060028BB RID: 10427 RVA: 0x000ADBBF File Offset: 0x000ABDBF
		public void SetTileColors(int minData, int maxData)
		{
			this._minData = minData;
			this._maxData = maxData;
		}

		// Token: 0x060028BC RID: 10428 RVA: 0x000ADBCF File Offset: 0x000ABDCF
		public void Awake()
		{
			this._debugViewer = base.gameObject.AddComponent<DebugTileDataViewer>();
			this._debugViewer.tileCoordinatesOn = false;
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x000ADBF0 File Offset: 0x000ABDF0
		public void Update()
		{
			foreach (Vector2Int coordinates in this._sourceMatrix.Dimensions.allPositionsWithin)
			{
				int data = this._sourceMatrix[coordinates];
				if (data != 2147483647)
				{
					this._debugViewer.stringData[coordinates] = string.Format("{0}", data);
					this._debugViewer.squareTileData[coordinates] = new Color(1f, 0f, 0f, Maf.Map((float)data, (float)this._minData, (float)this._maxData, 0f, 0.5f));
				}
			}
		}

		// Token: 0x04002270 RID: 8816
		private TileMatrixInt _sourceMatrix;

		// Token: 0x04002271 RID: 8817
		private int _minData = -1;

		// Token: 0x04002272 RID: 8818
		private int _maxData = -1;

		// Token: 0x04002273 RID: 8819
		private DebugTileDataViewer _debugViewer;
	}
}
