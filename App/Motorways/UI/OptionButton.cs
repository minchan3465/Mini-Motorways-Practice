using System;
using UnityEngine;
using UnityEngine.Events;

namespace Motorways.UI
{
	// Token: 0x0200073A RID: 1850
	public abstract class OptionButton : MonoBehaviour
	{
		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x060033AF RID: 13231 RVA: 0x000F484E File Offset: 0x000F2A4E
		public int SelectedOptionIndex
		{
			get
			{
				return this._currentIndex;
			}
		}

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x060033B0 RID: 13232 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual int NumberOfOptions
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060033B1 RID: 13233 RVA: 0x000F4856 File Offset: 0x000F2A56
		public virtual void SetOption(int index)
		{
			this.SetOption(index, true);
		}

		// Token: 0x060033B2 RID: 13234 RVA: 0x000F4860 File Offset: 0x000F2A60
		public virtual void SetOption(int index, bool invokeMethod)
		{
			int clampedIndex = Mathf.Clamp(index, 0, this.NumberOfOptions - 1);
			this._currentIndex = clampedIndex;
			if (Diagnostics.Verify(this._currentIndex >= 0 && this._currentIndex < this.NumberOfOptions, "Options index {0} isn't valid. Must be between 0 and {1}", this._currentIndex, this.NumberOfOptions) && invokeMethod)
			{
				this.onOptionChanged.Invoke(this._currentIndex);
			}
		}

		// Token: 0x04002C2A RID: 11306
		public OptionButton.OptionEvent onOptionChanged;

		// Token: 0x04002C2B RID: 11307
		public bool wrap;

		// Token: 0x04002C2C RID: 11308
		protected int _currentIndex = -1;

		// Token: 0x0200073B RID: 1851
		[Serializable]
		public class OptionEvent : UnityEvent<int>
		{
		}
	}
}
