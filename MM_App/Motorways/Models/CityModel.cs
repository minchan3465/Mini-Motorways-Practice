using System;
using Factory;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004DB RID: 1243
	public class CityModel : Model<EmptyModelFrame, CityModel.IObserver>
	{
		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x0600205F RID: 8287 RVA: 0x000807D2 File Offset: 0x0007E9D2
		// (set) Token: 0x06002060 RID: 8288 RVA: 0x000807DA File Offset: 0x0007E9DA
		[Serialize(true, null)]
		public GameMode Mode { get; private set; }

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06002061 RID: 8289 RVA: 0x000807E3 File Offset: 0x0007E9E3
		// (set) Token: 0x06002062 RID: 8290 RVA: 0x000807EB File Offset: 0x0007E9EB
		[Serialize(true, null)]
		public GameMode InitialMode { get; private set; }

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06002063 RID: 8291 RVA: 0x000807F4 File Offset: 0x0007E9F4
		// (set) Token: 0x06002064 RID: 8292 RVA: 0x000807FC File Offset: 0x0007E9FC
		[Serialize(false, null)]
		public GameRules Rules { get; private set; }

		// Token: 0x06002065 RID: 8293 RVA: 0x00080805 File Offset: 0x0007EA05
		public override void Reset()
		{
			this.Mode = GameMode.Normal;
			this.startOffset = Vector3Fixed.zero;
			this.latestLaneChangeFrame = -1;
			this.InitialMode = GameMode.Normal;
			this.Rules = null;
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x0008082E File Offset: 0x0007EA2E
		public override void OnReleasedFromScope(IScope scope)
		{
			if (this.pseudorandomGenerator != null)
			{
				scope.Release(this.pseudorandomGenerator);
				this.pseudorandomGenerator = null;
			}
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x0008084C File Offset: 0x0007EA4C
		public void SetGameMode(GameMode mode, GameRules rules)
		{
			this.Mode = mode;
			this.Rules = rules;
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x0008085C File Offset: 0x0007EA5C
		public void StartGameInMode(GameMode mode, GameRules rules)
		{
			this.Mode = mode;
			this.Rules = rules;
			this.InitialMode = mode;
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x00080874 File Offset: 0x0007EA74
		public void OnLanesAdded()
		{
			foreach (CityModel.IObserver observer in base.Observers)
			{
				observer.OnLanesAdded();
			}
			this.latestLaneChangeFrame = this._clock.FrameCount;
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x000808B8 File Offset: 0x0007EAB8
		public void OnLanesReleased()
		{
			foreach (CityModel.IObserver observer in base.Observers)
			{
				observer.OnLanesReleased();
			}
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x000808E8 File Offset: 0x0007EAE8
		public void OnCarparkAdded(CarparkModel carparkModel)
		{
			foreach (CityModel.IObserver observer in base.Observers)
			{
				observer.OnCarparkAdded(carparkModel);
			}
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x00080919 File Offset: 0x0007EB19
		public CityModel() : base(1)
		{
		}

		// Token: 0x04001AE4 RID: 6884
		public string cityName;

		// Token: 0x04001AE8 RID: 6888
		public PseudorandomGenerator pseudorandomGenerator;

		// Token: 0x04001AE9 RID: 6889
		public Vector3Fixed startOffset;

		// Token: 0x04001AEA RID: 6890
		public int latestLaneChangeFrame;

		// Token: 0x04001AEB RID: 6891
		[Dependency]
		private Clock _clock;

		// Token: 0x020004DC RID: 1244
		public interface IObserver
		{
			// Token: 0x0600206D RID: 8301
			void OnLanesAdded();

			// Token: 0x0600206E RID: 8302
			void OnLanesReleased();

			// Token: 0x0600206F RID: 8303
			void OnCarparkAdded(CarparkModel carparkModel);
		}
	}
}
