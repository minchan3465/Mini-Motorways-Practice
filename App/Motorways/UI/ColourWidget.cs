using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x0200071F RID: 1823
	public class ColourWidget : MonoBehaviour, IView, ICreatedInScopeHandler, IReusable
	{
		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06003223 RID: 12835 RVA: 0x000ED27E File Offset: 0x000EB47E
		// (set) Token: 0x06003224 RID: 12836 RVA: 0x000ED290 File Offset: 0x000EB490
		private bool _swatchEclipseActive
		{
			get
			{
				return this.ColourButtonAnimator.GetBool("SwatchEclipse_Active");
			}
			set
			{
				this.ColourButtonAnimator.SetBool("SwatchEclipse_Active", value);
			}
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06003225 RID: 12837 RVA: 0x000ED2A3 File Offset: 0x000EB4A3
		// (set) Token: 0x06003226 RID: 12838 RVA: 0x000ED2B5 File Offset: 0x000EB4B5
		private bool _radialWidgetActive
		{
			get
			{
				return this.RadialColourWidgetAnimator.GetBool("SetActive");
			}
			set
			{
				this.RadialColourWidgetAnimator.SetBool("SetActive", value);
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06003227 RID: 12839 RVA: 0x000ED2C8 File Offset: 0x000EB4C8
		public int CurrentColour
		{
			get
			{
				return this._colourMovementCounter % this.GetColourGroupCount();
			}
		}

		// Token: 0x06003228 RID: 12840 RVA: 0x000ED2D7 File Offset: 0x000EB4D7
		private void ChangeColour()
		{
			this.RadialColourWidgetAnimator.SetTrigger("ChangeColour");
		}

		// Token: 0x06003229 RID: 12841 RVA: 0x000ED2E9 File Offset: 0x000EB4E9
		public void AfterColourChanged()
		{
			this._colourMovementCounter++;
			this.RefreshColours(false);
		}

		// Token: 0x0600322A RID: 12842 RVA: 0x000ED300 File Offset: 0x000EB500
		public void ColourButton()
		{
			Debug.Log("ColourButton pressed from ColourWidget.");
			if (this._waitForActivity != null)
			{
				base.StopCoroutine(this._waitForActivity);
			}
			this._waitForActivity = base.StartCoroutine(this.WaitForInactivity());
			if (!this._radialWidgetActive)
			{
				this.SetRadialColourWidgetVisible(true);
				return;
			}
			this.ChangeColour();
		}

		// Token: 0x0600322B RID: 12843 RVA: 0x000ED353 File Offset: 0x000EB553
		public void SetRadialColourWidgetVisible(bool visible)
		{
			this._swatchEclipseActive = visible;
			this._radialWidgetActive = visible;
		}

		// Token: 0x0600322C RID: 12844 RVA: 0x000ED363 File Offset: 0x000EB563
		private IEnumerator WaitForInactivity()
		{
			yield return new WaitForSeconds((float)this.InactiveTimerInSeconds);
			this.SetRadialColourWidgetVisible(false);
			yield break;
		}

		// Token: 0x0600322D RID: 12845 RVA: 0x000ED372 File Offset: 0x000EB572
		private int GetColourGroupCount()
		{
			return this._scope.Get<City>().Definition.schedulePlanner.demandOscillationData.Count;
		}

		// Token: 0x0600322E RID: 12846 RVA: 0x0000222C File Offset: 0x0000042C
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.ContinueTicking;
		}

		// Token: 0x0600322F RID: 12847 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		// Token: 0x06003230 RID: 12848 RVA: 0x000ED393 File Offset: 0x000EB593
		public void OnCreatedInScope(IScope scope)
		{
			this._scope = scope;
		}

		// Token: 0x06003231 RID: 12849 RVA: 0x000ED39C File Offset: 0x000EB59C
		private Color GetColorForIndex(int index)
		{
			return this._themeColors[(index + this._colourMovementCounter) % this._colourGroupCount].GetColor(ThemeComponentGroupTarget.BuildingBase);
		}

		// Token: 0x06003232 RID: 12850 RVA: 0x000ED3C0 File Offset: 0x000EB5C0
		public void RefreshColours(bool resetCounter = false)
		{
			if (resetCounter)
			{
				this._colourMovementCounter = 0;
			}
			this._colourGroupCount = this.GetColourGroupCount();
			Theme theme = this._scope.Get<IThemeDatabase>().GetTheme() as Theme;
			if (theme != null && theme.buildingColorGroups != null)
			{
				this._themeColors = theme.buildingColorGroups.GetRange(0, this._colourGroupCount);
			}
			Diagnostics.Verify(this.ColourSwatches.Length == 6, "There must be 6 colour swatches in the ColourWidget!");
			for (int colourIndex = 0; colourIndex < this.ColourSwatches.Length; colourIndex++)
			{
				ColourWidgetSwatch colourSwatch = this.ColourSwatches[colourIndex];
				colourSwatch.SwatchColor = this.GetColorForIndex(this._colourGroupCount - 2 + (colourSwatch.SwatchSlot - 1));
			}
			this.SetColourButtonSwatch.SwatchColor = this.ColourSwatches[2].SwatchColor;
		}

		// Token: 0x06003233 RID: 12851 RVA: 0x000ED489 File Offset: 0x000EB689
		public void Reset()
		{
			this._colourMovementCounter = 0;
			this._themeColors = null;
			this._colourGroupCount = 0;
			this._clickedSinceLastWait = false;
		}

		// Token: 0x04002B00 RID: 11008
		public Animator ColourButtonAnimator;

		// Token: 0x04002B01 RID: 11009
		public Animator RadialColourWidgetAnimator;

		// Token: 0x04002B02 RID: 11010
		public ColourWidgetSwatch SetColourButtonSwatch;

		// Token: 0x04002B03 RID: 11011
		public ColourWidgetSwatch[] ColourSwatches;

		// Token: 0x04002B04 RID: 11012
		public int InactiveTimerInSeconds = 5;

		// Token: 0x04002B05 RID: 11013
		public FloatingElement FloatingElement;

		// Token: 0x04002B06 RID: 11014
		public RectTransform RectTransform;

		// Token: 0x04002B07 RID: 11015
		public RectTransform HitboxRect;

		// Token: 0x04002B08 RID: 11016
		[Dependency]
		private IScope _scope;

		// Token: 0x04002B09 RID: 11017
		private const int ColourButtonIndex = 2;

		// Token: 0x04002B0A RID: 11018
		private int _colourMovementCounter;

		// Token: 0x04002B0B RID: 11019
		private int _colourGroupCount;

		// Token: 0x04002B0C RID: 11020
		private List<ColorGroup> _themeColors;

		// Token: 0x04002B0D RID: 11021
		private bool _clickedSinceLastWait;

		// Token: 0x04002B0E RID: 11022
		private Coroutine _waitForActivity;

		// Token: 0x04002B0F RID: 11023
		private int _currentColour;
	}
}
