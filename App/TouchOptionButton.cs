using System;
using System.Collections.Generic;
using Motorways.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DA RID: 474
public class TouchOptionButton : OptionButton
{
	// Token: 0x06000B50 RID: 2896 RVA: 0x000269D7 File Offset: 0x00024BD7
	private void Start()
	{
		if (this.NumberOfOptions > 0 && this._currentIndex < 0)
		{
			this.SetOption(0, false);
		}
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x000269F3 File Offset: 0x00024BF3
	private void OnEnable()
	{
		if (this.NumberOfOptions > 0)
		{
			this.SetOption(this._currentIndex, false);
		}
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x00026A0C File Offset: 0x00024C0C
	public void OnLeftPressed()
	{
		int iterations = 0;
		int newIndex = this._currentIndex;
		do
		{
			newIndex--;
			iterations++;
			if (newIndex < 0)
			{
				newIndex = (this.wrap ? (this.NumberOfOptions - 1) : 0);
			}
		}
		while (this._blockedOptions.Contains(newIndex) && iterations <= this.NumberOfOptions);
		if (Diagnostics.Verify(iterations <= this.NumberOfOptions, "We've skipped more options than are available on {0}", base.name))
		{
			this.SetOption(newIndex);
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00026A80 File Offset: 0x00024C80
	public void OnRightPressed()
	{
		int iterations = 0;
		int newIndex = this._currentIndex;
		do
		{
			newIndex++;
			iterations++;
			if (newIndex >= this.NumberOfOptions)
			{
				newIndex = (this.wrap ? 0 : (this.NumberOfOptions - 1));
			}
		}
		while (this._blockedOptions.Contains(newIndex) && iterations <= this.NumberOfOptions);
		if (iterations <= this.NumberOfOptions)
		{
			this.SetOption(newIndex);
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00026AE2 File Offset: 0x00024CE2
	public void SkipOption(int optionIndex)
	{
		if (!this._blockedOptions.Contains(optionIndex))
		{
			this._blockedOptions.Add(optionIndex);
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00026AFE File Offset: 0x00024CFE
	public void UnskipOption(int optionIndex)
	{
		if (this._blockedOptions.Contains(optionIndex))
		{
			this._blockedOptions.Remove(optionIndex);
		}
	}

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06000B56 RID: 2902 RVA: 0x00026B1B File Offset: 0x00024D1B
	public override int NumberOfOptions
	{
		get
		{
			return this.options.Length;
		}
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00026B28 File Offset: 0x00024D28
	public override void SetOption(int index, bool invokeMethod)
	{
		base.SetOption(index, invokeMethod);
		for (int optionIndex = 0; optionIndex < this.options.Length; optionIndex++)
		{
			this.options[optionIndex].SetActive(optionIndex == this._currentIndex);
		}
		if (!this.wrap)
		{
			int firstValidIndex = 0;
			while (this._blockedOptions.Contains(firstValidIndex))
			{
				firstValidIndex++;
			}
			bool leftClamped = this._currentIndex == firstValidIndex;
			int lastValidIndex = this.options.Length - 1;
			while (this._blockedOptions.Contains(lastValidIndex))
			{
				lastValidIndex--;
			}
			bool rightClamped = this._currentIndex == lastValidIndex;
			switch (this._rangeClampTransition)
			{
			case TouchOptionButton.RangeClampTransition.None:
				break;
			case TouchOptionButton.RangeClampTransition.UnInteractable:
				if (this.leftButton != null)
				{
					this.leftButton.interactable = !leftClamped;
				}
				if (this.rightButton != null)
				{
					this.rightButton.interactable = !rightClamped;
					return;
				}
				break;
			case TouchOptionButton.RangeClampTransition.Animation:
				if (this.leftButton != null)
				{
					this.leftButton.animator.SetBool(this._clampedAnimParam, leftClamped);
				}
				if (this.rightButton != null)
				{
					this.rightButton.animator.SetBool(this._clampedAnimParam, rightClamped);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x04000676 RID: 1654
	public GameObject[] options;

	// Token: 0x04000677 RID: 1655
	public TouchButton leftButton;

	// Token: 0x04000678 RID: 1656
	public TouchButton rightButton;

	// Token: 0x04000679 RID: 1657
	[SerializeField]
	private List<int> _blockedOptions = new List<int>();

	// Token: 0x0400067A RID: 1658
	[SerializeField]
	private TouchOptionButton.RangeClampTransition _rangeClampTransition = TouchOptionButton.RangeClampTransition.UnInteractable;

	// Token: 0x0400067B RID: 1659
	[SerializeField]
	private string _clampedAnimParam;

	// Token: 0x020001DB RID: 475
	public enum RangeClampTransition
	{
		// Token: 0x0400067D RID: 1661
		None,
		// Token: 0x0400067E RID: 1662
		UnInteractable,
		// Token: 0x0400067F RID: 1663
		Animation
	}
}
