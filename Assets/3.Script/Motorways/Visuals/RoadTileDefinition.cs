using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
    [CreateAssetMenu(menuName = "Motorways/Road Tile Definition")]
    public class RoadTileDefinition : ScriptableObject {
        [Header("Assets")]
        public Mesh MainMesh;      // 일반 도로 메쉬
        public Mesh BridgeMesh;    // 다리 버전 메쉬 (선택 사항)
        public Mesh TunnelMesh;    // 터널 버전 메쉬 (선택 사항)

        [Header("Settings")]
        public bool IsIntersection; // 교차로 여부 (신호등 로직 등에 사용)
    }
}