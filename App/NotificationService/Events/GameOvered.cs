using System;
using System.Collections.Generic;
using Factory;
using Motorways;

namespace NotificationService.Events
{
	// Token: 0x020002AC RID: 684
	[Factory.Serializable(1)]
	public class GameOvered : INotificationEventTypeWithData, INotificationEventType, INotificationEventTypeQuery
	{
		// Token: 0x060010E4 RID: 4324 RVA: 0x0003973E File Offset: 0x0003793E
		public bool InitFromJson(JSON.Dictionary json)
		{
			return json.ContainsKey("Map") && Enum.TryParse<MapDefinition.CityNames>(json.GetString("Map"), true, out this.Map);
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00039766 File Offset: 0x00037966
		public void ToJson(ref Dictionary<string, object> json)
		{
			json["Map"] = this.Map.ToString();
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00039788 File Offset: 0x00037988
		public bool DataMatches(INotificationEventTypeWithData eventTypeWithData)
		{
			GameOvered mapPlayed = eventTypeWithData as GameOvered;
			return mapPlayed != null && this.Map == mapPlayed.Map;
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x000397AF File Offset: 0x000379AF
		public override string ToString()
		{
			return string.Format("GameOvered-{0}", this.Map.ToString());
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x060010E8 RID: 4328 RVA: 0x000397CC File Offset: 0x000379CC
		public string QueryName
		{
			get
			{
				return "GameOvered";
			}
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x000397D4 File Offset: 0x000379D4
		public bool Matches(INotificationEventType eventType, DateTime onDate)
		{
			INotificationEventTypeWithData eventTypeWithData = eventType as INotificationEventTypeWithData;
			return eventTypeWithData != null && this.DataMatches(eventTypeWithData);
		}

		// Token: 0x04000EDE RID: 3806
		public MapDefinition.CityNames Map;
	}
}
