using UnityEngine;

namespace Motorways.Views {
    //원작의 RoadView와 유사하게, 단일 타일의 도로 및 아웃라인 메시를 실제로 렌더링하는 클래스입니다.
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RoadView : MonoBehaviour {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        //외곽선(Outline)은 별도의 자식 오브젝트로 생성하여 관리합니다.
        private GameObject _outlineObj;
        private MeshFilter _outlineFilter;
        private MeshRenderer _outlineRenderer;

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            
            //외곽선 오브젝트 생성 및 설정
            _outlineObj = new GameObject("Outline");
            _outlineObj.transform.SetParent(this.transform, false);
            _outlineFilter = _outlineObj.AddComponent<MeshFilter>();
            _outlineRenderer = _outlineObj.AddComponent<MeshRenderer>();
        }

        
        //초기 재질 및 렌더링 순서를 설정합니다.
        public void Initialize(Material mainMat, Material outlineMat, int sortingOrder) {
            _meshRenderer.sharedMaterial = mainMat;
            _meshRenderer.sortingOrder = sortingOrder;
            
            _outlineRenderer.sharedMaterial = outlineMat;
            _outlineRenderer.sortingOrder = sortingOrder - 1; //아웃라인은 항상 본체 뒤에 위치
        }

        public void SetVisibility(bool isVisible) {
            _meshRenderer.enabled = isVisible;
            _outlineRenderer.enabled = isVisible;
        }

        
        //RoadTileDefinition 정보를 바탕으로 Mesh와 회전값을 업데이트합니다.
        public void UpdateMesh(RoadTileDefinition definition) {
            if (definition == null || definition.mesh == null) {
                _meshFilter.sharedMesh = null;
                _outlineFilter.sharedMesh = null;
                return;
            }

            //RoadMeshData를 실제 Unity Mesh 객체로 변환하여 할당합니다.
            _meshFilter.sharedMesh = CreateMeshFromData(definition.mesh.road);
            _outlineFilter.sharedMesh = CreateMeshFromData(definition.mesh.outline);

            //45도(1 step) 단위의 회전 적용
            float angle = definition.rotationSteps * 45f;
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }

        
        //커스텀 RoadMeshData 구조체를 Unity 메시 데이터로 변환합니다.
        private Mesh CreateMeshFromData(RoadMeshData data) {
            if (data == null || data.vertices == null) return null;
            
            Mesh mesh = new Mesh();
            mesh.vertices = data.vertices;
            mesh.uv = data.uvs;
            mesh.triangles = data.triangles;
            
            //애니메이션이나 셰이더 효과용 추가 UV 데이터 처리
            if (data.uv2 != null && data.uv2.Length == data.vertices.Length) {
                mesh.uv2 = data.uv2;
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
