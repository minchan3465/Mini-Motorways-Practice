using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class ControllerActionState : IComparable<ControllerActionState>
{
	// Token: 0x170001BB RID: 443
	// (get) Token: 0x060007C1 RID: 1985 RVA: 0x00018EEA File Offset: 0x000170EA
	// (set) Token: 0x060007C2 RID: 1986 RVA: 0x00018EF2 File Offset: 0x000170F2
	public bool BoolValue { get; protected set; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x060007C3 RID: 1987 RVA: 0x00018EFB File Offset: 0x000170FB
	// (set) Token: 0x060007C4 RID: 1988 RVA: 0x00018F03 File Offset: 0x00017103
	public Vector2 Vector2Value { get; protected set; }

	// Token: 0x060007C5 RID: 1989 RVA: 0x000045E9 File Offset: 0x000027E9
	protected ControllerActionState()
	{
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00018F0C File Offset: 0x0001710C
	public static ControllerActionState CreateBoolActionState(bool boolValue)
	{
		return new ControllerActionState
		{
			BoolValue = boolValue
		};
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x00018F1A File Offset: 0x0001711A
	public static ControllerActionState CreateVector2ActionState(bool boolValue, Vector2 vectorValue)
	{
		return new ControllerActionState
		{
			BoolValue = boolValue,
			Vector2Value = vectorValue
		};
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00018F30 File Offset: 0x00017130
	public int CompareTo(ControllerActionState other)
	{
		return this.BoolValue.CompareTo(other.BoolValue);
	}
}
