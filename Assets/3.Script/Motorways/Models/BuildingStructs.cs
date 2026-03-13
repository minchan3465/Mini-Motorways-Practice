using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models { 
	[System.Serializable]
	public class BuildingLayout {
		public Vector2Int Footprint;    //크기
		public List<TileDirection> Driveways;	//가능한 진입로 방향들.
        public Vector2Int LocalEntrance;
    }

    [System.Serializable]
    public class ScheduledBuilding {
        public BuildingType Type;
        public int GroupIndex;       //색상
        public float SpawnTime;      //등장 예정 시간 (게임 시간 기준)
        public int SpawnAttempts;    //실패 횟수 (나중에 난이도 조절용) << ?

        //튜토리얼 등을 위한 강제 지정 옵션 (필요시 사용)
        public bool UseFixedPosition;
        public Vector2Int FixedPosition;
    }
}
