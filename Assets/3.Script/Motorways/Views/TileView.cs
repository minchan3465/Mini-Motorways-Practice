using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Views {
	public class TileView : MonoBehaviour {
		public Vector2Int Coordinates { get; private set; }

		private RoadView _activeRoadView;
		private RoadView _mothballedRoadView;

		//코너용 뷰 추가
		private RoadView _activeCornerView;
		private RoadView _mothballedCornerView;

		private RoadTileAtlas _atlas;

		public void Initialize(Vector2Int coord, RoadTileAtlas atlas, Material roadMat, Material outlineMat, Material mothballedMat, Material bridgeOutlineMat = null) {
			this.Coordinates = coord;
			this._atlas = atlas;
			//타일 중심 좌표 설정
			this.transform.position = new Vector3(coord.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, coord.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);

			// 물 타일인지 검사하여 다리 전용 아웃라인 설정
			Material activeOutline = outlineMat;
			TileData data = MapManager.Instance.GetTileData(coord);
			if (data != null && data.type == TileLogicType.Water && bridgeOutlineMat != null) {
				activeOutline = bridgeOutlineMat;
			}

			//1.활성 도로용 View 생성
			//2.Mothballed 도로용 View 생성
			_activeRoadView = CreateRoadView("ActiveRoad", roadMat, activeOutline, 10);
			_activeRoadView.transform.localPosition = new Vector3(0, 0.01f, 0);

			_mothballedRoadView = CreateRoadView("MothballedRoad", mothballedMat, activeOutline, 5);

			//3. 코너용 View 생성 (위치는 타일 우측 하단 모서리)
			_activeCornerView = CreateRoadView("ActiveCorner", roadMat, activeOutline, 11);
			_activeCornerView.transform.localPosition = new Vector3(MapSettings.HALF_TILE, 0.01f, MapSettings.HALF_TILE);

			_mothballedCornerView = CreateRoadView("MothballedCorner", mothballedMat, activeOutline, 6);
			_mothballedCornerView.transform.localPosition = new Vector3(MapSettings.HALF_TILE, 0, MapSettings.HALF_TILE);
		}

		private RoadView CreateRoadView(string name, Material mainMat, Material outlineMat, int sortingOrder) {
			GameObject obj = new GameObject(name);
			obj.transform.SetParent(this.transform, false);
			RoadView rv = obj.AddComponent<RoadView>();
			rv.Initialize(mainMat, outlineMat, sortingOrder);
			return rv;
		}

		public void Refresh(TileData data) {
			//도로 리프레시
			RefreshRoads(data);

			//코너 리프레시
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

			//1. Active View: 오직 Active 상태인 도로만 포함 (Mothballed와 단절되어 보임)
			RoadTileSignature activeSig = BuildSignature(data, false);
			if (activeSig != null && activeSig.Count > 0) {
				_activeRoadView.SetVisibility(true);
				_activeRoadView.UpdateMesh(_atlas.ConstructDefinitionFromSignature(activeSig));
			} else {
				_activeRoadView.SetVisibility(false);
				_activeRoadView.UpdateMesh(null);
			}

			//2. Mothballed View: Active + Mothballed를 모두 포함하여 연결 유지
			//타일에 Mothballed 도로가 하나라도 존재할 때만 활성화
			bool hasMothballedInTile = false;
			for (int i = 0; i < 8; i++) {
				if (data.RoadStates[i] == RoadState.Mothballed) {
					hasMothballedInTile = true;
					break;
				}
			}

			if (hasMothballedInTile) {
				RoadTileSignature fullSig = BuildSignature(data, true);
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

			//코너 리프레시 로직
			UpdateCornerView(_activeCornerView, corner, RoadState.Active);
			UpdateCornerView(_mothballedCornerView, corner, RoadState.Mothballed);
		}

		private void UpdateCornerView(RoadView view, CornerData corner, RoadState targetState) {
			CornerDiagonalType diag = CornerDiagonalType.None;
			if (corner.HasDiagonal(CornerDiagonalType.SW_to_NE) && corner.GetState(CornerDiagonalType.SW_to_NE) == targetState) {
				diag = CornerDiagonalType.SW_to_NE;
			} else if (corner.HasDiagonal(CornerDiagonalType.NW_to_SE) && corner.GetState(CornerDiagonalType.NW_to_SE) == targetState) {
				diag = CornerDiagonalType.NW_to_SE;
			}

			if (diag != CornerDiagonalType.None) {
				view.SetVisibility(true);
				view.UpdateMesh(_atlas.GetCornerDefinition(diag));
			} else {
				view.SetVisibility(false);
				view.UpdateMesh(null);
			}
		}

		private RoadTileSignature BuildSignature(TileData data, bool includeMothballed) {
			RoadTileSignature sig = new RoadTileSignature();
			List<TileDirection> dirs = new List<TileDirection>();

			for (int i = 0; i < 8; i++) {
				RoadState s = data.RoadStates[i];
				//includeMothballed가 true면 Active와 Mothballed 모두 포함, false면 Active만 포함
				if (s == RoadState.Active || (includeMothballed && s == RoadState.Mothballed)) {
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
