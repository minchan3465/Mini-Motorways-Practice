using System;
using Motorways.Constants;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000627 RID: 1575
	public class VehicleMeshCombiner
	{
		// Token: 0x06002C0E RID: 11278 RVA: 0x000C3488 File Offset: 0x000C1688
		public VehicleMeshCombiner(GameObject vehiclePrefab)
		{
			this.combinedMeshVehiclePrefab = UnityEngine.Object.Instantiate<GameObject>(vehiclePrefab);
			this.combinedMeshVehiclePrefab.SetActive(false);
			this.combinedMeshVehiclePrefab.hideFlags = HideFlags.HideAndDontSave;
			GameObject combinedMeshGameObject = new GameObject("Combined Vehicle Mesh");
			combinedMeshGameObject.transform.parent = this.combinedMeshVehiclePrefab.transform;
			VehicleMesh[] vehicleMeshes = this.combinedMeshVehiclePrefab.GetComponentsInChildren<VehicleMesh>();
			Mesh combinedVehicleMesh = this.CombineVehicleMesh(vehicleMeshes);
			MeshFilter meshFilter = combinedMeshGameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = combinedVehicleMesh;
			meshFilter.gameObject.layer = LayerConstants.HeadlightOcclusionLayerId;
			VehicleView vehicleView = this.combinedMeshVehiclePrefab.GetComponentInChildren<VehicleView>();
			Material vehicleMaterial = vehicleView.vehicleMaterial;
			VehicleMesh[] array = vehicleMeshes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
			MeshRenderer meshRenderer = combinedMeshGameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = vehicleMaterial;
			vehicleView.CombinedMeshVehicleRenderer = meshRenderer;
		}

		// Token: 0x06002C0F RID: 11279 RVA: 0x000C3564 File Offset: 0x000C1764
		private Mesh CombineVehicleMesh(VehicleMesh[] vehicleMeshes)
		{
			CombineInstance[] combineInstances = new CombineInstance[vehicleMeshes.Length];
			for (int combineInstanceIndex = 0; combineInstanceIndex < combineInstances.Length; combineInstanceIndex++)
			{
				this.Combine(combineInstanceIndex, vehicleMeshes[combineInstanceIndex], combineInstances);
			}
			Mesh mesh = new Mesh();
			mesh.name = "Combined Vehicle Mesh";
			mesh.CombineMeshes(combineInstances);
			return mesh;
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x000C35AC File Offset: 0x000C17AC
		private void Combine(int index, VehicleMesh vehicleMesh, in CombineInstance[] combineInstances)
		{
			MeshFilter meshFilter = vehicleMesh.GetComponent<MeshFilter>();
			Mesh meshWithVertexColors = UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh);
			CombinedMeshThemeComponent.SetRelativeVertexColorIndexForMesh(meshWithVertexColors, vehicleMesh.groupTarget);
			combineInstances[index].mesh = meshWithVertexColors;
			combineInstances[index].transform = meshFilter.transform.localToWorldMatrix;
		}

		// Token: 0x04002631 RID: 9777
		private const string CombinedVehicleMeshName = "Combined Vehicle Mesh";

		// Token: 0x04002632 RID: 9778
		public readonly GameObject combinedMeshVehiclePrefab;
	}
}
