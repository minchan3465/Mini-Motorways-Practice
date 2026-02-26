using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Utility
{
	// Token: 0x02000462 RID: 1122
	public static class Geometry
	{
		// Token: 0x06001C16 RID: 7190 RVA: 0x00067A70 File Offset: 0x00065C70
		public static bool TryLineSegmentIntersection(LineSegment line0, LineSegment line1, out Vector2 intersection, bool extendToForceIntersection = false)
		{
			intersection = Vector2.zero;
			Vector2 r = line0.Direction * line0.Length;
			Vector2 s = line1.Direction * line1.Length;
			float rCrossS = r.Cross(s);
			float qpCrossS = (line1.Start - line0.Start).Cross(s);
			float qpCrossR = (line1.Start - line0.Start).Cross(r);
			if (rCrossS == 0f)
			{
				if (qpCrossR != 0f)
				{
					return false;
				}
				float rDotR = Vector2.Dot(r, r);
				float t0 = Vector2.Dot(r, line1.Start - line0.Start) / rDotR;
				float t = Vector2.Dot(r, line1.Start + s - line0.Start) / rDotR;
				if ((t0 < 0f && t < 0f) || (t0 > 1f && t > 1f))
				{
					return false;
				}
				t0 = Mathf.Clamp01(t0);
				t = Mathf.Clamp01(t);
				intersection = line0.Start + r * (t0 + t) * 0.5f;
				return true;
			}
			else
			{
				float t2 = qpCrossS / rCrossS;
				float u = qpCrossR / rCrossS;
				if (extendToForceIntersection || (Mathf.Approximately(t2, Mathf.Clamp01(t2)) && Mathf.Approximately(u, Mathf.Clamp01(u))))
				{
					intersection = line0.Start + t2 * line0.Direction * line0.Length;
					return true;
				}
				return false;
			}
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x00067C10 File Offset: 0x00065E10
		public static Vector2 GetExtrudedLineSegmentIntersection(LineSegment line0, LineSegment line1, float extrusion)
		{
			if (line0.IsNull)
			{
				return line1.Start + line1.Normal * -extrusion;
			}
			if (line1.IsNull)
			{
				return line0.End + line0.Normal * -extrusion;
			}
			if (line0.Direction == line1.Direction)
			{
				return line0.End + line0.Normal * -extrusion;
			}
			if (extrusion == 0f)
			{
				return line0.End;
			}
			Vector2 line0Offset = line0.Normal * -extrusion;
			Vector2 line1Offset = line1.Normal * -extrusion;
			LineSegment line2 = new LineSegment(line0.Start + line0Offset, line0.End + line0Offset);
			LineSegment line1Extrusion = new LineSegment(line1.Start + line1Offset, line1.End + line1Offset);
			Vector2 intersection;
			if (Geometry.TryLineSegmentIntersection(line2, line1Extrusion, out intersection, true))
			{
				return intersection;
			}
			Vector2 midNormal = ((line0.Normal + line1.Normal) * 0.5f).normalized * -extrusion;
			return line0.End + midNormal;
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x00067D50 File Offset: 0x00065F50
		public static Geometry.CircleLineIntersection TryCircleLineSegmentIntersection(Circle circle, LineSegment lineSegment)
		{
			Geometry.CircleLineIntersection intersections = default(Geometry.CircleLineIntersection);
			float startToClosestPoint = Vector2.Dot(circle.Origin - lineSegment.Start, lineSegment.Direction);
			Vector2 closestPoint = lineSegment.GetPosition(startToClosestPoint);
			float distance = (circle.Origin - closestPoint).magnitude;
			if (Mathf.Approximately(distance, circle.Radius))
			{
				if (startToClosestPoint >= 0f && startToClosestPoint <= lineSegment.Length)
				{
					intersections.count = 1;
					intersections.first = closestPoint;
					return intersections;
				}
			}
			else if (distance < circle.Radius)
			{
				float sagittaLength = circle.Radius - distance;
				float intersectionOffset = Mathf.Sqrt(8f * circle.Radius * sagittaLength - 4f * sagittaLength * sagittaLength) * 0.5f;
				if (startToClosestPoint - intersectionOffset >= 0f && startToClosestPoint - intersectionOffset <= lineSegment.Length)
				{
					intersections.count = 1;
					intersections.first = lineSegment.GetPosition(startToClosestPoint - intersectionOffset);
				}
				if (startToClosestPoint + intersectionOffset >= 0f && startToClosestPoint + intersectionOffset <= lineSegment.Length)
				{
					Vector2 intersection = lineSegment.GetPosition(startToClosestPoint + intersectionOffset);
					if (intersections.count == 0)
					{
						intersections.count = 1;
						intersections.first = intersection;
					}
					else
					{
						intersections.count = 2;
						intersections.second = intersection;
					}
				}
				return intersections;
			}
			return intersections;
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x00067EA8 File Offset: 0x000660A8
		public static List<Vector2Int> GetTileCoordinatesUnderLine(Vector2Int start, Vector2Int end)
		{
			int dx = end.x - start.x;
			int num = end.y - start.y;
			int nx = Mathf.Abs(dx);
			int ny = Mathf.Abs(num);
			int signX = (dx > 0) ? 1 : -1;
			int signY = (num > 0) ? 1 : -1;
			Vector2Int p = start;
			List<Vector2Int> points = new List<Vector2Int>
			{
				p
			};
			int ix = 0;
			int iy = 0;
			while (ix < nx || iy < ny)
			{
				int decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
				if (decision == 0)
				{
					p.x += signX;
					p.y += signY;
					ix++;
					iy++;
				}
				else if (decision < 0)
				{
					p.x += signX;
					ix++;
				}
				else
				{
					p.y += signY;
					iy++;
				}
				points.Add(p);
			}
			return points;
		}

		// Token: 0x02000463 RID: 1123
		public struct CircleLineIntersection
		{
			// Token: 0x06001C1A RID: 7194 RVA: 0x00067F9F File Offset: 0x0006619F
			public Vector2 GetIntersection(int index)
			{
				if (index == 0)
				{
					return this.first;
				}
				return this.second;
			}

			// Token: 0x040017CF RID: 6095
			public Vector2 first;

			// Token: 0x040017D0 RID: 6096
			public Vector2 second;

			// Token: 0x040017D1 RID: 6097
			public int count;
		}
	}
}
