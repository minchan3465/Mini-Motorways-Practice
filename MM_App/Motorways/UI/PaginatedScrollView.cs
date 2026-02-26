using System;
using Easing;
using Screens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200073C RID: 1852
	public class PaginatedScrollView : MonoBehaviour
	{
		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x060033B5 RID: 13237 RVA: 0x000F48EB File Offset: 0x000F2AEB
		// (set) Token: 0x060033B6 RID: 13238 RVA: 0x000F48F3 File Offset: 0x000F2AF3
		public int CurrentPage { get; private set; }

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x060033B7 RID: 13239 RVA: 0x000F48FC File Offset: 0x000F2AFC
		public int TotalPages
		{
			get
			{
				return this.pages.Length;
			}
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x000F4906 File Offset: 0x000F2B06
		public void SetPage(int pageNumber)
		{
			this.SetPage(pageNumber, false);
		}

		// Token: 0x060033B9 RID: 13241 RVA: 0x000F4910 File Offset: 0x000F2B10
		public void SetPage(int pageNumber, bool instantly)
		{
			if (this.CurrentPage == pageNumber)
			{
				return;
			}
			for (int pageIndex = 0; pageIndex < this.pages.Length; pageIndex++)
			{
				this.pages[pageIndex].gameObject.SetActive(pageIndex == pageNumber || pageIndex == this.CurrentPage);
				this.pages[pageIndex].SetInteractable(pageIndex == pageNumber);
			}
			if (!instantly)
			{
				if (pageNumber != this.CurrentPage)
				{
					this.SetScrollAmount((float)((pageNumber < this.CurrentPage) ? 0 : 1));
					float endPoint = (float)((pageNumber < this.CurrentPage) ? 1 : 0);
					float startingPoint = this.isHorizontal ? this.scrollRect.horizontalNormalizedPosition : this.scrollRect.verticalNormalizedPosition;
					this.progressFloat.Start(startingPoint, endPoint, 0.4f, Easings.Functions.QuinticEaseOut, 0f);
				}
			}
			else
			{
				this.SetScrollAmount((float)((pageNumber >= this.CurrentPage) ? 0 : 1));
			}
			this.CurrentPage = pageNumber;
			PaginatedScrollView.PageSelectedEvent onPageSelected = this._onPageSelected;
			if (onPageSelected == null)
			{
				return;
			}
			onPageSelected.Invoke(this.CurrentPage);
		}

		// Token: 0x060033BA RID: 13242 RVA: 0x000F4A09 File Offset: 0x000F2C09
		private void Start()
		{
			this.RefreshPageTransforms(0);
		}

		// Token: 0x060033BB RID: 13243 RVA: 0x000F4A12 File Offset: 0x000F2C12
		public Selectable GetFirstSelectableOnCurrentPage()
		{
			return this.pages[this.CurrentPage].gameObject.GetComponentInChildren<Selectable>();
		}

		// Token: 0x060033BC RID: 13244 RVA: 0x000F4A2C File Offset: 0x000F2C2C
		public void RefreshPageTransforms(int initialPageIndex = 0)
		{
			this.CurrentPage = 1;
			this.SetPage(initialPageIndex, true);
			if (!this.isHorizontal)
			{
				this.progressFloat.Stop();
				BaseScalingScreen componentInParent = base.GetComponentInParent<BaseScalingScreen>();
				Canvas baseCanvas = base.GetComponentInParent<Canvas>();
				CanvasScaler baseCanvasScaler = componentInParent.GetComponentInParent<CanvasScaler>();
				RectTransform component = componentInParent.GetComponent<RectTransform>();
				Vector2 baseRectSizeDelta = component.sizeDelta;
				RectTransform safeAreaRect = component.GetComponentInChildren<SafeArea>().GetComponent<RectTransform>();
				float safeAreaDelta = safeAreaRect.anchorMax.y - safeAreaRect.anchorMin.y;
				float pageHeight;
				float pageSpacing;
				switch (baseCanvas.renderMode)
				{
				case RenderMode.ScreenSpaceOverlay:
					switch (baseCanvasScaler.uiScaleMode)
					{
					case CanvasScaler.ScaleMode.ScaleWithScreenSize:
						switch (baseCanvasScaler.screenMatchMode)
						{
						case CanvasScaler.ScreenMatchMode.Expand:
						{
							float scaleRatio = baseRectSizeDelta.x / baseRectSizeDelta.y;
							float referenceRatio = baseCanvasScaler.referenceResolution.x / baseCanvasScaler.referenceResolution.y;
							float verticalScale = (scaleRatio < referenceRatio) ? (referenceRatio / scaleRatio) : 1f;
							pageHeight = baseCanvasScaler.referenceResolution.y * verticalScale * safeAreaDelta;
							pageSpacing = baseCanvasScaler.referenceResolution.y * verticalScale - pageHeight;
							goto IL_218;
						}
						}
						PaginatedScrollView.Log.Error("Paginated Scroll View might not support screen match mode {0}. Please ensure/implement!", new object[]
						{
							baseCanvasScaler.screenMatchMode
						});
						pageHeight = baseCanvasScaler.referenceResolution.y * safeAreaDelta;
						pageSpacing = baseCanvasScaler.referenceResolution.y - pageHeight;
						goto IL_218;
					}
					PaginatedScrollView.Log.Error("Paginated Scroll View might not support ui scale mode {0}. Please ensure/implement!", new object[]
					{
						baseCanvasScaler.uiScaleMode
					});
					pageHeight = baseCanvasScaler.referenceResolution.y * safeAreaDelta;
					pageSpacing = baseCanvasScaler.referenceResolution.y - pageHeight;
					goto IL_218;
				case RenderMode.WorldSpace:
					pageHeight = baseRectSizeDelta.y * safeAreaDelta;
					pageSpacing = baseRectSizeDelta.y - pageHeight;
					goto IL_218;
				}
				PaginatedScrollView.Log.Error("Paginated Scroll View might not support render mode {0}. Please ensure/implement!", new object[]
				{
					baseCanvas.renderMode
				});
				pageHeight = baseCanvasScaler.referenceResolution.y * safeAreaDelta;
				pageSpacing = baseCanvasScaler.referenceResolution.y - pageHeight;
				IL_218:
				this._layoutGroup.spacing = pageSpacing;
				for (int childIndex = 0; childIndex < this.scrollRect.content.childCount; childIndex++)
				{
					RectTransform component2 = this.scrollRect.content.GetChild(childIndex).GetComponent<RectTransform>();
					component2.sizeDelta = new Vector2(component2.sizeDelta.x, pageHeight);
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.scrollRect.content.GetComponent<RectTransform>());
				this.RefreshScrollAmount();
				return;
			}
			throw new NotImplementedException("Horizontal paginated scroll view is not yet supported.");
		}

		// Token: 0x060033BD RID: 13245 RVA: 0x000F4CD3 File Offset: 0x000F2ED3
		private void RefreshScrollAmount()
		{
			if (this.isHorizontal)
			{
				this.SetScrollAmount(Mathf.Round(this.scrollRect.horizontalNormalizedPosition));
				return;
			}
			this.SetScrollAmount(Mathf.Round(this.scrollRect.verticalNormalizedPosition));
		}

		// Token: 0x060033BE RID: 13246 RVA: 0x000F4D0A File Offset: 0x000F2F0A
		private void SetScrollAmount(float amount)
		{
			if (this.isHorizontal)
			{
				this.scrollRect.horizontalNormalizedPosition = amount;
				return;
			}
			this.scrollRect.verticalNormalizedPosition = amount;
		}

		// Token: 0x060033BF RID: 13247 RVA: 0x000F4D30 File Offset: 0x000F2F30
		private void Update()
		{
			if (this.progressFloat.IsActive)
			{
				float newProgress = this.progressFloat.Tick(Time.deltaTime);
				this.SetScrollAmount(newProgress);
			}
		}

		// Token: 0x04002C2D RID: 11309
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("PaginatedScrollView");

		// Token: 0x04002C2E RID: 11310
		public bool isHorizontal;

		// Token: 0x04002C2F RID: 11311
		public DelegateCanvasGroup[] pages;

		// Token: 0x04002C30 RID: 11312
		[SerializeField]
		private VerticalLayoutGroup _layoutGroup;

		// Token: 0x04002C31 RID: 11313
		[SerializeField]
		private PaginatedScrollView.PageSelectedEvent _onPageSelected = new PaginatedScrollView.PageSelectedEvent();

		// Token: 0x04002C33 RID: 11315
		public ScrollRect scrollRect;

		// Token: 0x04002C34 RID: 11316
		public TweenFloat progressFloat = new TweenFloat();

		// Token: 0x0200073D RID: 1853
		[Serializable]
		public class PageSelectedEvent : UnityEvent<int>
		{
		}
	}
}
