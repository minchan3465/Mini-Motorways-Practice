using System;

namespace Motorways.Audio
{
	// Token: 0x020006C7 RID: 1735
	public class DestinationInstancer : ImmediateAudioModule
	{
		// Token: 0x06002FBE RID: 12222 RVA: 0x000DF017 File Offset: 0x000DD217
		public DestinationInstancer(AudioEventFilter filter) : base(filter, "", 1f, -1f, "", 1f)
		{
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x000DF039 File Offset: 0x000DD239
		protected override void OnAudioEvent(AudioEvent e)
		{
			this.CreateDestinationGroup(e);
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x000DF044 File Offset: 0x000DD244
		private void CreateDestinationGroup(AudioEvent e)
		{
			Dbug.Log.Info("DestinationInstancer.CreateDestinationGroup(): DestGroups.Count is {0}. e.GroupIndex is {1}.", new object[]
			{
				Get.Loadout.DestinationGroups.Count,
				e.GroupIndex
			});
			if (Get.Loadout.DestinationGroups.Count > e.GroupIndex)
			{
				return;
			}
			Get.Loadout.GetDestinationGroup(e.GroupIndex).OnEvents(e);
		}
	}
}
