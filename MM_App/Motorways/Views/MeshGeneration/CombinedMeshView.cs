using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Constants;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000620 RID: 1568
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class CombinedMeshView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x06002BED RID: 11245 RVA: 0x000C24E8 File Offset: 0x000C06E8
		private int SortingLayerForMeshType(CombinedMeshView.CombinedMeshType type)
		{
			if (type == CombinedMeshView.CombinedMeshType.Carpark)
			{
				return SortingLayer.NameToID("CarparkOutline");
			}
			if (type != CombinedMeshView.CombinedMeshType.House)
			{
				return SortingLayer.NameToID("Default");
			}
			return SortingLayer.NameToID("BuildingLower");
		}

		// Token: 0x06002BEE RID: 11246 RVA: 0x000C2513 File Offset: 0x000C0713
		private int LayerForMeshType(CombinedMeshView.CombinedMeshType type)
		{
			if (type == CombinedMeshView.CombinedMeshType.Carpark)
			{
				return LayerConstants.DefaultLayerId;
			}
			if (type != CombinedMeshView.CombinedMeshType.House)
			{
				return LayerConstants.DefaultLayerId;
			}
			return LayerConstants.HeadlightOcclusionLayerId;
		}

		// Token: 0x06002BEF RID: 11247 RVA: 0x000C2530 File Offset: 0x000C0730
		private void Awake()
		{
			foreach (object obj in Enum.GetValues(typeof(CombinedMeshView.CombinedMeshType)))
			{
				CombinedMeshView.CombinedMeshType combinedMeshType = (CombinedMeshView.CombinedMeshType)obj;
				GameObject combinedMeshForType = new GameObject(string.Format("Combined {0} Meshes", combinedMeshType));
				combinedMeshForType.gameObject.transform.parent = base.transform;
				this._combinedMeshesForType.Add(combinedMeshType, combinedMeshForType);
			}
			this._materialPropertyBlock = new MaterialPropertyBlock();
		}

		// Token: 0x06002BF0 RID: 11248 RVA: 0x000C25D0 File Offset: 0x000C07D0
		public CombinedMeshView.Handle AddMesh(CombinedMeshView.CombinedMeshType combinedMeshType, Mesh mesh, Matrix4x4 localToWorldMatrix)
		{
			GameObject combinedMeshesForType = this._combinedMeshesForType[combinedMeshType];
			MeshFilter currentMeshFilter = null;
			if (this._currentMeshFilters.ContainsKey(combinedMeshType))
			{
				currentMeshFilter = this._currentMeshFilters[combinedMeshType];
			}
			else
			{
				this._currentMeshFilters.Add(combinedMeshType, null);
			}
			CombineInstance newMeshInstance = new CombineInstance
			{
				mesh = mesh,
				transform = localToWorldMatrix
			};
			CombineInstance[] combineInstances;
			if (currentMeshFilter == null || currentMeshFilter.mesh.vertexCount + mesh.vertexCount > 65535)
			{
				currentMeshFilter = this.CreateGameObjectForCombinedMesh(combinedMeshesForType, combinedMeshType);
				combineInstances = new CombineInstance[]
				{
					newMeshInstance
				};
				this._currentMeshFilters[combinedMeshType] = currentMeshFilter;
			}
			else
			{
				CombineInstance existingMeshInstance = new CombineInstance
				{
					mesh = currentMeshFilter.mesh,
					transform = currentMeshFilter.transform.localToWorldMatrix
				};
				combineInstances = new CombineInstance[]
				{
					existingMeshInstance,
					newMeshInstance
				};
			}
			Mesh newMesh = new Mesh();
			newMesh.CombineMeshes(combineInstances);
			currentMeshFilter.mesh = newMesh;
			if (!this._combinedMeshHandles.ContainsKey(combinedMeshType))
			{
				this._combinedMeshHandles[combinedMeshType] = new List<CombinedMeshView.Handle>();
			}
			CombinedMeshView.Handle handle = new CombinedMeshView.Handle(combinedMeshType, mesh, localToWorldMatrix, currentMeshFilter);
			this._combinedMeshHandles[combinedMeshType].Add(handle);
			return handle;
		}

		// Token: 0x06002BF1 RID: 11249 RVA: 0x000C2714 File Offset: 0x000C0914
		private MeshFilter CreateGameObjectForCombinedMesh(GameObject parent, CombinedMeshView.CombinedMeshType combinedMeshType)
		{
			GameObject gameObject = new GameObject(string.Format("Combined {0} Mesh", combinedMeshType));
			gameObject.transform.parent = parent.transform;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = this._combinedMeshMaterials.vertexColorMaterial;
			meshRenderer.sortingLayerID = this.SortingLayerForMeshType(combinedMeshType);
			meshRenderer.gameObject.layer = this.LayerForMeshType(combinedMeshType);
			if (combinedMeshType == CombinedMeshView.CombinedMeshType.House)
			{
				meshRenderer.GetPropertyBlock(this._materialPropertyBlock);
				this._materialPropertyBlock.SetFloat(ShaderConstants.MotorwayIdShaderId, 0f);
				this._materialPropertyBlock.SetFloat(ShaderConstants.HeadlightOcclusionTypeId, ShaderConstants.HeadlightNonVehicleOcclusionTypeId);
				meshRenderer.SetPropertyBlock(this._materialPropertyBlock);
			}
			return meshFilter;
		}

		// Token: 0x06002BF2 RID: 11250 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002BF3 RID: 11251 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002BF4 RID: 11252 RVA: 0x000C27CC File Offset: 0x000C09CC
		public void Reset()
		{
			foreach (GameObject gameObject in this._combinedMeshesForType.Values)
			{
				foreach (object obj in gameObject.transform)
				{
					GameObject gameObject2 = ((Transform)obj).gameObject;
					gameObject2.SetActive(false);
					UnityEngine.Object.Destroy(gameObject2);
				}
			}
			this._currentMeshFilters.Clear();
			this._combinedMeshHandles.Clear();
		}

		// Token: 0x06002BF5 RID: 11253 RVA: 0x000C2884 File Offset: 0x000C0A84
		public void RemoveMesh(CombinedMeshView.Handle meshToRemove)
		{
			if (meshToRemove == null)
			{
				return;
			}
			List<CombinedMeshView.Handle> handles;
			if (this._combinedMeshHandles.TryGetValue(meshToRemove.type, out handles))
			{
				handles.Remove(meshToRemove);
				if (handles.Count == 0)
				{
					GameObject gameObject = meshToRemove.meshFilterOfCombinedMesh.gameObject;
					gameObject.SetActive(false);
					UnityEngine.Object.Destroy(gameObject);
					return;
				}
				Mesh newMesh = new Mesh();
				CombineInstance[] combineInstances = new CombineInstance[handles.Count];
				int combineInstanceIndex = 0;
				foreach (CombinedMeshView.Handle handle in handles)
				{
					CombineInstance combineInstance = new CombineInstance
					{
						mesh = handle.mesh,
						transform = handle.localToWorldMatrix
					};
					combineInstances[combineInstanceIndex] = combineInstance;
					combineInstanceIndex++;
				}
				newMesh.CombineMeshes(combineInstances);
				meshToRemove.meshFilterOfCombinedMesh.mesh = newMesh;
			}
		}

		// Token: 0x04002615 RID: 9749
		private const int MaxVertexCountPerMesh = 65535;

		// Token: 0x04002616 RID: 9750
		[Dependency]
		private CombinedMeshMaterials _combinedMeshMaterials;

		// Token: 0x04002617 RID: 9751
		private readonly Dictionary<CombinedMeshView.CombinedMeshType, GameObject> _combinedMeshesForType = new Dictionary<CombinedMeshView.CombinedMeshType, GameObject>();

		// Token: 0x04002618 RID: 9752
		private readonly Dictionary<CombinedMeshView.CombinedMeshType, MeshFilter> _currentMeshFilters = new Dictionary<CombinedMeshView.CombinedMeshType, MeshFilter>();

		// Token: 0x04002619 RID: 9753
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x0400261A RID: 9754
		private readonly Dictionary<CombinedMeshView.CombinedMeshType, List<CombinedMeshView.Handle>> _combinedMeshHandles = new Dictionary<CombinedMeshView.CombinedMeshType, List<CombinedMeshView.Handle>>();

		// Token: 0x02000621 RID: 1569
		public enum CombinedMeshType
		{
			// Token: 0x0400261C RID: 9756
			Carpark,
			// Token: 0x0400261D RID: 9757
			House
		}

		// Token: 0x02000622 RID: 1570
		public class Handle
		{
			// Token: 0x06002BF7 RID: 11255 RVA: 0x000C2999 File Offset: 0x000C0B99
			public Handle(CombinedMeshView.CombinedMeshType type, Mesh mesh, Matrix4x4 localToWorldMatrix, MeshFilter meshFilterOfCombinedMesh)
			{
				this.type = type;
				this.mesh = mesh;
				this.localToWorldMatrix = localToWorldMatrix;
				this.meshFilterOfCombinedMesh = meshFilterOfCombinedMesh;
			}

			// Token: 0x0400261E RID: 9758
			public readonly CombinedMeshView.CombinedMeshType type;

			// Token: 0x0400261F RID: 9759
			public readonly Mesh mesh;

			// Token: 0x04002620 RID: 9760
			public readonly Matrix4x4 localToWorldMatrix;

			// Token: 0x04002621 RID: 9761
			public readonly MeshFilter meshFilterOfCombinedMesh;
		}
	}
}
