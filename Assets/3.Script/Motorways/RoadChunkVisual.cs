using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Rendering;

	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RoadChunkVisual : MonoBehaviour {
		[Header("Settings")]
		public RoadTileAtlas atlas; //구워둔 아틀라스 연결.
		public Vector2Int chunkCoord; //이 청크의 좌표.
		public int chunkSize = 16;

		[Header("Materials")]
		//Material 0: 도로 본체, Material 1: 테두리
		public Material roadMaterial;
		public Material outlineMaterial;

		//내부 데이터
		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;
		private Mesh _combinedMesh;

		//최적화용 버퍼
		private MeshBuffer _roadBuffer = new MeshBuffer();
		private MeshBuffer _outlineBuffer = new MeshBuffer();

        //성능 개선. 매번 new List를 하지 않고 전역에서 재사용하여 GC 억제
        private List<Vector3> _allVerts = new List<Vector3>(16384);
        private List<Vector2> _allUVs = new List<Vector2>(16384);
        private List<int> _trisRoad = new List<int>(16384);
        private List<int> _trisOutline = new List<int>(16384);

        private bool _isDirty = false;

		private void Awake() {
			TryGetComponent(out _meshFilter);
			TryGetComponent(out _meshRenderer);

			_combinedMesh = new Mesh();
			_combinedMesh.name = $"Chunk_{chunkCoord}";
			_meshFilter.mesh = _combinedMesh;

			if(roadMaterial != null && outlineMaterial != null) {
				_meshRenderer.materials = new Material[] { roadMaterial, outlineMaterial };
			}
		}

        public void Initialize(RoadTileAtlas atlasAsset) {
            this.atlas = atlasAsset;
            if (roadMaterial != null && outlineMaterial != null) {
                _meshRenderer.materials = new Material[] { roadMaterial, outlineMaterial };
            }
        }

        public void MarkDirty() => _isDirty = true;

        private void LateUpdate() {
			if (_isDirty) {
				RebuildMesh();
				_isDirty = false;
			}
		}

        private void RebuildMesh() {
            if (atlas == null || MapManager.Instance == null) return;

            _roadBuffer.Clear();
            _outlineBuffer.Clear();

            // 1. 16x16 루프
            for (int x = 0; x < chunkSize; x++) {
                for (int y = 0; y < chunkSize; y++) {
                    // 로컬 좌표 -> 전체 맵 좌표 변환
                    int mapX = chunkCoord.x * chunkSize + x;
                    int mapY = chunkCoord.y * chunkSize + y;
                    Vector2Int targetCoord = new Vector2Int(mapX, mapY);
                    TileData tile = MapManager.Instance.GetTileData(targetCoord);// 맵 범위 체크

                    if (tile == null || !tile.HasAnyRoad) continue;// 도로가 없는 타일은 패스

                    // 시그니처 생성
                    RoadTileSignature signature = BuildSignatureFromTile(tile);
                    if (signature == null || signature.Count == 0) continue;

                    // 아틀라스 조회
                    RoadTileDefinition def = atlas.ConstructDefinitionFromSignature(signature);

                    if (def != null && def.mesh != null) {
                        // 청크 내부의 로컬 위치 오프셋 계산
                        Vector3 tilePosOffset = new Vector3(x + 0.5f, 0, y + 0.5f);

                        if (def.mesh.road != null)
                            _roadBuffer.Append(def.mesh.road, tilePosOffset, def.rotationSteps);
                        if (def.mesh.outline != null)
                            _outlineBuffer.Append(def.mesh.outline, tilePosOffset, def.rotationSteps);
                    }
                }
            }

            // 5. 최종 메쉬 적용

            // 정점 데이터 합치기 (SubMesh를 쓰더라도 Vertices는 하나로 합쳐야 함)
            // 간단한 처리를 위해 SubMesh 구조에 맞춰 CombineInstance를 쓸 수도 있지만,
            // 여기서는 하나의 Vertices 리스트에 다 넣고, Triangles 인덱스만 분리하는 고급 기법을 씁니다.

            _allVerts.Clear(); _allUVs.Clear();
            _trisRoad.Clear(); _trisOutline.Clear();

            _allVerts.AddRange(_roadBuffer.vertices);
            int outlineVertOffset = _allVerts.Count;
            _allVerts.AddRange(_outlineBuffer.vertices);

            _allUVs.AddRange(_roadBuffer.uvs);
            _allUVs.AddRange(_outlineBuffer.uvs);

            _trisRoad.AddRange(_roadBuffer.triangles);
            foreach (int tri in _outlineBuffer.triangles) _trisOutline.Add(tri + outlineVertOffset);

            // Unity Mesh에 업로드
            _combinedMesh.Clear();

            _combinedMesh.SetVertices(_allVerts);
            _combinedMesh.SetUVs(0, _allUVs);

            _combinedMesh.subMeshCount = 2;
            _combinedMesh.SetTriangles(_trisRoad, 0);    // SubMesh 0 -> Material 0
            _combinedMesh.SetTriangles(_trisOutline, 1); // SubMesh 1 -> Material 1

            _combinedMesh.RecalculateBounds();
            //_combinedMesh.RecalculateNormals(); // 조명 필요 시
        }

        // TileData의 Lane/RoadState 정보를 바탕으로 Signature를 만드는 헬퍼
        private RoadTileSignature BuildSignatureFromTile(TileData tile) {
            RoadTileSignature sig = new RoadTileSignature();
            List<TileDirection> activeDirs = new List<TileDirection>();
            // 8방향 검사
            // TileData.RoadStates 배열 순서는 TileDirection 열거형 비트 순서와 일치한다고 가정
            // (North=0, NorthEast=1, East=2 ...)
            for (int i = 0; i < 8; i++) {
                if (tile.RoadStates[i] != RoadState.None) activeDirs.Add((TileDirection)(1 << i));
            }
            if (activeDirs.Count == 0) return sig;

            if (activeDirs.Count == 1) {
                sig.AddConnection(new RoadTileConnection(new RoadTileNode(activeDirs[0], RoadType.TwoLane), new RoadTileNode(activeDirs[0], RoadType.TwoLane)));
            } else {
                for (int a = 0; a < activeDirs.Count; a++) {
                    for (int b = a + 1; b < activeDirs.Count; b++) {
                        sig.AddConnection(new RoadTileConnection(
                            new RoadTileNode(activeDirs[a], RoadType.TwoLane),
                            new RoadTileNode(activeDirs[b], RoadType.TwoLane)));
                    }
                }
            }
            return sig;
        }
    }
}
