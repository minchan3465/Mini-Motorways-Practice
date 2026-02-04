using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems { 
	using Core.Data;

	public class MapDebugVisualizer : MonoBehaviour {
		private void OnDrawGizmos() {
			if (MapBootstrapper.Grid == null) return;

			foreach (var kvp in MapBootstrapper.Grid) {
				CellData cell = kvp.Value;
				if (cell.Type != TileLogicType.Empty) {
					switch (cell.Type) {
						//case TileLogicType.Empty:
						//	Gizmos.color = new Color(255f, 255f, 255f, 72f);
						//	break;
						case TileLogicType.Obstacle:
							Gizmos.color = Color.red;
							break;
						case TileLogicType.Road:
							Gizmos.color = Color.gray;
							break;
						case TileLogicType.Supply:
							Gizmos.color = Color.blue;
							break;
						case TileLogicType.Demand:
							Gizmos.color = Color.yellow;
							break;
						case TileLogicType.Entrance:
							Gizmos.color = Color.cyan;
							break;
						case TileLogicType.Restricted:
							break;
					}

					// 시각적 확인을 위해 위치를 다시 월드로 변환해서 그리기
					Vector3 center = new Vector3(cell.Coordinate.x + 0.5f, 0, cell.Coordinate.y + 0.5f);
					Gizmos.DrawWireCube(center, Vector3.one * 0.9f);

					if ((cell.Type.Equals(TileLogicType.Road) || cell.Type.Equals(TileLogicType.Entrance)) && cell.ConnectionMask != RoadDirection.None) {
						Gizmos.color = Color.magenta;
						DrawConnections(center, cell.ConnectionMask);
					}

				}
			}
		}

		// 비트마스크를 풀어서 선으로 그리는 헬퍼 함수
		private void DrawConnections(Vector3 center, RoadDirection mask) {
			// 각 비트가 켜져 있는지 확인하고 선 긋기
			if (mask.HasFlag(RoadDirection.North)) Gizmos.DrawLine(center, center + new Vector3(0, 0, 0.5f));
			if (mask.HasFlag(RoadDirection.South)) Gizmos.DrawLine(center, center + new Vector3(0, 0, -0.5f));
			if (mask.HasFlag(RoadDirection.East)) Gizmos.DrawLine(center, center + new Vector3(0.5f, 0, 0));
			if (mask.HasFlag(RoadDirection.West)) Gizmos.DrawLine(center, center + new Vector3(-0.5f, 0, 0));

			// 대각선
			if (mask.HasFlag(RoadDirection.NorthEast)) Gizmos.DrawLine(center, center + new Vector3(0.5f, 0, 0.5f));
			if (mask.HasFlag(RoadDirection.SouthEast)) Gizmos.DrawLine(center, center + new Vector3(0.5f, 0, -0.5f));
			if (mask.HasFlag(RoadDirection.SouthWest)) Gizmos.DrawLine(center, center + new Vector3(-0.5f, 0, -0.5f));
			if (mask.HasFlag(RoadDirection.NorthWest)) Gizmos.DrawLine(center, center + new Vector3(-0.5f, 0, 0.5f));
		}
	}
}

