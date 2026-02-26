using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200039D RID: 925
	[ExecuteAlways]
	public class CityTilemapVisualGroupPreview : MonoBehaviour
	{
		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x0600160C RID: 5644 RVA: 0x0004BBE9 File Offset: 0x00049DE9
		// (set) Token: 0x0600160D RID: 5645 RVA: 0x0004BBF1 File Offset: 0x00049DF1
		public MapVisualGroup TargetVisualGroup { get; private set; }

		// Token: 0x0600160E RID: 5646 RVA: 0x0004BBFA File Offset: 0x00049DFA
		public void SetVisualGroup(MapVisualGroup visualGroup)
		{
			this.TargetVisualGroup = visualGroup;
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x0004BC04 File Offset: 0x00049E04
		public void Teardown()
		{
			foreach (CityTilemapMeshPreview cityTilemapMeshPreview in this._meshPreviews)
			{
				UnityEngine.Object.DestroyImmediate(cityTilemapMeshPreview.gameObject);
			}
			this._meshPreviews.Clear();
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x0004BC64 File Offset: 0x00049E64
		public void Rebuild()
		{
			this.Teardown();
			int meshIndex = 0;
			int startingSortingOrder = (this.TargetVisualGroup.groupType == MapVisualGroupType.Mountains) ? 3 : 0;
			foreach (MapMeshLayer meshLayer in this.TargetVisualGroup.containedLayers)
			{
				Mesh mesh = this.TargetVisualGroup.generatedMeshes[meshLayer];
				CityTilemapMeshPreview meshPreview = new GameObject
				{
					hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset),
					name = "Mesh Preview",
					transform = 
					{
						parent = base.transform
					}
				}.AddComponent<CityTilemapMeshPreview>();
				meshPreview.SetPreviewMesh(mesh);
				List<Material> materials = CityTilemapVisualGroupPreview.FindAssetsOfType<Material>(this._themeKeys[this.TargetVisualGroup.groupType][meshIndex]);
				if (materials.Count > 0)
				{
					meshPreview.SetPreviewMaterial(materials[0]);
				}
				meshPreview.SetSortingLayer(this._sortingLayers[this.TargetVisualGroup.groupType]);
				meshPreview.SetSortingOrder(startingSortingOrder + meshIndex);
				this._meshPreviews.Add(meshPreview);
				meshIndex++;
			}
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x0004BD70 File Offset: 0x00049F70
		public void SetAllPreviewMaterials(Material material)
		{
			foreach (CityTilemapMeshPreview cityTilemapMeshPreview in this._meshPreviews)
			{
				cityTilemapMeshPreview.SetPreviewMaterial(material);
			}
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x0004BDC4 File Offset: 0x00049FC4
		private static List<T> FindAssetsOfType<T>(string filterString) where T : class
		{
			return new List<T>();
		}

		// Token: 0x040012CC RID: 4812
		private readonly List<CityTilemapMeshPreview> _meshPreviews = new List<CityTilemapMeshPreview>();

		// Token: 0x040012CD RID: 4813
		private readonly IReadOnlyDictionary<MapVisualGroupType, string> _sortingLayers = new Dictionary<MapVisualGroupType, string>
		{
			{
				MapVisualGroupType.Land,
				"Landscape"
			},
			{
				MapVisualGroupType.Mountains,
				"Mountain"
			}
		};

		// Token: 0x040012CE RID: 4814
		private readonly IReadOnlyDictionary<MapVisualGroupType, string[]> _themeKeys = new Dictionary<MapVisualGroupType, string[]>
		{
			{
				MapVisualGroupType.Land,
				new string[]
				{
					"Binding_Land"
				}
			},
			{
				MapVisualGroupType.Mountains,
				new string[]
				{
					"Binding_MountainA",
					"Binding_MountainB",
					"Binding_MountainC",
					"Binding_Shadow"
				}
			}
		};
	}
}
