using System;

namespace Motorways.Themes
{
	// Token: 0x0200047B RID: 1147
	public static class ThemedMaterialTypeExtensions
	{
		// Token: 0x06001C85 RID: 7301 RVA: 0x0006A230 File Offset: 0x00068430
		public static bool TryParse(this string parseString, out ThemedMaterialType result)
		{
			for (int enumIndex = 0; enumIndex < ThemedMaterialTypeExtensions.EnumNames.Length; enumIndex++)
			{
				if (ThemedMaterialTypeExtensions.EnumNames[enumIndex] == parseString)
				{
					result = (ThemedMaterialType)enumIndex;
					return true;
				}
			}
			result = ThemedMaterialType.Light;
			return false;
		}

		// Token: 0x04001882 RID: 6274
		private static readonly string[] EnumNames = Enum.GetNames(typeof(ThemedMaterialType));
	}
}
