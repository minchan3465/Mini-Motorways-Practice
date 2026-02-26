using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Factory.Allocators;
using JetBrains.Annotations;

namespace Factory
{
	// Token: 0x020002E1 RID: 737
	public class Assembler : IDisposable
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x0600121C RID: 4636 RVA: 0x0003C308 File Offset: 0x0003A508
		// (set) Token: 0x0600121D RID: 4637 RVA: 0x0003C310 File Offset: 0x0003A510
		public bool IsValidatingObjectScrubbing { get; set; }

		// Token: 0x0600121E RID: 4638 RVA: 0x0003C319 File Offset: 0x0003A519
		public Assembler(string name)
		{
			this.Name = name;
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x0600121F RID: 4639 RVA: 0x0003C349 File Offset: 0x0003A549
		// (set) Token: 0x06001220 RID: 4640 RVA: 0x0003C351 File Offset: 0x0003A551
		public string Name { get; private set; }

		// Token: 0x06001221 RID: 4641 RVA: 0x0003C35C File Offset: 0x0003A55C
		public Assembler.TypeConfigurator<TConcrete> Register<TInterface, TConcrete>() where TConcrete : class, TInterface
		{
			Type interfaceType = typeof(TInterface);
			Type concreteType = typeof(TConcrete);
			if (typeof(TInterface) != typeof(TConcrete))
			{
				Assembler.Log.Info("Creating TypeAssembler for {0}, bound to interface {1}.", new object[]
				{
					typeof(TConcrete),
					typeof(TInterface)
				});
			}
			else
			{
				Assembler.Log.Info("Creating TypeAssembler for {0}.", new object[]
				{
					typeof(TConcrete)
				});
			}
			Assembler.TypeAssembler<TConcrete> typeAssembler = new Assembler.TypeAssembler<TConcrete>(interfaceType);
			this._typeAssemblers[interfaceType] = typeAssembler;
			if (concreteType != interfaceType)
			{
				this._typeAssemblers[concreteType] = typeAssembler;
			}
			if (TypeUtilities.GetCustomAttribute<SerializableAttribute>(concreteType) != null)
			{
				ITypeSerializer typeSerializer = new TypeSerializer<TConcrete>();
				this._typeSerializers[typeSerializer.TypeId] = typeSerializer;
				this._typeIds[concreteType] = typeSerializer.TypeId;
				int serializerHashCode = typeSerializer.GetHashCode();
				this._globalTypeSerializerHashCode ^= serializerHashCode;
				Assembler.Log.Info("Creating TypeSerializer for {0} with a hash code of {1}. The assembler's serializer hash code is now {2}.", new object[]
				{
					typeof(TConcrete),
					serializerHashCode,
					this._globalTypeSerializerHashCode
				});
			}
			return new Assembler.TypeConfigurator<TConcrete>(this, interfaceType);
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0003C4A1 File Offset: 0x0003A6A1
		public Assembler.TypeConfigurator<T> Register<T>() where T : class
		{
			return this.Register<T, T>();
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0003C4A9 File Offset: 0x0003A6A9
		public T Create<T>(IScope scope) where T : class
		{
			return this.Create(typeof(T), scope) as T;
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x0003C4C8 File Offset: 0x0003A6C8
		public object Create(Type type, IScope scope)
		{
			Assembler.ITypeAssembler typeAssembler;
			if (!this._typeAssemblers.TryGetValue(type, out typeAssembler))
			{
				return null;
			}
			return typeAssembler.Create(scope);
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x0003C4F0 File Offset: 0x0003A6F0
		public void Assemble([NotNull] object obj, IScope scope)
		{
			Assembler.ITypeAssembler typeAssembler;
			if (!this._typeAssemblers.TryGetValue(obj.GetType(), out typeAssembler))
			{
				Diagnostics.FailAssert(string.Format("{0} could not assemble {1}.", this.Name, obj), Array.Empty<object>());
				return;
			}
			typeAssembler.Assemble(obj, scope);
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x0003C538 File Offset: 0x0003A738
		public Assembler GetAssemblerForType(Type type)
		{
			Assembler.ITypeAssembler typeAssembler;
			if (!this._typeAssemblers.TryGetValue(type, out typeAssembler))
			{
				return null;
			}
			Assembler assemblerForType = typeAssembler.ScopeAssembler;
			if (assemblerForType != null)
			{
				return assemblerForType;
			}
			return this;
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0003C564 File Offset: 0x0003A764
		public IEnumerable<Type> GetRegisteredTypesAssignableToType(Type type)
		{
			foreach (Type baseType in this._typeAssemblers.Keys)
			{
				if (type.IsAssignableFrom(baseType))
				{
					yield return baseType;
				}
			}
			Dictionary<Type, Assembler.ITypeAssembler>.KeyCollection.Enumerator enumerator = default(Dictionary<Type, Assembler.ITypeAssembler>.KeyCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0003C57C File Offset: 0x0003A77C
		public object Import(ImportContext context)
		{
			object result;
			try
			{
				result = this.ImportUnsafe(context);
			}
			catch (Exception e)
			{
				Diagnostics.FailAssert("{0}", new object[]
				{
					e
				});
				result = null;
			}
			return result;
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x0003C5C0 File Offset: 0x0003A7C0
		public void Dispose()
		{
			foreach (Assembler.ITypeAssembler typeAssembler in this._typeAssemblers.Values)
			{
				typeAssembler.Dispose();
			}
			this._typeAssemblers.Clear();
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0003C620 File Offset: 0x0003A820
		private object ImportUnsafe(ImportContext context)
		{
			List<ITypeSerializer> typeSerializers = new List<ITypeSerializer>();
			List<List<object>> typedObjects = new List<List<object>>();
			Stopwatch stopwatch = Stopwatch.StartNew();
			Stopwatch instancingStopwatch = Stopwatch.StartNew();
			long initialStreamPosition = context.Reader.BaseStream.Position;
			long totalImportSize = context.Reader.ReadInt64();
			if (context.Reader.BaseStream.Length - initialStreamPosition < totalImportSize)
			{
				Assembler.Log.Error("Malformed stream encountered during import. Total import size is reported as {0} bytes, but only {1} bytes left are the stream.", new object[]
				{
					totalImportSize,
					context.Reader.BaseStream.Length - initialStreamPosition
				});
				return null;
			}
			int importedGlobalTypeSerializerHashCode = context.Reader.ReadInt32();
			if (importedGlobalTypeSerializerHashCode != this._globalTypeSerializerHashCode)
			{
				Assembler.Log.Info("Unable to import stream as the stream's global serializer hash code ({0}) differs from ours ({1}).", new object[]
				{
					importedGlobalTypeSerializerHashCode,
					this._globalTypeSerializerHashCode
				});
				return null;
			}
			int rootObjectVersion = context.Reader.ReadInt32();
			int typeCount = context.Reader.ReadInt32();
			for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
			{
				int typeId = context.Reader.ReadInt32();
				ITypeSerializer serializer = this.GetSerializer(typeId);
				if (!Diagnostics.Verify(serializer != null, "Unable to import type with id {0}.", typeId))
				{
					return null;
				}
				int serializerHashCode = context.Reader.ReadInt32();
				if (serializer.GetHashCode() != serializerHashCode)
				{
					Assembler.Log.Info("Unable to import type {0} because the serializer hash codes differ. Theirs is {1}, ours is {2}.", new object[]
					{
						serializer.Type,
						serializerHashCode,
						serializer.GetHashCode()
					});
					return null;
				}
				if (typeIndex == 0 && serializer.Version != rootObjectVersion)
				{
					Assembler.Log.Info("Unable to import root object type of version {0} with local serializer of version {1}.", new object[]
					{
						rootObjectVersion,
						serializer.Version
					});
					return null;
				}
				int objectCount = context.Reader.ReadInt32();
				List<object> objectsOfType = new List<object>();
				for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
				{
					object newObject = context.Scope.Get(serializer.Type);
					context.AddObject(newObject);
					objectsOfType.Add(newObject);
				}
				typeSerializers.Add(serializer);
				typedObjects.Add(objectsOfType);
			}
			instancingStopwatch.Stop();
			bool didObjectsDeserialise = true;
			int typeIndex2 = 0;
			while (typeIndex2 < typeSerializers.Count && didObjectsDeserialise)
			{
				ITypeSerializer serializer2 = typeSerializers[typeIndex2];
				List<object> objectsOfType2 = typedObjects[typeIndex2];
				try
				{
					foreach (object obj in objectsOfType2)
					{
						if (serializer2.Deserialize(obj, context) == null)
						{
							Assembler.Log.Error("Object of type {0} failed to deserialise.", new object[]
							{
								serializer2.Type
							});
							didObjectsDeserialise = false;
							break;
						}
					}
				}
				catch (InvalidCastException exception)
				{
					Assembler.Log.Error("Caught exception while during deserialisation.\n{0}", new object[]
					{
						exception
					});
					didObjectsDeserialise = false;
				}
				typeIndex2++;
			}
			if (!didObjectsDeserialise)
			{
				foreach (List<object> list in typedObjects)
				{
					foreach (object obj2 in list)
					{
						context.Scope.Release(obj2);
					}
				}
				return null;
			}
			context.MapDictionaries();
			for (int typeIndex3 = 0; typeIndex3 < typeSerializers.Count; typeIndex3++)
			{
				ITypeSerializer serializer3 = typeSerializers[typeIndex3];
				if (typeof(IDeserializedHandler).IsAssignableFrom(serializer3.Type))
				{
					foreach (object obj3 in typedObjects[typeIndex3])
					{
						IDeserializedHandler handler = obj3 as IDeserializedHandler;
						if (Diagnostics.Verify(handler != null, "Unable to find IDeserializedHandler interface on {0}.", obj3))
						{
							handler.OnDeserialized(context.Scope);
						}
					}
				}
			}
			object rootObject = null;
			if (typedObjects.Count > 0 && typedObjects[0].Count > 0)
			{
				rootObject = typedObjects[0][0];
			}
			stopwatch.Stop();
			long totalTime = stopwatch.ElapsedTicks;
			long instancingTime = instancingStopwatch.ElapsedTicks;
			Assembler.Log.Info("Deserialized {0}:\n\tinstancing: {1:0.00}s ({2:00}%)\n\tdeserialising: {3:0.00}s ({4:00}%)", new object[]
			{
				(rootObject != null) ? rootObject.GetType() : null,
				(float)instancingTime / (float)Stopwatch.Frequency,
				(float)instancingTime / (float)totalTime * 100f,
				(float)(totalTime - instancingTime) / (float)Stopwatch.Frequency,
				(float)(totalTime - instancingTime) / (float)totalTime * 100f
			});
			return rootObject;
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0003CB10 File Offset: 0x0003AD10
		public bool Export(object obj, ExportContext context)
		{
			ExportContext.ObjectLibrary objectLibrary = context.Library;
			Stopwatch stopwatch = Stopwatch.StartNew();
			Stopwatch collationStopwatch = Stopwatch.StartNew();
			long initialStreamPosition = context.Writer.BaseStream.Position;
			long totalExportSize = 0L;
			context.Writer.Write(totalExportSize);
			context.Writer.Write(this._globalTypeSerializerHashCode);
			ITypeSerializer rootObjectSerializer = this.GetSerializer(obj.GetType());
			if (!Diagnostics.Verify(rootObjectSerializer != null, "Cannot find type serializer for root object {0}.", obj))
			{
				return false;
			}
			context.Writer.Write(rootObjectSerializer.Version);
			int objectCount = 0;
			List<object> collatedObjects = new List<object>();
			collatedObjects.Add(obj);
			while (collatedObjects.Count > 0)
			{
				objectCount++;
				object collatedObject = collatedObjects[collatedObjects.Count - 1];
				collatedObjects.RemoveAt(collatedObjects.Count - 1);
				if (!objectLibrary.ContainsObject(collatedObject))
				{
					objectLibrary.AddObject(collatedObject);
					ITypeSerializer serializer = this.GetSerializer(collatedObject.GetType());
					if (!Diagnostics.Verify(serializer != null, "Cannot find type serializer for {0}.", collatedObject))
					{
						return false;
					}
					foreach (object nestedObject in serializer.GetNestedObjects(collatedObject))
					{
						if (nestedObject != null)
						{
							collatedObjects.Add(nestedObject);
						}
					}
				}
			}
			objectLibrary.BuildIndex();
			context.Writer.Write(objectLibrary.Types.Count);
			foreach (Type objectType in objectLibrary.Types)
			{
				context.Writer.Write(this._typeIds[objectType]);
				context.Writer.Write(this.GetSerializer(objectType).GetHashCode());
				context.Writer.Write(objectLibrary.GetObjectsOfType(objectType).Count);
			}
			long lastStreamPosition = context.Writer.BaseStream.Position;
			collationStopwatch.Stop();
			Assembler.Log.Info("Collated {0} objects in {1:0.00}s. Table of contents is {2} bytes.", new object[]
			{
				objectCount,
				collationStopwatch.ElapsedTicks / Stopwatch.Frequency,
				lastStreamPosition - initialStreamPosition
			});
			bool didObjectsSerialize = true;
			foreach (Type objectType2 in objectLibrary.Types)
			{
				Stopwatch typeSerializerStopwatch = Stopwatch.StartNew();
				objectCount = 0;
				ITypeSerializer serializer2 = this.GetSerializer(objectType2);
				foreach (object objToSerialize in objectLibrary.GetObjectsOfType(objectType2))
				{
					objectCount++;
					if (!serializer2.Serialize(objToSerialize, context))
					{
						didObjectsSerialize = false;
						break;
					}
				}
				typeSerializerStopwatch.Stop();
				Assembler.Log.Info("Serialized {0} x {1} in {2:0.00}s, {3} bytes.", new object[]
				{
					objectCount,
					objectType2,
					typeSerializerStopwatch.ElapsedTicks / Stopwatch.Frequency,
					context.Writer.BaseStream.Position - lastStreamPosition
				});
				lastStreamPosition = context.Writer.BaseStream.Position;
				if (!didObjectsSerialize)
				{
					break;
				}
			}
			if (!didObjectsSerialize)
			{
				Assembler.Log.Info("Failed to serialize!", Array.Empty<object>());
				stopwatch.Stop();
				return false;
			}
			context.Writer.BaseStream.Position = initialStreamPosition;
			totalExportSize = lastStreamPosition - initialStreamPosition;
			context.Writer.Write(totalExportSize);
			context.Writer.BaseStream.Position = lastStreamPosition;
			stopwatch.Stop();
			Assembler.Log.Info("Serialized {0} in {1:0.00}s total, {2} bytes.", new object[]
			{
				obj.GetType(),
				stopwatch.ElapsedTicks / Stopwatch.Frequency,
				context.Writer.BaseStream.Position - initialStreamPosition
			});
			return true;
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x0003CF28 File Offset: 0x0003B128
		public bool Release<T>(T obj, IScope context)
		{
			Assembler.ITypeAssembler assembler;
			if (this._typeAssemblers.TryGetValue(obj.GetType(), out assembler))
			{
				assembler.Release(obj, context);
				return true;
			}
			return false;
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x0003CF64 File Offset: 0x0003B164
		public Type TranslateTypeId(int typeId)
		{
			ITypeSerializer serializer = this.GetSerializer(typeId);
			if (Diagnostics.Verify(serializer != null, "Cannot determine type for unknown type id {0}.", typeId))
			{
				return serializer.Type;
			}
			return null;
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x0003CF97 File Offset: 0x0003B197
		public int GlobalTypeSerializerHashCode
		{
			get
			{
				return this._globalTypeSerializerHashCode;
			}
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x0003CFA0 File Offset: 0x0003B1A0
		private Assembler.ITypeAssembler GetAssembler(Type type)
		{
			Assembler.ITypeAssembler typeAssembler;
			if (this._typeAssemblers.TryGetValue(type, out typeAssembler))
			{
				return typeAssembler;
			}
			return null;
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x0003CFC0 File Offset: 0x0003B1C0
		public static Func<object, object> CreateGetDelegate(Type declaringType, MethodInfo method)
		{
			return (Func<object, object>)typeof(Assembler).GetMethod("CreateGenericGetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
			{
				declaringType,
				method.ReturnType
			}).Invoke(null, new object[]
			{
				method
			});
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x0003D010 File Offset: 0x0003B210
		private static Func<object, object> CreateGenericGetDelegate<TTarget, TReturn>(MethodInfo method) where TTarget : class
		{
			Func<TTarget, TReturn> typedDelegate = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), method);
			return (object target) => typedDelegate((TTarget)((object)target));
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x0003D040 File Offset: 0x0003B240
		public static Action<object, object> CreateSetDelegate(Type declaringType, MethodInfo method)
		{
			return (Action<object, object>)typeof(Assembler).GetMethod("CreateGenericSetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
			{
				declaringType,
				method.GetParameters()[0].ParameterType
			}).Invoke(null, new object[]
			{
				method
			});
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x0003D097 File Offset: 0x0003B297
		private static Action<object, object> CreateGenericSetDelegate<TTarget, TParam>(MethodInfo method) where TTarget : class
		{
			Action<TTarget, TParam> typedDelegate = (Action<TTarget, TParam>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam>), method);
			return delegate(object target, object param)
			{
				typedDelegate((TTarget)((object)target), (TParam)((object)param));
			};
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0003D0C4 File Offset: 0x0003B2C4
		private ITypeSerializer GetSerializer(Type objectType)
		{
			int typeId;
			ITypeSerializer typeSerializer;
			if (this._typeIds.TryGetValue(objectType, out typeId) && this._typeSerializers.TryGetValue(typeId, out typeSerializer))
			{
				return typeSerializer;
			}
			return null;
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x0003D0F4 File Offset: 0x0003B2F4
		private ITypeSerializer GetSerializer(int typeId)
		{
			ITypeSerializer typeSerializer;
			if (this._typeSerializers.TryGetValue(typeId, out typeSerializer))
			{
				return typeSerializer;
			}
			return null;
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x0003D114 File Offset: 0x0003B314
		public static void DontCall_EnsureAOTGenericCallsAreCompiled<TTarget, TParam>() where TTarget : class
		{
			Assembler.CreateGenericGetDelegate<TTarget, TParam>(null);
			Assembler.CreateGenericSetDelegate<TTarget, TParam>(null);
		}

		// Token: 0x04000FDE RID: 4062
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Assembler");

		// Token: 0x04000FDF RID: 4063
		private readonly Dictionary<Type, Assembler.ITypeAssembler> _typeAssemblers = new Dictionary<Type, Assembler.ITypeAssembler>();

		// Token: 0x04000FE0 RID: 4064
		private readonly Dictionary<int, ITypeSerializer> _typeSerializers = new Dictionary<int, ITypeSerializer>();

		// Token: 0x04000FE1 RID: 4065
		private readonly Dictionary<Type, int> _typeIds = new Dictionary<Type, int>();

		// Token: 0x04000FE2 RID: 4066
		private int _globalTypeSerializerHashCode;

		// Token: 0x020002E2 RID: 738
		private interface ITypeAssembler : IDisposable
		{
			// Token: 0x06001238 RID: 4664
			object Create(IScope context);

			// Token: 0x06001239 RID: 4665
			void Assemble([NotNull] object obj, IScope context);

			// Token: 0x0600123A RID: 4666
			bool Release(object obj, IScope context);

			// Token: 0x1700039D RID: 925
			// (get) Token: 0x0600123B RID: 4667
			Assembler ScopeAssembler { get; }
		}

		// Token: 0x020002E3 RID: 739
		private class TypeAssembler<T> : Assembler.ITypeAssembler, IDisposable where T : class
		{
			// Token: 0x1700039E RID: 926
			// (get) Token: 0x0600123C RID: 4668 RVA: 0x0003D135 File Offset: 0x0003B335
			// (set) Token: 0x0600123D RID: 4669 RVA: 0x0003D13D File Offset: 0x0003B33D
			public Binding Binding { get; set; }

			// Token: 0x1700039F RID: 927
			// (get) Token: 0x0600123E RID: 4670 RVA: 0x0003D146 File Offset: 0x0003B346
			// (set) Token: 0x0600123F RID: 4671 RVA: 0x0003D14E File Offset: 0x0003B34E
			public IAllocator<T> Allocator { get; set; }

			// Token: 0x170003A0 RID: 928
			// (get) Token: 0x06001240 RID: 4672 RVA: 0x0003D157 File Offset: 0x0003B357
			// (set) Token: 0x06001241 RID: 4673 RVA: 0x0003D15F File Offset: 0x0003B35F
			public bool EstablishesScope { get; set; }

			// Token: 0x170003A1 RID: 929
			// (get) Token: 0x06001242 RID: 4674 RVA: 0x0003D168 File Offset: 0x0003B368
			// (set) Token: 0x06001243 RID: 4675 RVA: 0x0003D170 File Offset: 0x0003B370
			public Assembler ScopeAssembler { get; set; }

			// Token: 0x06001244 RID: 4676 RVA: 0x0003D17C File Offset: 0x0003B37C
			public TypeAssembler(Type interfaceType)
			{
				this.Binding = Binding.Free;
				Type concreteType = typeof(T);
				if (concreteType != interfaceType)
				{
					if (interfaceType.IsInterface)
					{
						Type boundType = concreteType;
						while (Array.IndexOf<Type>(boundType.GetInterfaces(), interfaceType) >= 0)
						{
							this._interfaceTypes.Add(boundType);
							boundType = boundType.BaseType;
						}
					}
					else
					{
						Type boundType2 = concreteType;
						while (boundType2 != interfaceType)
						{
							this._interfaceTypes.Add(boundType2);
							boundType2 = boundType2.BaseType;
						}
					}
				}
				this._interfaceTypes.Add(interfaceType);
				while (concreteType != null)
				{
					foreach (FieldInfo field in concreteType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (field.IsDefined(typeof(DependencyAttribute), false))
						{
							Assembler.TypeAssembler<T>.Dependency fieldDependency = Assembler.TypeAssembler<T>.Dependency.CreateField(field);
							this.AddDependency(fieldDependency);
						}
					}
					foreach (PropertyInfo property in concreteType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (property.IsDefined(typeof(DependencyAttribute), false))
						{
							Assembler.TypeAssembler<T>.Dependency propertyDependency = Assembler.TypeAssembler<T>.Dependency.CreateProperty(property);
							this.AddDependency(propertyDependency);
						}
					}
					concreteType = concreteType.BaseType;
				}
				this._hasCreatedHandler = typeof(ICreatedInScopeHandler).IsAssignableFrom(typeof(T));
				this._hasReleasedHandler = typeof(IReleasedFromScopeHandler).IsAssignableFrom(typeof(T));
			}

			// Token: 0x06001245 RID: 4677 RVA: 0x0003D2F8 File Offset: 0x0003B4F8
			public object Create(IScope scope)
			{
				T newObject = this.Allocator.Allocate(scope);
				if (newObject == null)
				{
					return null;
				}
				IScope assemblingScope = scope;
				if (this.EstablishesScope)
				{
					Scope newScope = new Scope(this.ScopeAssembler ?? scope.Assembler, newObject);
					newScope.ParentScope = scope;
					scope.AddChildScope(newScope, newObject);
					scope = newScope;
				}
				if (scope == null)
				{
					return newObject;
				}
				IScope boundScope = null;
				if (this.Binding == Binding.Scope)
				{
					boundScope = assemblingScope;
				}
				else if (this.Binding == Binding.EstablishedScope)
				{
					boundScope = scope;
				}
				if (boundScope != null)
				{
					foreach (Type interfaceType in this._interfaceTypes)
					{
						boundScope.Set(interfaceType, newObject);
					}
				}
				this.Assemble(newObject, scope);
				this.Allocator.OnObjectAssembled(newObject, scope);
				return newObject;
			}

			// Token: 0x06001246 RID: 4678 RVA: 0x0003D3F0 File Offset: 0x0003B5F0
			public void Assemble(object obj, IScope scope)
			{
				if (this._dependencies != null)
				{
					foreach (Assembler.TypeAssembler<T>.Dependency dependency in this._dependencies)
					{
						int length = dependency.GetLengthDelegate(obj);
						for (int index = 0; index < length; index++)
						{
							object service = scope.Get(dependency.Type);
							dependency.SetDelegate(obj, index, service);
						}
					}
				}
				if (this._hasCreatedHandler)
				{
					(obj as ICreatedInScopeHandler).OnCreatedInScope(scope);
				}
			}

			// Token: 0x06001247 RID: 4679 RVA: 0x0003D490 File Offset: 0x0003B690
			public bool Release(object obj, IScope scope)
			{
				if (this._hasReleasedHandler)
				{
					(obj as IReleasedFromScopeHandler).OnReleasedFromScope(scope);
				}
				bool success = this.Allocator.Release((T)((object)obj), scope);
				if (this.Binding == Binding.Scope)
				{
					foreach (Type interfaceType in this._interfaceTypes)
					{
						scope.Unset(interfaceType);
					}
				}
				return success;
			}

			// Token: 0x06001248 RID: 4680 RVA: 0x0003D514 File Offset: 0x0003B714
			public void Dispose()
			{
				if (this.Allocator != null)
				{
					this.Allocator.Dispose();
					this.Allocator = null;
				}
			}

			// Token: 0x06001249 RID: 4681 RVA: 0x0003D530 File Offset: 0x0003B730
			private void AddDependency(Assembler.TypeAssembler<T>.Dependency dependency)
			{
				if (this._dependencies == null)
				{
					this._dependencies = new List<Assembler.TypeAssembler<T>.Dependency>();
				}
				this._dependencies.Add(dependency);
			}

			// Token: 0x04000FE9 RID: 4073
			private List<Type> _interfaceTypes = new List<Type>();

			// Token: 0x04000FEA RID: 4074
			private List<Assembler.TypeAssembler<T>.Dependency> _dependencies;

			// Token: 0x04000FEB RID: 4075
			private bool _hasCreatedHandler;

			// Token: 0x04000FEC RID: 4076
			private bool _hasReleasedHandler;

			// Token: 0x020002E4 RID: 740
			private class Dependency
			{
				// Token: 0x170003A2 RID: 930
				// (get) Token: 0x0600124A RID: 4682 RVA: 0x0003D551 File Offset: 0x0003B751
				public Type Type { get; }

				// Token: 0x170003A3 RID: 931
				// (get) Token: 0x0600124B RID: 4683 RVA: 0x0003D559 File Offset: 0x0003B759
				public Action<object, int, object> SetDelegate { get; }

				// Token: 0x170003A4 RID: 932
				// (get) Token: 0x0600124C RID: 4684 RVA: 0x0003D561 File Offset: 0x0003B761
				public Func<object, int> GetLengthDelegate { get; }

				// Token: 0x0600124D RID: 4685 RVA: 0x0003D56C File Offset: 0x0003B76C
				public static Assembler.TypeAssembler<T>.Dependency CreateField(FieldInfo field)
				{
					if (field.FieldType.IsArray)
					{
						Action<object, int, object> setDelegate = delegate(object target, int index, object param)
						{
							(field.GetValue(target) as IList)[index] = param;
						};
						Func<object, int> getLengthDelegate = (object target) => (field.GetValue(target) as IList).Count;
						return new Assembler.TypeAssembler<T>.Dependency(field.FieldType.GetElementType(), setDelegate, getLengthDelegate);
					}
					Action<object, object> setDelegate2 = delegate(object target, object param)
					{
						field.SetValue(target, param);
					};
					return new Assembler.TypeAssembler<T>.Dependency(field.FieldType, setDelegate2);
				}

				// Token: 0x0600124E RID: 4686 RVA: 0x0003D5E8 File Offset: 0x0003B7E8
				public static Assembler.TypeAssembler<T>.Dependency CreateProperty(PropertyInfo property)
				{
					MethodInfo setMethod = property.GetSetMethod(true);
					if (setMethod == null)
					{
						Assembler.Log.Error("Unable to get set method for property {0}.", new object[]
						{
							property
						});
						return null;
					}
					return new Assembler.TypeAssembler<T>.Dependency(property.PropertyType, Assembler.CreateSetDelegate(typeof(T), setMethod));
				}

				// Token: 0x0600124F RID: 4687 RVA: 0x0003D63C File Offset: 0x0003B83C
				private Dependency(Type type, Action<object, object> setDelegate)
				{
					this.Type = type;
					this.SetDelegate = delegate(object target, int index, object param)
					{
						setDelegate(target, param);
					};
					this.GetLengthDelegate = this.DefaultGetLengthDelegate;
				}

				// Token: 0x06001250 RID: 4688 RVA: 0x0003D6A8 File Offset: 0x0003B8A8
				private Dependency(Type elementType, Action<object, int, object> setDelegate, Func<object, int> getLengthDelegate)
				{
					this.Type = elementType;
					this.SetDelegate = setDelegate;
					this.GetLengthDelegate = getLengthDelegate;
				}

				// Token: 0x04000FF0 RID: 4080
				private readonly Func<object, int> DefaultGetLengthDelegate = (object target) => 1;
			}
		}

		// Token: 0x020002E8 RID: 744
		public class TypeConfigurator<T> where T : class
		{
			// Token: 0x0600125B RID: 4699 RVA: 0x0003D751 File Offset: 0x0003B951
			public TypeConfigurator(Assembler assembler, Type interfaceType)
			{
				this._typeAssembler = (assembler._typeAssemblers[interfaceType] as Assembler.TypeAssembler<T>);
			}

			// Token: 0x0600125C RID: 4700 RVA: 0x0003D770 File Offset: 0x0003B970
			public Assembler.TypeConfigurator<T> Binding(Binding binding)
			{
				this._typeAssembler.Binding = binding;
				return this;
			}

			// Token: 0x0600125D RID: 4701 RVA: 0x0003D77F File Offset: 0x0003B97F
			public Assembler.TypeConfigurator<T> Allocator(IAllocator<T> allocator)
			{
				this._typeAssembler.Allocator = allocator;
				return this;
			}

			// Token: 0x0600125E RID: 4702 RVA: 0x0003D78E File Offset: 0x0003B98E
			public Assembler.TypeConfigurator<T> EstablishScope(Assembler assembler = null)
			{
				this._typeAssembler.EstablishesScope = true;
				this._typeAssembler.ScopeAssembler = assembler;
				return this;
			}

			// Token: 0x04000FF6 RID: 4086
			private readonly Assembler.TypeAssembler<T> _typeAssembler;
		}
	}
}
