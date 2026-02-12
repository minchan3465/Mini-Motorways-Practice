using System;
using Easing;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200045B RID: 1115
	public abstract class Tween<T>
	{
		// Token: 0x06001BEF RID: 7151 RVA: 0x00066FB2 File Offset: 0x000651B2
		public void Reset()
		{
			this._time = 0f;
			this._duration = 0f;
			this._startValue = default(T);
			this._endValue = default(T);
			this._function = Easings.Functions.Linear;
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x00066FE9 File Offset: 0x000651E9
		public void Start(T start, T end, float duration, Easings.Functions easingFunction, float delay = 0f)
		{
			this._time = -delay;
			this._startValue = start;
			this._endValue = end;
			this._duration = duration;
			this._function = easingFunction;
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x00067011 File Offset: 0x00065211
		public void Stop()
		{
			this._duration = 0f;
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x0006701E File Offset: 0x0006521E
		public void Set(T val, float duration = 0f)
		{
			this._endValue = val;
			this._duration = duration;
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x00067030 File Offset: 0x00065230
		public T Tick(float deltaTime)
		{
			if (!Diagnostics.Verify(this._duration > 0f, "Please don't tick an inactive tween."))
			{
				return this._endValue;
			}
			this._time += deltaTime;
			if (this._time >= this._duration)
			{
				this._duration = 0f;
			}
			return this.Value;
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06001BF4 RID: 7156 RVA: 0x0006708A File Offset: 0x0006528A
		public bool IsActive
		{
			get
			{
				return this._duration > 0f;
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06001BF5 RID: 7157 RVA: 0x0006709C File Offset: 0x0006529C
		public T Value
		{
			get
			{
				if (this._duration <= 0f)
				{
					return this._endValue;
				}
				return this.LerpValue(this._startValue, this._endValue, Easings.Interpolate(Mathf.Max(0f, this._time) / this._duration, this._function));
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x000670F1 File Offset: 0x000652F1
		public T End
		{
			get
			{
				return this._endValue;
			}
		}

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06001BF7 RID: 7159 RVA: 0x000670F9 File Offset: 0x000652F9
		public float Duration
		{
			get
			{
				return this._duration;
			}
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x00067104 File Offset: 0x00065304
		public override string ToString()
		{
			return string.Format("[Tween(Start={0}, End={1}, Value={2}, Time={3}, Duration={4})]", new object[]
			{
				this._startValue,
				this._endValue,
				this.Value,
				this._time,
				this._duration
			});
		}

		// Token: 0x06001BF9 RID: 7161
		protected abstract T LerpValue(T startValue, T endValue, float alpha);

		// Token: 0x0400173F RID: 5951
		private float _time;

		// Token: 0x04001740 RID: 5952
		private float _duration;

		// Token: 0x04001741 RID: 5953
		private T _startValue;

		// Token: 0x04001742 RID: 5954
		private T _endValue;

		// Token: 0x04001743 RID: 5955
		private Easings.Functions _function;
	}
}
