using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using JetBrains.Annotations;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004EE RID: 1262
	public class IntersectionDecisionDatabaseModel : IModel, IReusable, IReleasedFromScopeHandler, IDeserializedHandler
	{
		// Token: 0x0600211D RID: 8477 RVA: 0x00083EE8 File Offset: 0x000820E8
		public void AddDecision(IntersectionEntryDecision newDecision)
		{
			if (newDecision.Verdict == IntersectionEntryVerdict.Unknown || newDecision.Verdict == IntersectionEntryVerdict.NoReservedLane || newDecision.Verdict == IntersectionEntryVerdict.NoIntersectingLanes)
			{
				this._scope.Release(newDecision);
				return;
			}
			IntersectionEntryDecision latestDecision = this.GetLatestDecision(newDecision.QueryingVehicle);
			if (latestDecision != null && newDecision.IsRepeatOfEarlierDecision(latestDecision))
			{
				latestDecision.ExtendDuration(newDecision.LatestFrameCount);
				this._scope.Release(newDecision);
				return;
			}
			newDecision.SetId(this._nextId);
			this._nextId++;
			this._decisions.Add(newDecision);
			List<IntersectionEntryDecision> vehicleDecisions;
			if (!this._queryingVehicleIndex.TryGetValue(newDecision.QueryingVehicle, out vehicleDecisions))
			{
				vehicleDecisions = new List<IntersectionEntryDecision>();
				this._queryingVehicleIndex.Add(newDecision.QueryingVehicle, vehicleDecisions);
			}
			vehicleDecisions.Insert(0, newDecision);
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x00083FAC File Offset: 0x000821AC
		[CanBeNull]
		public IntersectionEntryDecision GetLatestDecision([NotNull] VehicleModel vehicleModel)
		{
			List<IntersectionEntryDecision> decisions;
			if (this._queryingVehicleIndex.TryGetValue(vehicleModel, out decisions) && decisions.Count > 0)
			{
				return decisions[0];
			}
			return null;
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x00083FDC File Offset: 0x000821DC
		public List<IntersectionEntryDecision> GetDecisions([NotNull] VehicleModel vehicleModel)
		{
			List<IntersectionEntryDecision> decisions;
			if (this._queryingVehicleIndex.TryGetValue(vehicleModel, out decisions))
			{
				return decisions;
			}
			return null;
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x00083FFC File Offset: 0x000821FC
		public void OnDeserialized(IScope context)
		{
			foreach (IntersectionEntryDecision decision in this._decisions)
			{
				this._nextId = Mathf.Max(this._nextId, decision.Id + 1);
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x00084064 File Offset: 0x00082264
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (IntersectionEntryDecision decision in this._decisions)
			{
				scope.Release(decision);
			}
			this._decisions.Clear();
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x000840C4 File Offset: 0x000822C4
		public void Reset()
		{
			this._nextId = 1;
			this._decisions.Clear();
			this._queryingVehicleIndex.Clear();
		}

		// Token: 0x04001B62 RID: 7010
		[Serialize(false, null)]
		private int _nextId = 1;

		// Token: 0x04001B63 RID: 7011
		private readonly List<IntersectionEntryDecision> _decisions = new List<IntersectionEntryDecision>();

		// Token: 0x04001B64 RID: 7012
		private readonly Dictionary<VehicleModel, List<IntersectionEntryDecision>> _queryingVehicleIndex = new Dictionary<VehicleModel, List<IntersectionEntryDecision>>();

		// Token: 0x04001B65 RID: 7013
		[Dependency]
		private IScope _scope;
	}
}
