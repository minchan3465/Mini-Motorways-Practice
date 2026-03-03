using UnityEngine;

namespace Motorways.Rendering {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RoadView : MonoBehaviour {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        private GameObject _outlineObj;
        private MeshFilter _outlineFilter;
        private MeshRenderer _outlineRenderer;

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            
            _outlineObj = new GameObject("Outline");
            _outlineObj.transform.SetParent(this.transform, false);
            _outlineFilter = _outlineObj.AddComponent<MeshFilter>();
            _outlineRenderer = _outlineObj.AddComponent<MeshRenderer>();
        }

        public void Initialize(Material mainMat, Material outlineMat, int sortingOrder) {
            _meshRenderer.sharedMaterial = mainMat;
            _meshRenderer.sortingOrder = sortingOrder;
            
            _outlineRenderer.sharedMaterial = outlineMat;
            _outlineRenderer.sortingOrder = sortingOrder - 1;
        }

        public void SetVisibility(bool isVisible) {
            _meshRenderer.enabled = isVisible;
            _outlineRenderer.enabled = isVisible;
        }

        public void UpdateMesh(RoadTileDefinition definition) {
            if (definition == null || definition.mesh == null) {
                _meshFilter.sharedMesh = null;
                _outlineFilter.sharedMesh = null;
                return;
            }

            // 원본 MeshData에서 Mesh 생성 (또는 캐싱된 메시 사용)
            // 현재 RoadMeshData는 배열 형태이므로 이를 Mesh 객체로 변환하는 과정이 필요할 수 있습니다.
            // 여기서는 단순화를 위해 RoadMeshData에 Unity Mesh 필드가 있다고 가정하거나 새로 생성합니다.
            
            _meshFilter.sharedMesh = CreateMeshFromData(definition.mesh.road);
            _outlineFilter.sharedMesh = CreateMeshFromData(definition.mesh.outline);

            float angle = definition.rotationSteps * 45f;
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }

        private Mesh CreateMeshFromData(RoadMeshData data) {
            if (data == null || data.vertices == null) return null;
            
            Mesh mesh = new Mesh();
            mesh.vertices = data.vertices;
            mesh.uv = data.uvs;
            mesh.triangles = data.triangles;
            
            // UV2, UV3 등 애니메이션 데이터가 필요하다면 여기서 추가 세팅
            if (data.uv2 != null && data.uv2.Length == data.vertices.Length) {
                mesh.uv2 = data.uv2;
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
