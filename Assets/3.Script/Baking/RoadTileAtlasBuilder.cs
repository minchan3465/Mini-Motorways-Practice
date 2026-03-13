using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Baking {
	using Motorways.Utils;

	public class RoadTileAtlasBuilder {
		private static readonly TileDirection[] AllDirections = new TileDirection[]
		{
			TileDirection.North, 
			TileDirection.NorthEast, 
			TileDirection.East, 
			TileDirection.SouthEast,
			TileDirection.South, 
			TileDirection.SouthWest, 
			TileDirection.West, 
			TileDirection.NorthWest
		};

		//타일의 모든 연결 조합은 2^8 = 256가지임.
		//이를 모두 생성하는 대신, 회전 대칭을 고려하여 고유한 시그니처만 추출함.

		public List<RoadTileSignature> GenerateAllUniqueSignatures() {
			List<RoadTileSignature> uniqueSignatures = new List<RoadTileSignature>();
			HashSet<byte> processedMasks = new HashSet<byte>();

			for (int i = 1; i < 256; i++) {
				byte mask = (byte)i;

				//1. 비트 마스크 회전 중복 검사
				bool isDuplicate = false;
				for (int step = 0; step < 8; step++) {
					int s = step % 8;
					byte rotatedMask = (byte)((mask << s) | (mask >> (8 - s)));
					if (processedMasks.Contains(rotatedMask)) {
						isDuplicate = true;
						break;
					}
				}
				if (isDuplicate) continue;

				processedMasks.Add(mask);

				//2. 현재 마스크에 해당하는 활성 방향 추출
				List<TileDirection> activeDirs = new List<TileDirection>();
				for (int dirIndex = 0; dirIndex < 8; dirIndex++) {
					if ((mask & (1 << dirIndex)) != 0) activeDirs.Add(AllDirections[dirIndex]);
				}

				//3. 시그니처 생성 (모든 노드 간 연결 설정)
				RoadTileSignature tempSignature = new RoadTileSignature();
				if (activeDirs.Count == 1) {
					tempSignature.AddConnection(new RoadTileConnection(new RoadTileNode(activeDirs[0], RoadType.TwoLane), new RoadTileNode(activeDirs[0], RoadType.TwoLane)));
				} else {
					for (int a = 0; a < activeDirs.Count; a++) {
						for (int b = a + 1; b < activeDirs.Count; b++) {
							tempSignature.AddConnection(new RoadTileConnection(
								new RoadTileNode(activeDirs[a], RoadType.TwoLane),
								new RoadTileNode(activeDirs[b], RoadType.TwoLane)));
						}
					}
				}
				uniqueSignatures.Add(tempSignature);
			}
			return uniqueSignatures;
		}

		public List<Vector2> ConstructPathForConnection(RoadTileConnection connection) {
			List<Vector2> pathPoints = new List<Vector2>();

			TileDirection inDir = connection.input.direction;
			TileDirection outDir = connection.output.direction;

			float cornerHandleScale = 0.276f * MapSettings.TILE_SIZE;
			float tightCornerHandleScale = 0.1f * MapSettings.TILE_SIZE;
			int resolution = 24;

			//타일 반지름 설정
			float tileRadius = MapSettings.HALF_TILE;

			//대각선 연결 시 추가 확장 거리
			float diagonalExtension = 0.15f * MapSettings.TILE_SIZE;

			Vector2 inBase = ((Vector2)TileUtils.GetDirectionVector(inDir)).normalized;
			Vector2 outBase = ((Vector2)TileUtils.GetDirectionVector(outDir)).normalized;

			Vector2 inPos = inBase * tileRadius;
			Vector2 outPos = outBase * tileRadius;

			//대각선 판별
			bool inIsDiagonal = Mathf.Abs(inBase.x) > 0.1f && Mathf.Abs(inBase.y) > 0.1f;
			bool outIsDiagonal = Mathf.Abs(outBase.x) > 0.1f && Mathf.Abs(outBase.y) > 0.1f;

			//대각선일 경우 보간 곡선 시작점 조정
			Vector2 inCurveStart = inIsDiagonal ? inPos - (inBase * diagonalExtension) : inPos;
			Vector2 outCurveStart = outIsDiagonal ? outPos - (outBase * diagonalExtension) : outPos;

			if (inIsDiagonal) pathPoints.Add(inPos);

			if (inDir != outDir) {
				float inHandleScale = cornerHandleScale;
				float outHandleScale = cornerHandleScale;

				//급커브(1단계 차이)일 경우 핸들 스케일 축소
				if (Vector2.Dot(inBase, outBase) > 0.1f) {
					inHandleScale = tightCornerHandleScale;
					outHandleScale = tightCornerHandleScale;
				}

				Vector2 handleA = inCurveStart - (inBase * inHandleScale);
				Vector2 handleB = outCurveStart - (outBase * outHandleScale);

				for (int i = 0; i<=resolution; i++) {
					float t = i / (float)resolution;
					pathPoints.Add(BezierUtils.GetPoint(inCurveStart, handleA, handleB, outCurveStart, t));
				}

				if (outIsDiagonal) pathPoints.Add(outPos);
			} else {
				//U턴 처리
				pathPoints.Add(inCurveStart);
				pathPoints.Add(Vector2.zero);
			}
			if (outIsDiagonal) pathPoints.Add(outPos);

			return pathPoints;
		}

		//대각선 교차점 코너 경로 생성
		public List<Vector2> ConstructCornerPath() {
			List<Vector2> pathPoints = new List<Vector2>();

			//타일 경계에 맞춘 코너 반지름 계산
			float cornerRadius = (Mathf.Sqrt(2f) - 1f) * MapSettings.HALF_TILE;

			Vector2 dir = new Vector2(1, 1).normalized;

			Vector2 startPos = -dir * cornerRadius;
			Vector2 endPos = dir * cornerRadius;

			pathPoints.Add(startPos);
			pathPoints.Add(endPos);

			return pathPoints;
		}

		public RoadMeshData ConstructMeshFromPath(List<Vector2> visualPoints, float roadWidth, bool roundEnds, float yOffset = 0f) {
			RoadMeshData data = new RoadMeshData();

			if (visualPoints == null || visualPoints.Count < 2) return data;

			float halfWidth = roadWidth * 0.5f;
			int pointCount = visualPoints.Count;

			int capVerticesCount = roundEnds ? 12 : 0;
			int totalVertices = (pointCount * 2) + capVerticesCount;

			data.vertices = new Vector3[totalVertices];
			data.uvs = new Vector2[totalVertices];
			data.uv2 = new Vector2[totalVertices];

			int bodyTriangles = (pointCount - 1) * 6;
			int capTriangles = roundEnds ? (12 * 3) : 0;
			data.triangles = new int[bodyTriangles + capTriangles];

			float currentLength = 0f;
			
			//정점 및 UV 생성
			for (int i =0; i< pointCount; i++) {
				Vector2 currentPoint = visualPoints[i];
				Vector2 forward = Vector2.zero;

				if (!roundEnds && i == 0) {
					forward = -currentPoint.normalized;
				} else if (!roundEnds && i == pointCount - 1) {
					forward = currentPoint.normalized;
				} else {
					if (i < pointCount - 1) {
						forward += (visualPoints[i + 1] - currentPoint);
					}
					if (i > 0) {
						forward += (currentPoint - visualPoints[i - 1]);
					}
					forward = forward.normalized;
				}

				Vector2 left = new Vector2(-forward.y, forward.x);
				if (i > 0) currentLength += Vector2.Distance(visualPoints[i], visualPoints[i - 1]);

				data.vertices[i * 2] = new Vector3(currentPoint.x + left.x * halfWidth, yOffset, currentPoint.y + left.y * halfWidth);
				data.uvs[i * 2] = new Vector2(0f, currentLength);

				data.vertices[i * 2 + 1] = new Vector3(currentPoint.x - left.x * halfWidth, yOffset, currentPoint.y - left.y * halfWidth);
				data.uvs[i * 2 + 1] = new Vector2(1f, currentLength);

				data.uv2[i * 2] = currentPoint;
				data.uv2[i * 2 + 1] = currentPoint;

				if (roundEnds && i == pointCount - 1) {
					Vector2 endPos = currentPoint;
					float angleStep = 15f;
					int capStartIndex = pointCount * 2;

					for (int capIndex = 1; capIndex <= 12; capIndex++) {
						float rad = -angleStep * capIndex * Mathf.Deg2Rad;
						float cos = Mathf.Cos(rad);
						float sin = Mathf.Sin(rad);
						Vector2 rotatedLeft = new Vector2(left.x * cos - left.y * sin, left.x * sin + left.y * cos);

						data.vertices[capStartIndex + capIndex - 1] = new Vector3(endPos.x + rotatedLeft.x * halfWidth, 0f, endPos.y + rotatedLeft.y * halfWidth);
						data.uvs[capStartIndex + capIndex - 1] = new Vector2(0.5f, currentLength + halfWidth);
						data.uv2[capStartIndex + capIndex - 1] = endPos;
					}
				}
			}

			int triIndex = 0;
			for (int i = 0; i < pointCount - 1; i++) {
				int leftCurrent = i * 2;
				int rightCurrent = i * 2 + 1;
				int leftNext = (i + 1) * 2;
				int rightNext = (i + 1) * 2 + 1;

				data.triangles[triIndex++] = leftCurrent;
				data.triangles[triIndex++] = rightNext;
				data.triangles[triIndex++] = rightCurrent;

				data.triangles[triIndex++] = leftCurrent;
				data.triangles[triIndex++] = leftNext;
				data.triangles[triIndex++] = rightNext;
			}

			if (roundEnds) {
				int capStartIndex = pointCount * 2;
				int centerRightIndex = (pointCount - 1) * 2 + 1;
				int lastLeftIndex = (pointCount - 1) * 2;

				int previousIndex = lastLeftIndex;
				for (int capIndex = 0; capIndex < 12; capIndex++) {
					int currentCapVertex = capStartIndex + capIndex;
					data.triangles[triIndex++] = centerRightIndex;
					data.triangles[triIndex++] = previousIndex;
					data.triangles[triIndex++] = currentCapVertex;
					previousIndex = currentCapVertex;
				}
			}
			return data;
		}

		public RoadMeshData CombineMeshData(List<RoadMeshData> meshList) {
			if (meshList == null || meshList.Count == 0) return new RoadMeshData();

			int totalVerts = 0;
			int totalTris = 0;

			foreach (var mesh in meshList) {
				if (mesh.vertices != null) totalVerts += mesh.vertices.Length;
				if (mesh.triangles != null) totalTris += mesh.triangles.Length;
			}

			RoadMeshData combined = new RoadMeshData();
			combined.vertices = new Vector3[totalVerts];
			combined.uvs = new Vector2[totalVerts];
			combined.uv2 = new Vector2[totalVerts];
			combined.triangles = new int[totalTris];

			int vertOffset = 0;
			int triOffset = 0;

			foreach (var mesh in meshList) {
				if (mesh.vertices == null || mesh.vertices.Length == 0) continue;

				System.Array.Copy(mesh.vertices, 0, combined.vertices, vertOffset, mesh.vertices.Length);
				System.Array.Copy(mesh.uvs, 0, combined.uvs, vertOffset, mesh.uvs.Length);

				if (mesh.uv2 != null && mesh.uv2.Length == mesh.vertices.Length) {
					System.Array.Copy(mesh.uv2, 0, combined.uv2, vertOffset, mesh.uv2.Length);
				} else {
					for (int i = 0; i < mesh.vertices.Length; i++) {
						combined.uv2[vertOffset + i] = Vector2.zero;
					}
				}

				for (int i = 0; i < mesh.triangles.Length; i++) {
					combined.triangles[triOffset + i] = mesh.triangles[i] + vertOffset;
				}

				vertOffset += mesh.vertices.Length;
				triOffset += mesh.triangles.Length;
			}

			return combined;
		}
	}
}
