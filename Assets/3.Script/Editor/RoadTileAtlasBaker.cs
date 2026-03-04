#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Motorways.Editor {
	using Motorways.Baking;

	public class RoadTileAtlasBaker{
		[MenuItem("Motorways/Bake Road Tile Atlas")]
		public static void BakeAtlas() {
			string savePath = "Assets/2.Model/RoadTileAtlas.asset";

			//새 아틀라스 에셋 생성
			RoadTileAtlas atlas = ScriptableObject.CreateInstance<RoadTileAtlas>();
			AssetDatabase.CreateAsset(atlas, savePath);

			//빌더 준비
			RoadTileAtlasBuilder builder = new RoadTileAtlasBuilder();

			//공정 1. 모든 고유 시그니처 추출.
			List<RoadTileSignature> uniqueSignatures = builder.GenerateAllUniqueSignatures();

			int index = 0;

			float bodyWidth = 0.4f;
			float outlineWidth = 0.5f;
			float bodyYOffset = 0.01f;   //본체는 약간 위에 배치하여 깜빡임 방지
			//float outlineYOffset = 0.00f; //테두리는 바닥에 밀착

			foreach (var signature in uniqueSignatures) {
				RoadTileDefinition def = new RoadTileDefinition();
				def.index = index++;
				def.signature = signature;
				def.mesh = new RoadTileMesh();

				//List<Vector2> combinedVisualPoints = new List<Vector2>();
				List<RoadMeshData> roadMeshes = new List<RoadMeshData>();
				List<RoadMeshData> outlineMeshes = new List<RoadMeshData>();


				//공정 2. 각 연결선마다 베지어 곡선 도출.
				foreach (var connection in signature.connections) {
					List<Vector2> pathPoints = builder.ConstructPathForConnection(connection);
					def.connectionToPath.Add(connection, pathPoints);
					//combinedVisualPoints.AddRange(pathPoints);
					bool roundEnds = signature.IsDeadEnd;
					roadMeshes.Add(builder.ConstructMeshFromPath(pathPoints, bodyWidth, roundEnds, bodyYOffset));
					outlineMeshes.Add(builder.ConstructMeshFromPath(pathPoints, outlineWidth, roundEnds, 0f));
				}

				//끝이면 둥글게 마감.
				//bool roundEnds = signature.IsDeadEnd;

				def.mesh.road = builder.CombineMeshData(roadMeshes);
				def.mesh.outline = builder.CombineMeshData(outlineMeshes);

				atlas.definitions.Add(def);
			}

			//--- 2. 코너 메쉬 굽기 추가 ---
			List<Vector2> cornerPath = builder.ConstructCornerPath();
			atlas.cornerMesh = new RoadTileMesh();
			//코너는 끝을 둥글게(roundEnds) 마감할 필요가 없습니다. 빈틈만 메우기 때문입니다.
			atlas.cornerMesh.road = builder.ConstructMeshFromPath(cornerPath, bodyWidth, false, bodyYOffset);
			atlas.cornerMesh.outline = builder.ConstructMeshFromPath(cornerPath, outlineWidth, false, 0f);


			EditorUtility.SetDirty(atlas);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log($"[Motorways] 아틀라스 굽기 완료! 총 {uniqueSignatures.Count}개의 고유 타일 메쉬가 생성되었습니다. 위치: {savePath}");
		}
	}
}
#endif