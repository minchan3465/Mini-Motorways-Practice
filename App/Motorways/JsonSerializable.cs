using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Motorways
{
	// Token: 0x020003DC RID: 988
	public class JsonSerializable
	{
		// Token: 0x060017E4 RID: 6116 RVA: 0x00055168 File Offset: 0x00053368
		public void LoadFromJson(JSON.Dictionary jsonDictionary)
		{
			if (jsonDictionary == null || jsonDictionary.Keys.Count == 0)
			{
				return;
			}
			foreach (PropertyInfo property in this.GetJsonSerializableProperties())
			{
				string key = JsonSerializable.GetJsonSerializableName(property);
				if (jsonDictionary.ContainsKey(key))
				{
					if (property.PropertyType == typeof(int) || property.PropertyType == typeof(short) || property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
					{
						int value = jsonDictionary.GetInt(key, 0);
						property.SetValue(this, value);
					}
					else if (property.PropertyType == typeof(float))
					{
						property.SetValue(this, jsonDictionary.GetFloat(key, 0f));
					}
					else if (property.PropertyType == typeof(string))
					{
						property.SetValue(this, jsonDictionary.GetString(key));
					}
					else
					{
						Diagnostics.FailAssert("Type {0} not supported", new object[]
						{
							property.DeclaringType
						});
					}
				}
			}
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000552CC File Offset: 0x000534CC
		public void Merge(JsonSerializable other, DateTime ourTimestamp, DateTime theirTimestamp)
		{
			foreach (PropertyInfo property in this.GetJsonSerializableProperties())
			{
				JsonSerializableAttribute[] attributes = (JsonSerializableAttribute[])property.GetCustomAttributes(typeof(JsonSerializableAttribute), true);
				JsonSerializableAttribute.MergeStrategy mergeStrategy = attributes[0].mergeStrategy;
				object ourValue = property.GetValue(this);
				object theirValue = property.GetValue(other);
				bool isComparable = typeof(IComparable).IsAssignableFrom(property.PropertyType);
				object resultingValue;
				if (mergeStrategy == JsonSerializableAttribute.MergeStrategy.Max)
				{
					if (Diagnostics.Verify(isComparable, "Can't compare object of type {0}! Defaulting to our value", property.PropertyType))
					{
						resultingValue = (((ourValue as IComparable).CompareTo(theirValue as IComparable) >= 0) ? ourValue : theirValue);
					}
					else
					{
						resultingValue = ourValue;
					}
				}
				else if (mergeStrategy == JsonSerializableAttribute.MergeStrategy.Min)
				{
					if (Diagnostics.Verify(isComparable, "Can't compare object of type {0}! Defaulting to our value", property.PropertyType))
					{
						resultingValue = (((ourValue as IComparable).CompareTo(theirValue as IComparable) <= 0) ? ourValue : theirValue);
					}
					else
					{
						resultingValue = ourValue;
					}
				}
				else if (mergeStrategy == JsonSerializableAttribute.MergeStrategy.Latest)
				{
					resultingValue = ((ourTimestamp > theirTimestamp) ? ourValue : theirValue);
				}
				else
				{
					Diagnostics.FailAssert("Unknown merge strategy {0}, defaulting to our value", new object[]
					{
						attributes[0].mergeStrategy
					});
					resultingValue = ourValue;
				}
				property.SetValue(this, resultingValue);
			}
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x0005542C File Offset: 0x0005362C
		public Dictionary<string, object> Save()
		{
			Dictionary<string, object> result = new Dictionary<string, object>();
			foreach (PropertyInfo property in this.GetJsonSerializableProperties())
			{
				string key = JsonSerializable.GetJsonSerializableName(property);
				result.Add(key, property.GetValue(this));
			}
			return result;
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x00055490 File Offset: 0x00053690
		public IEnumerable<PropertyInfo> GetJsonSerializableProperties()
		{
			return from p in base.GetType().GetProperties()
			where p.IsDefined(typeof(JsonSerializableAttribute), true)
			select p;
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x000554C1 File Offset: 0x000536C1
		public static string GetJsonSerializableName(PropertyInfo property)
		{
			return ((JsonSerializableAttribute[])property.GetCustomAttributes(typeof(JsonSerializableAttribute), true))[0].serializedName;
		}
	}
}
