using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004FE RID: 1278
	public class SnapshotModel : IModel, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x060021E6 RID: 8678 RVA: 0x0008893B File Offset: 0x00086B3B
		public void Reset()
		{
			this.vehicleDispatches.Clear();
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x00088948 File Offset: 0x00086B48
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (VehicleDispatchRecord vehicleDispatchRecord in this.vehicleDispatches)
			{
				scope.Release(vehicleDispatchRecord);
			}
			this.vehicleDispatches.Clear();
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Inspect()
		{
		}

		// Token: 0x04001BD4 RID: 7124
		public List<VehicleDispatchRecord> vehicleDispatches = new List<VehicleDispatchRecord>();
	}
}
