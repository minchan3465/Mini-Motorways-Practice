using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI.CreditsScreen
{
	// Token: 0x0200075C RID: 1884
	public class CreditsSectionView : MonoBehaviour
	{
		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06003488 RID: 13448 RVA: 0x000F7006 File Offset: 0x000F5206
		public CreditsSectionStyle Style
		{
			get
			{
				return this._style;
			}
		}

		// Token: 0x06003489 RID: 13449 RVA: 0x000F700E File Offset: 0x000F520E
		public void SetHeaderText(string text, string localizationId)
		{
			this._header.text = text;
			if (string.IsNullOrEmpty(localizationId))
			{
				localizationId = "None";
			}
			this._header.GetComponent<LocalizedTextUI>().startingStringIdString = localizationId;
		}

		// Token: 0x0600348A RID: 13450 RVA: 0x000F703C File Offset: 0x000F523C
		public void SetContentText(string text, bool alphabetize)
		{
			string[] strings = text.Split('\n', StringSplitOptions.None);
			if (alphabetize)
			{
				Array.Sort<string>(strings);
			}
			if (this.UseColumns())
			{
				ValueTuple<string[], string[]> columns = this.SeparateEvenly(strings);
				this._contentLeftColumn.text = string.Join("\n", columns.Item1);
				this._contentRightColumn.text = string.Join("\n", columns.Item2);
				return;
			}
			this._content.text = string.Join("\n", strings);
		}

		// Token: 0x0600348B RID: 13451 RVA: 0x000F70B9 File Offset: 0x000F52B9
		public void SetLogoSprite(Sprite logoSprite)
		{
			if (this.LogoImage != null)
			{
				this.LogoImage.sprite = logoSprite;
				return;
			}
			base.GetComponentInChildren<Image>().sprite = logoSprite;
		}

		// Token: 0x0600348C RID: 13452 RVA: 0x000F70E2 File Offset: 0x000F52E2
		private bool UseColumns()
		{
			if (this._content != null)
			{
				return false;
			}
			if (this._contentLeftColumn != null && this._contentRightColumn != null)
			{
				return true;
			}
			Diagnostics.FailAssert("Credits Section View is set up incorrectly! Either the Content, (exclusive) or both of the ContentColumns should be assigned.", Array.Empty<object>());
			return false;
		}

		// Token: 0x0600348D RID: 13453 RVA: 0x000F7124 File Offset: 0x000F5324
		private ValueTuple<string[], string[]> SeparateEvenly(string[] strings)
		{
			int totalCount = strings.Length;
			int num = (totalCount % 2 == 0) ? (totalCount / 2) : (totalCount / 2 + 1);
			int rightCount = totalCount / 2;
			string[] leftColumn = new string[num];
			string[] rightColumn = new string[rightCount];
			bool useLeftColumn = true;
			for (int i = 0; i < totalCount; i++)
			{
				(useLeftColumn ? leftColumn : rightColumn)[i / 2] = strings[i];
				useLeftColumn = !useLeftColumn;
			}
			return new ValueTuple<string[], string[]>(leftColumn, rightColumn);
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x000F7187 File Offset: 0x000F5387
		private bool ShouldShowHeader()
		{
			return this.Style == CreditsSectionStyle.License || this.Style == CreditsSectionStyle.JumboHeader || this.Style == CreditsSectionStyle.SmallHeader || this.Style == CreditsSectionStyle.StandardCredits || this.Style == CreditsSectionStyle.TwoColumnCredits;
		}

		// Token: 0x0600348F RID: 13455 RVA: 0x000F71B7 File Offset: 0x000F53B7
		private bool ShouldShowContent()
		{
			return this.Style == CreditsSectionStyle.License || this.Style == CreditsSectionStyle.StandardCredits;
		}

		// Token: 0x06003490 RID: 13456 RVA: 0x000F71CD File Offset: 0x000F53CD
		private bool ShouldShowColumns()
		{
			return this.Style == CreditsSectionStyle.TwoColumnCredits;
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x000F71D8 File Offset: 0x000F53D8
		private bool ShouldShowLogo()
		{
			return this.Style == CreditsSectionStyle.Logo;
		}

		// Token: 0x04002CE5 RID: 11493
		[SerializeField]
		private CreditsSectionStyle _style;

		// Token: 0x04002CE6 RID: 11494
		[ShowIf("ShouldShowHeader")]
		[SerializeField]
		private TextMeshProUGUI _header;

		// Token: 0x04002CE7 RID: 11495
		[SerializeField]
		[ShowIf("ShouldShowContent")]
		private TextMeshProUGUI _content;

		// Token: 0x04002CE8 RID: 11496
		[ShowIf("ShouldShowColumns")]
		[SerializeField]
		private TextMeshProUGUI _contentLeftColumn;

		// Token: 0x04002CE9 RID: 11497
		[SerializeField]
		[ShowIf("ShouldShowColumns")]
		private TextMeshProUGUI _contentRightColumn;

		// Token: 0x04002CEA RID: 11498
		[ShowIf("ShouldShowLogo")]
		[SerializeField]
		private Image LogoImage;
	}
}
