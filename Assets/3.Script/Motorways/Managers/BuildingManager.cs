using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Motorways.Models;

	public class BuildingManager : MonoBehaviour {
		public static BuildingManager Instance;

		[Header("Prefabs")]
		public GameObject HousePrefab;
		public GameObject DestinationPrefab;
		public GameObject VehiclePrefab;

		public List<ScheduledBuilding> ScheduleList { get; private set; } = new List<ScheduledBuilding>();
		public List<BuildingLayout> HouseLayouts { get; private set; }
		public List<BuildingLayout> DestinationLayouts { get; private set; }

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
			InitializeLayouts();
		}

		private void Start() {
			ScheduleBuilding(BuildingType.House, 0, 2.0f);
			ScheduleBuilding(BuildingType.Destination, 0, 4.0f);
		}

		private void InitializeLayouts() {
			HouseLayouts = new List<BuildingLayout> {
				new BuildingLayout {
					Footprint = new Vector2Int(1, 1),
					LocalEntrance = new Vector2Int(0, 0),
					Driveways = new List<TileDirection> { TileDirection.North }
				}
			};

			DestinationLayouts = new List<BuildingLayout> {
				new BuildingLayout {
					Footprint = new Vector2Int(2, 3),        // 원본 크기
					LocalEntrance = new Vector2Int(0, 0),    // 원본 주차장 위치
					Driveways = new List<TileDirection> { TileDirection.West } // 원본 입구 방향
				}
			};
		}

		public void ScheduleBuilding(BuildingType type, int groupIndex, float delay) {
			float spawnTime = Time.time + delay;    //나중에 TimeManager 시간으로 교체
			ScheduledBuilding ticket = new ScheduledBuilding {
				Type = type,
				GroupIndex = groupIndex,
				SpawnTime = spawnTime,
				SpawnAttempts = 0
			};

			ScheduleList.Add(ticket);
			ScheduleList.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
		}
	}
}
