using UnityEngine;

namespace Motorways {
    public static class MapSettings {
        public const float TILE_SIZE = 2f;
        public const float HALF_TILE = TILE_SIZE / 2f;

        // 원작 Los Angeles 테마 기준 그룹별 색상 코드 (Group Index 0 ~ 4)
        public static readonly Color[] BuildingGroupColors = new Color[] {
            new Color(0.988f, 0.749f, 0.357f, 1f), // Group 0 (A): #FCBF5B
            new Color(0.436f, 0.822f, 0.945f, 1f), // Group 1 (B): #6FD1F1
            new Color(0.933f, 0.208f, 0.293f, 1f), // Group 2 (C): #EE354B
            new Color(0.259f, 0.388f, 0.549f, 1f), // Group 3 (D): #42638C
            new Color(0.384f, 0.800f, 0.525f, 1f)  // Group 4 (E): #62CC86
        };

        public static Color GetGroupColor(int index) {
            if (index < 0 || index >= BuildingGroupColors.Length) return Color.white;
            return BuildingGroupColors[index];
        }
    }
}
