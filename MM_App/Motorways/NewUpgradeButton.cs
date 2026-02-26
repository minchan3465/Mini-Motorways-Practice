using System;
using Client;
using Motorways.Themes;
using Motorways.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x0200044C RID: 1100
	public class NewUpgradeButton : TouchButton, IThemeComponent
	{
		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06001B4E RID: 6990 RVA: 0x00064021 File Offset: 0x00062221
		public UpgradeIcon PrimaryIcon
		{
			get
			{
				return this.icons[0];
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0006402B File Offset: 0x0006222B
		public UpgradeIcon SecondaryIcon
		{
			get
			{
				return this.icons[1];
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06001B50 RID: 6992 RVA: 0x00064035 File Offset: 0x00062235
		public NumberBubble PrimaryNumberBubble
		{
			get
			{
				return this.numberBubbles[0];
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06001B51 RID: 6993 RVA: 0x0006403F File Offset: 0x0006223F
		public NumberBubble SecondaryNumberBubble
		{
			get
			{
				return this.numberBubbles[1];
			}
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x00064049 File Offset: 0x00062249
		public RectTransform GetIconRect(int index)
		{
			return this.icons[index].GetComponent<RectTransform>();
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06001B53 RID: 6995 RVA: 0x00064058 File Offset: 0x00062258
		// (set) Token: 0x06001B54 RID: 6996 RVA: 0x00064065 File Offset: 0x00062265
		public Sprite Sprite
		{
			get
			{
				return this.imageRenderer.sprite;
			}
			set
			{
				this.imageRenderer.sprite = value;
			}
		}

		// Token: 0x06001B55 RID: 6997 RVA: 0x00064073 File Offset: 0x00062273
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.SetHighlighted(true);
		}

		// Token: 0x06001B56 RID: 6998 RVA: 0x00064083 File Offset: 0x00062283
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			this.SetHighlighted(false);
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x00064093 File Offset: 0x00062293
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.SetHighlighted(true);
		}

		// Token: 0x06001B58 RID: 7000 RVA: 0x000640A3 File Offset: 0x000622A3
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.SetHighlighted(false);
		}

		// Token: 0x06001B59 RID: 7001 RVA: 0x000640B4 File Offset: 0x000622B4
		private void SetHighlighted(bool isHighlighted)
		{
			for (int iconIndex = 0; iconIndex < this.icons.Length; iconIndex++)
			{
				this.icons[iconIndex].IsHighlighted = isHighlighted;
			}
		}

		// Token: 0x06001B5A RID: 7002 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06001B5B RID: 7003 RVA: 0x000640E4 File Offset: 0x000622E4
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			ThemeBlendingResult blendingResult = ThemeBlendingResult.StopBlending;
			this._currentTheme = (newTheme as Theme);
			for (int iconIndex = 0; iconIndex < this.icons.Length; iconIndex++)
			{
				if (this.icons[iconIndex].ApplyBlendedTheme(oldTheme, newTheme, progress) == ThemeBlendingResult.ContinueBlending)
				{
					blendingResult = ThemeBlendingResult.ContinueBlending;
				}
			}
			return blendingResult;
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x00064128 File Offset: 0x00062328
		public void ApplyTheme(ITheme newTheme)
		{
			this._currentTheme = (newTheme as Theme);
			for (int iconIndex = 0; iconIndex < this.icons.Length; iconIndex++)
			{
				this.icons[iconIndex].ApplyTheme(newTheme);
			}
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06001B5E RID: 7006 RVA: 0x00064164 File Offset: 0x00062364
		public void SetInteractable(bool isInteractable)
		{
			base.interactable = isInteractable;
			base.transform.localScale = Vector3.one;
			if (!base.interactable)
			{
				base.transform.localScale *= this.disabledScale;
			}
			UpgradeIcon[] array = this.icons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].IsDisabled = !isInteractable;
			}
			foreach (ThemedComponent component in this.nestedThemeComponents)
			{
				if (isInteractable)
				{
					component.ApplyTheme(this._currentTheme);
				}
				else
				{
					component.SetColor(this._currentTheme.GetColor(ThemedMaterialType.DisabledUpgradeOption, "_Color"));
				}
			}
		}

		// Token: 0x040016D5 RID: 5845
		public UpgradeType primaryUpgradeType;

		// Token: 0x040016D6 RID: 5846
		public Image imageRenderer;

		// Token: 0x040016D7 RID: 5847
		public LocalizedTextUI buttonName;

		// Token: 0x040016D8 RID: 5848
		public LocalizedTextUI buttonAdditionalConcrete;

		// Token: 0x040016D9 RID: 5849
		public LocalizedTextUI buttonDescription;

		// Token: 0x040016DA RID: 5850
		public UpgradeIcon[] icons;

		// Token: 0x040016DB RID: 5851
		public NumberBubble[] numberBubbles;

		// Token: 0x040016DC RID: 5852
		public RectTransform iconParent;

		// Token: 0x040016DD RID: 5853
		public float disabledScale = 0.7f;

		// Token: 0x040016DE RID: 5854
		public ThemedComponent[] nestedThemeComponents;

		// Token: 0x040016DF RID: 5855
		private Theme _currentTheme;
	}
}
