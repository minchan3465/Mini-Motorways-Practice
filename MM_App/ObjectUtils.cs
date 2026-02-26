using System;
using UnityEngine;

// Token: 0x02000267 RID: 615
public static class ObjectUtils
{
	// Token: 0x06000EA2 RID: 3746 RVA: 0x00031A20 File Offset: 0x0002FC20
	public static bool IsNullOrDestroyed<T>(T obj)
	{
		if (obj != null)
		{
			UnityEngine.Object unityObject = obj as UnityEngine.Object;
			return unityObject != null && unityObject == null;
		}
		return true;
	}
}
