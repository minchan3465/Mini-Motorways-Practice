using System;

// Token: 0x02000152 RID: 338
public interface IControllerButtonToSymbolService
{
	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x0600076C RID: 1900
	bool HasMappings { get; }

	// Token: 0x0600076D RID: 1901
	string GetTextMeshProSymbolTextForControllerButton(ControllerButton buttonType);
}
