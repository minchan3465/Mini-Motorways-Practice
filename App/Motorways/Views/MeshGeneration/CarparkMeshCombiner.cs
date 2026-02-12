using System;
using System.Collections.Generic;
using Motorways.Models;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000623 RID: 1571
	public class CarparkMeshCombiner
	{
		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06002BF8 RID: 11256 RVA: 0x000C29BE File Offset: 0x000C0BBE
		public Mesh HorizontalSingleCarparkLeftEntrance { get; }

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06002BF9 RID: 11257 RVA: 0x000C29C6 File Offset: 0x000C0BC6
		public Mesh HorizontalSingleCarparkRightEntrance { get; }

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06002BFA RID: 11258 RVA: 0x000C29CE File Offset: 0x000C0BCE
		public Mesh HorizontalDoubleCarpark { get; }

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06002BFB RID: 11259 RVA: 0x000C29D6 File Offset: 0x000C0BD6
		public Mesh ReversedHorizontalDoubleCarpark { get; }

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06002BFC RID: 11260 RVA: 0x000C29DE File Offset: 0x000C0BDE
		public Mesh BoatTerminalCarpark { get; }

		// Token: 0x06002BFD RID: 11261 RVA: 0x000C29E8 File Offset: 0x000C0BE8
		public CarparkMeshCombiner(GameObject carparkPrefab)
		{
			this._meshInfo = carparkPrefab.GetComponentInChildren<CarparkMeshCombineInfo>();
			this.HorizontalSingleCarparkLeftEntrance = this.GenerateCarparkMesh(false, CarparkEntrance.TopLeft, false, false);
			this.HorizontalSingleCarparkRightEntrance = this.GenerateCarparkMesh(false, CarparkEntrance.BottomRight, false, false);
			this.HorizontalDoubleCarpark = this.GenerateCarparkMesh(true, CarparkEntrance.TopLeftAndBottomRight, false, false);
			this.ReversedHorizontalDoubleCarpark = this.GenerateCarparkMesh(true, CarparkEntrance.TopLeftAndBottomRight, true, false);
			this.BoatTerminalCarpark = this.GenerateCarparkMesh(true, CarparkEntrance.TopLeftAndBottomRight, false, true);
			this.combinedCarparkPrefab = UnityEngine.Object.Instantiate<GameObject>(carparkPrefab);
			this.combinedCarparkPrefab.gameObject.SetActive(false);
			this.combinedCarparkPrefab.name = carparkPrefab.name;
			this.combinedCarparkPrefab.hideFlags = HideFlags.HideAndDontSave;
			this.combinedCarparkPrefab.GetComponentInChildren<CarparkMeshCombineInfo>().Remove();
		}

		// Token: 0x06002BFE RID: 11262 RVA: 0x000C2AA4 File Offset: 0x000C0CA4
		private Mesh GenerateCarparkMesh(bool supportsTwoDestinations, CarparkEntrance carparkEntrance, bool reversed, bool supportsBoats = false)
		{
			Mesh mesh = new Mesh
			{
				name = "Combined Carpark Mesh"
			};
			List<CarparkMesh> meshes = new List<CarparkMesh>();
			CarparkMesh topLeftOpen;
			CarparkMesh topLeftClosed;
			CarparkMesh bottomRightOpen;
			CarparkMesh bottomRightClosed;
			CarparkMesh[] carparkTiles;
			if (supportsTwoDestinations)
			{
				if (reversed)
				{
					topLeftOpen = this._meshInfo.reversedDoubleCarparkTopLeftOpen;
					topLeftClosed = this._meshInfo.reversedDoubleCarparkTopLeftClosed;
					bottomRightOpen = this._meshInfo.reversedDoubleCarparkBottomRightOpen;
					bottomRightClosed = this._meshInfo.reversedDoubleCarparkBottomRightClosed;
					carparkTiles = this._meshInfo.reversedDoubleCarparkTiles;
				}
				else
				{
					topLeftOpen = this._meshInfo.doubleCarparkTopLeftOpen;
					topLeftClosed = this._meshInfo.doubleCarparkTopLeftClosed;
					bottomRightOpen = this._meshInfo.doubleCarparkBottomRightOpen;
					bottomRightClosed = this._meshInfo.doubleCarparkBottomRightClosed;
					carparkTiles = this._meshInfo.doubleCarparkTiles;
				}
			}
			else
			{
				topLeftOpen = this._meshInfo.singleCarparkTopLeftOpen;
				topLeftClosed = this._meshInfo.singleCarparkTopLeftClosed;
				bottomRightOpen = this._meshInfo.singleCarparkBottomRightOpen;
				bottomRightClosed = this._meshInfo.singleCarparkBottomRightClosed;
				carparkTiles = this._meshInfo.singleCarparkMeshes;
			}
			if (supportsBoats)
			{
				for (int i = 0; i < carparkTiles.Length; i++)
				{
					if (carparkTiles[i] == this._meshInfo.doubleCarparkTopLeftCorner)
					{
						carparkTiles[i] = this._meshInfo.boatTopLeftCorner;
					}
					else if (carparkTiles[i] == this._meshInfo.doubleCarparkTopRightCorner)
					{
						carparkTiles[i] = this._meshInfo.boatTopRightCorner;
					}
				}
				meshes.Add(this._meshInfo.boatTopLeftEntrance);
				meshes.Add(this._meshInfo.boatTopRightEntrance);
			}
			else
			{
				for (int j = 0; j < carparkTiles.Length; j++)
				{
					if (carparkTiles[j] == this._meshInfo.boatTopLeftCorner)
					{
						carparkTiles[j] = this._meshInfo.doubleCarparkTopLeftCorner;
					}
					else if (carparkTiles[j] == this._meshInfo.boatTopRightCorner)
					{
						carparkTiles[j] = this._meshInfo.doubleCarparkTopRightCorner;
					}
				}
			}
			meshes.Add(carparkEntrance.HasFlag(CarparkEntrance.TopLeft) ? topLeftOpen : topLeftClosed);
			meshes.Add(carparkEntrance.HasFlag(CarparkEntrance.BottomRight) ? bottomRightOpen : bottomRightClosed);
			meshes.AddRange(carparkTiles);
			List<CombineInstance> combineInstances = new List<CombineInstance>();
			foreach (CarparkMesh carparkMesh in meshes)
			{
				foreach (CarparkMesh.ThemedMesh meshAndTheme in carparkMesh.meshes)
				{
					this.CombineMesh(meshAndTheme.meshRenderer, meshAndTheme.themedMaterialType, combineInstances);
				}
			}
			mesh.CombineMeshes(combineInstances.ToArray());
			return mesh;
		}

		// Token: 0x06002BFF RID: 11263 RVA: 0x000C2D6C File Offset: 0x000C0F6C
		private void CombineMesh(MeshRenderer meshRenderer, ThemedMaterialType themedMaterialType, List<CombineInstance> combineInstances)
		{
			MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
			Mesh meshWithVertexColors = UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh);
			CombinedMeshThemeComponent.SetAbsoluteVertexColorIndexForMesh(meshWithVertexColors, themedMaterialType);
			CombineInstance combineInstanceOutline = new CombineInstance
			{
				mesh = meshWithVertexColors,
				transform = meshFilter.transform.localToWorldMatrix
			};
			combineInstances.Add(combineInstanceOutline);
		}

		// Token: 0x04002622 RID: 9762
		private readonly CarparkMeshCombineInfo _meshInfo;

		// Token: 0x04002628 RID: 9768
		public readonly GameObject combinedCarparkPrefab;
	}
}
