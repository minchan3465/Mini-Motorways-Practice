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
		public List<Destination> ActiveDestinations { get; private set; } = new List<Destination>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
			InitializeLayouts();
		}

		private void Start() {
			ScheduleBuilding(BuildingType.House, 1, 2.0f);
			ScheduleBuilding(BuildingType.House, 1, 2.0f);
			ScheduleBuilding(BuildingType.Destination, 1, 4.0f);
		}

		private void InitializeLayouts() {
			// 집: 1x1. 입구는 자기 자신(0,0)
			HouseLayouts = new List<BuildingLayout> {
				new BuildingLayout { Footprint = new Vector2Int(1, 1), LocalEntrance = new Vector2Int(0, 0), Driveways = new List<TileDirection> { TileDirection.North } }
			};

			// 목적지: (가로 3x2, 세로 2x3)
			// LocalEntrance: 하단 좌측(0,0) 타일로부터 입구 타일까지의 정수 오프셋
			DestinationLayouts = new List<BuildingLayout> {
				// Horizontal (3x2) - 긴 면(3)의 좌측 '상단 또는 하단'에 위치함)
				new BuildingLayout { Footprint = new Vector2Int(3, 2), LocalEntrance = new Vector2Int(0, 0), Driveways = new List<TileDirection> { TileDirection.South } }, // 중앙 아래
				new BuildingLayout { Footprint = new Vector2Int(3, 2), LocalEntrance = new Vector2Int(0, 1), Driveways = new List<TileDirection> { TileDirection.North } }, // 중앙 위
				
				// Vertical (2x3) - 긴 면(3)의 '좌측 또는 우측' 하단에 위치함
				new BuildingLayout { Footprint = new Vector2Int(2, 3), LocalEntrance = new Vector2Int(0, 0), Driveways = new List<TileDirection> { TileDirection.West } },  // 중앙 왼쪽
				new BuildingLayout { Footprint = new Vector2Int(2, 3), LocalEntrance = new Vector2Int(1, 0), Driveways = new List<TileDirection> { TileDirection.East } }   // 중앙 오른쪽
			};
		}

		public void ScheduleBuilding(BuildingType type, int groupIndex, float delay) {
			float currentTime = 0f;
			if (Process.ClockProcess.Instance != null && Process.ClockProcess.Instance.Model != null) {
				currentTime = Process.ClockProcess.Instance.Model.ExpansionTime;
			} else {
				currentTime = Time.time; // 백업용
			}

			float spawnTime = currentTime + delay;
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
