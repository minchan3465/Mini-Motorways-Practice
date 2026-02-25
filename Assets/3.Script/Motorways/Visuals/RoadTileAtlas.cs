using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
    using Motorways.Utils;

    [CreateAssetMenu(menuName = "Motorways/Road Tile Atlas")]
    public class RoadTileAtlas : ScriptableObject {
        //인스펙터에서 데이터를 입력받기 위한 구조체.
        [System.Serializable]
        public struct TileBinding {
            [Tooltip("정규화된 방향 마스크 (예: North | South = 직선)")]
            public TileDirection CanonicalSignature;
            public RoadTileDefinition Definition;
		}

        [Header("Mappings")]
        [SerializeField]
        private List<TileBinding> _bindings = new List<TileBinding>();

        private Dictionary<TileDirection, RoadTileDefinition> _cache;

        private void OnEnable() {
            BuildCache();
        }

        public void BuildCache() {
            _cache = new Dictionary<TileDirection, RoadTileDefinition>();
            foreach (var binding in _bindings) {
                if (binding.Definition != null && !_cache.ContainsKey(binding.CanonicalSignature)) {
                    _cache.Add(binding.CanonicalSignature, binding.Definition);
                }
            }
        }

        //시그니처를 받아 메쉬 정의를 반환하는 메서드
        public RoadTileDefinition GetDefinition(RoadSignature signature) {
            if (_cache == null) BuildCache();

            //시그니처의 CanonicalMask(회전된 정규 형태)를 키로 사용
            if (_cache.TryGetValue(signature.CanonicalMask, out RoadTileDefinition def)) {
                return def;
            }

            //매핑된 게 없으면 null 반환 (또는 기본 에러 메쉬)
            return null;
        }
    }
}
