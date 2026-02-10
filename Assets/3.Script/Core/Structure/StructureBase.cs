using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Structure {
	using Core.Data;

	public abstract class StructureBase : MonoBehaviour {
		public Vector2Int RootCoordinate { get; protected set; } // 기준점 (좌하단)
		public Vector2Int EntranceCoordinate { get; protected set; } // 입구(도로) 좌표
		public RoadDirection EntranceDir { get; protected set; } // 입구가 뻗어나가는 방향

		public virtual void Initialize(Vector2Int root, RoadDirection dir) {
			RootCoordinate = root;
			EntranceDir = dir;
			CalculateEntrancePos();

			// 시각적 회전 처리는 자식에서
		}

		protected abstract void CalculateEntrancePos();
	}
}
