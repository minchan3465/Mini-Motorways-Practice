using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	public static class GroupColor {
		// 각 부위별 색상을 담는 구조체
		public struct ColorSet {
			public Color Base;
			public Color Top;
			public Color Side;

			public ColorSet(Color b, Color t, Color s) {
				Base = b;
				Top = t;
				Side = s;
			}
		}

		// 원작 Los Angeles Colorful 테마 순정 데이터 (보정 전)
		private static readonly ColorSet[] _groupColors = new ColorSet[6] {
			// index 0: 테스트용 (White)
			new ColorSet(
				new Color(0.878f, 0.878f, 0.878f), // Base (#E0E0E0)
				new Color(1.000f, 1.000f, 1.000f), // Top  (#FFFFFF)
				new Color(0.659f, 0.659f, 0.659f)  // Side (#A8A8A8)
			),

			// index 1: Group A (노랑/오렌지)
			new ColorSet(
				new Color(0.988f, 0.749f, 0.357f), // Base (#FCBF5B)
				new Color(1.000f, 0.851f, 0.596f), // Top (#FFD998)
				new Color(0.627f, 0.345f, 0.396f)  // Side (#A05865)
			),

			// index 2: Group B (하늘/블루)
			new ColorSet(
				new Color(0.436f, 0.822f, 0.945f), // Base (#6FD1F1)
				new Color(0.475f, 0.820f, 0.914f), // Top (#79D1E9)
				new Color(0.319f, 0.390f, 0.624f)  // Side (#51639F)
			),

			// index 3: Group C (빨강/핑크)
			new ColorSet(
				new Color(0.933f, 0.208f, 0.293f), // Base (#EE354B)
				new Color(0.945f, 0.420f, 0.424f), // Top (#F16B6C)
				new Color(0.489f, 0.177f, 0.365f)  // Side (#7D2D5D)
			),

			// index 4: Group D (남색/네이비)
			new ColorSet(
				new Color(0.259f, 0.388f, 0.549f), // Base (#42638C)
				new Color(0.416f, 0.565f, 0.729f), // Top (#6A90BA)
				new Color(0.200f, 0.192f, 0.365f)  // Side (#33315D)
			),

			// index 5: Group E (초록/그린)
			new ColorSet(
				new Color(0.384f, 0.800f, 0.525f), // Base (#62CC86)
				new Color(0.490f, 0.859f, 0.604f), // Top (#7DDB9A)
				new Color(0.153f, 0.455f, 0.486f)  // Side (#27897A)
			)
		};

		// 해당 인덱스의 전체 색상 세트를 반환합니다.
		public static ColorSet GetGroupColorSet(int index) {
			if (index < 0 || index >= _groupColors.Length) return _groupColors[0];
			return _groupColors[index];
		}

		// 기존 메서드 유지 (Base 색상만 반환)
		public static Color GetGroupColor(int index) {
			return GetGroupColorSet(index).Base;
		}
	}
}
