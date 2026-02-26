using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class BrowserOAuthClient : IOAuthClient
{
	// Token: 0x06000375 RID: 885 RVA: 0x0000E738 File Offset: 0x0000C938
	public void RequestAuthorization(string authorizationUrl, string callbackUrl, IOAuthClient.AuthorizationRequestDelegate callback)
	{
		Application.OpenURL(authorizationUrl);
		callback(OAuthAuthorizationResult.Unknown);
	}
}
