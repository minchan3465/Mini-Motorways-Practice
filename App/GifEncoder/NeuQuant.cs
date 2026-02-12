using System;

namespace GifEncoder
{
	// Token: 0x02000278 RID: 632
	public class NeuQuant
	{
		// Token: 0x06000F9B RID: 3995 RVA: 0x00034364 File Offset: 0x00032564
		public NeuQuant(byte[] thepic, int len, int sample)
		{
			this.thepicture = thepic;
			this.lengthcount = len;
			this.samplefac = sample;
			this.network = new int[NeuQuant.PaletteSize][];
			for (int i = 0; i < NeuQuant.PaletteSize; i++)
			{
				this.network[i] = new int[4];
				int[] p = this.network[i];
				p[0] = (p[1] = (p[2] = (i << NeuQuant.netbiasshift + 8) / NeuQuant.PaletteSize));
				this.freq[i] = NeuQuant.intbias / NeuQuant.PaletteSize;
				this.bias[i] = 0;
			}
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00034440 File Offset: 0x00032640
		public byte[] ColorMap()
		{
			byte[] map = new byte[3 * NeuQuant.PaletteSize];
			int[] index = new int[NeuQuant.PaletteSize];
			for (int i = 0; i < NeuQuant.PaletteSize; i++)
			{
				index[this.network[i][3]] = i;
			}
			int j = 0;
			for (int k = 0; k < NeuQuant.PaletteSize; k++)
			{
				int l = index[k];
				map[j++] = (byte)this.network[l][0];
				map[j++] = (byte)this.network[l][1];
				map[j++] = (byte)this.network[l][2];
			}
			return map;
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x000344D8 File Offset: 0x000326D8
		public void Inxbuild()
		{
			int previouscol = 0;
			int startpos = 0;
			for (int i = 0; i < NeuQuant.PaletteSize; i++)
			{
				int[] p = this.network[i];
				int smallpos = i;
				int smallval = p[1];
				int[] q;
				for (int j = i + 1; j < NeuQuant.PaletteSize; j++)
				{
					q = this.network[j];
					if (q[1] < smallval)
					{
						smallpos = j;
						smallval = q[1];
					}
				}
				q = this.network[smallpos];
				if (i != smallpos)
				{
					int j = q[0];
					q[0] = p[0];
					p[0] = j;
					j = q[1];
					q[1] = p[1];
					p[1] = j;
					j = q[2];
					q[2] = p[2];
					p[2] = j;
					j = q[3];
					q[3] = p[3];
					p[3] = j;
				}
				if (smallval != previouscol)
				{
					this.netindex[previouscol] = startpos + i >> 1;
					for (int j = previouscol + 1; j < smallval; j++)
					{
						this.netindex[j] = i;
					}
					previouscol = smallval;
					startpos = i;
				}
			}
			this.netindex[previouscol] = startpos + NeuQuant.maxnetpos >> 1;
			for (int j = previouscol + 1; j < 256; j++)
			{
				this.netindex[j] = NeuQuant.maxnetpos;
			}
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x000345F8 File Offset: 0x000327F8
		public void Learn()
		{
			if (this.lengthcount < NeuQuant.minpicturebytes)
			{
				this.samplefac = 1;
			}
			this.alphadec = 30 + (this.samplefac - 1) / 3;
			byte[] p = this.thepicture;
			int pix = 0;
			int lim = this.lengthcount;
			int samplepixels = this.lengthcount / (3 * this.samplefac);
			int delta = samplepixels / NeuQuant.ncycles;
			int alpha = NeuQuant.initalpha;
			int radius = NeuQuant.initradius;
			int rad = radius >> NeuQuant.radiusbiasshift;
			if (rad <= 1)
			{
				rad = 0;
			}
			int i;
			for (i = 0; i < rad; i++)
			{
				this.radpower[i] = alpha * ((rad * rad - i * i) * NeuQuant.radbias / (rad * rad));
			}
			int step;
			if (this.lengthcount < NeuQuant.minpicturebytes)
			{
				step = 3;
			}
			else if (this.lengthcount % NeuQuant.prime1 != 0)
			{
				step = 3 * NeuQuant.prime1;
			}
			else if (this.lengthcount % NeuQuant.prime2 != 0)
			{
				step = 3 * NeuQuant.prime2;
			}
			else if (this.lengthcount % NeuQuant.prime3 != 0)
			{
				step = 3 * NeuQuant.prime3;
			}
			else
			{
				step = 3 * NeuQuant.prime4;
			}
			i = 0;
			while (i < samplepixels)
			{
				int b = (int)(p[pix] & byte.MaxValue) << NeuQuant.netbiasshift;
				int g = (int)(p[pix + 1] & byte.MaxValue) << NeuQuant.netbiasshift;
				int r = (int)(p[pix + 2] & byte.MaxValue) << NeuQuant.netbiasshift;
				int j = this.Contest(b, g, r);
				this.Altersingle(alpha, j, b, g, r);
				if (rad != 0)
				{
					this.Alterneigh(rad, j, b, g, r);
				}
				pix += step;
				if (pix >= lim)
				{
					pix -= this.lengthcount;
				}
				i++;
				if (delta == 0)
				{
					delta = 1;
				}
				if (i % delta == 0)
				{
					alpha -= alpha / this.alphadec;
					radius -= radius / NeuQuant.radiusdec;
					rad = radius >> NeuQuant.radiusbiasshift;
					if (rad <= 1)
					{
						rad = 0;
					}
					for (j = 0; j < rad; j++)
					{
						this.radpower[j] = alpha * ((rad * rad - j * j) * NeuQuant.radbias / (rad * rad));
					}
				}
			}
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x00034814 File Offset: 0x00032A14
		public int Map(int b, int g, int r)
		{
			int bestd = 1000;
			int best = -1;
			int i = this.netindex[g];
			int j = i - 1;
			while (i < NeuQuant.PaletteSize || j >= 0)
			{
				if (i < NeuQuant.PaletteSize)
				{
					int[] p = this.network[i];
					int dist = p[1] - g;
					if (dist >= bestd)
					{
						i = NeuQuant.PaletteSize;
					}
					else
					{
						i++;
						if (dist < 0)
						{
							dist = -dist;
						}
						int a = p[0] - b;
						if (a < 0)
						{
							a = -a;
						}
						dist += a;
						if (dist < bestd)
						{
							a = p[2] - r;
							if (a < 0)
							{
								a = -a;
							}
							dist += a;
							if (dist < bestd)
							{
								bestd = dist;
								best = p[3];
							}
						}
					}
				}
				if (j >= 0)
				{
					int[] p = this.network[j];
					int dist = g - p[1];
					if (dist >= bestd)
					{
						j = -1;
					}
					else
					{
						j--;
						if (dist < 0)
						{
							dist = -dist;
						}
						int a = p[0] - b;
						if (a < 0)
						{
							a = -a;
						}
						dist += a;
						if (dist < bestd)
						{
							a = p[2] - r;
							if (a < 0)
							{
								a = -a;
							}
							dist += a;
							if (dist < bestd)
							{
								bestd = dist;
								best = p[3];
							}
						}
					}
				}
			}
			return best;
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x00034919 File Offset: 0x00032B19
		public byte[] Process()
		{
			this.Learn();
			this.Unbiasnet();
			this.Inxbuild();
			return this.ColorMap();
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x00034934 File Offset: 0x00032B34
		public void Unbiasnet()
		{
			for (int i = 0; i < NeuQuant.PaletteSize; i++)
			{
				this.network[i][0] >>= NeuQuant.netbiasshift;
				this.network[i][1] >>= NeuQuant.netbiasshift;
				this.network[i][2] >>= NeuQuant.netbiasshift;
				this.network[i][3] = i;
			}
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x000349AC File Offset: 0x00032BAC
		protected void Alterneigh(int rad, int i, int b, int g, int r)
		{
			int lo = i - rad;
			if (lo < -1)
			{
				lo = -1;
			}
			int hi = i + rad;
			if (hi > NeuQuant.PaletteSize)
			{
				hi = NeuQuant.PaletteSize;
			}
			int j = i + 1;
			int k = i - 1;
			int l = 1;
			while (j < hi || k > lo)
			{
				int a = this.radpower[l++];
				if (j < hi)
				{
					int[] p = this.network[j++];
					try
					{
						p[0] -= a * (p[0] - b) / NeuQuant.alpharadbias;
						p[1] -= a * (p[1] - g) / NeuQuant.alpharadbias;
						p[2] -= a * (p[2] - r) / NeuQuant.alpharadbias;
					}
					catch (Exception)
					{
					}
				}
				if (k > lo)
				{
					int[] p = this.network[k--];
					try
					{
						p[0] -= a * (p[0] - b) / NeuQuant.alpharadbias;
						p[1] -= a * (p[1] - g) / NeuQuant.alpharadbias;
						p[2] -= a * (p[2] - r) / NeuQuant.alpharadbias;
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x00034AF4 File Offset: 0x00032CF4
		protected void Altersingle(int alpha, int i, int b, int g, int r)
		{
			int[] j = this.network[i];
			j[0] -= alpha * (j[0] - b) / NeuQuant.initalpha;
			j[1] -= alpha * (j[1] - g) / NeuQuant.initalpha;
			j[2] -= alpha * (j[2] - r) / NeuQuant.initalpha;
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x00034B54 File Offset: 0x00032D54
		protected int Contest(int b, int g, int r)
		{
			int bestd = int.MaxValue;
			int bestbiasd = bestd;
			int bestpos = -1;
			int bestbiaspos = bestpos;
			for (int i = 0; i < NeuQuant.PaletteSize; i++)
			{
				int[] array = this.network[i];
				int dist = array[0] - b;
				if (dist < 0)
				{
					dist = -dist;
				}
				int a = array[1] - g;
				if (a < 0)
				{
					a = -a;
				}
				dist += a;
				a = array[2] - r;
				if (a < 0)
				{
					a = -a;
				}
				dist += a;
				if (dist < bestd)
				{
					bestd = dist;
					bestpos = i;
				}
				int biasdist = dist - (this.bias[i] >> NeuQuant.intbiasshift - NeuQuant.netbiasshift);
				if (biasdist < bestbiasd)
				{
					bestbiasd = biasdist;
					bestbiaspos = i;
				}
				int betafreq = this.freq[i] >> NeuQuant.betashift;
				this.freq[i] -= betafreq;
				this.bias[i] += betafreq << NeuQuant.gammashift;
			}
			this.freq[bestpos] += NeuQuant.beta;
			this.bias[bestpos] -= NeuQuant.betagamma;
			return bestbiaspos;
		}

		// Token: 0x04000DE8 RID: 3560
		public static readonly int PaletteSize = 255;

		// Token: 0x04000DE9 RID: 3561
		protected static readonly int prime1 = 499;

		// Token: 0x04000DEA RID: 3562
		protected static readonly int prime2 = 491;

		// Token: 0x04000DEB RID: 3563
		protected static readonly int prime3 = 487;

		// Token: 0x04000DEC RID: 3564
		protected static readonly int prime4 = 503;

		// Token: 0x04000DED RID: 3565
		protected static readonly int minpicturebytes = 3 * NeuQuant.prime4;

		// Token: 0x04000DEE RID: 3566
		protected static readonly int maxnetpos = NeuQuant.PaletteSize - 1;

		// Token: 0x04000DEF RID: 3567
		protected static readonly int netbiasshift = 4;

		// Token: 0x04000DF0 RID: 3568
		protected static readonly int ncycles = 100;

		// Token: 0x04000DF1 RID: 3569
		protected static readonly int intbiasshift = 16;

		// Token: 0x04000DF2 RID: 3570
		protected static readonly int intbias = 1 << NeuQuant.intbiasshift;

		// Token: 0x04000DF3 RID: 3571
		protected static readonly int gammashift = 10;

		// Token: 0x04000DF4 RID: 3572
		protected static readonly int gamma = 1 << NeuQuant.gammashift;

		// Token: 0x04000DF5 RID: 3573
		protected static readonly int betashift = 10;

		// Token: 0x04000DF6 RID: 3574
		protected static readonly int beta = NeuQuant.intbias >> NeuQuant.betashift;

		// Token: 0x04000DF7 RID: 3575
		protected static readonly int betagamma = NeuQuant.intbias << NeuQuant.gammashift - NeuQuant.betashift;

		// Token: 0x04000DF8 RID: 3576
		protected static readonly int initrad = NeuQuant.PaletteSize >> 3;

		// Token: 0x04000DF9 RID: 3577
		protected static readonly int radiusbiasshift = 6;

		// Token: 0x04000DFA RID: 3578
		protected static readonly int radiusbias = 1 << NeuQuant.radiusbiasshift;

		// Token: 0x04000DFB RID: 3579
		protected static readonly int initradius = NeuQuant.initrad * NeuQuant.radiusbias;

		// Token: 0x04000DFC RID: 3580
		protected static readonly int radiusdec = 30;

		// Token: 0x04000DFD RID: 3581
		protected static readonly int alphabiasshift = 10;

		// Token: 0x04000DFE RID: 3582
		protected static readonly int initalpha = 1 << NeuQuant.alphabiasshift;

		// Token: 0x04000DFF RID: 3583
		protected int alphadec;

		// Token: 0x04000E00 RID: 3584
		protected static readonly int radbiasshift = 8;

		// Token: 0x04000E01 RID: 3585
		protected static readonly int radbias = 1 << NeuQuant.radbiasshift;

		// Token: 0x04000E02 RID: 3586
		protected static readonly int alpharadbshift = NeuQuant.alphabiasshift + NeuQuant.radbiasshift;

		// Token: 0x04000E03 RID: 3587
		protected static readonly int alpharadbias = 1 << NeuQuant.alpharadbshift;

		// Token: 0x04000E04 RID: 3588
		protected byte[] thepicture;

		// Token: 0x04000E05 RID: 3589
		protected int lengthcount;

		// Token: 0x04000E06 RID: 3590
		protected int samplefac;

		// Token: 0x04000E07 RID: 3591
		protected int[][] network;

		// Token: 0x04000E08 RID: 3592
		protected int[] netindex = new int[256];

		// Token: 0x04000E09 RID: 3593
		protected int[] bias = new int[NeuQuant.PaletteSize];

		// Token: 0x04000E0A RID: 3594
		protected int[] freq = new int[NeuQuant.PaletteSize];

		// Token: 0x04000E0B RID: 3595
		protected int[] radpower = new int[NeuQuant.initrad];
	}
}
