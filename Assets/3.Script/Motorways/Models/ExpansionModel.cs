using UnityEngine;

namespace Motorways.Models {
	public class ExpansionModel {
		// 원작 기준 줌 크기
		public float StartSize = 15f;
		public float EndSize = 25f;
		public float DurationDays = 50f; // 50일에 걸쳐 확장 (원작 기본값 예시)
		public float DelayDays = 7f;    // 2일 후부터 확장 시작

		// 현재 계산된 타겟 줌 (MapManager 등에서 참조)
		public float CurrentTargetZoom = 7.5f;
		
		// 플레이 영역 확장 관련
		public RectInt InitialPlayableArea = new RectInt(-9, -6, 18, 12);
		public RectInt MaxPlayableArea = new RectInt(-15, -10, 30, 20);

		public void Reset() {
			CurrentTargetZoom = StartSize;
		}
	}
}
