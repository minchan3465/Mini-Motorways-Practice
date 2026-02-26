using System;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000634 RID: 1588
	public class AudioSampleData : IGAT16BitDataProvider
	{
		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x06002C3D RID: 11325 RVA: 0x000C45FE File Offset: 0x000C27FE
		// (set) Token: 0x06002C3E RID: 11326 RVA: 0x000C4606 File Offset: 0x000C2806
		public string Name { get; private set; }

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06002C3F RID: 11327 RVA: 0x000C460F File Offset: 0x000C280F
		// (set) Token: 0x06002C40 RID: 11328 RVA: 0x000C4617 File Offset: 0x000C2817
		public int Offset { get; private set; }

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06002C41 RID: 11329 RVA: 0x000C4620 File Offset: 0x000C2820
		// (set) Token: 0x06002C42 RID: 11330 RVA: 0x000C4628 File Offset: 0x000C2828
		public int Length { get; private set; }

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06002C43 RID: 11331 RVA: 0x000C4631 File Offset: 0x000C2831
		// (set) Token: 0x06002C44 RID: 11332 RVA: 0x000C4639 File Offset: 0x000C2839
		public int NativeLength { get; private set; }

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06002C45 RID: 11333 RVA: 0x000C4642 File Offset: 0x000C2842
		// (set) Token: 0x06002C46 RID: 11334 RVA: 0x000C464A File Offset: 0x000C284A
		public double LastUseTime { get; private set; }

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06002C47 RID: 11335 RVA: 0x000C4653 File Offset: 0x000C2853
		// (set) Token: 0x06002C48 RID: 11336 RVA: 0x000C465B File Offset: 0x000C285B
		public GATData GATData { get; private set; }

		// Token: 0x06002C49 RID: 11337 RVA: 0x000C4664 File Offset: 0x000C2864
		public AudioSampleData(AudioDataBank bank, string name, int offset, int nativeLength, int resampledLength)
		{
			this.LastUseTime = -1.0;
			this.bank = bank;
			this.Name = name;
			this.Offset = offset;
			this.NativeLength = nativeLength;
			this.Length = resampledLength;
			this.GATData = new GATData(this);
			this.GATData.SampleName = name;
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x06002C4A RID: 11338 RVA: 0x000C46C3 File Offset: 0x000C28C3
		public short[] SampleData
		{
			get
			{
				if (this.LastUseTime == -1.0)
				{
					this.data = this.bank.DecompressSample(this);
				}
				this.LastUseTime = AudioSettings.dspTime;
				return this.data;
			}
		}

		// Token: 0x06002C4B RID: 11339 RVA: 0x000C46F9 File Offset: 0x000C28F9
		public void Release()
		{
			this.data = null;
			this.LastUseTime = -1.0;
		}

		// Token: 0x04002683 RID: 9859
		private AudioDataBank bank;

		// Token: 0x04002684 RID: 9860
		private short[] data;
	}
}
