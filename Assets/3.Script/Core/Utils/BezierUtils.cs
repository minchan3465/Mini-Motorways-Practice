using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils {
	public static class BezierUtils {
		//베지어 곡선은 전에 정리한게 있었지만... 코드 엎으면서 삭제해버린 ㅠㅠ
		//2차 베지어 곡선 공식 : (1-t)²P1 + 2(1-t)P2 + t²P3
		//3차 베지어 곡선 공식 : (1-t)³P1 + 3(1-t)²P2 + 3(1-t)²P3 + t³P4
		//... 아마 중앙은 n차 베지어 기준,
		//갈수록 제곱이 n, n-1, n-2 .. 되며, 중앙을 지나면 데칼코마니로 다시 늘어남.
		//또한 곱셈은 파스칼 삼각형과 동일하게 생깁니다. (찾아보기)

		//실제 사용할 베지어 공식은 2차가 될 예정. (보통 최대 3차를 쓰고, 2~3차를 붙혀서 곡선을 표현한다고 하네요.)
		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;

			//위의 공식대로 작성한다면.
			Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
			return p;
		}

		//베지어 곡선을 미분한다면, 접선(Tangent, 진행 방향)을 구할 수 있습니다.
		//이는 외적을 사용하는 방법. => 벡터값을 구하기.
		//이걸 왜 사용하냐? ->  도로의 '너비' 방향을 구하기 위해서.
		//즉, 도로의 모습은 직선이나 회선하는 어떠한 벡터가 존재할텐데, 그걸 구하는거다!
		//따라서 이걸 구하는 공식도 있다. (3차 베지어 곡선의 미분)
		//수학적 계산을 하면 되긴 하지만... 귀찮으니 그냥 공식을 적고 외우자.
		//2차 베지어 곡선 미분 공식 : 2(1-t)(P1-P0) + 2t(P2-P1).
		//3차 베지어 곡선 미분 공식 : 3(1-t)²(P1 - P0) + 6(1-t)t(P2 - P1) + 3t²(P3 - P2).
	
		//베지어 곡선의 미분 공식을 적용.
		public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
			Vector3 p = (2 * (1 - t) * (p1 - p0)) + (2 * t * (p2 - p1));
			return p.normalized;
			//노멀라이즈드 = 단위벡터로 변환해서 도출.
			//-> 우리는 방향을 구하고 싶은거지, 벡터값을 구하고 싶은게 아님.
		}
	}
}
