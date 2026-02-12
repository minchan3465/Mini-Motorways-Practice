using System;

// Token: 0x02000218 RID: 536
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class JsonSerializableAttribute : Attribute
{
	// Token: 0x06000CD3 RID: 3283 RVA: 0x00029ADC File Offset: 0x00027CDC
	public JsonSerializableAttribute(string name, JsonSerializableAttribute.MergeStrategy strategy)
	{
		this.serializedName = name;
		this.mergeStrategy = strategy;
	}

	// Token: 0x04000726 RID: 1830
	public string serializedName;

	// Token: 0x04000727 RID: 1831
	public JsonSerializableAttribute.MergeStrategy mergeStrategy;

	// Token: 0x02000219 RID: 537
	public enum MergeStrategy
	{
		// Token: 0x04000729 RID: 1833
		Max,
		// Token: 0x0400072A RID: 1834
		Min,
		// Token: 0x0400072B RID: 1835
		Latest
	}
}
