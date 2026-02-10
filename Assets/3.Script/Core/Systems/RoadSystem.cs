using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;
	using Core.Utils;
	using Core.Structure;
	using Core.Managers;

	//TODO : 기능화로 바꾸기 (형태 변경 x 상태로 만드는게 제일 BEST.)

	public class RoadSystem : MonoBehaviour {
		public static RoadSystem Instance = null;
		public int LatestMapUpdateFrame { get; private set; }

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		//--- 도로 상호작용 ---
		public void ConnectRoads(Vector2Int from, Vector2Int to) {
			///sqrMagnitude가 distance보다 훨~씬 빠름.
			///if ((from - to).sqrMagnitude > 2.25f) return;
			///근데 정확한 거리가 필요하면, Distance로 하는게 좋다네요. (어짜피 sqrMagninute와 다른점은 제곱근이냐 아니냐)
			if ((from - to).sqrMagnitude > 2.25f) return;
			//if (Vector2Int.Distance(from, to) > 1.5f) return;

			// 데이터 준비 (없으면 생성)
			if (!EnsureRoadNode(from) || !EnsureRoadNode(to)) return;

			// 건설 불가 지역 체크 (장애물 등)
			if (!IsRoadBuildable(from) || !IsRoadBuildable(to)) return;

			// 실제 연결 수행
			CreateConnection(from, to);
		}
		public void RemoveRoad(Vector2Int coord) {
			// CellData가 아니라, '실제 존재하는 도로(Lane)'를 기준으로 삭제를 판단합니다.
			// Lane 방식의 장점: 타일 상태를 일일이 확인 안 해도, 연결된 Lane만 끄집어내면 됨.

			if (RoadNetwork.Instance == null) return;

			// 해당 좌표에 연결된 모든 Lane을 가져옵니다.
			// 리스트를 복사해서 가져와야 순회 중 삭제(Modify) 오류가 안 납니다.
			List<Lane> connectedLanes = GetAllLanesAt(coord);

			foreach (var lane in connectedLanes) {
				// [핵심] 예약자 확인 (Lane에게 위임)
				// 예약자가 있으면 Mothballed, 없으면 즉시 파괴
				if (lane.VehiclesOnLane.Count > 0) {
					lane.State = LaneState.Mothballed;

					// 반대편 차선도 같이 Mothballed 처리 (시각적 동기화)
					Lane opposite = GetLane(lane.EndNode, lane.StartNode);
					if (opposite != null) opposite.State = LaneState.Mothballed;
				} else {
					// 예약 없으면 즉시 파괴 (연결 끊기)
					RemoveConnection(lane.StartNode, lane.EndNode);
				}
			}

			LatestMapUpdateFrame = Time.frameCount;
		}


		//--- 도로 건설 & 삭제 ---
		private void CreateConnection(Vector2Int from, Vector2Int to) {
			Vector2Int dirVec = to - from;
			RoadDirection dirToTarget = DirUtiles.GetDirectionFromVector(dirVec);
			RoadDirection dirToOrigin = DirUtiles.GetDirectionFromVector(-dirVec);

			CellData fromData = MapBootstrapper.Grid[from];
			CellData toData = MapBootstrapper.Grid[to];

			bool alreadyConnected = fromData.HasConnection(dirToTarget) && toData.HasConnection(dirToOrigin);

			if (alreadyConnected) {
				// [부활] 이미 연결됨 -> Mothballed 였던 Lane을 다시 Active로 복구
				RestoreLaneState(from, to);
				RestoreLaneState(to, from);
			} else {
				// [신규] 자원 소모 및 생성
				if (!ResourceManager.Instance.TryConsumeResource(ItemType.Road)) return;

				// 마스크 갱신
				fromData.ConnectionMask |= dirToTarget;
				toData.ConnectionMask |= dirToOrigin;

				// Lane 생성
				CreateLaneObject(from, to);
				CreateLaneObject(to, from);
			}

			// 구조체 재할당
			MapBootstrapper.Grid[from] = fromData;
			MapBootstrapper.Grid[to] = toData;

			LatestMapUpdateFrame = Time.frameCount;
		}
		private void RemoveConnection(Vector2Int from, Vector2Int to) {
			// 1. 데이터 검증
			if (!MapBootstrapper.Grid.TryGetValue(from, out CellData fromData)) return;
			if (!MapBootstrapper.Grid.TryGetValue(to, out CellData toData)) return;

			Vector2Int dirVec = to - from;
			RoadDirection dirToTarget = DirUtiles.GetDirectionFromVector(dirVec);
			RoadDirection dirToOrigin = DirUtiles.GetDirectionFromVector(-dirVec);

			// 연결이 없으면 중단
			if (!fromData.HasConnection(dirToTarget)) return;

			// 2. 자원 반환
			ResourceManager.Instance.AddResource(ItemType.Road, 1);

			// 3. Lane 객체 파괴 (메모리 및 RoadNetwork에서 제거)
			DestroyLaneObject(from, to);
			DestroyLaneObject(to, from);

			// 4. 마스크 해제
			fromData.ConnectionMask &= ~dirToTarget;
			toData.ConnectionMask &= ~dirToOrigin;

			MapBootstrapper.Grid[from] = fromData;
			MapBootstrapper.Grid[to] = toData;

			LatestMapUpdateFrame = Time.frameCount;

			// 5. [중요] 고립된 타일 정리 (양쪽 모두 체크)
			CheckAndCleanupIsolatedTile(from);
			CheckAndCleanupIsolatedTile(to);
		}


		//--- 차량에 의한 삭제 트리거 ---
		public void CheckAndProcessMothballedLane(Lane lane) {
			if (lane.State == LaneState.Mothballed && lane.VehiclesOnLane.Count == 0) {
				Lane opposite = GetLane(lane.EndNode, lane.StartNode);
				bool oppositeEmpty = (opposite == null) || (opposite.VehiclesOnLane.Count == 0);

				if (oppositeEmpty) {
					RemoveConnection(lane.StartNode, lane.EndNode);
				}
			}
		}

		//Lance 관련 관리
		private void CreateLaneObject(Vector2Int start, Vector2Int end) {
			if (RoadNetwork.Instance == null) return;
			Vector3 startPos = new Vector3(start.x + 0.5f, 0, start.y + 0.5f);
			Vector3 endPos = new Vector3(end.x + 0.5f, 0, end.y + 0.5f);

			Lane newLane = new Lane(start, end, startPos, endPos);
			RoadNetwork.Instance.RegisterLane(newLane);
		}
		private void DestroyLaneObject(Vector2Int start, Vector2Int end) {
			if (RoadNetwork.Instance == null) return;
			// RoadNetwork에서 찾아내서 삭제
			Lane target = GetLane(start, end);
			if (target != null) {
				RoadNetwork.Instance.UnRegisterLane(target);
			}
		}
		private void RestoreLaneState(Vector2Int start, Vector2Int end) {
			Lane target = GetLane(start, end);
			if (target != null) target.State = LaneState.Active;
		}

		//--- 유틸 ---
		private Lane GetLane(Vector2Int start, Vector2Int end) {
			if (RoadNetwork.Instance == null) return null;
			var lanes = RoadNetwork.Instance.GetOutboundLanes(start);
			if (lanes == null) return null;
			return lanes.Find(l => l.EndNode == end);
		}
		private List<Lane> GetAllLanesAt(Vector2Int coord) {
			if (RoadNetwork.Instance == null) return new List<Lane>();
			var origin = RoadNetwork.Instance.GetOutboundLanes(coord);
			if (origin == null) return new List<Lane>();
			return new List<Lane>(origin); // 복사본 반환
		}
		private bool EnsureRoadNode(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if (data.Type == TileLogicType.Empty) {
					data.Type = TileLogicType.Road;
					MapBootstrapper.Grid[coord] = data;
				}
				return data.IsDriveable;
			} else {
				CellData newRoad = new CellData(coord);
				newRoad.Type = TileLogicType.Road;
				MapBootstrapper.Grid.Add(coord, newRoad);
				return true;
			}
		}
		public bool IsRoadBuildable(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				//return data.IsDriveable || data.IsFullyEmpty; <<?
			}
			return true;
		}

		private void CheckAndCleanupIsolatedTile(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if (data.Type != TileLogicType.Road) return;

				// 연결된 곳이 하나도 없다면 (ConnectionMask가 None이면)
				if (data.ConnectionMask == RoadDirection.None) {
					data.Type = TileLogicType.Empty;
					// CellData는 값 타입(struct)이 아닐 수 있지만(Class로 선언됨), 
					// 안전하게 다시 할당
					MapBootstrapper.Grid[coord] = data;
				}
			}
		}
	}
}
