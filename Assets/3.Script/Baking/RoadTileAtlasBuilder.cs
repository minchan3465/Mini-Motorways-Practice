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
			//이게 왜 또 있냐고 묻는다면... enum의 값은 비트 마스크로 구별하기 위해 1,2,4,8,16...으로 계산되므로 for문 돌리기 애매함.
		};

		//타일의 경로 모양은 2^8으로, 256가지가 된다.
		//이걸 모두 만들기에는 너무... 너무 많으므로, 모양이 겹치는 Mesh는 추후 회전 작업으로 대입.
		//즉, 경우의 수를 압축 및 중복 제거를 해주기.

		public List<RoadTileSignature> GenerateAllUniqueSignatures() {
			List<RoadTileSignature> uniqueSignatures = new List<RoadTileSignature>();
			HashSet<byte> processedMasks = new HashSet<byte>();

			for (int i = 1; i < 256; i++) { // 1(00000001) 부터 255(11111111)까지 
				byte mask = (byte)i;

				// 1. 비트 마스크 기반 회전 중복 검사 (수학적으로 가장 정확함)
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

				processedMasks.Add(mask); // 고유 마스크 등록

				// 2. 고유 마스크에 해당하는 방향 추출
				List<TileDirection> activeDirs = new List<TileDirection>();
				for (int dirIndex = 0; dirIndex < 8; dirIndex++) {
					if ((mask & (1 << dirIndex)) != 0) activeDirs.Add(AllDirections[dirIndex]);
				}

				// 3. 시그니처 조립 (기존 로직과 동일)
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

			//List<RoadTileSignature> uniqueSignatures = new List<RoadTileSignature>();
			//int totalCombinations = 1 << 8; //이론상 가능한 256가지의 모든 조합 생성.

			////한번만 굽는 작업을 할 예정이니, 256번 작업을 매번하는건 아님...
			//for (int i = 0; i < totalCombinations; i++) {
			//	List<TileDirection> activeDirs = new List<TileDirection>();
			//	for (int dirIndex = 0; dirIndex < 8; dirIndex++) {
			//		if ((i & (1 << dirIndex)) != 0) activeDirs.Add(AllDirections[dirIndex]);
			//	}
			//	if (activeDirs.Count == 0) continue;


			//	RoadTileSignature tempSignature = new RoadTileSignature();

			//	// 조합 로직: 막힌 길이면 U턴 1개, 그 이상이면 모든 활성 방향끼리 명확한 쌍으로 연결
			//	if (activeDirs.Count == 1) {
			//		tempSignature.AddConnection(new RoadTileConnection(new RoadTileNode(activeDirs[0], RoadType.TwoLane), new RoadTileNode(activeDirs[0], RoadType.TwoLane)));
			//	} else {
			//		for (int a = 0; a < activeDirs.Count; a++) {
			//			for (int b = a + 1; b < activeDirs.Count; b++) {
			//				tempSignature.AddConnection(new RoadTileConnection(
			//					new RoadTileNode(activeDirs[a], RoadType.TwoLane),
			//					new RoadTileNode(activeDirs[b], RoadType.TwoLane)));
			//			}
			//		}
			//	}

			//	//회전 중복을 검사합시다.
			//	bool isDuplicate = false;
			//	foreach (RoadTileSignature existingSig in uniqueSignatures) {
			//		for (int step = 0; step < 8; step++) {
			//			if (existingSig.Equals(tempSignature.CreateRotatedSignature(step))) {
			//				isDuplicate = true;
			//				break;
			//			}
			//		}
			//		if (isDuplicate) break;
			//	}
			//	if (!isDuplicate) uniqueSignatures.Add(tempSignature);
			//}
			//return uniqueSignatures;
		}

		public List<Vector2> ConstructPathForConnection(RoadTileConnection connection) {
			//TODO: BezierUtils를 활용하여 Input에서 Output으로 가는 수학적 곡선 도출
			//대각선 Extend 처리, 직각 CornerHandleScale 처리 등 원작 수학 로직 구현

			List<Vector2> pathPoints = new List<Vector2>();

			TileDirection inDir = connection.input.direction;
			TileDirection outDir = connection.output.direction;

			float cornerHandleScale = 0.276f;
			float tightCornerHandleScale = 0.1f;
			int resolution = 24;

			//타일 반경 설정.
			float tileRadius = 0.5f;

			//대각선 연결 시, 추가적으로 조금 직선으로 떧어나올 거리.
			float diagonalExtension = 0.15f;

			//방향 벡터 구하고, 타일 경계의 시작점과 도착점을 계산합니다.
			//TileUtils.GetDirectionVector는 (1,1) 같은 정수 벡터를 반환하므로 반드시 정규화(normalized)가 필요합니다.
			Vector2 inBase = ((Vector2)TileUtils.GetDirectionVector(inDir)).normalized;
			Vector2 outBase = ((Vector2)TileUtils.GetDirectionVector(outDir)).normalized;

			Vector2 inPos = inBase * tileRadius;
			Vector2 outPos = outBase * tileRadius;

			//대각선 판별.
			bool inIsDiagonal = Mathf.Abs(inBase.x) > 0.1f && Mathf.Abs(inBase.y) > 0.1f;
			bool outIsDiagonal = Mathf.Abs(outBase.x) > 0.1f && Mathf.Abs(outBase.y) > 0.1f;

			//대각선이면, 타일 경계에서 바로 곡선 시작 x. 강제로 직선 구간을 밀어넣기.
			Vector2 inCurveStart = inIsDiagonal ? inPos - (inBase * diagonalExtension) : inPos;
			Vector2 outCurveStart = outIsDiagonal ? outPos - (outBase * diagonalExtension) : outPos;

			//연장된 직선 구간의 점을 먼저 경로에 추가합니다.
			if (inIsDiagonal) pathPoints.Add(inPos);

			//입력 방향과 출력 방향에 따른 베지어 제어점 계산.
			if (inDir != outDir) {
				//--일반 교차로 및 코서--
				float inHandleScale = cornerHandleScale;
				float outHandleScale = cornerHandleScale;

				//내적을 통하여, 두 방향 사이의 각도를 유추합니다.
				//1에 가까울수록 예각 -> 타이트한 코너로 판정하여, 핸들 길이 줄이기.
				if (Vector2.Dot(inBase, outBase) > 0.1f) {
					inHandleScale = tightCornerHandleScale;
					outHandleScale = tightCornerHandleScale;
				}

				Vector2 handleA = inCurveStart - (inBase * inHandleScale);
				Vector2 handleB = outCurveStart - (outBase * outHandleScale);

				for (int i = 0; i<=resolution; i++) {
					float t = i / (float)resolution;
					pathPoints.Add(BezierUtils.GetPoint(inCurveStart, handleA, handleB, outCurveStart, t)); //3차 베지어 곡선
				}

				// 막다른 길이 아닐 때만 끝점 연장 추가
				if (outIsDiagonal) pathPoints.Add(outPos);
			} else {
				//--U-Turn (막힌 길)--
				pathPoints.Add(inCurveStart);
				pathPoints.Add(Vector2.zero);
			}
			if (outIsDiagonal) pathPoints.Add(outPos);

			return pathPoints;
		}

		// [추가됨] 대각선 틈새를 메우는 코너 전용 경로 생성 (길이: 약 0.414)
		public List<Vector2> ConstructCornerPath() {
			List<Vector2> pathPoints = new List<Vector2>();

			// 타일 중심(0.5, 0.5)을 잇는 대각선 사이의 빈 공간 길이
			float cornerRadius = (Mathf.Sqrt(2f) - 1f) * 0.5f;

			// 코너 조각은 중심(0,0)을 가로지르는 단순한 직선 경로입니다.
			// 남서(SW)에서 북동(NE) 방향으로 관통하는 기준 경로 하나만 만들면, 
			// 나중에 회전시켜서 북서(NW) <-> 남동(SE) 방향으로도 쓸 수 있습니다.
			Vector2 dir = new Vector2(1, 1).normalized; // 북동쪽 방향 벡터

			Vector2 startPos = -dir * cornerRadius; // 남서쪽 끝
			Vector2 endPos = dir * cornerRadius;    // 북동쪽 끝

			pathPoints.Add(startPos);
			pathPoints.Add(endPos);

			return pathPoints;
		}

		public RoadMeshData ConstructMeshFromPath(List<Vector2> visualPoints, float roadWidth, bool roundEnds, float yOffset = 0f) {    //TODO: visualPoints를 순회하며 좌우 정점(Vertices)과 삼각형(Triangles) 생성
			RoadMeshData data = new RoadMeshData();

			if (visualPoints == null || visualPoints.Count < 2) return data;

			float halfWidth = roadWidth * 0.5f;
			int pointCount = visualPoints.Count;

			int capVerticesCount = roundEnds ? 12 : 0;
			int totalVertices = (pointCount * 2) + capVerticesCount;

			//좌우 정점이 필요하니, 전체 포인트 수의 두배를 늘려줍시다.
			data.vertices = new Vector3[totalVertices];
			data.uvs = new Vector2[totalVertices];

			data.uv2 = new Vector2[totalVertices];//애니메이션용
			Vector2 centerPivot = Vector2.zero;

			//사각형 (Quad) 하나당, 삼각형 2개... ~> 정점 인덱스 6개가 됩니다.
			int bodyTriangles = (pointCount - 1) * 6;
			int capTriangles = roundEnds ? (12 * 3) : 0;
			data.triangles = new int[bodyTriangles + capTriangles];

			float currentLength = 0f;
			
			//정점, UV 계산
			for (int i =0; i< pointCount; i++) {
				Vector2 currentPoint = visualPoints[i];
				Vector2 forward = Vector2.zero;

				//중간 점들은 이전 점과 다음 점을 이용해 방향을 평균화(스무딩) 합니다.
				if (!roundEnds && i == 0) {
					// 시작점: 위치 벡터의 역방향이 곡선의 출발 진행 방향 (inBase 없이 해결)
					forward = -currentPoint.normalized;
				} else if (!roundEnds && i == pointCount - 1) {
					// 끝점: 위치 벡터의 정방향이 곡선의 도착 진행 방향
					forward = currentPoint.normalized;
				} else {
					// 중간 점들 (또는 라운드 캡이 켜진 경우의 양끝점): 이전/다음 점을 이용해 방향 스무딩
					if (i < pointCount - 1) {
						forward += (visualPoints[i + 1] - currentPoint);
					}
					if (i > 0) {
						forward += (currentPoint - visualPoints[i - 1]);
					}
					forward = forward.normalized; // 원작처럼 더한 뒤 정규화
				}

				Vector2 left = new Vector2(-forward.y, forward.x);
				if (i > 0) currentLength += Vector2.Distance(visualPoints[i], visualPoints[i - 1]);

				//왼쪽 정점
				data.vertices[i * 2] = new Vector3(currentPoint.x + left.x * halfWidth, yOffset, currentPoint.y + left.y * halfWidth);
				data.uvs[i * 2] = new Vector2(0f, currentLength); //U는 0 (왼쪽 끝)

				//오른쪽 정점
				data.vertices[i * 2 + 1] = new Vector3(currentPoint.x - left.x * halfWidth, yOffset, currentPoint.y - left.y * halfWidth);
				data.uvs[i * 2 + 1] = new Vector2(1f, currentLength);//U는 1 (오른쪽 끝)

				data.uv2[i * 2] = currentPoint;
				data.uv2[i * 2 + 1] = currentPoint;

				if (roundEnds && i == pointCount - 1) {
					Vector2 endPos = currentPoint;
					float angleStep = 15f;
					int capStartIndex = pointCount * 2;

					for (int capIndex = 1; capIndex <= 12; capIndex++) {
						// 기준점(left)을 회전시켜 부채꼴 정점 위치 도출
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
				int centerRightIndex = (pointCount - 1) * 2 + 1; // 원의 중심 역할 (마지막 우측 정점)
				int lastLeftIndex = (pointCount - 1) * 2;        // 부채꼴 시작점 (마지막 좌측 정점)

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

		// [추가] 여러 개의 RoadMeshData를 하나로 병합하는 함수
		public RoadMeshData CombineMeshData(List<RoadMeshData> meshList) {
			if (meshList == null || meshList.Count == 0) return new RoadMeshData();

			int totalVerts = 0;
			int totalTris = 0;

			// 전체 크기 계산
			foreach (var mesh in meshList) {
				if (mesh.vertices != null) totalVerts += mesh.vertices.Length;
				if (mesh.triangles != null) totalTris += mesh.triangles.Length;
			}

			RoadMeshData combined = new RoadMeshData();
			combined.vertices = new Vector3[totalVerts];
			combined.uvs = new Vector2[totalVerts];
			combined.uv2 = new Vector2[totalVerts];	//애니메이션용...
			combined.triangles = new int[totalTris];

			int vertOffset = 0;
			int triOffset = 0;

			foreach (var mesh in meshList) {
				if (mesh.vertices == null || mesh.vertices.Length == 0) continue;

				// 정점 복사
				System.Array.Copy(mesh.vertices, 0, combined.vertices, vertOffset, mesh.vertices.Length);
				System.Array.Copy(mesh.uvs, 0, combined.uvs, vertOffset, mesh.uvs.Length);

				if (mesh.uv2 != null && mesh.uv2.Length == mesh.vertices.Length) {
					System.Array.Copy(mesh.uv2, 0, combined.uv2, vertOffset, mesh.uv2.Length);
				} else {
					// 방어 코드: 만약 uv2가 없다면 0으로라도 채워줍니다.
					for (int i = 0; i < mesh.vertices.Length; i++) {
						combined.uv2[vertOffset + i] = Vector2.zero;
					}
				}

				// 삼각형 인덱스 복사 (오프셋 적용)
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
