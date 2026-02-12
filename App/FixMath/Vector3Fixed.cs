using System;
using System.ComponentModel;
using UnityEngine;

namespace FixMath
{
	// Token: 0x02000276 RID: 630
	[Serializable]
	public struct Vector3Fixed : IEquatable<Vector3Fixed>
	{
		// Token: 0x17000326 RID: 806
		public Fix64 this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
					return Fix64.Zero;
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					return;
				case 1:
					this.y = value;
					return;
				case 2:
					this.z = value;
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0003351E File Offset: 0x0003171E
		public Vector3Fixed normalized
		{
			get
			{
				return Vector3Fixed.Normalize(this);
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0003352C File Offset: 0x0003172C
		public Fix64 magnitude
		{
			get
			{
				return Fix64.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000F4D RID: 3917 RVA: 0x0003357B File Offset: 0x0003177B
		public Fix64 sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000F4E RID: 3918 RVA: 0x000335BA File Offset: 0x000317BA
		public static Vector3Fixed zero
		{
			get
			{
				return Vector3Fixed.zeroVector;
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000F4F RID: 3919 RVA: 0x000335C1 File Offset: 0x000317C1
		public static Vector3Fixed one
		{
			get
			{
				return Vector3Fixed.oneVector;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000F50 RID: 3920 RVA: 0x000335C8 File Offset: 0x000317C8
		public static Vector3Fixed forward
		{
			get
			{
				return Vector3Fixed.forwardVector;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000F51 RID: 3921 RVA: 0x000335CF File Offset: 0x000317CF
		public static Vector3Fixed back
		{
			get
			{
				return Vector3Fixed.backVector;
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000F52 RID: 3922 RVA: 0x000335D6 File Offset: 0x000317D6
		public static Vector3Fixed up
		{
			get
			{
				return Vector3Fixed.upVector;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000F53 RID: 3923 RVA: 0x000335DD File Offset: 0x000317DD
		public static Vector3Fixed down
		{
			get
			{
				return Vector3Fixed.downVector;
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000F54 RID: 3924 RVA: 0x000335E4 File Offset: 0x000317E4
		public static Vector3Fixed left
		{
			get
			{
				return Vector3Fixed.leftVector;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x000335EB File Offset: 0x000317EB
		public static Vector3Fixed right
		{
			get
			{
				return Vector3Fixed.rightVector;
			}
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x000335F2 File Offset: 0x000317F2
		public Vector3Fixed(Vector3Fixed vector3Fixed)
		{
			this = new Vector3Fixed(vector3Fixed.x, vector3Fixed.y, vector3Fixed.z);
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x0003360C File Offset: 0x0003180C
		public Vector3Fixed(Vector3 vector3Float)
		{
			this = new Vector3Fixed(vector3Float.x, vector3Float.y, vector3Float.z);
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x00033626 File Offset: 0x00031826
		public Vector3Fixed(Vector2 vector2Float)
		{
			this = new Vector3Fixed(vector2Float.x, vector2Float.y, 0f);
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0003363F File Offset: 0x0003183F
		public Vector3Fixed(Vector2Int vector2Int)
		{
			this = new Vector3Fixed((Fix64)((long)vector2Int.x), (Fix64)((long)vector2Int.y), Fix64Consts.Zero);
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x00033666 File Offset: 0x00031866
		public Vector3Fixed(Vector2Fixed vector2Fixed)
		{
			this = new Vector3Fixed(vector2Fixed.x, vector2Fixed.y, Fix64Consts.Zero);
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0003367F File Offset: 0x0003187F
		public Vector3Fixed(Fix64 xValue, Fix64 yValue, Fix64 zValue)
		{
			this.x = xValue;
			this.y = yValue;
			this.z = zValue;
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x00033696 File Offset: 0x00031896
		public Vector3Fixed(float xValue, float yValue, float zValue)
		{
			this.x = (Fix64)xValue;
			this.y = (Fix64)yValue;
			this.z = (Fix64)zValue;
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x000336BC File Offset: 0x000318BC
		public Vector3Fixed(Fix64 x, Fix64 y)
		{
			this = new Vector3Fixed(x, y, Fix64.Zero);
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x000336CB File Offset: 0x000318CB
		public Vector3Fixed(float x, float y)
		{
			this = new Vector3Fixed(x, y, 0f);
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x000336DA File Offset: 0x000318DA
		public static Vector3Fixed Slerp(Vector3Fixed a, Vector3Fixed b, Fix64 t)
		{
			return Vector3Fixed.SlerpUnclamped(a, b, Fix64.Clamp01(t));
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed SlerpUnclamped(Vector3Fixed a, Vector3Fixed b, Fix64 t)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x00015E3F File Offset: 0x0001403F
		public static void OrthoNormalize(ref Vector3Fixed normal, ref Vector3Fixed tangent)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x00015E3F File Offset: 0x0001403F
		public static void OrthoNormalize(ref Vector3Fixed normal, ref Vector3Fixed tangent, ref Vector3Fixed binormal)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed RotateTowards(Vector3Fixed current, Vector3Fixed target, Fix64 maxRadiansDelta, Fix64 maxMagnitudeDelta)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x000336E9 File Offset: 0x000318E9
		public static Vector3Fixed Lerp(Vector3Fixed a, Vector3Fixed b, Fix64 t)
		{
			return Vector3Fixed.LerpUnclamped(a, b, Fix64.Clamp01(t));
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x000336F8 File Offset: 0x000318F8
		public static Vector3Fixed LerpUnclamped(Vector3Fixed a, Vector3Fixed b, Fix64 t)
		{
			return new Vector3Fixed(Fix64.Lerp(a.x, b.x, t), Fix64.Lerp(a.y, b.y, t), Fix64.Lerp(a.z, b.z, t));
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed MoveTowards(Vector3Fixed current, Vector3Fixed target, Fix64 maxDistanceDelta)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed SmoothDamp(Vector3Fixed current, Vector3Fixed target, ref Vector3Fixed currentVelocity, Fix64 smoothTime, Fix64 maxSpeed)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed SmoothDamp(Vector3Fixed current, Vector3Fixed target, ref Vector3Fixed currentVelocity, Fix64 smoothTime)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed SmoothDamp(Vector3Fixed current, Vector3Fixed target, ref Vector3Fixed currentVelocity, Fix64 smoothTime, [DefaultValue("Mathf.Infinity")] Fix64 maxSpeed, [DefaultValue("Time.deltaTime")] Fix64 deltaTime)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0003367F File Offset: 0x0003187F
		public void Set(Fix64 newX, Fix64 newY, Fix64 newZ)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x00033696 File Offset: 0x00031896
		public void Set(float newX, float newY, float newZ)
		{
			this.x = (Fix64)newX;
			this.y = (Fix64)newY;
			this.z = (Fix64)newZ;
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x00033735 File Offset: 0x00031935
		public static Vector3Fixed Scale(Vector3Fixed a, Vector3Fixed b)
		{
			return new Vector3Fixed(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x00033770 File Offset: 0x00031970
		public void Scale(Vector3Fixed scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x000337C2 File Offset: 0x000319C2
		public void ScaleUniform(Fix64 scale)
		{
			this.x *= scale;
			this.y *= scale;
			this.z *= scale;
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x000337FC File Offset: 0x000319FC
		public static Vector3Fixed Cross(Vector3Fixed lhs, Vector3Fixed rhs)
		{
			return new Vector3Fixed(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x00033883 File Offset: 0x00031A83
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x000338BA File Offset: 0x00031ABA
		public override bool Equals(object other)
		{
			return other != null && typeof(Vector3Fixed).IsAssignableFrom(other.GetType()) && this.Equals((Vector3Fixed)other);
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x000338E4 File Offset: 0x00031AE4
		public bool Equals(Vector3Fixed other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed Reflect(Vector3Fixed inDirection, Vector3Fixed inNormal)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x00033920 File Offset: 0x00031B20
		public static Vector3Fixed Normalize(Vector3Fixed value)
		{
			Vector3Fixed normalizedValue = new Vector3Fixed(value);
			normalizedValue.Normalize();
			return normalizedValue;
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x00033940 File Offset: 0x00031B40
		public void Normalize()
		{
			Fix64 ourMagnitude = this.magnitude;
			this.x /= ourMagnitude;
			this.y /= ourMagnitude;
			this.z /= ourMagnitude;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0003398A File Offset: 0x00031B8A
		public static Fix64 Dot(Vector3Fixed lhs, Vector3Fixed rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed Project(Vector3Fixed vector, Vector3Fixed onNormal)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Vector3Fixed ProjectOnPlane(Vector3Fixed vector, Vector3Fixed planeNormal)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x000339C9 File Offset: 0x00031BC9
		public static Fix64 Angle(Vector3Fixed from, Vector3Fixed to)
		{
			return Fix64.Acos(Vector3Fixed.Dot(from.normalized, to.normalized));
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x000339E4 File Offset: 0x00031BE4
		public static Vector3Fixed RotateByQuaternion(Vector3Fixed v, Quaternion q)
		{
			Vector3Fixed u = new Vector3Fixed(q.x, q.y, q.z);
			Fix64 s = (Fix64)q.w;
			return Fix64Consts.Two * Vector3Fixed.Dot(u, v) * u + (s * s - Vector3Fixed.Dot(u, u)) * v + Fix64Consts.Two * s * Vector3Fixed.Cross(u, v);
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x00015E3F File Offset: 0x0001403F
		public static Fix64 SignedAngle(Vector3Fixed from, Vector3Fixed to, Vector3Fixed axis)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x00033A68 File Offset: 0x00031C68
		public static Fix64 Distance(Vector3Fixed a, Vector3Fixed b)
		{
			return (a - b).magnitude;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x00033A84 File Offset: 0x00031C84
		public static Vector3Fixed ClampMagnitude(Vector3Fixed vector, Fix64 maxLength)
		{
			maxLength * maxLength;
			if (Vector3Fixed.SqrMagnitude(vector) > maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x00033AAB File Offset: 0x00031CAB
		public static Fix64 Magnitude(Vector3Fixed vector)
		{
			return Fix64.Sqrt(Vector3Fixed.SqrMagnitude(vector));
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0003357B File Offset: 0x0003177B
		public static Fix64 SqrMagnitude(Vector3Fixed vector)
		{
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x00033AB8 File Offset: 0x00031CB8
		public static Vector3Fixed Min(Vector3Fixed lhs, Vector3Fixed rhs)
		{
			return new Vector3Fixed(Fix64.Min(lhs.x, rhs.x), Fix64.Min(lhs.y, rhs.y), Fix64.Min(lhs.z, rhs.z));
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x00033AF2 File Offset: 0x00031CF2
		public static Vector3Fixed Max(Vector3Fixed lhs, Vector3Fixed rhs)
		{
			return new Vector3Fixed(Fix64.Max(lhs.x, rhs.x), Fix64.Max(lhs.y, rhs.y), Fix64.Max(lhs.z, rhs.z));
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x00033B2C File Offset: 0x00031D2C
		public static Vector3Fixed operator +(Vector3Fixed a, Vector3Fixed b)
		{
			return new Vector3Fixed(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x00033B66 File Offset: 0x00031D66
		public static Vector3Fixed operator -(Vector3Fixed a, Vector3Fixed b)
		{
			return new Vector3Fixed(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x00033BA0 File Offset: 0x00031DA0
		public static Vector3Fixed operator -(Vector3Fixed a)
		{
			return new Vector3Fixed(-a.x, -a.y, -a.z);
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x00033BC8 File Offset: 0x00031DC8
		public static Vector3Fixed operator *(Vector3Fixed a, Fix64 d)
		{
			return new Vector3Fixed(a.x * d, a.y * d, a.z * d);
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x00033BF3 File Offset: 0x00031DF3
		public static Vector3Fixed operator *(Fix64 d, Vector3Fixed a)
		{
			return new Vector3Fixed(a.x * d, a.y * d, a.z * d);
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x00033C1E File Offset: 0x00031E1E
		public static Vector3Fixed operator /(Vector3Fixed a, Fix64 d)
		{
			return new Vector3Fixed(a.x / d, a.y / d, a.z / d);
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x000338E4 File Offset: 0x00031AE4
		public static bool operator ==(Vector3Fixed lhs, Vector3Fixed rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x00033C49 File Offset: 0x00031E49
		public static bool operator !=(Vector3Fixed lhs, Vector3Fixed rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x00033C84 File Offset: 0x00031E84
		public static explicit operator Vector3Fixed(Vector3 value)
		{
			return new Vector3Fixed(value);
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x00033C8C File Offset: 0x00031E8C
		public static explicit operator Vector3(Vector3Fixed value)
		{
			return new Vector3((float)value.x, (float)value.y, (float)value.z);
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x00033CB7 File Offset: 0x00031EB7
		public static explicit operator Vector2(Vector3Fixed value)
		{
			return new Vector3((float)value.x, (float)value.y);
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x00033CDC File Offset: 0x00031EDC
		public override string ToString()
		{
			return ((Vector3)this).ToString();
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x00033D04 File Offset: 0x00031F04
		public string ToString(string format)
		{
			return ((Vector3)this).ToString(format);
		}

		// Token: 0x04000DC1 RID: 3521
		public static readonly Fix64 kEpsilon = (Fix64)1E-05f;

		// Token: 0x04000DC2 RID: 3522
		public static readonly Fix64 kEpsilonNormalSqrt = (Fix64)1E-15f;

		// Token: 0x04000DC3 RID: 3523
		public Fix64 x;

		// Token: 0x04000DC4 RID: 3524
		public Fix64 y;

		// Token: 0x04000DC5 RID: 3525
		public Fix64 z;

		// Token: 0x04000DC6 RID: 3526
		private static readonly Vector3Fixed zeroVector = new Vector3Fixed(0f, 0f, 0f);

		// Token: 0x04000DC7 RID: 3527
		private static readonly Vector3Fixed oneVector = new Vector3Fixed(1f, 1f, 1f);

		// Token: 0x04000DC8 RID: 3528
		private static readonly Vector3Fixed upVector = new Vector3Fixed(0f, 1f, 0f);

		// Token: 0x04000DC9 RID: 3529
		private static readonly Vector3Fixed downVector = new Vector3Fixed(0f, -1f, 0f);

		// Token: 0x04000DCA RID: 3530
		private static readonly Vector3Fixed leftVector = new Vector3Fixed(-1f, 0f, 0f);

		// Token: 0x04000DCB RID: 3531
		private static readonly Vector3Fixed rightVector = new Vector3Fixed(1f, 0f, 0f);

		// Token: 0x04000DCC RID: 3532
		private static readonly Vector3Fixed forwardVector = new Vector3Fixed(0f, 0f, 1f);

		// Token: 0x04000DCD RID: 3533
		private static readonly Vector3Fixed backVector = new Vector3Fixed(0f, 0f, -1f);
	}
}
