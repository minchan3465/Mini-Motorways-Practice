using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using JetBrains.Annotations;
using Motorways.Models;
using Server;

// Token: 0x02000199 RID: 409
[Factory.Serializable(1)]
public class IntersectionEntryDecision : IReusable, IReleasedFromScopeHandler
{
	// Token: 0x06000923 RID: 2339 RVA: 0x0001DB98 File Offset: 0x0001BD98
	public void Initialize(RoadChunkModel.InboundVehicle inboundVehicle)
	{
		this._earliestFrameCount = this._clock.FrameCount;
		this._latestFrameCount = this._clock.FrameCount;
		this._vehicle = inboundVehicle.vehicle;
		this._currentLane = inboundVehicle.vehicle.CurrentFrame.lane;
		this._distanceAlongCurrentLane = inboundVehicle.vehicle.CurrentFrame.distanceAlongLane;
		this._targetLane = inboundVehicle.chosenLane;
		this._intersection = this._targetLane.roadChunk;
		this._waitTime = this._clock.Time - inboundVehicle.committedTimestamp;
		this._verdict = IntersectionEntryVerdict.Unknown;
		foreach (VehicleModel traversingVehicle in this._intersection.traversingVehicles)
		{
			IntersectionEntryVehicleContext vehicleContext = this._scope.Get<IntersectionEntryVehicleContext>();
			vehicleContext.Initialize(traversingVehicle);
			this._vehicleContexts.Add(vehicleContext);
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x06000924 RID: 2340 RVA: 0x0001DCA4 File Offset: 0x0001BEA4
	public int Id
	{
		get
		{
			return this._id;
		}
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0001DCAC File Offset: 0x0001BEAC
	public void SetId(int id)
	{
		this._id = id;
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0001DCB5 File Offset: 0x0001BEB5
	public bool IsRepeatOfEarlierDecision(IntersectionEntryDecision earlierDecision)
	{
		return this._intersection == earlierDecision._intersection && this._vehicle == earlierDecision._vehicle && this._verdict == earlierDecision._verdict && this.FirstBlockingVehicle == earlierDecision.FirstBlockingVehicle;
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x06000927 RID: 2343 RVA: 0x0001DCF4 File Offset: 0x0001BEF4
	[CanBeNull]
	public VehicleModel FirstBlockingVehicle
	{
		get
		{
			foreach (IntersectionEntryVehicleContext vehicleContext in this._vehicleContexts)
			{
				if (vehicleContext.WasBlocking)
				{
					return vehicleContext.Vehicle;
				}
			}
			return null;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06000928 RID: 2344 RVA: 0x0001DD54 File Offset: 0x0001BF54
	[NotNull]
	[ItemNotNull]
	public List<VehicleModel> BlockingVehicles
	{
		get
		{
			List<VehicleModel> blockingVehicles = new List<VehicleModel>();
			foreach (IntersectionEntryVehicleContext vehicleContext in this._vehicleContexts)
			{
				if (vehicleContext.WasBlocking)
				{
					blockingVehicles.Add(vehicleContext.Vehicle);
				}
			}
			return blockingVehicles;
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06000929 RID: 2345 RVA: 0x0001DDBC File Offset: 0x0001BFBC
	public int EarliestFrameCount
	{
		get
		{
			return this._earliestFrameCount;
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x0600092A RID: 2346 RVA: 0x0001DDC4 File Offset: 0x0001BFC4
	public int LatestFrameCount
	{
		get
		{
			return this._latestFrameCount;
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0001DDCC File Offset: 0x0001BFCC
	public void ExtendDuration(int newEndFrameCount)
	{
		this._latestFrameCount = newEndFrameCount;
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x0600092C RID: 2348 RVA: 0x0001DDD5 File Offset: 0x0001BFD5
	public IntersectionEntryVerdict Verdict
	{
		get
		{
			return this._verdict;
		}
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0001DDDD File Offset: 0x0001BFDD
	public void SetVerdict(IntersectionEntryVerdict value)
	{
		this._verdict = value;
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x0600092E RID: 2350 RVA: 0x0001DDE6 File Offset: 0x0001BFE6
	public bool WasEntryApproved
	{
		get
		{
			return this._verdict == IntersectionEntryVerdict.NoIntersectingLanes || this._verdict == IntersectionEntryVerdict.Shoved || this._verdict == IntersectionEntryVerdict.NoBlockingVehicles || this._verdict == IntersectionEntryVerdict.ExceededMaximumWaitTime;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x0600092F RID: 2351 RVA: 0x0001DE0E File Offset: 0x0001C00E
	public VehicleModel QueryingVehicle
	{
		get
		{
			return this._vehicle;
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06000930 RID: 2352 RVA: 0x0001DE16 File Offset: 0x0001C016
	public LaneModel CurrentLane
	{
		get
		{
			return this._currentLane;
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06000931 RID: 2353 RVA: 0x0001DE1E File Offset: 0x0001C01E
	public Fix64 DistanceAlongCurrentLane
	{
		get
		{
			return this._distanceAlongCurrentLane;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06000932 RID: 2354 RVA: 0x0001DE26 File Offset: 0x0001C026
	public LaneModel TargetLane
	{
		get
		{
			return this._targetLane;
		}
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06000933 RID: 2355 RVA: 0x0001DE2E File Offset: 0x0001C02E
	public Fix64 WaitTime
	{
		get
		{
			return this._waitTime;
		}
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x0001DE36 File Offset: 0x0001C036
	public void RemoveCurrentLane()
	{
		this._currentLane = null;
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0001DE3F File Offset: 0x0001C03F
	public void RemoveTargetLane()
	{
		this._targetLane = null;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0001DE48 File Offset: 0x0001C048
	public void SetTraversingVehicleInfluence(VehicleModel vehicle, IntersectionEntryVehicleInfluence influence)
	{
		foreach (IntersectionEntryVehicleContext vehicleContext in this._vehicleContexts)
		{
			if (vehicleContext.Vehicle == vehicle)
			{
				vehicleContext.Influence = influence;
				return;
			}
		}
		Diagnostics.FailAssert(string.Format("{0} is not part of an intersection that it is having an influence on.", vehicle), Array.Empty<object>());
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0001DEBC File Offset: 0x0001C0BC
	public void SetInboundVehicleInfluence(RoadChunkModel.InboundVehicle inboundVehicle, IntersectionEntryVehicleInfluence influence)
	{
		IntersectionEntryVehicleContext inboundVehicleContext = this._scope.Get<IntersectionEntryVehicleContext>();
		inboundVehicleContext.Initialize(inboundVehicle.vehicle);
		inboundVehicleContext.Influence = influence;
		this._vehicleContexts.Add(inboundVehicleContext);
	}

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06000938 RID: 2360 RVA: 0x0001DEF4 File Offset: 0x0001C0F4
	public IReadOnlyList<IntersectionEntryVehicleContext> OtherVehicleContexts
	{
		get
		{
			return this._vehicleContexts;
		}
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0001DEFC File Offset: 0x0001C0FC
	public void Reset()
	{
		this._id = -1;
		this._earliestFrameCount = 0;
		this._latestFrameCount = 0;
		this._intersection = null;
		this._targetLane = null;
		this._vehicle = null;
		this._currentLane = null;
		this._distanceAlongCurrentLane = default(Fix64);
		this._waitTime = default(Fix64);
		this._vehicleContexts.Clear();
		this._verdict = IntersectionEntryVerdict.Unknown;
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x0001DF64 File Offset: 0x0001C164
	public void OnReleasedFromScope(IScope scope)
	{
		foreach (IntersectionEntryVehicleContext vehicleContext in this._vehicleContexts)
		{
			scope.Release(vehicleContext);
		}
		this._vehicleContexts.Clear();
	}

	// Token: 0x040004A0 RID: 1184
	private int _id = -1;

	// Token: 0x040004A1 RID: 1185
	private int _earliestFrameCount;

	// Token: 0x040004A2 RID: 1186
	private int _latestFrameCount;

	// Token: 0x040004A3 RID: 1187
	private RoadChunkModel _intersection;

	// Token: 0x040004A4 RID: 1188
	private LaneModel _targetLane;

	// Token: 0x040004A5 RID: 1189
	private VehicleModel _vehicle;

	// Token: 0x040004A6 RID: 1190
	private LaneModel _currentLane;

	// Token: 0x040004A7 RID: 1191
	private Fix64 _distanceAlongCurrentLane;

	// Token: 0x040004A8 RID: 1192
	private Fix64 _waitTime;

	// Token: 0x040004A9 RID: 1193
	private readonly List<IntersectionEntryVehicleContext> _vehicleContexts = new List<IntersectionEntryVehicleContext>();

	// Token: 0x040004AA RID: 1194
	private IntersectionEntryVerdict _verdict;

	// Token: 0x040004AB RID: 1195
	[Dependency]
	private IScope _scope;

	// Token: 0x040004AC RID: 1196
	[Dependency]
	private Clock _clock;
}
