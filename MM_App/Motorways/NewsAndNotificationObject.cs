using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003BC RID: 956
	[Serializable]
	public class NewsAndNotificationObject
	{
		// Token: 0x060016CD RID: 5837 RVA: 0x00052B68 File Offset: 0x00050D68
		public NewsAndNotificationObject(string contentIndicatorID, StringId headerID, StringId bodyID, string weblink, NewsAndNotificationObject.DateTimeEditable startDateTimeEditable, NewsAndNotificationObject.DateTimeEditable endDateTimeEditable, NewsAndNotificationObject.RuntimeVariant availableVariant)
		{
			this.ContentIndicatorID = contentIndicatorID;
			this.HeaderID = headerID;
			this.BodyID = bodyID;
			this.WebLink = weblink;
			this.StartDateTimeEditable = startDateTimeEditable;
			this.EndDateTimeEditable = endDateTimeEditable;
			this.AvailableVariant = availableVariant;
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x00052BA5 File Offset: 0x00050DA5
		public DateTime StartDateTime()
		{
			return new DateTime(this.StartDateTimeEditable.Year, this.StartDateTimeEditable.Month, this.StartDateTimeEditable.Day);
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x00052BCD File Offset: 0x00050DCD
		public DateTime EndDateTime()
		{
			return new DateTime(this.EndDateTimeEditable.Year, this.EndDateTimeEditable.Month, this.EndDateTimeEditable.Day);
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x00052BF8 File Offset: 0x00050DF8
		public static NewsAndNotificationObject.RuntimeVariant EnvironmentToVariant(IEnvironment environment)
		{
			if (environment is WindowsSteamEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Steam;
			}
			if (environment is WindowsHumbleEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Steam;
			}
			if (environment is macOSSteamEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Steam;
			}
			if (environment is macOSHumbleEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Steam;
			}
			if (environment is iOSAppStoreEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Arcade;
			}
			if (environment is iOSRetailDemoEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Arcade;
			}
			if (environment is tvOSAppStoreEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Arcade;
			}
			if (environment is tvOSRetailDemoEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Arcade;
			}
			if (environment is macOSAppStoreEnvironment)
			{
				return NewsAndNotificationObject.RuntimeVariant.Arcade;
			}
			return NewsAndNotificationObject.RuntimeVariant.DefaultEditor;
		}

		// Token: 0x0400135D RID: 4957
		public string ContentIndicatorID;

		// Token: 0x0400135E RID: 4958
		public StringId HeaderID;

		// Token: 0x0400135F RID: 4959
		public StringId BodyID;

		// Token: 0x04001360 RID: 4960
		public string WebLink;

		// Token: 0x04001361 RID: 4961
		public NewsAndNotificationObject.DateTimeEditable StartDateTimeEditable;

		// Token: 0x04001362 RID: 4962
		public NewsAndNotificationObject.DateTimeEditable EndDateTimeEditable;

		// Token: 0x04001363 RID: 4963
		public RuntimePlatform AvailablePlatform;

		// Token: 0x04001364 RID: 4964
		public NewsAndNotificationObject.RuntimeVariant AvailableVariant;

		// Token: 0x020003BD RID: 957
		[Serializable]
		public struct DateTimeEditable
		{
			// Token: 0x04001365 RID: 4965
			public int Day;

			// Token: 0x04001366 RID: 4966
			public int Month;

			// Token: 0x04001367 RID: 4967
			public int Year;
		}

		// Token: 0x020003BE RID: 958
		[Serializable]
		public enum RuntimeVariant
		{
			// Token: 0x04001369 RID: 4969
			DefaultEditor,
			// Token: 0x0400136A RID: 4970
			Steam,
			// Token: 0x0400136B RID: 4971
			Humble,
			// Token: 0x0400136C RID: 4972
			Arcade,
			// Token: 0x0400136D RID: 4973
			AppStore,
			// Token: 0x0400136E RID: 4974
			WeGame,
			// Token: 0x0400136F RID: 4975
			Demo,
			// Token: 0x04001370 RID: 4976
			Eshop
		}
	}
}
