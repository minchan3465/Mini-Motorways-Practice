using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
    [CreateAssetMenu(fileName = "RoadVisualSettings", menuName = "Motorways/Road Visual Settings")]
    public class RoadVisualSettings : ScriptableObject {
        [Header("Road Geometry")]
        [Tooltip("도로의 실제 폭 (차량이 다니는 아스팔트 영역)")]
        public float RoadWidth = 0.6f;

        [Tooltip("도로 테두리(외곽선)의 폭")]
        public float OutlineWidth = 0.1f;

        [Tooltip("끝부분(DeadEnd)을 둥글게 처리할 때 사용할 버텍스 개수")]
        public int CapResolution = 8;

        [Header("Animation")]
        [Tooltip("도로가 생성될 때 걸리는 시간")]
        public float AppearDuration = 0.3f;
    }
}