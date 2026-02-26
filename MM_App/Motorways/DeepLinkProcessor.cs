using System;
using System.Collections.Generic;
using System.Net;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003C9 RID: 969
	public class DeepLinkProcessor : ICreatedInScopeHandler, IReleasedFromScopeHandler
	{
		// Token: 0x06001701 RID: 5889 RVA: 0x00053B38 File Offset: 0x00051D38
		public void OnCreatedInScope(IScope scope)
		{
			Diagnostics.Log.Info("DeepLinkProcessor", "OnCreatedInScope(), Subscribing to callback", Array.Empty<object>());
			Application.deepLinkActivated += this.OnDeepLinkActivated;
			if (!string.IsNullOrEmpty(Application.absoluteURL))
			{
				this.OnDeepLinkActivated(Application.absoluteURL);
			}
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x00053B76 File Offset: 0x00051D76
		public void OnReleasedFromScope(IScope scope)
		{
			Application.deepLinkActivated -= this.OnDeepLinkActivated;
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x00053B8C File Offset: 0x00051D8C
		public void OnDeepLinkActivated(string url)
		{
			this._deepLinkURL = url;
			this.hasChallengeToUse = false;
			this.parameters.Clear();
			Diagnostics.Log.Info("DeepLinkProcessor", "Deeplink url received {0}", new object[]
			{
				url
			});
			if (url.Contains("https://api.dinopoloclub.com/1/minimotorways/start-challenge/") || url.Contains("https//api.dinopoloclub.com/1/minimotorways/start-challenge/"))
			{
				this.ExtractParametersFromUrl();
				this.ProcessParameters();
			}
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00053BF4 File Offset: 0x00051DF4
		private void ExtractParametersFromUrl()
		{
			string[] array = new Uri(this._deepLinkURL).Query.TrimStart('?').Split('&', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] parameter = array[i].Split('=', StringSplitOptions.None);
				if (parameter.Length == 2)
				{
					this.parameters.Add(parameter[0], WebUtility.UrlDecode(parameter[1]));
					Diagnostics.Log.Info("DeepLinkProcessor", "Parameter found: {0} {1}", new object[]
					{
						parameter[0],
						parameter[0]
					});
				}
			}
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00053C78 File Offset: 0x00051E78
		private void ProcessParameters()
		{
			string activityName;
			if (!this.parameters.TryGetValue("a", out activityName))
			{
				Diagnostics.Log.Warn("DeepLinkProcessor", "activity parameter invalid. Expected {0}", new object[]
				{
					"a"
				});
				return;
			}
			if (this.challengeDatabase == null)
			{
				Diagnostics.Log.Error("DeepLinkProcessor", "challengeDatabase is null", Array.Empty<object>());
				return;
			}
			PlayTogetherChallengeDatabase.Challenge challenge;
			if (!this.challengeDatabase.TryGetChallenge(activityName, out challenge))
			{
				Diagnostics.Log.Warn("DeepLinkProcessor", "unrecognized activityName {0}", new object[]
				{
					activityName
				});
				return;
			}
			Diagnostics.Log.Info("DeepLinkProcessor", string.Format("challenge found {0} {1} ({2})", challenge.ChallengeId, challenge.MapName, challenge.GameMode), Array.Empty<object>());
			this.hasChallengeToUse = true;
			this.challengeMap = challenge.MapName;
			this.challengeMode = challenge.GameMode;
		}

		// Token: 0x040013B1 RID: 5041
		private const string FALLBACK_URL = "https://api.dinopoloclub.com/1/minimotorways/start-challenge/";

		// Token: 0x040013B2 RID: 5042
		private const string STRIPPED_FALLBACK_URL = "https//api.dinopoloclub.com/1/minimotorways/start-challenge/";

		// Token: 0x040013B3 RID: 5043
		private const string CHALLENGE_PREFIX = "grp.dinopoloclub.minimotorways.challenges.";

		// Token: 0x040013B4 RID: 5044
		private const string ACTIVITY_PARAMETER = "a";

		// Token: 0x040013B5 RID: 5045
		private string _deepLinkURL;

		// Token: 0x040013B6 RID: 5046
		private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

		// Token: 0x040013B7 RID: 5047
		public bool hasChallengeToUse;

		// Token: 0x040013B8 RID: 5048
		public string challengeMap;

		// Token: 0x040013B9 RID: 5049
		public GameMode challengeMode;

		// Token: 0x040013BA RID: 5050
		[Dependency]
		private PlayTogetherChallengeDatabase challengeDatabase;
	}
}
