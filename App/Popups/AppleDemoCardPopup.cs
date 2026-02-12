using System;
using Factory;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
	// Token: 0x020002D1 RID: 721
	public class AppleDemoCardPopup : BasePopup
	{
		// Token: 0x060011B8 RID: 4536 RVA: 0x0003B040 File Offset: 0x00039240
		public void Initialise(bool showFrontCard = false)
		{
			if (showFrontCard)
			{
				this.AssignBestSuitedSprite(this._frontCards);
				return;
			}
			foreach (AppleDemoCardPopup.LocalizedFrontCards cards in this._cards)
			{
				if (cards.locale == this._locales.CurrentLocaleId)
				{
					this.AssignBestSuitedSprite(cards.sprites);
					return;
				}
			}
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x0003B096 File Offset: 0x00039296
		public override void OnOpened(float delay)
		{
			base.OnOpened(delay);
			this._timeOpened = Time.time;
			this.inputState.BlockAllInput = true;
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x0003B0B6 File Offset: 0x000392B6
		public override void OnClosed(Action onComplete = null, bool skipTransition = false)
		{
			this._popupStack.ResetReturnBlur();
			base.OnClosed(onComplete, false);
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x0003B0CB File Offset: 0x000392CB
		public override bool CanBeDismissed()
		{
			return Time.time - this._timeOpened > this.minimumTimeShown;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x0003B0E1 File Offset: 0x000392E1
		public void OnClicked()
		{
			this._pendingDismissal = true;
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x0003B0EC File Offset: 0x000392EC
		private void Update()
		{
			if (this.isFullyVisible)
			{
				float durationShown = Time.time - this._timeOpened;
				if (durationShown > this.maximumTimeShown)
				{
					this._pendingDismissal = true;
				}
				if (this._pendingDismissal && durationShown > this.minimumTimeShown)
				{
					this._popupStack.PopPopup(false);
				}
			}
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0003B13C File Offset: 0x0003933C
		private void AssignBestSuitedSprite(Sprite[] sprites)
		{
			Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
			float screenAspectRatio = (float)screenSize.x / (float)screenSize.y;
			int closestAspectIndex = 0;
			float closestAspectRatio = (float)sprites[closestAspectIndex].texture.width / (float)sprites[closestAspectIndex].texture.height;
			for (int candidateIndex = 1; candidateIndex < sprites.Length; candidateIndex++)
			{
				Vector2Int cardSize = new Vector2Int(sprites[candidateIndex].texture.width, sprites[candidateIndex].texture.height);
				float aspect = (float)cardSize.x / (float)cardSize.y;
				if (cardSize == screenSize)
				{
					closestAspectIndex = candidateIndex;
					break;
				}
				if (Mathf.Abs(aspect - screenAspectRatio) < Math.Abs(closestAspectRatio - screenAspectRatio))
				{
					closestAspectIndex = candidateIndex;
					closestAspectRatio = aspect;
				}
			}
			this._image.sprite = sprites[closestAspectIndex];
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x0003B20A File Offset: 0x0003940A
		public override void Reset()
		{
			this._pendingDismissal = false;
			this._timeOpened = 0f;
			base.Reset();
		}

		// Token: 0x04000F47 RID: 3911
		public float minimumTimeShown = 5f;

		// Token: 0x04000F48 RID: 3912
		public float maximumTimeShown = 10f;

		// Token: 0x04000F49 RID: 3913
		[Dependency]
		private LocaleDatabase _locales;

		// Token: 0x04000F4A RID: 3914
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F4B RID: 3915
		[SerializeField]
		private Image _image;

		// Token: 0x04000F4C RID: 3916
		private float _timeOpened;

		// Token: 0x04000F4D RID: 3917
		private bool _pendingDismissal;

		// Token: 0x04000F4E RID: 3918
		[SerializeField]
		private Sprite[] _frontCards;

		// Token: 0x04000F4F RID: 3919
		[SerializeField]
		private AppleDemoCardPopup.LocalizedFrontCards[] _cards = new AppleDemoCardPopup.LocalizedFrontCards[]
		{
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.ar,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.pt_BR,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.ca,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.de,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.it,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.ja,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.ko,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.nl,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.tr,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.zh_TW,
				sprites = new Sprite[5]
			},
			new AppleDemoCardPopup.LocalizedFrontCards
			{
				locale = LocaleDatabase.LocaleId.en_US,
				sprites = new Sprite[5]
			}
		};

		// Token: 0x020002D2 RID: 722
		[System.Serializable]
		public class LocalizedFrontCards
		{
			// Token: 0x04000F50 RID: 3920
			public LocaleDatabase.LocaleId locale;

			// Token: 0x04000F51 RID: 3921
			public Sprite[] sprites;
		}
	}
}
