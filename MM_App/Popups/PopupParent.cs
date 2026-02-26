using System;
using Factory;
using Factory.Pools;
using Motorways;
using UnityEngine;

namespace Popups
{
	// Token: 0x020002DB RID: 731
	public class PopupParent : MonoBehaviour, IReusable
	{
		// Token: 0x17000392 RID: 914
		// (get) Token: 0x060011FE RID: 4606 RVA: 0x0003BC0B File Offset: 0x00039E0B
		public float FullBlurStrength
		{
			get
			{
				if (!this._themeDatabase.IsInNightMode)
				{
					return this._fullBlurStrengthDay;
				}
				return this._fullBlurStrengthNight;
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x060011FF RID: 4607 RVA: 0x0003BC27 File Offset: 0x00039E27
		public float FullBlurRange
		{
			get
			{
				if (!this._themeDatabase.IsInNightMode)
				{
					return this._fullBlurRangeDay;
				}
				return this._fullBlurRangeNight;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001200 RID: 4608 RVA: 0x0003BC43 File Offset: 0x00039E43
		public float TweenDuration
		{
			get
			{
				return this._tweenDuration;
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001201 RID: 4609 RVA: 0x0003BC4B File Offset: 0x00039E4B
		public float FirstPopupDelay
		{
			get
			{
				return this._firstPopupDelay;
			}
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x0003BC53 File Offset: 0x00039E53
		public float FullBlurOffset()
		{
			if (this._themeDatabase.IsInNightMode)
			{
				if (!this._hasTempRange)
				{
					return this._fullBlurOffsetNight;
				}
				return this._tempBlurOffsetNight;
			}
			else
			{
				if (!this._hasTempRange)
				{
					return this._fullBlurOffsetDay;
				}
				return this._tempBlurOffsetDay;
			}
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0003BC8D File Offset: 0x00039E8D
		public void SetTempOffset(float day, float night)
		{
			this._tempBlurOffsetDay = day;
			this._tempBlurOffsetNight = night;
			this._hasTempRange = true;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x0003BCA4 File Offset: 0x00039EA4
		public void ClearTempRange()
		{
			this._tempBlurOffsetDay = this._fullBlurOffsetDay;
			this._tempBlurOffsetNight = this._fullBlurOffsetNight;
			this._hasTempRange = false;
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x04000F8B RID: 3979
		[SerializeField]
		private float _fullBlurStrengthDay;

		// Token: 0x04000F8C RID: 3980
		[SerializeField]
		private float _fullBlurStrengthNight;

		// Token: 0x04000F8D RID: 3981
		[SerializeField]
		private float _fullBlurRangeDay;

		// Token: 0x04000F8E RID: 3982
		[SerializeField]
		private float _fullBlurRangeNight;

		// Token: 0x04000F8F RID: 3983
		[SerializeField]
		private float _fullBlurOffsetDay;

		// Token: 0x04000F90 RID: 3984
		[SerializeField]
		private float _fullBlurOffsetNight;

		// Token: 0x04000F91 RID: 3985
		[SerializeField]
		private float _tweenDuration;

		// Token: 0x04000F92 RID: 3986
		[SerializeField]
		private float _firstPopupDelay;

		// Token: 0x04000F93 RID: 3987
		[Dependency]
		private MotorwaysThemeDatabase _themeDatabase;

		// Token: 0x04000F94 RID: 3988
		private bool _hasTempRange;

		// Token: 0x04000F95 RID: 3989
		private float _tempBlurOffsetDay;

		// Token: 0x04000F96 RID: 3990
		private float _tempBlurOffsetNight;
	}
}
