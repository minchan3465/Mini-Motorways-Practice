using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;

	public class RoadVisualizer : MonoBehaviour {
		public static RoadVisualizer Instance = null;

		[Header("Settings")]
		[SerializeField] private Material _roadMaterial;
		[SerializeField] private Transform _meshContainer;

		private Dictionary<Vector2Int, GameObject> _spawnedRoads = new Dictionary<Vector2Int, GameObject>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			if (_meshContainer == null) _meshContainer = transform;
		}

		public void UpdateRoadVisual(Vector2Int coord, CellData data) {
			if (_spawnedRoads.ContainsKey(coord)) {
				Destroy(_spawnedRoads[coord]);
				_spawnedRoads.Remove(coord);
			}

			if (data.Type != TileLogicType.Road) return;

			GameObject go = new GameObject($"Road_{coord}");
			go.transform.SetParent(_meshContainer);
			go.transform.position = new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f); //타일 중앙 위치

			MeshFilter mf = go.AddComponent<MeshFilter>();
			MeshRenderer mr = go.AddComponent<MeshRenderer>();
			mr.material = _roadMaterial;

			Vector3 start = new Vector3(0, 0, -0.5f);
			Vector3 end = new Vector3(0, 0, 0.5f);

			mf.mesh = RoadMeshGenerator.GenerateStraightMesh(start, end);

			_spawnedRoads.Add(coord, go);
		}

		public void RemoveRoadVisual(Vector2Int coord) {
			if(_spawnedRoads.ContainsKey(coord)) {
				Destroy(_spawnedRoads[coord]);
				_spawnedRoads.Remove(coord);
			}
		}
	}
}
