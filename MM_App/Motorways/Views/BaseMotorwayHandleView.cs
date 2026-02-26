using System;
using Client;
using Factory;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000580 RID: 1408
	public class BaseMotorwayHandleView : MonoBehaviour
	{
		// Token: 0x060026BD RID: 9917 RVA: 0x000A4EDC File Offset: 0x000A30DC
		public virtual void Initialize(IScope parentScope, int motorwayNumber)
		{
			this._parentScope = parentScope;
			this._motorwayNumberLocText = base.GetComponentInChildren<LocalizedTextUI>();
			LocalizedTextUI motorwayNumberLocText = this._motorwayNumberLocText;
			this._textThemeComponent = ((motorwayNumberLocText != null) ? motorwayNumberLocText.GetComponent<ThemedComponent>() : null);
			if (!Diagnostics.Verify(this._motorwayNumberLocText != null, "No LocalizedTextUI in the UnbuiltMotorwayHandleView") || !Diagnostics.Verify(this._textThemeComponent != null, "No ThemedComponent on the  LocalizedTextUI in the UnbuiltMotorwayHandleView"))
			{
				return;
			}
			this._motorwayNumberLocText.HandleParentAllocated(parentScope);
			this._motorwayNumberLocText.LocString = StandaloneLocString.CreateLocalizedNumberString(parentScope, motorwayNumber);
		}

		// Token: 0x060026BE RID: 9918 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x060026BF RID: 9919 RVA: 0x000A4F63 File Offset: 0x000A3163
		public virtual void ApplyTheme(ITheme newTheme)
		{
			this._textThemeComponent.ApplyTheme(newTheme);
		}

		// Token: 0x060026C0 RID: 9920 RVA: 0x000A4F71 File Offset: 0x000A3171
		public virtual void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this._textThemeComponent.InitializeTheme(themeDatabase);
		}

		// Token: 0x060026C1 RID: 9921 RVA: 0x000A4F7F File Offset: 0x000A317F
		public virtual void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			this._textThemeComponent.ReleaseTheme(themeDatabase);
		}

		// Token: 0x040020AE RID: 8366
		private IScope _parentScope;

		// Token: 0x040020AF RID: 8367
		private LocalizedTextUI _motorwayNumberLocText;

		// Token: 0x040020B0 RID: 8368
		private ThemedComponent _textThemeComponent;
	}
}
