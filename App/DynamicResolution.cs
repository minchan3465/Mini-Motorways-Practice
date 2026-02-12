using System;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class DynamicResolution : MonoBehaviour
{
	// Token: 0x06000E41 RID: 3649 RVA: 0x00030769 File Offset: 0x0002E969
	public void OnEnable()
	{
		if (Application.isEditor)
		{
			base.GetComponent<Camera>().allowDynamicResolution = true;
		}
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x00030780 File Offset: 0x0002E980
	public void Update()
	{
		if (Application.isEditor)
		{
			float widthScale = Mathf.Clamp01((float)this._width / (float)Screen.width);
			float heightScale = Mathf.Clamp01((float)this._height / (float)Screen.height);
			if (this._lastWidthScale != widthScale || this._lastHeightScale != heightScale)
			{
				if (widthScale > 0f && heightScale > 0f)
				{
					ScalableBufferManager.ResizeBuffers(widthScale, heightScale);
					DynamicResolution.Log.Info("Scaling the resolution by {0}x{1} to emulate {2}x{3}.", new object[]
					{
						widthScale,
						heightScale,
						this._width,
						this._height
					});
				}
				this._lastWidthScale = widthScale;
				this._lastHeightScale = heightScale;
			}
		}
	}

	// Token: 0x0400086E RID: 2158
	[SerializeField]
	private int _width = 1024;

	// Token: 0x0400086F RID: 2159
	[SerializeField]
	private int _height = 768;

	// Token: 0x04000870 RID: 2160
	private float _lastWidthScale;

	// Token: 0x04000871 RID: 2161
	private float _lastHeightScale;

	// Token: 0x04000872 RID: 2162
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DynamicResolution");
}
