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
			//float bodyYOffset = 0.00f;   //본체는 약간 위에 배치하여 깜빡임 방지
			//float outlineYOffset = 0.00f; //테두리는 바닥에 밀착

			foreach (var signature in uniqueSignatures) {
				RoadTileDefinition def = new RoadTileDefinition();
				def.index = index++;
				def.signature = signature;
				def.mesh = new RoadTileMesh();

				List<Vector2> combinedVisualPoints = new List<Vector2>();

				//공정 2. 각 연결선마다 베지어 곡선 도출.
				foreach (var connection in signature.connections) {
					List<Vector2> pathPoints = builder.ConstructPathForConnection(connection);
					def.connectionToPath.Add(connection, pathPoints);
					combinedVisualPoints.AddRange(pathPoints);
				}

				//끝이면 둥글게 마감.
				bool roundEnds = signature.IsDeadEnd;

				def.mesh.road = builder.ConstructMeshFromPath(combinedVisualPoints, bodyWidth, roundEnds);
				def.mesh.outline = builder.ConstructMeshFromPath(combinedVisualPoints, outlineWidth, roundEnds);

				atlas.definitions.Add(def);
			}

			EditorUtility.SetDirty(atlas);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log($"[Motorways] 아틀라스 굽기 완료! 총 {uniqueSignatures.Count}개의 고유 타일 메쉬가 생성되었습니다. 위치: {savePath}");
		}
	}
}
#endif