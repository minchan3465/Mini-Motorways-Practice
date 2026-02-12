using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000740 RID: 1856
	[CreateAssetMenu(fileName = "New Supported Locale Database", menuName = "Motorways/Locale/Supported Locale Database", order = 2)]
	public class SupportedLocaleDatabase : ScriptableObject
	{
		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060033DF RID: 13279 RVA: 0x000F54C1 File Offset: 0x000F36C1
		public List<LocaleDatabase.LocaleId> SupportedLocales
		{
			get
			{
				return this._supportedLocales;
			}
		}

		// Token: 0x04002C58 RID: 11352
		[SerializeField]
		private List<LocaleDatabase.LocaleId> _supportedLocales = new List<LocaleDatabase.LocaleId>();
	}
}
