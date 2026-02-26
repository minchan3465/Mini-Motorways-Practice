using System;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class PassageSpout : MonoBehaviour
{
	// Token: 0x06000B6C RID: 2924 RVA: 0x000270BA File Offset: 0x000252BA
	public void ShowBridge()
	{
		this.bridgeRoot.SetActive(true);
		this.tunnelRoot.SetActive(false);
		this.dryingTunnelRenderer.enabled = false;
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x000270E0 File Offset: 0x000252E0
	public void ShowTunnel()
	{
		this.bridgeRoot.SetActive(false);
		this.tunnelRoot.SetActive(true);
		this.dryingTunnelRenderer.enabled = false;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00027106 File Offset: 0x00025306
	public void ShowDryingTunnel(MaterialPropertyBlock propertyBlock)
	{
		this.dryingTunnelRenderer.enabled = true;
		this.dryingTunnelRenderer.SetPropertyBlock(propertyBlock);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00027120 File Offset: 0x00025320
	public void HideDryingTunnel()
	{
		this.dryingTunnelRenderer.enabled = false;
	}

	// Token: 0x0400068E RID: 1678
	public GameObject bridgeRoot;

	// Token: 0x0400068F RID: 1679
	public GameObject tunnelRoot;

	// Token: 0x04000690 RID: 1680
	public MeshFilter dryingTunnelMesh;

	// Token: 0x04000691 RID: 1681
	public MeshRenderer dryingTunnelRenderer;
}
