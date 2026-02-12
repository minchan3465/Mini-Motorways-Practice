using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006AD RID: 1709
	public class MusicData
	{
		// Token: 0x06002F25 RID: 12069 RVA: 0x000DBA44 File Offset: 0x000D9C44
		public double FadeInTime()
		{
			double baseline = (AudioSystem.Instance.ActivePulseTimeScale == TimeScale.Single) ? this.FadeInTimeNormal : this.FadeInTimePaused;
			double progress = (this.FadeInProgressionZ < 0.001) ? Maf.Lerp(this.FadeInProgression, this.FadeInProgressionZ, (double)Get.ZoomOutProgress) : this.FadeInProgression;
			if (!this.FadeInProgressionAsMultiplier)
			{
				return baseline + progress;
			}
			return baseline * progress;
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x06002F26 RID: 12070 RVA: 0x000DBAB1 File Offset: 0x000D9CB1
		// (set) Token: 0x06002F27 RID: 12071 RVA: 0x000DBAB9 File Offset: 0x000D9CB9
		public List<string> NoteWindow { get; private set; }

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x06002F28 RID: 12072 RVA: 0x000DBAC2 File Offset: 0x000D9CC2
		// (set) Token: 0x06002F29 RID: 12073 RVA: 0x000DBACA File Offset: 0x000D9CCA
		public int NotePointer
		{
			get
			{
				return this._notePointer;
			}
			set
			{
				this._notePointer = value;
				this.UpdateNoteWindow(-1, 1f, 0, 0f, false);
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x06002F2A RID: 12074 RVA: 0x000DBAE6 File Offset: 0x000D9CE6
		// (set) Token: 0x06002F2B RID: 12075 RVA: 0x000DBAED File Offset: 0x000D9CED
		public static int TotalCommonToneAttempts { get; private set; }

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x06002F2C RID: 12076 RVA: 0x000DBAF5 File Offset: 0x000D9CF5
		// (set) Token: 0x06002F2D RID: 12077 RVA: 0x000DBAFC File Offset: 0x000D9CFC
		public static int TotalCommonToneFailures { get; private set; }

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x06002F2E RID: 12078 RVA: 0x000DBB04 File Offset: 0x000D9D04
		// (set) Token: 0x06002F2F RID: 12079 RVA: 0x000DBB0B File Offset: 0x000D9D0B
		public static int TotalCommonToneMaxIterations { get; private set; }

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x06002F30 RID: 12080 RVA: 0x000DBB13 File Offset: 0x000D9D13
		// (set) Token: 0x06002F31 RID: 12081 RVA: 0x000DBB1B File Offset: 0x000D9D1B
		public int CommonToneAttempts { get; private set; }

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x06002F32 RID: 12082 RVA: 0x000DBB24 File Offset: 0x000D9D24
		// (set) Token: 0x06002F33 RID: 12083 RVA: 0x000DBB2C File Offset: 0x000D9D2C
		public int CommonToneFailures { get; private set; }

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x06002F34 RID: 12084 RVA: 0x000DBB35 File Offset: 0x000D9D35
		// (set) Token: 0x06002F35 RID: 12085 RVA: 0x000DBB3D File Offset: 0x000D9D3D
		public int CommonToneMaxIterations { get; private set; }

		// Token: 0x06002F36 RID: 12086 RVA: 0x000DBB48 File Offset: 0x000D9D48
		public void UpdateNoteWindow(int commonTones = -1, float chordChangeProbability = 1f, int transposeBy = 0, float keyChangeProbability = 0f, bool forceChange = false)
		{
			MusicData.<>c__DisplayClass48_0 CS$<>8__locals1 = new MusicData.<>c__DisplayClass48_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.commonTones = commonTones;
			if (!this.DefaultNoteWindowBehavior && !forceChange)
			{
				return;
			}
			if (CS$<>8__locals1.commonTones < 0)
			{
				CS$<>8__locals1.commonTones = Get.MaxGroups - 1;
			}
			if (transposeBy != 0 && Rando.FlipCoin(keyChangeProbability))
			{
				this.NoteWindow = Note.Transpose(transposeBy, this.NoteWindow);
				this.CurrentKey = Maf.FloorMod(this.CurrentKey + transposeBy, 12);
				this.CurrentScale.Key = this.CurrentKey;
			}
			if (Rando.FlipCoin(chordChangeProbability))
			{
				CS$<>8__locals1.<UpdateNoteWindow>g__FindNewChord|0();
			}
			Dbug.Assert(this.CurrentScale != null);
			Dbug.Assert(this.CurrentQuality != null);
			if (this is Menu)
			{
				MusicData.NoteWindowMenu = this.NoteWindow.ToList<string>();
				MusicData.CurrentScaleMenu = this.CurrentScale;
				MusicData.CurrentQualityMenu = this.CurrentQuality;
			}
			DestinationGroup.DivvyUpNoteWindow();
		}

		// Token: 0x06002F37 RID: 12087 RVA: 0x000DBC2F File Offset: 0x000D9E2F
		public float EchoDuration()
		{
			return Get.Pulse.Duratio((this.EchoDuratios != null) ? Rando.Pick<float>(this.EchoDuratios) : 1f);
		}

		// Token: 0x06002F38 RID: 12088 RVA: 0x000DBC50 File Offset: 0x000D9E50
		public void SetFadeInTimes(double normal, double paused)
		{
			this.FadeInTimeNormal = normal;
			this.FadeInTimePaused = paused;
		}

		// Token: 0x06002F39 RID: 12089 RVA: 0x000DBC60 File Offset: 0x000D9E60
		public void SetFadeInProgression(double start, double end, bool asMultiplier)
		{
			this.FadeInProgressionAsMultiplier = asMultiplier;
			this.FadeInProgression = start;
			this.FadeInProgressionZ = end;
		}

		// Token: 0x06002F3A RID: 12090 RVA: 0x000DBC77 File Offset: 0x000D9E77
		public void SetTremolo(Param.LFO a, Param.LFO z = null)
		{
			this.Tremolo = a;
			this.TremoloZ = z;
		}

		// Token: 0x06002F3B RID: 12091 RVA: 0x000DBC87 File Offset: 0x000D9E87
		public void SetVibrato(Param.Vibrato a, Param.Vibrato z = null)
		{
			this.Vibrato = a;
			this.VibratoZ = z;
		}

		// Token: 0x06002F3C RID: 12092 RVA: 0x000DBC97 File Offset: 0x000D9E97
		public void SetPortamento(Param.Portamento a, Param.Portamento z = null)
		{
			this.Portamento = a;
			this.PortamentoZ = z;
		}

		// Token: 0x06002F3D RID: 12093 RVA: 0x000DBCA7 File Offset: 0x000D9EA7
		public void SetVoiceLimits(double globalFadeOut, int globalPolyphony, double localFadeOut = 0.0, int localPolyphony = 5)
		{
			this.GlobalFadeOut = globalFadeOut;
			this.GlobalPolyphony = globalPolyphony;
			this.LocalPolyphony = localPolyphony;
			this.LocalFadeOut = localFadeOut;
		}

		// Token: 0x06002F3E RID: 12094 RVA: 0x000DBCC6 File Offset: 0x000D9EC6
		public void SetQualities(List<Quality> dayQualities, List<Quality> nightQualities = null, bool defaultNoteWindowBehavior = true)
		{
			this.DefaultNoteWindowBehavior = defaultNoteWindowBehavior;
			this.DayQualities = dayQualities.ToList<Quality>();
			this.NightQualities = (((nightQualities != null) ? nightQualities.ToList<Quality>() : null) ?? null);
		}

		// Token: 0x06002F3F RID: 12095 RVA: 0x000DBCF2 File Offset: 0x000D9EF2
		public void SetNoteSequenceStyles(List<MusicData.NoteSequenceType> styles)
		{
			this.NoteSequenceStyles = styles.ToList<MusicData.NoteSequenceType>();
		}

		// Token: 0x06002F40 RID: 12096 RVA: 0x000DBD00 File Offset: 0x000D9F00
		public void SetRhythms(List<Rhythm> rhythms, MusicData.RhythmUpdateType type = MusicData.RhythmUpdateType.RandomParallel)
		{
			this.Rhythms = rhythms.ToList<Rhythm>();
			this.RhythmType = type;
		}

		// Token: 0x06002F41 RID: 12097 RVA: 0x000DBD18 File Offset: 0x000D9F18
		public void SetDrumSequencer(Rhythm rhythm, bool boom = false, bool bap = false, bool hat = false, bool useEuclideanGates = true, float delayDuration = -1f, float attackDuration = -1f)
		{
			this.DrumSequencerRhythm = (rhythm ?? this.DrumSequencerRhythm);
			this.Boom = boom;
			this.Bap = bap;
			this.Hat = hat;
			this.UseEuclideanDrumGates = useEuclideanGates;
			this.DrumDelayDuration = ((delayDuration < 0f) ? this.DrumDelayDuration : delayDuration);
			this.DrumAttackDuration = ((attackDuration < 0f) ? this.DrumAttackDuration : attackDuration);
		}

		// Token: 0x06002F42 RID: 12098 RVA: 0x000DBD88 File Offset: 0x000D9F88
		public void UpdateDrumSequencer(Rhythm rhythm, bool boom = false, bool bap = false, bool hat = false, bool flipEuclideanGates = false)
		{
			bool changePulse = rhythm != this.DrumSequencerRhythm;
			this.DrumSequencerRhythm = (rhythm ?? this.DrumSequencerRhythm);
			if (this.DrumSequencerRhythm == null)
			{
				return;
			}
			this.Boom = boom;
			this.Bap = bap;
			this.Hat = hat;
			if (flipEuclideanGates)
			{
				this.UseEuclideanDrumGates = !this.UseEuclideanDrumGates;
			}
			if (changePulse)
			{
				DrumSequencer drumSequencer = Get.Loadout.DrumSequencer;
				if (drumSequencer == null)
				{
					return;
				}
				drumSequencer.ChangePulse(this.DrumSequencerRhythm);
			}
		}

		// Token: 0x06002F43 RID: 12099 RVA: 0x000DBE02 File Offset: 0x000DA002
		public void SetEchoDuratios(List<float> duratios)
		{
			this.EchoDuratios = duratios.ToList<float>();
		}

		// Token: 0x06002F44 RID: 12100 RVA: 0x000DBE10 File Offset: 0x000DA010
		public void SetSeed(int seed)
		{
			this.Seed = seed;
		}

		// Token: 0x06002F45 RID: 12101 RVA: 0x000DBE19 File Offset: 0x000DA019
		public void SetKeyDeltas(List<int> weekend, int starting = 20)
		{
			this.WeekendTranspositions = (weekend ?? Liszt.From<int>(new int[1]));
			this.StartingKey = ((starting != 20) ? starting : this.D20.Pick<int>(this.WeekendTranspositions));
		}

		// Token: 0x06002F46 RID: 12102 RVA: 0x000DBE50 File Offset: 0x000DA050
		public void SetWeekendChances(float chordChange = 1f, float keyChange = 1f)
		{
			this.WeekendQualityChangeChance = chordChange;
			this.WeekendKeyChangeChance = keyChange;
		}

		// Token: 0x06002F47 RID: 12103 RVA: 0x000DBE60 File Offset: 0x000DA060
		public void SetEasterEggHorn(string suffix)
		{
			this.EasterEggHorn = suffix;
		}

		// Token: 0x06002F48 RID: 12104 RVA: 0x000DBE6C File Offset: 0x000DA06C
		public void UpdateTrain(int patternLengthOverride = -1, float kickDoublingProbability = -1f, int bVariablePulse = -1, params string[] engines)
		{
			if (patternLengthOverride > 0)
			{
				Get.Loadout.Train.PatternLengthOverride = patternLengthOverride;
			}
			if (kickDoublingProbability >= --0f)
			{
				Get.Loadout.Train.KickDoublingProbability = kickDoublingProbability;
			}
			if (bVariablePulse > -1)
			{
				Get.Loadout.Train.VariablePulseMode = (bVariablePulse == 1);
			}
			if (engines.Length != 0)
			{
				Get.Loadout.Train.TrainEngines = engines.ToList<string>();
			}
			Get.Loadout.Train.Reseed();
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x000DBEE6 File Offset: 0x000DA0E6
		public virtual int ChordSize()
		{
			return this.NoteWindow.Count;
		}

		// Token: 0x06002F4A RID: 12106 RVA: 0x000DBEF3 File Offset: 0x000DA0F3
		public virtual float ChordSpread()
		{
			return 0.05f;
		}

		// Token: 0x06002F4B RID: 12107 RVA: 0x000DBEFC File Offset: 0x000DA0FC
		public virtual float SamplePitchSign()
		{
			if (!Get.Game.Simulation.IsPaused && !Get.State.HasFlag(StateType.ModeDelete) && !Rando.FlipCoin(Clock.GainFactor))
			{
				return 1f;
			}
			return -1f;
		}

		// Token: 0x06002F4C RID: 12108 RVA: 0x000DBF4C File Offset: 0x000DA14C
		public MusicData()
		{
			this.SetFadeInProgression(0.0, 0.0, false);
			this.SetFadeInTimes(0.0, 0.0);
			this.SetQualities(QualityDatabase.ALL, null, true);
			this.SetKeyDeltas(Rando.Numbers(13, -6), 20);
			this.SetWeekendChances(1f, 1f);
			this.SetTremolo(new Param.LFO(new Param.Data(0.25f, 10f), new Param.Data(0f, 0.5f)), null);
			this.SetVibrato(new Param.Vibrato(new Param.Data(4f, 10f), 15), null);
			this.SetPortamento(new Param.Portamento(0, 0, 0.0, 0.0), null);
			this.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.AutoReroll));
			this.SetVoiceLimits((double)this.D20.Range(0.01f, 2f), this.D20.Pick<int>(new int[]
			{
				1,
				2,
				3,
				0
			}), 0.0, 5);
			this.SetRhythms(Rhythm.Frags(-1), MusicData.RhythmUpdateType.RandomParallel);
			this.SetEchoDuratios(Liszt.From<float>(new float[]
			{
				0.5f,
				0.33333334f,
				0.25f,
				0.16666667f,
				0.125f,
				1.25f,
				1.3333334f,
				1.5f
			}));
			this.SetDrumSequencer(new Rhythm(0f, new float[]
			{
				1f
			}), false, false, false, true, -1f, -1f);
			if (!(this is Menu))
			{
				Settings.PITCH_NIGHT = this.D20.Range(1.2f, 1.6666666f);
				Settings.PITCH_PAUSE = this.D20.Range(0.8333333f, 0.9375f);
			}
		}

		// Token: 0x06002F4D RID: 12109 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void Injections()
		{
		}

		// Token: 0x06002F4E RID: 12110 RVA: 0x000DC30A File Offset: 0x000DA50A
		public virtual void OnNewWeek()
		{
			this.SetEchoDuratios(Liszt.From<float>(new float[]
			{
				this.Rhythms.SafeGet(this.RhythmPointer).Duration
			}));
			FX.UpdateEcho();
		}

		// Token: 0x06002F4F RID: 12111 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDestinationActivated(int index)
		{
		}

		// Token: 0x06002F50 RID: 12112 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDawn()
		{
		}

		// Token: 0x06002F51 RID: 12113 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDusk()
		{
		}

		// Token: 0x06002F52 RID: 12114 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDay()
		{
		}

		// Token: 0x06002F53 RID: 12115 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnHour()
		{
		}

		// Token: 0x06002F54 RID: 12116 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDestinationConnected(int groupIndex)
		{
		}

		// Token: 0x06002F55 RID: 12117 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnHouseConnected(int groupIndex)
		{
		}

		// Token: 0x06002F56 RID: 12118 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnConnection()
		{
		}

		// Token: 0x06002F57 RID: 12119 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDrumPulse()
		{
		}

		// Token: 0x06002F58 RID: 12120 RVA: 0x000DC33B File Offset: 0x000DA53B
		public virtual void OnTrainArrived()
		{
			Get.Loadout.Train.Reseed();
		}

		// Token: 0x06002F59 RID: 12121 RVA: 0x000DC34C File Offset: 0x000DA54C
		public virtual void OnRhythmUpdate(int groupIndex)
		{
			switch (this.RhythmType)
			{
			case MusicData.RhythmUpdateType.RandomParallel:
				goto IL_111;
			case MusicData.RhythmUpdateType.LinearParallel:
				using (List<DestinationGroup>.Enumerator enumerator = Get.Loadout.DestinationGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DestinationGroup d = enumerator.Current;
						d.Module.ChangePulse(this.Rhythms.SafeGet(d.Index + this.RhythmPointer));
					}
					goto IL_1B7;
				}
				break;
			case MusicData.RhythmUpdateType.LinearUniform:
				goto IL_16A;
			case MusicData.RhythmUpdateType.RandomSingle:
				break;
			case MusicData.RhythmUpdateType.RandomAll:
				using (List<DestinationGroup>.Enumerator enumerator = Get.Loadout.DestinationGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DestinationGroup destinationGroup = enumerator.Current;
						destinationGroup.Module.ChangePulse(Rando.Pick<Rhythm>(this.Rhythms));
					}
					goto IL_1B7;
				}
				goto IL_111;
			default:
				goto IL_1B7;
			}
			List<DestinationGroup> destinationGroups = Get.Loadout.DestinationGroups;
			if (groupIndex >= 0 && groupIndex < destinationGroups.Count)
			{
				Get.Loadout.DestinationGroups[groupIndex].Module.ChangePulse(Rando.Pick<Rhythm>(this.Rhythms));
				goto IL_1B7;
			}
			goto IL_1B7;
			IL_111:
			using (List<DestinationGroup>.Enumerator enumerator = Get.Loadout.DestinationGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DestinationGroup d2 = enumerator.Current;
					d2.Module.ChangePulse(this.Rhythms.SafeGet(d2.Seed + this.RhythmPointer));
				}
				goto IL_1B7;
			}
			IL_16A:
			foreach (DestinationGroup destinationGroup2 in Get.Loadout.DestinationGroups)
			{
				destinationGroup2.Module.ChangePulse(this.Rhythms.SafeGet(this.RhythmPointer));
			}
			IL_1B7:
			this.RhythmPointer++;
		}

		// Token: 0x06002F5A RID: 12122 RVA: 0x000DC554 File Offset: 0x000DA754
		public void Initialize()
		{
			this.CurrentQualities = this.DayQualities.ToList<Quality>();
			if (Get.State.HasFlag(StateType.ModeNight) && this.NightQualities != null)
			{
				this.CurrentQualities = this.NightQualities.ToList<Quality>();
			}
			this.Timbres.Shuffle(this.D20, -1);
			this.GroupEngines.Shuffle(this.D20, -1);
			this.GroupPrefices.Shuffle(this.D20, -1);
			this.Timbres = this.Timbres.GetRange(0, 6);
			this.GroupEngines = this.GroupEngines.GetRange(0, 6);
			this.GroupPrefices = this.GroupPrefices.GetRange(0, 6);
			this.CurrentKey = Maf.FloorMod(this.StartingKey, 12);
			this.UpdateNoteWindow(-1, 1f, 0, 0f, true);
			Dbug.Log.Info(this._ToString(), Array.Empty<object>());
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x000DC653 File Offset: 0x000DA853
		public virtual void PostLoad()
		{
			this.UpdateDrumSequencer(this.DrumSequencerRhythm, this.Boom, this.Bap, this.Hat, false);
			this.timeAtStart = Time.time;
		}

		// Token: 0x06002F5C RID: 12124 RVA: 0x000DC680 File Offset: 0x000DA880
		private string _ToString()
		{
			string format = "{0}.\n\nSeed: {1}, StartingKey: {2}\n\nRhythms:\n{3}\n\nPitch Night: {4}, Pitch Pause: {5}\n\nTimbres: {6}\n\nEngines:\n{7}\n\nVoice Limiting:\n{8}\n\nNote Sequence Styles:\n{9}\n\nCurrentScale:\n{10}";
			object[] array = new object[11];
			array[0] = base.GetType().ToString();
			array[1] = this.Seed;
			array[2] = this.StartingKey;
			array[3] = string.Join<Rhythm>("\n", this.Rhythms);
			array[4] = Settings.PITCH_NIGHT;
			array[5] = Settings.PITCH_PAUSE;
			array[6] = string.Join(", ", this.Timbres);
			array[7] = string.Join<MusicData.EngineData>("\n", this.GroupEngines);
			array[8] = string.Format("FadeTime: {0:0.###}, Polyphony: {1}", this.GlobalFadeOut, this.GlobalPolyphony);
			array[9] = string.Join<MusicData.NoteSequenceType>("\n", this.NoteSequenceStyles);
			int num = 10;
			Scale currentScale = this.CurrentScale;
			array[num] = (((currentScale != null) ? currentScale.FullName() : null) ?? "Time Out");
			return string.Format(format, array);
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x000DC771 File Offset: 0x000DA971
		public override string ToString()
		{
			return this._ToString();
		}

		// Token: 0x06002F5E RID: 12126 RVA: 0x000DC77C File Offset: 0x000DA97C
		public virtual Rhythm PickInitRhythm(int groupIndex)
		{
			switch (this.RhythmType)
			{
			case MusicData.RhythmUpdateType.RandomParallel:
			case MusicData.RhythmUpdateType.RandomSingle:
				return Rando.Pick<Rhythm>(this.Rhythms);
			case MusicData.RhythmUpdateType.LinearParallel:
				return this.Rhythms.SafeGet(groupIndex);
			case MusicData.RhythmUpdateType.LinearUniform:
				return new D20(this.Seed).Pick<Rhythm>(this.Rhythms);
			default:
				return this.Rhythms.SafeGet(groupIndex);
			}
		}

		// Token: 0x040028AC RID: 10412
		public AudioSample Bass;

		// Token: 0x040028AE RID: 10414
		public List<Quality> CurrentQualities;

		// Token: 0x040028AF RID: 10415
		public Scale CurrentScale;

		// Token: 0x040028B0 RID: 10416
		public Quality CurrentQuality;

		// Token: 0x040028B1 RID: 10417
		public static List<string> NoteWindowMenu;

		// Token: 0x040028B2 RID: 10418
		public static Scale CurrentScaleMenu;

		// Token: 0x040028B3 RID: 10419
		public static Quality CurrentQualityMenu;

		// Token: 0x040028B4 RID: 10420
		private int _notePointer;

		// Token: 0x040028B5 RID: 10421
		public D20 D20 = new D20(-1);

		// Token: 0x040028B6 RID: 10422
		public int RhythmPointer;

		// Token: 0x040028B7 RID: 10423
		public int CurrentKey;

		// Token: 0x040028B8 RID: 10424
		public int StartingKey;

		// Token: 0x040028B9 RID: 10425
		public static int MenuKey;

		// Token: 0x040028C0 RID: 10432
		private double FadeInTimeNormal;

		// Token: 0x040028C1 RID: 10433
		private double FadeInTimePaused;

		// Token: 0x040028C2 RID: 10434
		private double FadeInProgression;

		// Token: 0x040028C3 RID: 10435
		private double FadeInProgressionZ;

		// Token: 0x040028C4 RID: 10436
		private bool FadeInProgressionAsMultiplier;

		// Token: 0x040028C5 RID: 10437
		public Param.LFO Tremolo;

		// Token: 0x040028C6 RID: 10438
		public Param.LFO TremoloZ;

		// Token: 0x040028C7 RID: 10439
		public Param.Vibrato Vibrato;

		// Token: 0x040028C8 RID: 10440
		public Param.Vibrato VibratoZ;

		// Token: 0x040028C9 RID: 10441
		public Param.Portamento Portamento;

		// Token: 0x040028CA RID: 10442
		public Param.Portamento PortamentoZ;

		// Token: 0x040028CB RID: 10443
		public int GlobalPolyphony;

		// Token: 0x040028CC RID: 10444
		public double GlobalFadeOut;

		// Token: 0x040028CD RID: 10445
		public int LocalPolyphony;

		// Token: 0x040028CE RID: 10446
		public double LocalFadeOut;

		// Token: 0x040028CF RID: 10447
		public List<Quality> DayQualities;

		// Token: 0x040028D0 RID: 10448
		public List<Quality> NightQualities;

		// Token: 0x040028D1 RID: 10449
		public bool DefaultNoteWindowBehavior;

		// Token: 0x040028D2 RID: 10450
		public List<MusicData.NoteSequenceType> NoteSequenceStyles;

		// Token: 0x040028D3 RID: 10451
		private MusicData.RhythmUpdateType RhythmType;

		// Token: 0x040028D4 RID: 10452
		public List<Rhythm> Rhythms;

		// Token: 0x040028D5 RID: 10453
		public Rhythm DrumSequencerRhythm;

		// Token: 0x040028D6 RID: 10454
		public bool Boom;

		// Token: 0x040028D7 RID: 10455
		public bool Bap;

		// Token: 0x040028D8 RID: 10456
		public bool Hat;

		// Token: 0x040028D9 RID: 10457
		public float DrumVolume = 0.6f;

		// Token: 0x040028DA RID: 10458
		public bool UseEuclideanDrumGates = true;

		// Token: 0x040028DB RID: 10459
		public float DrumDelayDuration = 5f;

		// Token: 0x040028DC RID: 10460
		public float DrumAttackDuration;

		// Token: 0x040028DD RID: 10461
		public List<float> EchoDuratios;

		// Token: 0x040028DE RID: 10462
		protected int Seed = -1;

		// Token: 0x040028DF RID: 10463
		public List<int> WeekendTranspositions;

		// Token: 0x040028E0 RID: 10464
		public float WeekendQualityChangeChance;

		// Token: 0x040028E1 RID: 10465
		public float WeekendKeyChangeChance;

		// Token: 0x040028E2 RID: 10466
		public string EasterEggHorn = Rando.Pick<string>(new string[]
		{
			"01",
			"02",
			"03",
			"04",
			"05"
		});

		// Token: 0x040028E3 RID: 10467
		public List<string> GroupPrefices = new List<string>
		{
			"LineLoop_CIRCLE",
			"LineLoop_CIRCLE",
			"LineLoop_CIRCLE",
			"LineLoop_CIRCLE",
			"LineLoop_CIRCLE",
			"LineLoop_CIRCLE"
		};

		// Token: 0x040028E4 RID: 10468
		public List<MusicData.EngineData> GroupEngines = Liszt.From<MusicData.EngineData>(new MusicData.EngineData[]
		{
			new MusicData.EngineData("Three", 0.75f, 1.25f, 0.966051f),
			new MusicData.EngineData("engine-1", 0.33f, 0.75f, 0.384952f),
			new MusicData.EngineData("Four", 1f, 1.75f, 0.881049f),
			new MusicData.EngineData("Orange", 0.5f, 0.75f, 1f),
			new MusicData.EngineData("Shinkansen", 1.25f, 2f, 0.977237f),
			new MusicData.EngineData("Scooter", 0.66f, 1.5f, 0.870964f)
		});

		// Token: 0x040028E5 RID: 10469
		public List<string> Timbres = Liszt.From<string>(new string[]
		{
			"CIRCLE",
			"CROSS",
			"EGG",
			"SQUARE",
			"WEDGE",
			"PENTAGON"
		});

		// Token: 0x040028E6 RID: 10470
		protected float timeAtStart = -1f;

		// Token: 0x020006AE RID: 1710
		public enum RhythmUpdateType
		{
			// Token: 0x040028E8 RID: 10472
			RandomParallel,
			// Token: 0x040028E9 RID: 10473
			LinearParallel,
			// Token: 0x040028EA RID: 10474
			LinearUniform,
			// Token: 0x040028EB RID: 10475
			RandomSingle,
			// Token: 0x040028EC RID: 10476
			RandomAll
		}

		// Token: 0x020006AF RID: 1711
		public enum NoteSequenceType
		{
			// Token: 0x040028EE RID: 10478
			Forward,
			// Token: 0x040028EF RID: 10479
			Backward,
			// Token: 0x040028F0 RID: 10480
			PingPong,
			// Token: 0x040028F1 RID: 10481
			Seeded,
			// Token: 0x040028F2 RID: 10482
			Chaotic,
			// Token: 0x040028F3 RID: 10483
			AutoReroll
		}

		// Token: 0x020006B0 RID: 1712
		public struct EngineData
		{
			// Token: 0x170007FC RID: 2044
			// (get) Token: 0x06002F5F RID: 12127 RVA: 0x000DC7E4 File Offset: 0x000DA9E4
			public string Sample
			{
				get
				{
					if (!(this.Prefix != "engine-1"))
					{
						return "engine-1";
					}
					return "Engine_" + this.Prefix + "_Noise_" + UnityEngine.Random.Range(0, 7).ToString();
				}
			}

			// Token: 0x06002F60 RID: 12128 RVA: 0x000DC82D File Offset: 0x000DAA2D
			public EngineData(string prefix, float pitchMin, float pitchMax, float gain = 1f)
			{
				this.Prefix = prefix;
				this.PitchRange = new Vector2(pitchMin, pitchMax);
				this.Gain = gain;
			}

			// Token: 0x06002F61 RID: 12129 RVA: 0x000DC84B File Offset: 0x000DAA4B
			public override string ToString()
			{
				return string.Format("EngineData[{0}], PitchRange: {1},{2}", this.Prefix, this.PitchRange.x, this.PitchRange.y);
			}

			// Token: 0x040028F4 RID: 10484
			public string Prefix;

			// Token: 0x040028F5 RID: 10485
			public Vector2 PitchRange;

			// Token: 0x040028F6 RID: 10486
			public float Gain;
		}
	}
}
