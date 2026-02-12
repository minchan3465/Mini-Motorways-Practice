using System;

// Token: 0x02000216 RID: 534
public interface IStorableTypeHandlerRegistry
{
	// Token: 0x06000CCC RID: 3276
	void RegisterHandler<T>(IStorableTypeHandler storableTypeHandler) where T : IStorable;

	// Token: 0x06000CCD RID: 3277
	IStorableTypeHandler GetHandlerForType(Type storableType);

	// Token: 0x06000CCE RID: 3278
	IStorableTypeHandler GetHandlerForFilename(string filename, out string playerId, out string deviceId);
}
