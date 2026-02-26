using System;
using UnityEngine;

namespace FixMath
{
	// Token: 0x02000274 RID: 628
	public struct RectFixed
	{
		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x00032742 File Offset: 0x00030942
		// (set) Token: 0x06000EF2 RID: 3826 RVA: 0x00032755 File Offset: 0x00030955
		public Vector3Fixed Position
		{
			get
			{
				return new Vector3Fixed(this.x, this.y);
			}
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0003276F File Offset: 0x0003096F
		// (set) Token: 0x06000EF4 RID: 3828 RVA: 0x000327AC File Offset: 0x000309AC
		public Vector3Fixed Center
		{
			get
			{
				return new Vector3Fixed(this.x + this.width / Fix64Consts.Two, this.y + this.height / Fix64Consts.Two);
			}
			set
			{
				this.x = value.x - this.width / Fix64Consts.Two;
				this.y = value.y - this.height / Fix64Consts.Two;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x000327FB File Offset: 0x000309FB
		// (set) Token: 0x06000EF6 RID: 3830 RVA: 0x0003280E File Offset: 0x00030A0E
		public Vector3Fixed Min
		{
			get
			{
				return new Vector3Fixed(this.xMin, this.yMin);
			}
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x00032828 File Offset: 0x00030A28
		// (set) Token: 0x06000EF8 RID: 3832 RVA: 0x0003283B File Offset: 0x00030A3B
		public Vector3Fixed Max
		{
			get
			{
				return new Vector3Fixed(this.xMax, this.yMax);
			}
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x00032855 File Offset: 0x00030A55
		// (set) Token: 0x06000EFA RID: 3834 RVA: 0x00032868 File Offset: 0x00030A68
		public Vector3Fixed Size
		{
			get
			{
				return new Vector3Fixed(this.width, this.height);
			}
			set
			{
				this.width = value.x;
				this.height = value.y;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000EFB RID: 3835 RVA: 0x00032882 File Offset: 0x00030A82
		// (set) Token: 0x06000EFC RID: 3836 RVA: 0x0003288C File Offset: 0x00030A8C
		public Fix64 xMin
		{
			get
			{
				return this.x;
			}
			set
			{
				Fix64 xMax = this.xMax;
				this.x = value;
				this.width = xMax - this.x;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000EFD RID: 3837 RVA: 0x000328B9 File Offset: 0x00030AB9
		// (set) Token: 0x06000EFE RID: 3838 RVA: 0x000328C4 File Offset: 0x00030AC4
		public Fix64 yMin
		{
			get
			{
				return this.y;
			}
			set
			{
				Fix64 yMax = this.yMax;
				this.y = value;
				this.height = yMax - this.y;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x000328F1 File Offset: 0x00030AF1
		// (set) Token: 0x06000F00 RID: 3840 RVA: 0x00032904 File Offset: 0x00030B04
		public Fix64 xMax
		{
			get
			{
				return this.width + this.x;
			}
			set
			{
				this.width = value - this.x;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000F01 RID: 3841 RVA: 0x00032918 File Offset: 0x00030B18
		// (set) Token: 0x06000F02 RID: 3842 RVA: 0x0003292B File Offset: 0x00030B2B
		public Fix64 yMax
		{
			get
			{
				return this.height + this.y;
			}
			set
			{
				this.height = value - this.y;
			}
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0003293F File Offset: 0x00030B3F
		public RectFixed(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0003295E File Offset: 0x00030B5E
		public RectFixed(Vector3Fixed position, Vector3Fixed size)
		{
			this.x = position.x;
			this.y = position.y;
			this.width = size.x;
			this.height = size.y;
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x00032990 File Offset: 0x00030B90
		public RectFixed(RectFixed source)
		{
			this.x = source.xMin;
			this.y = source.yMin;
			this.width = source.width;
			this.height = source.height;
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x000329C4 File Offset: 0x00030BC4
		public static bool operator !=(RectFixed lhs, RectFixed rhs)
		{
			return !(lhs.x == rhs.x) || !(lhs.y == rhs.y) || !(lhs.width == rhs.width) || !(lhs.height == rhs.height);
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x00032A20 File Offset: 0x00030C20
		public static bool operator ==(RectFixed lhs, RectFixed rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x00032A7C File Offset: 0x00030C7C
		public static RectFixed MinMaxRect(Fix64 xmin, Fix64 ymin, Fix64 xmax, Fix64 ymax)
		{
			return new RectFixed(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0003293F File Offset: 0x00030B3F
		public void Set(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x00032A94 File Offset: 0x00030C94
		public override string ToString()
		{
			return string.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", new object[]
			{
				this.x,
				this.y,
				this.width,
				this.height
			});
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x00032AEC File Offset: 0x00030CEC
		public bool Contains(Vector2Int point)
		{
			return (Fix64)((long)point.x) >= this.xMin && (Fix64)((long)point.x) <= this.xMax && (Fix64)((long)point.y) >= this.yMin && (Fix64)((long)point.y) <= this.yMax;
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x00032B64 File Offset: 0x00030D64
		private static RectFixed OrderMinMax(RectFixed rect)
		{
			if (rect.xMin > rect.xMax)
			{
				Fix64 xMin = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = xMin;
			}
			if (rect.yMin > rect.yMax)
			{
				Fix64 yMin = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = yMin;
			}
			return rect;
		}

		// Token: 0x06000F0D RID: 3853 RVA: 0x00032BD8 File Offset: 0x00030DD8
		public bool Overlaps(RectFixed other)
		{
			return other.xMax > this.xMin && other.xMin < this.xMax && other.yMax > this.yMin && other.yMin < this.yMax;
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x00032C38 File Offset: 0x00030E38
		public static Vector3Fixed NormalizedToPoint(RectFixed rectangle, Vector3Fixed normalizedRectCoordinates)
		{
			return new Vector3Fixed(Fix64.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Fix64.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x00032C70 File Offset: 0x00030E70
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.width.GetHashCode() << 2 ^ this.y.GetHashCode() >> 2 ^ this.height.GetHashCode() >> 1;
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x00032CCC File Offset: 0x00030ECC
		public override bool Equals(object other)
		{
			if (!(other is RectFixed))
			{
				return false;
			}
			RectFixed rect = (RectFixed)other;
			return this.x.Equals(rect.x) && this.y.Equals(rect.y) && this.width.Equals(rect.width) && this.height.Equals(rect.height);
		}

		// Token: 0x04000DB4 RID: 3508
		public Fix64 x;

		// Token: 0x04000DB5 RID: 3509
		public Fix64 y;

		// Token: 0x04000DB6 RID: 3510
		public Fix64 width;

		// Token: 0x04000DB7 RID: 3511
		public Fix64 height;
	}
}
