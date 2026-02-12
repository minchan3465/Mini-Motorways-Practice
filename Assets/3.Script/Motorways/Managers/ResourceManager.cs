using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {

	public class ResourceManager : MonoBehaviour {
		public static ResourceManager Instance = null;

		[Header("Initial Settings")]
		[SerializeField] private int _startRoadCount = 30;

		private Dictionary<ItemType, int> _inventory = new Dictionary<ItemType, int>();

		//나중에 UI 연동이나 자원 변경 시에 호출할 이벤트들. 어떻게 활용할지는 미정.
		//public event Action<ItemType, int> OnResourceChanged;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			// 초기화
			_inventory.Add(ItemType.Road, _startRoadCount);
			_inventory.Add(ItemType.TrafficLight, 0);
			_inventory.Add(ItemType.Roundabout, 0);
			_inventory.Add(ItemType.Motorway, 0);
		}

		private void Start() {
			NotifyChange(ItemType.Road);
		}

		//------------------- 조회
		public int GetCount(ItemType type) {
			if (_inventory.TryGetValue(type, out int count)) return count;
			return 0;
		}
		public bool HasResource(ItemType type, int amount = 1) { 
			//갯수가 이상일때만 true
			return GetCount(type) >= amount; 
		}

		//------------------- 조작
		public void AddResource(ItemType type, int amount) {
			if (!_inventory.ContainsKey(type)) _inventory[type] = 0;    //혹시 모를 예외처리 및 초기화.
			
			_inventory[type] += amount;
			NotifyChange(type);
		}

		//아이템 소비합니다. 근데 인게임에서는 모든지 타일 한칸에 1개씩 소모하기 때문에 amount = 1로 설정.
		public bool TryConsumeResource(ItemType type, int amount = 1) {
			if (HasResource(type, amount)) {
				_inventory[type] -= amount;
				NotifyChange(type);
				return true;
			}
			//소비 못했음요 ㅇㅅㅇ...
			return false;
		}

		//나중에 UI 알릴때 사용.
		private void NotifyChange(ItemType type) {
			//OnResourceChanged?.Invoke(type, _inventory[type]);
			Debug.Log($"[Resource] {type}: {_inventory[type]}");
		}
	}
}
