using System;

// Token: 0x0200008B RID: 139
public interface IInGameDevToolEnumParameter
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060001D3 RID: 467
	InGameDevToolParameterType TypeOfParameter { get; }

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060001D4 RID: 468
	string ParameterName { get; }

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060001D5 RID: 469
	string ParameterSerializationValue { get; }

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060001D6 RID: 470
	string ParameterSerializationDefaultValue { get; }

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060001D7 RID: 471
	string ModelParameterFieldName { get; }

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060001D8 RID: 472
	string ParameterEditorDisplayName { get; }

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060001D9 RID: 473
	string ParameterEditorTooltip { get; }

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060001DA RID: 474
	bool ShouldSetValueOnField { get; }

	// Token: 0x060001DB RID: 475
	void UpdateParameterValueFromModelField<ModelType>(ModelType modelInstance);

	// Token: 0x060001DC RID: 476
	void UpdateModelFieldFromParameterValue<ModelType>(ModelType modelInstance);
}
