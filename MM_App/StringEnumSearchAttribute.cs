using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class StringEnumSearchAttribute : PropertyAttribute
{
	// Token: 0x06000EC9 RID: 3785 RVA: 0x00031E49 File Offset: 0x00030049
	public StringEnumSearchAttribute(Type enumType)
	{
		this.enumType = enumType;
	}

	// Token: 0x040008B0 RID: 2224
	public Type enumType;
}
