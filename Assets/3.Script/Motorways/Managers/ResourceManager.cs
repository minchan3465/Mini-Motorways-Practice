using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Motorways.UI;

namespace Motorways {
	public class ResourceManager : MonoBehaviour {
		public static ResourceManager Instance = null;

		[Header("Initial Settings")]
		[SerializeField] private int _startRoadCount = 30;
		[SerializeField] private int _startBridgeCount = 1;

		private Dictionary<ItemType, int> _inventory = new Dictionary<ItemType, int>();

		//���߿� UI �����̳� �ڿ� ���� �ÿ� ȣ���� �̺�Ʈ��. ��� Ȱ�������� ����.
		//public event Action<ItemType, int> OnResourceChanged;
		[Header("UI")]
		[SerializeField] private Inv_Road Inv_RoadUI;
		[SerializeField] private Inv_Bridge Inv_BridgeUI;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			//초기화
			_inventory.Add(ItemType.Road, _startRoadCount);
			_inventory.Add(ItemType.TrafficLight, 0);
			_inventory.Add(ItemType.Roundabout, 0);
			_inventory.Add(ItemType.Motorway, 0);
			_inventory.Add(ItemType.Bridge, _startBridgeCount);
		}

		private void Start() {
			NotifyChange(ItemType.Road);
		}

		//------------------- ��ȸ
		public int GetCount(ItemType type) {
			if (_inventory.TryGetValue(type, out int count)) return count;
			return 0;
		}
		public bool HasResource(ItemType type, int amount = 1) { 
			//������ �̻��϶��� true
			return GetCount(type) >= amount; 
		}

		//------------------- ����
		public void AddResource(ItemType type, int amount) {
			if (!_inventory.ContainsKey(type)) _inventory[type] = 0;    //Ȥ�� �� ����ó�� �� �ʱ�ȭ.
			
			_inventory[type] += amount;
			NotifyChange(type);
		}

		//������ �Һ��մϴ�. �ٵ� �ΰ��ӿ����� ����� Ÿ�� ��ĭ�� 1���� �Ҹ��ϱ� ������ amount = 1�� ����.
		public bool TryConsumeResource(ItemType type, int amount = 1) {
			if (HasResource(type, amount)) {
				_inventory[type] -= amount;
				NotifyChange(type);
				return true;
			}
			//�Һ� �������� ������...
			return false;
		}

		//���߿� UI �˸��� ���.
		private void NotifyChange(ItemType type) {
			switch (type) {
				case ItemType.Road:
					Inv_RoadUI.ChangeRoadCount();
					break;
				case ItemType.TrafficLight:
					break;
				case ItemType.Roundabout:
					break;
				case ItemType.Motorway:
					break;
				case ItemType.Bridge:
					Inv_BridgeUI.ChangeRoadCount();
					break;
			}
			//OnResourceChanged?.Invoke(type, _inventory[type]);
			//Debug.Log($"[Resource] {type}: {_inventory[type]}");
		}
	}
}
