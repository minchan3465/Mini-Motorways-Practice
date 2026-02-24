using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	using Motorways.Utils;
	public struct RoadSignature {
		//실제 맵에 존재하는 날것의 연결 상태 (예: 20 -> 동, 남 연결)
		public TileDirection RawMask;

		//에셋 매칭을 위해 정규화된 기준 상태 (예: 5 -> 북, 동 연결)
		//시각화 매니저는 이 값을 Key로 사용하여 메쉬나 스플라인 데이터를 찾습니다.
		public TileDirection CanonicalMask;

		//기준 상태(Canonical)의 에셋을 실제 맵 상태(Raw)로 맞추기 위해 시계방향으로 45도씩 회전해야 하는 횟수 (0 ~ 7)
		public int RotationSteps;

		public RoadSignature(TileDirection raw, TileDirection canonical, int rotationSteps) {
			RawMask = raw;
			CanonicalMask = canonical;
			RotationSteps = rotationSteps;
		}
	}

	public static class RoadSignatureAnalyzer {
		public static RoadSignature Analyze(TileData tileData) {
			byte rawMask = 0;

			//도로 상태를 모두 읽고, 8비트 마스크로 병합.
			TileDirection[] allDirs = (TileDirection[])Enum.GetValues(typeof(TileDirection));
			foreach(TileDirection dir in allDirs) {
				if (dir == TileDirection.None || dir == TileDirection.All) continue;

				RoadState state = tileData.GetRoadState(dir);
				if(state == RoadState.Active || state == RoadState.Pending || state == RoadState.Mothballed) {
					rawMask |= (byte)dir;
				}
			}

			if(rawMask == 0) {
				return new RoadSignature(TileDirection.None, TileDirection.None, 0);
			}

			//정규화
			byte canonicalMask = rawMask;
			int stepsToCanonical = 0;   //Raw 상태를 시계방향으로 몇 번 돌려야하는가?
			byte currentMask = rawMask;

			//45도씩 7번 회전해서 최소값을 찾습니다.
			for(int i = 1; i < 8; i++) {
				currentMask = RotateMaskClockwise(currentMask, 1);
				if(currentMask < canonicalMask) {
					canonicalMask = currentMask;
					stepsToCanonical = i;
				}
			}

			//실제 게임에  적용할 회전값.
			//우리는 최소값 기준으로 만들어진 메쉬를 인스턴스화 한 뒤, 원래 모양으로 되돌리기 위해 회전시켜야 함.
			int requiredRotationToRaw = (8 - stepsToCanonical) % 8;

			return new RoadSignature(
				(TileDirection)rawMask,
				(TileDirection)canonicalMask,
				requiredRotationToRaw
			);
		}

		//비트 마스크 연산.
		private static byte RotateMaskClockwise(byte mask, int steps) {
			steps = steps % 8;
			return (byte)((mask << steps) | (mask >> (8 - steps)));
		}
	}
}

