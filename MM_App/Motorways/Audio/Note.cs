using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200066A RID: 1642
	public static class Note
	{
		// Token: 0x06002DA6 RID: 11686 RVA: 0x000D2AD8 File Offset: 0x000D0CD8
		public static float GainFactor(string note)
		{
			float alpha = (float)Note.RANGE.IndexOf(note) / (float)Note.RANGE.Count;
			return Mathf.Lerp(Settings.Gain.KEYBOARD.x, Settings.Gain.KEYBOARD.y, Twerp.Ease.Out(alpha, 2));
		}

		// Token: 0x06002DA7 RID: 11687 RVA: 0x000D2B20 File Offset: 0x000D0D20
		public static List<string> TransposeRoot(int intervalDelta, params string[] notes)
		{
			List<string> newNotes = new List<string>();
			foreach (string note in notes)
			{
				newNotes.Add(Note.SCALE.SafeGet(Note.SCALE.IndexOf(note) + intervalDelta));
			}
			return newNotes;
		}

		// Token: 0x06002DA8 RID: 11688 RVA: 0x000D2B68 File Offset: 0x000D0D68
		public static List<string> Transpose(int intervalDelta, List<string> notes)
		{
			if (intervalDelta == 0)
			{
				return notes;
			}
			List<string> newNotes = new List<string>();
			foreach (string note in notes)
			{
				newNotes.Add(Note.Transpose(intervalDelta, note));
			}
			return newNotes;
		}

		// Token: 0x06002DA9 RID: 11689 RVA: 0x000D2BC8 File Offset: 0x000D0DC8
		public static string Transpose(int intervalDelta, string note)
		{
			if (intervalDelta == 0)
			{
				return note;
			}
			int newNote_i = Note.RANGE.IndexOf(note) + intervalDelta;
			if (newNote_i < 0)
			{
				int tooLow = newNote_i;
				newNote_i = Maf.FloorMod(newNote_i, 12);
				AudioSystem.Log.Warn("Requested transposition of {2} at index {0} was too low. Replaced with {1}", new object[]
				{
					tooLow,
					Note.RANGE[newNote_i],
					intervalDelta
				});
			}
			if (newNote_i > Note.RANGE.Count - 1)
			{
				int tooHigh = newNote_i;
				newNote_i = Note.RANGE.Count - 12 + Maf.FloorMod(newNote_i - Note.RANGE.Count, 12);
				AudioSystem.Log.Warn("Requested transposition of {2} at index {0} was too high. Replaced with {1}", new object[]
				{
					tooHigh,
					Note.RANGE[newNote_i],
					intervalDelta
				});
			}
			return Note.RANGE[newNote_i];
		}

		// Token: 0x06002DAA RID: 11690 RVA: 0x000D2CA0 File Offset: 0x000D0EA0
		private static List<string> GetFullRange()
		{
			List<string> noteRange = new List<string>();
			for (int o = 2; o <= 5; o++)
			{
				for (int i = 0; i < Note.SCALE.Count; i++)
				{
					noteRange.Add(Note.SCALE[i] + o.ToString());
				}
			}
			return noteRange;
		}

		// Token: 0x040027A3 RID: 10147
		public static readonly List<string> SCALE = Liszt.From<string>(new string[]
		{
			"C",
			"C#",
			"D",
			"D#",
			"E",
			"F",
			"F#",
			"G",
			"G#",
			"A",
			"A#",
			"B"
		});

		// Token: 0x040027A4 RID: 10148
		public static readonly List<string> RANGE = Note.GetFullRange();
	}
}
