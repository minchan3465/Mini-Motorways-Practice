using System;
using System.Collections.Generic;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006D3 RID: 1747
	public class House : Playback
	{
		// Token: 0x06003002 RID: 12290 RVA: 0x000E1478 File Offset: 0x000DF678
		public House(AudioEventFilter filter) : base(filter)
		{
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x000022F5 File Offset: 0x000004F5
		protected override void OnPulse()
		{
		}

		// Token: 0x06003004 RID: 12292 RVA: 0x000E1481 File Offset: 0x000DF681
		public override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnHouseSpawn), AudioEventType.HouseSpawned, -1);
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x000E14A4 File Offset: 0x000DF6A4
		private void OnHouseSpawn(AudioEvent e)
		{
			HouseView h = e.House;
			if (e.Type == AudioEventType.HouseSpawned && !(Get.Loadout.MusicData is Menu))
			{
				int times = Rando.Pick<int>(new int[]
				{
					5,
					6,
					7,
					8
				});
				Maf.Repeat(times, delegate(int i)
				{
					AudioPlayer ui = AudioPlayer.UI;
					string sampleName = "PeepAppears_" + Get.Loadout.MusicData.Timbres[h.groupIndex];
					double dspTime = AudioPlayer.EarliestSchedulableTime + (double)i * Get.Pulse.Master.Duration / (double)times;
					ui.PlaySample(sampleName, h.Pan.x, h.GetAttenuation(false, 25f) * 1f * Maf.VolCurve(1f - (float)i / (float)times), Mathf.Lerp(1f, 0.5f, Twerp.Ease.In((float)i / (float)times, 2)), 0.0, dspTime, false, null, false, false, 0f, true);
				}, false);
				List<DestinationGroup> destinationGroups = Get.Loadout.DestinationGroups;
				if (destinationGroups.Count > h.groupIndex)
				{
					List<string> notes = destinationGroups[h.groupIndex].Notes;
					AudioPlayer.Default.PlayChord("chordTone", notes, -1.0, (float)Get.Pulse.Master.Duration / (float)notes.Count, h.Attenuation * Settings.Gain.HOUSE_SPAWNED_CHORD[0], h.Attenuation * Settings.Gain.HOUSE_SPAWNED_CHORD[1], h.Pan.x, h.Pan.x, 0f, 0f, -1, false);
				}
			}
		}
	}
}
