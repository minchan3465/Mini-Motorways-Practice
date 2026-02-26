using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200059D RID: 1437
	public class CitySpawningView : MonoBehaviour, IReleasedFromScopeHandler, IReusable, IView
	{
		// Token: 0x06002824 RID: 10276 RVA: 0x000AB457 File Offset: 0x000A9657
		public void Reset()
		{
			this._hasSetName = false;
			this._knownHouseGroupIds.Clear();
			this._knownDestinationGroupIds.Clear();
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x000AB478 File Offset: 0x000A9678
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (object obj in this._tileGridsFolder.transform)
			{
				UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
			foreach (object obj2 in this._buildingPlacementsFolder.transform)
			{
				UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
			}
		}

		// Token: 0x06002826 RID: 10278 RVA: 0x0000222C File Offset: 0x0000042C
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002827 RID: 10279 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002828 RID: 10280 RVA: 0x000AB528 File Offset: 0x000A9728
		public void AddBuildingPlacementObject(GameObject buildingPlacementObject)
		{
			buildingPlacementObject.transform.SetParent(this._buildingPlacementsFolder.transform);
		}

		// Token: 0x06002829 RID: 10281 RVA: 0x000AB540 File Offset: 0x000A9740
		private void AddTileMatrixObject(string name, TileMatrixInt tileMatrix, int minData, int maxData)
		{
			GameObject gameObject = new GameObject(name);
			TileMatrixView tileMatrixView = gameObject.AddComponent<TileMatrixView>();
			tileMatrixView.SourceMatrix = tileMatrix;
			tileMatrixView.SetTileColors(minData, maxData);
			gameObject.transform.SetParent(this._tileGridsFolder.transform);
		}

		// Token: 0x040021ED RID: 8685
		[Dependency]
		private City _city;

		// Token: 0x040021EE RID: 8686
		[Dependency]
		private CityPlanModel _cityPlan;

		// Token: 0x040021EF RID: 8687
		[SerializeField]
		private GameObject _tileGridsFolder;

		// Token: 0x040021F0 RID: 8688
		[SerializeField]
		private GameObject _buildingPlacementsFolder;

		// Token: 0x040021F1 RID: 8689
		private bool _hasSetName;

		// Token: 0x040021F2 RID: 8690
		private HashSet<int> _knownHouseGroupIds = new HashSet<int>();

		// Token: 0x040021F3 RID: 8691
		private HashSet<int> _knownDestinationGroupIds = new HashSet<int>();
	}
}
