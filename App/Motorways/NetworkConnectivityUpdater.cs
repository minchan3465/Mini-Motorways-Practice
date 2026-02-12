using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Views;
using Server;

namespace Motorways
{
	// Token: 0x020003BA RID: 954
	public class NetworkConnectivityUpdater : CityModel.IObserver, IReusable, CarparkModel.IObserver
	{
		// Token: 0x060016B9 RID: 5817 RVA: 0x000524D1 File Offset: 0x000506D1
		public void Start()
		{
			this.TestDisconnectedBuildings();
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x000524D9 File Offset: 0x000506D9
		public void TestHouse(HouseView house)
		{
			if (!this._housesToTest.Contains(house))
			{
				house.NetworkConnectivity = NetworkConnectivity.Unknown;
				this._housesToTest.Add(house);
			}
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x000524FC File Offset: 0x000506FC
		public void TestDestination(DestinationView destination)
		{
			if (!this._destinationsToTest.Contains(destination))
			{
				destination.NetworkConnectivity = NetworkConnectivity.Unknown;
				this._destinationsToTest.Add(destination);
			}
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x0005251F File Offset: 0x0005071F
		public void Reset()
		{
			this._hasSubscribedToCity = false;
			this._housesToTest.Clear();
			this._destinationsToTest.Clear();
			this._testDisconnectedBuildingsDuringNextTick = false;
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x00052548 File Offset: 0x00050748
		public void Tick()
		{
			if (!this._hasSubscribedToCity)
			{
				this._simulation.GetModel<CityModel>().Subscribe(this);
				foreach (CarparkModel carparkModel in this._simulation.GetModels<CarparkModel>())
				{
					this.OnCarparkAdded(carparkModel);
				}
				this._hasSubscribedToCity = true;
			}
			if (this._testDisconnectedBuildingsDuringNextTick)
			{
				this.TestDisconnectedBuildings();
				this._testDisconnectedBuildingsDuringNextTick = false;
			}
			if (this._destinationsToTest.Count > 0)
			{
				DestinationView destination = this._destinationsToTest[0];
				this._destinationsToTest.RemoveAt(0);
				this.UpdateDestinationConnectivity(destination);
				return;
			}
			if (this._housesToTest.Count > 0)
			{
				HouseView house = this._housesToTest[0];
				this._housesToTest.RemoveAt(0);
				this.UpdateHouseConnectivity(house);
			}
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x000524D1 File Offset: 0x000506D1
		public void OnLanesAdded()
		{
			this.TestDisconnectedBuildings();
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00052617 File Offset: 0x00050817
		public void OnLanesReleased()
		{
			this.TestConnectedBuildings();
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x0005261F File Offset: 0x0005081F
		public void OnDestinationAdded()
		{
			this._testDisconnectedBuildingsDuringNextTick = true;
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00052628 File Offset: 0x00050828
		public void OnCarparkRemoved(CarparkModel carparkModel)
		{
			carparkModel.Unsubscribe(this);
			this.TestConnectedBuildings();
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00052638 File Offset: 0x00050838
		public void OnCarparkAdded(CarparkModel carparkModel)
		{
			if (carparkModel.SupportsTwoDestinations)
			{
				carparkModel.Subscribe(this);
			}
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x0005264C File Offset: 0x0005084C
		private void TestDisconnectedBuildings()
		{
			foreach (HouseView house in this._viewIndex.HouseViews)
			{
				if (house.NetworkConnectivity == NetworkConnectivity.Disconnected)
				{
					this.TestHouse(house);
				}
			}
			foreach (DestinationView destination in this._viewIndex.DestinationViews)
			{
				if (destination.NetworkConnectivity == NetworkConnectivity.Disconnected)
				{
					this.TestDestination(destination);
				}
			}
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x000526F4 File Offset: 0x000508F4
		private void TestConnectedBuildings()
		{
			foreach (HouseView house in this._viewIndex.HouseViews)
			{
				if (house.NetworkConnectivity == NetworkConnectivity.Connected)
				{
					bool houseHasPath = false;
					foreach (VehicleModel vehicle in house.Model.ownedVehicles)
					{
						if (vehicle.behaviorState != VehicleModel.BehaviorState.WaitingForDestination && vehicle.behaviorState != VehicleModel.BehaviorState.DrivingHome)
						{
							houseHasPath = true;
							break;
						}
					}
					if (!houseHasPath)
					{
						this.TestHouse(house);
					}
				}
			}
			foreach (DestinationView destination in this._viewIndex.DestinationViews)
			{
				if (destination.NetworkConnectivity == NetworkConnectivity.Connected)
				{
					this.TestDestination(destination);
				}
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00052800 File Offset: 0x00050A00
		private void UpdateDestinationConnectivity(DestinationView destination)
		{
			if (!destination.gameObject.activeInHierarchy)
			{
				return;
			}
			int groupIndex = destination.groupIndex;
			for (int houseIndex = 0; houseIndex < this._housesToTest.Count; houseIndex++)
			{
				HouseView house = this._housesToTest[houseIndex];
				if (house.groupIndex == groupIndex && this.AreHouseAndDestinationConnected(house.Model, destination.Model))
				{
					house.NetworkConnectivity = NetworkConnectivity.Connected;
					destination.NetworkConnectivity = NetworkConnectivity.Connected;
					this._housesToTest.RemoveAt(houseIndex);
					return;
				}
			}
			foreach (HouseModel houseModel in this._simulation.GetModels<HouseModel>())
			{
				if (houseModel.GroupIndex == groupIndex && !this.WillTestHouse(houseModel) && this.AreHouseAndDestinationConnected(houseModel, destination.Model))
				{
					destination.NetworkConnectivity = NetworkConnectivity.Connected;
					return;
				}
			}
			destination.NetworkConnectivity = NetworkConnectivity.Disconnected;
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x000528DC File Offset: 0x00050ADC
		private void UpdateHouseConnectivity(HouseView house)
		{
			int groupIndex = house.groupIndex;
			for (int destinationIndex = 0; destinationIndex < this._destinationsToTest.Count; destinationIndex++)
			{
				DestinationView destination = this._destinationsToTest[destinationIndex];
				if (destination.groupIndex == groupIndex && this.AreHouseAndDestinationConnected(house.Model, destination.Model))
				{
					house.NetworkConnectivity = NetworkConnectivity.Connected;
					destination.NetworkConnectivity = NetworkConnectivity.Connected;
					this._destinationsToTest.RemoveAt(destinationIndex);
					return;
				}
			}
			foreach (DestinationModel destinationModel in this._simulation.GetModels<DestinationModel>())
			{
				if (destinationModel.isActive && destinationModel.GroupIndex == groupIndex && !this.WillTestDestination(destinationModel) && this.AreHouseAndDestinationConnected(house.Model, destinationModel))
				{
					house.NetworkConnectivity = NetworkConnectivity.Connected;
					return;
				}
			}
			house.NetworkConnectivity = NetworkConnectivity.Disconnected;
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000529B0 File Offset: 0x00050BB0
		private bool AreHouseAndDestinationConnected(HouseModel house, DestinationModel destination)
		{
			LaneModel drivewayLane = house.DrivewayLane;
			return Diagnostics.Verify(drivewayLane != null, "House on tile {0} does not have a valid driveway!", house.tileModel) && this._pathfinder.AreLanesConnected(drivewayLane, destination.Carpark.entranceLanes, true);
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x000529F4 File Offset: 0x00050BF4
		private bool WillTestHouse(HouseModel houseModel)
		{
			using (List<HouseView>.Enumerator enumerator = this._housesToTest.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Model == houseModel)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00052A50 File Offset: 0x00050C50
		private bool WillTestDestination(DestinationModel destinationModel)
		{
			using (List<DestinationView>.Enumerator enumerator = this._destinationsToTest.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Model == destinationModel)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04001355 RID: 4949
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001356 RID: 4950
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x04001357 RID: 4951
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x04001358 RID: 4952
		private bool _hasSubscribedToCity;

		// Token: 0x04001359 RID: 4953
		private List<HouseView> _housesToTest = new List<HouseView>();

		// Token: 0x0400135A RID: 4954
		private List<DestinationView> _destinationsToTest = new List<DestinationView>();

		// Token: 0x0400135B RID: 4955
		private bool _testDisconnectedBuildingsDuringNextTick;
	}
}
