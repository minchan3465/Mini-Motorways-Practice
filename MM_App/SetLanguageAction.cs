using System;
using Factory;

// Token: 0x02000190 RID: 400
public class SetLanguageAction : PlayerAction
{
	// Token: 0x0600090F RID: 2319 RVA: 0x0001D978 File Offset: 0x0001BB78
	public override void OnActionBegin(float timestamp)
	{
		base.OnActionBegin(timestamp);
		this._player.LocaleId = this._localeId;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000020A2 File Offset: 0x000002A2
	public override void Tick(float frameTime)
	{
		this.OnActionComplete();
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0001D994 File Offset: 0x0001BB94
	private void CycleLocaleId(SetLanguageAction.CycleLanguageDirection direction)
	{
		Locale currentLocale = this._locales.CurrentLocale;
		int currentLocaleIndex = this._locales.GetIndex(currentLocale);
		int nextLocaleIndex = (direction == SetLanguageAction.CycleLanguageDirection.Forward) ? (currentLocaleIndex + 1) : (currentLocaleIndex - 1);
		if (nextLocaleIndex >= this._locales.LocaleCount)
		{
			nextLocaleIndex = 0;
		}
		if (nextLocaleIndex < 0)
		{
			nextLocaleIndex = this._locales.LocaleCount - 1;
		}
		this._localeId = this._locales.GetLocale(nextLocaleIndex).Id;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0001D9FF File Offset: 0x0001BBFF
	public static SetLanguageAction CreateCycleForwardSetLanguageAction(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		SetLanguageAction setLanguageAction = scope.Get<SetLanguageAction>();
		setLanguageAction.CycleLocaleId(SetLanguageAction.CycleLanguageDirection.Forward);
		setLanguageAction.InitializeAction(owningGroup, timestamp);
		setLanguageAction.OnActionBegin(timestamp);
		return setLanguageAction;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0001DA1D File Offset: 0x0001BC1D
	public static SetLanguageAction CreateCycleBackwardSetLanguageAction(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		SetLanguageAction setLanguageAction = scope.Get<SetLanguageAction>();
		setLanguageAction.CycleLocaleId(SetLanguageAction.CycleLanguageDirection.Backward);
		setLanguageAction.InitializeAction(owningGroup, timestamp);
		setLanguageAction.OnActionBegin(timestamp);
		return setLanguageAction;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0001DA3B File Offset: 0x0001BC3B
	public override void Reset()
	{
		base.Reset();
		this._localeId = LocaleDatabase.LocaleId.en_US;
	}

	// Token: 0x04000487 RID: 1159
	[Dependency]
	protected IActivePlayer _player;

	// Token: 0x04000488 RID: 1160
	[Dependency]
	private LocaleDatabase _locales;

	// Token: 0x04000489 RID: 1161
	private LocaleDatabase.LocaleId _localeId = LocaleDatabase.LocaleId.en_US;

	// Token: 0x02000191 RID: 401
	private enum CycleLanguageDirection
	{
		// Token: 0x0400048B RID: 1163
		Forward,
		// Token: 0x0400048C RID: 1164
		Backward
	}
}
