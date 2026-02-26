using System;
using JetBrains.Annotations;

// Token: 0x020000EA RID: 234
public interface IOAuthClient
{
	// Token: 0x060004DA RID: 1242
	void RequestAuthorization([NotNull] string authorizationUrl, [CanBeNull] string callbackUrl, IOAuthClient.AuthorizationRequestDelegate callback);

	// Token: 0x020000EB RID: 235
	// (Invoke) Token: 0x060004DC RID: 1244
	public delegate void AuthorizationRequestDelegate(OAuthAuthorizationResult result);
}
