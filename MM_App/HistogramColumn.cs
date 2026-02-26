using System;
using Easing;
using Motorways.Themes;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class HistogramColumn : MonoBehaviour
{
	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x00024DAC File Offset: 0x00022FAC
	public float NumberOfEntries
	{
		get
		{
			return this._numberOfEntries;
		}
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06000AFA RID: 2810 RVA: 0x00024DB4 File Offset: 0x00022FB4
	public float StartRange
	{
		get
		{
			return this._startRange;
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06000AFB RID: 2811 RVA: 0x00024DBC File Offset: 0x00022FBC
	public float EndRange
	{
		get
		{
			return this._endRange;
		}
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00024DC4 File Offset: 0x00022FC4
	public void Initialise(float startRange, float endRange, float numberOfEntries, bool evenColumn)
	{
		this._startRange = startRange;
		this._endRange = endRange;
		this._numberOfEntries = numberOfEntries;
		this._toggler.SetSelectedTheme(evenColumn);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00024DE8 File Offset: 0x00022FE8
	public void SetHeight(float height, float duration, float delay, Easings.Functions easingFunction)
	{
		this.SubRectTransform.sizeDelta = new Vector2(0f, 0f);
		this._targetHeight = height;
		this._tweenTimer = -delay;
		this._tweenDuration = duration;
		this._tweenFunction = easingFunction;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x00024E24 File Offset: 0x00023024
	public void Update()
	{
		if (this._tweenDuration > 0f)
		{
			this._tweenTimer += Time.deltaTime;
			float newHeight = this._targetHeight;
			if (this._tweenTimer >= this._tweenDuration)
			{
				this._tweenDuration = 0f;
			}
			else if (this._tweenTimer <= 0f)
			{
				newHeight = 0f;
			}
			else
			{
				newHeight = this._targetHeight * Easings.Interpolate(this._tweenTimer / this._tweenDuration, this._tweenFunction);
			}
			this.SubRectTransform.sizeDelta = new Vector2(0f, newHeight);
		}
	}

	// Token: 0x04000615 RID: 1557
	public RectTransform RectTransform;

	// Token: 0x04000616 RID: 1558
	public RectTransform SubRectTransform;

	// Token: 0x04000617 RID: 1559
	[SerializeField]
	private ThemeTypeToggler _toggler;

	// Token: 0x04000618 RID: 1560
	private float _startRange;

	// Token: 0x04000619 RID: 1561
	private float _endRange;

	// Token: 0x0400061A RID: 1562
	private float _numberOfEntries;

	// Token: 0x0400061B RID: 1563
	private float _targetHeight;

	// Token: 0x0400061C RID: 1564
	private float _tweenTimer;

	// Token: 0x0400061D RID: 1565
	private float _tweenDuration;

	// Token: 0x0400061E RID: 1566
	private Easings.Functions _tweenFunction;
}
