using UnityEngine;

namespace Motorways.Models {
	// 원작의 ClockModel.cs 구조를 모방한 시간 데이터 클래스입니다.
	public class ClockModel {
		// 원작 시간 공식: 1시간 = 약 0.833초 (실제 시간)
		public const float SecondsPerHour = 0.8333333333333334f;
		// 1일 = 24시간 * 0.833... = 20초
		public const float SecondsPerDay = 20.0f;
		// 1주 = 7일 * 20초 = 140초
		public const float SecondsPerWeek = 140.0f;

		// 누적 시뮬레이션 시간
		public float Time { get; set; }
		// 확장에 관여하는 시간 (게임이 일시정지 되거나 주말 보상 창이 떴을 때 제어용)
		public float ExpansionTime { get; set; }

		public bool IsPaused { get; set; }
		public bool ExpansionTimeManuallyPaused { get; set; }

		// 현재 몇 시간째인지 반환
		public int Hour => (int)(Time / SecondsPerHour);
		// 현재 며칠째인지 반환 (0부터 시작: 0=월요일, 6=일요일)
		public int Day => Hour / 24;
		// 현재 몇 주차인지 반환 (0부터 시작)
		public int Week => Hour / 168;

		public float FractionalDays => Time / SecondsPerDay;

		public int ExpansionHour => (int)(ExpansionTime / SecondsPerHour);
		public int ExpansionDay => ExpansionHour / 24;
		public int ExpansionWeek => ExpansionHour / 168;

		public void Reset() {
			Time = 0f;
			ExpansionTime = 0f;
			IsPaused = false;
			ExpansionTimeManuallyPaused = false;
		}
	}
}
