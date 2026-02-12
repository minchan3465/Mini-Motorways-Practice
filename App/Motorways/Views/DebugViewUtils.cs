using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200059E RID: 1438
	public static class DebugViewUtils
	{
		// Token: 0x0600282C RID: 10284 RVA: 0x000AB5B4 File Offset: 0x000A97B4
		public static Texture2D Create2DTexture(int width, int height, Color color)
		{
			Color[] pix = new Color[width * height];
			for (int pixelIndex = 0; pixelIndex < pix.Length; pixelIndex++)
			{
				pix[pixelIndex] = color;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(pix);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x040021F4 RID: 8692
		public static readonly Texture2D DebugWindowBackground = DebugViewUtils.Create2DTexture(2, 2, Color.Lerp(Color.gray, Color.clear, 0.2f));
	}
}
