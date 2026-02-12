using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Factory
{
	// Token: 0x02000321 RID: 801
	public static class TypeUtilities
	{
		// Token: 0x0600136A RID: 4970 RVA: 0x0004040A File Offset: 0x0003E60A
		public static int GetTypeId<T>()
		{
			return TypeUtilities.GetTypeId(typeof(T));
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0004041B File Offset: 0x0003E61B
		public static int GetTypeId(Type type)
		{
			return TypeUtilities.CalculateMD5(type.FullName);
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x00040428 File Offset: 0x0003E628
		public static int CalculateMD5(string name)
		{
			return BitConverter.ToInt32(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name)), 0);
		}

		// Token: 0x0600136D RID: 4973 RVA: 0x00040448 File Offset: 0x0003E648
		public static T GetCustomAttribute<T>(Type type) where T : Attribute
		{
			T attribute = type.GetCustomAttribute(true);
			if (attribute != null)
			{
				return attribute;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				attribute = interfaces[i].GetCustomAttribute(true);
				if (attribute != null)
				{
					return attribute;
				}
			}
			return default(T);
		}
	}
}
