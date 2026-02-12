using System;
using System.IO;

namespace GifEncoder
{
	// Token: 0x02000277 RID: 631
	public class LZWEncoder
	{
		// Token: 0x06000F90 RID: 3984 RVA: 0x00033E1C File Offset: 0x0003201C
		public LZWEncoder(int width, int height, byte[] pixels, int color_depth)
		{
			this.imgW = width;
			this.imgH = height;
			this.pixAry = pixels;
			this.initCodeSize = Math.Max(2, color_depth);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x00033EC0 File Offset: 0x000320C0
		private void Add(byte c, Stream outs)
		{
			byte[] array = this.accum;
			int num = this.a_count;
			this.a_count = num + 1;
			array[num] = c;
			if (this.a_count >= 254)
			{
				this.Flush(outs);
			}
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x00033EFA File Offset: 0x000320FA
		private void ClearTable(Stream outs)
		{
			this.ResetCodeTable(this.hsize);
			this.free_ent = this.ClearCode + 2;
			this.clear_flg = true;
			this.Output(this.ClearCode, outs);
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x00033F2C File Offset: 0x0003212C
		private void ResetCodeTable(int hsize)
		{
			for (int i = 0; i < hsize; i++)
			{
				this.htab[i] = -1;
			}
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00033F50 File Offset: 0x00032150
		private void Compress(int init_bits, Stream outs)
		{
			this.g_init_bits = init_bits;
			this.clear_flg = false;
			this.n_bits = this.g_init_bits;
			this.maxcode = this.MaxCode(this.n_bits);
			this.ClearCode = 1 << init_bits - 1;
			this.EOFCode = this.ClearCode + 1;
			this.free_ent = this.ClearCode + 2;
			this.a_count = 0;
			int ent = this.NextPixel();
			int hshift = 0;
			for (int fcode = this.hsize; fcode < 65536; fcode *= 2)
			{
				hshift++;
			}
			hshift = 8 - hshift;
			int hsize_reg = this.hsize;
			this.ResetCodeTable(hsize_reg);
			this.Output(this.ClearCode, outs);
			int c;
			while ((c = this.NextPixel()) != LZWEncoder.EOF)
			{
				int fcode = (c << this.maxbits) + ent;
				int i = c << hshift ^ ent;
				if (this.htab[i] == fcode)
				{
					ent = this.codetab[i];
				}
				else
				{
					if (this.htab[i] >= 0)
					{
						int disp = hsize_reg - i;
						if (i == 0)
						{
							disp = 1;
						}
						for (;;)
						{
							if ((i -= disp) < 0)
							{
								i += hsize_reg;
							}
							if (this.htab[i] == fcode)
							{
								break;
							}
							if (this.htab[i] < 0)
							{
								goto IL_121;
							}
						}
						ent = this.codetab[i];
						continue;
					}
					IL_121:
					this.Output(ent, outs);
					ent = c;
					if (this.free_ent < this.maxmaxcode)
					{
						int[] array = this.codetab;
						int num = i;
						int num2 = this.free_ent;
						this.free_ent = num2 + 1;
						array[num] = num2;
						this.htab[i] = fcode;
					}
					else
					{
						this.ClearTable(outs);
					}
				}
			}
			this.Output(ent, outs);
			this.Output(this.EOFCode, outs);
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x000340EC File Offset: 0x000322EC
		public void Encode(Stream os)
		{
			os.WriteByte(Convert.ToByte(this.initCodeSize));
			this.remaining = this.imgW * this.imgH;
			this.curPixel = 0;
			this.Compress(this.initCodeSize + 1, os);
			os.WriteByte(0);
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0003413A File Offset: 0x0003233A
		private void Flush(Stream outs)
		{
			if (this.a_count > 0)
			{
				outs.WriteByte(Convert.ToByte(this.a_count));
				outs.Write(this.accum, 0, this.a_count);
				this.a_count = 0;
			}
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x00034170 File Offset: 0x00032370
		private int MaxCode(int n_bits)
		{
			return (1 << n_bits) - 1;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0003417C File Offset: 0x0003237C
		private int NextPixel()
		{
			if (this.remaining == 0)
			{
				return LZWEncoder.EOF;
			}
			this.remaining--;
			if (this.curPixel + 1 < this.pixAry.GetUpperBound(0))
			{
				byte[] array = this.pixAry;
				int num = this.curPixel;
				this.curPixel = num + 1;
				return array[num] & 255;
			}
			return 255;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x000341E0 File Offset: 0x000323E0
		private void Output(int code, Stream outs)
		{
			this.cur_accum &= this.masks[this.cur_bits];
			if (this.cur_bits > 0)
			{
				this.cur_accum |= code << this.cur_bits;
			}
			else
			{
				this.cur_accum = code;
			}
			this.cur_bits += this.n_bits;
			while (this.cur_bits >= 8)
			{
				this.Add((byte)(this.cur_accum & 255), outs);
				this.cur_accum >>= 8;
				this.cur_bits -= 8;
			}
			if (this.free_ent > this.maxcode || this.clear_flg)
			{
				if (this.clear_flg)
				{
					this.maxcode = this.MaxCode(this.n_bits = this.g_init_bits);
					this.clear_flg = false;
				}
				else
				{
					this.n_bits++;
					if (this.n_bits == this.maxbits)
					{
						this.maxcode = this.maxmaxcode;
					}
					else
					{
						this.maxcode = this.MaxCode(this.n_bits);
					}
				}
			}
			if (code == this.EOFCode)
			{
				while (this.cur_bits > 0)
				{
					this.Add((byte)(this.cur_accum & 255), outs);
					this.cur_accum >>= 8;
					this.cur_bits -= 8;
				}
				this.Flush(outs);
			}
		}

		// Token: 0x04000DCE RID: 3534
		private static readonly int EOF = -1;

		// Token: 0x04000DCF RID: 3535
		private int imgW;

		// Token: 0x04000DD0 RID: 3536
		private int imgH;

		// Token: 0x04000DD1 RID: 3537
		private byte[] pixAry;

		// Token: 0x04000DD2 RID: 3538
		private int initCodeSize;

		// Token: 0x04000DD3 RID: 3539
		private int remaining;

		// Token: 0x04000DD4 RID: 3540
		private int curPixel;

		// Token: 0x04000DD5 RID: 3541
		private static readonly int BITS = 12;

		// Token: 0x04000DD6 RID: 3542
		private static readonly int HSIZE = 5003;

		// Token: 0x04000DD7 RID: 3543
		private int n_bits;

		// Token: 0x04000DD8 RID: 3544
		private int maxbits = LZWEncoder.BITS;

		// Token: 0x04000DD9 RID: 3545
		private int maxcode;

		// Token: 0x04000DDA RID: 3546
		private int maxmaxcode = 1 << LZWEncoder.BITS;

		// Token: 0x04000DDB RID: 3547
		private int[] htab = new int[LZWEncoder.HSIZE];

		// Token: 0x04000DDC RID: 3548
		private int[] codetab = new int[LZWEncoder.HSIZE];

		// Token: 0x04000DDD RID: 3549
		private int hsize = LZWEncoder.HSIZE;

		// Token: 0x04000DDE RID: 3550
		private int free_ent;

		// Token: 0x04000DDF RID: 3551
		private bool clear_flg;

		// Token: 0x04000DE0 RID: 3552
		private int g_init_bits;

		// Token: 0x04000DE1 RID: 3553
		private int ClearCode;

		// Token: 0x04000DE2 RID: 3554
		private int EOFCode;

		// Token: 0x04000DE3 RID: 3555
		private int cur_accum;

		// Token: 0x04000DE4 RID: 3556
		private int cur_bits;

		// Token: 0x04000DE5 RID: 3557
		private int[] masks = new int[]
		{
			0,
			1,
			3,
			7,
			15,
			31,
			63,
			127,
			255,
			511,
			1023,
			2047,
			4095,
			8191,
			16383,
			32767,
			65535
		};

		// Token: 0x04000DE6 RID: 3558
		private int a_count;

		// Token: 0x04000DE7 RID: 3559
		private byte[] accum = new byte[256];
	}
}
