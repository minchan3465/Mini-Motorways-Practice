using System;
using System.Collections.Generic;

namespace Motorways.Pathfinding
{
	// Token: 0x020004C3 RID: 1219
	public static class CollectionExtensions
	{
		// Token: 0x06001FB1 RID: 8113 RVA: 0x0007D8FC File Offset: 0x0007BAFC
		public static V GetOrCreate<K, V>(this IDictionary<K, V> collection, K key) where V : new()
		{
			V value;
			if (!collection.TryGetValue(key, out value))
			{
				value = (collection[key] = Activator.CreateInstance<V>());
			}
			return value;
		}
	}
}
