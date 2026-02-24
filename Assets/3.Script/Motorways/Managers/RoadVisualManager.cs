using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Motorways.Visuals;

	public class RoadVisualManager : MonoBehaviour {
		public static RoadVisualManager Instance;

		[Tooltip("도로 폭, 해상도 등 렌더링 상수 ScriptableObject")]
		public RoadVisualSettings VisualSettings;

		[Tooltip("URP용 도로 아스팔트 머티리얼")]
		public Material RoadMaterial;

		private Dictionary<Vector2Int, GameObject> _roadObjects = new Dictionary<Vector2Int, GameObject>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		public void UpdateTileVisuals(IEnumerable<Vector2Int> changedNodes) {
			foreach(Vector2Int node in changedNodes) {
				UpdateSingleTile(node);
			}
		}

		private void UpdateSingleTile(Vector2Int coord) {
			if(_roadObjects.TryGetValue(coord, out GameObject existingObj)) {
				Destroy(existingObj);
				_roadObjects.Remove(coord);
			}

			if (!MapManager.Instance._grid.TryGetValue(coord, out TileData tileData)) return;

			RoadSignature signature = RoadSignatureAnalyzer.Analyze(tileData);
			if (signature.RawMask == TileDirection.None) return;

			Vector3 tileWorldPos = new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f);
			List<RoadVisualPath> paths = RoadPathBuilder.BuildPaths(signature, tileWorldPos);

			if (paths.Count == 0) return;

			List<CombineInstance> combineInstances = new List<CombineInstance>();
			foreach(RoadVisualPath path in paths) {
				Mesh pathMesh = RoadMeshBuilder.BuildRoadMesh(path, VisualSettings);
				if(pathMesh != null) {
					CombineInstance ci = new CombineInstance();
					ci.mesh = pathMesh;
					//RoadPathBuilder에서 이미 로컬 좌표를 월드 좌표 기준으로 더해주었으므로 identity 사용
					ci.transform = Matrix4x4.identity;
					combineInstances.Add(ci);
				}
			}

			if (combineInstances.Count == 0) return;

			Mesh finalMesh = new Mesh();

			finalMesh.name = $"MergedRoadMesh_{coord.x}_{coord.y}";
			//두번째 파라미터를 true로 하여, 단일 머티리얼 렌더링 보장.
			finalMesh.CombineMeshes(combineInstances.ToArray(), true, false);

			GameObject roadObj = new GameObject($"RoadVisual_{coord.x}_{coord.y}");
			roadObj.transform.SetParent(this.transform);
			roadObj.transform.position = Vector3.zero;

			MeshFilter meshFilter = roadObj.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = finalMesh;

			MeshRenderer meshRenderer = roadObj.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = RoadMaterial;

			_roadObjects.Add(coord, roadObj);
		}
	}
}
