using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RoadView : MonoBehaviour {
		[SerializeField] private MeshFilter _meshFilter;
		[SerializeField] private MeshRenderer _meshRenderer;

		private void Awake() {
			TryGetComponent(out _meshFilter);
			TryGetComponent(out _meshRenderer);
		}

		public void Initialize(RoadTileDefinition definition, RoadSignature signature, Material material) {
			if (definition == null || definition.MainMesh == null) {
				// 정의가 없으면 비활성화 (또는 에러 처리)
				gameObject.SetActive(false);
				return;
			}

			//메쉬 할당. SharedMesh로 메모리 절약.
			_meshFilter.sharedMesh = definition.MainMesh;
			_meshRenderer.sharedMaterial = material;

			//2. 위치 설정 (중심점 기준)
			//주의: 호출하는 쪽(Manager)에서 transform.position을 설정해주는 것이 더 깔끔하지만,
			//기존 코드 유지를 위해 여기서 로컬 0,0,0으로 둡니다.
			transform.localPosition = Vector3.zero;

			//3. 회전 설정 (핵심)
			//아틀라스의 메쉬는 Canonical(정규) 방향이므로, 
			//시그니처 분석기가 계산해준 '원래대로 되돌리기 위한 회전값(RotationSteps)'을 적용해야 합니다.
			float angle = signature.RotationSteps * 45f; // 90도 단위일 수도 있음. 로직에 따라 확인 필요.
			transform.localRotation = Quaternion.Euler(0, angle, 0);

			gameObject.SetActive(true);
		}

		public void Deactive() {
			gameObject.SetActive(false);
		}
	}
}

