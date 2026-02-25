using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	using Motorways.Utils;

	public static class RoadSignatureAnalyzer {
		public static RoadSignature Analyze(TileData tileData) {
			byte rawMask = 0;
			List<RoadTileNode> activeNodes = new List<RoadTileNode>();

			//도로 상태를 모두 읽고, 8비트 마스크로 병합.
			TileDirection[] allDirs = (TileDirection[])Enum.GetValues(typeof(TileDirection));
			foreach (TileDirection dir in allDirs) {
				if (dir == TileDirection.None || dir == TileDirection.All) continue;

				RoadState state = tileData.GetRoadState(dir);
				if (state == RoadState.Active || state == RoadState.Pending || state == RoadState.Mothballed) {
					rawMask |= (byte)dir;

					// TODO: 향후 TileData에서 실제 RoadType과 MotorwayId를 가져오도록 수정해야 합니다.
					RoadType type = RoadType.TwoLane;
					int motorwayId = -1;

					activeNodes.Add(new RoadTileNode(dir, type, motorwayId));
				}
			}

			if (rawMask == 0 || activeNodes.Count == 0) {
				return new RoadSignature(TileDirection.None, TileDirection.None, 0, new List<RoadConnection>());
			}

			//정규화
			byte canonicalMask = rawMask;
			int stepsToCanonical = 0;   //Raw 상태를 시계방향으로 몇 번 돌려야하는가?
			byte currentMask = rawMask;

			//~~45도씩 7번 회전해서 최소값을 찾습니다.~~
			//--> 45도로 회전하니까, 대각선 길이가 부족함에도 불구하고 직선 형태의 도로를 사용함.
			//따라서 90도씩 회전하되, 상하좌우 길이 0.5 도로와 대각선 길이 0.707 (피타고라스) 두가지 유형으로 나누기.
			for(int i = 1; i < 4; i++) {
				currentMask = RotateMaskClockwise(currentMask, 2);
				if(currentMask < canonicalMask) {
					canonicalMask = currentMask;
					stepsToCanonical = i * 2;
				}
			}

			//실제 게임에  적용할 회전값.
			//우리는 최소값 기준으로 만들어진 메쉬를 인스턴스화 한 뒤, 원래 모양으로 되돌리기 위해 회전시켜야 함.
			int requiredRotationToRaw = (8 - stepsToCanonical) % 8;
			TileDirection canonicalDir = (TileDirection)canonicalMask;

			//노드들을 정규화(Canonical) 상태로 회전
			List<RoadTileNode> canonicalNodes = new List<RoadTileNode>();
			foreach (var node in activeNodes) {
				canonicalNodes.Add(node.Rotate(stepsToCanonical));
			}

			//--- 분류(Classification) 후 생성(Generation) ---

			//1. 표준 상태(Canonical)에서의 연결선 리스트 생성
			List<RoadConnection> standardConnections = GenerateStandardConnections(canonicalNodes);

			//2. 표준 연결선들에 실제 회전값을 적용하여 최종 연결선 생성
			List<RoadConnection> finalConnections = new List<RoadConnection>();
			foreach (var conn in standardConnections) {
				finalConnections.Add(conn.Rotate(requiredRotationToRaw));
			}

			return new RoadSignature(
				(TileDirection)rawMask,
				canonicalDir,
				requiredRotationToRaw,
				finalConnections
			);
		}

		//비트 마스크 연산.
		private static byte RotateMaskClockwise(byte mask, int steps) {
			steps = steps % 8;
			return (byte)((mask << steps) | (mask >> (8 - steps)));
		}

		private static List<RoadConnection> GenerateStandardConnections(List<RoadTileNode> canonicalNodes) {
			List<RoadConnection> connections = new List<RoadConnection>();

			if (canonicalNodes.Count == 1) {
				// 1. 막다른 길: 가장자리 노드 -> 중앙(None) 노드
				RoadTileNode edgeNode = canonicalNodes[0];
				RoadTileNode centerNode = new RoadTileNode(TileDirection.None, edgeNode.Type, edgeNode.MotorwayId);
				connections.Add(new RoadConnection(edgeNode, centerNode));
			} else if (canonicalNodes.Count == 2) {
				// 2. 일반 도로(직선/코너): 가장자리 노드 -> 가장자리 노드
				connections.Add(new RoadConnection(canonicalNodes[0], canonicalNodes[1]));
			} else if (canonicalNodes.Count > 2) {
				// 3. 교차로(3방향 이상): 모든 가장자리 노드 -> 중앙(Hub)으로 연결
				foreach (RoadTileNode node in canonicalNodes) {
					RoadTileNode centerNode = new RoadTileNode(TileDirection.None, node.Type, node.MotorwayId);
					connections.Add(new RoadConnection(node, centerNode));
				}
			}

			return connections;
		}
	}
}

