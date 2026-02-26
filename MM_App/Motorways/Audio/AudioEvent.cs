using System;
using Motorways.Views;
using Motorways.Views.Trains;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Motorways.Audio
{
	// Token: 0x0200063D RID: 1597
	public class AudioEvent
	{
		// Token: 0x06002C8C RID: 11404 RVA: 0x000CF01C File Offset: 0x000CD21C
		private AudioEvent(double dspTime, AudioEventType type)
		{
			this.Id = AudioEvent.nextId;
			AudioEvent.nextId++;
			this.UIEventType = UIEventType.None;
			this.Screen = ScreenStack.MotorwaysScreen.None;
			this.PreviousScreen = ScreenStack.MotorwaysScreen.None;
			this.Duration = -1f;
			this.DspTime = dspTime;
			this.Pan = 0.5f;
			this.Vehicle = null;
			this.Motorway = null;
			this.Train = null;
			this.TrafficLight = null;
			this.Type = type;
			this.IsPaused = false;
		}

		// Token: 0x06002C8D RID: 11405 RVA: 0x000CF0A2 File Offset: 0x000CD2A2
		public static AudioEvent CreateEvent(double dspTime, AudioEventType type, float pan = 0.5f, float duration = -1f, bool condition = true, City city = null)
		{
			return new AudioEvent(dspTime, type)
			{
				Pan = pan,
				Duration = duration,
				Condition = condition,
				City = city
			};
		}

		// Token: 0x06002C8E RID: 11406 RVA: 0x000CF0C9 File Offset: 0x000CD2C9
		public static AudioEvent CreateTrainEvent(double dspTime, AudioEventType type, TrainView train)
		{
			return new AudioEvent(dspTime, type)
			{
				Train = train
			};
		}

		// Token: 0x06002C8F RID: 11407 RVA: 0x000CF0D9 File Offset: 0x000CD2D9
		public static AudioEvent CreateDestinationEvent(AudioEventType type, DestinationView destination, bool condition = true)
		{
			return new AudioEvent(-1.0, type)
			{
				City = destination.City,
				Destination = destination,
				Condition = condition
			};
		}

		// Token: 0x06002C90 RID: 11408 RVA: 0x000CF104 File Offset: 0x000CD304
		public static AudioEvent CreateHouseEvent(AudioEventType type, HouseView house, bool condition = true)
		{
			return new AudioEvent(-1.0, type)
			{
				City = house.City,
				House = house,
				Condition = condition
			};
		}

		// Token: 0x06002C91 RID: 11409 RVA: 0x000CF130 File Offset: 0x000CD330
		public static AudioEvent CreateVehicleEvent(AudioEventType type, VehicleView vehicle, HouseView house = null, DestinationView destination = null, MotorwayView motorway = null)
		{
			return new AudioEvent(-1.0, type)
			{
				City = vehicle.City,
				Vehicle = vehicle,
				House = ((house == null) ? vehicle.House : house),
				Destination = ((destination == null) ? vehicle.Destination : destination),
				Motorway = motorway
			};
		}

		// Token: 0x06002C92 RID: 11410 RVA: 0x000CF197 File Offset: 0x000CD397
		public static AudioEvent CreateMotorwayEvent(AudioEventType type, MotorwayView motorway, float pan = 0.5f, float attenuation = 1f, float magnitude = 0f)
		{
			return new AudioEvent(-1.0, type)
			{
				City = motorway.City,
				Motorway = motorway,
				Pan = pan,
				Attenuation = attenuation,
				Magnitude = magnitude
			};
		}

		// Token: 0x06002C93 RID: 11411 RVA: 0x000CF1D1 File Offset: 0x000CD3D1
		public static AudioEvent CreateTrafficLightEvent(AudioEventType type, TrafficLightView trafficLight, TileDirectionBitfield rightOfWay)
		{
			return new AudioEvent(-1.0, type)
			{
				TrafficLight = trafficLight,
				City = trafficLight.City,
				Directions = rightOfWay
			};
		}

		// Token: 0x06002C94 RID: 11412 RVA: 0x000CF1FC File Offset: 0x000CD3FC
		public static AudioEvent CreateUpgradeEvent(AudioEventType type, UpgradeType upgradeType, bool success = true, MotorwayView motorway = null, Vector2 panXY = default(Vector2))
		{
			return new AudioEvent(-1.0, type)
			{
				UpgradeType = upgradeType,
				Condition = success,
				Motorway = motorway,
				PanXY = Get.Pan(panXY)
			};
		}

		// Token: 0x06002C95 RID: 11413 RVA: 0x000CF230 File Offset: 0x000CD430
		public static AudioEvent CreateUIEvent(UIEventType type, UIAudioProfile profile = UIAudioProfile.None, float duration = -1f, bool condition = true, PointerEventData data = null, ScreenStack.MotorwaysScreen screen = ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen previousScreen = ScreenStack.MotorwaysScreen.None)
		{
			return new AudioEvent(AudioSystem.Instance.DspTime, AudioEventType.UserInterface)
			{
				UIAudioProfile = profile,
				UIEventType = type,
				Duration = duration,
				Screen = screen,
				PreviousScreen = previousScreen,
				Condition = condition,
				Pan = ((data != null) ? Maf.Normalize(data.position[0], 0f, (float)UnityEngine.Screen.width, true) : 0.5f),
				PointerEventData = data
			};
		}

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06002C96 RID: 11414 RVA: 0x000CF2B8 File Offset: 0x000CD4B8
		// (set) Token: 0x06002C97 RID: 11415 RVA: 0x000CF2C0 File Offset: 0x000CD4C0
		public int Id { get; private set; }

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06002C98 RID: 11416 RVA: 0x000CF2C9 File Offset: 0x000CD4C9
		// (set) Token: 0x06002C99 RID: 11417 RVA: 0x000CF2D1 File Offset: 0x000CD4D1
		public AudioEventType Type { get; private set; }

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06002C9A RID: 11418 RVA: 0x000CF2DA File Offset: 0x000CD4DA
		// (set) Token: 0x06002C9B RID: 11419 RVA: 0x000CF2E2 File Offset: 0x000CD4E2
		public double DspTime { get; private set; }

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06002C9C RID: 11420 RVA: 0x000CF2EB File Offset: 0x000CD4EB
		// (set) Token: 0x06002C9D RID: 11421 RVA: 0x000CF2F3 File Offset: 0x000CD4F3
		public float Pan { get; private set; }

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06002C9E RID: 11422 RVA: 0x000CF2FC File Offset: 0x000CD4FC
		// (set) Token: 0x06002C9F RID: 11423 RVA: 0x000CF304 File Offset: 0x000CD504
		public float Attenuation { get; private set; }

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06002CA0 RID: 11424 RVA: 0x000CF30D File Offset: 0x000CD50D
		// (set) Token: 0x06002CA1 RID: 11425 RVA: 0x000CF315 File Offset: 0x000CD515
		public float Magnitude { get; private set; }

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06002CA2 RID: 11426 RVA: 0x000CF31E File Offset: 0x000CD51E
		// (set) Token: 0x06002CA3 RID: 11427 RVA: 0x000CF326 File Offset: 0x000CD526
		public Vector2 PanXY { get; private set; }

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06002CA4 RID: 11428 RVA: 0x000CF32F File Offset: 0x000CD52F
		// (set) Token: 0x06002CA5 RID: 11429 RVA: 0x000CF337 File Offset: 0x000CD537
		public bool IsPaused { get; private set; }

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06002CA6 RID: 11430 RVA: 0x000CF340 File Offset: 0x000CD540
		// (set) Token: 0x06002CA7 RID: 11431 RVA: 0x000CF348 File Offset: 0x000CD548
		public UIEventType UIEventType { get; private set; }

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06002CA8 RID: 11432 RVA: 0x000CF351 File Offset: 0x000CD551
		// (set) Token: 0x06002CA9 RID: 11433 RVA: 0x000CF359 File Offset: 0x000CD559
		public UIAudioProfile UIAudioProfile { get; private set; }

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06002CAA RID: 11434 RVA: 0x000CF362 File Offset: 0x000CD562
		// (set) Token: 0x06002CAB RID: 11435 RVA: 0x000CF36A File Offset: 0x000CD56A
		public ScreenStack.MotorwaysScreen Screen { get; private set; }

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06002CAC RID: 11436 RVA: 0x000CF373 File Offset: 0x000CD573
		// (set) Token: 0x06002CAD RID: 11437 RVA: 0x000CF37B File Offset: 0x000CD57B
		public ScreenStack.MotorwaysScreen PreviousScreen { get; private set; }

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06002CAE RID: 11438 RVA: 0x000CF384 File Offset: 0x000CD584
		// (set) Token: 0x06002CAF RID: 11439 RVA: 0x000CF38C File Offset: 0x000CD58C
		public float Duration { get; private set; }

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06002CB0 RID: 11440 RVA: 0x000CF395 File Offset: 0x000CD595
		// (set) Token: 0x06002CB1 RID: 11441 RVA: 0x000CF39D File Offset: 0x000CD59D
		public VehicleView Vehicle { get; private set; }

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06002CB2 RID: 11442 RVA: 0x000CF3A6 File Offset: 0x000CD5A6
		// (set) Token: 0x06002CB3 RID: 11443 RVA: 0x000CF3AE File Offset: 0x000CD5AE
		public TrainView Train { get; private set; }

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06002CB4 RID: 11444 RVA: 0x000CF3B7 File Offset: 0x000CD5B7
		// (set) Token: 0x06002CB5 RID: 11445 RVA: 0x000CF3BF File Offset: 0x000CD5BF
		public MotorwayView Motorway { get; private set; }

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06002CB6 RID: 11446 RVA: 0x000CF3C8 File Offset: 0x000CD5C8
		// (set) Token: 0x06002CB7 RID: 11447 RVA: 0x000CF3D0 File Offset: 0x000CD5D0
		public HouseView House { get; private set; }

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06002CB8 RID: 11448 RVA: 0x000CF3D9 File Offset: 0x000CD5D9
		// (set) Token: 0x06002CB9 RID: 11449 RVA: 0x000CF3E1 File Offset: 0x000CD5E1
		public DestinationView Destination { get; private set; }

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06002CBA RID: 11450 RVA: 0x000CF3EA File Offset: 0x000CD5EA
		public DestinationView NeighboringDestination
		{
			get
			{
				if (this.Destination != null)
				{
					return this.Destination.NeighboringDestination;
				}
				return null;
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06002CBB RID: 11451 RVA: 0x000CF407 File Offset: 0x000CD607
		// (set) Token: 0x06002CBC RID: 11452 RVA: 0x000CF40F File Offset: 0x000CD60F
		public City City { get; private set; }

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06002CBD RID: 11453 RVA: 0x000CF418 File Offset: 0x000CD618
		// (set) Token: 0x06002CBE RID: 11454 RVA: 0x000CF420 File Offset: 0x000CD620
		public UpgradeType UpgradeType { get; private set; }

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06002CBF RID: 11455 RVA: 0x000CF429 File Offset: 0x000CD629
		// (set) Token: 0x06002CC0 RID: 11456 RVA: 0x000CF431 File Offset: 0x000CD631
		public bool Condition { get; private set; }

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06002CC1 RID: 11457 RVA: 0x000CF43A File Offset: 0x000CD63A
		// (set) Token: 0x06002CC2 RID: 11458 RVA: 0x000CF442 File Offset: 0x000CD642
		public PointerEventData PointerEventData { get; private set; }

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06002CC3 RID: 11459 RVA: 0x000CF44B File Offset: 0x000CD64B
		// (set) Token: 0x06002CC4 RID: 11460 RVA: 0x000CF453 File Offset: 0x000CD653
		public TrafficLightView TrafficLight { get; private set; }

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06002CC5 RID: 11461 RVA: 0x000CF45C File Offset: 0x000CD65C
		// (set) Token: 0x06002CC6 RID: 11462 RVA: 0x000CF464 File Offset: 0x000CD664
		public TileDirectionBitfield Directions { get; private set; }

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06002CC7 RID: 11463 RVA: 0x000CF470 File Offset: 0x000CD670
		public int GroupIndex
		{
			get
			{
				if (this.House != null && this.House.Model != null)
				{
					return this.House.Model.GroupIndex;
				}
				if (this.Destination != null && this.Destination.Model != null)
				{
					return this.Destination.Model.GroupIndex;
				}
				if (this.Vehicle != null && this.Vehicle.Model != null)
				{
					return this.Vehicle.House.Model.GroupIndex;
				}
				return -1;
			}
		}

		// Token: 0x06002CC8 RID: 11464 RVA: 0x000CF508 File Offset: 0x000CD708
		public override string ToString()
		{
			string eventString = string.Format("[AudioEvent: Type={0}, DspTime={1}, Pan={2}, Id={3}", new object[]
			{
				this.Type,
				this.DspTime,
				this.Pan,
				this.Id
			});
			if (this.Vehicle != null)
			{
				eventString += string.Format(", Vehicle={0}", this.Vehicle);
			}
			if (this.House != null)
			{
				eventString += string.Format(", House={0}", this.House);
			}
			if (this.Destination != null)
			{
				eventString += string.Format(", Destination={0}", this.Destination);
			}
			if (this.TrafficLight != null)
			{
				eventString += string.Format(", TrafficLight={0}, Directions={1}", this.TrafficLight, this.Directions);
			}
			if (this.UIEventType != UIEventType.None)
			{
				eventString += string.Format(", UIEventType={0}, Duration={1}, Screen={2}, PreviousScreen={3}, UIAudioProfile={4}", new object[]
				{
					this.UIEventType,
					this.Duration,
					this.Screen,
					this.PreviousScreen,
					this.UIAudioProfile
				});
			}
			return eventString + "]";
		}

		// Token: 0x04002713 RID: 10003
		private static int nextId = 1;
	}
}
