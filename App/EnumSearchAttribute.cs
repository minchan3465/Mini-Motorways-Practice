using System;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class EnumSearchAttribute : PropertyAttribute
{
	// Token: 0x06000E45 RID: 3653 RVA: 0x00030868 File Offset: 0x0002EA68
	public EnumSearchAttribute(Type enumType, bool isString = false)
	{
		this.enumType = enumType;
		this.isString = isString;
	}

	// Token: 0x04000873 RID: 2163
	public Type enumType;

	// Token: 0x04000874 RID: 2164
	public bool isString;
}
