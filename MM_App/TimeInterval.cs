using System;

// Token: 0x0200024E RID: 590
public class TimeInterval
{
	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06000E11 RID: 3601 RVA: 0x0002F9E1 File Offset: 0x0002DBE1
	// (set) Token: 0x06000E12 RID: 3602 RVA: 0x0002F9E9 File Offset: 0x0002DBE9
	public float UnsyncedDelta { get; set; }

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06000E13 RID: 3603 RVA: 0x0002F9F2 File Offset: 0x0002DBF2
	// (set) Token: 0x06000E14 RID: 3604 RVA: 0x0002F9FA File Offset: 0x0002DBFA
	public float Delta { get; set; }

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06000E15 RID: 3605 RVA: 0x0002FA03 File Offset: 0x0002DC03
	public float ScaledDelta
	{
		get
		{
			if (!this.IsPaused)
			{
				return this.UnpausedScaledDelta;
			}
			return 0f;
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06000E16 RID: 3606 RVA: 0x0002FA19 File Offset: 0x0002DC19
	public float UnpausedScaledDelta
	{
		get
		{
			return this.Scale.ScaleTime(this.Delta);
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06000E17 RID: 3607 RVA: 0x0002FA2C File Offset: 0x0002DC2C
	// (set) Token: 0x06000E18 RID: 3608 RVA: 0x0002FA34 File Offset: 0x0002DC34
	public TimeScale Scale { get; set; } = TimeScale.Single;

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06000E19 RID: 3609 RVA: 0x0002FA3D File Offset: 0x0002DC3D
	// (set) Token: 0x06000E1A RID: 3610 RVA: 0x0002FA45 File Offset: 0x0002DC45
	public bool IsPaused { get; set; }
}
