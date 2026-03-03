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

        //���� ���� ��, �ѹ� ȣ��.
        public void Initialize() {
            _signatureToDefinition = new Dictionary<RoadTileSignature, RoadTileDefinition>();

            foreach (var def in definitions) {
                //�� Definition�� ���� �ñ״�ó ������ �������� ��ųʸ��� ����ؾ� �մϴ�.
                //��Ʋ�󽺸� ���� �� Definition ���ο� ���� Signature�� �����صδ� ����� ���� �����մϴ�.
                if (def.signature == null) continue;

                // ���� ���
                _signatureToDefinition[def.signature] = def;

                // [����ȭ/����] 8����(45�� ����) ȸ������ ���� ���� �� ��� ���� �����Ͽ� ĳ��
                for (int step = 0; step < 8; step++) {
                    RoadTileSignature rotatedSignature = def.signature.CreateRotatedSignature(step);

                    // �ߺ� ��� ���� (��Ī�� Ÿ�� ��)
                    if (!_signatureToDefinition.ContainsKey(rotatedSignature)) {
                        // ��Ÿ�ӿ� �� �� �ֵ��� ��ȸ����(8-step)�� ������ Definition ����
                        //int reverseStep = (8 - step) % 8;
                        //_signatureToDefinition.Add(rotatedSignature, def.CreateRotatedDefinition(reverseStep));
                        _signatureToDefinition.Add(rotatedSignature, def.CreateRotatedDefinition(step));
                    }
                }
            }
        }

        public RoadTileDefinition GetCornerDefinition(CornerDiagonalType type) {
            if (cornerMesh == null) return null;
            
            RoadTileDefinition def = new RoadTileDefinition();
            def.mesh = cornerMesh;
            
            if (type == CornerDiagonalType.SW_to_NE) {
                def.rotationSteps = 0;
            } else if (type == CornerDiagonalType.NW_to_SE) {
                // 90도 회전 (1 step = 45도이므로 2 steps)
                def.rotationSteps = 2;
            }
            
            return def;
        }

        public RoadTileDefinition ConstructDefinitionFromSignature(RoadTileSignature signatrue) {
            if (_signatureToDefinition == null) Initialize();

            //���� �ñ״�ó �״�� �˻�
            if(_signatureToDefinition.TryGetValue(signatrue, out var def)) {
                return def;
			}

            return null;
		}
    }
}
