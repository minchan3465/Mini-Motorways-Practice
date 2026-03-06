using UnityEngine;
using Motorways.Managers;

namespace Motorways.Views {
	public class GridView : MonoBehaviour {
		public static GridView Instance;

		[Header("References")]
		[SerializeField] 
		private MeshRenderer _gridRenderer;

		[Header("Transition Settings")]
		[SerializeField] 
		private float _transitionDuration = 0.1f; // 0.15~0.2초 권장 (두께가 자라나는 시간)

		private readonly Color _buildColor = new Color(0.933f, 0.871f, 0.769f, 1.0f);    // 0.6f, 0.75f, 0.8f 강제 색상 변경
		private readonly Color _removeColor = new Color(1.0f, 0.35f, 0.35f, 1.0f);

		private readonly Color _buildBgColor = new Color(0, 0, 0, 0.05f); // 건설 시 거의 투명
		private readonly Color _removeBgColor = new Color(0, 0, 0, 0.3f);  // 삭제 시 반투명 블랙 (어두워짐)

		private Material _gridMaterial;
		private float _currentThickness = 0f;
		private float _targetThickness = 0f;
		private float _velocity = 0f; // SmoothDamp 가속도 계산용

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			if (_gridRenderer != null) {
				// 머티리얼 인스턴스 참조 및 초기화
				_gridRenderer.sortingLayerName = "Grid";
				_gridRenderer.sortingOrder = -100;
				_gridMaterial = _gridRenderer.material;
				_gridMaterial.SetFloat("_Thickness", 0f);
				_gridRenderer.gameObject.SetActive(false);
			}
		}

		private void Update() {
			// 원작의 애니메이션 연출 (두께가 0.0에서 1.0으로 자라남)
			if (!Mathf.Approximately(_currentThickness, _targetThickness)) {
				// SmoothDamp를 사용하여 자연스러운 가감속 연출 (원작의 SineEaseInOut과 유사)
				_currentThickness = Mathf.SmoothDamp(_currentThickness, _targetThickness, ref _velocity, _transitionDuration);

				// 셰이더 프로퍼티 업데이트
				if (_gridMaterial != null) {
					_gridMaterial.SetFloat("_Thickness", _currentThickness);
				}

				// 완전히 사라지면 비활성화하여 드로우콜 최적화
				if (_currentThickness < 0.005f && _targetThickness == 0f) {
					_currentThickness = 0f;
					_gridRenderer.gameObject.SetActive(false);
				}
			}
		}

		// 모드에 따라 색상을 설정하며 활성화
		public void SetVisible(bool visible, bool isRemoval = false) {
			_targetThickness = visible ? 1f : 0f;
			if (visible) {
				_gridRenderer.gameObject.SetActive(true);
				_gridMaterial.SetColor("_Color", isRemoval ? _removeColor : _buildColor);
				_gridMaterial.SetColor("_BackgroundColor", isRemoval ? _removeBgColor : _buildBgColor);
				// 켜지는 순간 현재 맵의 설치 가능 크기를 셰이더로 전송
				SyncPlayableArea();
			}
		}

		private void SyncPlayableArea() {
			if (MapManager.Instance == null || _gridMaterial == null) return;

			RectInt area = MapManager.Instance.PlayableArea;

			// 1. 원작 방식의 구역 데이터 전달 (minX, minY, maxX, maxY)
			// 셰이더 내부의 좌표 판정을 위해 월드 좌표 스케일 적용
			Vector4 bounds = new Vector4(area.xMin, area.yMin, area.xMax, area.yMax) * MapSettings.TILE_SIZE;
			_gridMaterial.SetVector("_PLAYABLE_AREA", bounds);

			// 최하단 배치를 위해 Y값을 낮게 설정
			Vector3 center = new Vector3((area.xMin + area.xMax) * MapSettings.TILE_SIZE * 0.5f, -0.01f, (area.yMin + area.yMax) * MapSettings.TILE_SIZE * 0.5f);
			_gridRenderer.transform.position = center;

			// 스케일 불균형을 막기 위해 가로/세로를 동일하게 크게 설정
			_gridRenderer.transform.localScale = new Vector3(20 * MapSettings.TILE_SIZE, 1, 20 * MapSettings.TILE_SIZE);
		}
	}
}
