using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	public class RoadView : MonoBehaviour {
		[SerializeField] private MeshFilter _meshFilter;
		[SerializeField] private MeshRenderer _meshRenderer;

		private void Awake() {
			_meshFilter = GetComponent<MeshFilter>();
			if (_meshFilter == null) _meshFilter = gameObject.AddComponent<MeshFilter>();

			_meshRenderer = GetComponent<MeshRenderer>();
			if (_meshRenderer == null) _meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}

		public void Initialize(Mesh mesh, Material material, Vector2Int coord, int rotationSteps) {
			if (_meshFilter == null) Awake();

			//메쉬 할당. SharedMesh로 메모리 절약.
			_meshFilter.sharedMesh = mesh;
			_meshRenderer.sharedMaterial = material;

			//위치 설정.
			transform.position = new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f);

			//회전 설정.
			transform.rotation = Quaternion.Euler(0, rotationSteps * 45f, 0);

			gameObject.SetActive(true);
		}

		public void Deactive() {
			gameObject.SetActive(false);
		}
	}
}

