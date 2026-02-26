using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class MotorwaysStringKey : StringKey
{
	// Token: 0x06000E8A RID: 3722 RVA: 0x00031649 File Offset: 0x0002F849
	public MotorwaysStringKey()
	{
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x00031651 File Offset: 0x0002F851
	public MotorwaysStringKey(StringId newId, Dictionary<StringParameterId, string> newParameters = null)
	{
		this.BasicInit(newId, newParameters);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x00031661 File Offset: 0x0002F861
	public MotorwaysStringKey(StringId newId, int newCount, Dictionary<StringParameterId, string> newParameters = null)
	{
		this.IntInit(newId, newCount, newParameters);
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00031672 File Offset: 0x0002F872
	public MotorwaysStringKey(StringId newId, float newCount, Dictionary<StringParameterId, string> newParameters = null)
	{
		this.FloatInit(newId, newCount, newParameters);
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00031683 File Offset: 0x0002F883
	public void BasicInit(StringId newId, Dictionary<StringParameterId, string> newParameters = null)
	{
		this.id = newId;
		this.parameters = MotorwaysStringKey.ConvertToStringDictionary(newParameters);
		this.count = 0;
		this.isPlural = false;
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x000316A6 File Offset: 0x0002F8A6
	public void IntInit(StringId newId, int newCount, Dictionary<StringParameterId, string> newParameters = null)
	{
		this.id = newId;
		this.parameters = MotorwaysStringKey.ConvertToStringDictionary(newParameters);
		this.count = newCount;
		this.isPlural = true;
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x000316CC File Offset: 0x0002F8CC
	public void FloatInit(StringId newId, float newCount, Dictionary<StringParameterId, string> newParameters = null)
	{
		this.id = newId;
		this.parameters = MotorwaysStringKey.ConvertToStringDictionary(newParameters);
		if ((float)this.count < 1f)
		{
			this.count = Mathf.FloorToInt(newCount);
		}
		else if ((float)this.count > 1f)
		{
			this.count = Mathf.CeilToInt(newCount);
		}
		else
		{
			this.count = 1;
		}
		this.isPlural = true;
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x00031732 File Offset: 0x0002F932
	public override void InitWithStringId(StringId stringId)
	{
		this.BasicInit(stringId, null);
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x0003173C File Offset: 0x0002F93C
	public override void InitWithStringId(StringId stringId, int newCount, Dictionary<string, string> newParameters = null)
	{
		this.IntInit(stringId, newCount, MotorwaysStringKey.ConvertToEnumDictionary(newParameters));
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0003174C File Offset: 0x0002F94C
	public override void InitWithStringId(StringId stringId, float newCount, Dictionary<string, string> newParameters = null)
	{
		this.FloatInit(stringId, newCount, MotorwaysStringKey.ConvertToEnumDictionary(newParameters));
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0003175C File Offset: 0x0002F95C
	public override void InitWithString(string stringKey)
	{
		StringId enumKey = StringId.None;
		if (Enum.TryParse<StringId>(stringKey, out enumKey) && enumKey != StringId.None)
		{
			this.InitWithStringId(enumKey);
		}
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x00031780 File Offset: 0x0002F980
	public override void InitWithString(string stringKey, int newCount, Dictionary<string, string> newParameters = null)
	{
		StringId enumKey = StringId.None;
		if (Enum.TryParse<StringId>(stringKey, out enumKey) && enumKey != StringId.None)
		{
			this.InitWithStringId(enumKey, newCount, newParameters);
		}
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x000317A8 File Offset: 0x0002F9A8
	public override void InitWithString(string stringKey, float newCount, Dictionary<string, string> newParameters = null)
	{
		StringId enumKey = StringId.None;
		if (Enum.TryParse<StringId>(stringKey, out enumKey) && enumKey != StringId.None)
		{
			this.InitWithStringId(enumKey, newCount, newParameters);
		}
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000317CD File Offset: 0x0002F9CD
	public override void InitWithNonLocalizedString(string nonLocalizedString)
	{
		this.InitWithStringId(StringId.PassThroughString, 0, new Dictionary<string, string>
		{
			{
				"PassThroughString",
				nonLocalizedString
			}
		});
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x000317EC File Offset: 0x0002F9EC
	public override void Reset()
	{
		this.id = StringId.None;
		this.parameters = null;
		this.count = 0;
		this.isPlural = false;
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0003180A File Offset: 0x0002FA0A
	public static implicit operator MotorwaysStringKey(StringId id)
	{
		return new MotorwaysStringKey(id, null);
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x00031814 File Offset: 0x0002FA14
	private static Dictionary<string, string> ConvertToStringDictionary(Dictionary<StringParameterId, string> originalParameters)
	{
		if (originalParameters == null)
		{
			return null;
		}
		Dictionary<string, string> newParameters = new Dictionary<string, string>();
		foreach (KeyValuePair<StringParameterId, string> entry in originalParameters)
		{
			newParameters.Add(entry.Key.ToString(), entry.Value);
		}
		return newParameters;
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0003188C File Offset: 0x0002FA8C
	private static Dictionary<StringParameterId, string> ConvertToEnumDictionary(Dictionary<string, string> originalParameters)
	{
		if (originalParameters == null)
		{
			return null;
		}
		Dictionary<StringParameterId, string> newParameters = new Dictionary<StringParameterId, string>();
		foreach (KeyValuePair<string, string> entry in originalParameters)
		{
			StringParameterId parameterId = StringParameterId.None;
			if (Diagnostics.Verify(Enum.TryParse<StringParameterId>(entry.Key, out parameterId) && parameterId > StringParameterId.None, "Could not convert {0} into a string parameter id", entry.Key))
			{
				newParameters.Add(parameterId, entry.Value);
			}
		}
		return newParameters;
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0003191C File Offset: 0x0002FB1C
	public override bool Equals(StringKey other)
	{
		if (other is MotorwaysStringKey && other != null)
		{
			MotorwaysStringKey otherStringKey = other as MotorwaysStringKey;
			return this.id.Equals(otherStringKey.id) && ((this.parameters == null && otherStringKey.parameters == null) || this.parameters.Equals(otherStringKey.parameters)) && this.count == otherStringKey.count && this.isPlural == otherStringKey.isPlural;
		}
		return false;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x000319A3 File Offset: 0x0002FBA3
	public override int GetCount()
	{
		return this.count;
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x000319AC File Offset: 0x0002FBAC
	public override int GetHashCode()
	{
		int hashCode = this.id.GetHashCode() ^ this.count.GetHashCode() ^ this.isPlural.GetHashCode();
		if (this.parameters != null)
		{
			hashCode ^= this.parameters.GetHashCode();
		}
		return hashCode;
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x000319FA File Offset: 0x0002FBFA
	public override Dictionary<string, string> GetParameters()
	{
		return this.parameters;
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x00031A02 File Offset: 0x0002FC02
	public override string GetStringId()
	{
		return this.id.ToString();
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x00031A15 File Offset: 0x0002FC15
	public override bool IsPlural()
	{
		return this.isPlural;
	}

	// Token: 0x040008A4 RID: 2212
	protected StringId id;

	// Token: 0x040008A5 RID: 2213
	protected Dictionary<string, string> parameters;

	// Token: 0x040008A6 RID: 2214
	protected int count;

	// Token: 0x040008A7 RID: 2215
	protected bool isPlural;
}
