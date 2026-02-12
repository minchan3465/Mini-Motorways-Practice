using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000624 RID: 1572
	public class DestinationMeshCombiner
	{
		// Token: 0x06002C00 RID: 11264 RVA: 0x000C2DC0 File Offset: 0x000C0FC0
		public Mesh GetCombinedMesh(DestinationMesh.Type type, TileDirection direction, int groupIndex, int visualVariantIndex)
		{
			Diagnostics.Verify(this._meshForDirection.ContainsKey(new ValueTuple<DestinationMesh.Type, TileDirection, int, int>(type, direction, groupIndex, visualVariantIndex)), string.Format("Couldn't get cached destination mesh for parameters: ({0}, {1}, {2}, {3}). Total meshes in cache: {4}", new object[]
			{
				type,
				direction,
				groupIndex,
				visualVariantIndex,
				this._meshForDirection.Count
			}));
			return this._meshForDirection[new ValueTuple<DestinationMesh.Type, TileDirection, int, int>(type, direction, groupIndex, visualVariantIndex)];
		}

		// Token: 0x06002C01 RID: 11265 RVA: 0x000C2E48 File Offset: 0x000C1048
		public DestinationMeshCombiner(GameObject destinationPrefab)
		{
			List<DestinationVisualVariant> destinationVisualVariants = new List<DestinationVisualVariant>();
			for (int i = 0; i < destinationPrefab.transform.childCount; i++)
			{
				DestinationVisualVariant destinationVisualVariant = destinationPrefab.transform.GetChild(i).GetComponent<DestinationVisualVariant>();
				if (destinationVisualVariant != null)
				{
					destinationVisualVariants.Add(destinationVisualVariant);
				}
				else
				{
					Debug.LogError("No DestinationVisualVariant component found in a top level child (" + destinationPrefab.transform.GetChild(i).name + ") on Destination prefab. Please add a DestinationVisualVariant component to each child of the Destination prefab.");
				}
			}
			for (int visualVariantIndex = 0; visualVariantIndex < destinationVisualVariants.Count; visualVariantIndex++)
			{
				DestinationMesh[] destinationMeshes = destinationVisualVariants[visualVariantIndex].gameObject.GetComponentsInChildren<DestinationMesh>(true);
				Array.Sort<DestinationMesh>(destinationMeshes, delegate(DestinationMesh meshA, DestinationMesh meshB)
				{
					float meshAz = meshA.transform.position.z;
					float meshBz = meshB.transform.position.z;
					if (meshAz > meshBz)
					{
						return -1;
					}
					if (meshAz >= meshBz)
					{
						return 0;
					}
					return 1;
				});
				foreach (object obj in Enum.GetValues(typeof(DestinationMesh.Type)))
				{
					DestinationMesh.Type destinationMeshType = (DestinationMesh.Type)obj;
					this.CreateMergedMesh(destinationMeshType, destinationMeshes, visualVariantIndex);
				}
				DestinationMesh[] array = destinationMeshes;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06002C02 RID: 11266 RVA: 0x000C2FA4 File Offset: 0x000C11A4
		private void CreateMergedMesh(DestinationMesh.Type type, DestinationMesh[] destinationMeshes, int visualVariantIndex)
		{
			Mesh baseMesh = this.CreateBaseDestinationMesh(type, destinationMeshes);
			TileDirection[] doorDirections = DestinationMeshCombiner.DoorDirections;
			if (type == DestinationMesh.Type.StationHorizontal || type == DestinationMesh.Type.StationVertical)
			{
				doorDirections = DestinationMeshCombiner.DoorDirectionsForStations;
			}
			foreach (TileDirection direction in doorDirections)
			{
				this.CreateDestinationMeshesForDirection(type, destinationMeshes, baseMesh, direction, visualVariantIndex);
			}
		}

		// Token: 0x06002C03 RID: 11267 RVA: 0x000C2FF0 File Offset: 0x000C11F0
		private Mesh CreateBaseDestinationMesh(DestinationMesh.Type type, DestinationMesh[] destinationMeshes)
		{
			Mesh baseMesh = new Mesh();
			List<CombineInstance> combineInstancesBase = new List<CombineInstance>();
			foreach (DestinationMesh destinationMesh in destinationMeshes)
			{
				if (destinationMesh.type == type && destinationMesh.direction == TileDirection.None)
				{
					this.CombineMesh(destinationMesh, combineInstancesBase);
				}
			}
			baseMesh.CombineMeshes(combineInstancesBase.ToArray());
			return baseMesh;
		}

		// Token: 0x06002C04 RID: 11268 RVA: 0x000C304C File Offset: 0x000C124C
		private void CreateDestinationMeshesForDirection(DestinationMesh.Type type, DestinationMesh[] destinationMeshes, Mesh baseMesh, TileDirection direction, int visualVariantIndex)
		{
			List<CombineInstance> combineInstancesDoor = new List<CombineInstance>
			{
				new CombineInstance
				{
					mesh = baseMesh,
					transform = Matrix4x4.identity
				}
			};
			foreach (DestinationMesh destinationMesh in destinationMeshes)
			{
				if (destinationMesh.type == type && destinationMesh.direction == direction)
				{
					this.CombineMesh(destinationMesh, combineInstancesDoor);
				}
			}
			Mesh doorMesh = new Mesh
			{
				name = string.Format("Destination ({0}, {1} Door)", type.ToString(), direction.ToString())
			};
			doorMesh.CombineMeshes(combineInstancesDoor.ToArray());
			List<Color> existingColors = new List<Color>();
			doorMesh.GetColors(existingColors);
			for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
			{
				Mesh meshForGroup = UnityEngine.Object.Instantiate<Mesh>(doorMesh);
				Color[] colors = new Color[doorMesh.vertexCount];
				int themeComponentGroupOffset = CombinedMeshThemeComponent.RelativeThemeComponentGroupTargetOffsetForGroup(groupIndex);
				for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
				{
					float themeComponentGroupTargetIndex = existingColors[colorIndex].r;
					colors[colorIndex] = new Color((float)themeComponentGroupOffset + themeComponentGroupTargetIndex, existingColors[colorIndex].g, existingColors[colorIndex].b, existingColors[colorIndex].a);
				}
				meshForGroup.SetColors(colors);
				this._meshForDirection[new ValueTuple<DestinationMesh.Type, TileDirection, int, int>(type, direction, groupIndex, visualVariantIndex)] = meshForGroup;
			}
		}

		// Token: 0x06002C05 RID: 11269 RVA: 0x000C31C0 File Offset: 0x000C13C0
		private void CombineMesh(DestinationMesh destinationMesh, in List<CombineInstance> combineInstances)
		{
			MeshFilter meshFilter = destinationMesh.GetComponent<MeshFilter>();
			Mesh meshWithVertexColors = UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh);
			CombinedMeshThemeComponent.SetRelativeVertexColorIndexForMesh(meshWithVertexColors, destinationMesh.groupTarget);
			CombineInstance combineInstance = new CombineInstance
			{
				mesh = meshWithVertexColors,
				transform = meshFilter.transform.localToWorldMatrix
			};
			combineInstances.Add(combineInstance);
		}

		// Token: 0x04002629 RID: 9769
		private static readonly TileDirection[] DoorDirectionsForStations = new TileDirection[]
		{
			TileDirection.North,
			TileDirection.South,
			TileDirection.East,
			TileDirection.West
		};

		// Token: 0x0400262A RID: 9770
		private static readonly TileDirection[] DoorDirections = new TileDirection[]
		{
			TileDirection.South,
			TileDirection.West
		};

		// Token: 0x0400262B RID: 9771
		[TupleElementNames(new string[]
		{
			null,
			null,
			"groupIndex",
			"visualVariantIndex"
		})]
		private readonly Dictionary<ValueTuple<DestinationMesh.Type, TileDirection, int, int>, Mesh> _meshForDirection = new Dictionary<ValueTuple<DestinationMesh.Type, TileDirection, int, int>, Mesh>();
	}
}
