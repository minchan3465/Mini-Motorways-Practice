using System;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class SplashScreenCheck : MonoBehaviour
{
	// Token: 0x06000E00 RID: 3584 RVA: 0x0002F6E6 File Offset: 0x0002D8E6
	private void Awake()
	{
		if (Camera.main == null)
		{
			this.disabledCamera.SetActive(true);
			UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x04000830 RID: 2096
	public GameObject disabledCamera;
}
