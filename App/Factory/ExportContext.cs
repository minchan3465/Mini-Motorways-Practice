using System;
using System.Collections.Generic;
using System.IO;

namespace Factory
{
	// Token: 0x020002F3 RID: 755
	public class ExportContext
	{
		// Token: 0x06001292 RID: 4754 RVA: 0x0003E213 File Offset: 0x0003C413
		public ExportContext(BinaryWriter writer, IScope scope)
		{
			this._writer = writer;
			this._scope = scope;
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x0003E234 File Offset: 0x0003C434
		public BinaryWriter Writer
		{
			get
			{
				return this._writer;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06001294 RID: 4756 RVA: 0x0003E23C File Offset: 0x0003C43C
		public IScope Scope
		{
			get
			{
				return this._scope;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06001295 RID: 4757 RVA: 0x0003E244 File Offset: 0x0003C444
		public ExportContext.ObjectLibrary Library
		{
			get
			{
				return this._objectLibrary;
			}
		}

		// Token: 0x0400101B RID: 4123
		private readonly BinaryWriter _writer;

		// Token: 0x0400101C RID: 4124
		private IScope _scope;

		// Token: 0x0400101D RID: 4125
		private ExportContext.ObjectLibrary _objectLibrary = new ExportContext.ObjectLibrary();

		// Token: 0x020002F4 RID: 756
		public class ObjectLibrary
		{
			// Token: 0x06001296 RID: 4758 RVA: 0x0003E24C File Offset: 0x0003C44C
			public bool ContainsObject(object obj)
			{
				Type objType = obj.GetType();
				return this._typedObjects.ContainsKey(objType) && this._typedObjects[objType].objectIndex.ContainsKey(obj);
			}

			// Token: 0x06001297 RID: 4759 RVA: 0x0003E288 File Offset: 0x0003C488
			public void AddObject(object obj)
			{
				Type objType = obj.GetType();
				ExportContext.ObjectLibrary.TypedObjectCollection exportedType;
				if (!this._typedObjects.TryGetValue(objType, out exportedType))
				{
					exportedType = new ExportContext.ObjectLibrary.TypedObjectCollection();
					this._typedObjects[objType] = exportedType;
					this._types.Add(objType);
				}
				exportedType.objectIndex[obj] = exportedType.objects.Count;
				exportedType.objects.Add(obj);
			}

			// Token: 0x06001298 RID: 4760 RVA: 0x0003E2F0 File Offset: 0x0003C4F0
			public void BuildIndex()
			{
				int baseObjectId = 1;
				foreach (Type type in this._types)
				{
					ExportContext.ObjectLibrary.TypedObjectCollection et = this._typedObjects[type];
					et.baseObjectId = baseObjectId;
					baseObjectId += et.objects.Count;
				}
			}

			// Token: 0x170003B0 RID: 944
			// (get) Token: 0x06001299 RID: 4761 RVA: 0x0003E360 File Offset: 0x0003C560
			public ICollection<Type> Types
			{
				get
				{
					return this._types;
				}
			}

			// Token: 0x0600129A RID: 4762 RVA: 0x0003E368 File Offset: 0x0003C568
			public ICollection<object> GetObjectsOfType(Type objType)
			{
				return this._typedObjects[objType].objects;
			}

			// Token: 0x0600129B RID: 4763 RVA: 0x0003E37C File Offset: 0x0003C57C
			public int GetObjectId(object obj)
			{
				ExportContext.ObjectLibrary.TypedObjectCollection et;
				if (!Diagnostics.Verify(this._typedObjects.TryGetValue(obj.GetType(), out et)))
				{
					return -1;
				}
				return et.baseObjectId + et.objectIndex[obj];
			}

			// Token: 0x0400101E RID: 4126
			private List<Type> _types = new List<Type>();

			// Token: 0x0400101F RID: 4127
			private Dictionary<Type, ExportContext.ObjectLibrary.TypedObjectCollection> _typedObjects = new Dictionary<Type, ExportContext.ObjectLibrary.TypedObjectCollection>();

			// Token: 0x020002F5 RID: 757
			private class TypedObjectCollection
			{
				// Token: 0x04001020 RID: 4128
				public int baseObjectId;

				// Token: 0x04001021 RID: 4129
				public List<object> objects = new List<object>();

				// Token: 0x04001022 RID: 4130
				public Dictionary<object, int> objectIndex = new Dictionary<object, int>();
			}
		}
	}
}
