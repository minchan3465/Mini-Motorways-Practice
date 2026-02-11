using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Simulation.Buildings {

	public enum ColorType {
		Red,
		Blue,
		Yellow,
		Green
		//기타 등등 색
	}
	public abstract class Building {
		public int Id { get; private set; }
		private static int _nextId = 0;

		public Vector2Int RootCoordinate;    // 건물 본체가 위치한 타일
		public Vector2Int EntranceCoordinate; // 도로와 연결되는 진입로 타일
		public ColorType Color;

		//건물이 차지하는 영역 범위.
		public List<Vector2Int> OccupiedCoordinates = new List<Vector2Int>();
		public abstract void InitializeFootprint();

		public Building(Vector2Int root, ColorType color) {
			Id = _nextId++;
			RootCoordinate = root;
			Color = color;
		}
	}
}

