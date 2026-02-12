using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200057D RID: 1405
	public class UpgradeBarHorizontalAnchorSizer : MonoBehaviour
	{
		// Token: 0x0600269D RID: 9885 RVA: 0x000A4382 File Offset: 0x000A2582
		public void Initialize(IScope scope)
		{
			this._visualConstants = scope.Get<VisualConstantsData>();
			this.BuildSecondaryList();
		}

		// Token: 0x0600269E RID: 9886 RVA: 0x000A4398 File Offset: 0x000A2598
		public void ToggleUpgradeGroups(bool enableLeftGroup, bool enableCenterGroup, bool enableRightGroup)
		{
			this._horizontalLayoutLeft.gameObject.SetActive(enableLeftGroup);
			this._horizontalLayoutLeftInactive.gameObject.SetActive(enableLeftGroup);
			this._horizontalLayoutCenter.SetActive(enableCenterGroup);
			this._horizontalLayoutCenterInactive.SetActive(enableCenterGroup);
			this._horizontalLayoutRight.gameObject.SetActive(enableRightGroup);
			this._horizontalLayoutRightInactive.gameObject.SetActive(enableRightGroup);
			this.UpdateSizing();
		}

		// Token: 0x0600269F RID: 9887 RVA: 0x000A4408 File Offset: 0x000A2608
		private void BuildSecondaryList()
		{
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[6]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[7]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[8]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[4]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[2]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[3]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[0]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[1]);
			this._upgradeButtonsSortedLeftToRight.Add(this._upgradeButtons[5]);
		}

		// Token: 0x060026A0 RID: 9888 RVA: 0x000A44E4 File Offset: 0x000A26E4
		public void UpdateSizing()
		{
			for (int i = 0; i < this._upgradeButtonsSortedLeftToRight.Count; i++)
			{
				int indexToCompareTo = -1;
				if (i < 6)
				{
					indexToCompareTo = i + 1;
				}
				else if (i > 7)
				{
					indexToCompareTo = i - 1;
				}
				int dividerIndex;
				if (i < 6)
				{
					dividerIndex = i;
				}
				else
				{
					dividerIndex = this._dividers.Count - 1;
				}
				float newWidth = this._upgradeButtonsSortedLeftToRight[i]._visualElementIcon.rect.width;
				if (indexToCompareTo > 0 && indexToCompareTo < this._upgradeButtonsSortedLeftToRight.Count)
				{
					UpgradeButtonCount count = this._upgradeButtonsSortedLeftToRight[indexToCompareTo]._count;
					if (count != null && count.AccountedIconNumber > 0)
					{
						this._dividers[dividerIndex].sizeDelta = new Vector2(this._visualConstants.UpgradeBarSeparationPaddingWithCount, 0f);
					}
					else if (count != null && count.AccountedIconNumber == 0 && this._upgradeButtonsSortedLeftToRight[indexToCompareTo]._anchor.gameObject.activeInHierarchy)
					{
						this._dividers[dividerIndex].sizeDelta = new Vector2(this._visualConstants.UpgradeBarSeparationPadding, 0f);
					}
					else if (count != null && count.AccountedIconNumber == 0 && !this._upgradeButtonsSortedLeftToRight[indexToCompareTo]._anchor.gameObject.activeInHierarchy)
					{
						this._dividers[dividerIndex].sizeDelta = new Vector2(0f, 0f);
					}
					else
					{
						this._dividers[dividerIndex].sizeDelta = new Vector2(this._visualConstants.UpgradeBarSeparationPadding, 0f);
					}
				}
				this._upgradeButtonsSortedLeftToRight[i]._anchor.sizeDelta = new Vector2(newWidth, 0f);
			}
			Rect concreteCounterRect = this._upgradeButtons[0]._visualElementCounter.rect;
			float halfConcreteWidth = this._upgradeButtons[0]._visualElementIcon.rect.width * 0.5f;
			if (!this._horizontalLayoutCenter.activeInHierarchy && !this._horizontalLayoutRight.gameObject.activeInHierarchy)
			{
				this._horizontalLayoutLeft.padding.right = 0;
				this._horizontalLayoutLeftInactive.padding.right = 0;
				this._horizontalLayoutLeft.childAlignment = TextAnchor.MiddleCenter;
				this._horizontalLayoutLeftInactive.childAlignment = TextAnchor.MiddleCenter;
			}
			else
			{
				int rightOffset = (int)(halfConcreteWidth + this._visualConstants.UpgradeBarSeparationPadding);
				int rightOffsetInactive = (int)(halfConcreteWidth + this._visualConstants.UpgradeBarLeftInactiveSeparationPadding);
				this._horizontalLayoutLeft.padding.right = rightOffset;
				this._horizontalLayoutLeftInactive.padding.right = rightOffsetInactive;
				this._horizontalLayoutLeft.childAlignment = TextAnchor.MiddleRight;
				this._horizontalLayoutLeftInactive.childAlignment = TextAnchor.MiddleRight;
			}
			float counterConcreteExtent = concreteCounterRect.x + concreteCounterRect.width * 0.5f;
			float rightSidePadding = Mathf.Max(halfConcreteWidth, counterConcreteExtent) + this._visualConstants.UpgradeBarRightSeparationPadding;
			this._horizontalLayoutRight.padding.left = (int)rightSidePadding;
			this._horizontalLayoutLeft.CalculateLayoutInputHorizontal();
			this._horizontalLayoutRight.CalculateLayoutInputHorizontal();
			this._horizontalLayoutLeft.SetLayoutHorizontal();
			this._horizontalLayoutRight.SetLayoutHorizontal();
		}

		// Token: 0x04002091 RID: 8337
		[SerializeField]
		private HorizontalLayoutGroup _horizontalLayoutLeft;

		// Token: 0x04002092 RID: 8338
		[SerializeField]
		private GameObject _horizontalLayoutCenter;

		// Token: 0x04002093 RID: 8339
		[SerializeField]
		private HorizontalLayoutGroup _horizontalLayoutRight;

		// Token: 0x04002094 RID: 8340
		[SerializeField]
		private HorizontalLayoutGroup _horizontalLayoutLeftInactive;

		// Token: 0x04002095 RID: 8341
		[SerializeField]
		private GameObject _horizontalLayoutCenterInactive;

		// Token: 0x04002096 RID: 8342
		[SerializeField]
		private HorizontalLayoutGroup _horizontalLayoutRightInactive;

		// Token: 0x04002097 RID: 8343
		[SerializeField]
		[Tooltip("in order of UpgradeType enum")]
		private List<UpgradeButtonHolder> _upgradeButtons = new List<UpgradeButtonHolder>();

		// Token: 0x04002098 RID: 8344
		private readonly List<UpgradeButtonHolder> _upgradeButtonsSortedLeftToRight = new List<UpgradeButtonHolder>();

		// Token: 0x04002099 RID: 8345
		[SerializeField]
		private List<RectTransform> _dividers = new List<RectTransform>();

		// Token: 0x0400209A RID: 8346
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x0400209B RID: 8347
		private const int _concreteIndex = 6;
	}
}
