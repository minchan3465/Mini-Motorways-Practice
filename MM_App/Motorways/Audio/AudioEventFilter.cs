using System;
using Motorways.Views;

namespace Motorways.Audio
{
	// Token: 0x0200063C RID: 1596
	public struct AudioEventFilter
	{
		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06002C7A RID: 11386 RVA: 0x000CEDA9 File Offset: 0x000CCFA9
		// (set) Token: 0x06002C7B RID: 11387 RVA: 0x000CEDB1 File Offset: 0x000CCFB1
		public AudioEventType Type { readonly get; set; }

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000CEDBA File Offset: 0x000CCFBA
		// (set) Token: 0x06002C7D RID: 11389 RVA: 0x000CEDC2 File Offset: 0x000CCFC2
		public UIEventType UIEventType { readonly get; set; }

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06002C7E RID: 11390 RVA: 0x000CEDCB File Offset: 0x000CCFCB
		// (set) Token: 0x06002C7F RID: 11391 RVA: 0x000CEDD3 File Offset: 0x000CCFD3
		public UIAudioProfile UIAudioProfile { readonly get; set; }

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06002C80 RID: 11392 RVA: 0x000CEDDC File Offset: 0x000CCFDC
		// (set) Token: 0x06002C81 RID: 11393 RVA: 0x000CEDE4 File Offset: 0x000CCFE4
		public DestinationView Destination { readonly get; set; }

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06002C82 RID: 11394 RVA: 0x000CEDED File Offset: 0x000CCFED
		// (set) Token: 0x06002C83 RID: 11395 RVA: 0x000CEDF5 File Offset: 0x000CCFF5
		public VehicleView Vehicle { readonly get; set; }

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06002C84 RID: 11396 RVA: 0x000CEDFE File Offset: 0x000CCFFE
		// (set) Token: 0x06002C85 RID: 11397 RVA: 0x000CEE06 File Offset: 0x000CD006
		public int GroupIndex { readonly get; set; }

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06002C86 RID: 11398 RVA: 0x000CEE0F File Offset: 0x000CD00F
		// (set) Token: 0x06002C87 RID: 11399 RVA: 0x000CEE17 File Offset: 0x000CD017
		public ScreenStack.MotorwaysScreen Screen { readonly get; set; }

		// Token: 0x06002C88 RID: 11400 RVA: 0x000CEE20 File Offset: 0x000CD020
		public AudioEventFilter(AudioEventType type)
		{
			this = default(AudioEventFilter);
			this.Type = type;
			this.UIEventType = UIEventType.None;
			this.UIAudioProfile = UIAudioProfile.None;
			this.Destination = null;
			this.Vehicle = null;
			this.GroupIndex = -1;
			this.Screen = ScreenStack.MotorwaysScreen.None;
		}

		// Token: 0x06002C89 RID: 11401 RVA: 0x000CEE5A File Offset: 0x000CD05A
		public AudioEventFilter(UIEventType type, UIAudioProfile audioProfile = UIAudioProfile.None)
		{
			this = default(AudioEventFilter);
			this.Type = AudioEventType.UserInterface;
			this.UIAudioProfile = audioProfile;
			this.UIEventType = type;
			this.Destination = null;
			this.Vehicle = null;
			this.GroupIndex = -1;
			this.Screen = ScreenStack.MotorwaysScreen.None;
		}

		// Token: 0x06002C8A RID: 11402 RVA: 0x000CEE9C File Offset: 0x000CD09C
		public bool IsEventFiltered(AudioEvent audioEvent)
		{
			return (this.Type & audioEvent.Type) != AudioEventType.None && (this.UIEventType == UIEventType.None || (this.UIEventType & audioEvent.UIEventType) != UIEventType.None) && (this.UIAudioProfile == UIAudioProfile.None || (this.UIAudioProfile & audioEvent.UIAudioProfile) != UIAudioProfile.None) && (!(this.Destination != null) || !(this.Destination != audioEvent.Destination)) && (!(this.Vehicle != null) || this.Vehicle.Id == audioEvent.Vehicle.Id) && (this.GroupIndex <= -1 || this.GroupIndex == audioEvent.GroupIndex) && (this.Screen == ScreenStack.MotorwaysScreen.None || this.Screen == audioEvent.Screen);
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x000CEF6C File Offset: 0x000CD16C
		public static AudioEventFilter FromJSON(JSON.Dictionary jsonFilter)
		{
			AudioEventFilter filter = new AudioEventFilter(AudioEventType.None);
			if (jsonFilter == null)
			{
				return filter;
			}
			string type = jsonFilter.GetString("type");
			if (type == null)
			{
				return filter;
			}
			filter.Type = (AudioEventType)Enum.Parse(typeof(AudioEventType), type);
			if (jsonFilter.ContainsKey("uiEventType"))
			{
				filter.UIEventType = (UIEventType)Enum.Parse(typeof(UIEventType), jsonFilter.GetString("uiEventType"));
			}
			if (jsonFilter.ContainsKey("uiProfile"))
			{
				filter.UIAudioProfile = (UIAudioProfile)Enum.Parse(typeof(UIAudioProfile), jsonFilter.GetString("uiProfile"));
			}
			return filter;
		}
	}
}
