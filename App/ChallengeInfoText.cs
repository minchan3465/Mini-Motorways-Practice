using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class ChallengeInfoText : MonoBehaviour
{
	// Token: 0x06000A78 RID: 2680 RVA: 0x00022998 File Offset: 0x00020B98
	public void SetChallengeInfo(ChallengeData data, bool isWildcard, IScope scope)
	{
		this._challengeIcon.SetChallengeIcons(data.icon, isWildcard, data.subIcon, data.subIconBackground);
		float localizationParameter = data.GetSelectedModifierLocalizationParameter();
		MotorwaysStringKey titleKey = scope.Get<MotorwaysStringKey>();
		StringId challengeTitleStringId;
		if (!string.IsNullOrEmpty(data.challengeName) && Diagnostics.Verify(Enum.TryParse<StringId>(data.challengeName, out challengeTitleStringId), "{0} is an invalid string id!", data.challengeDescription))
		{
			titleKey.InitWithStringId(challengeTitleStringId, data.GetSelectedModifierLocalizationParameter(), new Dictionary<string, string>
			{
				{
					"Num",
					localizationParameter.ToString()
				}
			});
			this._challengeHeader.LocString = StandaloneLocString.CreateString(scope, titleKey);
		}
		if (!string.IsNullOrEmpty(data.challengeDescription))
		{
			MotorwaysStringKey descriptionKey = scope.Get<MotorwaysStringKey>();
			StringId challengeDescriptionStringId;
			if (Diagnostics.Verify(Enum.TryParse<StringId>(data.challengeDescription, out challengeDescriptionStringId), "{0} is an invalid string id!", data.challengeDescription))
			{
				descriptionKey.InitWithStringId(challengeDescriptionStringId, data.GetSelectedModifierLocalizationParameter(), new Dictionary<string, string>
				{
					{
						"Num",
						localizationParameter.ToString()
					}
				});
				this._challengeDescription.LocString = StandaloneLocString.CreateString(scope, descriptionKey);
			}
		}
	}

	// Token: 0x0400058F RID: 1423
	[SerializeField]
	private LocalizedTextUI _challengeHeader;

	// Token: 0x04000590 RID: 1424
	[SerializeField]
	private LocalizedTextUI _challengeDescription;

	// Token: 0x04000591 RID: 1425
	[SerializeField]
	private ChallengeIcon _challengeIcon;
}
