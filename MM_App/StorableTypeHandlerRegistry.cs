using System;
using System.Collections.Generic;

// Token: 0x02000232 RID: 562
public class StorableTypeHandlerRegistry : IStorableTypeHandlerRegistry
{
	// Token: 0x06000D5A RID: 3418 RVA: 0x0002BD35 File Offset: 0x00029F35
	public void RegisterHandler<T>(IStorableTypeHandler storableTypeHandler) where T : IStorable
	{
		this._typeHandlers[typeof(T)] = storableTypeHandler;
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x0002BD50 File Offset: 0x00029F50
	public IStorableTypeHandler GetHandlerForType(Type storableType)
	{
		foreach (KeyValuePair<Type, IStorableTypeHandler> entry in this._typeHandlers)
		{
			if (entry.Key.IsAssignableFrom(storableType))
			{
				return entry.Value;
			}
		}
		return null;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x0002BDB8 File Offset: 0x00029FB8
	public IStorableTypeHandler GetHandlerForFilename(string filename, out string playerId, out string deviceId)
	{
		foreach (IStorableTypeHandler type in this._typeHandlers.Values)
		{
			if (type.IsFilenameRecognized(filename, out playerId, out deviceId))
			{
				return type;
			}
		}
		playerId = null;
		deviceId = null;
		return null;
	}

	// Token: 0x04000788 RID: 1928
	private readonly Dictionary<Type, IStorableTypeHandler> _typeHandlers = new Dictionary<Type, IStorableTypeHandler>();
}
