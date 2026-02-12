using System;
using UnityEngine;

namespace Utils.Geometry
{
	// Token: 0x0200027E RID: 638
	public class AxisAlignedBoundingBox
	{
		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x00035501 File Offset: 0x00033701
		public Vector2 TopLeft
		{
			get
			{
				return new Vector2(this.min.x, this.max.y);
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x0003551E File Offset: 0x0003371E
		public Vector2 TopRight
		{
			get
			{
				return new Vector2(this.max.x, this.max.y);
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x0003553B File Offset: 0x0003373B
		public Vector2 BottomLeft
		{
			get
			{
				return new Vector2(this.min.x, this.min.y);
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06000FCA RID: 4042 RVA: 0x00035558 File Offset: 0x00033758
		public Vector2 BottomRight
		{
			get
			{
				return new Vector2(this.max.x, this.min.y);
			}
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x00035575 File Offset: 0x00033775
		public AxisAlignedBoundingBox(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x0003558C File Offset: 0x0003378C
		public bool IntersectWithLine(Vector2 start, Vector2 end)
		{
			return LineIntersection.LineLineIntersection(start, end - start, this.BottomLeft, this.BottomRight - this.BottomLeft).type != LineIntersection.IntersectionInfo.IntersectionType.None || LineIntersection.LineLineIntersection(start, end - start, this.BottomLeft, this.TopLeft - this.BottomLeft).type != LineIntersection.IntersectionInfo.IntersectionType.None || LineIntersection.LineLineIntersection(start, end - start, this.BottomRight, this.TopRight - this.BottomRight).type != LineIntersection.IntersectionInfo.IntersectionType.None || LineIntersection.LineLineIntersection(start, end - start, this.TopLeft, this.TopRight - this.TopLeft).type != LineIntersection.IntersectionInfo.IntersectionType.None || (start.x >= this.min.x && start.x <= this.max.x && start.y >= this.min.y && start.y <= this.max.y && (end.x >= this.min.x && end.x <= this.max.x && end.y >= this.min.y) && end.y <= this.max.y);
		}

		// Token: 0x04000E22 RID: 3618
		public Vector2 min;

		// Token: 0x04000E23 RID: 3619
		public Vector2 max;
	}
}
