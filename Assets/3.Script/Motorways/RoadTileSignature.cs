using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways {
    [Serializable]
    public class RoadTileSignature : IEquatable<RoadTileSignature>, IComparable<RoadTileSignature> {
        [SerializeField]
        private List<RoadTileConnection> _connections = new List<RoadTileConnection>();

        //교차로 조합 생성을 위해 입력/출력 노드를 따로 추적합니다.
        private readonly List<RoadTileNode> _inputNodes = new List<RoadTileNode>();
        private readonly List<RoadTileNode> _outputNodes = new List<RoadTileNode>();

        public IEnumerable<RoadTileConnection> connections => _connections;
        public int Count => _connections.Count;

        public bool IsDeadEnd => _connections.Count == 1 && _connections[0].IsUTurn;

        public void AddConnection(RoadTileConnection connection) {
            if (!_connections.Contains(connection)) {
                _connections.Add(connection);
                _connections.Sort();
            }
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
            for (int i = 0; i < _connections.Count; i++) {
                if (!_connections[i].Equals(other._connections[i])) return false;
            }
            return true;
        }
        public override bool Equals(object obj) => Equals(obj as RoadTileSignature);
        public override int GetHashCode() {
            int hash = 17;
            foreach (var conn in _connections) hash = hash * 31 + conn.GetHashCode();
            return hash;
        }
        public int CompareTo(RoadTileSignature other) {
            if (this._connections.Count != other._connections.Count)
                return this._connections.Count - other._connections.Count;
            for (int i = 0; i < _connections.Count; i++) {
                int cmp = this._connections[i].CompareTo(other._connections[i]);
                if (cmp != 0) return cmp;
            }
            return 0;
        }
    }
}
