using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;
	using Core.Utils;
	using Core.Systems.Structure;

	public class StructureManager : MonoBehaviour {
		public static StructureManager Instance = null;

		[Header("Prefabs")]
		[SerializeField] private GameObject _housePrefab;
		[SerializeField] private GameObject _destinationPrefab;
		[SerializeField] private Transform _structureContainer;

		private List<House> _houses = new List<House>();
		private List<Destination> _destinations = new List<Destination>();

		private List<CarMovement> _allCars = new List<CarMovement>();
		public List<CarMovement> AllCars => _allCars;

		public int HouseCount => _houses.Count;
		public int DestinationCount => _destinations.Count;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
			if (_structureContainer == null) _structureContainer = transform;
		}

		//차량 관련한걸 StructureManager에서 하는게 좀 마음에 안들긴 하지만, 테스트를 위해서라면...
		public void RegisterCar(CarMovement car) {
			if (!_allCars.Contains(car)) _allCars.Add(car);
		}
		public void UnregisterCar(CarMovement car) {
			if (_allCars.Contains(car)) _allCars.Remove(car);
		}
		/// <summary>
		//private void Start() {
		//	SpawnHouse();
		//	SpawnHouse();
		//	SpawnDestination();
		//}
		/// </summary>
		/// 


		//----------------------- 집 스폰
		public void SpawnHouse() {
			if (TryFindValidPosition(1, 1, isHouse: true, out Vector2Int spawnPos)) {
				Vector3 worldPos = new Vector3(spawnPos.x + 0.5f, 0, spawnPos.y + 0.5f);
				GameObject go = Instantiate(_housePrefab, worldPos, Quaternion.identity, _structureContainer);

				if (go.TryGetComponent(out House house)) {
					RoadDirection vaildDir = GetRandomVaildDirectionForHouse(spawnPos);

					house.Initialize(spawnPos, vaildDir);

					UpdateGridType(spawnPos, TileLogicType.Supply);
					UpdateGridType(house.EntranceCoordinate, TileLogicType.Entrance);
					//RoadSystem.Instance.CreateRoadNode(house.EntranceCoordinate);
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

			if (TryFindValidPosition(w, h, isHouse: false , out Vector2Int spawnPos)) {
				Vector3 worldPos = new Vector3(spawnPos.x + (w * 0.5f), 0, spawnPos.y + (h * 0.5f));
				GameObject go = Instantiate(_destinationPrefab, worldPos, Quaternion.identity, _structureContainer);

				if (go.TryGetComponent(out Destination dest)) {
					// [중요] Initialize 전에 형태(Shape) 정보가 필요하므로 별도 설정 함수 호출은 유지하되,
					// 내부에서 입구 방향을 결정하도록 로직 위임 가능. 
					// 하지만 여기서는 매니저가 주도권을 갖기 위해 방향을 계산해서 넘겨줌.
					//~> AI의 도움을 받음. 솔직히 무슨 소리인지 잘 모르겠음.
					RoadDirection validDir = GetRandomValidDirectionForDestination(spawnPos, isHorizontal);

					dest.SetupDestination(spawnPos, isHorizontal, validDir);

					for (int x = 0; x < w; x++) {
						for (int y = 0; y < h; y++) {
							Vector2Int currentTile = spawnPos + new Vector2Int(x, y);
							UpdateGridType(currentTile, TileLogicType.Demand);
						}
					}

					UpdateGridType(dest.EntranceCoordinate, TileLogicType.Entrance);
					//RoadSystem.Instance.CreateRoadNode(dest.EntranceCoordinate);
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
			foreach (var dest in _destinations) {
				if(dest.HasUnassignedPin()) {
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
		//private bool TryDisPatchToSpecific(House house, Destination dest) {
		//	List<Vector2Int> path = Pathfinder.FindPath(house.EntranceCoordinate, dest.EntranceCoordinate);

		//	if(path != null) {
		//		house.DispatchCar(path, dest);
		//		dest.RegisterIncomingCar();
		//		return true;
		//	}
		//	return false;
		//}

		//----------------------- 공간 탐색 알고리즘~~~ 타일 관련된 메서드들 모음.
		private bool TryFindValidPosition(int width, int height, bool isHouse, out Vector2Int bestPos) {
			//간단한 구현: 맵 전체를 랜덤하게 몇번 해봅니다.
			//실제로는 Buffer Zone으로, 이격거리를 줘야한다고 함.
			//...그래서 실제 게임에서 건물이 개같이 나온거구나...

			//2026.02.06 TODO : 가중치를 통하여 계산하는걸로 로직 수정.

			bestPos = Vector2Int.zero;
			List<Vector2Int> candidates = new List<Vector2Int>();
			float totalWeight = 0f;

			foreach(var kvp in MapBootstrapper.Grid) {
				CellData cell = kvp.Value;

				if(cell.Type != TileLogicType.Empty) continue;  //만약 비어있는 땅이 아니라면 설치 못하니까 패스.
				float w = isHouse ? cell.HouseWeight : cell.DestinationWeight;
				if (w <= 0.01f) continue;

				candidates.Add(cell.Coordinate);
				totalWeight += w;
			}

			if (candidates.Count.Equals(0)) return false;

			// 2. 룰렛 휠 선택 (가중치 랜덤)
			for (int i = 0; i < 20; i++) {
				float randomPoint = Random.value * totalWeight;
				float currentSum = 0f;
				Vector2Int selectedPos = candidates[0];

				foreach(var pos in candidates) {
					CellData c = MapBootstrapper.Grid[pos];
					float w = isHouse ? c.HouseWeight : c.DestinationWeight;
					currentSum += w;

					if(currentSum >= randomPoint) {
						selectedPos = pos;
						break;
					}
				}

				if (CheckAreaEmpty(selectedPos, width, height)) {
					if (isHouse) {
						// 집은 입구 하나만 있으면 됨 (방향은 나중에 정함)
						if (CheckEntranceSpace(selectedPos, width, height)) {
							bestPos = selectedPos;
							return true;
						}
					} else {
						// 목적지는 크기가 크므로 다시 확인
						if (CheckEntranceSpace(selectedPos, width, height)) { // 목적지 전용 체크 로직 필요시 교체
							bestPos = selectedPos;
							return true;
						}
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
		private bool CheckEntranceSpace(Vector2Int root, int w, int h) {
			Vector2Int[] checks = {
				root + new Vector2Int(0, h),  // North
				root + new Vector2Int(0, -1), // South
				root + new Vector2Int(w, 0),  // East
				root + new Vector2Int(-1, 0)  // West
			};

			foreach (var pos in checks) {
				if (IsValidEntrance(pos)) return true;
			}
			return false;
		}
		private RoadDirection GetRandomVaildDirectionForHouse(Vector2Int root) {
			RoadDirection[] dirs = { 
				RoadDirection.North, 
				RoadDirection.East, 
				RoadDirection.South, 
				RoadDirection.West,

				RoadDirection.NorthEast, 
				RoadDirection.NorthWest, 
				RoadDirection.SouthEast, 
				RoadDirection.SouthWest
			};
			ShuffleArray(dirs);

			foreach(var dir in dirs) {
				Vector2Int checkPos = root + DirUtiles.GetVectorFromDirection(dir);
				//도로 지을 수 있음?
				if (RoadSystem.Instance.IsRoadBuildable(checkPos)) return dir;
			}
			//막혀있으면 그냥 북쪽으로.
			return RoadDirection.North;
		}
		private RoadDirection GetRandomValidDirectionForDestination(Vector2Int root, bool isHorizontal) {
			//집과는 더 까다로운 방식일거 같다...ㅁ ㅜ섭다 ㅠ
			RoadDirection[] candidates;

			if(isHorizontal) {
				candidates = new RoadDirection[] { RoadDirection.North, RoadDirection.South };
			} else {
				candidates = new RoadDirection[] { RoadDirection.East, RoadDirection.West };
			}
			//사실 섞는게 의미 있는건가 싶긴 한데. 그냥 50% 확률 또 만들기 귀찮으니 섞는걸로 합시다.
			ShuffleArray(candidates);
			foreach(var dir in candidates) {
				Vector2Int anchorOffset = GetBaseAnchorOffset(dir, isHorizontal);
				Vector2Int checkPos = root + anchorOffset + DirUtiles.GetVectorFromDirection(dir);

				if (RoadSystem.Instance.IsRoadBuildable(checkPos)) return dir;
			}

			//자리 없으면 그냥 설치해버리기. (이것도 나중에 바꿔야함.)
			return candidates[0];
		}

		private Vector2Int GetBaseAnchorOffset(RoadDirection dir, bool isHorizontal) {
			if (isHorizontal) {
				return (dir.Equals(RoadDirection.North)) ? new Vector2Int(2, 1) : new Vector2Int(2, 0);
			} else {
				return (dir.Equals(RoadDirection.East)) ? new Vector2Int(1, 2) : new Vector2Int(0, 2);
			}
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
				if (type != TileLogicType.Road) {
					data.ConnectionMask = RoadDirection.None;
					data.MothballedMask = RoadDirection.None;
				}
				MapBootstrapper.Grid[pos] = data;
			} else {
				CellData newData = new CellData(pos) {
					Type = type,
				};
				MapBootstrapper.Grid.Add(pos, newData);
			}
		}

		//유틸: InteractionController에서 사용하기 위해 집 찾는 메서드
		public House GetHouseAt(Vector2Int pos) {
			foreach (var house in _houses) {
				if (house.RootCoordinate.Equals(pos)) return house;
			}
			return null;
		}

		//배열 섞기 유틸
		/* Sawp을 Tuples 방식으로.
		찾아보다가, temp 교환보다 간단한 방식을 찾음.
		C#에는 Tuple Deconstruction 방식이 있는데, 설명하면 기니까, 임시 객체를 만들어 교환합니다.
		temp방식은 고전적이지만, 성능이 살짝 더 좋음.
		나중에 이 작업을 수억번하는게 잦다면, temp가 더 최적화 방식이라고 함.
		근데 나는 구조물 생성할때만 한번씩 하고, 방향도 정해져있기 때문에 이걸 써도 최적화는 괜찮을지도?

		+ Tuples에 대한 고찰
		어.. Tuples. 즉, 튜플은 그냥 하나의 변수에 자료형을 여러개 담을 수 있는 기능이라고 합니다.
		그 변수는 사실상 배열 형태라고 볼 수 있겠죠.
			예시) (double, int) test = (2.5, 1); 이 가능함.

		내부의 값을 구하는건 배열처럼 []을 사용하는게 아닌, item을 사용해서 구합니다.
		index가 아니라 범위의 1,2를 사용함.

		여기서 중요한건, 배열이지만, 배열과 동일한 형태는 아니라 사용하기 좀 껄끄럽다 정도?
		어렵네요. 정말 간단해서 활용처는 무궁무진할거 같지만, 이걸 자주 쓸지와 어디서 쓸지는 예상이 잘 안가네요.
		*/
		private void ShuffleArray<T>(T[] array) {
			for(int i = array.Length - 1; i > 0; i--) {
				int j = Random.Range(0, i + 1);

				
				(array[i], array[j]) = (array[j], array[i]);
			}
		}
	}
}
