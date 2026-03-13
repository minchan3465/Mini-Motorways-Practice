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
		public List<House> ActiveHouses { get; private set; } = new List<House>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
			InitializeLayouts();
		}

		private void Start() {
			GenerateInitialSchedule();
		}

		private void GenerateInitialSchedule() {
			float[] spawnDays = {
				//1주차 (Color 1, 2)
				1.0f, 3.5f, 
				//2주차 
				8.0f, 12.0f,
				//3주차 (Color 3 등장)
				16.0f, 20.0f,
				//4주차
				23.0f, 27.0f,
				//5주차 (Color 4 등장)
				31.0f, 34.0f,
				//6주차
				37.0f, 41.0f,
				//7주차 (Color 5 등장)
				44.0f, 48.0f,
				//8주차
				51.0f, 55.0f,
				//9주차 (Color 6 등장)
				58.0f, 62.0f,
				//10주차 (마지막 스폰 구간)
				65.0f, 69.0f
			};

			int[] colors = {
				1, 1,
				2, 1,
				3, 2,
				1, 3,
				4, 2,
				1, 4,
				5, 3,
				2, 5,
				6, 4,
				1, 6
			};

			for (int i = 0; i < spawnDays.Length; i++) {
				float timeInSeconds = (i == 0) ? 0f : spawnDays[i] * 20.0f; 
				ScheduleBuilding(BuildingType.Destination, colors[i], timeInSeconds);
			}
		}

		private void InitializeLayouts() {
			//집: 1x1. 입구는 자기 자신(0,0)
			HouseLayouts = new List<BuildingLayout> {
				new BuildingLayout { Footprint = new Vector2Int(1, 1), LocalEntrance = new Vector2Int(0, 0), Driveways = new List<TileDirection> { TileDirection.North } }
			};

			//목적지: (가로 3x2, 세로 2x3)
			//LocalEntrance: 하단 좌측(0,0) 타일로부터 입구 타일까지의 정수 오프셋
			DestinationLayouts = new List<BuildingLayout> {
				//Horizontal (3x2) - 긴 면(3)의 좌측 '상단 또는 하단'에 위치함)
				new BuildingLayout { Footprint = new Vector2Int(3, 2), LocalEntrance = new Vector2Int(0, 0), Driveways = new List<TileDirection> { TileDirection.South } }, //중앙 아래
				new BuildingLayout { Footprint = new Vector2Int(3, 2), LocalEntrance = new Vector2Int(0, 1), Driveways = new List<TileDirection> { TileDirection.North } }, //중앙 위
				
				//Vertical (2x3) - 긴 면(3)의 '좌측 또는 우측' 하단에 위치함
				new BuildingLayout { Footprint = new Vector2Int(2, 3), LocalEntrance = new Vector2Int(0, 0), Driveways = new List<TileDirection> { TileDirection.West } },  //중앙 왼쪽
				new BuildingLayout { Footprint = new Vector2Int(2, 3), LocalEntrance = new Vector2Int(1, 0), Driveways = new List<TileDirection> { TileDirection.East } }   //중앙 오른쪽
			};
		}

		public void ScheduleBuilding(BuildingType type, int groupIndex, float delay) {
			float currentTime = 0f;
			if (Process.ClockProcess.Instance != null && Process.ClockProcess.Instance.Model != null) {
				currentTime = Process.ClockProcess.Instance.Model.ExpansionTime;
			} else {
				currentTime = Time.time; //백업용
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
