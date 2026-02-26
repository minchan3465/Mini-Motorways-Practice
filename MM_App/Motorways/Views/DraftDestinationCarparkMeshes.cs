using System;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005BF RID: 1471
	public class DraftDestinationCarparkMeshes : MonoBehaviour
	{
		// Token: 0x060028FC RID: 10492 RVA: 0x000B01A0 File Offset: 0x000AE3A0
		public void SetVisibleCarparkMesh(bool isDouble, DrivewayDirection drivewayDirection, TileDirection carparkSide, bool supportsBoats, bool focusOnSecondDestination = false)
		{
			this._singleHorizontalDrivewayWest.SetActive(false);
			this._singleHorizontalDrivewayEast.SetActive(false);
			this._singleVerticalDrivewayNorth.SetActive(false);
			this._singleVerticalDrivewaySouth.SetActive(false);
			this._doubleHorizontalFirstDestination.SetActive(false);
			this._doubleHorizontalSecondDestination.SetActive(false);
			this._doubleVerticalFirstDestination.SetActive(false);
			this._doubleVerticalSecondDestination.SetActive(false);
			this._trainStationHorizontalFlippedFirstDestination.SetActive(false);
			this._trainStationHorizontalFlippedSecondDestination.SetActive(false);
			this._trainStationVerticalFlippedFirstDestination.SetActive(false);
			this._trainStationVerticalFlippedSecondDestination.SetActive(false);
			this._boatTerminalFirstDestination.SetActive(false);
			this._boatTerminalSecondDestination.SetActive(false);
			if (supportsBoats)
			{
				if (focusOnSecondDestination)
				{
					this._boatTerminalSecondDestination.SetActive(true);
					return;
				}
				this._boatTerminalFirstDestination.SetActive(true);
				return;
			}
			else if (isDouble)
			{
				if (carparkSide == TileDirection.South)
				{
					if (focusOnSecondDestination)
					{
						this._doubleHorizontalSecondDestination.SetActive(true);
						return;
					}
					this._doubleHorizontalFirstDestination.SetActive(true);
					return;
				}
				else if (carparkSide == TileDirection.West)
				{
					if (focusOnSecondDestination)
					{
						this._doubleVerticalSecondDestination.SetActive(true);
						return;
					}
					this._doubleVerticalFirstDestination.SetActive(true);
					return;
				}
				else if (carparkSide == TileDirection.North)
				{
					Diagnostics.Log.Info("DraftDestinationCarparkMeshes", "Enabling horizontal flipped train carpark!", Array.Empty<object>());
					if (focusOnSecondDestination)
					{
						this._trainStationHorizontalFlippedSecondDestination.SetActive(true);
						return;
					}
					this._trainStationHorizontalFlippedFirstDestination.SetActive(true);
					return;
				}
				else
				{
					Diagnostics.Log.Info("DraftDestinationCarparkMeshes", "Enabling vertical flipped train carpark!", Array.Empty<object>());
					if (focusOnSecondDestination)
					{
						this._trainStationVerticalFlippedSecondDestination.SetActive(true);
						return;
					}
					this._trainStationVerticalFlippedFirstDestination.SetActive(true);
					return;
				}
			}
			else
			{
				if (drivewayDirection == DrivewayDirection.West)
				{
					this._singleHorizontalDrivewayWest.SetActive(true);
					return;
				}
				if (drivewayDirection == DrivewayDirection.East)
				{
					this._singleHorizontalDrivewayEast.SetActive(true);
					return;
				}
				if (drivewayDirection == DrivewayDirection.North)
				{
					this._singleVerticalDrivewayNorth.SetActive(true);
					return;
				}
				this._singleVerticalDrivewaySouth.SetActive(true);
				return;
			}
		}

		// Token: 0x040022B1 RID: 8881
		[SerializeField]
		private GameObject _singleHorizontalDrivewayWest;

		// Token: 0x040022B2 RID: 8882
		[SerializeField]
		private GameObject _singleHorizontalDrivewayEast;

		// Token: 0x040022B3 RID: 8883
		[SerializeField]
		private GameObject _singleVerticalDrivewayNorth;

		// Token: 0x040022B4 RID: 8884
		[SerializeField]
		private GameObject _singleVerticalDrivewaySouth;

		// Token: 0x040022B5 RID: 8885
		[SerializeField]
		private GameObject _doubleHorizontalFirstDestination;

		// Token: 0x040022B6 RID: 8886
		[SerializeField]
		private GameObject _doubleHorizontalSecondDestination;

		// Token: 0x040022B7 RID: 8887
		[SerializeField]
		private GameObject _doubleVerticalFirstDestination;

		// Token: 0x040022B8 RID: 8888
		[SerializeField]
		private GameObject _doubleVerticalSecondDestination;

		// Token: 0x040022B9 RID: 8889
		[SerializeField]
		private GameObject _trainStationHorizontalFlippedFirstDestination;

		// Token: 0x040022BA RID: 8890
		[SerializeField]
		private GameObject _trainStationHorizontalFlippedSecondDestination;

		// Token: 0x040022BB RID: 8891
		[SerializeField]
		private GameObject _trainStationVerticalFlippedFirstDestination;

		// Token: 0x040022BC RID: 8892
		[SerializeField]
		private GameObject _trainStationVerticalFlippedSecondDestination;

		// Token: 0x040022BD RID: 8893
		[SerializeField]
		private GameObject _boatTerminalFirstDestination;

		// Token: 0x040022BE RID: 8894
		[SerializeField]
		private GameObject _boatTerminalSecondDestination;
	}
}
