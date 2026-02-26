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
	// Token: 0x02000646 RID: 1606
	public static class Get
	{
		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06002CD8 RID: 11480 RVA: 0x000CF782 File Offset: 0x000CD982
		public static AudioLoadout Loadout
		{
			get
			{
				AudioEnvironment instance = AudioEnvironment.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.Loadout;
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06002CD9 RID: 11481 RVA: 0x000CF794 File Offset: 0x000CD994
		public static AudioEnvironment Environment
		{
			get
			{
				return AudioEnvironment.Instance;
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06002CDA RID: 11482 RVA: 0x000CF79B File Offset: 0x000CD99B
		public static AudioMixbus Mixbus
		{
			get
			{
				return AudioSystem.Mixbus;
			}
		}

		// Token: 0x06002CDB RID: 11483 RVA: 0x000CF7A4 File Offset: 0x000CD9A4
		public static bool HasAny<TEnum>(this TEnum state, params TEnum[] options) where TEnum : Enum
		{
			foreach (TEnum t in options)
			{
				if (state.HasFlag(t))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002CDC RID: 11484 RVA: 0x000CF7E4 File Offset: 0x000CD9E4
		public static bool HasAll<TEnum>(this TEnum state, params TEnum[] options) where TEnum : Enum
		{
			int trueCount = 0;
			foreach (TEnum t in options)
			{
				if (state.HasFlag(t))
				{
					trueCount++;
				}
			}
			return trueCount == options.Length;
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06002CDD RID: 11485 RVA: 0x000CF82A File Offset: 0x000CDA2A
		public static City City
		{
			get
			{
				AudioEnvironment instance = AudioEnvironment.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.City;
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06002CDE RID: 11486 RVA: 0x000CF83C File Offset: 0x000CDA3C
		public static ClockModel Clock
		{
			get
			{
				AudioEnvironment instance = AudioEnvironment.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.ClockModel;
			}
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06002CDF RID: 11487 RVA: 0x000CF84E File Offset: 0x000CDA4E
		public static CameraView Camera
		{
			get
			{
				AudioEnvironment instance = AudioEnvironment.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.CameraView;
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06002CE0 RID: 11488 RVA: 0x000CF860 File Offset: 0x000CDA60
		public static TilemapView TilemapView
		{
			get
			{
				AudioEnvironment instance = AudioEnvironment.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.TilemapView;
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x06002CE1 RID: 11489 RVA: 0x000CF872 File Offset: 0x000CDA72
		public static MotorwaysGame Game
		{
			get
			{
				return AudioEnvironment.Game;
			}
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06002CE2 RID: 11490 RVA: 0x000CF879 File Offset: 0x000CDA79
		public static SimulationConstantsData GameConstants
		{
			get
			{
				return Get.Game.Scope.Get<SimulationConstantsData>();
			}
		}

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x06002CE3 RID: 11491 RVA: 0x000CF88A File Offset: 0x000CDA8A
		public static int Hour
		{
			get
			{
				ClockModel clock = Get.Clock;
				return ((clock != null) ? clock.Hour : 24) % 24;
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x06002CE4 RID: 11492 RVA: 0x000CF8A1 File Offset: 0x000CDAA1
		public static int Day
		{
			get
			{
				ClockModel clock = Get.Clock;
				return ((clock != null) ? clock.Day : 7) % 7;
			}
		}

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x06002CE5 RID: 11493 RVA: 0x000CF8B6 File Offset: 0x000CDAB6
		public static bool IsDaytime
		{
			get
			{
				return Maf.IsWithin((float)Get.Hour, 6, 18);
			}
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06002CE6 RID: 11494 RVA: 0x000CF8C6 File Offset: 0x000CDAC6
		public static int Week
		{
			get
			{
				ClockModel clock = Get.Clock;
				if (clock == null)
				{
					return 0;
				}
				return clock.Week;
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06002CE7 RID: 11495 RVA: 0x000CF8D8 File Offset: 0x000CDAD8
		public static float WeekProgress
		{
			get
			{
				return ((float)Get.Day + (float)Get.Hour / 23f) / 7f;
			}
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x000CF8F4 File Offset: 0x000CDAF4
		public static float FacingDegrees(IAudioView view)
		{
			return view.transform.rotation.eulerAngles.z;
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x000CF919 File Offset: 0x000CDB19
		public static float NormBiDeltaAngle(IAudioView from, IAudioView to)
		{
			return Mathf.DeltaAngle(Get.FacingDegrees(from), Get.FacingDegrees(to)) / 180f;
		}

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06002CEA RID: 11498 RVA: 0x000CF932 File Offset: 0x000CDB32
		public static int AudibleGroups
		{
			get
			{
				return AudioEnvironment.Instance.AudibleGroups;
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06002CEB RID: 11499 RVA: 0x000CF93E File Offset: 0x000CDB3E
		public static int MaxGroups
		{
			get
			{
				if (Get.Game.StartedWithGameMode != GameMode.Normal)
				{
					return 5;
				}
				return Get.City.Definition.schedulePlanner.scheduleGroups.Count;
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06002CEC RID: 11500 RVA: 0x000CF968 File Offset: 0x000CDB68
		public static float Zoom
		{
			get
			{
				City city = Get.City;
				CameraView cam = Get.Camera;
				if (city == null || city.Rules.DoesIgnorePlayableArea() || cam == null)
				{
					return Settings.Attenuation.Zoom.MENU;
				}
				float alpha = Maf.Normalize(cam.playerOrthoZoom, (float)city.Definition.cameraZoom.endSize, cam.MinZoom, true);
				return Mathf.Lerp(Settings.Attenuation.Zoom.DYNAMIC_RANGE.x, Settings.Attenuation.Zoom.DYNAMIC_RANGE.y, Maf.VolCurve(alpha));
			}
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06002CED RID: 11501 RVA: 0x000CF9E2 File Offset: 0x000CDBE2
		public static float ZoomSmooth
		{
			get
			{
				AudioEnvironment instance = AudioEnvironment.Instance;
				if (instance == null)
				{
					return 1f;
				}
				return instance.ZoomSmooth;
			}
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06002CEE RID: 11502 RVA: 0x000CF9F8 File Offset: 0x000CDBF8
		public static float ZoomOutProgress
		{
			get
			{
				City city = Get.City;
				CameraView cam = Get.Camera;
				if (city == null || city.Rules.DoesIgnorePlayableArea() || cam == null)
				{
					return 0f;
				}
				return Maf.Normalize(cam.FixedZoom, (float)city.Definition.cameraZoom.startSize, (float)city.Definition.cameraZoom.endSize, true);
			}
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x000CFA64 File Offset: 0x000CDC64
		public static Vector2 Pan(Vector2 screenPos)
		{
			float xBorderFactor = 1.5f;
			float value = Maf.Normalize(screenPos.x, (float)(-1 * Screen.width) * xBorderFactor, (float)Screen.width + (float)Screen.width * xBorderFactor, true);
			float y = Mathf.Clamp01(screenPos.y / (float)Screen.height);
			return new Vector2(Mathf.Clamp01(value), y);
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000CFABA File Offset: 0x000CDCBA
		public static float PanX(Vector2 screenPos)
		{
			return Get.Pan(screenPos).x;
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000CFAC8 File Offset: 0x000CDCC8
		public static float Attenuation(Vector2 screenPos, bool zoom = true, float falloffFactor = 5f)
		{
			float distanceBetweenTiles = 100f;
			if (Get.City != null)
			{
				TilemapView tilemapView = Get.TilemapView;
				distanceBetweenTiles = ((tilemapView != null) ? tilemapView.ScreenDistanceBetweenTiles : 100f);
			}
			float w = (float)Screen.width / distanceBetweenTiles;
			float h = (float)Screen.height / distanceBetweenTiles;
			Vector2 sourcePos = new Vector2(screenPos.x / distanceBetweenTiles, screenPos.y / distanceBetweenTiles);
			float x;
			if (sourcePos.x > w)
			{
				x = Maf.Map(sourcePos.x, w, w + falloffFactor, 1f, 0f);
			}
			else if (screenPos.x < 0f)
			{
				x = Maf.Map(sourcePos.x, 0f, 0f - falloffFactor, 1f, 0f);
			}
			else
			{
				x = 1f;
			}
			float y;
			if (sourcePos.y > h)
			{
				y = Maf.Map(sourcePos.y, h, h + falloffFactor, 1f, 0f);
			}
			else if (screenPos.y < 0f)
			{
				y = Maf.Map(sourcePos.y, 0f, 0f - falloffFactor, 1f, 0f);
			}
			else
			{
				y = (sourcePos.y = 1f);
			}
			float xy = Maf.VolCurve(x * y);
			if (!zoom)
			{
				return xy;
			}
			return Get.ZoomSmooth * xy;
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x000CFC08 File Offset: 0x000CDE08
		public static int ConnectedViewCount()
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			int viewCount = 0;
			foreach (DestinationModel destinationModel in Get.City.Scope.Get<ISimulation>().GetModels<DestinationModel>())
			{
				DestinationView v = viewIndex.GetDestinationView(destinationModel);
				if (v != null && v.NetworkConnectivity == NetworkConnectivity.Connected)
				{
					viewCount++;
				}
			}
			foreach (HouseModel houseModel in Get.City.Scope.Get<ISimulation>().GetModels<HouseModel>())
			{
				HouseView v2 = viewIndex.GetHouseView(houseModel);
				if (v2 != null && v2.NetworkConnectivity == NetworkConnectivity.Connected)
				{
					viewCount++;
				}
			}
			return viewCount;
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x000CFCD0 File Offset: 0x000CDED0
		public static void AddDestinationsInto(List<List<DestinationView>> outResults)
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			foreach (DestinationModel destinationModel in Get.City.Scope.Get<ISimulation>().GetModels<DestinationModel>())
			{
				DestinationView v = viewIndex.GetDestinationView(destinationModel);
				if (!(v == null) && v.Model.isActive && v.groupIndex > -1)
				{
					Get.ExtendListToFitIndex<DestinationView>(outResults, v.groupIndex);
					outResults[v.groupIndex].Add(v);
				}
			}
		}

		// Token: 0x06002CF4 RID: 11508 RVA: 0x000CFD6C File Offset: 0x000CDF6C
		public static void AddDisconnectedsInto(List<List<IAudioView>> outResults)
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			foreach (DestinationModel destinationModel in Get.City.Scope.Get<ISimulation>().GetModels<DestinationModel>())
			{
				DestinationView v = viewIndex.GetDestinationView(destinationModel);
				if (!(v == null) && v.NetworkConnectivity != NetworkConnectivity.Connected && v.groupIndex > -1)
				{
					Get.ExtendListToFitIndex<IAudioView>(outResults, v.groupIndex);
					outResults[v.groupIndex].Add(v);
				}
			}
			foreach (HouseModel houseModel in Get.City.Scope.Get<ISimulation>().GetModels<HouseModel>())
			{
				HouseView v2 = viewIndex.GetHouseView(houseModel);
				if (!(v2 == null) && v2.NetworkConnectivity != NetworkConnectivity.Connected && v2.groupIndex > -1)
				{
					Get.ExtendListToFitIndex<IAudioView>(outResults, v2.groupIndex);
					outResults[v2.groupIndex].Add(v2);
				}
			}
		}

		// Token: 0x06002CF5 RID: 11509 RVA: 0x000CFE80 File Offset: 0x000CE080
		public static void AddHousesInto(List<List<HouseView>> results)
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			foreach (HouseModel houseModel in Get.City.Scope.Get<ISimulation>().GetModels<HouseModel>())
			{
				HouseView v = viewIndex.GetHouseView(houseModel);
				if (!(v == null) && v.NetworkConnectivity == NetworkConnectivity.Connected && v.groupIndex > -1)
				{
					Get.ExtendListToFitIndex<HouseView>(results, v.groupIndex);
					results[v.groupIndex].Add(v);
				}
			}
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x000CFF18 File Offset: 0x000CE118
		public static void AddVehiclesInto(List<List<VehicleView>> results)
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			foreach (VehicleModel vehicleModel in Get.City.Scope.Get<ISimulation>().GetModels<VehicleModel>())
			{
				VehicleView v = viewIndex.GetVehicleView(vehicleModel);
				if (!(v == null) && v.groupIndex > -1)
				{
					Get.ExtendListToFitIndex<VehicleView>(results, v.groupIndex);
					results[v.groupIndex].Add(v);
				}
			}
		}

		// Token: 0x06002CF7 RID: 11511 RVA: 0x000CFFA4 File Offset: 0x000CE1A4
		public static void AddBoatsInto(List<BoatView> results)
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			foreach (BoatModel trainModel in Get.City.Scope.Get<ISimulation>().GetModels<BoatModel>())
			{
				BoatView v = viewIndex.GetBoatView(trainModel);
				if (!(v == null))
				{
					results.Add(v);
				}
			}
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x000D000C File Offset: 0x000CE20C
		public static void AddTrainsInto(List<TrainView> results)
		{
			ViewIndex viewIndex = Get.City.Scope.Get<ViewIndex>();
			foreach (TrainModel trainModel in Get.City.Scope.Get<ISimulation>().GetModels<TrainModel>())
			{
				TrainView v = viewIndex.GetTrainView(trainModel);
				if (!(v == null))
				{
					results.Add(v);
				}
			}
		}

		// Token: 0x06002CF9 RID: 11513 RVA: 0x000D0074 File Offset: 0x000CE274
		private static void ExtendListToFitIndex<T>(List<List<T>> container, int toFitIndex)
		{
			while (container.Count < toFitIndex + 1)
			{
				container.Add(new List<T>());
			}
		}

		// Token: 0x0400274F RID: 10063
		public static StateType State;

		// Token: 0x02000647 RID: 1607
		public static class Pulse
		{
			// Token: 0x170007BC RID: 1980
			// (get) Token: 0x06002CFA RID: 11514 RVA: 0x000D008E File Offset: 0x000CE28E
			// (set) Token: 0x06002CFB RID: 11515 RVA: 0x000D009A File Offset: 0x000CE29A
			public static TimeScale Scale
			{
				get
				{
					return AudioSystem.Instance.ScheduledPulseTimeScale;
				}
				set
				{
					AudioSystem.Instance.ScheduledPulseTimeScale = value;
				}
			}

			// Token: 0x06002CFC RID: 11516 RVA: 0x000D00A8 File Offset: 0x000CE2A8
			public static double HybridTime(PulsedAudioModule module)
			{
				double nextPulseDelta = module.NextPulseTime - AudioPlayer.EarliestSchedulableTime;
				if (nextPulseDelta >= Get.Pulse.Master.Duration * 0.125 || Math.Sign(nextPulseDelta) <= 0)
				{
					return -1.0;
				}
				return module.NextPulseTime;
			}

			// Token: 0x06002CFD RID: 11517 RVA: 0x000D00ED File Offset: 0x000CE2ED
			public static float Subduration(int divisor)
			{
				return (float)Get.Pulse.Master.Duration / (float)divisor;
			}

			// Token: 0x06002CFE RID: 11518 RVA: 0x000D00F8 File Offset: 0x000CE2F8
			public static float Subduration(params int[] divisorChoices)
			{
				return (float)Rando.Pick<int>(divisorChoices);
			}

			// Token: 0x06002CFF RID: 11519 RVA: 0x000D0101 File Offset: 0x000CE301
			public static float SubdurationMs(int divisor)
			{
				return 1000f * Get.Pulse.Subduration(divisor);
			}

			// Token: 0x06002D00 RID: 11520 RVA: 0x000D010F File Offset: 0x000CE30F
			public static float Duratio(float factor)
			{
				return (float)Get.Pulse.Master.Duration * factor;
			}

			// Token: 0x06002D01 RID: 11521 RVA: 0x000D0119 File Offset: 0x000CE319
			public static float Duratio(params float[] factorChoices)
			{
				return Rando.Pick<float>(factorChoices);
			}

			// Token: 0x06002D02 RID: 11522 RVA: 0x000D0124 File Offset: 0x000CE324
			public static double QuantizedTime(double pulseDivisor)
			{
				double quantizedPulseDuration = Get.Pulse.Master.Duration / pulseDivisor;
				double candidate;
				for (candidate = Get.Pulse.Master.Next - Get.Pulse.Master.Duration; candidate < AudioPlayer.EarliestSchedulableTime; candidate += quantizedPulseDuration)
				{
				}
				return candidate;
			}

			// Token: 0x02000648 RID: 1608
			public static class Master
			{
				// Token: 0x170007BD RID: 1981
				// (get) Token: 0x06002D03 RID: 11523 RVA: 0x000D0154 File Offset: 0x000CE354
				public static double Next
				{
					get
					{
						return AudioSystem.Instance.Database.MasterPulse.PulseInfo.PulseDspTime + Get.Pulse.Master.Duration;
					}
				}

				// Token: 0x170007BE RID: 1982
				// (get) Token: 0x06002D04 RID: 11524 RVA: 0x000D0175 File Offset: 0x000CE375
				public static double Duration
				{
					get
					{
						return AudioSystem.Instance.Database.MasterPulse.PulseInfo.PulseDuration;
					}
				}
			}
		}
	}
}
