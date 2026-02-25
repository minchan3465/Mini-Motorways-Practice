using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Motorways.Managers {
	using Motorways.Visuals;

	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RoadVisualManager : MonoBehaviour {
		public static RoadVisualManager Instance;

		[Header("Assets & Settings")]
		[Tooltip("미리 구워진 메쉬들이 등록된 아틀라스")]
		public RoadTileAtlas RoadAtlas;

		[Tooltip("빛 연산이 없는 URP Unlit 머티리얼 할당")]
		public Material RoadUnlitMaterial;

		// --- 최적화: 오브젝트 풀링 및 상태 관리 ---
		// 현재 맵에 활성화된 도로 뷰 (좌표 -> 뷰)
		private Dictionary<Vector2Int, RoadView> _activeViews = new Dictionary<Vector2Int, RoadView>();

		// 재사용을 위해 대기 중인 뷰 큐
		private Queue<RoadView> _viewPool = new Queue<RoadView>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		// 외부(MapManager 등)에서 타일이 변경되었을 때 호출하는 진입점
		public void UpdateTileVisuals(IEnumerable<Vector2Int> changedNodes) {
			foreach (Vector2Int node in changedNodes) {
				UpdateSingleTile(node);
			}
		}

		private void UpdateSingleTile(Vector2Int coord) {
			// 1. 맵 데이터에서 해당 좌표의 타일 상태를 가져옴
			if (!MapManager.Instance._grid.TryGetValue(coord, out TileData tileData)) {
				RemoveRoadView(coord);
				return;
			}

			// 2. 타일 데이터 분석 -> 시그니처 생성
			RoadSignature signature = RoadSignatureAnalyzer.Analyze(tileData);

			// 도로가 없는 빈 타일이거나 활성 노드가 없으면 시각물 제거
			if (signature.Connections == null || signature.Connections.Count == 0) {
				RemoveRoadView(coord);
				return;
			}

			// 3. 아틀라스에서 알맞은 메쉬 정의(Definition) 검색
			RoadTileDefinition definition = RoadAtlas.GetDefinition(signature);

			if (definition == null) {
				Debug.LogWarning($"[RoadVisualManager] 매핑된 메쉬가 아틀라스에 없습니다! Signature: {signature.CanonicalMask}");
				RemoveRoadView(coord);
				return;
			}

			// 4. 오브젝트 풀에서 RoadView 가져오기 (없으면 새로 생성)
			RoadView view = GetOrCreateView(coord);

			// 5. RoadView 초기화 (메쉬 할당 및 회전 적용)
			view.Initialize(definition, signature, RoadUnlitMaterial);

			// 6. 위치 설정 (타일의 중앙)
			view.transform.position = new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f);
		}

		// 특정 좌표의 도로 시각물을 풀(Pool)로 반환
		private void RemoveRoadView(Vector2Int coord) {
			if (_activeViews.TryGetValue(coord, out RoadView view)) {
				view.Deactive();
				_activeViews.Remove(coord);
				_viewPool.Enqueue(view); // 풀에 반납
			}
		}

		// 풀에서 꺼내오거나 새로 생성하는 헬퍼 메서드
		private RoadView GetOrCreateView(Vector2Int coord) {
			if (_activeViews.TryGetValue(coord, out RoadView existingView)) {
				return existingView; // 이미 해당 위치에 뷰가 있으면 재사용
			}

			RoadView newView;
			if (_viewPool.Count > 0) {
				newView = _viewPool.Dequeue(); // 풀에서 꺼냄
			} else {
				// 풀이 비어있으면 새로 생성
				GameObject go = new GameObject("RoadView_Pooled");
				go.transform.SetParent(this.transform);
				newView = go.AddComponent<RoadView>();
			}

			_activeViews[coord] = newView;
			return newView;
		}

		//과거의 잔재...
		//public static RoadVisualManager Instance;

		//[Header("Settings")]
		//public RoadVisualSettings VisualSettings;

		//[Tooltip("빛 연산이 없는 URP Unlit 머티리얼 할당")]
		//public Material RoadUnlitMaterial;

		//private MeshFilter _meshFilter;
		//private MeshRenderer _meshRenderer;
		//private Mesh _batchedMesh;

		////최적화 리팩토링.
		////private Dictionary<Vector2Int, GameObject> _roadObjects = new Dictionary<Vector2Int, GameObject>();
		////에서
		////1. 메쉬 캐시. (정규화 형태 -> 생성된 메쉬)
		////private Dictionary<TileDirection, Mesh> _meshCache = new Dictionary<TileDirection, Mesh>();
		////2. 오브젝트 풀
		////private Queue<RoadView> _viewPool = new Queue<RoadView>();
		////현재 활성화된 뷰 관리 (좌표 -> 뷰)
		////private Dictionary<Vector2Int, RoadView> _activeViews = new Dictionary<Vector2Int, RoadView>();

		////---근데 또 리팩토링 하느라 위에 최적화 의미 없음 ㅋㅋㅋ, 대신 캐시는 그대로 이어받음.
		//// 타일 좌표별 로컬 정점 데이터를 캐싱하는 클래스
		//private class TileMeshData {
		//	public List<Vector3> Vertices = new List<Vector3>();
		//	public List<int> Triangles = new List<int>();
		//	public List<Vector2> UVs = new List<Vector2>();
		//}
		//private Dictionary<Vector2Int, TileMeshData> _tileDataCache = new Dictionary<Vector2Int, TileMeshData>();

		//private bool _isDirty = false;

		////-----------------------------------------

		////싱글톤
		//private void Awake() {
		//	if (Instance == null) Instance = this;
		//	else Destroy(gameObject);

		//	_meshFilter = GetComponent<MeshFilter>();
		//	if (_meshFilter == null) _meshFilter = gameObject.AddComponent<MeshFilter>();

		//	_meshRenderer = GetComponent<MeshRenderer>();
		//	if (_meshRenderer == null) _meshRenderer = gameObject.AddComponent<MeshRenderer>();

		//	_meshRenderer.sharedMaterial = RoadUnlitMaterial;

		//	_batchedMesh = new Mesh();
		//	_batchedMesh.name = "Batched_Road_Mesh";

		//	//맵이 넓어져 정점이 65,535개를 초과해도 깨지지 않도록 32비트 버퍼 강제 적용
		//	_batchedMesh.indexFormat = IndexFormat.UInt32;
		//	_meshFilter.sharedMesh = _batchedMesh;
		//}

		////-----------------------------------------

		//public void UpdateTileVisuals(IEnumerable<Vector2Int> changedNodes) {
		//	foreach (Vector2Int node in changedNodes) {
		//		UpdateSingleTile(node);
		//	}
		//	_isDirty = true;	//갱신 플래그 활성화.
		//}

		//private void UpdateSingleTile(Vector2Int coord) {
		//	//변경된 타일의 기존 캐시 데이터 삭제.
		//	_tileDataCache.Remove(coord);

		//	//데이터 확인
		//	if (!MapManager.Instance._grid.TryGetValue(coord, out TileData tileData)) return;

		//	//서명 분석
		//	RoadSignature signature = RoadSignatureAnalyzer.Analyze(tileData);
		//	if (signature.Connections == null || signature.Connections.Count == 0) return;

		//	// 해당 타일을 관통하는 모든 연결선 메쉬 생성
		//	List<RoadVisualPath> paths = RoadPathBuilder.BuildPathsFromConnections(signature);

		//	TileMeshData newData = new TileMeshData();

		//	// 타일의 월드 좌표 기준점
		//	Vector3 tileWorldPos = new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f);

		//	foreach (RoadVisualPath path in paths) {
		//		Mesh pathMesh = RoadMeshBuilder.BuildRoadMesh(path, VisualSettings);
		//		if (pathMesh == null) continue;

		//		Vector3[] localVerts = pathMesh.vertices;
		//		int[] localTris = pathMesh.triangles;
		//		Vector2[] localUVs = pathMesh.uv;

		//		int vertexOffset = newData.Vertices.Count;

		//		//로컬 좌표를 월드 좌표로 변환하여 저장
		//		for (int i = 0; i < localVerts.Length; i++) {
		//			newData.Vertices.Add(localVerts[i] + tileWorldPos);
		//			newData.UVs.Add(localUVs[i]);
		//		}

		//		//현재까지 누적된 정점 개수만큼 인덱스 오프셋 적용
		//		for (int i = 0; i < localTris.Length; i++) {
		//			newData.Triangles.Add(localTris[i] + vertexOffset);
		//		}
		//	}

		//	_tileDataCache[coord] = newData;
		//}

		//private void LateUpdate() {
		//	//한 프레임에 여러 타일이 변경되었더라도, 렌더링 병합 연산은 프레임 끝에 단 1번만 수행
		//	if (_isDirty) {
		//		ApplyBatchedMesh();
		//		_isDirty = false;
		//	}
		//}

		//private void ApplyBatchedMesh() {
		//	_batchedMesh.Clear();

		//	List<Vector3> allVertices = new List<Vector3>();
		//	List<int> allTriangles = new List<int>();
		//	List<Vector2> allUVs = new List<Vector2>();

		//	foreach (var kvp in _tileDataCache) {
		//		TileMeshData data = kvp.Value;
		//		int currentVertexCount = allVertices.Count;

		//		allVertices.AddRange(data.Vertices);
		//		allUVs.AddRange(data.UVs);

		//		foreach (int tri in data.Triangles) {
		//			allTriangles.Add(tri + currentVertexCount);
		//		}
		//	}

		//	_batchedMesh.SetVertices(allVertices);
		//	_batchedMesh.SetUVs(0, allUVs);
		//	_batchedMesh.SetTriangles(allTriangles, 0);
		//}





		//private Mesh GetMeshFromCacheOrBuild(TileDirection canonicalMask) {
		//	//캐시에 있으면 그거 리턴.
		//	if (_meshCache.TryGetValue(canonicalMask, out Mesh cachedMesh)) {
		//		return cachedMesh;
		//	}

		//	//없으면... 만들어야지...
		//	List<RoadVisualPath> paths = RoadPathBuilder.BuildCanonicalPaths(canonicalMask);

		//	if (paths.Count == 0) return null;

		//	List<CombineInstance> combineInstances = new List<CombineInstance>();
		//	foreach (RoadVisualPath path in paths) {
		//		Mesh pathMesh = RoadMeshBuilder.BuildRoadMesh(path, VisualSettings);
		//		if (pathMesh != null) {
		//			CombineInstance ci = new CombineInstance();
		//			ci.mesh = pathMesh;
		//			//RoadPathBuilder에서 이미 로컬 좌표를 월드 좌표 기준으로 더해주었으므로 identity 사용
		//			ci.transform = Matrix4x4.identity;
		//			combineInstances.Add(ci);
		//		}
		//	}

		//	Mesh finalMesh = new Mesh();
		//	finalMesh.name = $"RoadMesh_{canonicalMask}";
		//	finalMesh.CombineMeshes(combineInstances.ToArray(), true, false);

		//	_meshCache.Add(canonicalMask, finalMesh);	//캐시 저장.
		//	return finalMesh;
		//}

		//private RoadView GetViewFromPool() {
		//	if(_viewPool.Count > 0) {
		//		return _viewPool.Dequeue();
		//	}

		//	//풀이 비어있으면 새로 생성
		//	GameObject go = new GameObject("RoadView_Pooled");
		//	go.transform.SetParent(this.transform);
		//	MeshFilter mf = go.AddComponent<MeshFilter>();
		//	MeshRenderer mr = go.AddComponent<MeshRenderer>();
		//	RoadView view = go.AddComponent<RoadView>();

		//	// private 필드 할당 (리플렉션 없이 하려면 RoadView에 public/Set 메서드 추가 필요)
		//	// 편의상 Unity Inspector에서 할당하듯 컴포넌트 세팅
		//	// 실제로는 RoadView 스크립트에 Awake에서 GetComponent 하거나 여기서 할당 함수를 호출
		//	// 간단하게 RoadView 코드를 수정해서 [SerializeField] 대신 GetComponent 쓰게 하거나, 
		//	// 여기서 세팅해줄 수 있습니다. 
		//	// *이번 예제에서는 RoadView 내부에서 GetComponent를 쓰는게 안전하므로 RoadView 코드 수정 권장*

		//	return view;
		//}
	}
}
