using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;

// Token: 0x0200019A RID: 410
[Factory.Serializable(1)]
public class IntersectionEntryVehicleContext : IReusable
{
	// Token: 0x0600093C RID: 2364 RVA: 0x0001DFE0 File Offset: 0x0001C1E0
	public void Initialize(VehicleModel vehicle)
	{
		this._vehicle = vehicle;
		this._lane = this._vehicle.CurrentFrame.lane;
		this._speed = this._vehicle.CurrentFrame.speed;
		this._distanceAlongLane = this._vehicle.CurrentFrame.distanceAlongLane;
		this._influence = IntersectionEntryVehicleInfluence.Unknown;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0001E03D File Offset: 0x0001C23D
	public void Reset()
	{
		this._vehicle = null;
		this._lane = null;
		this._speed = default(Fix64);
		this._distanceAlongLane = default(Fix64);
		this._influence = IntersectionEntryVehicleInfluence.Unknown;
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600093E RID: 2366 RVA: 0x0001E06C File Offset: 0x0001C26C
	public VehicleModel Vehicle
	{
		get
		{
			return this._vehicle;
		}
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600093F RID: 2367 RVA: 0x0001E074 File Offset: 0x0001C274
	public Fix64 Speed
	{
		get
		{
			return this._speed;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x06000940 RID: 2368 RVA: 0x0001E07C File Offset: 0x0001C27C
	public LaneModel Lane
	{
		get
		{
			return this._lane;
		}
	}

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06000941 RID: 2369 RVA: 0x0001E084 File Offset: 0x0001C284
	public Fix64 DistanceAlongLane
	{
		get
		{
			return this._distanceAlongLane;
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06000942 RID: 2370 RVA: 0x0001E08C File Offset: 0x0001C28C
	// (set) Token: 0x06000943 RID: 2371 RVA: 0x0001E094 File Offset: 0x0001C294
	public IntersectionEntryVehicleInfluence Influence
	{
		get
		{
			return this._influence;
		}
		set
		{
			this._influence = value;
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06000944 RID: 2372 RVA: 0x0001E09D File Offset: 0x0001C29D
	public bool WasBlocking
	{
		get
		{
			return this._influence == IntersectionEntryVehicleInfluence.OnIntersectingLane || this._influence == IntersectionEntryVehicleInfluence.SameExitNoSpace || this._influence == IntersectionEntryVehicleInfluence.ReservedIntersectingLane;
		}
	}

	// Token: 0x040004AD RID: 1197
	private VehicleModel _vehicle;

	// Token: 0x040004AE RID: 1198
	private LaneModel _lane;

	// Token: 0x040004AF RID: 1199
	private Fix64 _speed;

	// Token: 0x040004B0 RID: 1200
	private Fix64 _distanceAlongLane;

	// Token: 0x040004B1 RID: 1201
	private IntersectionEntryVehicleInfluence _influence;
}
