using System;

namespace Motorways.Audio
{
	// Token: 0x02000631 RID: 1585
	public class Attribute
	{
		// Token: 0x06002C31 RID: 11313 RVA: 0x000C40AC File Offset: 0x000C22AC
		public bool GetBool(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.BOOLEAN)
			{
				return (bool)this.val;
			}
			if (this.type == Attribute.ValueType.STRING && loadout != null)
			{
				Attribute constant = loadout.GetConstant(this.GetString(null));
				if (constant != null)
				{
					return constant.GetBool(null);
				}
			}
			Diagnostics.FailAssert("GetFloat() failed for attribute {0}.", new object[]
			{
				this
			});
			return false;
		}

		// Token: 0x06002C32 RID: 11314 RVA: 0x000C4108 File Offset: 0x000C2308
		public int GetInt(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.INTEGER)
			{
				return (int)this.val;
			}
			if (this.type == Attribute.ValueType.STRING && loadout != null)
			{
				Attribute constant = loadout.GetConstant(this.GetString(null));
				if (constant != null)
				{
					return constant.GetInt(null);
				}
			}
			Diagnostics.FailAssert("GetInt() failed for attribute {0}.", new object[]
			{
				this
			});
			return 0;
		}

		// Token: 0x06002C33 RID: 11315 RVA: 0x000C4164 File Offset: 0x000C2364
		public int[] GetIntArray(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.INTEGER_ARRAY)
			{
				return this.val as int[];
			}
			if (this.type == Attribute.ValueType.INTEGER)
			{
				return new int[]
				{
					this.GetInt(null)
				};
			}
			if (this.type == Attribute.ValueType.STRING && loadout != null)
			{
				Attribute constant = loadout.GetConstant(this.GetString(null));
				if (constant != null)
				{
					return constant.GetIntArray(null);
				}
			}
			Diagnostics.FailAssert("GetIntArray() failed for attribute {0}.", new object[]
			{
				this
			});
			return null;
		}

		// Token: 0x06002C34 RID: 11316 RVA: 0x000C41DC File Offset: 0x000C23DC
		public float GetFloat(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.FLOAT)
			{
				return (float)this.val;
			}
			if (this.type == Attribute.ValueType.INTEGER)
			{
				return (float)this.GetInt(null);
			}
			if (this.type == Attribute.ValueType.STRING && loadout != null)
			{
				Attribute constant = loadout.GetConstant(this.GetString(null));
				if (constant != null)
				{
					return constant.GetFloat(null);
				}
			}
			Diagnostics.FailAssert("GetFloat() failed for attribute {0}.", new object[]
			{
				this
			});
			return 0f;
		}

		// Token: 0x06002C35 RID: 11317 RVA: 0x000C4250 File Offset: 0x000C2450
		public float[] GetFloatArray(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.FLOAT_ARRAY)
			{
				return this.val as float[];
			}
			if (this.type == Attribute.ValueType.FLOAT)
			{
				return new float[]
				{
					this.GetFloat(null)
				};
			}
			if (this.type == Attribute.ValueType.INTEGER_ARRAY)
			{
				int[] intArray = this.val as int[];
				float[] floatArray = new float[intArray.Length];
				for (int i = 0; i < intArray.Length; i++)
				{
					floatArray[i] = (float)intArray[i];
				}
				return floatArray;
			}
			if (this.type == Attribute.ValueType.INTEGER)
			{
				return new float[]
				{
					(float)this.GetInt(null)
				};
			}
			if (this.type == Attribute.ValueType.STRING && loadout != null)
			{
				Attribute constant = loadout.GetConstant(this.GetString(null));
				if (constant != null)
				{
					return constant.GetFloatArray(null);
				}
			}
			Diagnostics.FailAssert("GetFloatArray() failed for attribute {0}.", new object[]
			{
				this
			});
			return null;
		}

		// Token: 0x06002C36 RID: 11318 RVA: 0x000C4318 File Offset: 0x000C2518
		public string GetString(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.STRING)
			{
				if (loadout != null)
				{
					Attribute constant = loadout.GetConstant(this.GetString(null));
					if (constant != null)
					{
						return constant.GetString(null);
					}
				}
				return this.val as string;
			}
			Diagnostics.FailAssert("GetString() failed for attribute {0}.", new object[]
			{
				this
			});
			return null;
		}

		// Token: 0x06002C37 RID: 11319 RVA: 0x000C436C File Offset: 0x000C256C
		public string[] GetStringArray(AudioLoadout loadout = null)
		{
			if (this.type == Attribute.ValueType.STRING_ARRAY)
			{
				return this.val as string[];
			}
			if (this.type == Attribute.ValueType.STRING)
			{
				if (loadout != null)
				{
					Attribute constant = loadout.GetConstant(this.GetString(null));
					if (constant != null)
					{
						return constant.GetStringArray(null);
					}
				}
				return new string[]
				{
					this.GetString(null)
				};
			}
			Diagnostics.FailAssert("GetFloat() failed for attribute {0}.", new object[]
			{
				this
			});
			return null;
		}

		// Token: 0x06002C38 RID: 11320 RVA: 0x000C43D9 File Offset: 0x000C25D9
		public override string ToString()
		{
			return string.Format("[Attribute Type={0}, Value={1}]", this.type, this.val);
		}

		// Token: 0x06002C39 RID: 11321 RVA: 0x000C43F8 File Offset: 0x000C25F8
		public static Attribute FromJSON(object jsonAttribute)
		{
			if (jsonAttribute == null)
			{
				return null;
			}
			Attribute attribute = new Attribute();
			if (jsonAttribute is bool)
			{
				attribute.type = Attribute.ValueType.BOOLEAN;
				attribute.val = (bool)jsonAttribute;
			}
			else if (jsonAttribute is long)
			{
				attribute.type = Attribute.ValueType.INTEGER;
				attribute.val = Convert.ToInt32((long)jsonAttribute);
			}
			else if (jsonAttribute is string)
			{
				attribute.type = Attribute.ValueType.STRING;
				attribute.val = string.Copy(jsonAttribute as string);
			}
			else
			{
				if (!(jsonAttribute is JSON.Array))
				{
					try
					{
						attribute.type = Attribute.ValueType.FLOAT;
						attribute.val = Convert.ToSingle(jsonAttribute);
					}
					catch
					{
						return null;
					}
					return attribute;
				}
				JSON.Array jsonArray = jsonAttribute as JSON.Array;
				if (jsonArray.Count <= 0)
				{
					return null;
				}
				if (jsonArray[0] is string)
				{
					attribute.type = Attribute.ValueType.STRING_ARRAY;
					string[] array = new string[jsonArray.Count];
					for (int i = 0; i < jsonArray.Count; i++)
					{
						array[i] = jsonArray.GetString(i);
					}
					attribute.val = array;
				}
				else
				{
					attribute.type = Attribute.ValueType.INTEGER_ARRAY;
					for (int j = 0; j < jsonArray.Count; j++)
					{
						if (!(jsonArray[j] is long))
						{
							attribute.type = Attribute.ValueType.FLOAT_ARRAY;
							break;
						}
					}
					if (attribute.type == Attribute.ValueType.INTEGER_ARRAY)
					{
						int[] array2 = new int[jsonArray.Count];
						for (int k = 0; k < jsonArray.Count; k++)
						{
							array2[k] = jsonArray.GetInt(k);
						}
						attribute.val = array2;
					}
					else
					{
						float[] array3 = new float[jsonArray.Count];
						for (int l = 0; l < jsonArray.Count; l++)
						{
							array3[l] = jsonArray.GetFloat(l);
						}
						attribute.val = array3;
					}
				}
			}
			return attribute;
		}

		// Token: 0x04002673 RID: 9843
		private Attribute.ValueType type;

		// Token: 0x04002674 RID: 9844
		private object val;

		// Token: 0x02000632 RID: 1586
		private enum ValueType
		{
			// Token: 0x04002676 RID: 9846
			BOOLEAN,
			// Token: 0x04002677 RID: 9847
			INTEGER,
			// Token: 0x04002678 RID: 9848
			INTEGER_ARRAY,
			// Token: 0x04002679 RID: 9849
			FLOAT,
			// Token: 0x0400267A RID: 9850
			FLOAT_ARRAY,
			// Token: 0x0400267B RID: 9851
			STRING,
			// Token: 0x0400267C RID: 9852
			STRING_ARRAY
		}
	}
}
