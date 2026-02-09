using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.Systems.Structure {
	using Core.Data;
	using Core.Utils;

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

		//주차 공간
		private bool[] _parkingSlots = new bool[3];
		private readonly Vector3[] _slotOffsets = new Vector3[] {
			new Vector3(-0.25f, 0, -0.25f),
			Vector3.zero,
			new Vector3(0.25f, 0, 0.25f),
		};

		//----------------------- 런타임 목적지 기능들.

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

		public Vector3 GetParkingPosition(out int slotIndex) {
			slotIndex = -1;
			for(int i = 0; i<3; i++) {
				if(!_parkingSlots[i]) {
					slotIndex = i;
					_parkingSlots[i] = true;
					break;
				}
			}
			//만약 주차장 자리가 없다면, 이 방향으로 가십사; (임시)
			if (slotIndex.Equals(-1)) slotIndex = 1;

			Vector2Int parkingTileOffset;
			if (IsHorizontal) {
				// 가로형 주차장은 (2,0)과 (2,1)에 위치.
				// 편의상 슬롯에 따라 타일을 나눕시다. (슬롯 2는 위쪽 타일, 나머지는 아래쪽 타일 등)
				// 여기선 그냥 (2,0)을 메인 주차장으로 씁니다.
				parkingTileOffset = new Vector2Int(2, 0);
			} else {
				// 세로형 주차장은 (0,2)와 (1,2)에 위치.
				parkingTileOffset = new Vector2Int(0, 2);
			}
			Vector2Int parkingGridPos = RootCoordinate + parkingTileOffset;
			Vector3 tileCenter = new Vector3(parkingGridPos.x + 0.5f, 0, parkingGridPos.y + 0.5f);

			return tileCenter + _slotOffsets[slotIndex];
		}

		public void ReleaseParkingSlot(int slotIndex) {
			if(slotIndex >= 0 && slotIndex < 3) {
				_parkingSlots[slotIndex] = false;
			}
		}

		//---------------------생성시 계산
		public void SetupDestination(Vector2Int root, bool isHorizontal, RoadDirection dir) {
			IsHorizontal = isHorizontal;
			base.Initialize(root, dir);
			ApplyShapeRotation();
		}

		private void ApplyShapeRotation() {
			if (IsHorizontal) {
				transform.rotation = Quaternion.Euler(0, 0, 0);
			} else {
				transform.rotation = Quaternion.Euler(0, -90, 0);
			}
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

			EntranceCoordinate = RootCoordinate + anchorOffset + DirUtiles.GetVectorFromDirection(EntranceDir);

		}
	}
}
