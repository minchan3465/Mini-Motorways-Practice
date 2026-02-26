using System;
using Factory;

namespace Popups
{
	// Token: 0x020002D0 RID: 720
	public abstract class AbstractConfirmationPopup : BasePopup
	{
		// Token: 0x060011B5 RID: 4533
		public abstract void Initialise(IScope scope, StringId mainPromptStringId, Action onNoPressed, Action onYesPressed, StringId additionalInfoStringId = StringId.None);

		// Token: 0x060011B6 RID: 4534
		public abstract void Initialise(IScope scope, StringId mainPromptStringId, Action onClosed, StringId additionalInfoStringId = StringId.None);
	}
}
