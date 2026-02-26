using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Factory
{
	// Token: 0x020002ED RID: 749
	public class CompositeSerializer : ISerializer
	{
		// Token: 0x0600126C RID: 4716 RVA: 0x0003D968 File Offset: 0x0003BB68
		public CompositeSerializer(Type type)
		{
			Type serialisingType = type;
			while (serialisingType != null)
			{
				foreach (FieldInfo field in serialisingType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (CompositeSerializer.Member.IsFieldSerialized(field))
					{
						CompositeSerializer.Member fieldMember = CompositeSerializer.Member.CreateField(field);
						if (Diagnostics.Verify(fieldMember != null, "Unable to create serializer for field {0} on type {1}.", field, serialisingType))
						{
							this._members.Add(fieldMember);
						}
					}
				}
				foreach (PropertyInfo property in serialisingType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (CompositeSerializer.Member.IsPropertySerialized(property))
					{
						CompositeSerializer.Member propertyMember = CompositeSerializer.Member.CreateProperty(type, property);
						if (Diagnostics.Verify(propertyMember != null, "Unable to create serializer for property {0} on type {1}.", property, serialisingType))
						{
							this._members.Add(propertyMember);
						}
					}
				}
				serialisingType = serialisingType.BaseType;
			}
			foreach (CompositeSerializer.Member member in this._members)
			{
				if (member.CanNestObjects)
				{
					if (this._nestingMembers == null)
					{
						this._nestingMembers = new List<CompositeSerializer.Member>();
					}
					this._nestingMembers.Add(member);
				}
			}
			this._hashCode = 1;
			foreach (CompositeSerializer.Member member2 in this._members)
			{
				this._hashCode = 31 * this._hashCode + member2.GetHashCode();
			}
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0003DB04 File Offset: 0x0003BD04
		public virtual bool Serialize(object obj, ExportContext context)
		{
			bool didMembersSerialize = true;
			foreach (CompositeSerializer.Member member in this._members)
			{
				didMembersSerialize = (member.Serialize(obj, context) && didMembersSerialize);
			}
			return didMembersSerialize;
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0003DB5C File Offset: 0x0003BD5C
		public virtual object Deserialize(object intoObject, ImportContext context)
		{
			bool didMembersDeserialize = true;
			foreach (CompositeSerializer.Member member in this._members)
			{
				didMembersDeserialize &= member.Deserialize(intoObject, context);
			}
			if (Diagnostics.Verify(didMembersDeserialize))
			{
				return intoObject;
			}
			return null;
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x0600126F RID: 4719 RVA: 0x0003DBC0 File Offset: 0x0003BDC0
		public bool CanNestObjects
		{
			get
			{
				return this._nestingMembers != null;
			}
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x0003DBCB File Offset: 0x0003BDCB
		public IEnumerable<object> GetNestedObjects(object obj)
		{
			if (this._nestingMembers == null)
			{
				yield break;
			}
			foreach (CompositeSerializer.Member member in this._nestingMembers)
			{
				foreach (object nestedObj in member.GetNestedObjects(obj))
				{
					yield return nestedObj;
				}
				IEnumerator<object> enumerator2 = null;
			}
			List<CompositeSerializer.Member>.Enumerator enumerator = default(List<CompositeSerializer.Member>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x0003DBE2 File Offset: 0x0003BDE2
		public override int GetHashCode()
		{
			return this._hashCode;
		}

		// Token: 0x04001004 RID: 4100
		private readonly List<CompositeSerializer.Member> _members = new List<CompositeSerializer.Member>();

		// Token: 0x04001005 RID: 4101
		private readonly List<CompositeSerializer.Member> _nestingMembers;

		// Token: 0x04001006 RID: 4102
		private readonly int _hashCode;

		// Token: 0x020002EE RID: 750
		private class Member
		{
			// Token: 0x06001272 RID: 4722 RVA: 0x0003DBEC File Offset: 0x0003BDEC
			public static bool IsPropertySerialized(PropertyInfo property)
			{
				SerializeAttribute attribute = property.GetCustomAttribute<SerializeAttribute>();
				return attribute != null && attribute.IsSerialized;
			}

			// Token: 0x06001273 RID: 4723 RVA: 0x0003DC0C File Offset: 0x0003BE0C
			public static CompositeSerializer.Member CreateProperty(Type declaringType, PropertyInfo property)
			{
				MethodInfo getMethod = property.GetGetMethod();
				MethodInfo setMethod = property.CanWrite ? property.GetSetMethod() : null;
				ISerializer serializer = null;
				SerializeAttribute attribute = property.GetCustomAttribute<SerializeAttribute>();
				if (attribute != null)
				{
					serializer = attribute.CustomSerializer;
				}
				Action<object, object> setDelegate;
				if (setMethod != null)
				{
					setDelegate = Assembler.CreateSetDelegate(declaringType, setMethod);
				}
				else
				{
					setDelegate = delegate(object target, object param)
					{
						property.SetValue(target, param, BindingFlags.Instance | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture);
					};
				}
				if (getMethod == null || setDelegate == null)
				{
					return null;
				}
				if (serializer == null)
				{
					serializer = SerializerLibrary.GetSerializer(property.PropertyType);
					if (serializer == null)
					{
						return null;
					}
				}
				int hashCode = TypeUtilities.CalculateMD5(property.Name) ^ TypeUtilities.CalculateMD5(property.PropertyType.FullName);
				return new CompositeSerializer.Member(serializer, Assembler.CreateGetDelegate(declaringType, getMethod), setDelegate, hashCode);
			}

			// Token: 0x06001274 RID: 4724 RVA: 0x0003DCEC File Offset: 0x0003BEEC
			public static bool IsFieldSerialized(FieldInfo field)
			{
				if (field.IsDefined(typeof(CompilerGeneratedAttribute), true))
				{
					return false;
				}
				SerializeAttribute attribute = field.GetCustomAttribute<SerializeAttribute>();
				if (attribute != null)
				{
					if (!attribute.IsSerialized)
					{
						return false;
					}
				}
				else if (field.GetCustomAttribute<DependencyAttribute>() != null)
				{
					return false;
				}
				return true;
			}

			// Token: 0x06001275 RID: 4725 RVA: 0x0003DD30 File Offset: 0x0003BF30
			public static CompositeSerializer.Member CreateField(FieldInfo field)
			{
				ISerializer serializer = null;
				SerializeAttribute attribute = field.GetCustomAttribute<SerializeAttribute>();
				if (attribute != null)
				{
					serializer = attribute.CustomSerializer;
				}
				if (serializer == null)
				{
					serializer = SerializerLibrary.GetSerializer(field.FieldType);
					if (serializer == null)
					{
						return null;
					}
				}
				Action<object, object> setDelegate = new Action<object, object>(field.SetValue);
				if (field.IsInitOnly)
				{
					setDelegate = null;
				}
				int hashCode = TypeUtilities.CalculateMD5(field.Name) ^ TypeUtilities.CalculateMD5(field.FieldType.FullName);
				return new CompositeSerializer.Member(serializer, new Func<object, object>(field.GetValue), setDelegate, hashCode);
			}

			// Token: 0x06001276 RID: 4726 RVA: 0x0003DDAE File Offset: 0x0003BFAE
			private Member(ISerializer serializer, Func<object, object> getDelegate, Action<object, object> setDelegate, int hashCode)
			{
				this._serializer = serializer;
				this._getDelegate = getDelegate;
				this._setDelegate = setDelegate;
				this._hashCode = hashCode;
			}

			// Token: 0x06001277 RID: 4727 RVA: 0x0003DDD3 File Offset: 0x0003BFD3
			public bool Serialize(object obj, ExportContext context)
			{
				return this._serializer.Serialize(this._getDelegate(obj), context);
			}

			// Token: 0x06001278 RID: 4728 RVA: 0x0003DDF0 File Offset: 0x0003BFF0
			public bool Deserialize(object obj, ImportContext context)
			{
				object val = this._serializer.Deserialize(this._getDelegate(obj), context);
				if (this._setDelegate != null)
				{
					this._setDelegate(obj, val);
				}
				return true;
			}

			// Token: 0x170003A8 RID: 936
			// (get) Token: 0x06001279 RID: 4729 RVA: 0x0003DE2C File Offset: 0x0003C02C
			public bool CanNestObjects
			{
				get
				{
					return this._serializer.CanNestObjects;
				}
			}

			// Token: 0x0600127A RID: 4730 RVA: 0x0003DE39 File Offset: 0x0003C039
			public IEnumerable<object> GetNestedObjects(object obj)
			{
				foreach (object nestedObj in this._serializer.GetNestedObjects(this._getDelegate(obj)))
				{
					yield return nestedObj;
				}
				IEnumerator<object> enumerator = null;
				yield break;
				yield break;
			}

			// Token: 0x0600127B RID: 4731 RVA: 0x0003DE50 File Offset: 0x0003C050
			public override int GetHashCode()
			{
				return this._hashCode;
			}

			// Token: 0x04001007 RID: 4103
			private readonly Func<object, object> _getDelegate;

			// Token: 0x04001008 RID: 4104
			private readonly Action<object, object> _setDelegate;

			// Token: 0x04001009 RID: 4105
			private readonly ISerializer _serializer;

			// Token: 0x0400100A RID: 4106
			private readonly int _hashCode;
		}
	}
}
