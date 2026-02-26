using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005E2 RID: 1506
	public class MotorwayPolygon
	{
		// Token: 0x06002A18 RID: 10776 RVA: 0x000B7378 File Offset: 0x000B5578
		public MotorwayPolygon(int motorwayId, List<MotorwayPoint> points)
		{
			this.motorwayId = motorwayId;
			this.points = points;
			List<MotorwayEdge> edgeList = new List<MotorwayEdge>();
			int current = 0;
			int previous = points.Count - 1;
			while (current < points.Count)
			{
				MotorwayPoint previousPoint = points[previous];
				MotorwayPoint currentPoint = points[current];
				switch (previousPoint.type)
				{
				case MotorwayPointType.Left:
					if (currentPoint.type == MotorwayPointType.Left || currentPoint.type == MotorwayPointType.LeftEnd)
					{
						goto IL_95;
					}
					goto IL_BF;
				case MotorwayPointType.Right:
					if (currentPoint.type == MotorwayPointType.Right || currentPoint.type == MotorwayPointType.RightEnd)
					{
						goto IL_BA;
					}
					goto IL_BF;
				case MotorwayPointType.LeftEnd:
					if (currentPoint.type == MotorwayPointType.RightEnd)
					{
						goto IL_72;
					}
					if (currentPoint.type != MotorwayPointType.Left)
					{
						goto IL_BF;
					}
					goto IL_95;
				case MotorwayPointType.RightEnd:
					if (currentPoint.type == MotorwayPointType.LeftEnd)
					{
						goto IL_72;
					}
					if (currentPoint.type != MotorwayPointType.Right)
					{
						goto IL_BF;
					}
					goto IL_BA;
				default:
					goto IL_BF;
				}
				IL_EF:
				if (previousPoint.position == currentPoint.position)
				{
					Diagnostics.FailAssert("Edge has same start and end point. current:{0}, previous: {1}", new object[]
					{
						current,
						previous
					});
				}
				Vector2 normal = (currentPoint.position - previousPoint.position).GetNormal().normalized;
				if (this.IsClockwise)
				{
					normal = -normal;
				}
				MotorwayEdgeType edgeType;
				edgeList.Add(new MotorwayEdge(previousPoint, currentPoint, normal, edgeType));
				previous = current++;
				continue;
				IL_72:
				edgeType = MotorwayEdgeType.End;
				goto IL_EF;
				IL_95:
				edgeType = MotorwayEdgeType.Left;
				goto IL_EF;
				IL_BA:
				edgeType = MotorwayEdgeType.Right;
				goto IL_EF;
				IL_BF:
				edgeType = MotorwayEdgeType.End;
				Diagnostics.FailAssert("Unknown edge combination {0}, {1}", new object[]
				{
					previousPoint.type,
					currentPoint.type
				});
				goto IL_EF;
			}
			this.edges = edgeList;
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x06002A19 RID: 10777 RVA: 0x000B7508 File Offset: 0x000B5708
		private bool IsClockwise
		{
			get
			{
				float sum = 0f;
				for (int i = 0; i < this.points.Count; i++)
				{
					Vector2 v = this.points[i].position;
					Vector2 v2 = this.points[(i + 1) % this.points.Count].position;
					sum += (v2.x - v.x) * (v2.y + v.y);
				}
				return (double)sum > 0.0;
			}
		}

		// Token: 0x0400240F RID: 9231
		public readonly int motorwayId;

		// Token: 0x04002410 RID: 9232
		public readonly IReadOnlyList<MotorwayPoint> points;

		// Token: 0x04002411 RID: 9233
		public readonly IReadOnlyList<MotorwayEdge> edges;
	}
}
