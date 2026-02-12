using System;
using JetBrains.Annotations;

namespace Factory
{
	// Token: 0x020002F2 RID: 754
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class DependencyAttribute : Attribute
	{
	}
}
