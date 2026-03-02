using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Motorways.Rendering;
	public class RoadChunkManager : MonoBehaviour {
		public static RoadChunkManager Instance;

		[Header("Settings")]
		public RoadTileAtlas roadAtlas;
		public Material roadMaterial;
		public Material outlineMaterial;
		public Material mothballedMaterial;
		public int chunkSize = 16;

		private Dictionary<Vector2Int, RoadChunkVisual> _chunks = new Dictionary<Vector2Int, RoadChunkVisual>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		public RoadChunkVisual GetOrCreateChunk(Vector2Int worldCoord) {
			int chunkX = Mathf.FloorToInt((float)worldCoord.x / chunkSize);
			int chunkY = Mathf.FloorToInt((float)worldCoord.y / chunkSize);
			Vector2Int chunkCoord = new Vector2Int(chunkX, chunkY);

			if (!_chunks.TryGetValue(chunkCoord, out RoadChunkVisual chunk)) {
				// 새 청크 게임오브젝트 생성
				GameObject chunkObj = new GameObject($"RoadChunk_{chunkCoord.x}_{chunkCoord.y}");
				chunkObj.transform.SetParent(this.transform);

				chunk = chunkObj.AddComponent<RoadChunkVisual>();
				chunk.chunkCoord = chunkCoord;
				chunk.chunkSize = this.chunkSize;
				chunk.roadMaterial = this.roadMaterial;
				chunk.outlineMaterial = this.outlineMaterial;
				chunk.mothballedMaterial = this.mothballedMaterial;
				chunkObj.transform.position = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);

				// 아틀라스 주입 및 초기화
				chunk.Initialize(this.roadAtlas);

				_chunks.Add(chunkCoord, chunk);
			}

			return chunk;
		}

		public void MarkChunksDirty(HashSet<Vector2Int> changedNodes) {
			HashSet<RoadChunkVisual> chunksToUpdate = new HashSet<RoadChunkVisual>();

			foreach (Vector2Int nodeCoord in changedNodes) {
				RoadChunkVisual chunk = GetOrCreateChunk(nodeCoord);
				chunksToUpdate.Add(chunk);
			}

			// 모인 청크들에게만 재조립(Rebuild) 명령 하달
			foreach (RoadChunkVisual chunk in chunksToUpdate) {
				chunk.MarkDirty();
			}
		}
	}
}
