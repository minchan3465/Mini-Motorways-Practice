using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class DebugTileDataViewer : MonoBehaviour
{
	// Token: 0x06000921 RID: 2337 RVA: 0x0001DB12 File Offset: 0x0001BD12
	public void Clear()
	{
		this.squareTileData.Clear();
		this.checkerSquareTileData.Clear();
		this.stringData.Clear();
	}

	// Token: 0x04000496 RID: 1174
	public Dictionary<Vector2Int, Color> squareTileData = new Dictionary<Vector2Int, Color>();

	// Token: 0x04000497 RID: 1175
	public Dictionary<Vector2Int, Color> checkerSquareTileData = new Dictionary<Vector2Int, Color>();

	// Token: 0x04000498 RID: 1176
	public Dictionary<Vector2Int, string> stringData = new Dictionary<Vector2Int, string>();

	// Token: 0x04000499 RID: 1177
	public bool squareTilesOn = true;

	// Token: 0x0400049A RID: 1178
	public bool checkerTilesOn = true;

	// Token: 0x0400049B RID: 1179
	public bool tileTextOn = true;

	// Token: 0x0400049C RID: 1180
	public bool tileCoordinatesOn = true;

	// Token: 0x0400049D RID: 1181
	public int textSize = 10;

	// Token: 0x0400049E RID: 1182
	[ResizableTextArea]
	public string context;

	// Token: 0x0400049F RID: 1183
	public bool onlyDrawWhenSelected = true;
}
