using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;
	using Core.Systems.Structure;

	public class StructureManager : MonoBehaviour {
		public static StructureManager Instance = null;

		[Header("Prefabs")]
		[SerializeField] private GameObject _housePrefab;
		[SerializeField] private GameObject _destinationPrefab;
		[SerializeField] private Transform _structureContainer;

		private List<House> _houses = new List<House>();
		private List<Destination> _destinations = new List<Destination>();



		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
			if (_structureContainer == null) _structureContainer = transform;
		}

		private void Start() {
			SpawnHouse();
			SpawnHouse();
			SpawnDestination();
		}

		//----------------------- 집 스폰
		public void SpawnHouse() {
			if (TryFindValidPosition(1, 1, out Vector2Int spawnPos, out RoadDirection validDir)) {
				Vector3 worldPos = new Vector3(spawnPos.x + 0.5f, 0, spawnPos.y + 0.5f);
				GameObject go = Instantiate(_housePrefab, worldPos, Quaternion.identity, _structureContainer);

				if (go.TryGetComponent(out House house)) {
					house.Initialize(spawnPos, validDir);

					UpdateGridType(spawnPos, TileLogicType.Supply);
					UpdateGridType(house.EntranceCoordinate, TileLogicType.Entrance);

					RoadSystem.Instance.CreateRoadNode(house.EntranceCoordinate);

					RegisterHouse(house);
				}
				//Debug.Log($"House Spawned at {spawnPos}");
			} else {
				Debug.LogWarning($"집 지을 공간이 없습니다.");
			}

		}
		public void RegisterHouse(House h) => _houses.Add(h);

		//----------------------- 목적지 스폰
		public void SpawnDestination() {
			bool isHorizontal = Random.value > 0.5F;
			int w = isHorizontal ? 3 : 2;
			int h = isHorizontal ? 2 : 3;

			if (TryFindValidPosition(w, h, out Vector2Int spawnPos, out RoadDirection _)) {
				Vector3 worldPos = new Vector3(spawnPos.x + (w * 0.5f), 0, spawnPos.y + (h * 0.5f));
				GameObject go = Instantiate(_destinationPrefab, worldPos, Quaternion.identity, _structureContainer);

				if (go.TryGetComponent(out Destination dest)) {
					dest.SetupDestination(spawnPos, isHorizontal);

					for (int x = 0; x < w; x++) {
						for (int y = 0; y < h; y++) {
							Vector2Int currentTile = spawnPos + new Vector2Int(x, y);
							UpdateGridType(currentTile, TileLogicType.Demand);
						}
					}

					UpdateGridType(dest.EntranceCoordinate, TileLogicType.Entrance);
					RoadSystem.Instance.CreateRoadNode(dest.EntranceCoordinate);

					RegisterDestination(dest);
				}
				//Debug.Log($"Destination Spawned at {spawnPos} (Size : {w} x {h}");
			} else {
				Debug.LogWarning($"목적지를 지을 공간이 없습니다.");
			}
		}
		public void RegisterDestination(Destination d) => _destinations.Add(d);

		//----------------------- 집 <-> 목적지 매칭 시스템
		public void OnPinCreated(Destination dest) => CheckPendingRequests(); // 핀 생기면 체크
		public void OnCarAvailable(House house) => CheckPendingRequests();    // 차 돌아오면 체크 (Fix 3)

		public void CheckPendingRequests() {
			int pinsFound = 0;
			foreach (var dest in _destinations) {
				if(dest.HasUnassignedPin()) {
					pinsFound++;
					TryDispatchCarTo(dest);
				}
			}
		}

		//특정 목적지로 차를 보냄 (가장 가까운 집 검색)
		private void TryDispatchCarTo(Destination targetDest) {
			if (!targetDest.HasUnassignedPin()) return;

			House bestHouse = null;
			List<Vector2Int> bestPath = null;
			int minPathLength = int.MaxValue;

			foreach (var house in _houses) {
				if (!house.HasAvailableCar()) continue;

				List<Vector2Int> path = Pathfinder.FindPath(house.EntranceCoordinate, targetDest.EntranceCoordinate);

				if (path != null) {
					if (path.Count < minPathLength) {
						minPathLength = path.Count;
						bestPath = path;
						bestHouse = house;
					}
				}

			}
			if (bestHouse != null) {
				// 목적지 핀 개수를 여기서 바로 줄이면 안 됨 (차가 도착해야 줄어듦)
				// 하지만 '처리 중인 핀'으로 간주하지 않으면 매 프레임 차를 계속 보냄.
				// 따라서 Destination에 'IncomingCars' 개념이 필요하지만,
				// 지금은 간단히 Destination의 핀을 임시로 하나 줄이는 효과를 냄?
				// -> 아니요, 구조적으로 Destination에 RequestQueue를 만드는 게 정석이지만,
				// -> 지금은 로그만 찍고, 핀 감소는 차 도착 시에만 하겠습니다.
				// -> (단, 무한 출동을 막기 위해 쿨타임이나 플래그가 필요함. 일단 보류)

				bestHouse.DispatchCar(bestPath, targetDest);
				targetDest.RegisterIncomingCar();
				//Debug.Log($" 출동: {bestHouse.name} -> {targetDest.name} (거리: {minPathLength})");

				// [임시 해결책] 무한 출동 방지를 위해, 
				// Destination 로직을 수정해서 '할당된 핀' 개념을 넣어야 합니다.
				// 여기서는 일단 출동 성공했으니 true 반환.
			}
		}
		//OnCarAvailable 전용 특정 집에서 특정 목적지로 지정해주는 메서드
		private bool TryDisPatchToSpecific(House house, Destination dest) {
			List<Vector2Int> path = Pathfinder.FindPath(house.EntranceCoordinate, dest.EntranceCoordinate);

			if(path != null) {
				house.DispatchCar(path, dest);
				dest.RegisterIncomingCar();
				return true;
			}
			return false;
		}

		//----------------------- 공간 탐색 알고리즘~~~ 타일 관련된 메서드들 모음.
		private bool TryFindValidPosition(int width, int height, out Vector2Int bestPos, out RoadDirection bestDir) {
			//간단한 구현: 맵 전체를 랜덤하게 몇번 해봅니다.
			//실제로는 Buffer Zone으로, 이격거리를 줘야한다고 함.
			//...그래서 실제 게임에서 건물이 개같이 나온거구나...

			bestPos = Vector2Int.zero;
			bestDir = RoadDirection.North;

			for (int i = 0; i < 50; i++) {  //50번 시도해서 안되면 그냥 부족한거나 마찬가지 ㅠ
											//맵 범위 내 랜덤 좌표를 가져옵니다.

				//맵 범위는 이미 잡은게 있으니 훔쳐오기 ㅋ
				BoundsInt bounds = MapBootstrapper.MapBounds;
				int x = Random.Range(bounds.xMin + 2, bounds.xMax - width - 2);
				int y = Random.Range(bounds.yMin + 2, bounds.yMax - width - 2);
				Vector2Int candidate = new Vector2Int(x, y);

				if (CheckAreaEmpty(candidate, width, height)) {
					//선택된 공간이 비어있는 곳인가?
					if (CheckEntranceSpace(candidate, width, height, out bestDir)) {
						//주변에 입구를 낼 수 있는 공간까지 이제 체크해야함.
						bestPos = candidate;
						return true;
					}
				}
			}
			return false;
		}
		private bool CheckAreaEmpty(Vector2Int root, int w, int h) {
			for (int x = 0; x < w; x++) {
				for (int y = 0; y < h; y++) {
					Vector2Int p = root + new Vector2Int(x, y);
					if (MapBootstrapper.Grid.TryGetValue(p, out CellData data)) {
						//만약 해당 위치가 비어있는게 아니라면.
						if (!data.Type.Equals(TileLogicType.Empty)) return false;
					} else {
						//이러면 맵 밖일 가능성이 높긴한데 데이터 없으면 Empty일텐데 말이죠.
						return false;
					}
				}
			}
			return true;
		}
		private bool CheckEntranceSpace(Vector2Int root, int w, int h, out RoadDirection validDir) {
			validDir = RoadDirection.North;

			Vector2Int northPos = root + new Vector2Int(0, h);
			Vector2Int southPos = root + new Vector2Int(0, -h);
			Vector2Int EastPos = root + new Vector2Int(w, 0);
			Vector2Int WestPos = root + new Vector2Int(-w, 0);

			if (IsValidEntrance(northPos)) {
				validDir = RoadDirection.North;
				return true;
			}
			if (IsValidEntrance(southPos)) {
				validDir = RoadDirection.South;
				return true;
			}
			if (IsValidEntrance(EastPos)) {
				validDir = RoadDirection.East;
				return true;
			}
			if (IsValidEntrance(WestPos)) {
				validDir = RoadDirection.West;
				return true;
			}

			//나중가면 false로 바꿔야함. (테스트용으로 true로 한거)
			return false;
		}

		public bool IsValidEntrance(Vector2Int pos) {
			if (MapBootstrapper.Grid.TryGetValue(pos, out CellData data)) {
				return data.Type.Equals(TileLogicType.Empty) || data.Type.Equals(TileLogicType.Road);
			}
			return false;
		}

		// 그리드 타입 업데이트 헬퍼
		public void UpdateGridType(Vector2Int pos, TileLogicType type) {
			if (MapBootstrapper.Grid.TryGetValue(pos, out CellData data)) {
				data.Type = type;
				MapBootstrapper.Grid[pos] = data;
			} else {
				CellData newData = new CellData {
					Coordinate = pos,
					Type = type,
					ConnectionMask = RoadDirection.None
				};
				MapBootstrapper.Grid.Add(pos, newData);
			}
		}
	}
}
