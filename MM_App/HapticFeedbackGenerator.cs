using System;
using Factory;

// Token: 0x0200025E RID: 606
public class HapticFeedbackGenerator
{
	// Token: 0x06000E60 RID: 3680 RVA: 0x00030B8D File Offset: 0x0002ED8D
	public void GenerateFeedback(HapticFeedbackType feedbackType)
	{
		if (!this._player.HasActivePlayer || !this._player.IsVibrationEnabled)
		{
			return;
		}
		this._hardwareCapabilities.GenerateHapticFeedback(feedbackType);
	}

	// Token: 0x0400087F RID: 2175
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x04000880 RID: 2176
	[Dependency]
	private IActivePlayer _player;
}
