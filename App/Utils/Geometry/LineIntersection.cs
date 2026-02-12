using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils.Geometry
{
	// Token: 0x0200027F RID: 639
	public class LineIntersection
	{
		// Token: 0x06000FCD RID: 4045 RVA: 0x000356F3 File Offset: 0x000338F3
		private static float Cross(float aX, float aY, float bX, float bY)
		{
			return aX * bY - aY * bX;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x000356FC File Offset: 0x000338FC
		public static bool IntersectLines(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersect, LineIntersection.LineIntersectMode lineIntersectMode = LineIntersection.LineIntersectMode.Segments)
		{
			intersect = new Vector2(float.NaN, float.NaN);
			if (lineIntersectMode == LineIntersection.LineIntersectMode.Segments)
			{
				Rect ar = LineIntersection.<IntersectLines>g__RectFromSegment|4_2(a1, a2);
				Rect br = LineIntersection.<IntersectLines>g__RectFromSegment|4_2(b1, b2);
				if (!ar.Overlaps(br))
				{
					return false;
				}
			}
			Vector3 cc = Vector3.Cross(Vector3.Cross(LineIntersection.<IntersectLines>g__HCoord|4_0(a1), LineIntersection.<IntersectLines>g__HCoord|4_0(a2)), Vector3.Cross(LineIntersection.<IntersectLines>g__HCoord|4_0(b1), LineIntersection.<IntersectLines>g__HCoord|4_0(b2)));
			if (Mathf.Abs(cc.z) < 1E-06f)
			{
				return false;
			}
			Vector2 x = cc * (1f / cc.z);
			bool flag;
			switch (lineIntersectMode)
			{
			case LineIntersection.LineIntersectMode.Rays:
				flag = (!LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, a1, a2, false) || !LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, b1, b2, false));
				break;
			case LineIntersection.LineIntersectMode.RayLine:
				flag = !LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, a1, a2, false);
				break;
			case LineIntersection.LineIntersectMode.RaySegment:
				flag = (!LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, a1, a2, false) || !LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, b1, b2, true));
				break;
			case LineIntersection.LineIntersectMode.Segments:
				flag = (!LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, a1, a2, true) || !LineIntersection.<IntersectLines>g__IsIntersectPointWithinSegment|4_1(x, b1, b2, true));
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				return false;
			}
			intersect = x;
			return true;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0003582C File Offset: 0x00033A2C
		public static int LineLineIntersection(float startAx, float startAy, float endAx, float endAy, float startBx, float startBy, float endBx, float endBy, out float pointX, out float pointY)
		{
			pointX = 0f;
			pointY = 0f;
			float atoBx = startBx - startAx;
			float atoBy = startBy - startAy;
			float a = LineIntersection.Cross(endAx, endAy, endBx, endBy);
			float testTwo = LineIntersection.Cross(atoBx, atoBy, endAx, endAy);
			bool testOnePassed = Mathf.Approximately(a, 0f);
			bool testTwoPassed = Mathf.Approximately(testTwo, 0f);
			if (testOnePassed && testTwoPassed)
			{
				return LineIntersection.Collinear;
			}
			if (!testOnePassed)
			{
				float endACrossEndB = LineIntersection.Cross(endAx, endAy, endBx, endBy);
				float t = LineIntersection.Cross(atoBx, atoBy, endBx, endBy) / endACrossEndB;
				float u = testTwo / endACrossEndB;
				if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
				{
					pointX = startAx + t * endAx;
					pointY = startAy + t * endAy;
					return LineIntersection.Point;
				}
			}
			return LineIntersection.None;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000358F4 File Offset: 0x00033AF4
		public static LineIntersection.IntersectionInfo LineLineIntersection(Vector2 startA, Vector2 endA, Vector2 startB, Vector2 endB)
		{
			Vector2 atoB = startB - startA;
			float testOne = endA.Cross(endB);
			float testTwo = atoB.Cross(endA);
			if (Mathf.Approximately(testOne, 0f) && Mathf.Approximately(testTwo, 0f))
			{
				return new LineIntersection.IntersectionInfo(LineIntersection.IntersectionInfo.IntersectionType.Collinear);
			}
			if (!Mathf.Approximately(testOne, 0f))
			{
				float t = atoB.Cross(endB) / endA.Cross(endB);
				float u = atoB.Cross(endA) / endA.Cross(endB);
				if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
				{
					Vector2 intersection = startA + t * endA;
					return new LineIntersection.IntersectionInfo(LineIntersection.IntersectionInfo.IntersectionType.Point, intersection);
				}
			}
			return new LineIntersection.IntersectionInfo(LineIntersection.IntersectionInfo.IntersectionType.None);
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x000359BE File Offset: 0x00033BBE
		[CompilerGenerated]
		internal static Vector3 <IntersectLines>g__HCoord|4_0(Vector2 p)
		{
			return new Vector3(p.x, p.y, 1f);
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x000359D8 File Offset: 0x00033BD8
		[CompilerGenerated]
		internal static bool <IntersectLines>g__IsIntersectPointWithinSegment|4_1(Vector2 p, Vector2 a, Vector2 b, bool bidirectional = false)
		{
			int i = (Mathf.Abs(b.x - a.x) < Mathf.Abs(b.y - a.y)) ? 1 : 0;
			float j = p[i] - a[i];
			float d = b[i] - a[i];
			return (!bidirectional || Mathf.Abs(j) <= Mathf.Abs(d)) && j >= 0f == d >= 0f;
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x00035A60 File Offset: 0x00033C60
		[CompilerGenerated]
		internal static Rect <IntersectLines>g__RectFromSegment|4_2(Vector2 a, Vector2 b)
		{
			Vector2 min = LineIntersection.<IntersectLines>g__MinFrom|4_3(a, b);
			return new Rect(min, LineIntersection.<IntersectLines>g__MaxFrom|4_4(a, b) - min);
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x00035A88 File Offset: 0x00033C88
		[CompilerGenerated]
		internal static Vector2 <IntersectLines>g__MinFrom|4_3(Vector2 a, Vector2 b)
		{
			return new Vector2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x00035AB1 File Offset: 0x00033CB1
		[CompilerGenerated]
		internal static Vector2 <IntersectLines>g__MaxFrom|4_4(Vector2 a, Vector2 b)
		{
			return new Vector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		// Token: 0x04000E24 RID: 3620
		public static readonly int None = -1;

		// Token: 0x04000E25 RID: 3621
		public static readonly int Collinear = -2;

		// Token: 0x04000E26 RID: 3622
		public static readonly int Point = -3;

		// Token: 0x02000280 RID: 640
		public enum LineIntersectMode
		{
			// Token: 0x04000E28 RID: 3624
			Lines,
			// Token: 0x04000E29 RID: 3625
			Rays,
			// Token: 0x04000E2A RID: 3626
			RayLine,
			// Token: 0x04000E2B RID: 3627
			RaySegment,
			// Token: 0x04000E2C RID: 3628
			Segments
		}

		// Token: 0x02000281 RID: 641
		public struct IntersectionInfo
		{
			// Token: 0x06000FD8 RID: 4056 RVA: 0x00035ADA File Offset: 0x00033CDA
			public IntersectionInfo(LineIntersection.IntersectionInfo.IntersectionType type, Vector2 intersection)
			{
				this.type = type;
				this.intersection = intersection;
			}

			// Token: 0x06000FD9 RID: 4057 RVA: 0x00035AEA File Offset: 0x00033CEA
			public IntersectionInfo(LineIntersection.IntersectionInfo.IntersectionType type)
			{
				this.type = type;
				this.intersection = new Vector2(0f, 0f);
			}

			// Token: 0x04000E2D RID: 3629
			public LineIntersection.IntersectionInfo.IntersectionType type;

			// Token: 0x04000E2E RID: 3630
			public Vector2 intersection;

			// Token: 0x02000282 RID: 642
			public enum IntersectionType
			{
				// Token: 0x04000E30 RID: 3632
				Point,
				// Token: 0x04000E31 RID: 3633
				Collinear,
				// Token: 0x04000E32 RID: 3634
				None
			}
		}
	}
}
