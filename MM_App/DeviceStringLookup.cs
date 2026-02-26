using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Token: 0x02000259 RID: 601
public static class DeviceStringLookup
{
	// Token: 0x06000E3F RID: 3647 RVA: 0x00030254 File Offset: 0x0002E454
	public static string GetDeviceDisplayStringFromModel(string model)
	{
		if (DeviceStringLookup.modelToDisplay.ContainsKey(model))
		{
			return DeviceStringLookup.modelToDisplay[model];
		}
		string formattedKey = Regex.Match(model, "^[^0-9]*").Value;
		if (DeviceStringLookup.formattedKeyToDisplay.ContainsKey(formattedKey))
		{
			return DeviceStringLookup.formattedKeyToDisplay[formattedKey];
		}
		return formattedKey;
	}

	// Token: 0x0400086C RID: 2156
	private static Dictionary<string, string> modelToDisplay = new Dictionary<string, string>
	{
		{
			"i386",
			"iPhone Simulator"
		},
		{
			"x86_64",
			"iPhone Simulator"
		},
		{
			"iPhone8,1",
			"iPhone 6s"
		},
		{
			"iPhone8,2",
			"iPhone 6s Plus"
		},
		{
			"iPhone8,4",
			"iPhone SE"
		},
		{
			"iPhone9,1",
			"iPhone 7"
		},
		{
			"iPhone9,2",
			"iPhone 7 Plus"
		},
		{
			"iPhone9,3",
			"iPhone 7"
		},
		{
			"iPhone9,4",
			"iPhone 7 Plus"
		},
		{
			"iPhone10,1",
			"iPhone 8"
		},
		{
			"iPhone10,2",
			"iPhone 8 Plus"
		},
		{
			"iPhone10,3",
			"iPhone X"
		},
		{
			"iPhone10,4",
			"iPhone 8"
		},
		{
			"iPhone10,5",
			"iPhone 8 Plus"
		},
		{
			"iPhone10,6",
			"iPhone X"
		},
		{
			"iPhone11,2",
			"iPhone XS"
		},
		{
			"iPhone11,4",
			"iPhone XS Max"
		},
		{
			"iPhone11,6",
			"iPhone XS Max"
		},
		{
			"iPhone11,8",
			"iPhone XR"
		},
		{
			"iPhone12,1",
			"iPhone 11"
		},
		{
			"iPhone12,3",
			"iPhone 11 Pro"
		},
		{
			"iPhone12,5",
			"iPhone 11 Pro Max"
		},
		{
			"iPod7,1",
			"iPod"
		},
		{
			"iPod9,1",
			"iPod"
		},
		{
			"iPad4,7",
			"iPad mini 3"
		},
		{
			"iPad4,8",
			"iPad mini 3"
		},
		{
			"iPad4,9",
			"iPad mini 3"
		},
		{
			"iPad5,1",
			"iPad mini 4"
		},
		{
			"iPad5,2",
			"iPad mini 4"
		},
		{
			"iPad5,3",
			"iPad Air 2"
		},
		{
			"iPad5,4",
			"iPad Air 2"
		},
		{
			"iPad6,3",
			"iPad Pro"
		},
		{
			"iPad6,4",
			"iPad Pro"
		},
		{
			"iPad6,7",
			"iPad Pro"
		},
		{
			"iPad6,8",
			"iPad Pro"
		},
		{
			"iPad6,11",
			"iPad"
		},
		{
			"iPad6,12",
			"iPad"
		},
		{
			"iPad7,1",
			"iPad Pro"
		},
		{
			"iPad7,2",
			"iPad Pro"
		},
		{
			"iPad7,3",
			"iPad Pro"
		},
		{
			"iPad7,4",
			"iPad Pro"
		},
		{
			"iPad7,5",
			"iPad"
		},
		{
			"iPad7,6",
			"iPad"
		},
		{
			"iPad7,11",
			"iPad"
		},
		{
			"iPad7,12",
			"iPad"
		},
		{
			"iPad8,1",
			"iPad Pro"
		},
		{
			"iPad8,2",
			"iPad Pro"
		},
		{
			"iPad8,3",
			"iPad Pro"
		},
		{
			"iPad8,4",
			"iPad Pro"
		},
		{
			"iPad8,5",
			"iPad Pro"
		},
		{
			"iPad8,6",
			"iPad Pro"
		},
		{
			"iPad8,7",
			"iPad Pro"
		},
		{
			"iPad8,8",
			"iPad Pro"
		},
		{
			"iPad8,9",
			"iPad Pro"
		},
		{
			"iPad8,10",
			"iPad Pro"
		},
		{
			"iPad8,11",
			"iPad Pro"
		},
		{
			"iPad8,12",
			"iPad Pro"
		},
		{
			"iPad11,1",
			"iPad mini"
		},
		{
			"iPad11,2",
			"iPad mini"
		},
		{
			"iPad11,3",
			"iPad Air"
		},
		{
			"iPad11,4",
			"iPad Air"
		},
		{
			"AppleTV5,3",
			"Apple TV"
		},
		{
			"AppleTV6,2",
			"Apple TV 4K"
		}
	};

	// Token: 0x0400086D RID: 2157
	private static Dictionary<string, string> formattedKeyToDisplay = new Dictionary<string, string>
	{
		{
			"MacPro",
			"Mac Pro"
		},
		{
			"MacBook",
			"MacBook"
		},
		{
			"iMacPro",
			"iMac Pro"
		},
		{
			"iMac",
			"iMac"
		},
		{
			"Macmini",
			"Mac mini"
		},
		{
			"MacBookPro",
			"MacBook Pro"
		},
		{
			"MacBookAir",
			"MacBook Air"
		},
		{
			"iPhone",
			"iPhone"
		},
		{
			"iPad",
			"iPad"
		},
		{
			"iPod",
			"iPod"
		},
		{
			"AppleTV",
			"Apple TV"
		}
	};
}
