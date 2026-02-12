using System;
using System.Collections.Generic;
using Motorways.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace Motorways.UI
{
	// Token: 0x02000741 RID: 1857
	public class SymbolOptionButton : OptionButton
	{
		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060033E1 RID: 13281 RVA: 0x000F54DC File Offset: 0x000F36DC
		public override int NumberOfOptions
		{
			get
			{
				return this.optionCount;
			}
		}

		// Token: 0x060033E2 RID: 13282 RVA: 0x000F54E4 File Offset: 0x000F36E4
		private void Awake()
		{
			NumberBubble bubble = this._bubble;
			if (bubble != null)
			{
				bubble.Hide(true);
			}
			this.SetOption(0, true, false);
		}

		// Token: 0x060033E3 RID: 13283 RVA: 0x000F5504 File Offset: 0x000F3704
		public void NextOption()
		{
			int iterations = 0;
			do
			{
				this._currentIndex++;
				iterations++;
				if (this._currentIndex >= this.NumberOfOptions)
				{
					this._currentIndex = (this.wrap ? 0 : (this.NumberOfOptions - 1));
				}
			}
			while (this._blockedOptions.Contains(this._currentIndex) && iterations <= this.NumberOfOptions);
			this.SetOption(this._currentIndex);
			AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateUIEvent((this._currentIndex == 0) ? UIEventType.CheckboxUnchecked : UIEventType.CheckboxChecked, UIAudioProfile.None, -1f, true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
		}

		// Token: 0x060033E4 RID: 13284 RVA: 0x000F559B File Offset: 0x000F379B
		public void SetToTriggerOption(bool invokeMethod)
		{
			this.SetOption(this.triggerOnOption, invokeMethod);
		}

		// Token: 0x060033E5 RID: 13285 RVA: 0x000F55AA File Offset: 0x000F37AA
		public void SkipOption(int optionIndex)
		{
			if (!this._blockedOptions.Contains(optionIndex))
			{
				this._blockedOptions.Add(optionIndex);
			}
		}

		// Token: 0x060033E6 RID: 13286 RVA: 0x000F55C6 File Offset: 0x000F37C6
		public void UnskipOption(int optionIndex)
		{
			if (this._blockedOptions.Contains(optionIndex))
			{
				this._blockedOptions.Remove(optionIndex);
			}
		}

		// Token: 0x060033E7 RID: 13287 RVA: 0x000F55E4 File Offset: 0x000F37E4
		public void SetOption(int index, bool invokeMethod, bool invokeTriggerMethod)
		{
			base.SetOption(index, invokeMethod);
			if (invokeTriggerMethod)
			{
				if (this._currentIndex == this.triggerOnOption)
				{
					this.onOptionTriggered.Invoke(this.triggerValue);
				}
				else
				{
					this.onOptionTriggered.Invoke(!this.triggerValue);
				}
			}
			if (this._bubble != null)
			{
				if (this._currentIndex == 0)
				{
					this._bubble.Hide(false);
					return;
				}
				this._bubble.SetValue(this.GetVisibleIndex(), true);
			}
		}

		// Token: 0x060033E8 RID: 13288 RVA: 0x000F5668 File Offset: 0x000F3868
		private int GetVisibleIndex()
		{
			int visibleCount = 0;
			for (int optionIndex = 0; optionIndex < this._currentIndex; optionIndex++)
			{
				if (!this._blockedOptions.Contains(optionIndex))
				{
					visibleCount++;
				}
			}
			return visibleCount;
		}

		// Token: 0x060033E9 RID: 13289 RVA: 0x000F569B File Offset: 0x000F389B
		public override void SetOption(int index, bool invokeMethod)
		{
			this.SetOption(index, invokeMethod, invokeMethod);
		}

		// Token: 0x04002C59 RID: 11353
		public int optionCount;

		// Token: 0x04002C5A RID: 11354
		[Tooltip("On what index should the method trigger?")]
		public int triggerOnOption;

		// Token: 0x04002C5B RID: 11355
		[Tooltip("The value to send when OnOptionTrigger is invoked")]
		public bool triggerValue;

		// Token: 0x04002C5C RID: 11356
		[SerializeField]
		private NumberBubble _bubble;

		// Token: 0x04002C5D RID: 11357
		public SymbolOptionButton.OptionTrigger onOptionTriggered = new SymbolOptionButton.OptionTrigger();

		// Token: 0x04002C5E RID: 11358
		[SerializeField]
		private List<int> _blockedOptions = new List<int>();

		// Token: 0x02000742 RID: 1858
		[Serializable]
		public class OptionTrigger : UnityEvent<bool>
		{
		}
	}
}
