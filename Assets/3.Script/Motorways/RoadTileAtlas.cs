using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
    [CreateAssetMenu(fileName = "New RoadTileAtlas", menuName = "Motorways/Road Tile Atlas")]
    public class RoadTileAtlas : ScriptableObject {
        //[HideInInspector]
        public List<RoadTileDefinition> definitions = new List<RoadTileDefinition>();

        public RoadTileMesh cornerMesh;

        private Dictionary<RoadTileSignature, RoadTileDefinition> _signatureToDefinition;

        //게임 시작 시, 한번 호출.
        public void Initialize() {
            _signatureToDefinition = new Dictionary<RoadTileSignature, RoadTileDefinition>();

            foreach (var def in definitions) {
                //각 Definition이 가진 시그니처 정보를 바탕으로 딕셔너리에 등록해야 합니다.
                //아틀라스를 구울 때 Definition 내부에 원본 Signature를 저장해두는 방식이 가장 안전합니다.
                if (def.signature == null) continue;

                // 원본 등록
                _signatureToDefinition[def.signature] = def;

                // [최적화/수정] 8방향(45도 단위) 회전본을 게임 시작 시 모두 사전 연산하여 캐싱
                for (int step = 0; step < 8; step++) {
                    RoadTileSignature rotatedSignature = def.signature.CreateRotatedSignature(step);

                    // 중복 등록 방지 (대칭형 타일 등)
                    if (!_signatureToDefinition.ContainsKey(rotatedSignature)) {
                        // 런타임에 쓸 수 있도록 역회전값(8-step)을 적용한 Definition 생성
                        //int reverseStep = (8 - step) % 8;
                        //_signatureToDefinition.Add(rotatedSignature, def.CreateRotatedDefinition(reverseStep));
                        _signatureToDefinition.Add(rotatedSignature, def.CreateRotatedDefinition(step));
                    }
                }
            }
        }

        public RoadTileDefinition ConstructDefinitionFromSignature(RoadTileSignature signatrue) {
            if (_signatureToDefinition == null) Initialize();

            //원본 시그니처 그대로 검색
            if(_signatureToDefinition.TryGetValue(signatrue, out var def)) {
                return def;
			}

            return null;
		}
    }
}
