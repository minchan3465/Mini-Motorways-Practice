using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	public class RoadTile {
		public Vector2Int Coordinates;

		//이 RoadTile에서 시작되거나 끝나는 모든 Lane의 리스트
		public List<Lane> Lanes = new List<Lane>();

		public RoadTile(Vector2Int coords) { Coordinates = coords; }

		public Lane GetLaneTo(Vector2Int targetCoords) {
			return Lanes.Find(l => l.EndNode == targetCoords);
		}
		public void AddLane(Lane lane) {
			if (!Lanes.Contains(lane)) Lanes.Add(lane);
		}
		public void RemoveLane(Lane lane) {
			if (Lanes.Contains(lane)) Lanes.Remove(lane);
		}
	}
}


