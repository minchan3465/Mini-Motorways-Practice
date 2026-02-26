using System;
using System.IO;
using System.Runtime.InteropServices;
using GifEncoder;
using UnityEngine;

namespace Gif.Components
{
	// Token: 0x02000279 RID: 633
	public class AnimatedGifEncoder
	{
		// Token: 0x06000FA6 RID: 4006 RVA: 0x00034DA5 File Offset: 0x00032FA5
		public void SetDelay(int ms)
		{
			this._delay = (int)Math.Round((double)((float)ms / 10f));
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00034DBC File Offset: 0x00032FBC
		public void SetDispose(int code)
		{
			if (code >= 0)
			{
				this._dispose = code;
			}
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x00034DC9 File Offset: 0x00032FC9
		public void SetRepeat(int repeat)
		{
			if (repeat >= 0)
			{
				this._repeat = repeat;
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x00034DD8 File Offset: 0x00032FD8
		public bool AddFrame(Texture2D im)
		{
			if (im == null || !this._started)
			{
				return false;
			}
			bool ok = true;
			try
			{
				if (!this._sizeSet)
				{
					this.SetSize(im.width, im.height);
				}
				this._image = im;
				this.AnalyzePixels();
				if (this._firstFrame)
				{
					this.WriteLSD();
					this.WritePalette();
					if (this._repeat >= 0)
					{
						this.WriteNetscapeExt();
					}
				}
				this.WriteGraphicCtrlExt();
				this.WriteImageDesc();
				if (!this._firstFrame)
				{
					this.WritePalette();
				}
				this.WritePixels();
				this._firstFrame = false;
			}
			catch (IOException)
			{
				ok = false;
			}
			return ok;
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00034E84 File Offset: 0x00033084
		public bool AddComment(byte[] comment)
		{
			if (!this._started)
			{
				return false;
			}
			this._fs.WriteByte(33);
			this._fs.WriteByte(254);
			int totalLength = comment.Length;
			int blockLength;
			for (int next = 0; next < totalLength; next += blockLength)
			{
				blockLength = totalLength - next;
				blockLength = ((blockLength > 255) ? 255 : blockLength);
				this._fs.WriteByte((byte)blockLength);
				this._fs.Write(comment, next, blockLength);
			}
			this._fs.WriteByte(0);
			return true;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00034F08 File Offset: 0x00033108
		public bool Finish()
		{
			if (!this._started)
			{
				return false;
			}
			bool ok = true;
			this._started = false;
			try
			{
				this._fs.WriteByte(59);
				this._fs.Flush();
				if (this._closeStream)
				{
					this._fs.Close();
				}
			}
			catch (IOException)
			{
				ok = false;
			}
			this._transIndex = 0;
			this._fs = null;
			this._image = null;
			this._closeStream = false;
			this._firstFrame = true;
			AnimatedGifEncoder.FreeBuffer();
			return ok;
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00034F94 File Offset: 0x00033194
		public void SetFrameRate(float fps)
		{
			if (fps != 0f)
			{
				this._delay = (int)Math.Round((double)(100f / fps));
			}
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x00034FB2 File Offset: 0x000331B2
		public void SetQuality(int quality)
		{
			if (quality < 1)
			{
				quality = 1;
			}
			this._sample = quality;
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x00034FC4 File Offset: 0x000331C4
		public void SetSize(int w, int h)
		{
			if (this._started && !this._firstFrame)
			{
				return;
			}
			if (!Diagnostics.Verify(!this._sizeSet, "The size is already set! Finish this gif and start another instead."))
			{
				return;
			}
			this._width = w;
			this._height = h;
			if (this._width < 1)
			{
				this._width = 320;
			}
			if (this._height < 1)
			{
				this._height = 240;
			}
			this._sizeSet = true;
			AnimatedGifEncoder.AllocateBuffer(this._width, this._height);
			AnimatedGifEncoder._indexedPixels = new byte[this._width * this._height * 3];
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x00035060 File Offset: 0x00033260
		public bool Start(Stream os)
		{
			if (os == null)
			{
				return false;
			}
			bool ok = true;
			this._closeStream = false;
			this._fs = os;
			try
			{
				this.WriteString("GIF89a");
			}
			catch (IOException)
			{
				ok = false;
			}
			this._started = ok;
			return this._started;
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x000350B4 File Offset: 0x000332B4
		public bool Start(string file)
		{
			bool ok;
			try
			{
				this._fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				ok = this.Start(this._fs);
				this._closeStream = true;
			}
			catch (IOException)
			{
				ok = false;
			}
			this._started = ok;
			return this._started;
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00035108 File Offset: 0x00033308
		protected void AnalyzePixels()
		{
			object pixels = this._image.GetPixels32();
			GCHandle rawIndexedPixels = GCHandle.Alloc(AnimatedGifEncoder._indexedPixels, GCHandleType.Pinned);
			GCHandle rawColorTab = GCHandle.Alloc(AnimatedGifEncoder._colorTab, GCHandleType.Pinned);
			GCHandle rawPixels = GCHandle.Alloc(pixels, GCHandleType.Pinned);
			AnimatedGifEncoder.QuantizeImage(rawIndexedPixels.AddrOfPinnedObject(), rawColorTab.AddrOfPinnedObject(), rawPixels.AddrOfPinnedObject(), this._firstFrame);
			rawIndexedPixels.Free();
			rawColorTab.Free();
			rawPixels.Free();
			this._colorDepth = 8;
			this._palSize = 7;
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x00035184 File Offset: 0x00033384
		protected void WriteGraphicCtrlExt()
		{
			this._fs.WriteByte(33);
			this._fs.WriteByte(249);
			this._fs.WriteByte(4);
			if (this._firstFrame)
			{
				this._fs.WriteByte(0);
			}
			else
			{
				this._fs.WriteByte(5);
			}
			this.WriteShort(this._delay);
			this._fs.WriteByte(byte.MaxValue);
			this._fs.WriteByte(0);
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x00035204 File Offset: 0x00033404
		protected void WriteImageDesc()
		{
			this._fs.WriteByte(44);
			this.WriteShort(0);
			this.WriteShort(0);
			this.WriteShort(this._width);
			this.WriteShort(this._height);
			if (this._firstFrame)
			{
				this._fs.WriteByte(0);
				return;
			}
			this._fs.WriteByte(Convert.ToByte(128 | this._palSize));
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x00035278 File Offset: 0x00033478
		protected void WriteLSD()
		{
			this.WriteShort(this._width);
			this.WriteShort(this._height);
			this._fs.WriteByte(Convert.ToByte(240 | this._palSize));
			this._fs.WriteByte(0);
			this._fs.WriteByte(0);
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x000352D4 File Offset: 0x000334D4
		protected void WriteNetscapeExt()
		{
			this._fs.WriteByte(33);
			this._fs.WriteByte(byte.MaxValue);
			this._fs.WriteByte(11);
			this.WriteString("NETSCAPE2.0");
			this._fs.WriteByte(3);
			this._fs.WriteByte(1);
			this.WriteShort(this._repeat);
			this._fs.WriteByte(0);
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x00035348 File Offset: 0x00033548
		protected void WritePalette()
		{
			this._fs.Write(AnimatedGifEncoder._colorTab, 0, AnimatedGifEncoder._colorTab.Length);
			int i = 768 - AnimatedGifEncoder._colorTab.Length;
			for (int byteCount = 0; byteCount < i; byteCount++)
			{
				this._fs.WriteByte(0);
			}
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x00035393 File Offset: 0x00033593
		protected void WritePixels()
		{
			new LZWEncoder(this._width, this._height, AnimatedGifEncoder._indexedPixels, this._colorDepth).Encode(this._fs);
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x000353BC File Offset: 0x000335BC
		protected void WriteShort(int value)
		{
			this._fs.WriteByte(Convert.ToByte(value & 255));
			this._fs.WriteByte(Convert.ToByte(value >> 8 & 255));
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x000353F0 File Offset: 0x000335F0
		protected void WriteString(string s)
		{
			char[] chars = s.ToCharArray();
			for (int charIndex = 0; charIndex < chars.Length; charIndex++)
			{
				this._fs.WriteByte((byte)chars[charIndex]);
			}
		}

		// Token: 0x06000FBA RID: 4026
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
		private static extern void QuantizeImage(IntPtr indexedPixels, IntPtr colorMap, IntPtr pixels, bool isFirstImage);

		// Token: 0x06000FBB RID: 4027
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AllocateBuffer(int imageWidth, int imageHeight);

		// Token: 0x06000FBC RID: 4028
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
		private static extern void FreeBuffer();

		// Token: 0x04000E0C RID: 3596
		protected int _width;

		// Token: 0x04000E0D RID: 3597
		protected int _height;

		// Token: 0x04000E0E RID: 3598
		protected int _transIndex;

		// Token: 0x04000E0F RID: 3599
		protected int _repeat = -1;

		// Token: 0x04000E10 RID: 3600
		protected int _delay;

		// Token: 0x04000E11 RID: 3601
		protected bool _started;

		// Token: 0x04000E12 RID: 3602
		protected Stream _fs;

		// Token: 0x04000E13 RID: 3603
		protected Texture2D _image;

		// Token: 0x04000E14 RID: 3604
		protected int _colorDepth;

		// Token: 0x04000E15 RID: 3605
		protected int _palSize = 7;

		// Token: 0x04000E16 RID: 3606
		protected int _dispose = -1;

		// Token: 0x04000E17 RID: 3607
		protected bool _closeStream;

		// Token: 0x04000E18 RID: 3608
		protected bool _firstFrame = true;

		// Token: 0x04000E19 RID: 3609
		protected bool _sizeSet;

		// Token: 0x04000E1A RID: 3610
		protected int _sample = 10;

		// Token: 0x04000E1B RID: 3611
		protected static byte[] _indexedPixels = null;

		// Token: 0x04000E1C RID: 3612
		protected static byte[] _colorTab = new byte[765];

		// Token: 0x04000E1D RID: 3613
		private const int DefaultWidth = 320;

		// Token: 0x04000E1E RID: 3614
		private const int DefaultHeight = 240;
	}
}
