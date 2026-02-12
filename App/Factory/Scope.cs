using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Factory
{
	// Token: 0x020002FF RID: 767
	public class Scope : IScope
	{
		// Token: 0x060012CB RID: 4811 RVA: 0x0003E5F4 File Offset: 0x0003C7F4
		public Scope(Assembler assembler, object establishingObject = null)
		{
			this._assembler = assembler;
			this._establishingObject = establishingObject;
			this.Set<IScope>(this);
			this.Set<Scope>(this);
			this.Set<Assembler>(this._assembler);
			if (FeatureToggle.IsFeatureEnabled(Feature.TrackScopedAllocations))
			{
				this._outstandingAllocationCountsByType = new Dictionary<Type, int>();
				if (FeatureToggle.IsFeatureEnabled(Feature.RecordStackTracesForScopedAllocations))
				{
					this._outstandingAllocationsByType = new Dictionary<Type, List<Scope.Allocation>>();
				}
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x0003E679 File Offset: 0x0003C879
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x0003E681 File Offset: 0x0003C881
		public IScope ParentScope
		{
			get
			{
				return this._parentScope;
			}
			set
			{
				IScope parentScope = this._parentScope;
				this._parentScope = value;
			}
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0003E691 File Offset: 0x0003C891
		public void AddChildScope(IScope childScope, object establishingObject)
		{
			this._establishingObjectToChildScopes[establishingObject] = childScope;
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x060012CF RID: 4815 RVA: 0x0003E6A0 File Offset: 0x0003C8A0
		public Assembler Assembler
		{
			get
			{
				return this._assembler;
			}
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x0003E6A8 File Offset: 0x0003C8A8
		public object Get(Type type)
		{
			object boundObj;
			if (this._typeToBoundVariables.TryGetValue(type, out boundObj))
			{
				return boundObj;
			}
			object obj = this._assembler.Create(type, this);
			if (obj != null)
			{
				if (this._outstandingAllocationCountsByType != null)
				{
					Type concreteType = obj.GetType();
					int allocationCount;
					if (this._outstandingAllocationCountsByType.TryGetValue(concreteType, out allocationCount))
					{
						this._outstandingAllocationCountsByType[concreteType] = allocationCount + 1;
					}
					else
					{
						this._outstandingAllocationCountsByType.Add(concreteType, 1);
					}
					if (this._outstandingAllocationsByType != null)
					{
						if (!this._outstandingAllocationsByType.ContainsKey(concreteType))
						{
							this._outstandingAllocationsByType[concreteType] = new List<Scope.Allocation>();
						}
						this._outstandingAllocationsByType[concreteType].Add(new Scope.Allocation
						{
							obj = obj,
							stackTrace = new StackTrace(2, true)
						});
					}
				}
				return obj;
			}
			if (this._parentScope != null)
			{
				obj = this._parentScope.Get(type);
				if (obj != null)
				{
					return obj;
				}
			}
			Diagnostics.FailAssert("Unable to find assembler for type '{0}' anywhere in scope.", new object[]
			{
				type
			});
			return null;
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x0003E79D File Offset: 0x0003C99D
		public T Get<T>() where T : class
		{
			return this.Get(typeof(T)) as T;
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x0003E7B9 File Offset: 0x0003C9B9
		public void Assemble(object unboundObject)
		{
			this._assembler.Assemble(unboundObject, this);
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0003E7C8 File Offset: 0x0003C9C8
		public bool Release()
		{
			Scope.Log.Info("Releasing scope using assembler {0}.", new object[]
			{
				this.Assembler.Name
			});
			this._typeToBoundVariables.Remove(typeof(IScope));
			this._typeToBoundVariables.Remove(typeof(Scope));
			this._typeToBoundVariables.Remove(typeof(Assembler));
			if (this._establishingObject != null)
			{
				List<Type> establishingObjectTypes = null;
				foreach (KeyValuePair<Type, object> boundVariable in this._typeToBoundVariables)
				{
					if (boundVariable.Value == this._establishingObject)
					{
						if (establishingObjectTypes == null)
						{
							establishingObjectTypes = new List<Type>();
						}
						establishingObjectTypes.Add(boundVariable.Key);
					}
				}
				if (establishingObjectTypes != null)
				{
					foreach (Type boundType in establishingObjectTypes)
					{
						this._typeToBoundVariables.Remove(boundType);
					}
				}
			}
			for (int releasePass = 0; releasePass < 2; releasePass++)
			{
				if (this._typeToBoundVariables.Count > 0)
				{
					foreach (object boundVariable2 in new HashSet<object>(this._typeToBoundVariables.Values))
					{
						this.Release(boundVariable2);
					}
				}
			}
			this.ParentScope = null;
			foreach (IScopeObserver scopeObserver in this._observers)
			{
				scopeObserver.OnScopeReleased(this);
			}
			if (this._outstandingAllocationsByType != null)
			{
				if (this._outstandingAllocationsByType.Count > 0)
				{
					Scope.Log.Warn("Outstanding allocations in {0}:", new object[]
					{
						this
					});
					foreach (List<Scope.Allocation> list in this._outstandingAllocationsByType.Values)
					{
						foreach (Scope.Allocation allocation in list)
						{
							Scope.Log.Warn("{0}{1}", new object[]
							{
								allocation.obj,
								allocation.stackTrace
							});
						}
					}
				}
				this._outstandingAllocationsByType = null;
			}
			else if (this._outstandingAllocationCountsByType != null && this._outstandingAllocationCountsByType.Count > 0)
			{
				Scope.Log.Warn("Outstanding allocations in {0}:", new object[]
				{
					this
				});
				foreach (Type allocatedType in this._outstandingAllocationCountsByType.Keys)
				{
					int outstandingAllocationCount = this._outstandingAllocationCountsByType[allocatedType];
					Scope.Log.Warn("{0} instance{1} of {2}", new object[]
					{
						outstandingAllocationCount,
						(outstandingAllocationCount == 1) ? "" : "s",
						allocatedType
					});
				}
			}
			return true;
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x0003EB30 File Offset: 0x0003CD30
		public void Set(Type type, object variable)
		{
			this._typeToBoundVariables[type] = variable;
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x0003EB3F File Offset: 0x0003CD3F
		public void Set<T>(object variable)
		{
			this.Set(typeof(T), variable);
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0003EB52 File Offset: 0x0003CD52
		public void Unset(Type type)
		{
			this._typeToBoundVariables.Remove(type);
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0003EB64 File Offset: 0x0003CD64
		public T Import<T>(BinaryReader reader) where T : class
		{
			object obj = this._assembler.Import(new ImportContext(reader, this));
			if (obj != null && !typeof(T).IsAssignableFrom(obj.GetType()))
			{
				Scope.Log.Warn("Deserialisation of expected type {0} failed; got {1} instead.", new object[]
				{
					typeof(T),
					obj.GetType()
				});
				this.Release(obj);
				obj = null;
			}
			return obj as T;
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0003EBDE File Offset: 0x0003CDDE
		public object Import(BinaryReader reader)
		{
			return this._assembler.Import(new ImportContext(reader, this));
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0003EBF2 File Offset: 0x0003CDF2
		public bool Export(object obj, BinaryWriter writer)
		{
			return this._assembler.Export(obj, new ExportContext(writer, this));
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0003EC07 File Offset: 0x0003CE07
		public void Subscribe(IScopeObserver newObserver)
		{
			this._observers.Subscribe(newObserver);
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0003EC15 File Offset: 0x0003CE15
		public void Unsubscribe(IScopeObserver oldObserver)
		{
			this._observers.Unsubscribe(oldObserver);
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0003EC24 File Offset: 0x0003CE24
		public bool Release(object obj)
		{
			if (!Diagnostics.Verify(obj != null, "Please do not attempt to release a null object."))
			{
				return false;
			}
			bool success = true;
			if (!this._assembler.Release<object>(obj, this))
			{
				success = (this._parentScope != null && this._parentScope.Release(obj));
			}
			if (this._establishingObjectToChildScopes.ContainsKey(obj))
			{
				this._establishingObjectToChildScopes[obj].Release();
				this._establishingObjectToChildScopes.Remove(obj);
			}
			if (this._outstandingAllocationCountsByType != null)
			{
				Type concreteType = obj.GetType();
				int allocationCount;
				if (this._outstandingAllocationCountsByType.TryGetValue(concreteType, out allocationCount))
				{
					if (allocationCount == 1)
					{
						this._outstandingAllocationCountsByType.Remove(concreteType);
					}
					else
					{
						this._outstandingAllocationCountsByType[concreteType] = allocationCount - 1;
					}
				}
				if (this._outstandingAllocationsByType != null && this._outstandingAllocationsByType.ContainsKey(concreteType))
				{
					List<Scope.Allocation> allocations = this._outstandingAllocationsByType[concreteType];
					for (int allocationIndex = 0; allocationIndex < allocations.Count; allocationIndex++)
					{
						if (allocations[allocationIndex].obj == obj)
						{
							allocations.RemoveAt(allocationIndex);
							break;
						}
					}
					if (allocations.Count == 0)
					{
						this._outstandingAllocationsByType.Remove(concreteType);
					}
				}
			}
			if (!success)
			{
				Scope.Log.Error("Failed to release object {0} from scope with assembler '{1}'.", new object[]
				{
					obj,
					this._assembler.Name
				});
			}
			return success;
		}

		// Token: 0x0400102D RID: 4141
		private IScope _parentScope;

		// Token: 0x0400102E RID: 4142
		private readonly Dictionary<object, IScope> _establishingObjectToChildScopes = new Dictionary<object, IScope>();

		// Token: 0x0400102F RID: 4143
		private readonly object _establishingObject;

		// Token: 0x04001030 RID: 4144
		private readonly Assembler _assembler;

		// Token: 0x04001031 RID: 4145
		private readonly Dictionary<Type, object> _typeToBoundVariables = new Dictionary<Type, object>();

		// Token: 0x04001032 RID: 4146
		private readonly ObserverList<IScopeObserver> _observers = new ObserverList<IScopeObserver>(1);

		// Token: 0x04001033 RID: 4147
		private Dictionary<Type, int> _outstandingAllocationCountsByType;

		// Token: 0x04001034 RID: 4148
		private Dictionary<Type, List<Scope.Allocation>> _outstandingAllocationsByType;

		// Token: 0x04001035 RID: 4149
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Scope");

		// Token: 0x02000300 RID: 768
		private class Allocation
		{
			// Token: 0x04001036 RID: 4150
			public object obj;

			// Token: 0x04001037 RID: 4151
			public StackTrace stackTrace;
		}
	}
}
