using System;
using Factory;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004E1 RID: 1249
	public class ClockModel : Model<ClockModel.Frame, IEmptyModelObserver>
	{
		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06002090 RID: 8336 RVA: 0x000812DC File Offset: 0x0007F4DC
		public int Hour
		{
			get
			{
				return (int)((long)(base.CurrentFrame.time / (Fix64)0.8333333333333334));
			}
		}

		// Token: 0x06002091 RID: 8337 RVA: 0x00081302 File Offset: 0x0007F502
		public static int SecondsToHours(Fix64 seconds)
		{
			return (int)((long)(seconds / (Fix64)0.8333333333333334));
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x0008131E File Offset: 0x0007F51E
		public static Fix64 HoursToSeconds(int hours)
		{
			return (Fix64)((double)hours * 0.8333333333333334);
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06002093 RID: 8339 RVA: 0x00081331 File Offset: 0x0007F531
		public int Day
		{
			get
			{
				return this.Hour / 24;
			}
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x0008133C File Offset: 0x0007F53C
		public static int SecondsToDays(Fix64 seconds)
		{
			return (int)((long)(seconds / (Fix64)0.8333333333333334)) / 24;
		}

		// Token: 0x06002095 RID: 8341 RVA: 0x0008135B File Offset: 0x0007F55B
		public static Fix64 SecondsToFractionalDays(Fix64 seconds)
		{
			return seconds / (Fix64)0.8333333333333334 / (Fix64)24L;
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x0008137E File Offset: 0x0007F57E
		public static Fix64 DaysToSeconds(int days)
		{
			return (Fix64)((double)days * 0.8333333333333334 * 24.0);
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x0008139B File Offset: 0x0007F59B
		public static Fix64 DaysToSeconds(Fix64 days)
		{
			return Fix64.FastMul(days, (Fix64)20.0);
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06002098 RID: 8344 RVA: 0x000813B1 File Offset: 0x0007F5B1
		public int Week
		{
			get
			{
				return this.Hour / 168;
			}
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000813BF File Offset: 0x0007F5BF
		public static int SecondsToWeeks(Fix64 seconds)
		{
			return (int)((long)(seconds / (Fix64)0.8333333333333334)) / 168;
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x0600209A RID: 8346 RVA: 0x000813E1 File Offset: 0x0007F5E1
		public Fix64 Time
		{
			get
			{
				return base.CurrentFrame.time;
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x0600209B RID: 8347 RVA: 0x000813EE File Offset: 0x0007F5EE
		public Fix64 FractionalDays
		{
			get
			{
				return base.CurrentFrame.time / (Fix64)20.0;
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x0600209C RID: 8348 RVA: 0x0008140E File Offset: 0x0007F60E
		public int ExpansionHour
		{
			get
			{
				return (int)((long)(base.CurrentFrame.expansionTime / (Fix64)0.8333333333333334));
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x0600209D RID: 8349 RVA: 0x00081434 File Offset: 0x0007F634
		public int ExpansionDay
		{
			get
			{
				return this.ExpansionHour / 24;
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x0600209E RID: 8350 RVA: 0x0008143F File Offset: 0x0007F63F
		public int ExpansionWeek
		{
			get
			{
				return this.ExpansionHour / 168;
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x0600209F RID: 8351 RVA: 0x0008144D File Offset: 0x0007F64D
		public Fix64 ExpansionTime
		{
			get
			{
				return base.CurrentFrame.expansionTime;
			}
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x0008145A File Offset: 0x0007F65A
		public override void Reset()
		{
			base.Reset();
			this.isPaused = false;
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x00081469 File Offset: 0x0007F669
		public float GetInterpolatedTime(float alpha)
		{
			return (float)base.CurrentFrame.time + (float)(base.NextFrame.time - base.CurrentFrame.time) * alpha;
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x000814A0 File Offset: 0x0007F6A0
		public Fix64 GetInterpolatedExpansionTime(float alpha)
		{
			return base.CurrentFrame.expansionTime + (base.NextFrame.expansionTime - base.CurrentFrame.expansionTime) * (Fix64)alpha;
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x000814D8 File Offset: 0x0007F6D8
		public void SetExpansionTimeToDay(Fix64 expansionTimeDay)
		{
			base.CurrentFrame.expansionTime = ClockModel.DaysToSeconds(expansionTimeDay);
			base.NextFrame.expansionTime = base.CurrentFrame.expansionTime;
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x00081501 File Offset: 0x0007F701
		public ClockModel() : base(1)
		{
		}

		// Token: 0x04001B14 RID: 6932
		public const double SecondsPerHour = 0.8333333333333334;

		// Token: 0x04001B15 RID: 6933
		public const double SecondsPerWeek = 140.0;

		// Token: 0x04001B16 RID: 6934
		public bool isPaused;

		// Token: 0x04001B17 RID: 6935
		public bool expansionTimeManuallyPaused;

		// Token: 0x020004E2 RID: 1250
		public class Frame : IFrame
		{
			// Token: 0x060020A5 RID: 8357 RVA: 0x0008150A File Offset: 0x0007F70A
			public void Reset()
			{
				this.time = Fix64.Zero;
				this.expansionTime = Fix64.Zero;
			}

			// Token: 0x060020A6 RID: 8358 RVA: 0x00081522 File Offset: 0x0007F722
			public bool CloneInto(IFrame cloneState, IScope scope)
			{
				ClockModel.Frame frame = (ClockModel.Frame)cloneState;
				frame.time = this.time;
				frame.expansionTime = this.expansionTime;
				return true;
			}

			// Token: 0x04001B18 RID: 6936
			public Fix64 time;

			// Token: 0x04001B19 RID: 6937
			public Fix64 expansionTime;
		}
	}
}
