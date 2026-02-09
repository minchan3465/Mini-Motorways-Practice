using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.Structure {
	using Core.Data;
	using Core.Utils;

	public class House : StructureBase {
		[Header("House Settings")]
		[SerializeField] private int _availableCars;
		[SerializeField] private GameObject _carPrefab;
		
		private int _carCount = 2;

		private Queue<CarMovement> _carPool = new Queue<CarMovement>();

		public override void Initialize(Vector2Int root, RoadDirection dir) {
			base.Initialize(root, dir);
			_availableCars = _carCount;
		}

		protected override void CalculateEntrancePos() {
			Vector2Int dirVec = DirUtiles.GetVectorFromDirection(EntranceDir);
			EntranceCoordinate = RootCoordinate + dirVec;
		}

		public void DispatchCar(List<Vector2Int> path, Destination targetDest) {
			if (_availableCars <= 0) return;

			_availableCars -= 1;

			CarMovement car;

			if(_carPool.Count > 0) {
				car = _carPool.Dequeue();
			} else {
				GameObject carObj = Instantiate(_carPrefab, transform.position, Quaternion.identity, transform);
				carObj.TryGetComponent(out car);
			}

			if(car != null) {
				car.Initialize(this, targetDest, path);
			}
		}

		public void CarReturned(CarMovement car) {
			_availableCars += 1;
			if (_availableCars > _carCount) _availableCars = _carCount;

			if (car != null) {
				car.transform.position = transform.position;
				_carPool.Enqueue(car);
			}
			//차 돌아왔으니, 밀린 일이 있는지 매니저에게 물어봄.
			if(StructureManager.Instance != null) {
				StructureManager.Instance.OnCarAvailable(this);
			}
		}

		public bool HasAvailableCar() => _availableCars > 0;

		//플레이어가 드래그해서 입구 바꾸는거 계산
		public void TryRotateEntrance(RoadDirection newDir) {
			Vector2Int dirVec = DirUtiles.GetVectorFromDirection(newDir);
			Vector2Int newEntrancePos = RootCoordinate + dirVec;

			//여기 설치하는거 됩니까??
			if(StructureManager.Instance.IsValidEntrance(newEntrancePos)) {
				StructureManager.Instance.UpdateGridType(EntranceCoordinate, TileLogicType.Empty);

				EntranceDir = newDir;
				EntranceCoordinate = newEntrancePos;

				//새로운 입구 타일 설정...
				StructureManager.Instance.UpdateGridType(EntranceCoordinate, TileLogicType.Entrance);

				//보이는거 업데이트 시키는거~
				//UpdateVisualRotation();
			}
		}

		//집은 아마 회전 안할듯?
		//private void UpdateVisualRotation() {
		//	Vector2Int dir = DirUtiles.GetDirVector(EntranceDir);
		//	float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
		//	transform.rotation = Quaternion.Euler(0, angle, 0);
		//}
	}
}

