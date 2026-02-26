using System;
using Client;
using Factory;

namespace Motorways.Views
{
	// Token: 0x02000575 RID: 1397
	public class UpgradeBarWrapper : UpgradeBarClient
	{
		// Token: 0x0600263F RID: 9791 RVA: 0x000A2530 File Offset: 0x000A0730
		public override TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Tick(timeInterval, stepAlpha);
			}
			return base.Tick(timeInterval, stepAlpha);
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x000A2568 File Offset: 0x000A0768
		public override void OnCreatedInScope(IScope scope)
		{
			foreach (UpgradeBarClient upgradeBar in this.upgradeBars)
			{
				scope.Assemble(upgradeBar);
			}
		}

		// Token: 0x06002641 RID: 9793 RVA: 0x000A2598 File Offset: 0x000A0798
		public override void OnReleasedFromScope(IScope scope)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnReleasedFromScope(scope);
			}
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x000A25C4 File Offset: 0x000A07C4
		public override void SetVisibility(bool isVisible, bool instantly = false)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetVisibility(isVisible, instantly);
			}
			base.IsVisible = isVisible;
		}

		// Token: 0x06002643 RID: 9795 RVA: 0x000A25F8 File Offset: 0x000A07F8
		public override void SetUpgradeButtonVisible(UpgradeType type, bool visible)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetUpgradeButtonVisible(type, visible);
			}
		}

		// Token: 0x06002644 RID: 9796 RVA: 0x000A2624 File Offset: 0x000A0824
		public override void AddToUpgradeButtonStack(UpgradeType type, bool fromAnimation = false, int count = 1)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddToUpgradeButtonStack(type, fromAnimation, count);
			}
		}

		// Token: 0x06002645 RID: 9797 RVA: 0x000A2654 File Offset: 0x000A0854
		public override void AddPendingToUpgradeButtonStack(UpgradeType type, int count = 1)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddPendingToUpgradeButtonStack(type, count);
			}
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x000A2680 File Offset: 0x000A0880
		public override void RemoveFromUpgradeButtonStack(UpgradeType type, bool fromAnimation = false)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RemoveFromUpgradeButtonStack(type, fromAnimation);
			}
		}

		// Token: 0x06002647 RID: 9799 RVA: 0x000A26AC File Offset: 0x000A08AC
		public override void PulseUpgradeIcon(UpgradeType type)
		{
			UpgradeBarClient[] array = this.upgradeBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].PulseUpgradeIcon(type);
			}
		}

		// Token: 0x0400202F RID: 8239
		[EnumTypedArray(typeof(DeviceCategory))]
		public UpgradeBarClient[] upgradeBars;
	}
}
