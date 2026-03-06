using UnityEngine;
using Motorways.Managers;
using Motorways.Models;
using Motorways.Utils;
using Motorways.Views;

namespace Motorways.Actions {
	public class DrawRoadAction : MotorwaysPlayerAction {
		private Vector2Int _lastGridPointer;    //마지막에 연결된 혹은 시작 타일 좌표
		private Vector3 _clickOriginWorldPos;   //최초 클릭 시의 월드 좌표 (Deadzone 체크용)
		private bool _hasPassedDeadzone = false; //데드존을 넘었는지 여부
		private House _dragStartHouse = null;   //클릭 시작 시의 집 건물

		public override void OnActionBegin(float timestamp) {
			_lastGridPointer = _controller.CurrentGridPointer; //시작 위치 기록
			_clickOriginWorldPos = _controller.GetWorldPositionFromMouse();
			_hasPassedDeadzone = false;

			if (MapManager.Instance._grid.TryGetValue(_lastGridPointer, out TileData cell)) {
				if (cell.Building is House house) {
					_dragStartHouse = house;
					return; //집 건물을 클릭했으면 일단 건설 드래그는 대기
				} else if (cell.Building is Destination) {
					//목적지를 클릭했으면 건설 액션 자체를 취소
					OnActionCancel();
				}
			}
		}

		public override void Tick(float frameTime) {
			if (_isComplete) return;

			Vector3 currentMouseWorldPos = _controller.GetWorldPositionFromMouse();

			//단계1. 데드존 체크.
			if (!_hasPassedDeadzone) {
				//안넘었다면
				float distFromClick = Vector3.Distance(_clickOriginWorldPos, currentMouseWorldPos);

				//원형 데드존. 집 회전은 각도가 중요하나, 건설 드래그는 일단 이탈부터.
				if (distFromClick > _controller.InitialDragDeadzone) {
					//최초 클릭 지점에서 일정 거리를 넘어갔을때, 
					//단계2로 넘어감. (타일 중심에서 거리 계산).
					_hasPassedDeadzone = true;
				} else {
					//데드존 이내면 클릭으로 인식 (단계0 = 클릭모드)
					return;
				}
			}

			//단계2. 데드존을 벗어났으면
			//타일 중심에서 마우스가 어느 방향으로 넘어가는지 체크!
			if (_dragStartHouse != null) {
				ProcessHouseRotation(currentMouseWorldPos);
			} else {
				ProcessBuildDrag(currentMouseWorldPos);
				if (_controller.IsPointerValid) {
					Vector3 lastCenter = new Vector3(_lastGridPointer.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, _lastGridPointer.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
					Vector2Int snappedDir = _controller.CalculateSnappedDirection(currentMouseWorldPos - lastCenter);
					RoadPreviewView.Instance?.UpdatePreview(lastCenter, currentMouseWorldPos, snappedDir);
				}
			}
		}

		public override void OnActionComplete() {
			base.OnActionComplete();
			RoadPreviewView.Instance?.Hide();
		}

		public override void OnActionCancel() {
			base.OnActionCancel();
			RoadPreviewView.Instance?.Hide();
		}

		//---------- 건설(드래그) => 핵심 로직.
		private void ProcessBuildDrag(Vector3 mousePos) {
			//혹시 리소스 부족 처리.
			if (!ResourceManager.Instance.HasResource(ItemType.Road)) return;

			Vector3 lastTileCenter = new Vector3(_lastGridPointer.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, _lastGridPointer.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
			Vector3 diff = mousePos - lastTileCenter;
			float squareDist = Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.z));

			//단계4. 일정 거리 이상 드래그 되었을 경우.
			if (squareDist >= _controller.ConnectionDistanceThreshold) {
				//스냅핑된 방향을 구해서 시도합니다.
				Vector2Int offset = _controller.CalculateSnappedDirection(diff);
				Vector2Int candidatePos = _lastGridPointer + offset;

				if (!MapManager.Instance.IsInPlayableArea(candidatePos)) return;

				if (Vector2Int.Distance(_lastGridPointer, candidatePos) <= 1.5f) {
					if (MapManager.Instance._grid.TryGetValue(candidatePos, out TileData candidateTile)) {
						if (candidateTile.Building != null) {
							if (candidateTile.Building is Destination) {
								//드래그 도중 목적지를 만나면 건설 종료 처리
								OnActionComplete();
								return;
							} else if (candidateTile.Building is House targetHouse) {
								//외부 도로로부터 집으로 들어오는 길
								//집(candidatePos) 입장에서 이전 타일(도로, _lastGridPointer)을 바라보는 방향으로 회전
								Vector2Int dirToRoad = _lastGridPointer - candidatePos;
								TileDirection newDir = TileUtils.GetDirection(Vector2Int.zero, dirToRoad);

								if (newDir != TileDirection.None) {
									targetHouse.RotateEntrance(newDir);
								}

								_dragStartHouse = targetHouse;
								_lastGridPointer = candidatePos;
								_clickOriginWorldPos = new Vector3(candidatePos.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, candidatePos.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
								return;
							}
						}
					}
					//건물이 없거나 빈 공간이면 도로 건설 시도.
					RoadNetworkManager.Instance.TryBuildRoad(_lastGridPointer, candidatePos);
					_lastGridPointer = candidatePos;
				}
			}
		}

		//집 근처일때 동작. 드래그해서 회전을 시킴.
		private void ProcessHouseRotation(Vector3 mousePos) {
			Vector3 diff = mousePos - _clickOriginWorldPos;
			float squareDist = Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.z));

			if (squareDist >= _controller.ConnectionDistanceThreshold) {
				Vector2Int offset = _controller.CalculateSnappedDirection(diff);
				TileDirection newDir = TileUtils.GetDirection(Vector2Int.zero, offset);
				if (newDir != TileDirection.None) {
					_dragStartHouse.RotateEntrance(newDir);
					_lastGridPointer = _dragStartHouse.RoadCoordinate;
					_dragStartHouse = null;
				}
			}
		}
	}
}
