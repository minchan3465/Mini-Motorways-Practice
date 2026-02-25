using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Motorways.Editor {
	using Motorways.Visuals;
	using Motorways.Utils;

	public class RoadMeshBaker : EditorWindow {
        private RoadVisualSettings _settings;
        private string _savePath = "Assets/Motorways/Baked";

        [MenuItem("Motorways/Road Mesh Baker")]
		public static void ShowWindow() {
			GetWindow<RoadMeshBaker>("Road Baker");
		}

		private void OnGUI() {
			GUILayout.Label("Road Mesh Generator", EditorStyles.boldLabel);

			_settings = (RoadVisualSettings)EditorGUILayout.ObjectField("Visual Settings", _settings, typeof(RoadVisualSettings), false);
			_savePath = EditorGUILayout.TextField("Save Path", _savePath);

			if (GUILayout.Button("Bake Canonical Road Meshes")) {
				if (_settings == null) {
					EditorUtility.DisplayDialog("Error", "Please assign RoadVisualSettings first.", "OK");
					return;
				}
				BakeMeshes();
			}
		}

        private void BakeMeshes() {
            // 저장 폴더 확인 및 생성
            if (!AssetDatabase.IsValidFolder(_savePath)) {
                string[] folders = _savePath.Split('/');
                string currentPath = folders[0];
                for (int i = 1; i < folders.Length; i++) {
                    if (!AssetDatabase.IsValidFolder(currentPath + "/" + folders[i])) {
                        AssetDatabase.CreateFolder(currentPath, folders[i]);
                    }
                    currentPath += "/" + folders[i];
                }
            }

            // 서브 폴더 생성
            string meshPath = Path.Combine(_savePath, "Meshes");
            string defPath = Path.Combine(_savePath, "Definitions");
            if (!Directory.Exists(meshPath)) Directory.CreateDirectory(meshPath);
            if (!Directory.Exists(defPath)) Directory.CreateDirectory(defPath);

            // === 베이킹 작업 정의 ===
            List<BakeJob> jobs = new List<BakeJob>();

            // 1. Straight (직선: North <-> South)
            jobs.Add(new BakeJob("Road_Straight", new List<RoadConnection> {
                Connect(TileDirection.North, TileDirection.South)
            }, TileDirection.North | TileDirection.South));

            // 2. Corner (90도: North <-> East)
            jobs.Add(new BakeJob("Road_Corner", new List<RoadConnection> {
                Connect(TileDirection.North, TileDirection.East)
            }, TileDirection.North | TileDirection.East));

            // 3. DeadEnd (막다른 길: North -> Center)
            jobs.Add(new BakeJob("Road_DeadEnd", new List<RoadConnection> {
                Connect(TileDirection.North, TileDirection.None)
            }, TileDirection.North));

            // 4. T-Junction (삼거리: North, East, South -> Center Hub)
            jobs.Add(new BakeJob("Road_TJunction", new List<RoadConnection> {
                Connect(TileDirection.North, TileDirection.None),
                Connect(TileDirection.East, TileDirection.None),
                Connect(TileDirection.South, TileDirection.None)
            }, TileDirection.North | TileDirection.East | TileDirection.South));

            // 5. Cross (사거리: N, E, S, W -> Center Hub)
            jobs.Add(new BakeJob("Road_Cross", new List<RoadConnection> {
                Connect(TileDirection.North, TileDirection.None),
                Connect(TileDirection.East, TileDirection.None),
                Connect(TileDirection.South, TileDirection.None),
                Connect(TileDirection.West, TileDirection.None)
            }, TileDirection.North | TileDirection.East | TileDirection.South | TileDirection.West));


            // === 실행 ===
            int count = 0;
            foreach (var job in jobs) {
                EditorUtility.DisplayProgressBar("Baking Meshes", $"Generating {job.Name}...", (float)count / jobs.Count);

                // 1. 가짜 시그니처 생성
                RoadSignature signature = new RoadSignature(job.Mask, job.Mask, 0, job.Connections);

                // 2. 경로 빌드
                List<RoadVisualPath> paths = RoadPathBuilder.BuildPathsFromConnections(signature);

                // 3. 메쉬 빌드 (CombineInstance로 합치기 준비)
                CombineInstance[] combine = new CombineInstance[paths.Count];
                for (int i = 0; i < paths.Count; i++) {
                    Mesh subMesh = RoadMeshBuilder.BuildRoadMesh(paths[i], _settings);
                    combine[i].mesh = subMesh;
                    combine[i].transform = Matrix4x4.identity;
                }

                // 4. 최종 메쉬 합치기 (교차로 등은 여러 패스로 구성되므로)
                Mesh finalMesh = new Mesh();
                finalMesh.name = job.Name;
                finalMesh.CombineMeshes(combine, true, true);
                finalMesh.RecalculateNormals();
                finalMesh.RecalculateBounds();

                // 5. 메쉬 에셋 저장
                string assetPath = $"{meshPath}/{job.Name}.asset";
                AssetDatabase.CreateAsset(finalMesh, assetPath);

                // 6. Definition 에셋 생성 및 연결
                RoadTileDefinition def = ScriptableObject.CreateInstance<RoadTileDefinition>();
                def.MainMesh = finalMesh;
                def.IsIntersection = (job.Connections.Count > 2); // 3개 이상 연결이면 교차로로 간주

                string defAssetPath = $"{defPath}/Def_{job.Name}.asset";
                AssetDatabase.CreateAsset(def, defAssetPath);

                count++;
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=green>Baking Complete!</color> generated {jobs.Count} meshes and definitions.");
        }

        // 헬퍼: 연결 생성 (기본 TwoLane)
        private RoadConnection Connect(TileDirection from, TileDirection to) {
            return new RoadConnection(
                new RoadTileNode(from, RoadType.TwoLane),
                new RoadTileNode(to, RoadType.TwoLane)
            );
        }

        private struct BakeJob {
            public string Name;
            public List<RoadConnection> Connections;
            public TileDirection Mask;

            public BakeJob(string name, List<RoadConnection> conns, TileDirection mask) {
                Name = name;
                Connections = conns;
                Mask = mask;
            }
        }
    }
}
