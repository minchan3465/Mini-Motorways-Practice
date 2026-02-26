using System;
using UnityEngine;

namespace FixMath
{
	// Token: 0x02000275 RID: 629
	public struct Vector2Fixed : IEquatable<Vector2Fixed>
	{
		// Token: 0x1700031B RID: 795
		public Fix64 this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.x;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid Vector2 index!");
				}
				return this.y;
			}
			set
			{
				if (index == 0)
				{
					this.x = value;
					return;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid Vector2 index!");
				}
				this.y = value;
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000F13 RID: 3859 RVA: 0x00032D80 File Offset: 0x00030F80
		public Vector2Fixed normalized
		{
			get
			{
				Vector2Fixed vector2 = new Vector2Fixed(this.x, this.y);
				vector2.Normalize();
				return vector2;
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x00032DA8 File Offset: 0x00030FA8
		public Vector2Fixed tangent
		{
			get
			{
				return new Vector2Fixed(-this.y, this.x);
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x00032DC0 File Offset: 0x00030FC0
		public Fix64 magnitude
		{
			get
			{
				return Fix64.Sqrt(this.x * this.x + this.y * this.y);
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x00032DEE File Offset: 0x00030FEE
		public Fix64 sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06000F17 RID: 3863 RVA: 0x00032E17 File Offset: 0x00031017
		public static Vector2Fixed zero
		{
			get
			{
				return Vector2Fixed.zeroVector;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x00032E1E File Offset: 0x0003101E
		public static Vector2Fixed one
		{
			get
			{
				return Vector2Fixed.oneVector;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06000F19 RID: 3865 RVA: 0x00032E25 File Offset: 0x00031025
		public static Vector2Fixed up
		{
			get
			{
				return Vector2Fixed.upVector;
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x00032E2C File Offset: 0x0003102C
		public static Vector2Fixed down
		{
			get
			{
				return Vector2Fixed.downVector;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000F1B RID: 3867 RVA: 0x00032E33 File Offset: 0x00031033
		public static Vector2Fixed left
		{
			get
			{
				return Vector2Fixed.leftVector;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000F1C RID: 3868 RVA: 0x00032E3A File Offset: 0x0003103A
		public static Vector2Fixed right
		{
			get
			{
				return Vector2Fixed.rightVector;
			}
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x00032E41 File Offset: 0x00031041
		public Vector2Fixed(Fix64 x, Fix64 y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x00032E51 File Offset: 0x00031051
		public Vector2Fixed(float xValue, float yValue)
		{
			this.x = (Fix64)xValue;
			this.y = (Fix64)yValue;
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x00032E6B File Offset: 0x0003106B
		public Vector2Fixed(Vector2Fixed vector2Fixed)
		{
			this = new Vector2Fixed(vector2Fixed.x, vector2Fixed.y);
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x00032E7F File Offset: 0x0003107F
		public Vector2Fixed(Vector2 vector2Float)
		{
			this = new Vector2Fixed(vector2Float.x, vector2Float.y);
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x00032E93 File Offset: 0x00031093
		public Vector2Fixed(Vector2Int vector2Int)
		{
			this = new Vector2Fixed((Fix64)((long)vector2Int.x), (Fix64)((long)vector2Int.y));
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x00032EB5 File Offset: 0x000310B5
		public Vector2Fixed(Vector3Fixed vector3Fixed)
		{
			this = new Vector2Fixed(vector3Fixed.x, vector3Fixed.y);
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x00032EC9 File Offset: 0x000310C9
		public Vector2Fixed(Vector3 vector3Float)
		{
			this = new Vector2Fixed(vector3Float.x, vector3Float.y);
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x00032EDD File Offset: 0x000310DD
		public static explicit operator Vector2Fixed(Vector2 value)
		{
			return new Vector2Fixed(value);
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x00032EE5 File Offset: 0x000310E5
		public static explicit operator Vector2Fixed(Vector3 value)
		{
			return new Vector2Fixed(value);
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x00032EED File Offset: 0x000310ED
		public static explicit operator Vector2(Vector2Fixed value)
		{
			return new Vector2((float)value.x, (float)value.y);
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x00032F0C File Offset: 0x0003110C
		public static explicit operator Vector3Fixed(Vector2Fixed value)
		{
			return new Vector3Fixed(value.x, value.y, Fix64.Zero);
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x00032F24 File Offset: 0x00031124
		public static explicit operator Vector3(Vector2Fixed value)
		{
			return new Vector3((float)value.x, (float)value.y, 0f);
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x00032F48 File Offset: 0x00031148
		public static Vector2Fixed operator +(Vector2Fixed a, Vector2Fixed b)
		{
			return new Vector2Fixed(a.x + b.x, a.y + b.y);
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x00032F71 File Offset: 0x00031171
		public static Vector2Fixed operator -(Vector2Fixed a, Vector2Fixed b)
		{
			return new Vector2Fixed(a.x - b.x, a.y - b.y);
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x00032F9A File Offset: 0x0003119A
		public static Vector2Fixed operator -(Vector2Fixed a)
		{
			return new Vector2Fixed(-a.x, -a.y);
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x00032FB7 File Offset: 0x000311B7
		public static Vector2Fixed operator *(Vector2Fixed a, Fix64 d)
		{
			return new Vector2Fixed(a.x * d, a.y * d);
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x00032FD6 File Offset: 0x000311D6
		public static Vector2Fixed operator *(Fix64 d, Vector2Fixed a)
		{
			return new Vector2Fixed(a.x * d, a.y * d);
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x00032FF5 File Offset: 0x000311F5
		public static Vector2Fixed operator /(Vector2Fixed a, Fix64 d)
		{
			return new Vector2Fixed(a.x / d, a.y / d);
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x00033014 File Offset: 0x00031214
		public static bool operator ==(Vector2Fixed lhs, Vector2Fixed rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0003303C File Offset: 0x0003123C
		public static bool operator !=(Vector2Fixed lhs, Vector2Fixed rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y;
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x00032E41 File Offset: 0x00031041
		public void Set(Fix64 new_x, Fix64 new_y)
		{
			this.x = new_x;
			this.y = new_y;
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x00033064 File Offset: 0x00031264
		public static Vector2Fixed Lerp(Vector2Fixed a, Vector2Fixed b, Fix64 t)
		{
			t = Fix64.Clamp(t, Fix64.Zero, Fix64.One);
			return new Vector2Fixed(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x000330CC File Offset: 0x000312CC
		public static Vector2Fixed LerpUnclamped(Vector2Fixed a, Vector2Fixed b, Fix64 t)
		{
			return new Vector2Fixed(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x00033124 File Offset: 0x00031324
		public static Vector2Fixed MoveTowards(Vector2Fixed current, Vector2Fixed target, Fix64 maxDistanceDelta)
		{
			Vector2Fixed vector2 = target - current;
			Fix64 magnitude = vector2.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == Fix64.Zero)
			{
				return target;
			}
			return current + vector2 / magnitude * maxDistanceDelta;
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0003316C File Offset: 0x0003136C
		public static Vector2Fixed Scale(Vector2Fixed a, Vector2Fixed b)
		{
			return new Vector2Fixed(a.x * b.x, a.y * b.y);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00033195 File Offset: 0x00031395
		public void Scale(Vector2Fixed scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x000331C8 File Offset: 0x000313C8
		public void Normalize()
		{
			Fix64 magnitude = this.magnitude;
			if (magnitude > Fix64.Zero)
			{
				this /= magnitude;
				return;
			}
			this = Vector2Fixed.zero;
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x00033208 File Offset: 0x00031408
		public override string ToString()
		{
			return ((Vector2)this).ToString();
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00033230 File Offset: 0x00031430
		public string ToString(string format)
		{
			return ((Vector2)this).ToString(format);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00033251 File Offset: 0x00031451
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00033278 File Offset: 0x00031478
		public override bool Equals(object other)
		{
			return other is Vector2Fixed && this.Equals((Vector2Fixed)other);
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x00033014 File Offset: 0x00031214
		public bool Equals(Vector2Fixed other)
		{
			return this.x == other.x && this.y == other.y;
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00033290 File Offset: 0x00031490
		public bool Approximately(Vector2Fixed other)
		{
			return Fix64.Approximately(this.x, other.x) && Fix64.Approximately(this.y, other.y);
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x000332B8 File Offset: 0x000314B8
		public static Vector2Fixed Reflect(Vector2Fixed inDirection, Vector2Fixed inNormal)
		{
			return -Fix64Consts.Two * Vector2Fixed.Dot(inNormal, inDirection) * inNormal + inDirection;
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x000332DC File Offset: 0x000314DC
		public static Fix64 Dot(Vector2Fixed lhs, Vector2Fixed rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x00033305 File Offset: 0x00031505
		public static Fix64 Angle(Vector2Fixed from, Vector2Fixed to)
		{
			return Fix64.Acos(Fix64.Clamp(Vector2Fixed.Dot(from.normalized, to.normalized), -Fix64.One, Fix64.One));
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x00033334 File Offset: 0x00031534
		public static Fix64 Distance(Vector2Fixed a, Vector2Fixed b)
		{
			return (a - b).magnitude;
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x00033350 File Offset: 0x00031550
		public static Vector2Fixed ClampMagnitude(Vector2Fixed vector, Fix64 maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x00032DEE File Offset: 0x00030FEE
		public static Fix64 SqrMagnitude(Vector2Fixed a)
		{
			return a.x * a.x + a.y * a.y;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x00032DEE File Offset: 0x00030FEE
		public Fix64 SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y;
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x00033376 File Offset: 0x00031576
		public static Vector2Fixed Min(Vector2Fixed lhs, Vector2Fixed rhs)
		{
			return new Vector2Fixed(Fix64.Min(lhs.x, rhs.x), Fix64.Min(lhs.y, rhs.y));
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0003339F File Offset: 0x0003159F
		public static Vector2Fixed Max(Vector2Fixed lhs, Vector2Fixed rhs)
		{
			return new Vector2Fixed(Fix64.Max(lhs.x, rhs.x), Fix64.Max(lhs.y, rhs.y));
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x000333C8 File Offset: 0x000315C8
		public Vector2Fixed Rotated(Fix64 angle)
		{
			Fix64 sin = Fix64.Sin(angle);
			Fix64 cos = Fix64.Cos(angle);
			return new Vector2Fixed(cos * this.x - sin * this.y, sin * this.x + cos * this.y);
		}

		// Token: 0x04000DB8 RID: 3512
		public static readonly Fix64 kEpsilon = (Fix64)1E-05f;

		// Token: 0x04000DB9 RID: 3513
		public Fix64 x;

		// Token: 0x04000DBA RID: 3514
		public Fix64 y;

		// Token: 0x04000DBB RID: 3515
		private static readonly Vector2Fixed zeroVector = new Vector2Fixed(Fix64.Zero, Fix64.Zero);

		// Token: 0x04000DBC RID: 3516
		private static readonly Vector2Fixed oneVector = new Vector2Fixed(Fix64.One, Fix64.One);

		// Token: 0x04000DBD RID: 3517
		private static readonly Vector2Fixed upVector = new Vector2Fixed(Fix64.Zero, Fix64.One);

		// Token: 0x04000DBE RID: 3518
		private static readonly Vector2Fixed downVector = new Vector2Fixed(Fix64.Zero, -Fix64.One);

		// Token: 0x04000DBF RID: 3519
		private static readonly Vector2Fixed leftVector = new Vector2Fixed(-Fix64.One, Fix64.Zero);

		// Token: 0x04000DC0 RID: 3520
		private static readonly Vector2Fixed rightVector = new Vector2Fixed(Fix64.One, Fix64.Zero);
	}
}
