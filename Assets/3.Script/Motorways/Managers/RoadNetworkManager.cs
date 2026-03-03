using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Motorways.Models;
	using Motorways.Utils;

	public class RoadNetworkManager : MonoBehaviour {
		public static RoadNetworkManager Instance;

		//������ ���θ� ����.
		public List<Lane> AllLanes { get; private set; } = new List<Lane>();
		private List<Lane> _mothballedLanes = new List<Lane>();
		private HashSet<Lane> _systemLanes = new HashSet<Lane>();	//�ǹ��̳� �������� ����.

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Update() {
			ProcessMothballedLanes();
			if (CityModel.ChangedNodes.Count > 0) {
				TilemapView.Instance.UpdateTiles(CityModel.ChangedNodes);
				CityModel.ChangedNodes.Clear();
			}
		}

		//--- �ܺ� ���� ���� ---
		public void TryBuildRoad(Vector2Int from, Vector2Int to) {
			if (Vector2Int.Distance(from, to) > 1.5f) return;

			Lane existingLane = GetLane(from, to);

			//null�̶�� ���ΰ� ���� ����.
			if (existingLane != null) {
				//���� Mothballed ������ �����̶��.
				if (existingLane.State == RoadState.Mothballed) {
					RestoreMothballedLane(existingLane);
					//�ݴ����� ����.
					Lane opposite = GetLane(to, from);
					if (opposite != null) RestoreMothballedLane(opposite);
				}
				return; //�̹� Ȱ�� ���θ� ����.
			}

			//������ �ȵǾ��ִٸ�.
			//�ڿ� ����.
			if (!ResourceManager.Instance.TryConsumeResource(ItemType.Road, 1)) return;

			CreateLane(from, to);
			CreateLane(to, from);

			//���� ���� �Ǿ���.
		}

		//�ǹ��� �޼���
		public void BuildSystemRoad(Vector2Int from, Vector2Int to, out Lane outLane, out Lane inLane) {
			outLane = new Lane(from, to);
			inLane = new Lane(to, from);
			
			AllLanes.Add(outLane);
			AllLanes.Add(inLane);
			_systemLanes.Add(outLane); //�ý��� ���η� ���
			_systemLanes.Add(inLane);

			MapManager.Instance.ConnectLaneToMap(outLane);
			MapManager.Instance.ConnectLaneToMap(inLane);

			UpdateCornerDataForLane(from, to, isAdding: true);
			UpdateCornerDataForLane(to, from, isAdding: true);

			//CityModel.LatestLaneChangeFrame = Time.frameCount;
			CityModel.ChangedNodes.Add(from);
			CityModel.ChangedNodes.Add(to);
		}
		public void TryRemoveRoad(Vector2Int targetTile) {
			if (MapManager.Instance._grid.TryGetValue(targetTile, out TileData tile)) {
				if (tile.Building != null) return; // �ǹ� ���� ���� �Ұ�!
			}

			List<Lane> connectedLanes = AllLanes.FindAll(lane => lane.StartNode == targetTile || lane.EndNode == targetTile);
			if (connectedLanes.Count == 0) return; //������ġ

			foreach (Lane lane in connectedLanes) {
				if (_systemLanes.Contains(lane)) continue;
				SetLaneToMothballed(lane);
			}
		}
		public void MothballSystemRoad(Lane outLane, Lane inLane) {
			if (outLane != null) {
				SetLaneToMothballed(outLane);
				MapManager.Instance.DisconnectLaneFromMap(outLane);
			}

			if (inLane != null) {
				SetLaneToMothballed(inLane);
				MapManager.Instance.DisconnectLaneFromMap(inLane);
			}
		}

		//---���� ����---
		private void CreateLane(Vector2Int start, Vector2Int end) {
			Lane newLane = new Lane(start, end);
			AllLanes.Add(newLane);

			MapManager.Instance.ConnectLaneToMap(newLane);
			UpdateCornerDataForLane(start, end, isAdding: true);

			CityModel.LatestLaneChangeFrame = Time.frameCount;
			//CityModel.ChangedLanes.Add(newLane);
			CityModel.ChangedNodes.Add(start);
			CityModel.ChangedNodes.Add(end);
		}
		//�ڳ�(Corner) �밢�� ������ ó�� ����
		private void UpdateCornerDataForLane(Vector2Int start, Vector2Int end, bool isAdding) {
		    //1. �밢�� ���� Ȯ�� (x�� y�� ��� �������� �밢��)
		    int dx = end.x - start.x;
		    int dy = end.y - start.y;
		
		    if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1) {
		        //2. �ڳ��� ��ǥ(���� ��� ������ ����)�� �밢�� ���� ����
		        Vector2Int cornerCoord;
		        CornerDiagonalType diagonalType;
		
		        if (dx == 1 && dy == 1) {
		            //SW -> NE ���� (��: 0,0 -> 1,1)
		            //�ڳ� ��ǥ�� ���� �Ʒ� Ÿ���� ��ǥ�� ���󰩴ϴ�.
		            cornerCoord = start;
		            diagonalType = CornerDiagonalType.SW_to_NE;
		        }
		        else if (dx == -1 && dy == -1) {
		            //NE -> SW ���� (��: 1,1 -> 0,0)
		            cornerCoord = end;
		            diagonalType = CornerDiagonalType.SW_to_NE;
		        }
		        else if (dx == 1 && dy == -1) {
		            //NW -> SE ���� (��: 0,1 -> 1,0)
		            //�� ��� �ڳ� ��ǥ�� (start.x, end.y) ��, (0, 0)�� ���� �������Դϴ�.
		            cornerCoord = new Vector2Int(start.x, end.y);
		            diagonalType = CornerDiagonalType.NW_to_SE;
		        }
		        else { //dx == -1 && dy == 1
		            //SE -> NW ���� (��: 1,0 -> 0,1)
		            cornerCoord = new Vector2Int(end.x, start.y);
		            diagonalType = CornerDiagonalType.NW_to_SE;
		        }
		
		        //3. MapManager�� ���� �ڳ� ������ ������Ʈ
		        CornerData corner = MapManager.Instance.GetOrCreateCorner(cornerCoord);
		        if (isAdding) {
		            corner.AddDiagonal(diagonalType);
				} else {
		            corner.RemoveDiagonal(diagonalType);
		        }
		
		        //4. �ֺ� 4�� Ÿ��(�� �ڳʸ� �����ϴ� Ÿ�ϵ�)�� ûũ�� ������Ʈ�ϵ��� ����
		        //�ڳ� �������� ������ �������Ƿ� ������ ������ �ʿ��մϴ�.
		        CityModel.ChangedNodes.Add(cornerCoord);
		        CityModel.ChangedNodes.Add(new Vector2Int(cornerCoord.x + 1, cornerCoord.y));
		        CityModel.ChangedNodes.Add(new Vector2Int(cornerCoord.x, cornerCoord.y + 1));
		        CityModel.ChangedNodes.Add(new Vector2Int(cornerCoord.x + 1, cornerCoord.y + 1));
		    }
		}

		//---���� ���μ���---
		private void SetLaneToMothballed(Lane lane) {
			if (lane.State == RoadState.Mothballed) return;

			lane.State = RoadState.Mothballed;
			_mothballedLanes.Add(lane);

			// TileData 상태 업데이트
			if (MapManager.Instance._grid.TryGetValue(lane.StartNode, out TileData startTile)) {
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);
				startTile.SetRoadState(dir, RoadState.Mothballed);
			}
			
			// CornerData 상태 업데이트 추가
			SetCornerStateForLane(lane.StartNode, lane.EndNode, RoadState.Mothballed);

			CityModel.LatestLaneChangeFrame = Time.frameCount;
			CityModel.ChangedNodes.Add(lane.StartNode);
			CityModel.ChangedNodes.Add(lane.EndNode);
		}
		private void RestoreMothballedLane(Lane lane) {
			if (lane.State == RoadState.Mothballed) {
				lane.State = RoadState.Active;
				_mothballedLanes.Remove(lane);

				// TileData 상태 업데이트
				if (MapManager.Instance._grid.TryGetValue(lane.StartNode, out TileData startTile)) {
					TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);
					startTile.SetRoadState(dir, RoadState.Active);
				}
				
				// CornerData 상태 업데이트 추가
				SetCornerStateForLane(lane.StartNode, lane.EndNode, RoadState.Active);

				CityModel.LatestLaneChangeFrame = Time.frameCount;
				CityModel.ChangedNodes.Add(lane.StartNode);
				CityModel.ChangedNodes.Add(lane.EndNode);
			}
		}

		private void SetCornerStateForLane(Vector2Int start, Vector2Int end, RoadState state) {
			int dx = end.x - start.x;
			int dy = end.y - start.y;
			if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1) {
				Vector2Int cornerCoord;
				CornerDiagonalType diagonalType;
				if (dx == 1 && dy == 1) {
					cornerCoord = start;
					diagonalType = CornerDiagonalType.SW_to_NE;
				} else if (dx == -1 && dy == -1) {
					cornerCoord = end;
					diagonalType = CornerDiagonalType.SW_to_NE;
				} else if (dx == 1 && dy == -1) {
					cornerCoord = new Vector2Int(start.x, end.y);
					diagonalType = CornerDiagonalType.NW_to_SE;
				} else {
					cornerCoord = new Vector2Int(end.x, start.y);
					diagonalType = CornerDiagonalType.NW_to_SE;
				}

				CornerData corner = MapManager.Instance.GetCornerData(cornerCoord);
				if (corner != null) {
					corner.SetState(diagonalType, state);
					CityModel.ChangedNodes.Add(cornerCoord);
				}
			}
		}

		private void ProcessMothballedLanes() {
			if (_mothballedLanes.Count == 0) return;

			for (int i = _mothballedLanes.Count - 1; i >= 0; i--) {
				Lane lane = _mothballedLanes[i];

				if (lane.CanRelease()) {
					FinalizeLaneRemoval(lane);
					_mothballedLanes.RemoveAt(i);
				} else {
					//���� �Ұ��� ��, ������ �ִ� �����鿡�� Hotswap(��ȸ ��û)
					//�׷����� �Ұ����ϸ� �׳� ������ ��...
				}
			}
		}

		//---��¥ ����---
		private void FinalizeLaneRemoval(Lane lane) {
			//�Ա� ���� Mothballed�ε�, �츮�� ������ �ƴ��ݽ�.
			//��, AllLanes�� ���� �����̹Ƿ� ����ó��. (������ False�� ����ǰ�, False�� ���� ��ȯ x)
			bool wasPlayerBuilt = AllLanes.Remove(lane);
			bool isSystem = _systemLanes.Remove(lane); // �ý��� ��ο����� ����

			//�� ������ ����.
			MapManager.Instance.DisconnectLaneFromMap(lane);
			UpdateCornerDataForLane(lane.StartNode, lane.EndNode, isAdding: false);

			CityModel.ChangedNodes.Add(lane.StartNode);
			CityModel.ChangedNodes.Add(lane.EndNode);

			if (wasPlayerBuilt && !isSystem) {
				bool isCanonical = (lane.StartNode.x < lane.EndNode.x) ||
								   (lane.StartNode.x == lane.EndNode.x && lane.StartNode.y < lane.EndNode.y);

				if (isCanonical) ResourceManager.Instance.AddResource(ItemType.Road, 1);
			}
		}
		private Lane GetLane(Vector2Int start, Vector2Int end) {
			return AllLanes.Find(l => l.StartNode == start && l.EndNode == end);
		}
	}
}
