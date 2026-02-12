using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000626 RID: 1574
	public class HouseMeshCombiner
	{
		// Token: 0x06002C0A RID: 11274 RVA: 0x000C328C File Offset: 0x000C148C
		public Mesh MeshForGroupIndex(int groupIndex)
		{
			if (Diagnostics.Verify(groupIndex >= 0 && groupIndex < this._meshForGroup.Length, "groupIndex >= 0 && groupIndex < _meshForGroup.Length"))
			{
				return this._meshForGroup[groupIndex];
			}
			return null;
		}

		// Token: 0x06002C0B RID: 11275 RVA: 0x000C32B8 File Offset: 0x000C14B8
		public HouseMeshCombiner(GameObject housePrefab)
		{
			this.combinedMeshHousePrefab = UnityEngine.Object.Instantiate<GameObject>(housePrefab);
			this.combinedMeshHousePrefab.SetActive(false);
			this.combinedMeshHousePrefab.hideFlags = HideFlags.HideAndDontSave;
			HouseMesh[] houseMeshes = this.combinedMeshHousePrefab.GetComponentsInChildren<HouseMesh>(true);
			Mesh combinedHouseMesh = this.CombineHouseMesh(houseMeshes);
			List<Color> existingColors = new List<Color>();
			combinedHouseMesh.GetColors(existingColors);
			for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
			{
				Mesh meshForGroup = UnityEngine.Object.Instantiate<Mesh>(combinedHouseMesh);
				Color[] colors = new Color[combinedHouseMesh.vertexCount];
				int themeComponentGroupOffset = CombinedMeshThemeComponent.RelativeThemeComponentGroupTargetOffsetForGroup(groupIndex);
				for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
				{
					float themeComponentGroupTargetIndex = existingColors[colorIndex].r;
					colors[colorIndex] = new Color((float)themeComponentGroupOffset + themeComponentGroupTargetIndex, existingColors[colorIndex].g, existingColors[colorIndex].b, existingColors[colorIndex].a);
				}
				meshForGroup.SetColors(colors);
				this._meshForGroup[groupIndex] = meshForGroup;
			}
			HouseMesh[] array = houseMeshes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x06002C0C RID: 11276 RVA: 0x000C33EC File Offset: 0x000C15EC
		private Mesh CombineHouseMesh(HouseMesh[] houseMeshes)
		{
			CombineInstance[] combineInstances = new CombineInstance[houseMeshes.Length];
			for (int combineInstanceIndex = 0; combineInstanceIndex < combineInstances.Length; combineInstanceIndex++)
			{
				this.Combine(combineInstanceIndex, houseMeshes[combineInstanceIndex], combineInstances);
			}
			Mesh mesh = new Mesh();
			mesh.name = "Combined House Mesh";
			mesh.CombineMeshes(combineInstances);
			return mesh;
		}

		// Token: 0x06002C0D RID: 11277 RVA: 0x000C3434 File Offset: 0x000C1634
		private void Combine(int index, HouseMesh houseMesh, in CombineInstance[] combineInstances)
		{
			MeshFilter component = houseMesh.GetComponent<MeshFilter>();
			Mesh meshWithVertexColors = UnityEngine.Object.Instantiate<Mesh>(component.sharedMesh);
			CombinedMeshThemeComponent.SetRelativeVertexColorIndexForMesh(meshWithVertexColors, houseMesh.groupTarget);
			combineInstances[index].mesh = meshWithVertexColors;
			Transform transform = component.transform;
			combineInstances[index].transform = transform.localToWorldMatrix;
		}

		// Token: 0x0400262E RID: 9774
		private const string CombinedHouseMeshName = "Combined House Mesh";

		// Token: 0x0400262F RID: 9775
		public readonly GameObject combinedMeshHousePrefab;

		// Token: 0x04002630 RID: 9776
		private readonly Mesh[] _meshForGroup = new Mesh[MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS];
	}
}
