using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils.Geometry;

namespace Motorways.Views
{
	// Token: 0x020005E0 RID: 1504
	public static class MotorwayIntersectionUtil
	{
		// Token: 0x06002A0E RID: 10766 RVA: 0x000B6E10 File Offset: 0x000B5010
		public static bool PolygonIntersectsPolygon(MotorwayPolygon polygonA, MotorwayPolygon polygonB, out List<MotorwayIntersectionUtil.MotorwayIntersection> intersections)
		{
			intersections = new List<MotorwayIntersectionUtil.MotorwayIntersection>();
			foreach (MotorwayEdge polygonAEdge in polygonA.edges)
			{
				MotorwayIntersectionUtil.MotorwayIntersection intersection;
				MotorwayIntersectionUtil.EdgeIntersectsPolygon(polygonB, polygonAEdge, out intersection);
				if (intersection != null)
				{
					intersections.Add(intersection);
				}
			}
			return intersections.Count > 0;
		}

		// Token: 0x06002A0F RID: 10767 RVA: 0x000B6E7C File Offset: 0x000B507C
		private static bool EdgeIntersectsPolygon(MotorwayPolygon polygon, MotorwayEdge edge, out MotorwayIntersectionUtil.MotorwayIntersection intersection)
		{
			Vector2 edgeDirection = edge.to.position - edge.from.position;
			foreach (MotorwayEdge polygonEdge in polygon.edges)
			{
				Vector2 polygonEdgeDirection = polygonEdge.to.position - polygonEdge.from.position;
				LineIntersection.IntersectionInfo intersectionInfo = LineIntersection.LineLineIntersection(polygonEdge.from.position, polygonEdgeDirection, edge.from.position, edgeDirection);
				if (intersectionInfo.type == LineIntersection.IntersectionInfo.IntersectionType.Point)
				{
					intersection = new MotorwayIntersectionUtil.MotorwayIntersection(intersectionInfo.intersection, new ValueTuple<MotorwayEdge, MotorwayEdge>(polygonEdge, edge), new ValueTuple<MotorwayEdgeType, MotorwayEdgeType>(polygonEdge.type, edge.type));
					return true;
				}
			}
			intersection = null;
			return false;
		}

		// Token: 0x06002A10 RID: 10768 RVA: 0x000B6F54 File Offset: 0x000B5154
		public static bool EitherEndEdgeIntersectsBoundingBox(MotorwayGeometryInfo.MotorwayEndEdges motorwayEndEdges, Bounds boundingBox)
		{
			AxisAlignedBoundingBox alignedBoundingBox = new AxisAlignedBoundingBox(boundingBox.min, boundingBox.max);
			return alignedBoundingBox.IntersectWithLine(motorwayEndEdges.start.from, motorwayEndEdges.start.to) || alignedBoundingBox.IntersectWithLine(motorwayEndEdges.end.from, motorwayEndEdges.end.to);
		}

