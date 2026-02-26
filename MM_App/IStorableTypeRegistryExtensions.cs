using System;

// Token: 0x02000217 RID: 535
public static class IStorableTypeRegistryExtensions
{
	// Token: 0x06000CCF RID: 3279 RVA: 0x00029A96 File Offset: 0x00027C96
	public static bool IsFilenameRecognized(this IStorableTypeHandlerRegistry registry, string filename, out string playerId, out string deviceId)
	{
		return registry.GetHandlerForFilename(filename, out playerId, out deviceId) != null;
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x00029AA4 File Offset: 0x00027CA4
	public static bool IsFilenameRecognized(this IStorableTypeHandlerRegistry registry, string filename)
	{
		string text;
		string text2;
		return registry.IsFilenameRecognized(filename, out text, out text2);
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00029ABC File Offset: 0x00027CBC
	public static IStorableTypeHandler GetHandlerForStorable(this IStorableTypeHandlerRegistry registry, IStorable storable)
	{
		return registry.GetHandlerForType(storable.GetType());
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00029ACA File Offset: 0x00027CCA
	public static IStorableTypeHandler GetHandlerForType<T>(this IStorableTypeHandlerRegistry registry)
	{
		return registry.GetHandlerForType(typeof(T));
	}
}
