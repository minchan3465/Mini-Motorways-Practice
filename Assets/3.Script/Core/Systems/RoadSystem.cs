using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;
	using Core.Utils;
	using Core.Managers;

	//TODO : 기능화로 바꾸기 (형태 변경 x 상태로 만드는게 제일 BEST.)

	public class RoadSystem : MonoBehaviour {
		public static RoadSystem Instance = null;
		public int LatestMapUpdateFrame { get; private set; }

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		//--- 도로 연결 관련 메서드 ---
		private void CreateConnection(Vector2Int from, Vector2Int to) {
			Vector2Int dirVec = to - from;
			RoadDirection dirToTarget = DirUtiles.GetDirectionFromVector(dirVec);
			RoadDirection dirToOrigin = DirUtiles.GetDirectionFromVector(-dirVec);

			CellData fromData = MapBootstrapper.Grid[from];
			CellData toData = MapBootstrapper.Grid[to];

			bool alreadyConnected = fromData.HasConnection(dirToTarget) && toData.HasConnection(dirToOrigin);

			if (alreadyConnected) {
				//삭제 대기중이였다면 다시 삭제 취소
				fromData.MothballedMask &= ~dirToTarget;
				toData.MothballedMask &= ~dirToOrigin;
			} else {
				if (!ResourceManager.Instance.TryConsumeResource(ItemType.Road)) return;
				//연결.
				fromData.ConnectionMask |= dirToTarget;
				toData.ConnectionMask |= dirToOrigin;

				//안전장치.
				fromData.MothballedMask &= ~dirToTarget;
				toData.MothballedMask &= ~dirToOrigin;
			}

			//업데이트 알림.
			MapBootstrapper.Grid[from] = fromData;
			MapBootstrapper.Grid[to] = toData;
			LatestMapUpdateFrame = Time.frameCount;

			//TODO : 비주얼 갱신.
		}
		private void RemoveConnection(Vector2Int from, Vector2Int to) {
			if (!MapBootstrapper.Grid.ContainsKey(from) || !MapBootstrapper.Grid.ContainsKey(to)) return;

			CellData fromData = MapBootstrapper.Grid[from];
			CellData toData = MapBootstrapper.Grid[to];

			Vector2Int dirVec = to - from;
			RoadDirection dirToTarget = DirUtiles.GetDirectionFromVector(dirVec);
			RoadDirection dirToOrigin = DirUtiles.GetDirectionFromVector(-dirVec);

			//연결 없을수도 있으니, 그러면 종료.
			if (!fromData.HasConnection(dirToTarget)) return;
			//연결인거 확인했으니 도로 회수.
			ResourceManager.Instance.AddResource(ItemType.Road, 1);

			//연결 해제
			fromData.ConnectionMask &= ~dirToTarget;
			toData.ConnectionMask &= ~dirToOrigin;

			//삭제 대기 도로도 삭제.
			fromData.MothballedMask &= ~dirToTarget;
			toData.MothballedMask &= ~dirToOrigin;

			//업데이트
			MapBootstrapper.Grid[from] = fromData;
			MapBootstrapper.Grid[to] = toData;
			LatestMapUpdateFrame = Time.frameCount;

			//도로가 고립상태면 (연결된게 없으면)
			CheckAndCleanupIsolatedTile(from);
			CheckAndCleanupIsolatedTile(to);

			//TODO : 비주얼 갱신.
		}


		//--- 도로 건설 & 삭제---
		public void ConnectRoads(Vector2Int from, Vector2Int to) {
			///sqrMagnitude가 distance보다 훨~씬 빠름.
			///if ((from - to).sqrMagnitude > 2.25f) return;
			///근데 정확한 거리가 필요하면, Distance로 하는게 좋다네요. (어짜피 sqrMagninute와 다른점은 제곱근이냐 아니냐)
			if (Vector2Int.Distance(from, to) > 1.5f) return;
			if (!EnsureRoadNode(from) || !EnsureRoadNode(to)) return;   //타일 데이터 준비.
			if (!IsRoadBuildable(from) || !IsRoadBuildable(to)) return;

			//위의 조건들을 다 뚫었다면, 이제 연결합니다.
			CreateConnection(from, to);

			if (StructureManager.Instance != null) {
				StructureManager.Instance.CheckPendingRequests();
			}
		}
		public void RemoveRoad(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if (data.Type != TileLogicType.Road) return;

				if (data.ConnectionMask != RoadDirection.None) {
					data.MothballedMask |= data.ConnectionMask; //삭제할때 연결된 Mask 전부 삭제 대기로.
					MapBootstrapper.Grid[coord] = data;

					RoadDirection directionsToPrune = data.ModifyReservation(0);    //트리거용.
					if (directionsToPrune != RoadDirection.None) {
						//만약 연결 제거가 필요한 곳 이 있다면.
						ProcessPruning(coord, directionsToPrune);
					}
				}

				LatestMapUpdateFrame = Time.frameCount;
			}
		}


		//--- 참조 카운팅 (예약)---
		public void NotifyReservation(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				data.ModifyReservation(+1);
			}
		}
		public void NotifyRelease(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				RoadDirection directionsToPrune = data.ModifyReservation(-1);
				if (directionsToPrune != RoadDirection.None) {
					ProcessPruning(coord, directionsToPrune);
				}
			}
		}

		//--- 유틸 (내부 로직)

		//외부와 소통할 코드지만, 굳이 여기에 둬야할 메서드인가? MapBootstrapper에 옮겨도 될듯 합니다.
		public bool IsRoadBuildable(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				//도로거나 빈 땅여야 합니다.
				return data.IsDriveable || data.IsFullyEmpty;
			}
			return true;
			//맵밖은 가능이라고 일단 만듬. 근데 false로 나중에 바꿔줄수도 있음.
		}

		private bool EnsureRoadNode(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				//이미 존재함. Empty라면 Road로 타입만 변경
				if (data.Type == TileLogicType.Empty) {
					data.Type = TileLogicType.Road;
					MapBootstrapper.Grid[coord] = data;
				}
				//다른 건물이면 false, 도로면 true
				return data.IsDriveable;
			} else {
				//데이터 없음 -> 신규 생성
				CellData newRoad = new CellData(coord);
				newRoad.Type = TileLogicType.Road;
				MapBootstrapper.Grid.Add(coord, newRoad);
				return true;
			}
		}
		private void ProcessPruning(Vector2Int center, RoadDirection mask) {
			if ((mask & RoadDirection.North) != 0) RemoveConnection(center, center + Vector2Int.up);
			if ((mask & RoadDirection.South) != 0) RemoveConnection(center, center + Vector2Int.down);
			if ((mask & RoadDirection.East) != 0) RemoveConnection(center, center + Vector2Int.right);
			if ((mask & RoadDirection.West) != 0) RemoveConnection(center, center + Vector2Int.left);

			if ((mask & RoadDirection.NorthEast) != 0) RemoveConnection(center, center + new Vector2Int(1, 1));
			if ((mask & RoadDirection.NorthWest) != 0) RemoveConnection(center, center + new Vector2Int(-1, 1));
			if ((mask & RoadDirection.SouthEast) != 0) RemoveConnection(center, center + new Vector2Int(1, -1));
			if ((mask & RoadDirection.SouthWest) != 0) RemoveConnection(center, center + new Vector2Int(-1, -1));
		}
		private void CheckAndCleanupIsolatedTile(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if (data.Type != TileLogicType.Road) return;
				if (data.ConnectionMask == RoadDirection.None) {
					data.Type = TileLogicType.Empty;
					data.MothballedMask = RoadDirection.None;
					data.ReservationCount = 0;
					MapBootstrapper.Grid[coord] = data;

					//TODO : 비주얼 갱신(여긴 제거)
				}
			}
		}


		//--- 비주얼 연결 코드 ---
		private void RemoveVisualsAround(Vector2Int coord) {
			CheckAndRemoveVisual(coord, new Vector2Int(0, 1));
			CheckAndRemoveVisual(coord, new Vector2Int(0, -1));
			CheckAndRemoveVisual(coord, new Vector2Int(1, 0));
			CheckAndRemoveVisual(coord, new Vector2Int(-1, 0));
			CheckAndRemoveVisual(coord, new Vector2Int(1, 1));
			CheckAndRemoveVisual(coord, new Vector2Int(1, -1));
			CheckAndRemoveVisual(coord, new Vector2Int(-1, -1));
			CheckAndRemoveVisual(coord, new Vector2Int(-1, 1));
		}
		private void CheckAndRemoveVisual(Vector2Int center, Vector2Int offset) {
			//TODO : 나중에 할거
		}
	}
}
