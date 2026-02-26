using System;
using System.Collections.Generic;
using System.Linq;

namespace Motorways.Audio
{
	// Token: 0x0200066D RID: 1645
	public class Scale
	{
		// Token: 0x06002DB1 RID: 11697 RVA: 0x000D4310 File Offset: 0x000D2510
		public string FullName()
		{
			return Note.SCALE[this.Key] + " " + this.Name;
		}

		// Token: 0x06002DB2 RID: 11698 RVA: 0x000D4332 File Offset: 0x000D2532
		public Scale(int key, string name, List<int> intervals, List<int> baseStack = null)
		{
			this.Init(key, name, intervals, baseStack);
		}

		// Token: 0x06002DB3 RID: 11699 RVA: 0x000D434C File Offset: 0x000D254C
		private void Init(int key, string name, List<int> intervals, List<int> baseStack = null)
		{
			this.Key = key;
			this.Name = name;
			this.Intervals = intervals;
			this.BaseStack = (baseStack ?? Liszt.From<int>(new int[]
			{
				12
			}));
			int i_sum = 0;
			int i_i = 0;
			while (i_sum < this.BaseStack.Last<int>())
			{
				i_sum += this.Intervals.SafeGet(i_i++);
			}
			this.FullStack = this.BaseStack.ToList<int>();
			if (name.Contains("SUHMM"))
			{
				i_i = 0;
			}
			int preTransposeCeiling = Note.RANGE.Count - 11;
			while (this.FullStack.Last<int>() + this.Intervals.SafeGet(i_i) < preTransposeCeiling)
			{
				this.FullStack.Add(this.FullStack.Last<int>() + this.Intervals.SafeGet(i_i++));
			}
			this.Notes = Note.Transpose(key, Quality.IntervalsToNotes(this.FullStack));
		}

		// Token: 0x06002DB4 RID: 11700 RVA: 0x000D443A File Offset: 0x000D263A
		public void Restack(List<int> baseStack = null)
		{
			this.Init(this.Key, this.Name, this.Intervals, baseStack);
		}

		// Token: 0x06002DB5 RID: 11701 RVA: 0x000D4458 File Offset: 0x000D2658
		public Scale Rotate(int posDelta, string newName = "")
		{
			List<int> intervals = this.Intervals.Rotate(posDelta);
			int intervalDelta = 0;
			int p = 0;
			while (p < posDelta)
			{
				intervalDelta += this.Intervals[p++];
			}
			return new Scale(Maf.FloorMod(this.Key + intervalDelta, 12), (newName.Length > 0) ? newName : (this.Name + " " + (p + 1).ToString()), intervals, this.BaseStack);
		}

		// Token: 0x06002DB6 RID: 11702 RVA: 0x000D44D1 File Offset: 0x000D26D1
		public Scale Transpose(int keyDelta)
		{
			return new Scale(Maf.FloorMod(this.Key + keyDelta, 12), this.Name, this.Intervals, this.BaseStack);
		}

		// Token: 0x06002DB7 RID: 11703 RVA: 0x000D44FC File Offset: 0x000D26FC
		public override string ToString()
		{
			return string.Format("{0} {1} : {2}\nStack: {3}\n", new object[]
			{
				Note.SCALE[this.Key],
				this.Name,
				string.Join(", ", this.Notes),
				string.Join<int>(", ", this.FullStack)
			});
		}

		// Token: 0x040027C2 RID: 10178
		public int Key;

		// Token: 0x040027C3 RID: 10179
		public string Name;

		// Token: 0x040027C4 RID: 10180
		public List<int> Intervals;

		// Token: 0x040027C5 RID: 10181
		public List<int> BaseStack;

		// Token: 0x040027C6 RID: 10182
		public List<int> FullStack;

		// Token: 0x040027C7 RID: 10183
		public List<string> Notes;

		// Token: 0x040027C8 RID: 10184
		public bool IsOriginal = true;

		// Token: 0x0200066E RID: 1646
		public struct Data
		{
			// Token: 0x06002DB8 RID: 11704 RVA: 0x000D455B File Offset: 0x000D275B
			public Data(string name, params int[] stack)
			{
				this.Name = name;
				this.Stack = (((stack != null) ? stack.ToList<int>() : null) ?? null);
			}

			// Token: 0x040027C9 RID: 10185
			public string Name;

			// Token: 0x040027CA RID: 10186
			public List<int> Stack;
		}
	}
}
