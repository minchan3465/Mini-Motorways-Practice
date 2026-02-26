using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class MockEnvironment : IEnvironment
{
	// Token: 0x060006E0 RID: 1760 RVA: 0x000167ED File Offset: 0x000149ED
	public MockEnvironment(IEnvironment emulatedEnvironment)
	{
		this._emulatedEnvironment = emulatedEnvironment;
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x000167FC File Offset: 0x000149FC
	public virtual void PopulateAppAssembler(Assembler baseAssembler)
	{
		if (Diagnostics.Verify(this._emulatedEnvironment != null, "Cannot provide a null environment to the Mock Environment!"))
		{
			this._emulatedEnvironment.PopulateAppAssembler(baseAssembler);
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x0001681F File Offset: 0x00014A1F
	public virtual void PopulateGameAssembler(Assembler baseAssembler)
	{
		IEnvironment emulatedEnvironment = this._emulatedEnvironment;
		if (emulatedEnvironment == null)
		{
			return;
		}
		emulatedEnvironment.PopulateGameAssembler(baseAssembler);
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00011C09 File Offset: 0x0000FE09
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<BaseInputOverride>();
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x060006E4 RID: 1764 RVA: 0x00016832 File Offset: 0x00014A32
	public DeviceCategory DeviceCategory
	{
		get
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.MockPhone))
			{
				return DeviceCategory.Phone;
			}
			if (Diagnostics.Verify(this._emulatedEnvironment != null, "Emulated environment can't be null for mock environment!"))
			{
				return this._emulatedEnvironment.DeviceCategory;
			}
			return DeviceCategory.Desktop;
		}
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x060006E5 RID: 1765 RVA: 0x00016861 File Offset: 0x00014A61
	public List<string> FeatureConfigs
	{
		get
		{
			return new List<string>();
		}
	}

	// Token: 0x0400029A RID: 666
	private readonly IEnvironment _emulatedEnvironment;
}
