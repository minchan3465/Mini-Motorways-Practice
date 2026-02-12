using System;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class EnumTypedArray : PropertyAttribute
{
	// Token: 0x06000E46 RID: 3654 RVA: 0x0003087E File Offset: 0x0002EA7E
	public EnumTypedArray(Type TargetEnum)
	{
		this.TargetEnum = TargetEnum;
	}

	// Token: 0x04000875 RID: 2165
	public Type TargetEnum;
}
