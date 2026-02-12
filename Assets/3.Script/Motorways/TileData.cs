using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;

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
		North = 1 << 0,		//1
		NorthEast = 1 << 1, //2
		East = 1 << 2,		//4
		SouthEast = 1 << 3,	//8
		South = 1 << 4,		//16
		SouthWest = 1 << 5,	//32
		West = 1<< 6,		//64
		NorthWest = 1<<7,	//128
		All = 255
	}

	public class TileData {
		public Vector2Int coordinate { get; private set; }
		public TileLogicType type { get; set; }

		public RoadState[] RoadStates { get; private set; } //8방향 도로 상태 저장.

		public GameObject Object;	//타일 위에 오브젝트 참조.

		//건물 생성 가중치
		public float WeightHouseSpawn;
		public float WeightDestinationSpawn;

		public TileData(Vector2Int coord) {
			coordinate = coord;
			type = TileLogicType.None;

			RoadStates = new RoadState[8];
			for(int i = 0; i< RoadStates.Length; i++) {
				RoadStates[i] = RoadState.None;
			}
		}

		public bool IsBuildable() => type == TileLogicType.None;

		public RoadState GetRoadState(TileDirection direction) {
			//반드시 단일 방향만 들어와야함.
			int index = (int)Math.Log((int)direction,2);
			return RoadStates[index];
		}

		public void SetRoadState(TileDirection direction, RoadState state) {
			int index = TileUtils.GetDirectionIndex(direction);
			if (index >= 0 && index < RoadStates.Length) {
				RoadStates[index] = state;
			}
		}
	}
}

