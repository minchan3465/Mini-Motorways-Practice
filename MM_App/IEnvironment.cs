using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public interface IEnvironment
{
	// Token: 0x060004BB RID: 1211
	void PopulateAppAssembler(Assembler baseAssembler);

	// Token: 0x060004BC RID: 1212
	void PopulateGameAssembler(Assembler baseAssembler);

	// Token: 0x060004BD RID: 1213
	BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject);

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x060004BE RID: 1214
	DeviceCategory DeviceCategory { get; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x060004BF RID: 1215
	[CanBeNull]
	List<string> FeatureConfigs { get; }
}
