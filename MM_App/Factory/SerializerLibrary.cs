using System;
using System.Collections.Generic;
using FixMath;
using UnityEngine;

namespace Factory
{
	// Token: 0x02000303 RID: 771
	public static class SerializerLibrary
	{
		// Token: 0x060012E7 RID: 4839 RVA: 0x0003EDEC File Offset: 0x0003CFEC
		public static void RegisterSerializer<T>(ISerializer serializer)
		{
			SerializerLibrary.RegisterSerializer(typeof(T), serializer);
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0003EDFE File Offset: 0x0003CFFE
		public static void RegisterSerializer(Type type, ISerializer serializer)
		{
			SerializerLibrary._typeSerializers[type] = serializer;
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0003EE0C File Offset: 0x0003D00C
		public static ISerializer GetSerializer<T>()
		{
			return SerializerLibrary.GetSerializer(typeof(T));
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x0003EE20 File Offset: 0x0003D020
		public static ISerializer GetSerializer(Type type)
		{
			if (SerializerLibrary._objectSerializer == null)
			{
				SerializerLibrary.RegisterSerializer<bool>(new SerializerLibrary.BoolSerializer());
				SerializerLibrary.RegisterSerializer<char>(new SerializerLibrary.CharSerializer());
				SerializerLibrary.RegisterSerializer<short>(new SerializerLibrary.Int16Serializer());
				SerializerLibrary.RegisterSerializer<int>(new SerializerLibrary.Int32Serializer());
				SerializerLibrary.RegisterSerializer<uint>(new SerializerLibrary.UInt32Serializer());
				SerializerLibrary.RegisterSerializer<long>(new SerializerLibrary.Int64Serializer());
				SerializerLibrary.RegisterSerializer<ulong>(new SerializerLibrary.UInt64Serializer());
				SerializerLibrary.RegisterSerializer<Fix64>(new SerializerLibrary.Fix64Serializer());
				SerializerLibrary.RegisterSerializer<float>(new SerializerLibrary.SingleSerializer());
				SerializerLibrary.RegisterSerializer<double>(new SerializerLibrary.DoubleSerializer());
				SerializerLibrary.RegisterSerializer<string>(new SerializerLibrary.StringSerializer());
				SerializerLibrary.RegisterSerializer<DateTime>(new SerializerLibrary.DateTimeSerializer());
				SerializerLibrary.RegisterSerializer<Vector2>(new SerializerLibrary.Vector2Serializer());
				SerializerLibrary.RegisterSerializer<Vector2Int>(new SerializerLibrary.Vector2IntSerializer());
				SerializerLibrary.RegisterSerializer<Vector2Fixed>(new SerializerLibrary.Vector2FixedSerializer());
				SerializerLibrary.RegisterSerializer<Vector3>(new SerializerLibrary.Vector3Serializer());
				SerializerLibrary.RegisterSerializer<Vector3Fixed>(new SerializerLibrary.Vector3FixedSerializer());
				SerializerLibrary.RegisterSerializer<RectInt>(new SerializerLibrary.RectIntSerializer());
				SerializerLibrary.RegisterSerializer<Type>(new SerializerLibrary.TypeIdSerializer());
				SerializerLibrary._objectSerializer = new SerializerLibrary.ObjectSerializer();
			}
			if (SerializerLibrary._typeSerializers.ContainsKey(type))
			{
				return SerializerLibrary._typeSerializers[type];
			}
			if (type.IsEnum)
			{
				return SerializerLibrary.GetSerializer(Enum.GetUnderlyingType(type));
			}
			if (type.IsArray)
			{
				return Activator.CreateInstance(typeof(SerializerLibrary.ArraySerializer<>).MakeGenericType(new Type[]
				{
					type.GetElementType()
				})) as ISerializer;
			}
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return Activator.CreateInstance(typeof(SerializerLibrary.ListSerializer<>).MakeGenericType(new Type[]
					{
						type.GetGenericArguments()[0]
					})) as ISerializer;
				}
				if (type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
				{
					return Activator.CreateInstance(typeof(SerializerLibrary.DictionarySerializer<, >).MakeGenericType(new Type[]
					{
						type.GetGenericArguments()[0],
						type.GetGenericArguments()[1]
					})) as ISerializer;
				}
			}
			if (type.IsClass || type.IsInterface)
			{
				return SerializerLibrary._objectSerializer;
			}
			return null;
		}

		// Token: 0x0400103B RID: 4155
		private static Dictionary<Type, ISerializer> _typeSerializers = new Dictionary<Type, ISerializer>();

		// Token: 0x0400103C RID: 4156
		private static ISerializer _objectSerializer;

		// Token: 0x0400103D RID: 4157
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Serializer");

		// Token: 0x02000304 RID: 772
		private class BoolSerializer : PrimitiveSerializer
		{
			// Token: 0x060012EC RID: 4844 RVA: 0x0003F029 File Offset: 0x0003D229
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((bool)obj);
				return true;
			}

			// Token: 0x060012ED RID: 4845 RVA: 0x0003F03D File Offset: 0x0003D23D
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadBoolean();
			}
		}

