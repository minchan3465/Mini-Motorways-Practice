using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Motorways {
	[CreateAssetMenu(fileName = "New SmartTile", menuName = "MiniMotor/SmartTile")]
	public class SmartTile : Tile {
		[Header("Logic Data")]
		[Tooltip("이 타일이 게임 로직상 무엇인가?")]
		public TileLogicType logicType = TileLogicType.Empty;

		[Tooltip("건물 생성 확률 가중치 (0.0 ~ 1.0)")]
		[Range(0f, 1f)]
		public float spawnWeight = 1.0f;

		//필요하다면 타일 생성 시, 초기화 로직 추가 가능
		public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
			return base.StartUp(position, tilemap, go);
		}
	}
}

