using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000635 RID: 1589
	public class AudioDataBank
	{
		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x06002C4C RID: 11340 RVA: 0x000C4711 File Offset: 0x000C2911
		// (set) Token: 0x06002C4D RID: 11341 RVA: 0x000C4719 File Offset: 0x000C2919
		public string Id { get; private set; }

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06002C4E RID: 11342 RVA: 0x000C4722 File Offset: 0x000C2922
		// (set) Token: 0x06002C4F RID: 11343 RVA: 0x000C472A File Offset: 0x000C292A
		public int Frequency { get; private set; }

		// Token: 0x06002C50 RID: 11344 RVA: 0x000C4734 File Offset: 0x000C2934
		public AudioDataBank(string id, int frequency)
		{
			this.Id = id;
			this.Frequency = frequency;
			this.outputSampleRate = AudioSettings.outputSampleRate;
			if (AudioDataBank.maxDecompressedSamples < 0)
			{
				AudioDataBank.maxDecompressedSamples = Mathf.FloorToInt(10485760f * ((float)this.outputSampleRate / 48000f));
			}
			if (this.Frequency == this.outputSampleRate)
			{
				this.resampleFactor = 1f;
				return;
			}
			if (this.outputSampleRate < 0)
			{
				this.resampleFactor = 0f;
				return;
			}
			this.resampleFactor = (float)this.outputSampleRate / (float)this.Frequency;
			if (this.resampleFactor > 1.5f)
			{
				this.resampleFactor = 2f;
				return;
			}
			this.resampleFactor = 1f;
		}

		// Token: 0x06002C51 RID: 11345 RVA: 0x000C480E File Offset: 0x000C2A0E
		public void AddSampleData(string name, int offset, int length)
		{
			this.samples.Add(new AudioSampleData(this, name, offset, length, this.GetResampledLength(length)));
			this.sampleIndex[name] = this.samples.Count - 1;
		}

		// Token: 0x06002C52 RID: 11346 RVA: 0x000C4844 File Offset: 0x000C2A44
		public bool Load(bool async)
		{
			float startTime = Time.realtimeSinceStartup;
			string bankName = this.Id + ".bytes";
			string bankPath = Path.Combine(Application.streamingAssetsPath, "Sounds", this.Frequency.ToString(), bankName);
			this.compressedData = File.ReadAllBytes(bankPath);
			if (this.compressedData == null)
			{
				AudioSystem.Log.Error("AudioBank: Failed to load compressed data from '{0}'.", new object[]
				{
					bankPath
				});
				return false;
			}
			this.compressedDataHandle = GCHandle.Alloc(this.compressedData, GCHandleType.Pinned);
			float endTime = Time.realtimeSinceStartup;
			AudioSystem.Log.Info("AudioDataBank: Loaded compressed bank '{0}' in {1}s.", new object[]
			{
				this.Id,
				endTime - startTime
			});
			return true;
		}

		// Token: 0x06002C53 RID: 11347 RVA: 0x000C48FC File Offset: 0x000C2AFC
		public AudioSampleData GetSampleData(string name)
		{
			int index = -1;
			if (this.sampleIndex.TryGetValue(name, out index))
			{
				return this.samples[index];
			}
			return null;
		}

		// Token: 0x06002C54 RID: 11348 RVA: 0x000C492C File Offset: 0x000C2B2C
		public short[] DecompressSample(AudioSampleData sample)
		{
			if (AudioDataBank.totalDecompressedSamples + sample.Length > AudioDataBank.maxDecompressedSamples)
			{
				double currentTime = AudioSettings.dspTime;
				for (int i = 0; i < this.samples.Count; i++)
				{
					double sampleTime = this.samples[i].LastUseTime;
					if (sampleTime > 0.0 && currentTime - sampleTime > 30.0)
					{
						AudioDataBank.totalDecompressedSamples -= this.samples[i].Length;
						AudioDataBank.freeSampleBuffers.Add(this.samples[i].SampleData);
						this.samples[i].Release();
					}
				}
			}
			AudioDataBank.totalDecompressedSamples += sample.Length;
			short[] sampleBuffer = null;
			for (int j = 0; j < AudioDataBank.freeSampleBuffers.Count; j++)
			{
				short[] freeSampleBuffer = AudioDataBank.freeSampleBuffers[j];
				int overflow = freeSampleBuffer.Length - sample.Length;
				if (overflow >= 0)
				{
					if (overflow == 0)
					{
						sampleBuffer = freeSampleBuffer;
						break;
					}
					if (sampleBuffer == null || sampleBuffer.Length > freeSampleBuffer.Length)
					{
						sampleBuffer = freeSampleBuffer;
					}
				}
			}
			if (sampleBuffer != null)
			{
				AudioDataBank.freeSampleBuffers.Remove(sampleBuffer);
			}
			else if (sampleBuffer == null)
			{
				sampleBuffer = new short[sample.Length];
			}
			GCHandle dataHandle = GCHandle.Alloc(sampleBuffer, GCHandleType.Pinned);
			AudioDataBank.decompressAudioSample(dataHandle.AddrOfPinnedObject(), 0, sample.NativeLength, sample.Length, this.outputSampleRate, this.compressedDataHandle.AddrOfPinnedObject(), sample.Offset, this.Frequency);
			dataHandle.Free();
			return sampleBuffer;
		}

		// Token: 0x06002C55 RID: 11349 RVA: 0x000C4AB0 File Offset: 0x000C2CB0
		public static void PruneSampleBuffers()
		{
			int totalFreeBufferSize = 0;
			for (int i = 0; i < AudioDataBank.freeSampleBuffers.Count; i++)
			{
				totalFreeBufferSize += AudioDataBank.freeSampleBuffers[i].Length;
			}
			if (totalFreeBufferSize * 2 > AudioDataBank.totalDecompressedSamples)
			{
				AudioDataBank.freeSampleBuffers.Clear();
				totalFreeBufferSize = 0;
			}
			AudioSystem.Log.Info("AudioDataBank: {0} bytes allocated for playing samples, {2} bytes allocated for {1} free sample buffers.\n\t{3} bytes allocated in total.", new object[]
			{
				AudioDataBank.totalDecompressedSamples * 2,
				AudioDataBank.freeSampleBuffers.Count,
				totalFreeBufferSize * 2,
				(AudioDataBank.totalDecompressedSamples + totalFreeBufferSize) * 2
			});
		}

		// Token: 0x06002C56 RID: 11350 RVA: 0x000C4B4C File Offset: 0x000C2D4C
		public int GetResampledLength(int nativeLength)
		{
			if (this.resampleFactor == 1f)
			{
				return nativeLength;
			}
			return Mathf.CeilToInt((float)nativeLength * this.resampleFactor);
		}

		// Token: 0x06002C57 RID: 11351
		[DllImport("decompressAudio", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool decompressAudioSample(IntPtr decompressedAudio, int decompressedOffset, int decompressedLength, int resampledLength, int decompressedSampleRate, IntPtr compressedAudio, int compressedOffset, int compressedSampleRate);

		// Token: 0x04002687 RID: 9863
		private List<AudioSampleData> samples = new List<AudioSampleData>();

		// Token: 0x04002688 RID: 9864
		private Dictionary<string, int> sampleIndex = new Dictionary<string, int>();

		// Token: 0x04002689 RID: 9865
		private const double MaxSampleAge = 30.0;

		// Token: 0x0400268A RID: 9866
		private byte[] compressedData;

		// Token: 0x0400268B RID: 9867
		private GCHandle compressedDataHandle;

		// Token: 0x0400268C RID: 9868
		private int outputSampleRate;

		// Token: 0x0400268D RID: 9869
		private float resampleFactor = 1f;

		// Token: 0x0400268E RID: 9870
		private static int totalDecompressedSamples = 0;

		// Token: 0x0400268F RID: 9871
		private static int maxDecompressedSamples = -1;

		// Token: 0x04002690 RID: 9872
		private static List<short[]> freeSampleBuffers = new List<short[]>();
	}
}
