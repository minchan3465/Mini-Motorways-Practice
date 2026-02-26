using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways {
    [System.Serializable]
    public class RoadTileSignature : IEquatable<RoadTileSignature>, IComparable<RoadTileSignature> {
        private readonly List<RoadTileConnection> _connections = new List<RoadTileConnection>();

        //교차로 조합 생성을 위해 입력/출력 노드를 따로 추적합니다.
        private readonly List<RoadTileNode> _inputNodes = new List<RoadTileNode>();
        private readonly List<RoadTileNode> _outputNodes = new List<RoadTileNode>();

        public IEnumerable<RoadTileConnection> connections => _connections;
        public int Count => _connections.Count;

        public bool IsDeadEnd => _connections.Count == 1 && _connections[0].IsUTurn;

        public bool AddNode(RoadTileNode newNode) {
            if (_inputNodes.Contains(newNode) || _outputNodes.Contains(newNode)) {
                return false;
            }

            //첫 번째 노드인 경우 (막힌 길 처리)
            if (_inputNodes.Count == 0 && _outputNodes.Count ==0) {
                AddConnection(new RoadTileConnection(newNode, newNode));
                _inputNodes.Add(newNode);
                _outputNodes.Add(newNode);
                return true;
			}

            _inputNodes.Add(newNode);
            _outputNodes.Add(newNode);

            //기존에 막힌 길(U-Turn) 상태였다면, 이제 다른 길이 뚫렸으므로 기존 U-Turn 삭제
            if (IsDeadEnd) {
                _connections.RemoveAt(0);
			}

            //기존 입력 노드들 -> 새 노드로 가는 경로 생성
            foreach (var inputNode in _inputNodes) {
                if(!inputNode.Equals(newNode)) {
                    AddConnection(new RoadTileConnection(inputNode, newNode));
				}
			}

            //새 노드 -> 기존 출력 노드들로 가는 경로 생성
            foreach (var outputNode in _outputNodes) {
                if (!outputNode.Equals(newNode)) {
                    AddConnection(new RoadTileConnection(newNode, outputNode));
                }
            }

            return true;
        }

        private void AddConnection(RoadTileConnection connection) {
            _connections.Add(connection);
            _connections.Sort(); //항상 정렬 상태를 유지하여 시그니처 동치성 보장
        }

        public RoadTileSignature CreateRotatedSignature(int steps) {
            if (steps == 0) return this;

            RoadTileSignature rotatedSig = new RoadTileSignature();
            foreach(var conn in connections) {
                rotatedSig.AddConnection(conn.GetRotatedConnection(steps));
			}
            return rotatedSig;
		}

        //---비교 및 정렬---
        public bool Equals(RoadTileSignature other) {
            if (other == null || _connections.Count != other._connections.Count) return false;

            for(int i = 0; i < _connections.Count; i++) {
                if (!_connections[i].Equals(other._connections[i])) return false;
			}
            return true;
        }
        public override bool Equals(object obj) {
            return Equals(obj as RoadTileSignature);
        }
        public override int GetHashCode() {
            int hash = 17;
            foreach (var conn in _connections) {
                hash = hash * 31 + conn.GetHashCode();
            }
            return hash;
        }

        public int CompareTo(RoadTileSignature other) {
            if (this._connections.Count != other._connections.Count)
                return this._connections.Count - other._connections.Count;
            for(int i = 0; i < _connections.Count; i++) {
                int cmp = this._connections[i].CompareTo(other._connections[i]);
                if (cmp != 0) return cmp;
			}
            return 0;
        }

        public override string ToString() {
            return $"Signature[Count={_connections.Count}]";
        }
    }
}
