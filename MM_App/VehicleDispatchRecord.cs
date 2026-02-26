using System;
using Factory;
using Factory.Pools;
using UnityEngine;

// Token: 0x020001DC RID: 476
[Factory.Serializable(1)]
public class VehicleDispatchRecord : IReusable
{
	// Token: 0x06000B59 RID: 2905 RVA: 0x00026C78 File Offset: 0x00024E78
	public override bool Equals(object obj)
	{
		VehicleDispatchRecord otherRecord = obj as VehicleDispatchRecord;
		return otherRecord != null && this == otherRecord;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00026C98 File Offset: 0x00024E98
	public override int GetHashCode()
	{
		return this.SimulationFrame ^ this.HouseCoordinates.GetHashCode() ^ this.DestinationCoordinates.GetHashCode();
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00026CC4 File Offset: 0x00024EC4
	public static bool operator ==(VehicleDispatchRecord a, VehicleDispatchRecord b)
	{
		bool isANull = a == null;
		bool isBNull = b == null;
		return (isANull && isBNull) || (!isANull && !isBNull && (a.SimulationFrame == b.SimulationFrame && a.HouseCoordinates == b.HouseCoordinates) && a.DestinationCoordinates == b.DestinationCoordinates);
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00026D1D File Offset: 0x00024F1D
	public static bool operator !=(VehicleDispatchRecord a, VehicleDispatchRecord b)
	{
		return !(a == b);
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00026D29 File Offset: 0x00024F29
	public void Reset()
	{
		this.SimulationFrame = 0;
		this.HouseCoordinates = default(Vector2Int);
		this.DestinationCoordinates = default(Vector2Int);
	}

	// Token: 0x04000680 RID: 1664
	public int SimulationFrame;

	// Token: 0x04000681 RID: 1665
	public Vector2Int HouseCoordinates;

	// Token: 0x04000682 RID: 1666
	public Vector2Int DestinationCoordinates;
}
