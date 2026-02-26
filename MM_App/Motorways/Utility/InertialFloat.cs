using System;
using Easing;
using UnityEngine;

namespace Motorways.Utility
{
	// Token: 0x02000464 RID: 1124
	public class InertialFloat
	{
		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06001C1B RID: 7195 RVA: 0x00067FB1 File Offset: 0x000661B1
		// (set) Token: 0x06001C1C RID: 7196 RVA: 0x00067FB9 File Offset: 0x000661B9
		public float AverageVelocity { get; private set; }

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06001C1D RID: 7197 RVA: 0x00067FC2 File Offset: 0x000661C2
		// (set) Token: 0x06001C1E RID: 7198 RVA: 0x00067FCA File Offset: 0x000661CA
		public float SpringTargetAbsolute { get; set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001C1F RID: 7199 RVA: 0x00067FD3 File Offset: 0x000661D3
		// (set) Token: 0x06001C20 RID: 7200 RVA: 0x00067FDB File Offset: 0x000661DB
		public float Min
		{
			get
			{
				return this._min;
			}
			set
			{
				this._min = value;
			}
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001C21 RID: 7201 RVA: 0x00067FE4 File Offset: 0x000661E4
		// (set) Token: 0x06001C22 RID: 7202 RVA: 0x00067FEC File Offset: 0x000661EC
		public float Max
		{
			get
			{
				return this._max;
			}
			set
			{
				this._max = value;
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001C23 RID: 7203 RVA: 0x00067FF5 File Offset: 0x000661F5
		// (set) Token: 0x06001C24 RID: 7204 RVA: 0x00068018 File Offset: 0x00066218
		public float Range
		{
			get
			{
				if (this._range >= 0f)
				{
					return this._range;
				}
				return this._max - this._min;
			}
			set
			{
				this._range = value;
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001C25 RID: 7205 RVA: 0x00068021 File Offset: 0x00066221
		// (set) Token: 0x06001C26 RID: 7206 RVA: 0x00068029 File Offset: 0x00066229
		public float RawValue
		{
			get
			{
				return this._rawValue;
			}
			set
			{
				this._rawValue = value;
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06001C27 RID: 7207 RVA: 0x00068034 File Offset: 0x00066234
		public float ConstrainedValue
		{
			get
			{
				if (this._rawValue >= this._min && this._rawValue <= this._max)
				{
					return this._rawValue;
				}
				float distance;
				if (this._rawValue < this._min)
				{
					distance = this._min - this._rawValue;
				}
				else
				{
					distance = this._rawValue - this._max;
				}
				float t = distance * 0.55f / this.Range;
				t += 1f;
				t = 1f / t;
				t = 1f - t;
				t *= this.Range;
				if (this._rawValue < this._min)
				{
					return this._min - t;
				}
				return this._max + t;
			}
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x000680DF File Offset: 0x000662DF
		public InertialFloat(float springDuration, Easings.Functions springEasing)
		{
			this.Reset();
			this._springDuration = springDuration;
			this._springEasing = springEasing;
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x0006810D File Offset: 0x0006630D
		public void Reset()
		{
			this._min = 0f;
			this._max = 1f;
			this._range = -1f;
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x00068130 File Offset: 0x00066330
		public void Tick(float elapsedTime)
		{
			if (this._springTime > 0f)
			{
				this._springTime -= elapsedTime;
				float springTarget = this._rawValue;
				if (this._springTarget == InertialFloat.SpringTarget.Min)
				{
					springTarget = this._min;
				}
				else if (this._springTarget == InertialFloat.SpringTarget.Max)
				{
					springTarget = this._max;
				}
				else if (this._springTarget == InertialFloat.SpringTarget.Absolute)
				{
					springTarget = this.SpringTargetAbsolute;
				}
				if (this._springTime <= 0f)
				{
					this._springTime = -1f;
					this._rawValue = springTarget;
				}
				else
				{
					this._rawValue = this._springOrigin + Easings.Interpolate((this._springDuration - this._springTime) / this._springDuration, this._springEasing) * (springTarget - this._springOrigin);
				}
			}
			if (!this._resetVelocity)
			{
				float newVelocity = this._rawValue - this._previousRawValue;
				float alpha = 1f - Mathf.Exp(-elapsedTime / 0.3f);
				this.AverageVelocity = alpha * newVelocity + (1f - alpha) * this.AverageVelocity;
			}
			else
			{
				this._resetVelocity = false;
			}
			this._previousRawValue = this._rawValue;
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x00068244 File Offset: 0x00066444
		public void SpringBackToExtents()
		{
			if (this._rawValue > this._max)
			{
				this._springTarget = InertialFloat.SpringTarget.Max;
			}
			else
			{
				if (this._rawValue >= this._min)
				{
					return;
				}
				this._springTarget = InertialFloat.SpringTarget.Min;
			}
			this._springTime = this._springDuration;
			this._springOrigin = this._rawValue;
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x00068298 File Offset: 0x00066498
		public void SpringToMin()
		{
			this._springTarget = InertialFloat.SpringTarget.Min;
			this._springTime = this._springDuration;
			this._springOrigin = this._rawValue;
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000682B9 File Offset: 0x000664B9
		public void SpringTo(float target)
		{
			this._springTarget = InertialFloat.SpringTarget.Absolute;
			this.SpringTargetAbsolute = target;
			this._springTime = this._springDuration;
			this._springOrigin = this._rawValue;
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x000682E1 File Offset: 0x000664E1
		public void Hold()
		{
			this._springTarget = InertialFloat.SpringTarget.None;
			this._springTime = -1f;
			this._resetVelocity = true;
			this.AverageVelocity = 0f;
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001C2F RID: 7215 RVA: 0x00068307 File Offset: 0x00066507
		public bool IsWithinConstraints
		{
			get
			{
				return this._rawValue >= this._min && this._rawValue <= this._max;
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06001C30 RID: 7216 RVA: 0x0006832A File Offset: 0x0006652A
		public bool IsSpringing
		{
			get
			{
				return this._springTime >= 0f;
			}
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x0006833C File Offset: 0x0006653C
		public InertialFloat Clone()
		{
			return new InertialFloat(this._springDuration, this._springEasing)
			{
				_min = this._min,
				_max = this._max,
				_range = this._range,
				_rawValue = this._rawValue,
				AverageVelocity = this.AverageVelocity,
				_resetVelocity = this._resetVelocity,
				_previousRawValue = this._previousRawValue,
				_springTarget = this._springTarget,
				SpringTargetAbsolute = this.SpringTargetAbsolute,
				_springTime = this._springTime,
				_springOrigin = this._springOrigin
			};
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x000683E0 File Offset: 0x000665E0
		public override bool Equals(object obj)
		{
			if (typeof(InertialFloat).IsAssignableFrom(obj.GetType()))
			{
				InertialFloat otherFloat = (InertialFloat)obj;
				return Mathf.Approximately(this._min, otherFloat._min) && Mathf.Approximately(this._max, otherFloat._max) && Mathf.Approximately(this._range, otherFloat._range) && Mathf.Approximately(this._rawValue, otherFloat._rawValue) && Mathf.Approximately(this.AverageVelocity, otherFloat.AverageVelocity) && this._resetVelocity == otherFloat._resetVelocity && Mathf.Approximately(this._previousRawValue, otherFloat._previousRawValue) && this._springTarget == otherFloat._springTarget && Mathf.Approximately(this.SpringTargetAbsolute, otherFloat.SpringTargetAbsolute) && Mathf.Approximately(this._springTime, otherFloat._springTime) && Mathf.Approximately(this._springOrigin, otherFloat._springOrigin) && Mathf.Approximately(this._springDuration, otherFloat._springDuration);
			}
			return false;
		}

		// Token: 0x040017D2 RID: 6098
		private float _min;

		// Token: 0x040017D3 RID: 6099
		private float _max;

		// Token: 0x040017D4 RID: 6100
		private float _range;

		// Token: 0x040017D5 RID: 6101
		private float _rawValue;

		// Token: 0x040017D7 RID: 6103
		private bool _resetVelocity = true;

		// Token: 0x040017D8 RID: 6104
		private float _previousRawValue;

		// Token: 0x040017D9 RID: 6105
		private InertialFloat.SpringTarget _springTarget;

		// Token: 0x040017DB RID: 6107
		private float _springTime = -1f;

		// Token: 0x040017DC RID: 6108
		private float _springOrigin;

		// Token: 0x040017DD RID: 6109
		private readonly Easings.Functions _springEasing;

		// Token: 0x040017DE RID: 6110
		private const float Inertia = 0.55f;

		// Token: 0x040017DF RID: 6111
		private readonly float _springDuration;

		// Token: 0x02000465 RID: 1125
		private enum SpringTarget
		{
			// Token: 0x040017E1 RID: 6113
			None,
			// Token: 0x040017E2 RID: 6114
			Min,
			// Token: 0x040017E3 RID: 6115
			Max,
			// Token: 0x040017E4 RID: 6116
			Absolute
		}
	}
}