		// Token: 0x06002A11 RID: 10769 RVA: 0x000B6FBC File Offset: 0x000B51BC
		public static bool EndEdgeIntersectsPolygon(MotorwayPolygon motorwayPolygon, MotorwayGeometryInfo.MotorwayEndEdge edge, AxisAlignedBoundingBox boundingBox)
		{
			LineIntersection.IntersectionInfo? intersectionInfo;
			return boundingBox.IntersectWithLine(edge.from, edge.to) && (MotorwayIntersectionUtil.LineIntersectsMotorwayPolygon(motorwayPolygon, edge.from, edge.to, out intersectionInfo) || MotorwayIntersectionUtil.PointIsInsidePolygon(motorwayPolygon, edge.from) || MotorwayIntersectionUtil.PointIsInsidePolygon(motorwayPolygon, edge.to));
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x000B7014 File Offset: 0x000B5214
		private static bool PointIsInsidePolygon(MotorwayPolygon polygon, Vector2 point)
		{
			IReadOnlyList<MotorwayPoint> points = polygon.points;
			bool inside = false;
			int current = 0;
			int previous = points.Count - 1;
			while (current < points.Count)
			{
				bool flag = points[current].position.y > point.y != points[previous].position.y > point.y;
				bool testB = point.x < (points[previous].position.x - points[current].position.x) * (point.y - points[current].position.y) / (points[previous].position.y - points[current].position.y) + points[current].position.x;
				if (flag && testB)
				{
					inside = !inside;
				}
				previous = current++;
			}
			return inside;
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x000B710C File Offset: 0x000B530C
		private static bool LineIntersectsMotorwayPolygon(MotorwayPolygon polygon, Vector2 from, Vector2 to, out LineIntersection.IntersectionInfo? intersectionInfo)
		{
			Vector2 direction = to - from;
			foreach (MotorwayEdge polygonEdge in polygon.edges)
			{
				Vector2 polygonEdgeDirection = polygonEdge.to.position - polygonEdge.from.position;
				LineIntersection.IntersectionInfo intersection = LineIntersection.LineLineIntersection(polygonEdge.from.position, polygonEdgeDirection, from, direction);
				if (intersection.type == LineIntersection.IntersectionInfo.IntersectionType.Point)
				{
					intersectionInfo = new LineIntersection.IntersectionInfo?(intersection);
					return true;
				}
			}
			intersectionInfo = null;
			return false;
		}

		// Token: 0x020005E1 RID: 1505
		public class MotorwayIntersection
		{
			// Token: 0x06002A14 RID: 10772 RVA: 0x000B71B4 File Offset: 0x000B53B4
			public MotorwayIntersection(Vector2 point, ValueTuple<MotorwayEdge, MotorwayEdge> edges, ValueTuple<MotorwayEdgeType, MotorwayEdgeType> type)
			{
				this.point = point;
				this.edges = edges;
				this.type = type;
				if (this.edges.Item1.type == MotorwayEdgeType.End || this.edges.Item2.type == MotorwayEdgeType.End)
				{
					this.isSeparatingOrJoining = false;
					return;
				}
				Vector2 edgeATangentDirection = (this.edges.Item1.to.position - this.edges.Item1.from.position).normalized;
				Vector2 edgeBTangentDirection = (this.edges.Item2.to.position - this.edges.Item2.from.position).normalized;
				Vector2 lhs = (Vector2.Dot(edgeATangentDirection, this.edges.Item2.normal) > 0f) ? edgeATangentDirection : (-edgeATangentDirection);
				Vector2 outsideEdgeBDirection = (Vector2.Dot(edgeBTangentDirection, this.edges.Item1.normal) > 0f) ? edgeBTangentDirection : (-edgeBTangentDirection);
				float separatingOrJoiningAngle = Vector2.Dot(lhs, outsideEdgeBDirection);
				this.isSeparatingOrJoining = (separatingOrJoiningAngle > 0f);
			}

			// Token: 0x06002A15 RID: 10773 RVA: 0x000B72E0 File Offset: 0x000B54E0
			public bool Equals(MotorwayIntersectionUtil.MotorwayIntersection other)
			{
				return this.point.Equals(other.point) && this.type.Equals(other.type);
			}

			// Token: 0x06002A16 RID: 10774 RVA: 0x000B731C File Offset: 0x000B551C
			public override bool Equals(object obj)
			{
				MotorwayIntersectionUtil.MotorwayIntersection other = obj as MotorwayIntersectionUtil.MotorwayIntersection;
				return other != null && this.Equals(other);
			}

			// Token: 0x06002A17 RID: 10775 RVA: 0x000B733C File Offset: 0x000B553C
			public override int GetHashCode()
			{
				return this.point.GetHashCode() * 397 ^ this.type.GetHashCode();
			}

			// Token: 0x0400240B RID: 9227
			public readonly Vector2 point;

			// Token: 0x0400240C RID: 9228
			public readonly ValueTuple<MotorwayEdgeType, MotorwayEdgeType> type;

			// Token: 0x0400240D RID: 9229
			[TupleElementNames(new string[]
			{
				"A",
				"B"
			})]
			public readonly ValueTuple<MotorwayEdge, MotorwayEdge> edges;

			// Token: 0x0400240E RID: 9230
			public readonly bool isSeparatingOrJoining;
		}
	}
}
