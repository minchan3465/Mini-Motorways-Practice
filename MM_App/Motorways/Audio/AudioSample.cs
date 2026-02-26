using System;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200067A RID: 1658
	public class AudioSample
	{
		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06002E00 RID: 11776 RVA: 0x000D5A50 File Offset: 0x000D3C50
		// (set) Token: 0x06002E01 RID: 11777 RVA: 0x000D5A58 File Offset: 0x000D3C58
		public string PlayOrigin { get; private set; }

		// Token: 0x06002E02 RID: 11778 RVA: 0x000D5A64 File Offset: 0x000D3C64
		public AudioSample()
		{
			this._id = AudioSample._nextId++;
			this._sample = new GATRealTimeSample(null, this._panInfo);
			this._initialiseTime = -1.0;
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x000D5AB6 File Offset: 0x000D3CB6
		public bool Initialise(IGATDataOwner sampleData)
		{
			this.Recycle();
			this._sample.SetData(sampleData);
			this.Data = sampleData.AudioData;
			this._initialiseTime = AudioSystem.Instance.DspTime;
			return true;
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x06002E04 RID: 11780 RVA: 0x000D5AE7 File Offset: 0x000D3CE7
		// (set) Token: 0x06002E05 RID: 11781 RVA: 0x000D5AEF File Offset: 0x000D3CEF
		public string Name { get; set; }

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06002E06 RID: 11782 RVA: 0x000D5AF8 File Offset: 0x000D3CF8
		// (set) Token: 0x06002E07 RID: 11783 RVA: 0x000D5B00 File Offset: 0x000D3D00
		public bool IsImportant { get; set; }

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06002E08 RID: 11784 RVA: 0x000D5B09 File Offset: 0x000D3D09
		// (set) Token: 0x06002E09 RID: 11785 RVA: 0x000D5B16 File Offset: 0x000D3D16
		public bool IsLooping
		{
			get
			{
				return this._sample.Loop;
			}
			set
			{
				this._sample.Loop = value;
				this.Log("IsLooping = {0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06002E0A RID: 11786 RVA: 0x000D5B3E File Offset: 0x000D3D3E
		public GATRealTimeSample GATRealTimeSample
		{
			get
			{
				return this._sample;
			}
		}

		// Token: 0x06002E0B RID: 11787 RVA: 0x000D5B46 File Offset: 0x000D3D46
		public void ElegantStop()
		{
			this._sample.ElegantStop();
			this.Log("ElegantStop", Array.Empty<object>());
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06002E0C RID: 11788 RVA: 0x000D5B63 File Offset: 0x000D3D63
		// (set) Token: 0x06002E0D RID: 11789 RVA: 0x000D5B70 File Offset: 0x000D3D70
		public bool FadesIn
		{
			get
			{
				return this._sample.FadesIn;
			}
			set
			{
				this._sample.FadesIn = value;
				this.Log("FadesIn = {0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06002E0E RID: 11790 RVA: 0x000D5B98 File Offset: 0x000D3D98
		// (set) Token: 0x06002E0F RID: 11791 RVA: 0x000D5BA5 File Offset: 0x000D3DA5
		public double FadeInDuration
		{
			get
			{
				return this._sample.FadeInDuration;
			}
			set
			{
				this._sample.FadeInDuration = value;
				this.Log("FadeInDuration = {0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x06002E10 RID: 11792 RVA: 0x000D5BCD File Offset: 0x000D3DCD
		public void FadeOutAndStop(double fadeDuration)
		{
			this._sample.FadeOutAndStop(fadeDuration);
			this.Log("FadeOutAndStop({0})", new object[]
			{
				fadeDuration
			});
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x000D5BF5 File Offset: 0x000D3DF5
		public void ScheduleFadeOut(double fadeStartDspTime, double fadeDuration)
		{
			this._sample.ScheduleFadeOut(fadeStartDspTime, fadeDuration);
			this.Log("ScheduleFadeOut({0}, {1})", new object[]
			{
				fadeStartDspTime,
				fadeDuration
			});
		}

		// Token: 0x06002E12 RID: 11794 RVA: 0x000D5C28 File Offset: 0x000D3E28
		public void PlayPanned(float gain = 1f)
		{
			if (!this.IsImportant && (Get.State & StateType.Minimal) == StateType.Minimal)
			{
				return;
			}
			this.PlayOrigin = this.GetOrigin();
			this._panInfo.OnPlay();
			this._sample.PlayPanned(this.Player ?? GATManager.DefaultPlayer, gain);
			this.Log("PlayPanned({0})", new object[]
			{
				gain
			});
		}

		// Token: 0x06002E13 RID: 11795 RVA: 0x000D5C9C File Offset: 0x000D3E9C
		public void PlayScheduled(double dspTime, float gain = 1f)
		{
			if (!this.IsImportant && (Get.State & StateType.Minimal) == StateType.Minimal)
			{
				return;
			}
			this.PlayOrigin = this.GetOrigin();
			this._panInfo.OnPlay();
			this._sample.PlayScheduled(this.Player ?? GATManager.DefaultPlayer, dspTime, gain);
			this.Log("PlayScheduled({0}), Gain {1}", new object[]
			{
				dspTime,
				gain
			});
		}

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06002E14 RID: 11796 RVA: 0x000D5D1A File Offset: 0x000D3F1A
		// (set) Token: 0x06002E15 RID: 11797 RVA: 0x000D5D27 File Offset: 0x000D3F27
		public float FixedPan
		{
			get
			{
				return this._panInfo.FixedPan;
			}
			set
			{
				this._panInfo.FixedPan = value;
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06002E16 RID: 11798 RVA: 0x000D5D35 File Offset: 0x000D3F35
		// (set) Token: 0x06002E17 RID: 11799 RVA: 0x000D5D43 File Offset: 0x000D3F43
		public float Pitch
		{
			get
			{
				return (float)this._sample.Pitch;
			}
			set
			{
				this._sample.Pitch = (double)value;
				this.Log("Pitch = {0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x06002E18 RID: 11800 RVA: 0x000D5D6C File Offset: 0x000D3F6C
		public void SetStartPosition(float samplePoint)
		{
			this.GATRealTimeSample.StartPosition = Maf.FloorMod((int)(samplePoint * (float)AudioSettings.outputSampleRate), this.GATRealTimeSample.Length);
			this.GATRealTimeSample.SetLoopCallback(new GATRealTimeSample.SampleWillLoopHandler(this.ResetStartPosition));
		}

		// Token: 0x06002E19 RID: 11801 RVA: 0x000D5DA9 File Offset: 0x000D3FA9
		private bool ResetStartPosition(GATRealTimeSample loopingSample)
		{
			loopingSample.StartPosition = 0;
			return true;
		}

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06002E1A RID: 11802 RVA: 0x000D5DB3 File Offset: 0x000D3FB3
		public float Duration
		{
			get
			{
				return (float)this.GATRealTimeSample.Length / (float)AudioSettings.outputSampleRate;
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06002E1B RID: 11803 RVA: 0x000D5DC8 File Offset: 0x000D3FC8
		// (set) Token: 0x06002E1C RID: 11804 RVA: 0x000D5DD5 File Offset: 0x000D3FD5
		public IGATDynamicMixInfo DynamicMix
		{
			get
			{
				return this._sample.DynamicMix;
			}
			set
			{
				this._panInfo.DynamicMix = value;
				this._sample.ScheduleDynamicMix(value);
				this.Log("DynamicMin = {0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x06002E1D RID: 11805 RVA: 0x000D5E04 File Offset: 0x000D4004
		public override string ToString()
		{
			return string.Format("[AudioSample: Id={0}, PlayingStatus={1}, Name={2}, Origin={3}, Position={4} / {5}, Important={6}]", new object[]
			{
				this._id,
				this._sample.PlayingStatus,
				this.Name,
				this.PlayOrigin,
				this._sample.Position,
				this._sample.Length,
				this.IsImportant
			});
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06002E1E RID: 11806 RVA: 0x000D5E88 File Offset: 0x000D4088
		public bool CanRecycle
		{
			get
			{
				return this._sample.PlayingStatus == AGATWrappedSample.Status.ReadyToPlay && (this._initialiseTime < 0.0 || AudioSystem.Instance.DspTime - this._initialiseTime > 1.0);
			}
		}

		// Token: 0x06002E1F RID: 11807 RVA: 0x000D5EC8 File Offset: 0x000D40C8
		public void Recycle()
		{
			this.Log("Recycle()", Array.Empty<object>());
			this._panInfo.Recycle();
			this._sample.Reset();
			this._sample.Loop = false;
			this._sample.FadesIn = false;
			this._initialiseTime = -1.0;
			this.IsImportant = false;
			this.PlayOrigin = null;
		}

		// Token: 0x06002E20 RID: 11808 RVA: 0x000022F5 File Offset: 0x000004F5
		private void Log(string message, params object[] args)
		{
		}

		// Token: 0x06002E21 RID: 11809 RVA: 0x000D5F30 File Offset: 0x000D4130
		private string GetOrigin()
		{
			return "<unknown>";
		}

		// Token: 0x040027E4 RID: 10212
		private AudioSample.PanInfo _panInfo = new AudioSample.PanInfo();

		// Token: 0x040027E5 RID: 10213
		public GATPlayer Player;

		// Token: 0x040027E7 RID: 10215
		private GATRealTimeSample _sample;

		// Token: 0x040027E8 RID: 10216
		private double _initialiseTime;

		// Token: 0x040027E9 RID: 10217
		private int _id;

		// Token: 0x040027EA RID: 10218
		private static int _nextId = 1;

		// Token: 0x040027EB RID: 10219
		public GATData Data;

		// Token: 0x0200067B RID: 1659
		public class PanInfo : AGATPanInfo
		{
			// Token: 0x170007DD RID: 2013
			// (get) Token: 0x06002E23 RID: 11811 RVA: 0x000D5F3F File Offset: 0x000D413F
			// (set) Token: 0x06002E24 RID: 11812 RVA: 0x000D5F47 File Offset: 0x000D4147
			public IGATDynamicMixInfo DynamicMix { get; set; }

			// Token: 0x06002E25 RID: 11813 RVA: 0x000D5F50 File Offset: 0x000D4150
			public PanInfo()
			{
				this._channelsDirty = true;
			}

			// Token: 0x06002E26 RID: 11814 RVA: 0x000D5F7C File Offset: 0x000D417C
			public void Recycle()
			{
				this.DynamicMix = null;
				this.FixedPan = 0.5f;
				this._dynamicPanInfo.Active = false;
				this._dynamicPanInfo.SetGainForChannel(0.5f, 0);
				this._dynamicPanInfo.SetGainForChannel(0.5f, 1);
			}

			// Token: 0x06002E27 RID: 11815 RVA: 0x000D5FC9 File Offset: 0x000D41C9
			public void OnPlay()
			{
				this._dynamicPanInfo.Active = true;
				this._shouldSnapPan = true;
				this._channelsDirty = true;
			}

			// Token: 0x170007DE RID: 2014
			// (get) Token: 0x06002E28 RID: 11816 RVA: 0x000D5FE5 File Offset: 0x000D41E5
			// (set) Token: 0x06002E29 RID: 11817 RVA: 0x000D5FED File Offset: 0x000D41ED
			public float FixedPan
			{
				get
				{
					return this._fixedPan;
				}
				set
				{
					this._fixedPan = value;
					this._channelsDirty = true;
				}
			}

			// Token: 0x170007DF RID: 2015
			// (get) Token: 0x06002E2A RID: 11818 RVA: 0x000D5FFD File Offset: 0x000D41FD
			public override bool IsAudible
			{
				get
				{
					this.UpdateChannels();
					return this._dynamicPanInfo.IsAudible;
				}
			}

			// Token: 0x06002E2B RID: 11819 RVA: 0x000D6010 File Offset: 0x000D4210
			public override void PanMixSample(IGATBufferedSample sample, int length, float[] audioBuffer, float gain = 1f)
			{
				this.UpdateChannels();
				this._dynamicPanInfo.PanMixSample(sample, length, audioBuffer, gain);
			}

			// Token: 0x06002E2C RID: 11820 RVA: 0x000D6028 File Offset: 0x000D4228
			public override void PanMixProcessingBuffer(IGATBufferedSample sample, int length, float[] audioBuffer, float gain = 1f)
			{
				this.UpdateChannels();
				this._dynamicPanInfo.PanMixProcessingBuffer(sample, length, audioBuffer, gain);
			}

			// Token: 0x06002E2D RID: 11821 RVA: 0x000D6040 File Offset: 0x000D4240
			public override void SetGains(float[] gains)
			{
				if (gains.Length == 2)
				{
					this._dynamicPanInfo.SetGainForChannel(gains[0], 0);
					this._dynamicPanInfo.SetGainForChannel(gains[1], 1);
				}
			}

			// Token: 0x06002E2E RID: 11822 RVA: 0x000D6068 File Offset: 0x000D4268
			private void UpdateChannels()
			{
				IGATDynamicMixInfo dynamicMix = this.DynamicMix;
				if (dynamicMix == null && !this._channelsDirty)
				{
					return;
				}
				this._channelsDirty = false;
				float gain = 1f;
				float pan = this._fixedPan;
				if (dynamicMix != null)
				{
					float dynamicGain = dynamicMix.HasStaticGain ? dynamicMix.StaticGain : dynamicMix.Gain;
					gain = ((dynamicGain >= 0f) ? dynamicGain : gain);
					float dynamicPan = dynamicMix.HasStaticPan ? dynamicMix.StaticPan : dynamicMix.Pan;
					pan = ((dynamicPan >= 0f) ? dynamicPan : pan);
				}
				this._dynamicPanInfo.SetGainForChannel((1f - pan) * gain, 0);
				this._dynamicPanInfo.SetGainForChannel(pan * gain, 1);
				if (this._shouldSnapPan)
				{
					this._dynamicPanInfo.channelGains[0].Snap();
					this._dynamicPanInfo.channelGains[1].Snap();
				}
				this._shouldSnapPan = false;
			}

			// Token: 0x040027EE RID: 10222
			private float _fixedPan = 0.5f;

			// Token: 0x040027EF RID: 10223
			private bool _channelsDirty;

			// Token: 0x040027F0 RID: 10224
			private bool _shouldSnapPan;

			// Token: 0x040027F1 RID: 10225
			private GATDynamicPanInfo _dynamicPanInfo = new GATDynamicPanInfo(GATManager.DefaultPlayer, false);
		}
	}
}
