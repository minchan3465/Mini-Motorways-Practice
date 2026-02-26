using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Factory.Allocators;
using JetBrains.Annotations;
using Unity.Profiling;
using UnityEngine;

namespace Factory.Pools
{
	// Token: 0x02000328 RID: 808
	public class Pool<T> : IAllocator<T>, IDisposable, IPoolInspectable where T : IReusable
	{
		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x00040504 File Offset: 0x0003E704
		// (set) Token: 0x0600137D RID: 4989 RVA: 0x0004050C File Offset: 0x0003E70C
		public int InitialSize { get; set; }

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x0600137E RID: 4990 RVA: 0x00040515 File Offset: 0x0003E715
		// (set) Token: 0x0600137F RID: 4991 RVA: 0x0004051D File Offset: 0x0003E71D
		public GrowthStrategy GrowthStrategy { get; set; }

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x00040526 File Offset: 0x0003E726
		// (set) Token: 0x06001381 RID: 4993 RVA: 0x0004052E File Offset: 0x0003E72E
		public int BlockSize { get; set; }

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x00040537 File Offset: 0x0003E737
		// (set) Token: 0x06001383 RID: 4995 RVA: 0x0004053F File Offset: 0x0003E73F
		public int LastGrownBy { get; private set; }

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x00040548 File Offset: 0x0003E748
		public bool NoUsedEntries
		{
			get
			{
				return this._firstUsedEntry == null;
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x00040553 File Offset: 0x0003E753
		// (set) Token: 0x06001386 RID: 4998 RVA: 0x0004055B File Offset: 0x0003E75B
		public bool IsValidatingObjectScrubbing { get; set; }

		// Token: 0x06001387 RID: 4999 RVA: 0x00040564 File Offset: 0x0003E764
		public Pool(IAllocator<T> objectAllocator)
		{
			this._objectAllocator = objectAllocator;
			this.InitialSize = 10;
			this.GrowthStrategy = GrowthStrategy.Block;
			this.BlockSize = 10;
			if (FeatureToggle.IsFeatureEnabled(Feature.ValidatePooledObjectScrubbing))
			{
				this.IsValidatingObjectScrubbing = true;
				return;
			}
			this.IsValidatingObjectScrubbing = false;
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x000405D4 File Offset: 0x0003E7D4
		public T Allocate(IScope context)
		{
			if (this._firstFreeEntry == null)
			{
				if (this._firstUsedEntry == null)
				{
					this.Grow(this.InitialSize, context);
				}
				else
				{
					switch (this.GrowthStrategy)
					{
					case GrowthStrategy.OnDemand:
						this.Grow(1, context);
						break;
					case GrowthStrategy.Block:
						this.Grow(this.BlockSize, context);
						break;
					}
				}
			}
			if (this._firstFreeEntry == null)
			{
				return default(T);
			}
			Pool<T>.Entry freeEntry = this._firstFreeEntry;
			this._firstFreeEntry = freeEntry.Next;
			freeEntry.Next = this._firstUsedEntry;
			this._firstUsedEntry = freeEntry;
			this._allocatedObjectCount++;
			this._freeObjectCount--;
			this.OnObjectAllocated(freeEntry.Object, context);
			return freeEntry.Object;
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x0004069C File Offset: 0x0003E89C
		public bool Release(T obj, IScope context)
		{
			Pool<T>.Entry previousEntry = null;
			Pool<T>.Entry entry = this._firstUsedEntry;
			while (entry != null && entry.Object != obj)
			{
				previousEntry = entry;
				entry = entry.Next;
			}
			if (entry == null)
			{
				return false;
			}
			if (previousEntry == null)
			{
				this._firstUsedEntry = entry.Next;
			}
			else
			{
				previousEntry.Next = entry.Next;
			}
			entry.Next = this._firstFreeEntry;
			this._firstFreeEntry = entry;
			this._allocatedObjectCount--;
			this._freeObjectCount++;
			obj.Reset();
			if (this.IsValidatingObjectScrubbing)
			{
				if (this._referenceMembers == null)
				{
					bool releaseReferenceObject = false;
					T referenceObject;
					if (this._firstFreeEntry != null)
					{
						referenceObject = this._firstFreeEntry.Object;
					}
					else
					{
						referenceObject = this._objectAllocator.Allocate(context);
						releaseReferenceObject = true;
					}
					this._referenceMembers = new Dictionary<MemberInfo, int>();
					foreach (FieldInfo fieldInfo in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (fieldInfo.GetCustomAttribute<DependencyAttribute>() == null && fieldInfo.GetCustomAttribute<UnscrubbedAttribute>() == null)
						{
							if (typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType))
							{
								ICollection referenceCollection = fieldInfo.GetValue(referenceObject) as ICollection;
								int count = (referenceCollection == null) ? -1 : referenceCollection.Count;
								this._referenceMembers[fieldInfo] = count;
							}
							else if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.IsValueType)
							{
								object referenceValue = fieldInfo.GetValue(referenceObject);
								int referenceHash = (referenceValue == null) ? 0 : referenceValue.GetHashCode();
								this._referenceMembers[fieldInfo] = referenceHash;
							}
						}
					}
					if (typeof(Component).IsAssignableFrom(typeof(T)))
					{
						Component component = referenceObject as Component;
						Transform referenceTransform = (component != null) ? component.transform : null;
						if (referenceTransform != null)
						{
							this._referencePosition = referenceTransform.localPosition;
							this._referenceRotation = referenceTransform.localRotation;
							this._referenceScale = referenceTransform.localScale;
						}
					}
					if (releaseReferenceObject)
					{
						this._objectAllocator.Release(referenceObject, context);
					}
				}
				List<string> unscrubbedMemberNames = new List<string>();
				foreach (MemberInfo memberInfo in this._referenceMembers.Keys)
				{
					if (memberInfo is FieldInfo)
					{
						FieldInfo fieldInfo2 = memberInfo as FieldInfo;
						object scrubbedValue = fieldInfo2.GetValue(obj);
						int scrubbedHash;
						if (typeof(ICollection).IsAssignableFrom(fieldInfo2.FieldType))
						{
							ICollection scrubbedCollection = scrubbedValue as ICollection;
							scrubbedHash = ((scrubbedCollection == null) ? -1 : scrubbedCollection.Count);
						}
						else
						{
							scrubbedHash = ((scrubbedValue == null) ? 0 : scrubbedValue.GetHashCode());
						}
						int referenceHash2 = this._referenceMembers[memberInfo];
						if (scrubbedHash != referenceHash2)
						{
							unscrubbedMemberNames.Add(memberInfo.Name);
						}
					}
				}
				if (typeof(Component).IsAssignableFrom(typeof(T)))
				{
					Component component2 = obj as Component;
					Transform transform = (component2 != null) ? component2.transform : null;
					if (transform != null)
					{
						if (transform.localPosition != this._referencePosition)
						{
							unscrubbedMemberNames.Add("transform.localPosition");
						}
						if (transform.localRotation != this._referenceRotation)
						{
							unscrubbedMemberNames.Add("transform.localRotation");
						}
						if (transform.localScale != this._referenceScale)
						{
							unscrubbedMemberNames.Add("transform.localScale");
						}
					}
				}
				if (unscrubbedMemberNames.Count > 0)
				{
					Diagnostics.FailAssert("{0} has {1} ({2}).", new object[]
					{
						obj,
						(unscrubbedMemberNames.Count > 1) ? "unscrubbed members" : "an unscrubbed member",
						string.Join(", ", unscrubbedMemberNames)
					});
				}
			}
			this.OnObjectReleased(obj, context);
			return true;
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x00040AA8 File Offset: 0x0003ECA8
		public void Clear()
		{
			if (this._firstUsedEntry != null)
			{
				Pool<T>.Entry tail = this._firstUsedEntry;
				while (tail.Next != null)
				{
					tail = tail.Next;
					this._allocatedObjectCount--;
					this._freeObjectCount++;
				}
				tail.Next = this._firstFreeEntry;
				this._firstFreeEntry = this._firstUsedEntry;
				this._firstUsedEntry = null;
			}
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnObjectCreated(T obj, IScope context)
		{
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnObjectAllocated(T obj, IScope context)
		{
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnObjectAssembled(T obj, IScope context)
		{
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnObjectReleased(T obj, IScope context)
		{
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x00040B10 File Offset: 0x0003ED10
		public void Dispose()
		{
			for (Pool<T>.Entry entry = this._firstFreeEntry; entry != null; entry = entry.Next)
			{
				this._objectAllocator.Release(entry.Object, null);
			}
			this._firstFreeEntry = null;
			for (Pool<T>.Entry entry = this._firstUsedEntry; entry != null; entry = entry.Next)
			{
				this._objectAllocator.Release(entry.Object, null);
			}
			this._firstUsedEntry = null;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x00040B78 File Offset: 0x0003ED78
		private void Grow(int size, IScope context)
		{
			int increase = 0;
			for (int i = 0; i < size; i++)
			{
				T newObj = this._objectAllocator.Allocate(context);
				this.OnObjectCreated(newObj, context);
				Pool<T>.Entry newEntry = new Pool<T>.Entry(newObj, this._firstFreeEntry);
				this._firstFreeEntry = newEntry;
				increase++;
			}
			this.LastGrownBy = increase;
			this._freeObjectCount += size;
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x00040BD8 File Offset: 0x0003EDD8
		public void GetAllElements([NotNull] List<object> allocated, [NotNull] List<object> free)
		{
			allocated.Clear();
			for (Pool<T>.Entry currentEntry = this._firstUsedEntry; currentEntry != null; currentEntry = currentEntry.Next)
			{
				allocated.Add(currentEntry.Object);
			}
			free.Clear();
			for (Pool<T>.Entry currentEntry = this._firstFreeEntry; currentEntry != null; currentEntry = currentEntry.Next)
			{
				free.Add(currentEntry.Object);
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06001392 RID: 5010 RVA: 0x00040C39 File Offset: 0x0003EE39
		public int AllocatedObjectCount
		{
			get
			{
				return this._allocatedObjectCount;
			}
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x000020AA File Offset: 0x000002AA
		protected virtual bool DefaultExpanded()
		{
			return true;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x00040C44 File Offset: 0x0003EE44
		protected virtual string GroupingName(object entryInstance)
		{
			return "Hash Code " + entryInstance.GetHashCode().ToString();
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InspectEntryGrouping(object entryInstance, Dictionary<object, bool> expandedLookup)
		{
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void InspectEntry(object entryInstance)
		{
		}

		// Token: 0x04001069 RID: 4201
		private readonly IAllocator<T> _objectAllocator;

		// Token: 0x0400106A RID: 4202
		private Pool<T>.Entry _firstFreeEntry;

		// Token: 0x0400106B RID: 4203
		private Pool<T>.Entry _firstUsedEntry;

		// Token: 0x0400106C RID: 4204
		private int _allocatedObjectCount;

		// Token: 0x0400106D RID: 4205
		private int _freeObjectCount;

		// Token: 0x0400106E RID: 4206
		private Dictionary<MemberInfo, int> _referenceMembers;

		// Token: 0x0400106F RID: 4207
		private Vector3 _referencePosition;

		// Token: 0x04001070 RID: 4208
		private Quaternion _referenceRotation;

		// Token: 0x04001071 RID: 4209
		private Vector3 _referenceScale;

		// Token: 0x04001077 RID: 4215
		private readonly string GrowProfilerSampleName = "Pool<" + typeof(T).Name + ">.Grow";

		// Token: 0x04001078 RID: 4216
		private static readonly ProfilerMarker Profiler_ValidatingObjectScrubbing = new ProfilerMarker(ProfilerCategory.Memory, "Pool.ValidatingObjectScrubbing");

		// Token: 0x02000329 RID: 809
		private class Entry
		{
			// Token: 0x170003D8 RID: 984
			// (get) Token: 0x06001398 RID: 5016 RVA: 0x00040C7F File Offset: 0x0003EE7F
			// (set) Token: 0x06001399 RID: 5017 RVA: 0x00040C87 File Offset: 0x0003EE87
			public T Object { get; private set; }

			// Token: 0x170003D9 RID: 985
			// (get) Token: 0x0600139A RID: 5018 RVA: 0x00040C90 File Offset: 0x0003EE90
			// (set) Token: 0x0600139B RID: 5019 RVA: 0x00040C98 File Offset: 0x0003EE98
			public Pool<T>.Entry Next { get; set; }

			// Token: 0x0600139C RID: 5020 RVA: 0x00040CA1 File Offset: 0x0003EEA1
			public Entry(T obj, Pool<T>.Entry nextEntry)
			{
				this.Object = obj;
				this.Next = nextEntry;
			}
		}
	}
}
