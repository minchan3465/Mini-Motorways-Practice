using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003BB RID: 955
	[CreateAssetMenu(menuName = "Motorways/NewsAndNotifications")]
	public class NewsAndNotificationData : ScriptableObject
	{
		// Token: 0x060016CB RID: 5835 RVA: 0x00052ACC File Offset: 0x00050CCC
		[NotNull]
		public List<NewsAndNotificationObject> GetNotifications(RuntimePlatform platform)
		{
			List<NewsAndNotificationObject> results = new List<NewsAndNotificationObject>();
			NewsAndNotificationObject.RuntimeVariant currentVariant = NewsAndNotificationObject.EnvironmentToVariant(AppContainer.Environment);
			foreach (NewsAndNotificationObject newsObject in this._newsAndNotificationObjects)
			{
				if (newsObject.StartDateTime() < DateTime.UtcNow && newsObject.EndDateTime() > DateTime.UtcNow && newsObject.AvailablePlatform == platform && newsObject.AvailableVariant == currentVariant)
				{
					results.Add(newsObject);
				}
			}
			return results;
		}

		// Token: 0x0400135C RID: 4956
		[SerializeField]
		[Tooltip("The individual news and notifications.")]
		private List<NewsAndNotificationObject> _newsAndNotificationObjects;
	}
}
