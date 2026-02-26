using System;
using System.Collections.Generic;
using Factory;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.UI.NewContentIndicators
{
	// Token: 0x02000751 RID: 1873
	[CreateAssetMenu(menuName = "Motorways/UI/NewContentData")]
	public class NewContentData : ScriptableObject
	{
		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06003465 RID: 13413 RVA: 0x000F6A6D File Offset: 0x000F4C6D
		public float DelayBetweenNciIntros
		{
			get
			{
				return this._delayBetweenNciIntros;
			}
		}

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06003466 RID: 13414 RVA: 0x000F6A78 File Offset: 0x000F4C78
		// (remove) Token: 0x06003467 RID: 13415 RVA: 0x000F6AB0 File Offset: 0x000F4CB0
		public event Action<string> onNewContentSeen;

		// Token: 0x06003468 RID: 13416 RVA: 0x000F6AE8 File Offset: 0x000F4CE8
		public bool IsNewContent(string newContentId, bool bypassNewContentData = false)
		{
			if (!Diagnostics.Verify(!string.IsNullOrWhiteSpace(newContentId)))
			{
				return false;
			}
			if (!bypassNewContentData)
			{
				NewContentDataEntry newContentDataEntry = this._entries.Find((NewContentDataEntry entry) => entry.newContentId.Equals(newContentId));
				if (newContentDataEntry == null)
				{
					return false;
				}
				using (List<Feature>.Enumerator enumerator = newContentDataEntry.requiredFeatures.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FeatureToggle.IsFeatureDisabled(enumerator.Current))
						{
							return false;
						}
					}
				}
			}
			return !this._activePlayer.HasSeenNewContent(newContentId);
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x000F6B98 File Offset: 0x000F4D98
		public void SetNewContentSeen(string newContentId)
		{
			this._activePlayer.SetNewContentSeen(newContentId);
			Action<string> action = this.onNewContentSeen;
			if (action == null)
			{
				return;
			}
			action(newContentId);
		}

		// Token: 0x04002CBC RID: 11452
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x04002CBD RID: 11453
		[ReadOnly]
		[Tooltip("If a new City needs to be added for NCIs, use this format for the ID below")]
		[SerializeField]
		private string _newCityNciIdFormat = "NewCity-{MapDefinition.cityName}";

		// Token: 0x04002CBE RID: 11454
		[SerializeField]
		private float _delayBetweenNciIntros;

		// Token: 0x04002CBF RID: 11455
		[SerializeField]
		private List<NewContentDataEntry> _entries;
	}
}
