using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
public static class CityTilemapMeshGeneratorOptions
{
	// Token: 0x0600091D RID: 2333 RVA: 0x0001DAC7 File Offset: 0x0001BCC7
	private static bool GetOption(string key)
	{
		return key == "CityTilemapMeshGenerator_ShowPreviewMeshInGame";
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x0600091E RID: 2334 RVA: 0x0001DAD9 File Offset: 0x0001BCD9
	public static bool UpdatePreviewMeshWhileEditing
	{
		get
		{
			return CityTilemapMeshGeneratorOptions.GetOption("CityTilemapMeshGenerator_UpdatePreviewMeshWhileEditingTilemap");
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x0600091F RID: 2335 RVA: 0x0001DAE5 File Offset: 0x0001BCE5
	public static bool ShowWireMeshGizmo
	{
		get
		{
			return CityTilemapMeshGeneratorOptions.GetOption("CityTilemapMeshGenerator_ShowWireMeshGizmo");
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x06000920 RID: 2336 RVA: 0x0001DAF1 File Offset: 0x0001BCF1
	public static bool ShowPreviewMesh
	{
		get
		{
			return (Application.isPlaying && CityTilemapMeshGeneratorOptions.GetOption("CityTilemapMeshGenerator_ShowPreviewMeshInGame")) || CityTilemapMeshGeneratorOptions.GetOption("CityTilemapMeshGenerator_ShowPreviewMeshInPrefab");
		}
	}

	// Token: 0x04000492 RID: 1170
	public const string ShowPreviewMeshInPrefabEditorPrefKey = "CityTilemapMeshGenerator_ShowPreviewMeshInPrefab";

	// Token: 0x04000493 RID: 1171
	public const string ShowPreviewMeshInGameEditorPrefKey = "CityTilemapMeshGenerator_ShowPreviewMeshInGame";

	// Token: 0x04000494 RID: 1172
	public const string UpdatePreviewMeshWhileEditingTilemap = "CityTilemapMeshGenerator_UpdatePreviewMeshWhileEditingTilemap";

	// Token: 0x04000495 RID: 1173
	public const string ShowWireMeshGizmoEditorPrefKey = "CityTilemapMeshGenerator_ShowWireMeshGizmo";
}
