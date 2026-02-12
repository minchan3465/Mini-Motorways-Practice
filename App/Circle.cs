using System;
using UnityEngine;

// Token: 0x02000257 RID: 599
public struct Circle
{
	// Token: 0x06000E39 RID: 3641 RVA: 0x000301E2 File Offset: 0x0002E3E2
	public Circle(Vector2 origin, float radius)
	{
		this.origin = origin;
		this.radius = radius;
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06000E3A RID: 3642 RVA: 0x000301F2 File Offset: 0x0002E3F2
	public Vector2 Origin
	{
		get
		{
			return this.origin;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06000E3B RID: 3643 RVA: 0x000301FA File Offset: 0x0002E3FA
	public float Radius
	{
		get
		{
			return this.radius;
		}
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x00030202 File Offset: 0x0002E402
	public override string ToString()
	{
		return string.Format("[Line: Origin={0}, Radius={1}]", this.origin, this.radius);
	}

	// Token: 0x0400086A RID: 2154
	private Vector2 origin;

	// Token: 0x0400086B RID: 2155
	private float radius;
}
