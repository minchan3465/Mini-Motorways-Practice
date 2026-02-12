using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BB RID: 443
[Serializable]
public class ChallengeIcon : MonoBehaviour
{
	// Token: 0x06000A75 RID: 2677 RVA: 0x000228F0 File Offset: 0x00020AF0
	public void SetChallengeIcons(Sprite challengeIconSprite, bool isWildcardChallenge)
	{
		this.SetChallengeIcons(challengeIconSprite, isWildcardChallenge, null, null);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x000228FC File Offset: 0x00020AFC
	public void SetChallengeIcons(Sprite challengeIconSprite, bool isWildcardChallenge, Sprite subIcon, Sprite subiconBackground)
	{
		this._challengeIcon.sprite = challengeIconSprite;
		if (subIcon != null && subiconBackground != null)
		{
			this._challengeSubIcon.enabled = true;
			this._challengeSubIconBackground.enabled = true;
			this._challengeSubIcon.sprite = subIcon;
			this._challengeSubIconBackground.sprite = subiconBackground;
		}
		else
		{
			this._challengeSubIcon.enabled = false;
			this._challengeSubIconBackground.enabled = false;
		}
		this._normalBackground.gameObject.SetActive(!isWildcardChallenge);
		this._wildcardBackground.gameObject.SetActive(isWildcardChallenge);
	}

	// Token: 0x0400058A RID: 1418
	[SerializeField]
	private Image _challengeIcon;

	// Token: 0x0400058B RID: 1419
	[SerializeField]
	private Image _challengeSubIcon;

	// Token: 0x0400058C RID: 1420
	[SerializeField]
	private Image _challengeSubIconBackground;

	// Token: 0x0400058D RID: 1421
	[SerializeField]
	private Image _normalBackground;

	// Token: 0x0400058E RID: 1422
	[SerializeField]
	private Image _wildcardBackground;
}
