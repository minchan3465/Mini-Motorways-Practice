using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200000D RID: 13
public static class UnityAnalyticsBridge
{
	// Token: 0x06000056 RID: 86 RVA: 0x000034B1 File Offset: 0x000016B1
	private static string GetPlayerGroup()
	{
		if (Application.isEditor || FeatureToggle.IsFeatureEnabled(Feature.InGameDevTools) || FeatureToggle.IsFeatureEnabled(Feature.OptionsDebugMenu))
		{
			return "dev";
		}
		if (FeatureToggle.IsFeatureEnabled(Feature.BetaWatermark))
		{
			return "beta";
		}
		return "production";
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000022F5 File Offset: 0x000004F5
	public static void CustomEvent(string eventName, Dictionary<string, object> parameters)
	{
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000034E8 File Offset: 0x000016E8
	public static Task Initialize()
	{
		UnityAnalyticsBridge.<Initialize>d__5 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<UnityAnalyticsBridge.<Initialize>d__5>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	// Token: 0x0400002F RID: 47
	private const string BetaEnvironmentName = "beta";

	// Token: 0x04000030 RID: 48
	private const string ProductionEnvironmentName = "production";

	// Token: 0x04000031 RID: 49
	private const string DevEnvironmentName = "dev";
}
