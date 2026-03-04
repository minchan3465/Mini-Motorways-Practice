using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;
	using Motorways.Models;

	public enum TileLogicType {
		None,
		Empty,
		House,
		Destination,
		Water,
		Mountain,
		Carpark,
		Motorway,
	}

	[Flags]
	public enum TileDirection {
		None = 0,
		North = 1 << 0,     //1
		NorthEast = 1 << 1, //2
		East = 1 << 2,      //4
		SouthEast = 1 << 3, //8
		South = 1 << 4,     //16
		SouthWest = 1 << 5, //32
		West = 1 << 6,      //64
		NorthWest = 1 << 7, //128
		All = 255
	}

	public class TileData {
		public Vector2Int coordinate { get; private set; }
		public TileLogicType type { get; set; }

		public RoadState[] RoadStates { get; private set; } //8방향 도로 상태 저장.
		public Lane[] Lanes { get; private set; }

		public BuildingBase Building { get; set; }   //타일 위에 오브젝트 참조.

		public float creationTime { get; private set; }	//도로가 처음 설치된 시간.

		//건물 생성 가중치
		public float WeightHouseSpawn;
		public float WeightDestinationSpawn;

		public TileData(Vector2Int coord) {
			coordinate = coord;
			type = TileLogicType.None;

			RoadStates = new RoadState[8];
			Lanes = new Lane[8];

			for (int i = 0; i < RoadStates.Length; i++) {
				RoadStates[i] = RoadState.None;
			}

			creationTime = -1f;
		}
		public bool HasAnyRoad {
			get {
				for(int i = 0; i<8; i++) {
					if (RoadStates[i] != RoadState.None) return true;
				}
				return false;
			}
		}
		public bool IsBuildable() => type == TileLogicType.Empty && !HasAnyRoad;


		//---Lane---
		public void ConnectLane(TileDirection dir, Lane lane) {
			int index = GetIndex(dir);
			if (index == -1) return;

			Lanes[index] = lane;
			RoadStates[index] = RoadState.Active;

			//처음 도로가 깔리는 시점의 시간을 기록 (이미 기록되어 있다면 유지)
			if (creationTime < 0f) creationTime = Time.time;
			
		}
		public void DisconnectLane(TileDirection dir) {
			int index = GetIndex(dir);
			if (index == -1) return;

			Lanes[index] = null;
			RoadStates[index] = RoadState.None;

			if (!HasAnyRoad) creationTime = -1f;
		}
		public Lane GetLane(TileDirection dir) {
			int index = GetIndex(dir);
			if (index == -1) return null;
			return Lanes[index];
		}
		//---RoadState---
		public void SetRoadState(TileDirection direction, RoadState state) {
			int index = GetIndex(direction);
			if (index >= 0 && index < 8) {
				RoadStates[index] = state;
			}
		}
		public RoadState GetRoadState(TileDirection direction) {
			int index = GetIndex(direction);
			if (index >= 0 && index < 8) return RoadStates[index];
			return RoadState.None;
		}


		//인덱스 구하기
		private int GetIndex(TileDirection dir) {
			if (dir == TileDirection.None) return -1;
			return TileUtils.GetDirectionIndex(dir);
		}
	}
}

