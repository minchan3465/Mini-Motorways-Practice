using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.Systems.Structure {
	using Core.Data;

	public class Destination : StructureBase {
		[Header("Destination Settings")]
		[SerializeField] private int _maxParkingSlots = 6;

		[Header("Request Settings")]
		[SerializeField] private float _requestInterval = 5.0f; // n초마다 요청
		public int CurrentPins { get; private set; } = 0;

		//가로인지 세로인지 (2x3인지, 3x2인지...)
		//프로퍼티로 한 이유는 생성될때 가로세로인지 정하고, 이후 확인할때 사용하려고.
		public bool IsHorizontal { get; private set; }

		private Queue<GameObject> _parkedCars = new Queue<GameObject>();

		public int IncomingCars = 0;

		//주차장의 로컬 좌표 오프셋... (범위는 2x3이지만, 2x2는 건물, 2x1는 주차장으로 사용해야하기 때문)
		private Vector2Int[] _parkingOffsets;

		//-----------------------------

		private void Start() { StartCoroutine(GeneratePinRoutine()); }
		private IEnumerator GeneratePinRoutine() {
			// 게임 시작 후 약간의 딜레이
			yield return new WaitForSeconds(2.0f);
			WaitForSeconds wfs = new WaitForSeconds(_requestInterval);

			while (true) {
				yield return wfs;
				AddPin();
			}
		}

		private void AddPin() {
			CurrentPins += 1;
			Debug.Log($"[Destination] Pin Spawned. Now Pin Count : {CurrentPins} ( Pos : {RootCoordinate}");

			StructureManager.Instance.OnPinCreated(this);
		}

		public bool HasUnassignedPin() { return (CurrentPins - IncomingCars) > 0; }
		public void RegisterIncomingCar() { IncomingCars += 1; }

		public void CarArrived() {
			IncomingCars--;
			if (CurrentPins > 0) {
				CurrentPins -= 1;
			}
			Debug.Log($"차량 도착! ({name}) 남은 핀: {CurrentPins}");
		}

		public bool TryParkCar(GameObject car) {
			if(_parkedCars.Count < _maxParkingSlots) {
				_parkedCars.Enqueue(car);
				return true;
			}
			return false;
		}

		//---------------------생성시 계산
		public void SetupDestination(Vector2Int root, bool isHorizontal) {
			IsHorizontal = isHorizontal;
			RoadDirection validDir;

			if (IsHorizontal) {
				validDir = (Random.value > 0.5f) ? RoadDirection.North : RoadDirection.South;
				_parkingOffsets = new Vector2Int[] { new Vector2Int(2, 0), new Vector2Int(2, 1) };
			} else {
				validDir = (Random.value > 0.5f) ? RoadDirection.East : RoadDirection.West;
				_parkingOffsets = new Vector2Int[] { new Vector2Int(0, 2), new Vector2Int(1, 2) };
			}

			base.Initialize(root, validDir);

			ApplyShapeRotation();
		}

		private void ApplyShapeRotation() {
			if (IsHorizontal) {
				transform.rotation = Quaternion.Euler(0, 0, 0);
			} else {
				transform.rotation = Quaternion.Euler(0, -90, 0);
			}
		}


		public Vector3 GetParkingPosition() {
			if(_parkingOffsets != null && _parkingOffsets.Length > 0) {
				Vector2Int parkingTile = RootCoordinate + _parkingOffsets[0];
				return new Vector3(parkingTile.x + 0.5f, 0, parkingTile.y + 0.5f);
			}
			return transform.position;
		}

		protected override void CalculateEntrancePos() {
			//주차장같은 경우는 2x3에서 2x2의 건물에 딸린거니까, 그냥 거기에 맞춰서 나오면 됨.
			Vector2Int anchorOffset = Vector2Int.zero;

			if (IsHorizontal) {
				if (EntranceDir.Equals(RoadDirection.North)) {
					anchorOffset = new Vector2Int(2, 1);
				} else {
					// South
					anchorOffset = new Vector2Int(2, 0);
				}
			} else {
				// Vertical
				if (EntranceDir.Equals(RoadDirection.East)) {
					anchorOffset = new Vector2Int(1, 2);
				} else { 
					// West
					anchorOffset = new Vector2Int(0, 2);
				}
			}

			EntranceCoordinate = RootCoordinate + anchorOffset + GetDirVector(EntranceDir);

		}

		private Vector2Int GetDirVector(RoadDirection dir) {
			if (dir.Equals(RoadDirection.North)) return new Vector2Int(0, 1);
			if (dir.Equals(RoadDirection.South)) return new Vector2Int(0, -1);
			if (dir.Equals(RoadDirection.East)) return new Vector2Int(1, 0);
			if (dir.Equals(RoadDirection.West)) return new Vector2Int(-1, 0);
			return Vector2Int.zero;
		}
	}
}
