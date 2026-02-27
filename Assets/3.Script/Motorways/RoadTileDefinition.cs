using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	[Serializable]
	public class RoadTileDefinition {
		public int index = -1; //고유 인덱스
		public int rotationSteps = 0; //회전ㄱ밧.
		public RoadTileMesh mesh; //타일이 출력해야 할 최종 3D 메쉬 묶음

		public RoadTileSignature signature;

		//각 연결(Connection)별로 계산된 2D 경로 점들 (차량 이동 경로 계산 등 논리적 용도로 보관)
		public Dictionary<RoadTileConnection, List<Vector2>> connectionToPath = new Dictionary<RoadTileConnection, List<Vector2>>();

		public void Reset() {
			index = -1;
			mesh = null;
			rotationSteps = 0;
			signature = null;
			connectionToPath.Clear();
		}

		//특정 각도로 회전된 새로운 Definition을 생성하여 반환 (원작의 CreateRotatedDefinition
		public RoadTileDefinition CreateRotatedDefinition(int newRotationSteps) {
			RoadTileDefinition rotatedDef = new RoadTileDefinition();
			rotatedDef.mesh = this.mesh;    //메쉬 자체는 런타임에 회전시켜 출력하므로, 원본 그대로...
			rotatedDef.rotationSteps = newRotationSteps;

			if (this.signature != null) {
				int amountToRotate = (newRotationSteps - this.rotationSteps + 8) % 8;
				rotatedDef.signature = this.signature.CreateRotatedSignature(amountToRotate);
			}

			int amountToRotatePath = (newRotationSteps - this.rotationSteps + 8) % 8;
			foreach (var pair in connectionToPath) {
				//key
				RoadTileConnection rotatedConn = pair.Key.GetRotatedConnection(amountToRotatePath);

				//Value
				List<Vector2> rotatedPath = new List<Vector2>();
				foreach(var point in pair.Value) {
					float rad = amountToRotatePath * 45f * Mathf.Deg2Rad;
					float cos = Mathf.Cos(rad);
					float sin = Mathf.Sin(rad);
					float nx = point.x * cos + point.y * sin;
					float ny = -point.x * sin + point.y * cos;
					rotatedPath.Add(new Vector2(nx, ny));
				}

				rotatedDef.connectionToPath.Add(rotatedConn, rotatedPath);
			}

			return rotatedDef;
		}
	}
}
