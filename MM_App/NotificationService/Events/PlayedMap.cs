using System;
using System.Collections.Generic;
using Factory;
using Motorways;

namespace NotificationService.Events
{
	// Token: 0x020002B3 RID: 691
	[Factory.Serializable(1)]
	public class PlayedMap : INotificationEventTypeWithData, INotificationEventType, INotificationEventTypeQuery
	{
		// Token: 0x060010FB RID: 4347 RVA: 0x000399EC File Offset: 0x00037BEC
		public bool InitFromJson(JSON.Dictionary json)
		{
			return json.ContainsKey("Map") && Enum.TryParse<MapDefinition.CityNames>(json.GetString("Map"), true, out this.Map);
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00039A14 File Offset: 0x00037C14
		public void ToJson(ref Dictionary<string, object> json)
		{
			json["Map"] = this.Map.ToString();
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060010FD RID: 4349 RVA: 0x00039A33 File Offset: 0x00037C33
		public string QueryName
		{
			get
			{
				return "PlayedMap";
			}
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00039A3C File Offset: 0x00037C3C
		public bool Matches(INotificationEventType eventType, DateTime onDate)
		{
			INotificationEventTypeWithData eventTypeWithData = eventType as INotificationEventTypeWithData;
			return eventTypeWithData != null && this.DataMatches(eventTypeWithData);
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00039A5C File Offset: 0x00037C5C
		public override string ToString()
		{
			return string.Format("PlayedMap-{0}", this.Map.ToString());
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00039A7C File Offset: 0x00037C7C
		public bool DataMatches(INotificationEventTypeWithData eventTypeWithData)
		{
			PlayedMap mapPlayed = eventTypeWithData as PlayedMap;
			return mapPlayed != null && this.Map == mapPlayed.Map;
		}

		// Token: 0x04000EEE RID: 3822
		public MapDefinition.CityNames Map;
	}
}
