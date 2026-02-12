using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200066F RID: 1647
	public class Quality
	{
		// Token: 0x06002DB9 RID: 11705 RVA: 0x000D457B File Offset: 0x000D277B
		public string FullName()
		{
			return Note.SCALE[Get.Loadout.MusicData.CurrentKey] + " " + this.Name;
		}

		// Token: 0x06002DBA RID: 11706 RVA: 0x000D45A8 File Offset: 0x000D27A8
		public Quality(string name, List<int> intervals, List<int> baseStack = null)
		{
			this.BaseScale = new Scale(0, name, intervals, baseStack);
			this.Scales.Add(this.BaseScale);
			this.Name = name;
			this.Intervals = intervals;
			this.BaseStack = this.BaseScale.BaseStack;
			this.FullStack = this.BaseScale.FullStack;
		}

		// Token: 0x06002DBB RID: 11707 RVA: 0x000D4618 File Offset: 0x000D2818
		public static Quality Clone(Quality q, string newName = "")
		{
			if (q == null)
			{
				return null;
			}
			return new Quality((newName.Length > 0) ? newName : q.Name, q.Intervals, q.BaseStack)
			{
				Scales = q.Scales.ToList<Scale>(),
				IsKeyless = q.IsKeyless
			};
		}

		// Token: 0x06002DBC RID: 11708 RVA: 0x000D466C File Offset: 0x000D286C
		public Quality Modal(params string[] modeNames)
		{
			Quality q = Quality.Clone(this, "");
			q.IsKeyless = true;
			q.Scales[0].Name = ((modeNames.Length != 0) ? modeNames[0] : q.Scales[0].Name);
			for (int i = 1; i < q.Intervals.Count; i++)
			{
				q.Scales.Add(q.Scales[q.Scales.Count - 1].Rotate(1, (i < modeNames.Length) ? modeNames[i] : (q.Name + " " + (i + 1).ToString())));
			}
			return q;
		}

		// Token: 0x06002DBD RID: 11709 RVA: 0x000D4720 File Offset: 0x000D2920
		public Quality ModalVerbose(params Scale.Data[] modes)
		{
			Quality q = Quality.Clone(this, "");
			q.IsKeyless = true;
			for (int i = 0; i < q.Intervals.Count; i++)
			{
				if (i > 0)
				{
					q.Scales.Add(q.Scales[q.Scales.Count - 1].Rotate(1, (i < modes.Length) ? modes[i].Name : (q.Name + " " + (i + 1).ToString())));
				}
				else
				{
					q.Scales[i].Name = ((modes.Length != 0) ? modes[i].Name : q.Scales[i].Name);
				}
				q.Scales.Last<Scale>().Restack(q.BaseStack.Union(modes[i].Stack).ToList<int>());
			}
			return q;
		}

		// Token: 0x06002DBE RID: 11710 RVA: 0x000D481C File Offset: 0x000D2A1C
		public Quality Transpose(int delta)
		{
			Quality quality = Quality.Clone(this, "");
			quality.Scales.Edit(delegate(Scale x)
			{
				x.IsOriginal = false;
				return x.Transpose(delta);
			});
			return quality;
		}

		// Token: 0x06002DBF RID: 11711 RVA: 0x000D485C File Offset: 0x000D2A5C
		public Quality Chromatic(string addendName = "")
		{
			Quality q = Quality.Clone(this, "");
			Quality quality = q;
			quality.Name += ((addendName.Length > 0) ? (" " + addendName) : "");
			foreach (Scale scale in q.Scales.ToList<Scale>())
			{
				for (int i = 1; i < 12; i++)
				{
					Scale transposedScale = scale.Transpose(i);
					q.Scales.Add(transposedScale);
					transposedScale.IsOriginal = false;
				}
			}
			return q;
		}

		// Token: 0x06002DC0 RID: 11712 RVA: 0x000D4914 File Offset: 0x000D2B14
		public Quality Chromodal(params string[] modeNames)
		{
			return this.Modal(modeNames).Chromatic("");
		}

		// Token: 0x06002DC1 RID: 11713 RVA: 0x000D4928 File Offset: 0x000D2B28
		public Quality Keyless()
		{
			Quality q = Quality.Clone(this, "");
			for (int i = q.Scales.Count - 1; i > 0; i--)
			{
				if (!q.Scales[i].IsOriginal)
				{
					q.Scales.Remove(q.Scales[i]);
				}
			}
			q.Scales.Edit((Scale x) => x.Transpose(-x.Key));
			q.IsKeyless = true;
			return q;
		}

		// Token: 0x06002DC2 RID: 11714 RVA: 0x000D49B8 File Offset: 0x000D2BB8
		public static Quality GetMode(Quality q, int modeIndex, string newName = "")
		{
			if (modeIndex > q.Scales.Count - 1)
			{
				Dbug.Log.Error("Mode Index {0} requested from Quality {1} is out of range. Quality only has {2} scales.", new object[]
				{
					modeIndex,
					q.Name,
					(q.Scales.Count > 0) ? q.Scales.Count : 0
				});
			}
			return new Quality((newName.Length > 0) ? newName : q.Scales[modeIndex].Name, q.Scales[modeIndex].Intervals, q.Scales[modeIndex].BaseStack).Chromatic("");
		}

		// Token: 0x06002DC3 RID: 11715 RVA: 0x000D4A6E File Offset: 0x000D2C6E
		public Quality GetMode(int modeIndex, string newName = "")
		{
			return Quality.GetMode(this, modeIndex, newName);
		}

		// Token: 0x06002DC4 RID: 11716 RVA: 0x000D4A78 File Offset: 0x000D2C78
		public List<Quality> ToModes()
		{
			return Liszt.Make<Quality>(this.Scales.Count, (int i) => new Quality(this.Scales[i].Name, this.Scales[i].Intervals, this.Scales[i].BaseStack).Chromatic(""));
		}

		// Token: 0x06002DC5 RID: 11717 RVA: 0x000D4A96 File Offset: 0x000D2C96
		public Quality SetName(string name)
		{
			this.Name = name;
			return this;
		}

		// Token: 0x06002DC6 RID: 11718 RVA: 0x000D4AA0 File Offset: 0x000D2CA0
		public List<string> CommonToneChord(List<string> currentChord, int commonTones, int newSize, ref Scale scale, ref int iterations)
		{
			if (!Diagnostics.Verify(this.Scales.Count > 0))
			{
				Dbug.Log.Error("Quality {0} has no scales !", new object[]
				{
					this.Name
				});
			}
			List<Scale> scales = this.Scales;
			if (this.IsKeyless)
			{
				scales = this.Scales.ToList<Scale>();
				scales.Edit((Scale x) => x.Transpose(Get.Loadout.MusicData.CurrentKey));
			}
			List<string> newTones = new List<string>();
			List<int> ii = Rando.Numbers(scales.Count, 0);
			Func<string, bool> <>9__1;
			for (int i = 0; i < scales.Count; i++)
			{
				iterations++;
				Dbug.Log.Info("Iteration {0}.", new object[]
				{
					iterations
				});
				if (i > 11)
				{
					Dbug.Log.Info("Traversed 12 of the {0} scales in this quality with no success. exiting early...", new object[]
					{
						scales.Count
					});
					break;
				}
				if (iterations > 100)
				{
					Dbug.Log.Info("Taking too long ... exiting early.", Array.Empty<object>());
					break;
				}
				int shuffled_i = ii[i];
				string keyName = Note.SCALE[scales[shuffled_i].Key];
				IEnumerable<string> notes = scales[shuffled_i].Notes;
				Func<string, bool> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((string x) => currentChord.Contains(x)));
				}
				int ctc = notes.Count(predicate);
				if (ctc < commonTones)
				{
					Dbug.Log.Info("{0} > {1} : only {2} commonTones. continuing...", new object[]
					{
						this.Name,
						scales[shuffled_i].FullName(),
						ctc
					});
				}
				else
				{
					newTones = Liszt.Make<string>(newSize, (int x) => "");
					List<string> newCommonTones = new List<string>();
					List<string> newNonCommonTones = new List<string>();
					bool success = false;
					Dbug.Log.Info("CommonToneChord(), requesting {0} commonTones and {1} non common tones from {2} {3}", new object[]
					{
						commonTones,
						newSize - commonTones,
						keyName,
						this.Name
					});
					int notesLeft = scales[shuffled_i].Notes.Count;
					int nonCommonTones = newSize - commonTones;
					foreach (string note in scales[shuffled_i].Notes)
					{
						bool isCommonTone = currentChord.Contains(note);
						if (newCommonTones.Count < commonTones && isCommonTone)
						{
							newCommonTones.Add(note);
							int note_i = currentChord.IndexOf(note);
							newTones[note_i] = note;
							Dbug.Log.Info("Adding Common Tone {0}.", new object[]
							{
								note
							});
						}
						else if (newNonCommonTones.Count < nonCommonTones && !isCommonTone)
						{
							Dbug.Log.Info("Adding Non-Common Tone {0}.", new object[]
							{
								note
							});
							newNonCommonTones.Add(note);
						}
						notesLeft--;
						bool flag = notesLeft < newSize - (newCommonTones.Count + newNonCommonTones.Count);
						success = (newCommonTones.Count + newNonCommonTones.Count >= newSize);
						if (flag || success)
						{
							Dbug.Log.Info("Size fulfilled, or not enough notes left to succeed. Breaking early ...", Array.Empty<object>());
							break;
						}
					}
					if (success)
					{
						Dbug.Log.Info("Success! {2} {3} has {0} common tones and {1} non-common tones.", new object[]
						{
							newCommonTones.Count,
							newNonCommonTones.Count,
							keyName,
							this.Name
						});
						newTones.Edit(delegate(string x)
						{
							if (x == "")
							{
								int n_i = Rando.Index<string>(newNonCommonTones, -1);
								string result = newNonCommonTones[n_i];
								newNonCommonTones.RemoveAt(n_i);
								return result;
							}
							return x;
						});
						scale = scales[shuffled_i];
						break;
					}
					Dbug.Log.Info("{2} {3} only has {0} common tones, and {1} non-common tones. Continuing ...", new object[]
					{
						newCommonTones.Count,
						newNonCommonTones.Count,
						keyName,
						this.Name
					});
					newTones.Clear();
				}
			}
			return newTones;
		}

		// Token: 0x06002DC7 RID: 11719 RVA: 0x000D4F00 File Offset: 0x000D3100
		public static List<string> IntervalsToNotes(List<int> intervals)
		{
			return Liszt.Make<string>(Mathf.Min(intervals.Count, Note.RANGE.Count), (int i) => Note.RANGE[intervals[i]]);
		}

		// Token: 0x06002DC8 RID: 11720 RVA: 0x000D4F48 File Offset: 0x000D3148
		public static List<int> NotesToIntervals(List<string> notes)
		{
			return Liszt.Make<int>(notes.Count, (int i) => Note.RANGE.IndexOf(notes[i]));
		}

		// Token: 0x06002DC9 RID: 11721 RVA: 0x000D4F7E File Offset: 0x000D317E
		public List<string> Notes(string key, int size, out Scale newScale)
		{
			return this.Notes(Note.SCALE.IndexOf(key), size, out newScale);
		}

		// Token: 0x06002DCA RID: 11722 RVA: 0x000D4F93 File Offset: 0x000D3193
		public List<string> Notes(int key, int size, out Scale newScale)
		{
			newScale = (this.IsKeyless ? Rando.Pick<Scale>(this.Scales).Transpose(key) : this.Scales[key]);
			return newScale.Notes.ToList<string>().Whittle(size, -1);
		}

		// Token: 0x06002DCB RID: 11723 RVA: 0x000D4FD4 File Offset: 0x000D31D4
		public override string ToString()
		{
			return string.Format("Quality: {0}\nIntervals: {1}\nBaseStack: {2}\nFull Stack: {3}\nScales:\n{4}", new object[]
			{
				this.Name,
				string.Join<int>(", ", this.Intervals),
				(this.BaseStack != null) ? string.Join<int>(", ", this.BaseStack) : "null",
				string.Join<int>(", ", this.FullStack),
				string.Join<Scale>("\n", this.Scales)
			});
		}

		// Token: 0x040027CB RID: 10187
		public string Name;

		// Token: 0x040027CC RID: 10188
		public List<int> Intervals;

		// Token: 0x040027CD RID: 10189
		public List<int> BaseStack;

		// Token: 0x040027CE RID: 10190
		public List<int> FullStack;

		// Token: 0x040027CF RID: 10191
		public Scale BaseScale;

		// Token: 0x040027D0 RID: 10192
		public List<Scale> Scales = new List<Scale>();

		// Token: 0x040027D1 RID: 10193
		public bool IsKeyless;
	}
}
