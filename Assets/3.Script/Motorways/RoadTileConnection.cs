using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;

	[Serializable]
	public struct RoadTileConnection : IEquatable<RoadTileConnection>, IComparable<RoadTileConnection> {
		public RoadTileNode input;
		public RoadTileNode output;

		public bool IsUTurn => input.Equals(output);

		public RoadTileConnection(RoadTileNode nodeA, RoadTileNode nodeB) {
			if (nodeA.direction > nodeB.direction) {
				this.input = nodeB;
				this.output = nodeA;
			} else {
				this.input = nodeA;
				this.output = nodeB;
			}
		}
		
		public RoadTileConnection GetRotatedConnection(int steps) {
			return new RoadTileConnection(input.GetRotatedNode(steps), output.GetRotatedNode(steps));
		}

		//--- 정렬 및 비교 구현 ---
		public int CompareTo(RoadTileConnection other) {
			if(input.direction != other.input.direction) {
				return input.direction.CompareTo(other.input.direction);
			}
			return output.direction.CompareTo(other.output.direction);
		}

		public bool Equals(RoadTileConnection other) {
			return input.Equals(other.input) && output.Equals(other.output);
		}

		public override bool Equals(object obj) {
			return obj is RoadTileConnection other && Equals(other);
		}

		public override int GetHashCode() {
			return (input.GetHashCode() * 397) ^ output.GetHashCode();
		}

		public override string ToString() => $"Conn[{input}->{output}]";

	}
}
