using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Systems {
	using Core.Data;

	public class InteractionController : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private LayerMask _groundLayer;

		private PlayerInput _input;
		private Vector2 _mouseScreenPos;
		private Plane _groundPlane;

		private bool _isBuilding = false; // 현재 누르고 있는가?
		private Vector2Int _lastBuildCoord = new Vector2Int(-999, -999); // 중복 건설 방지용

		//마우스가 가리키고 있는 그리드 좌표
		public Vector2Int CurrentGridPointer { get; private set; }
		public bool IsPointerValid { get; private set; }

		private bool _isDragging = false;
		private Vector2Int _lastGridPointer; // 마지막으로 도로를 깔았던 위치

		private void Awake() {
			_input = new PlayerInput();
			_groundPlane = new Plane(Vector3.up, Vector3.zero);
			if (_mainCamera == null) _mainCamera = Camera.main;
		}

		private void OnEnable() {
			_input.Enable();
			_input.Player.CursorPosition.performed += OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;   // 눌렀을 때
			_input.Player.Build.canceled += OnBuildCanceled; // 뗐을 때
			//_input.Player.Build.performed += OnBuildPerformed;
		}
		private void OnDisable() {
			_input.Disable();
			_input.Player.CursorPosition.performed -= OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;   // 눌렀을 때
			_input.Player.Build.canceled += OnBuildCanceled; // 뗐을 때
			//_input.Player.Build.performed -= OnBuildPerformed;
		}




		private void OnBuildStarted(InputAction.CallbackContext context) {
			_isDragging = true;

			if (IsPointerValid) {
				TryPlaceRoad(CurrentGridPointer);
				_lastGridPointer = CurrentGridPointer; // 현재 위치 기억
			}
		}
		private void OnBuildCanceled(InputAction.CallbackContext context) {
			_isDragging = false;
		}

		private void OnCursorMove(InputAction.CallbackContext context) {
			_mouseScreenPos = context.ReadValue<Vector2>();
			CalculateGridPosition();

			if (_isDragging && IsPointerValid) {
				if (CurrentGridPointer != _lastGridPointer) {
					TryPlaceRoad(CurrentGridPointer);
					_lastGridPointer = CurrentGridPointer; // 위치 갱신
				}
			}
		}

		//private void OnBuildPerformed(InputAction.CallbackContext context) {
		//	if(IsPointerValid) {
		//		TryPlaceRoad(CurrentGridPointer);
		//	}
		//}

		private void CalculateGridPosition() {
			Ray ray = _mainCamera.ScreenPointToRay(_mouseScreenPos);

			//전에 배웠을 때, Physics.Raycast를 썼는데 왜 이번엔 Plane.Raycast?
			//Physics는 Mesh와 Collider를 구분하고, 좌표와 법선 벡터, 충돌한 Collider를 반환.
			//Plane은 수학적 계산 기반으로, 물리엔진을 사용하지 않아서 가벼움!
			//또한 내적을 사용하여, Ray의 거리만 반환하기 때문에 괜찮다이 또한 
			if (_groundPlane.Raycast(ray, out float enter)) {
				Vector3 hitPoint = ray.GetPoint(enter);

				//Floor = 소숫점 버리기를 통하여 int로 바꿔서 정확한 그리드 좌표를 찾는다.
				//반올림 개념보단, 가우스같은 느낌이면 될듯.
				int x = Mathf.FloorToInt(hitPoint.x);
				int y = Mathf.FloorToInt(hitPoint.z);

				CurrentGridPointer = new Vector2Int(x, y);
				IsPointerValid = true;
			} else {
				IsPointerValid = false;	
			}
		}

		private void TryPlaceRoad(Vector2Int coord) {
			//일단 데이터가 존재하는지 확인
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData existingCell)) {
				//이미 건물이거나 장애물이면 건설 불가
				if (existingCell.Type != TileLogicType.Empty && existingCell.Type != TileLogicType.Road) {
					Debug.Log($"건설 불가: 이곳엔 {existingCell.Type}이(가) 있습니다.");
					return;
				}
			}

			//도로 없제? 만든다 ㅇㅇ...
			if(!MapBootstrapper.Grid.ContainsKey(coord)) {
				//데이터가 없으면
				CellData newRoad = new CellData {
					Coordinate = coord,
					Type = TileLogicType.Road,
					Weight = 0,
					ConnectionMask = 0
				};
				MapBootstrapper.Grid.Add(coord, newRoad);
			} else {
				//데이터가 있으면 (Empty 타일이겠죠?)
				CellData data = MapBootstrapper.Grid[coord];
				data.Type = TileLogicType.Road;
				MapBootstrapper.Grid[coord] = data;
			}

			Debug.Log($"[Doro] 도로 건설 완료: {coord}");
		}

		private void OnDrawGizmos() {
			if(Application.isPlaying && IsPointerValid) {
				Gizmos.color = Color.cyan;
				Vector3 drawPos = new Vector3(CurrentGridPointer.x + 0.5f, 0, CurrentGridPointer.y + 0.5f);
				Gizmos.DrawWireCube(drawPos, Vector3.one * 0.95f);
			}
		}
	}
}