		// Token: 0x02000305 RID: 773
		private class CharSerializer : PrimitiveSerializer
		{
			// Token: 0x060012EF RID: 4847 RVA: 0x0003F04F File Offset: 0x0003D24F
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((char)obj);
				return true;
			}

			// Token: 0x060012F0 RID: 4848 RVA: 0x0003F063 File Offset: 0x0003D263
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadChar();
			}
		}

		// Token: 0x02000306 RID: 774
		private class Int16Serializer : PrimitiveSerializer
		{
			// Token: 0x060012F2 RID: 4850 RVA: 0x0003F075 File Offset: 0x0003D275
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((short)obj);
				return true;
			}

			// Token: 0x060012F3 RID: 4851 RVA: 0x0003F089 File Offset: 0x0003D289
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadInt16();
			}
		}

		// Token: 0x02000307 RID: 775
		private class Int32Serializer : PrimitiveSerializer
		{
			// Token: 0x060012F5 RID: 4853 RVA: 0x0003F09B File Offset: 0x0003D29B
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((int)obj);
				return true;
			}

			// Token: 0x060012F6 RID: 4854 RVA: 0x0003F0AF File Offset: 0x0003D2AF
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadInt32();
			}
		}

		// Token: 0x02000308 RID: 776
		private class UInt32Serializer : PrimitiveSerializer
		{
			// Token: 0x060012F8 RID: 4856 RVA: 0x0003F0C1 File Offset: 0x0003D2C1
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((uint)obj);
				return true;
			}

			// Token: 0x060012F9 RID: 4857 RVA: 0x0003F0D5 File Offset: 0x0003D2D5
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadUInt32();
			}
		}

		// Token: 0x02000309 RID: 777
		private class Int64Serializer : PrimitiveSerializer
		{
			// Token: 0x060012FB RID: 4859 RVA: 0x0003F0E7 File Offset: 0x0003D2E7
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((long)obj);
				return true;
			}

			// Token: 0x060012FC RID: 4860 RVA: 0x0003F0FB File Offset: 0x0003D2FB
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadInt64();
			}
		}

		// Token: 0x0200030A RID: 778
		private class UInt64Serializer : PrimitiveSerializer
		{
			// Token: 0x060012FE RID: 4862 RVA: 0x0003F10D File Offset: 0x0003D30D
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((ulong)obj);
				return true;
			}

			// Token: 0x060012FF RID: 4863 RVA: 0x0003F121 File Offset: 0x0003D321
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadUInt64();
			}
		}

		// Token: 0x0200030B RID: 779
		private class Fix64Serializer : PrimitiveSerializer
		{
			// Token: 0x06001301 RID: 4865 RVA: 0x0003F134 File Offset: 0x0003D334
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write(((Fix64)obj).RawValue);
				return true;
			}

			// Token: 0x06001302 RID: 4866 RVA: 0x0003F15B File Offset: 0x0003D35B
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return Fix64.FromRaw(context.Reader.ReadInt64());
			}
		}

		// Token: 0x0200030C RID: 780
		private class SingleSerializer : PrimitiveSerializer
		{
			// Token: 0x06001304 RID: 4868 RVA: 0x0003F172 File Offset: 0x0003D372
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((float)obj);
				return true;
			}

			// Token: 0x06001305 RID: 4869 RVA: 0x0003F186 File Offset: 0x0003D386
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadSingle();
			}
		}

		// Token: 0x0200030D RID: 781
		private class StringSerializer : PrimitiveSerializer
		{
			// Token: 0x06001307 RID: 4871 RVA: 0x0003F198 File Offset: 0x0003D398
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((string)obj);
				return true;
			}

			// Token: 0x06001308 RID: 4872 RVA: 0x0003F1AC File Offset: 0x0003D3AC
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadString();
			}
		}

		// Token: 0x0200030E RID: 782
		private class DateTimeSerializer : PrimitiveSerializer
		{
			// Token: 0x0600130A RID: 4874 RVA: 0x0003F1BC File Offset: 0x0003D3BC
			public override bool Serialize(object obj, ExportContext context)
			{
				long binaryDateTime = ((DateTime)obj).ToBinary();
				context.Writer.Write(binaryDateTime);
				return true;
			}

			// Token: 0x0600130B RID: 4875 RVA: 0x0003F1E5 File Offset: 0x0003D3E5
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return DateTime.FromBinary(context.Reader.ReadInt64());
			}
		}

		// Token: 0x0200030F RID: 783
		private class DoubleSerializer : PrimitiveSerializer
		{
			// Token: 0x0600130D RID: 4877 RVA: 0x0003F1FC File Offset: 0x0003D3FC
			public override bool Serialize(object obj, ExportContext context)
			{
				context.Writer.Write((double)obj);
				return true;
			}

			// Token: 0x0600130E RID: 4878 RVA: 0x0003F210 File Offset: 0x0003D410
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return context.Reader.ReadDouble();
			}
		}

		// Token: 0x02000310 RID: 784
		private class Vector2Serializer : PrimitiveSerializer
		{
			// Token: 0x06001310 RID: 4880 RVA: 0x0003F224 File Offset: 0x0003D424
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is Vector2)
				{
					Vector2 vector = (Vector2)obj;
					context.Writer.Write(vector.x);
					context.Writer.Write(vector.y);
					return true;
				}
				return false;
			}

			// Token: 0x06001311 RID: 4881 RVA: 0x0003F268 File Offset: 0x0003D468
			public override object Deserialize(object existingObj, ImportContext context)
			{
				float x = context.Reader.ReadSingle();
				float y = context.Reader.ReadSingle();
				return new Vector2(x, y);
			}
		}

		// Token: 0x02000311 RID: 785
		private class Vector2IntSerializer : PrimitiveSerializer
		{
			// Token: 0x06001313 RID: 4883 RVA: 0x0003F298 File Offset: 0x0003D498
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is Vector2Int)
				{
					Vector2Int vector = (Vector2Int)obj;
					context.Writer.Write(vector.x);
					context.Writer.Write(vector.y);
					return true;
				}
				return false;
			}

			// Token: 0x06001314 RID: 4884 RVA: 0x0003F2DC File Offset: 0x0003D4DC
			public override object Deserialize(object existingObj, ImportContext context)
			{
				int x = context.Reader.ReadInt32();
				int y = context.Reader.ReadInt32();
				return new Vector2Int(x, y);
			}
		}

		// Token: 0x02000312 RID: 786
		private class Vector2FixedSerializer : PrimitiveSerializer
		{
			// Token: 0x06001316 RID: 4886 RVA: 0x0003F30C File Offset: 0x0003D50C
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is Vector2Fixed)
				{
					Vector2Fixed vector = (Vector2Fixed)obj;
					context.Writer.Write(vector.x.RawValue);
					context.Writer.Write(vector.y.RawValue);
					return true;
				}
				return false;
			}

			// Token: 0x06001317 RID: 4887 RVA: 0x0003F35C File Offset: 0x0003D55C
			public override object Deserialize(object existingObj, ImportContext context)
			{
				Fix64 x = Fix64.FromRaw(context.Reader.ReadInt64());
				Fix64 y = Fix64.FromRaw(context.Reader.ReadInt64());
				return new Vector2Fixed(x, y);
			}
		}

		// Token: 0x02000313 RID: 787
		private class Vector3Serializer : PrimitiveSerializer
		{
			// Token: 0x06001319 RID: 4889 RVA: 0x0003F398 File Offset: 0x0003D598
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					context.Writer.Write(vector.x);
					context.Writer.Write(vector.y);
					context.Writer.Write(vector.z);
					return true;
				}
				return false;
			}

			// Token: 0x0600131A RID: 4890 RVA: 0x0003F3F0 File Offset: 0x0003D5F0
			public override object Deserialize(object existingObj, ImportContext context)
			{
				float x = context.Reader.ReadSingle();
				float y = context.Reader.ReadSingle();
				float z = context.Reader.ReadSingle();
				return new Vector3(x, y, z);
			}
		}

		// Token: 0x02000314 RID: 788
		private class Vector3FixedSerializer : PrimitiveSerializer
		{
			// Token: 0x0600131C RID: 4892 RVA: 0x0003F42C File Offset: 0x0003D62C
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is Vector3Fixed)
				{
					Vector3Fixed vector = (Vector3Fixed)obj;
					context.Writer.Write(vector.x.RawValue);
					context.Writer.Write(vector.y.RawValue);
					context.Writer.Write(vector.z.RawValue);
					return true;
				}
				return false;
			}

			// Token: 0x0600131D RID: 4893 RVA: 0x0003F490 File Offset: 0x0003D690
			public override object Deserialize(object existingObj, ImportContext context)
			{
				Fix64 xValue = Fix64.FromRaw(context.Reader.ReadInt64());
				Fix64 y = Fix64.FromRaw(context.Reader.ReadInt64());
				Fix64 z = Fix64.FromRaw(context.Reader.ReadInt64());
				return new Vector3Fixed(xValue, y, z);
			}
		}

		// Token: 0x02000315 RID: 789
		private class RectIntSerializer : PrimitiveSerializer
		{
			// Token: 0x0600131F RID: 4895 RVA: 0x0003F4DC File Offset: 0x0003D6DC
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is RectInt)
				{
					RectInt rect = (RectInt)obj;
					context.Writer.Write(rect.xMin);
					context.Writer.Write(rect.yMin);
					context.Writer.Write(rect.width);
					context.Writer.Write(rect.height);
					return true;
				}
				return false;
			}

			// Token: 0x06001320 RID: 4896 RVA: 0x0003F544 File Offset: 0x0003D744
			public override object Deserialize(object existingObj, ImportContext context)
			{
				int xMin = context.Reader.ReadInt32();
				int yMin = context.Reader.ReadInt32();
				int width = context.Reader.ReadInt32();
				int height = context.Reader.ReadInt32();
				return new RectInt(xMin, yMin, width, height);
			}
		}

		// Token: 0x02000316 RID: 790
		private class TypeIdSerializer : PrimitiveSerializer
		{
			// Token: 0x06001322 RID: 4898 RVA: 0x0003F590 File Offset: 0x0003D790
			public override bool Serialize(object obj, ExportContext context)
			{
				Type type = obj as Type;
				if (!Diagnostics.Verify(type != null, "TypeIdSerializer unable to convert {0} to System.Type.", obj))
				{
					return false;
				}
				context.Writer.Write(TypeUtilities.GetTypeId(type));
				return true;
			}

			// Token: 0x06001323 RID: 4899 RVA: 0x0003F5CC File Offset: 0x0003D7CC
			public override object Deserialize(object existingObj, ImportContext context)
			{
				int typeId = context.Reader.ReadInt32();
				return context.Scope.Assembler.TranslateTypeId(typeId);
			}
		}

		// Token: 0x02000317 RID: 791
		private class ObjectSerializer : ISerializer
		{
			// Token: 0x06001325 RID: 4901 RVA: 0x0003F5F8 File Offset: 0x0003D7F8
			public bool Serialize(object obj, ExportContext context)
			{
				if (obj == null)
				{
					context.Writer.Write(0);
					return true;
				}
				int objectId = context.Library.GetObjectId(obj);
				context.Writer.Write(objectId);
				return true;
			}

			// Token: 0x06001326 RID: 4902 RVA: 0x0003F630 File Offset: 0x0003D830
			public object Deserialize(object existingObj, ImportContext context)
			{
				return context.GetObject(context.Reader.ReadInt32());
			}

			// Token: 0x170003BE RID: 958
			// (get) Token: 0x06001327 RID: 4903 RVA: 0x000020AA File Offset: 0x000002AA
			public bool CanNestObjects
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06001328 RID: 4904 RVA: 0x0003F643 File Offset: 0x0003D843
			public IEnumerable<object> GetNestedObjects(object obj)
			{
				yield return obj;
				yield break;
			}
		}

		// Token: 0x02000319 RID: 793
		public class ArraySerializer<T> : ISerializer
		{
			// Token: 0x06001332 RID: 4914 RVA: 0x0003F707 File Offset: 0x0003D907
			public ArraySerializer()
			{
				this._itemSerializer = SerializerLibrary.GetSerializer(typeof(T));
			}

			// Token: 0x06001333 RID: 4915 RVA: 0x0003F724 File Offset: 0x0003D924
			public bool Serialize(object obj, ExportContext context)
			{
				T[] array = obj as T[];
				if (array != null)
				{
					context.Writer.Write(array.Length);
					foreach (T entry in array)
					{
						this._itemSerializer.Serialize(entry, context);
					}
					return true;
				}
				if (obj != null)
				{
					return false;
				}
				context.Writer.Write(0);
				return true;
			}

			// Token: 0x06001334 RID: 4916 RVA: 0x0003F78C File Offset: 0x0003D98C
			public object Deserialize(object existingObj, ImportContext context)
			{
				int arrayLength = context.Reader.ReadInt32();
				if (arrayLength < 0)
				{
					SerializerLibrary.Log.Error("An array of {0} was deserialized with {1} elements. It will be set to zero, but likely indicates that the array is being deserialized from the wrong point in the byte stream.", new object[]
					{
						typeof(T),
						arrayLength
					});
					arrayLength = 0;
				}
				T[] array = null;
				if (existingObj != null)
				{
					array = (existingObj as T[]);
					if (array != null && array.Length != arrayLength)
					{
						array = null;
					}
				}
				if (array == null)
				{
					array = new T[arrayLength];
				}
				for (int itemIndex = 0; itemIndex < arrayLength; itemIndex++)
				{
					object val = this._itemSerializer.Deserialize(null, context);
					if (val == null)
					{
						SerializerLibrary.Log.Warn("Failed to deserialise item #{0} in list of {1}.", new object[]
						{
							itemIndex,
							typeof(T)
						});
					}
					array[itemIndex] = (T)((object)val);
				}
				return array;
			}

			// Token: 0x170003C1 RID: 961
			// (get) Token: 0x06001335 RID: 4917 RVA: 0x0003F84D File Offset: 0x0003DA4D
			public bool CanNestObjects
			{
				get
				{
					return this._itemSerializer.CanNestObjects;
				}
			}

			// Token: 0x06001336 RID: 4918 RVA: 0x0003F85A File Offset: 0x0003DA5A
			public IEnumerable<object> GetNestedObjects(object obj)
			{
				T[] array = obj as T[];
				if (array != null)
				{
					foreach (T entry in array)
					{
						foreach (object nestedObj in this._itemSerializer.GetNestedObjects(entry))
						{
							yield return nestedObj;
						}
						IEnumerator<object> enumerator = null;
					}
					T[] array2 = null;
				}
				yield break;
				yield break;
			}

			// Token: 0x04001043 RID: 4163
			private readonly ISerializer _itemSerializer;
		}

		// Token: 0x0200031B RID: 795
		public class ListSerializer<T> : ISerializer
		{
			// Token: 0x06001340 RID: 4928 RVA: 0x0003FA57 File Offset: 0x0003DC57
			public ListSerializer()
			{
				this._itemSerializer = SerializerLibrary.GetSerializer(typeof(T));
			}

			// Token: 0x06001341 RID: 4929 RVA: 0x0003FA74 File Offset: 0x0003DC74
			public bool Serialize(object obj, ExportContext context)
			{
				List<T> list = obj as List<T>;
				if (list != null)
				{
					context.Writer.Write(list.Count);
					foreach (T entry in list)
					{
						this._itemSerializer.Serialize(entry, context);
					}
					return true;
				}
				if (obj != null)
				{
					return false;
				}
				context.Writer.Write(-1);
				return true;
			}

			// Token: 0x06001342 RID: 4930 RVA: 0x0003FB00 File Offset: 0x0003DD00
			public object Deserialize(object existingObj, ImportContext context)
			{
				int listCount = context.Reader.ReadInt32();
				List<T> list = existingObj as List<T>;
				if (list != null)
				{
					list.Clear();
				}
				else
				{
					list = ((listCount >= 0) ? new List<T>(listCount) : null);
				}
				if (listCount < 0 || list == null)
				{
					return list;
				}
				for (int itemIndex = 0; itemIndex < listCount; itemIndex++)
				{
					object val = this._itemSerializer.Deserialize(null, context);
					if (val == null)
					{
						SerializerLibrary.Log.Warn("Failed to deserialise item #{0} in array of {1}.", new object[]
						{
							itemIndex,
							typeof(T)
						});
					}
					list.Add((T)((object)val));
				}
				return list;
			}

			// Token: 0x170003C4 RID: 964
			// (get) Token: 0x06001343 RID: 4931 RVA: 0x0003FB97 File Offset: 0x0003DD97
			public bool CanNestObjects
			{
				get
				{
					return this._itemSerializer.CanNestObjects;
				}
			}

			// Token: 0x06001344 RID: 4932 RVA: 0x0003FBA4 File Offset: 0x0003DDA4
			public IEnumerable<object> GetNestedObjects(object obj)
			{
				List<T> array = obj as List<T>;
				if (array != null)
				{
					foreach (T entry in array)
					{
						foreach (object nestedObj in this._itemSerializer.GetNestedObjects(entry))
						{
							yield return nestedObj;
						}
						IEnumerator<object> enumerator2 = null;
					}
					List<T>.Enumerator enumerator = default(List<T>.Enumerator);
				}
				yield break;
				yield break;
			}

			// Token: 0x0400104D RID: 4173
			private readonly ISerializer _itemSerializer;
		}

		// Token: 0x0200031D RID: 797
		public class DictionarySerializer<TKey, TValue> : ISerializer
		{
			// Token: 0x0600134F RID: 4943 RVA: 0x0003FDD7 File Offset: 0x0003DFD7
			public DictionarySerializer()
			{
				this._keySerializer = SerializerLibrary.GetSerializer(typeof(TKey));
				this._valueSerializer = SerializerLibrary.GetSerializer(typeof(TValue));
			}

			// Token: 0x06001350 RID: 4944 RVA: 0x0003FE0C File Offset: 0x0003E00C
			public bool Serialize(object obj, ExportContext context)
			{
				Dictionary<TKey, TValue> dictionary = obj as Dictionary<TKey, TValue>;
				if (dictionary != null)
				{
					context.Writer.Write(dictionary.Count);
					foreach (KeyValuePair<TKey, TValue> entry in dictionary)
					{
						this._keySerializer.Serialize(entry.Key, context);
						this._valueSerializer.Serialize(entry.Value, context);
					}
					return true;
				}
				if (obj != null)
				{
					return false;
				}
				context.Writer.Write(0);
				return true;
			}

			// Token: 0x06001351 RID: 4945 RVA: 0x0003FEB8 File Offset: 0x0003E0B8
			public object Deserialize(object existingObj, ImportContext context)
			{
				int entryCount = context.Reader.ReadInt32();
				Dictionary<TKey, TValue> dictionary = existingObj as Dictionary<TKey, TValue>;
				if (dictionary != null)
				{
					dictionary.Clear();
				}
				else
				{
					dictionary = new Dictionary<TKey, TValue>(entryCount);
				}
				if (entryCount > 0)
				{
					if (this._keySerializer is SerializerLibrary.ObjectSerializer)
					{
						List<object> keys = new List<object>(entryCount);
						List<object> values = new List<object>(entryCount);
						for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
						{
							object key = this._keySerializer.Deserialize(null, context);
							object val = this._valueSerializer.Deserialize(null, context);
							keys.Add(key);
							values.Add(val);
						}
						context.AddUnmappedDictionary(dictionary, keys, values);
					}
					else
					{
						for (int entryIndex2 = 0; entryIndex2 < entryCount; entryIndex2++)
						{
							object key2 = this._keySerializer.Deserialize(null, context);
							object val2 = this._valueSerializer.Deserialize(null, context);
							if (key2 == null || val2 == null)
							{
								return null;
							}
							dictionary[(TKey)((object)key2)] = (TValue)((object)val2);
						}
					}
				}
				return dictionary;
			}

			// Token: 0x170003C7 RID: 967
			// (get) Token: 0x06001352 RID: 4946 RVA: 0x0003FFA2 File Offset: 0x0003E1A2
			public bool CanNestObjects
			{
				get
				{
					return this._keySerializer.CanNestObjects || this._valueSerializer.CanNestObjects;
				}
			}

			// Token: 0x06001353 RID: 4947 RVA: 0x0003FFBE File Offset: 0x0003E1BE
			public IEnumerable<object> GetNestedObjects(object obj)
			{
				Dictionary<TKey, TValue> dictionary = obj as Dictionary<TKey, TValue>;
				if (dictionary != null)
				{
					if (this._keySerializer.CanNestObjects)
					{
						foreach (TKey key in dictionary.Keys)
						{
							foreach (object nestedObj in this._keySerializer.GetNestedObjects(key))
							{
								yield return nestedObj;
							}
							IEnumerator<object> enumerator2 = null;
						}
						Dictionary<TKey, TValue>.KeyCollection.Enumerator enumerator = default(Dictionary<TKey, TValue>.KeyCollection.Enumerator);
					}
					if (this._valueSerializer.CanNestObjects)
					{
						foreach (TValue val in dictionary.Values)
						{
							foreach (object nestedObj2 in this._valueSerializer.GetNestedObjects(val))
							{
								yield return nestedObj2;
							}
							IEnumerator<object> enumerator2 = null;
						}
						Dictionary<TKey, TValue>.ValueCollection.Enumerator enumerator3 = default(Dictionary<TKey, TValue>.ValueCollection.Enumerator);
					}
				}
				yield break;
				yield break;
			}

			// Token: 0x04001056 RID: 4182
			private readonly ISerializer _keySerializer;

			// Token: 0x04001057 RID: 4183
			private readonly ISerializer _valueSerializer;
		}
	}
}
