using System;

// Token: 0x02000086 RID: 134
public abstract class InGameDevToolNumericParameter<ParamType, DerivedType> : InGameDevToolParameter<ParamType, DerivedType> where DerivedType : InGameDevToolNumericParameter<ParamType, DerivedType>
{
	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060001BE RID: 446 RVA: 0x0000623C File Offset: 0x0000443C
	// (set) Token: 0x060001BF RID: 447 RVA: 0x00006244 File Offset: 0x00004444
	public bool HasMinimumValue { get; protected set; }

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000624D File Offset: 0x0000444D
	// (set) Token: 0x060001C1 RID: 449 RVA: 0x00006255 File Offset: 0x00004455
	public ParamType MinimumValue { get; protected set; }

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000625E File Offset: 0x0000445E
	// (set) Token: 0x060001C3 RID: 451 RVA: 0x00006266 File Offset: 0x00004466
	public bool HasMaximumValue { get; protected set; }

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000626F File Offset: 0x0000446F
	// (set) Token: 0x060001C5 RID: 453 RVA: 0x00006277 File Offset: 0x00004477
	public ParamType MaximumValue { get; protected set; }

	// Token: 0x060001C6 RID: 454 RVA: 0x00006280 File Offset: 0x00004480
	public DerivedType SetMinimumValue(ParamType minimumValue)
	{
		this.HasMinimumValue = true;
		this.MinimumValue = minimumValue;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00006296 File Offset: 0x00004496
	public DerivedType SetMaximumValue(ParamType maximumValue)
	{
		this.HasMaximumValue = true;
		this.MaximumValue = maximumValue;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x000062AC File Offset: 0x000044AC
	protected InGameDevToolNumericParameter(InGameDevToolParameterType typeOfParameter) : base(typeOfParameter)
	{
		this.HasMinimumValue = false;
		this.HasMaximumValue = false;
	}
}
