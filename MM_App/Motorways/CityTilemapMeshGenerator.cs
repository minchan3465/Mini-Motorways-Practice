using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000398 RID: 920
	[ExecuteAlways]
	[RequireComponent(typeof(CityDefinition))]
	public class CityTilemapMeshGenerator : MonoBehaviour
	{
		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x060015F3 RID: 5619 RVA: 0x0004B64E File Offset: 0x0004984E
		// (set) Token: 0x060015F4 RID: 5620 RVA: 0x0004B656 File Offset: 0x00049856
		public CityTilemap CityTilemap { get; private set; }

		// Token: 0x060015F5 RID: 5621 RVA: 0x0004B660 File Offset: 0x00049860
		public void SetMeshPreviewMaterials(Material material)
		{
			foreach (CityTilemapVisualGroupPreview cityTilemapVisualGroupPreview in this._visualGroupPreviews.Values)
			{
				cityTilemapVisualGroupPreview.SetAllPreviewMaterials(material);
			}
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x0004B6B8 File Offset: 0x000498B8
		private void Awake()
		{
			this.CityTilemap = base.GetComponent<CityTilemap>();
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x0004B6C8 File Offset: 0x000498C8
		private void OnEnable()
		{
			CityTilemapVisualGroupPreview[] groupPreviews = base.GetComponentsInChildren<CityTilemapVisualGroupPreview>();
			MapVisualGroupType[] activeVisualGroups = this._activeVisualGroups;
			for (int j = 0; j < activeVisualGroups.Length; j++)
			{
				MapVisualGroupType visualGroupType = activeVisualGroups[j];
				this._visualGroupPreviews[visualGroupType] = groupPreviews.FirstOrDefault((CityTilemapVisualGroupPreview i) => i.TargetVisualGroup.groupType == visualGroupType);
				if (this._visualGroupPreviews[visualGroupType] == null)
				{
					GameObject visualGroupPreviewObject = new GameObject
					{
						hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset),
						name = string.Format("{0} Mesh Group Preview", visualGroupType),
						transform = 
						{
							parent = base.transform
						}
					};
					this._visualGroupPreviews[visualGroupType] = visualGroupPreviewObject.AddComponent<CityTilemapVisualGroupPreview>();
				}
			}
			if (this.CityTilemap == null)
			{
				this.CityTilemap = base.GetComponent<CityTilemap>();
			}
			this._landMeshVisualGroup = new MapVisualGroup
			{
				groupType = MapVisualGroupType.Land,
				sourceTilemap = this.CityTilemap.bridgeableTilemap,
				containedLayers = new MapMeshLayer[]
				{
					MapMeshLayer.Land
				}
			};
			this._visualGroupPreviews[this._landMeshVisualGroup.groupType].SetVisualGroup(this._landMeshVisualGroup);
			this._mountainVisualGroup = new MapVisualGroup
			{
				groupType = MapVisualGroupType.Mountains,
				sourceTilemap = this.CityTilemap.mountainTilemap,
				containedLayers = new MapMeshLayer[]
				{
					MapMeshLayer.MountainA,
					MapMeshLayer.MountainB,
					MapMeshLayer.MountainC
				}
			};
			this._visualGroupPreviews[this._mountainVisualGroup.groupType].SetVisualGroup(this._mountainVisualGroup);
			this._visualGroups = new MapVisualGroup[]
			{
				this._landMeshVisualGroup,
				this._mountainVisualGroup
			};
			this.RegenerateAllMapLayerMeshes();
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x0004B894 File Offset: 0x00049A94
		private void OnDisable()
		{
			if (!Application.isPlaying)
			{
				CityTilemapVisualGroupPreview[] componentsInChildren = base.GetComponentsInChildren<CityTilemapVisualGroupPreview>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
				}
				this._visualGroupPreviews.Clear();
			}
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x0004B8D5 File Offset: 0x00049AD5
		public void UpdatePreviewMeshVisibility(CityTilemapVisualGroupPreview groupPreview)
		{
			groupPreview.gameObject.SetActive(CityTilemapMeshGeneratorOptions.ShowPreviewMesh);
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x0004B8E8 File Offset: 0x00049AE8
		public void UpdateAllPreviewMeshVisibility()
		{
			foreach (CityTilemapVisualGroupPreview cityTilemapVisualGroupPreview in this._visualGroupPreviews.Values)
			{
				cityTilemapVisualGroupPreview.gameObject.SetActive(CityTilemapMeshGeneratorOptions.ShowPreviewMesh);
			}
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x0004B948 File Offset: 0x00049B48
		public void RegenerateAllMapLayerMeshes()
		{
			Diagnostics.FailAssert("CityTilemapMeshGenerator shouldn't be used at runtime! Remove it from any maps before building.", Array.Empty<object>());
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RegenerateMapLayerMesh(MapVisualGroup visualGroup)
		{
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x0004B959 File Offset: 0x00049B59
		public void UpdateMeshShownInPreview(CityTilemapVisualGroupPreview groupPreview)
		{
			groupPreview.Rebuild();
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x0004B964 File Offset: 0x00049B64
		private void OnDrawGizmos()
		{
			int colorIndex = 0;
			foreach (CityTilemapVisualGroupPreview groupPreview in this._visualGroupPreviews.Values)
			{
				if (CityTilemapMeshGeneratorOptions.ShowWireMeshGizmo)
				{
					Vector3 zeroCellCenter = groupPreview.TargetVisualGroup.sourceTilemap.GetCellCenterWorld(Vector3Int.zero);
					foreach (MapMeshLayer meshLayer in groupPreview.TargetVisualGroup.containedLayers)
					{
						Mesh mesh;
						if (groupPreview.TargetVisualGroup.generatedMeshes.TryGetValue(meshLayer, out mesh))
						{
							Gizmos.color = Color.HSVToRGB(this._baseWireframeHSV.x + (float)colorIndex++ * 0.2f, this._baseWireframeHSV.y, this._baseWireframeHSV.z);
							Gizmos.DrawWireMesh(mesh, zeroCellCenter, Quaternion.identity, new Vector3(2f, 2f, 2f));
						}
					}
				}
			}
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ExportAllMeshesAsFBX()
		{
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x000022F5 File Offset: 0x000004F5
		public static void ExportMeshAsFBX(string defaultFilename, Mesh mesh)
		{
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x000022F5 File Offset: 0x000004F5
		private static void ExportBinaryFBX(string filePath, UnityEngine.Object singleObject)
		{
		}

		// Token: 0x040012B9 RID: 4793
		private readonly Vector3 _baseWireframeHSV = new Vector3(0.1f, 0.9f, 0.9f);

		// Token: 0x040012BA RID: 4794
		private const float WireframeLayerHueShift = 0.2f;

		// Token: 0x040012BC RID: 4796
		private readonly MapVisualGroupType[] _activeVisualGroups = new MapVisualGroupType[]
		{
			MapVisualGroupType.Land,
			MapVisualGroupType.Mountains
		};

		// Token: 0x040012BD RID: 4797
		private readonly Dictionary<MapVisualGroupType, CityTilemapVisualGroupPreview> _visualGroupPreviews = new Dictionary<MapVisualGroupType, CityTilemapVisualGroupPreview>();

		// Token: 0x040012BE RID: 4798
		private MapVisualGroup _landMeshVisualGroup;

		// Token: 0x040012BF RID: 4799
		private MapVisualGroup _mountainVisualGroup;

		// Token: 0x040012C0 RID: 4800
		private MapVisualGroup[] _visualGroups = Array.Empty<MapVisualGroup>();

		// Token: 0x040012C1 RID: 4801
		private readonly IReadOnlyDictionary<MapMeshLayer, CityTilemapMeshGenerator.MeshLayerSettings> _meshLayerSettings = new Dictionary<MapMeshLayer, CityTilemapMeshGenerator.MeshLayerSettings>
		{
			{
				MapMeshLayer.Land,
				new CityTilemapMeshGenerator.MeshLayerSettings()
			},
			{
				MapMeshLayer.MountainA,
				new CityTilemapMeshGenerator.MeshLayerSettings()
			},
			{
				MapMeshLayer.MountainB,
				new CityTilemapMeshGenerator.MeshLayerSettings
				{
					erosion = 1
				}
			},
			{
				MapMeshLayer.MountainC,
				new CityTilemapMeshGenerator.MeshLayerSettings
				{
					erosion = 3,
					subdivisions = 2
				}
			}
		};

		// Token: 0x02000399 RID: 921
		private class MeshLayerSettings
		{
			// Token: 0x040012C2 RID: 4802
			public int erosion;

			// Token: 0x040012C3 RID: 4803
			public int subdivisions = 1;
		}

		// Token: 0x0200039A RID: 922
		private struct MeshSaveOperation
		{
			// Token: 0x040012C4 RID: 4804
			public string filename;

			// Token: 0x040012C5 RID: 4805
			public string objectName;

			// Token: 0x040012C6 RID: 4806
			public Mesh mesh;
		}
	}
}
