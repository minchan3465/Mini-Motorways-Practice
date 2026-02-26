using System;
using System.Collections.Generic;
using Motorways.Models;
using Motorways.Views;
using Motorways.Views.Boats;
using Motorways.Views.Trains;
using Server;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000638 RID: 1592
	public class AudioEnvironment
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06002C6D RID: 11373 RVA: 0x000CE7DB File Offset: 0x000CC9DB
		// (set) Token: 0x06002C6E RID: 11374 RVA: 0x000CE7E3 File Offset: 0x000CC9E3
		public City City { get; private set; }

		// Token: 0x06002C6F RID: 11375 RVA: 0x000CE7EC File Offset: 0x000CC9EC
		public AudioEnvironment(AudioLoadout loadout, City city, MotorwaysGame game)
		{
			this.City = city;
			AudioEnvironment.Instance = this;
			AudioEnvironment.Game = game;
			this.Loadout = loadout;
			this.Active = true;
		}

		// Token: 0x06002C70 RID: 11376 RVA: 0x000CE862 File Offset: 0x000CCA62
		public void Kill()
		{
			this.Loadout.Deactivate();
			this.Loadout = null;
			this.Active = false;
		}

		// Token: 0x06002C71 RID: 11377 RVA: 0x000CE880 File Offset: 0x000CCA80
		public int GetPinCount(int groupIndex = -1)
		{
			int pinCount = 0;
			if (this.Destinations == null)
			{
				return 0;
			}
			for (int i = 0; i < this.Destinations.Count; i++)
			{
				if (groupIndex == -1 || groupIndex == i)
				{
					foreach (DestinationView v in this.Destinations[i])
					{
						pinCount += v.PinCount;
					}
				}
			}
			return pinCount;
		}

		// Token: 0x06002C72 RID: 11378 RVA: 0x000CE908 File Offset: 0x000CCB08
		public int GetDisconnectedCount(int groupIndex = -1)
		{
			int disCount = 0;
			ViewIndex viewIndex = this.City.Scope.Get<ViewIndex>();
			foreach (DestinationModel destinationModel in this.City.Scope.Get<ISimulation>().GetModels<DestinationModel>())
			{
				DestinationView v = viewIndex.GetDestinationView(destinationModel);
				if (!(v == null) && (v.groupIndex == groupIndex || groupIndex == -1) && v.NetworkConnectivity == NetworkConnectivity.Disconnected)
				{
					disCount++;
				}
			}
			foreach (HouseModel houseModel in this.City.Scope.Get<ISimulation>().GetModels<HouseModel>())
			{
				HouseView v2 = viewIndex.GetHouseView(houseModel);
				if (!(v2 == null) && (v2.groupIndex == groupIndex || groupIndex == -1) && v2.NetworkConnectivity == NetworkConnectivity.Disconnected)
				{
					disCount++;
				}
			}
			return disCount;
		}

		// Token: 0x06002C73 RID: 11379 RVA: 0x000CE9F0 File Offset: 0x000CCBF0
		public int GetAudibleGroups()
		{
			int audibleGroups = 0;
			using (List<DestinationGroup>.Enumerator enumerator = this.Loadout.DestinationGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ViewsCount > 0)
					{
						audibleGroups++;
					}
				}
			}
			return audibleGroups;
		}

		// Token: 0x06002C74 RID: 11380 RVA: 0x000CEA50 File Offset: 0x000CCC50
		private void UpdateCityData()
		{
			AudioEnvironment.Clear<DestinationView>(this.Destinations);
			Get.AddDestinationsInto(this.Destinations);
			AudioEnvironment.Clear<HouseView>(this.Houses);
			Get.AddHousesInto(this.Houses);
			AudioEnvironment.Clear<VehicleView>(this.Vehicles);
			Get.AddVehiclesInto(this.Vehicles);
			AudioEnvironment.Clear<IAudioView>(this.Disconnecteds);
			Get.AddDisconnectedsInto(this.Disconnecteds);
			this.Trains.Clear();
			Get.AddTrainsInto(this.Trains);
			this.Boats.Clear();
			Get.AddBoatsInto(this.Boats);
			this.AudibleGroups = this.GetAudibleGroups();
			if (this.ZoomSmooth != Get.Zoom)
			{
				this.zoomElapsed += Time.deltaTime;
				this.ZoomSmooth = Mathf.Lerp(this.ZoomSmooth, Get.Zoom, this.zoomElapsed / 0.25f);
			}
			else
			{
				this.zoomElapsed = 0f;
			}
			if (!this.lateGame && this.AudibleGroups == Get.MaxGroups)
			{
				AudioEvent.CreateEvent(-1.0, AudioEventType.LateGame, 0.5f, -1f, true, null);
				Get.State |= StateType.LateGame;
				this.lateGame = true;
				return;
			}
			Get.State &= ~StateType.LateGame;
		}

		// Token: 0x06002C75 RID: 11381 RVA: 0x000CEB9C File Offset: 0x000CCD9C
		private static void Clear<T>(List<List<T>> container)
		{
			for (int groupIndex = 0; groupIndex < container.Count; groupIndex++)
			{
				container[groupIndex].Clear();
			}
			while (container.Count < Get.MaxGroups)
			{
				container.Add(new List<T>());
			}
		}

		// Token: 0x06002C76 RID: 11382 RVA: 0x000CEBE0 File Offset: 0x000CCDE0
		public void Update()
		{
			if (this.Active)
			{
				this.TimeDelta = Time.deltaTime;
				this.TimeElapsed += this.TimeDelta;
				this.UpdateCityData();
				if (!this.loadoutActivated)
				{
					this.Loadout.Activate(this);
					this.loadoutActivated = true;
					FX.ToggleNightMode(Get.State.HasFlag(StateType.ModeNight), true);
					IAudioSystem audioSystem = AudioSystem.Instance;
					audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.CityStart, 0.5f, -1f, true, null));
					foreach (List<DestinationView> list in this.Destinations)
					{
						foreach (DestinationView destinationView in list)
						{
							audioSystem.ScheduleEvent(AudioEvent.CreateDestinationEvent(AudioEventType.DestinationActivated, destinationView, true));
						}
					}
				}
				AudioLoadout loadout = this.Loadout;
				if (loadout != null)
				{
					loadout.Update();
				}
				AudioLoadout persistentLoadout = AudioLoadout.PersistentLoadout;
				if (persistentLoadout == null)
				{
					return;
				}
				persistentLoadout.Update();
			}
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x06002C77 RID: 11383 RVA: 0x000CED2C File Offset: 0x000CCF2C
		public ClockModel ClockModel
		{
			get
			{
				if (this._clockModel == null)
				{
					this._clockModel = this.City.Scope.Get<Simulation>().GetModel<ClockModel>();
				}
				return this._clockModel;
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x06002C78 RID: 11384 RVA: 0x000CED57 File Offset: 0x000CCF57
		public CameraView CameraView
		{
			get
			{
				if (this._cameraView == null)
				{
					this._cameraView = this.City.Scope.Get<CameraView>();
				}
				return this._cameraView;
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06002C79 RID: 11385 RVA: 0x000CED7D File Offset: 0x000CCF7D
		public TilemapView TilemapView
		{
			get
			{
				if (this._tilemapView == null)
				{
					this._tilemapView = this.City.Scope.Get<TilemapView>();
				}
				return this._tilemapView;
			}
		}

		// Token: 0x04002698 RID: 9880
		public AudioLoadout Loadout;

		// Token: 0x04002699 RID: 9881
		public static AudioEnvironment Instance;

		// Token: 0x0400269A RID: 9882
		public static MotorwaysGame Game;

		// Token: 0x0400269B RID: 9883
		private ClockModel _clockModel;

		// Token: 0x0400269C RID: 9884
		private CameraView _cameraView;

		// Token: 0x0400269D RID: 9885
		private TilemapView _tilemapView;

		// Token: 0x0400269E RID: 9886
		public bool Active;

		// Token: 0x0400269F RID: 9887
		public readonly List<List<VehicleView>> Vehicles = new List<List<VehicleView>>();

		// Token: 0x040026A0 RID: 9888
		public readonly List<List<HouseView>> Houses = new List<List<HouseView>>();

		// Token: 0x040026A1 RID: 9889
		public readonly List<TrainView> Trains = new List<TrainView>();

		// Token: 0x040026A2 RID: 9890
		public readonly List<BoatView> Boats = new List<BoatView>();

		// Token: 0x040026A3 RID: 9891
		public readonly List<List<DestinationView>> Destinations = new List<List<DestinationView>>();

		// Token: 0x040026A4 RID: 9892
		public readonly List<List<IAudioView>> Disconnecteds = new List<List<IAudioView>>();

		// Token: 0x040026A5 RID: 9893
		public float ZoomSmooth;

		// Token: 0x040026A6 RID: 9894
		public int AudibleGroups;

		// Token: 0x040026A7 RID: 9895
		public int BlockedDestinations;

		// Token: 0x040026A8 RID: 9896
		public float TimeElapsed;

		// Token: 0x040026A9 RID: 9897
		public float TimeDelta;

		// Token: 0x040026AA RID: 9898
		private bool lateGame;

		// Token: 0x040026AB RID: 9899
		private float zoomElapsed;

		// Token: 0x040026AC RID: 9900
		private bool loadoutActivated;
	}
}
