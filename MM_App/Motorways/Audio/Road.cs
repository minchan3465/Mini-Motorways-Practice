using System;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006D7 RID: 1751
	public class Road : Playback
	{
		// Token: 0x0600301A RID: 12314 RVA: 0x000E1478 File Offset: 0x000DF678
		public Road(AudioEventFilter filter) : base(filter)
		{
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x0600301B RID: 12315 RVA: 0x000E1AF3 File Offset: 0x000DFCF3
		// (set) Token: 0x0600301C RID: 12316 RVA: 0x000E1AFB File Offset: 0x000DFCFB
		public bool Success
		{
			get
			{
				return this._success;
			}
			set
			{
				this.skip = (!this._success && !value);
				this._success = value;
			}
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x000E1B1C File Offset: 0x000DFD1C
		protected override void OnPulse()
		{
			if (base.GetEvents(0))
			{
				while (this.audioEvents.Count > 1 && this.audioEvents[0].Type == AudioEventType.BuildRoad)
				{
					this.audioEvents.RemoveAt(0);
				}
				this.Success = this.audioEvents[0].Condition;
				this.HandleEvent(this.audioEvents[0]);
				this.audioEvents.Clear();
			}
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x000E1B98 File Offset: 0x000DFD98
		private void HandleEvent(AudioEvent e)
		{
			if (this.skip)
			{
				return;
			}
			string sampleName = null;
			Param.Group pG = new Param.Group(null, null);
			UpgradeDatabase model = Get.Game.Simulation.GetModel<UpgradeDatabaseModel>();
			int fullConcrete = 20;
			int concreteLeft = Math.Min(model.GetAvailableUpgradeCount(UpgradeType.Concrete), fullConcrete);
			float concreteAvailability = (concreteLeft > 1) ? ((float)concreteLeft / (float)fullConcrete) : 0f;
			AudioEventType type = e.Type;
			if (type <= AudioEventType.MothballRoad)
			{
				if (type != AudioEventType.BuildRoad)
				{
					if (type == AudioEventType.MothballRoad)
					{
						sampleName = "EraseRoad";
						pG = Settings.MOTHBALL_ROAD;
						pG.Gain.Range = pG.Gain.Range.Swap();
						pG.Pitch.Range.x = Mathf.Lerp(1f, 1.25f, concreteAvailability);
						pG.Pitch.Range.y = Mathf.Lerp(1f, 1.5f, concreteAvailability);
					}
				}
				else
				{
					sampleName = (e.Condition ? "DrawRoad" : "sineFX_04");
					pG = (e.Condition ? Settings.BUILD_ROAD : Settings.DELETE_ROAD);
					if (e.Condition)
					{
						pG.Pitch.Range.x = Mathf.Lerp(0.75f, 1f, concreteAvailability);
						pG.Pitch.Range.y = Mathf.Lerp(0.75f, 1.25f, concreteAvailability);
					}
				}
			}
			else if (type != AudioEventType.TreeBulldozed)
			{
				if (type != AudioEventType.BuildBridge)
				{
					if (type == AudioEventType.BuildTunnel)
					{
						sampleName = (e.Condition ? "Draw-Tunnel" : "sineFX_04");
						pG = (e.Condition ? Settings.BUILD_TUNNEL : Settings.DELETE_ROAD);
					}
				}
				else
				{
					sampleName = (e.Condition ? "Draw-Bridge" : "sineFX_04");
					pG = (e.Condition ? Settings.BUILD_BRIDGE : Settings.DELETE_ROAD);
				}
			}
			else
			{
				sampleName = "Bulldoze-Tree-0" + Rando.Pick<string>(new string[]
				{
					"1",
					"2"
				});
				pG = Settings.BULLDOZE_TREE;
				Get.Mixbus.BoingPitchInPlace(Rando.Range(1f, 4f, -1), Rando.Pick<float>(new float[]
				{
					0.5f,
					1f,
					1.5f
				}), Settings.PITCH_TREE_BULLDOZED.Random(-1), 0.5f);
			}
			if (sampleName != null)
			{
				AudioPlayer.UI.PlaySample(sampleName, 0.5f, Mathf.Lerp(pG.Gain.Range.x, pG.Gain.Range.y, SFX.MouseSpeed), Mathf.Lerp(pG.Pitch.Range.x, pG.Pitch.Range.y, SFX.MouseSpeed), 0.0, -1.0, false, null, false, false, 0f, true);
			}
		}

		// Token: 0x04002988 RID: 10632
		private bool _success;

		// Token: 0x04002989 RID: 10633
		private bool skip;
	}
}
