using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Motorways.EdgeLoopOperator
{
	// Token: 0x0200052C RID: 1324
	[NullableContext(2)]
	[Nullable(0)]
	public class Vertex
	{
		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x060022EC RID: 8940 RVA: 0x0008E8A5 File Offset: 0x0008CAA5
		// (set) Token: 0x060022ED RID: 8941 RVA: 0x0008E8AD File Offset: 0x0008CAAD
		public Vertex PairedVertex { get; private set; }

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x060022EE RID: 8942 RVA: 0x0008E8B6 File Offset: 0x0008CAB6
		public bool HasPairedVertex
		{
			get
			{
				return this.PairedVertex != null;
			}
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x0008E8C4 File Offset: 0x0008CAC4
		public Vertex.Proximity GetProximity()
		{
			if (this.isRightAngle)
			{
				if (this.sqrDistanceToClosestConnectedVertex < 0.5f)
				{
					return Vertex.Proximity.Close;
				}
				if (this.sqrDistanceToClosestConnectedVertex <= 4f)
				{
					return Vertex.Proximity.Medium;
				}
				return Vertex.Proximity.Far;
			}
			else
			{
				if (this.sqrDistanceToClosestConnectedVertex < 0.5f)
				{
					return Vertex.Proximity.Close;
				}
				if (this.sqrDistanceToClosestConnectedVertex <= 2f)
				{
					return Vertex.Proximity.Medium;
				}
				return Vertex.Proximity.Far;
			}
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x0008E918 File Offset: 0x0008CB18
		public Vertex(Vector3 newPosition, TopologyType newTopologyType)
		{
			this.position = newPosition;
			this.topologyType = newTopologyType;
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x0008E944 File Offset: 0x0008CB44
		[NullableContext(1)]
		public Vertex(Vertex copyTarget)
		{
			this.position = copyTarget.position;
			this.topologyType = copyTarget.topologyType;
			this.diagonalSectionTerminal = copyTarget.diagonalSectionTerminal;
			this.inDiagonalSection = copyTarget.inDiagonalSection;
			this.cachedMoveVector = copyTarget.cachedMoveVector;
			this.sqrDistanceToClosestConnectedVertex = copyTarget.sqrDistanceToClosestConnectedVertex;
			this.isRightAngle = copyTarget.isRightAngle;
			this.isAcuteAngle = copyTarget.isAcuteAngle;
			this.isComplexCorner = copyTarget.isComplexCorner;
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x0008E9D9 File Offset: 0x0008CBD9
		[NullableContext(1)]
		public static void PairVertices(Vertex a, Vertex b)
		{
			if (a.HasPairedVertex || b.HasPairedVertex)
			{
				Diagnostics.FailAssert(string.Format("Vertex {0} and/or {1} is/are already paired!", a, b), Array.Empty<object>());
				return;
			}
			a.PairedVertex = b;
			b.PairedVertex = a;
		}

		// Token: 0x04001CFF RID: 7423
		public Vector3 position;

		// Token: 0x04001D00 RID: 7424
		public TopologyType topologyType;

		// Token: 0x04001D01 RID: 7425
		public bool diagonalSectionTerminal;

		// Token: 0x04001D02 RID: 7426
		public bool inDiagonalSection;

		// Token: 0x04001D03 RID: 7427
		public Vector3 cachedMoveVector = Vector3.zero;

		// Token: 0x04001D04 RID: 7428
		public bool dontRecalculateMoveVector;

		// Token: 0x04001D05 RID: 7429
		public float sqrDistanceToClosestConnectedVertex = -1f;

		// Token: 0x04001D06 RID: 7430
		public bool isRightAngle;

		// Token: 0x04001D07 RID: 7431
		public bool isAcuteAngle;

		// Token: 0x04001D08 RID: 7432
		public bool isComplexCorner;

		// Token: 0x0200052D RID: 1325
		[NullableContext(0)]
		public enum Proximity
		{
			// Token: 0x04001D0B RID: 7435
			Close,
			// Token: 0x04001D0C RID: 7436
			Medium,
			// Token: 0x04001D0D RID: 7437
			Far
		}
	}
}
