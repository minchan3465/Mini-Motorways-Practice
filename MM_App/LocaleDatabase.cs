using System;
using System.Collections.Generic;
using Factory;
using Motorways.UI;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class LocaleDatabase : IReleasedFromScopeHandler
{
	// Token: 0x060008C1 RID: 2241 RVA: 0x0001CAA1 File Offset: 0x0001ACA1
	public Locale GetLocale(int index)
	{
		return this._locales[index];
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0001CAB0 File Offset: 0x0001ACB0
	public Locale GetLocale(LocaleDatabase.LocaleId localeId)
	{
		for (int localeIndex = 0; localeIndex < this._locales.Count; localeIndex++)
		{
			if (this._locales[localeIndex].Id == localeId)
			{
				return this._locales[localeIndex];
			}
		}
		return null;
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0001CAF8 File Offset: 0x0001ACF8
	public Locale GetLocale(string localeIdString)
	{
		LocaleDatabase.LocaleId parsedLocaleId = LocaleDatabase.LocaleId.Unknown;
		if (Enum.TryParse<LocaleDatabase.LocaleId>(localeIdString, false, out parsedLocaleId))
		{
			return this.GetLocale(parsedLocaleId);
		}
		return null;
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x060008C4 RID: 2244 RVA: 0x0001CB1B File Offset: 0x0001AD1B
	public int LocaleCount
	{
		get
		{
			return this._locales.Count;
		}
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0001CB28 File Offset: 0x0001AD28
	public bool IsLocaleSelectable(LocaleDatabase.LocaleId localeId)
	{
		Locale locale = this.GetLocale(localeId);
		return locale != null && locale.IsSelectable;
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0001CB48 File Offset: 0x0001AD48
	public int GetIndex(Locale locale)
	{
		for (int localeIndex = 0; localeIndex < this._locales.Count; localeIndex++)
		{
			if (this._locales[localeIndex] == locale)
			{
				return localeIndex;
			}
		}
		return -1;
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x060008C7 RID: 2247 RVA: 0x0001CB7D File Offset: 0x0001AD7D
	public Locale CurrentLocale
	{
		get
		{
			return this._currentLocale;
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x060008C8 RID: 2248 RVA: 0x0001CB85 File Offset: 0x0001AD85
	public Locale FallbackLocale
	{
		get
		{
			return this._fallbackLocale;
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x060008C9 RID: 2249 RVA: 0x0001CB8D File Offset: 0x0001AD8D
	// (set) Token: 0x060008CA RID: 2250 RVA: 0x0001CBA4 File Offset: 0x0001ADA4
	public LocaleDatabase.LocaleId CurrentLocaleId
	{
		get
		{
			if (this._currentLocale == null)
			{
				return LocaleDatabase.LocaleId.Unknown;
			}
			return this._currentLocale.Id;
		}
		protected set
		{
			Locale newLocale = this.GetLocale(value);
			if (newLocale != null && this._currentLocale != newLocale)
			{
				this._currentLocale = newLocale;
				int localizedObjectIndex = 0;
				while (localizedObjectIndex < this._localizedObjects.Count)
				{
					if (!this._localizedObjects[localizedObjectIndex].IsAlive)
					{
						this._localizedObjects.RemoveAt(localizedObjectIndex);
					}
					else
					{
						ILocalized localizedObject = this._localizedObjects[localizedObjectIndex].Target as ILocalized;
						if (localizedObject != null)
						{
							localizedObject.HandleLocaleChanged(newLocale);
						}
						localizedObjectIndex++;
					}
				}
			}
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0001CC24 File Offset: 0x0001AE24
	public Locale MatchLocale(string locale)
	{
		LocaleDatabase.Log.Info("Attempting to matching locale {0}.", new object[]
		{
			locale
		});
		locale = locale.Replace("-", "_");
		string[] localeChunks = locale.Split('_', StringSplitOptions.None);
		if (localeChunks.Length == 0)
		{
			return null;
		}
		string language = localeChunks[0];
		string lcid = language;
		if (language == "nb")
		{
			language = "no";
			lcid = "no";
		}
		if (localeChunks.Length > 1)
		{
			lcid = language + "_" + localeChunks[1];
		}
		if (!(lcid == "zh_Hant"))
		{
			if (lcid == "zh_Hans")
			{
				lcid = "zh_CN";
			}
		}
		else if (localeChunks.Length > 2 && localeChunks[2] == "HK")
		{
			lcid = "zh_HK";
		}
		else
		{
			lcid = "zh_TW";
		}
		Locale matchedLocale = this.GetLocale(lcid);
		if (matchedLocale != null)
		{
			return matchedLocale;
		}
		if (language == "en")
		{
			lcid = "en_GB";
		}
		else if (language == "es")
		{
			if (localeChunks.Length > 1)
			{
				lcid = "es_MX";
			}
			else
			{
				lcid = "es_ES";
			}
		}
		else if (language == "ga")
		{
			lcid = "ga_IE";
		}
		else if (language == "nn")
		{
			lcid = "nn_NO";
		}
		else if (language == "pt")
		{
			lcid = "pt_BR";
		}
		else if (language == "sv")
		{
			lcid = "sv_SE";
		}
		else if (language == "zh")
		{
			if (localeChunks.Length > 2 && localeChunks[1] == "Hant")
			{
				if (localeChunks[2] == "HK")
				{
					lcid = "zh_TW";
				}
				else
				{
					lcid = "zh_HK";
				}
			}
			else
			{
				lcid = "zh_CN";
			}
		}
		else
		{
			lcid = language;
		}
		LocaleDatabase.Log.Info("Checking again with locale id {0}.", new object[]
		{
			lcid
		});
		matchedLocale = this.GetLocale(lcid);
		if (matchedLocale != null)
		{
			return matchedLocale;
		}
		if (language == "en")
		{
			matchedLocale = this.GetLocale(LocaleDatabase.LocaleId.en_US);
			if (matchedLocale != null)
			{
				return matchedLocale;
			}
		}
		return null;
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0001CE18 File Offset: 0x0001B018
	public void AddLocalizedObject(ILocalized localizedObject)
	{
		this._localizedObjects.Add(new WeakReference(localizedObject));
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0001CE2C File Offset: 0x0001B02C
	public void RemoveLocalizedObject(ILocalized localizedObject)
	{
		for (int locObjectIndex = 0; locObjectIndex < this._localizedObjects.Count; locObjectIndex++)
		{
			if (this._localizedObjects[locObjectIndex].Target == localizedObject)
			{
				this._localizedObjects.RemoveAt(locObjectIndex);
				return;
			}
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0001CE70 File Offset: 0x0001B070
	private void OnPlayerDataChanged()
	{
		LocaleDatabase.LocaleId lastSavedLocaleId = this._player.LocaleId;
		if (lastSavedLocaleId == LocaleDatabase.LocaleId.Unknown || this.GetLocale(lastSavedLocaleId) == null)
		{
			LocaleDatabase.LocaleId preferredLocaleId = this._softwareCapabilities.PreferredLocaleId;
			if (this.GetLocale(preferredLocaleId) == null)
			{
				LocaleDatabase.Log.Warn("The preferred locale {0} is not a supported locale. Falling back to {1}.", new object[]
				{
					preferredLocaleId,
					LocaleDatabase.LocaleId.en_US
				});
				preferredLocaleId = LocaleDatabase.LocaleId.en_US;
			}
			this._player.LocaleId = preferredLocaleId;
		}
		else
		{
			LocaleDatabase.Log.Info("Using previously-configured locale {0}.", new object[]
			{
				lastSavedLocaleId
			});
		}
		this.CurrentLocaleId = this._player.LocaleId;
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0001CF10 File Offset: 0x0001B110
	public bool Load()
	{
		this._locales.Clear();
		float startTime = Time.realtimeSinceStartup;
		IContentProfile contentProfile = this._scope.Get<IContentProfile>();
		new List<LocaleDatabase.LocaleId>(contentProfile.SupportedLocales);
		bool canUseIncompleteLocales = contentProfile.CanUseIncompleteLocales;
		foreach (LocaleDatabase.LocaleId locale in this._supportedLocaleDatabase.SupportedLocales)
		{
			string localeFilename = "Locales/" + locale.ToString();
			JSON.Dictionary jsonDictionary = (JSON.Dictionary)JSON.Load(localeFilename, false);
			if (jsonDictionary == null)
			{
				LocaleDatabase.Log.Error("LocaleDatabase: Failed to load JSON for locale '{0}'.", new object[]
				{
					localeFilename
				});
			}
			else
			{
				Locale newLocale = Locale.FromJSON(jsonDictionary, this, this._scope);
				if (newLocale == null)
				{
					LocaleDatabase.Log.Error("LocaleDatabase: Failed to parse JSON for locale '{0}'.", new object[]
					{
						localeFilename
					});
				}
				else if (!newLocale.IsComplete && !canUseIncompleteLocales)
				{
					LocaleDatabase.Log.Error("LocaleDatabase: Skipping incomplete locale '{0}'.", new object[]
					{
						newLocale.Id
					});
				}
				else
				{
					int localeIndex = 0;
					while (localeIndex < this._locales.Count && this._locales[localeIndex].Id < newLocale.Id)
					{
						localeIndex++;
					}
					this._locales.Insert(localeIndex, newLocale);
				}
			}
		}
		this._player.DataChanged += this.OnPlayerDataChanged;
		LocaleDatabase.LocaleId defaultLocaleId = LocaleDatabase.LocaleId.en_US;
		this.CurrentLocaleId = defaultLocaleId;
		this._fallbackLocale = this.CurrentLocale;
		float endTime = Time.realtimeSinceStartup;
		LocaleDatabase.Log.Info("Loaded locales in {0}s.", new object[]
		{
			endTime - startTime
		});
		return true;
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0001D0E8 File Offset: 0x0001B2E8
	public void OnReleasedFromScope(IScope scope)
	{
		this._player.DataChanged -= this.OnPlayerDataChanged;
	}

	// Token: 0x04000436 RID: 1078
	public static Diagnostics.Log.Channel Log = new Diagnostics.Log.Channel("Localization");

	// Token: 0x04000437 RID: 1079
	public const LocaleDatabase.LocaleId DefaultLocaleId = LocaleDatabase.LocaleId.en_US;

	// Token: 0x04000438 RID: 1080
	[Dependency]
	private IScope _scope;

	// Token: 0x04000439 RID: 1081
	[Dependency]
	private IActivePlayer _player;

	// Token: 0x0400043A RID: 1082
	[Dependency]
	private ISoftwareCapabilities _softwareCapabilities;

	// Token: 0x0400043B RID: 1083
	[Dependency]
	private SupportedLocaleDatabase _supportedLocaleDatabase;

	// Token: 0x0400043C RID: 1084
	private List<Locale> _locales = new List<Locale>();

	// Token: 0x0400043D RID: 1085
	private Locale _currentLocale;

	// Token: 0x0400043E RID: 1086
	private Locale _fallbackLocale;

	// Token: 0x0400043F RID: 1087
	private List<WeakReference> _localizedObjects = new List<WeakReference>();

	// Token: 0x02000187 RID: 391
	public enum LocaleId
	{
		// Token: 0x04000441 RID: 1089
		Unknown,
		// Token: 0x04000442 RID: 1090
		en_US,
		// Token: 0x04000443 RID: 1091
		ar,
		// Token: 0x04000444 RID: 1092
		bg,
		// Token: 0x04000445 RID: 1093
		ca,
		// Token: 0x04000446 RID: 1094
		cs,
		// Token: 0x04000447 RID: 1095
		cy,
		// Token: 0x04000448 RID: 1096
		da,
		// Token: 0x04000449 RID: 1097
		de,
		// Token: 0x0400044A RID: 1098
		el,
		// Token: 0x0400044B RID: 1099
		en_AU,
		// Token: 0x0400044C RID: 1100
		en_GB,
		// Token: 0x0400044D RID: 1101
		eo,
		// Token: 0x0400044E RID: 1102
		es_ES,
		// Token: 0x0400044F RID: 1103
		es_MX,
		// Token: 0x04000450 RID: 1104
		fi,
		// Token: 0x04000451 RID: 1105
		fr,
		// Token: 0x04000452 RID: 1106
		ga_IE,
		// Token: 0x04000453 RID: 1107
		hi,
		// Token: 0x04000454 RID: 1108
		hr,
		// Token: 0x04000455 RID: 1109
		hu,
		// Token: 0x04000456 RID: 1110
		id,
		// Token: 0x04000457 RID: 1111
		it,
		// Token: 0x04000458 RID: 1112
		ja,
		// Token: 0x04000459 RID: 1113
		ko,
		// Token: 0x0400045A RID: 1114
		mi,
		// Token: 0x0400045B RID: 1115
		ms,
		// Token: 0x0400045C RID: 1116
		nl,
		// Token: 0x0400045D RID: 1117
		nn_NO,
		// Token: 0x0400045E RID: 1118
		no,
		// Token: 0x0400045F RID: 1119
		pl,
		// Token: 0x04000460 RID: 1120
		pt_BR,
		// Token: 0x04000461 RID: 1121
		pt_PT,
		// Token: 0x04000462 RID: 1122
		ru,
		// Token: 0x04000463 RID: 1123
		sk,
		// Token: 0x04000464 RID: 1124
		sr,
		// Token: 0x04000465 RID: 1125
		sr_CS,
		// Token: 0x04000466 RID: 1126
		sr_Latin,
		// Token: 0x04000467 RID: 1127
		sv_SE,
		// Token: 0x04000468 RID: 1128
		sv_FI,
		// Token: 0x04000469 RID: 1129
		tr,
		// Token: 0x0400046A RID: 1130
		tg,
		// Token: 0x0400046B RID: 1131
		th,
		// Token: 0x0400046C RID: 1132
		uk,
		// Token: 0x0400046D RID: 1133
		zh_CN,
		// Token: 0x0400046E RID: 1134
		zh_HK,
		// Token: 0x0400046F RID: 1135
		zh_TW
	}
}
