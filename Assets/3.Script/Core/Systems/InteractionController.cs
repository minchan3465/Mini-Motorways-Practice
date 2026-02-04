using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Systems {
	//using Core.Data;

	public class InteractionController : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private Camera _mainCamera;

		[SerializeField, Range(0.1f, 0.5f)] private float _dragSensitivity = 0.4f;

		private PlayerInput _input;
		private Vector2 _mouseScreenPos;
		private Plane _groundPlane;

		//마우스가 가리키고 있는 그리드 좌표
		public Vector2Int CurrentGridPointer { get; private set; }
		public bool IsPointerValid { get; private set; }

		private bool _isDraggingBuld = false;
		private bool _isDraggingRemove = false;
		private Vector2Int _lastGridPointer; // 마지막으로 도로를 깔았던 위치
		private Vector2Int _startDragPointer; // 처음으로 도로를 깔았던 위치

		//-----------------------------------------------

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
			_input.Player.Remove.started += OnRemoveStarted;
			_input.Player.Remove.canceled += OnRemoveCanceled;
		}
		private void OnDisable() {
			_input.Disable();
			_input.Player.CursorPosition.performed -= OnCursorMove;
			_input.Player.Build.started += OnBuildStarted;   // 눌렀을 때
			_input.Player.Build.canceled += OnBuildCanceled; // 뗐을 때
			_input.Player.Remove.started -= OnRemoveStarted;
			_input.Player.Remove.canceled -= OnRemoveCanceled;
		}


		//----------건설(좌클릭)
		private void OnBuildStarted(InputAction.CallbackContext context) {
			if (_isDraggingRemove) return;

			_isDraggingBuld = true;
			if (IsPointerValid) {
				if(!RoadSystem.Instance.IsRoadBuildable(CurrentGridPointer)) {
					_isDraggingBuld = false;
					return;
				}

				RoadSystem.Instance.CreateRoadNode(CurrentGridPointer);
				_lastGridPointer = CurrentGridPointer; // 현재 위치 기억
				_startDragPointer = CurrentGridPointer; // 시작점 기억
			}
		}
		private void OnBuildCanceled(InputAction.CallbackContext context) {
			if(_isDraggingBuld) {
				_isDraggingBuld = false;

				//도로 한개면... 삭제해야지?
				_isDraggingBuld = false;
				RoadSystem.Instance.CleanupifIsolated(_startDragPointer);

				//만약에 말야... 우리.. 에러날수도 있으니 점검용
				if(_startDragPointer != _lastGridPointer) {
					RoadSystem.Instance.CleanupifIsolated(_lastGridPointer);
				}
			}
		}

		//----------삭제(우클릭)
		private void OnRemoveStarted(InputAction.CallbackContext context) {
			if (_isDraggingBuld) return;
			_isDraggingRemove = true;

			if(IsPointerValid) {
				RoadSystem.Instance.RemoveRoad(CurrentGridPointer);
				_lastGridPointer = CurrentGridPointer;
			}
		}
		private void OnRemoveCanceled(InputAction.CallbackContext context) {
			_isDraggingRemove = false;
		}

		//---------- 조작(마우스 이동)
		private void OnCursorMove(InputAction.CallbackContext context) {
			_mouseScreenPos = context.ReadValue<Vector2>();
			CalculateGridPosition();

			if (_isDraggingBuld && IsPointerValid) {
				if (CurrentGridPointer != _lastGridPointer) {
					if (!RoadSystem.Instance.IsRoadBuildable(CurrentGridPointer)) {
						//건설할 좌표가 도로를 지을 수 없는 곳이면.
						//드래그와 건설 즉시 취소.
						_isDraggingBuld = false;

						//끊긴 지점 정리?
						RoadSystem.Instance.CleanupifIsolated(_lastGridPointer);
						return;
					}
					RoadSystem.Instance.ConnectRoads(_lastGridPointer, CurrentGridPointer);
					_lastGridPointer = CurrentGridPointer; // 위치 갱신
				}
			} else if (_isDraggingRemove && IsPointerValid) {
				if(CurrentGridPointer != _lastGridPointer) {
					RoadSystem.Instance.RemoveRoad(CurrentGridPointer);
					_lastGridPointer = CurrentGridPointer;
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

			/*
			전에 배웠을 때, Physics.Raycast를 썼는데 왜 이번엔 Plane.Raycast?
			Physics는 Mesh와 Collider를 구분하고, 좌표와 법선 벡터, 충돌한 Collider를 반환.
			Plane은 수학적 계산 기반으로, 물리엔진을 사용하지 않아서 가벼움!
			또한 내적을 사용하여, Ray의 거리만 반환하기 때문에 괜찮다이 또한  
			 */
			if (_groundPlane.Raycast(ray, out float enter)) {
				Vector3 hitPoint = ray.GetPoint(enter);

				//Floor = 소숫점 버리기를 통하여 int로 바꿔서 정확한 그리드 좌표를 찾는다.
				//반올림 개념보단, 가우스같은 느낌이면 될듯.
				int x = Mathf.FloorToInt(hitPoint.x);
				int y = Mathf.FloorToInt(hitPoint.z);
				Vector2Int candidateGridPos = new Vector2Int(x, y);

				//CurrentGridPointer = new Vector2Int(x, y);
				if(_isDraggingBuld) {
					Vector3 tileCenter = new Vector3(candidateGridPos.x + 0.5f, 0, candidateGridPos.y + 0.5f);

					float dist = (tileCenter - new Vector3(hitPoint.x, 0, hitPoint.z)).sqrMagnitude;

					//Distance가 아닌, sqrMagnitude를 사용했기 때문에, 제곱을 해줘야함...
					if(dist<= _dragSensitivity * _dragSensitivity) {
						CurrentGridPointer = candidateGridPos;
					}
				} else {
					CurrentGridPointer = candidateGridPos;
				}
				IsPointerValid = true;
			} else {
				IsPointerValid = false;	
			}
		}

		private void OnDrawGizmos() {
			if(Application.isPlaying && IsPointerValid) {
				Gizmos.color = Color.cyan;
				Vector3 drawPos = new Vector3(CurrentGridPointer.x + 0.5f, 0, CurrentGridPointer.y + 0.5f);
				Gizmos.DrawWireCube(drawPos, Vector3.one * 0.95f);

				if (_isDraggingBuld || _isDraggingRemove) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawWireSphere(drawPos, _dragSensitivity);
				}
			}
		}
	}
}
