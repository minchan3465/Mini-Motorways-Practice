using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Managers {
	using Core.Data;
	using Core.Systems;

	public class GameManager : MonoBehaviour {
		public static GameManager Instance = null;

		[Header("Game Flow Settings")]
		[SerializeField] private float _houseSpawnInterval = 5.0f; // 5초마다 집 시도
		[SerializeField] private float _destinationSpawnInterval = 20.0f; // 20초마다 목적지 시도
		[SerializeField] private int _roadRewardAmount = 10;    // 목적지 생성 시 도로 보상 -> 나중에 시간으로 바꿀거임~

		[Header("Limits")]
		[SerializeField] private int _maxHouses = 15;
		[SerializeField] private int _maxDestinations = 4;

		private float _houseTimer = 0f;
		private float _destTimer = 0f;

		//----------------------
		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		private void Start() {
			//일단 공란.
		}

		private void Update() {
			HandleSpawning();
		}

		private void HandleSpawning() {
			float dt = Time.deltaTime;
			_houseTimer += dt;
			_destTimer += dt;

			// 1. 집 스폰
			if (_houseTimer >= _houseSpawnInterval) {
				_houseTimer = 0f;

				// 현재 집 개수 확인 (StructureManager에 프로퍼티 필요)
				if (StructureManager.Instance.HouseCount < _maxHouses) {
					StructureManager.Instance.SpawnHouse();
				}
			}

			// 2. 목적지 스폰
			if (_destTimer >= _destinationSpawnInterval) {
				_destTimer = 0f;

				if (StructureManager.Instance.DestinationCount < _maxDestinations) {
					StructureManager.Instance.SpawnDestination();

					// [수정] ResourceManager를 통해 지급
					ResourceManager.Instance.AddResource(ItemType.Road, _roadRewardAmount);
					Debug.Log($"보상: 도로 +{_roadRewardAmount}");
				}
			}
		}
	}
}

