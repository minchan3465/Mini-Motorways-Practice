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

			// 새 아틀라스 에셋 파일 생성
			RoadTileAtlas atlas = ScriptableObject.CreateInstance<RoadTileAtlas>();
			AssetDatabase.CreateAsset(atlas, savePath);

			// 빌더 준비
			RoadTileAtlasBuilder builder = new RoadTileAtlasBuilder();

			// 단계 1. 가능한 모든 연결 시그니처 생성
			List<RoadTileSignature> uniqueSignatures = builder.GenerateAllUniqueSignatures();

			int index = 0;

			// 도로 두께 설정
			float bodyWidth = 0.8f;
			float outlineWidth = 1.2f;
			float bodyYOffset = 0.01f;   // 도로 본체를 약간 높게 배치하여 겹침 방지

			foreach (var signature in uniqueSignatures) {
				RoadTileDefinition def = new RoadTileDefinition();
				def.index = index++;
				def.signature = signature;
				def.mesh = new RoadTileMesh();

				List<RoadMeshData> roadMeshes = new List<RoadMeshData>();
				List<RoadMeshData> outlineMeshes = new List<RoadMeshData>();


				// 단계 2. 각 연결 상태에 따른 베지어 곡선 생성 및 메쉬화
				foreach (var connection in signature.connections) {
					List<Vector2> pathPoints = builder.ConstructPathForConnection(connection);
					def.connectionToPath.Add(connection, pathPoints);
					
					bool roundEnds = signature.IsDeadEnd;
					roadMeshes.Add(builder.ConstructMeshFromPath(pathPoints, bodyWidth, roundEnds, bodyYOffset));
					outlineMeshes.Add(builder.ConstructMeshFromPath(pathPoints, outlineWidth, roundEnds, 0f));
				}

				// 개별 메쉬들을 하나로 합쳐서 저장
				def.mesh.road = builder.CombineMeshData(roadMeshes);
				def.mesh.outline = builder.CombineMeshData(outlineMeshes);

				atlas.definitions.Add(def);
			}

			//--- 3. 코너(교차점)용 메쉬 추가 생성 ---
			List<Vector2> cornerPath = builder.ConstructCornerPath();
			atlas.cornerMesh = new RoadTileMesh();
			// 코너는 둥근 끝단 처리가 필요 없으므로 false 전달
			atlas.cornerMesh.road = builder.ConstructMeshFromPath(cornerPath, bodyWidth, false, bodyYOffset);
			atlas.cornerMesh.outline = builder.ConstructMeshFromPath(cornerPath, outlineWidth, false, 0f);


			EditorUtility.SetDirty(atlas);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log($"[Motorways] 아틀라스 생성 완료! 총 {uniqueSignatures.Count}개의 도로 타입 메쉬가 구워졌습니다. 위치: {savePath}");
		}
	}
}
#endif
