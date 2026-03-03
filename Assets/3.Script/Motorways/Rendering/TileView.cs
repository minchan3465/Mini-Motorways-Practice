using System.Collections.Generic;
using UnityEngine;
using Motorways.Rendering;

namespace Motorways {
    public class TileView : MonoBehaviour {
        public Vector2Int Coordinates { get; private set; }
        
        private RoadView _activeRoadView;
        private RoadView _mothballedRoadView;
        
        private RoadView _activeCornerView;
        private RoadView _mothballedCornerView;
        
        private RoadTileAtlas _atlas;

        public void Initialize(Vector2Int coord, RoadTileAtlas atlas, Material roadMat, Material outlineMat, Material mothballedMat) {
            this.Coordinates = coord;
            this._atlas = atlas;
            this.transform.position = new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f);

            _activeRoadView = CreateRoadView("ActiveRoad", roadMat, outlineMat, 10);
            _mothballedRoadView = CreateRoadView("MothballedRoad", mothballedMat, outlineMat, 5);

            _activeCornerView = CreateRoadView("ActiveCorner", roadMat, outlineMat, 11);
            _activeCornerView.transform.localPosition = new Vector3(0.5f, 0, 0.5f);
            
            _mothballedCornerView = CreateRoadView("MothballedCorner", mothballedMat, outlineMat, 6);
            _mothballedCornerView.transform.localPosition = new Vector3(0.5f, 0, 0.5f);
        }

        private RoadView CreateRoadView(string name, Material mainMat, Material outlineMat, int sortingOrder) {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(this.transform, false);
            RoadView rv = obj.AddComponent<RoadView>();
            rv.Initialize(mainMat, outlineMat, sortingOrder);
            return rv;
        }

        public void Refresh(TileData data) {
            RefreshRoads(data);
            RefreshCorners();
        }

        private void RefreshRoads(TileData data) {
            if (data == null || !data.HasAnyRoad) {
                _activeRoadView.SetVisibility(false);
                _activeRoadView.UpdateMesh(null);
                _mothballedRoadView.SetVisibility(false);
                _mothballedRoadView.UpdateMesh(null);
                return;
            }

            // 1. 활성 도로는 오직 Active 상태인 것들로만 시그니처 구성 (끝부분 마감처리 포함)
            RoadTileSignature activeSig = BuildSignatureByStates(data, new RoadState[] { RoadState.Active });
            if (activeSig != null && activeSig.Count > 0) {
                _activeRoadView.SetVisibility(true);
                _activeRoadView.UpdateMesh(_atlas.ConstructDefinitionFromSignature(activeSig));
            } else {
                _activeRoadView.SetVisibility(false);
                _activeRoadView.UpdateMesh(null);
            }

            // 2. Mothballed 도로는 'Active + Mothballed' 전체 구조를 시그니처로 사용
            // 이렇게 해야 끊기지 않고 연결된 형태(Ghost 형태)로 출력됨
            bool hasMothballed = false;
            for(int i=0; i<8; i++) if(data.RoadStates[i] == RoadState.Mothballed) { hasMothballed = true; break; }

            if (hasMothballed) {
                RoadTileSignature fullSig = BuildSignatureByStates(data, new RoadState[] { RoadState.Active, RoadState.Mothballed });
                _mothballedRoadView.SetVisibility(true);
                _mothballedRoadView.UpdateMesh(_atlas.ConstructDefinitionFromSignature(fullSig));
            } else {
                _mothballedRoadView.SetVisibility(false);
                _mothballedRoadView.UpdateMesh(null);
            }
        }

        private void RefreshCorners() {
            CornerData corner = MapManager.Instance.GetCornerData(Coordinates);
            
            if (corner == null || !corner.HasAnyDiagonal) {
                _activeCornerView.SetVisibility(false);
                _activeCornerView.UpdateMesh(null);
                _mothballedCornerView.SetVisibility(false);
                _mothballedCornerView.UpdateMesh(null);
                return;
            }

            CornerDiagonalType activeDiag = CornerDiagonalType.None;
            CornerDiagonalType mothballedDiag = CornerDiagonalType.None;

            if (corner.HasDiagonal(CornerDiagonalType.SW_to_NE)) {
                RoadState s = corner.GetState(CornerDiagonalType.SW_to_NE);
                if (s == RoadState.Active) activeDiag = CornerDiagonalType.SW_to_NE;
                else if (s == RoadState.Mothballed) mothballedDiag = CornerDiagonalType.SW_to_NE;
            }
            if (corner.HasDiagonal(CornerDiagonalType.NW_to_SE)) {
                RoadState s = corner.GetState(CornerDiagonalType.NW_to_SE);
                if (s == RoadState.Active) activeDiag = CornerDiagonalType.NW_to_SE;
                else if (s == RoadState.Mothballed) mothballedDiag = CornerDiagonalType.NW_to_SE;
            }

            if (activeDiag != CornerDiagonalType.None) {
                _activeCornerView.SetVisibility(true);
                _activeCornerView.UpdateMesh(_atlas.GetCornerDefinition(activeDiag));
            } else {
                _activeCornerView.SetVisibility(false);
                _activeCornerView.UpdateMesh(null);
            }

            if (mothballedDiag != CornerDiagonalType.None) {
                _mothballedCornerView.SetVisibility(true);
                _mothballedCornerView.UpdateMesh(_atlas.GetCornerDefinition(mothballedDiag));
            } else {
                _mothballedCornerView.SetVisibility(false);
                _mothballedCornerView.UpdateMesh(null);
            }
        }

        private RoadTileSignature BuildSignatureByStates(TileData data, RoadState[] targetStates) {
            RoadTileSignature sig = new RoadTileSignature();
            List<TileDirection> dirs = new List<TileDirection>();
            HashSet<RoadState> stateSet = new HashSet<RoadState>(targetStates);

            for (int i = 0; i < 8; i++) {
                if (stateSet.Contains(data.RoadStates[i])) {
                    dirs.Add((TileDirection)(1 << i));
                }
            }

            if (dirs.Count == 0) return null;

            if (dirs.Count == 1) {
                sig.AddConnection(new RoadTileConnection(
                    new RoadTileNode(dirs[0], RoadType.TwoLane), 
                    new RoadTileNode(dirs[0], RoadType.TwoLane)));
            } else {
                for (int a = 0; a < dirs.Count; a++) {
                    for (int b = a + 1; b < dirs.Count; b++) {
                        sig.AddConnection(new RoadTileConnection(
                            new RoadTileNode(dirs[a], RoadType.TwoLane),
                            new RoadTileNode(dirs[b], RoadType.TwoLane)));
                    }
                }
            }
            return sig;
        }
    }
}
