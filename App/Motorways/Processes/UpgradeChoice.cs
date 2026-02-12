using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;

namespace Motorways.Processes
{
	// Token: 0x020004BD RID: 1213
	[Factory.Serializable(1)]
	public class UpgradeChoice : IReusable
	{
		// Token: 0x06001F98 RID: 8088 RVA: 0x0007B62C File Offset: 0x0007982C
		public void ShuffleChoices(PseudorandomGenerator random)
		{
			for (int maxIndex = this.choices.Count; maxIndex > 0; maxIndex--)
			{
				int swapIndex = random.Int(maxIndex);
				UpgradePackageDefinition temp = this.choices[0];
				this.choices[0] = this.choices[swapIndex];
				this.choices[swapIndex] = temp;
			}
		}

		// Token: 0x06001F99 RID: 8089 RVA: 0x0007B689 File Offset: 0x00079889
		public void Reset()
		{
			this.disabledOptions = DisabledUpgradeOptions.None;
			this.isFree = false;
			this.choices.Clear();
		}

		// Token: 0x04001A40 RID: 6720
		public List<UpgradePackageDefinition> choices = new List<UpgradePackageDefinition>();

		// Token: 0x04001A41 RID: 6721
		public bool isFree;

		// Token: 0x04001A42 RID: 6722
		public DisabledUpgradeOptions disabledOptions;
	}
}
