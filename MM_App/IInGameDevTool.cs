using System;
using System.Collections.Generic;
using Server;
using UnityEngine;

// Token: 0x02000098 RID: 152
public interface IInGameDevTool
{
	// Token: 0x06000234 RID: 564
	IInGameDevTool SetCommandSerializationName(string newCommandSerializationName);

	// Token: 0x06000235 RID: 565
	string GetCommandSerializationName();

	// Token: 0x06000236 RID: 566
	string GetEditorToolDisplayName();

	// Token: 0x06000237 RID: 567
	string GetEditorToolDisplayNameWithoutHotkeyCode();

	// Token: 0x06000238 RID: 568
	string GetEditorToolIconPath();

	// Token: 0x06000239 RID: 569
	IEnumerable<IngameDevToolBoolParameter> BoolParameters();

	// Token: 0x0600023A RID: 570
	IEnumerable<IngameDevToolIntParameter> IntParameters();

	// Token: 0x0600023B RID: 571
	IEnumerable<IInGameDevToolEnumParameter> EnumParameters();

	// Token: 0x0600023C RID: 572
	IEnumerable<IngameDevToolFloatParameter> FloatParameters();

	// Token: 0x0600023D RID: 573
	IEnumerable<IngameDevToolStringParameter> StringParameters();

	// Token: 0x0600023E RID: 574
	IngameDevToolBoolParameter GetBoolParameter(string parameterName);

	// Token: 0x0600023F RID: 575
	IngameDevToolIntParameter GetIntParameter(string parameterName);

	// Token: 0x06000240 RID: 576
	IngameDevToolEnumParameter<EnumType> GetEnumParameter<EnumType>(string parameterName) where EnumType : struct;

	// Token: 0x06000241 RID: 577
	string GetEnumParameterValueAsString(string parameterName);

	// Token: 0x06000242 RID: 578
	IngameDevToolFloatParameter GetFloatParameter(string parameterName);

	// Token: 0x06000243 RID: 579
	IngameDevToolStringParameter GetStringParameter(string parameterName);

	// Token: 0x06000244 RID: 580
	void PrepareTool();

	// Token: 0x06000245 RID: 581
	void Tick(TimeInterval tickTime, float stepAlpha, out bool activatedThisTick);

	// Token: 0x06000246 RID: 582
	void CleanupTool();

	// Token: 0x06000247 RID: 583
	Action<CommandType, ISimulation> GetActionWithCommandType<CommandType>();

	// Token: 0x06000248 RID: 584
	bool InGameHotkeyActivated();

	// Token: 0x06000249 RID: 585
	bool InGameParameterHotKeyActivated();

	// Token: 0x0600024A RID: 586
	void OnHotkeyActivated(bool useDefaultParameters);

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600024B RID: 587
	// (set) Token: 0x0600024C RID: 588
	bool ResetToNoneAfterUse { get; set; }

	// Token: 0x0600024D RID: 589
	void OnToolSelected();

	// Token: 0x0600024E RID: 590
	void OnToolDeselected();

	// Token: 0x0600024F RID: 591
	bool HasHotKey();

	// Token: 0x06000250 RID: 592
	KeyCode GetHotKey();

	// Token: 0x06000251 RID: 593
	KeyCode GetModifierHotKey();
}
