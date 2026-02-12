using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200059B RID: 1435
	public class DestinationVisualVariant : MonoBehaviour
	{
		// Token: 0x040021D9 RID: 8665
		public DestinationLevel Level0Square;

		// Token: 0x040021DA RID: 8666
		public DestinationLevel Level0StationHorizontal;

		// Token: 0x040021DB RID: 8667
		public DestinationLevel Level0StationVertical;

		// Token: 0x040021DC RID: 8668
		public DestinationLevel Level1;

		// Token: 0x040021DD RID: 8669
		public DestinationPinAnimatorView PinAnimator;

		// Token: 0x040021DE RID: 8670
		public DestinationPinAnimatorView StationPinAnimator;

		// Token: 0x040021DF RID: 8671
		public DestinationPinAnimatorView StationPinAnimatorVertical;

		// Token: 0x040021E0 RID: 8672
		public ParticleSystem DisconnectedParticles_Square;

		// Token: 0x040021E1 RID: 8673
		public ParticleSystem DisconnectedParticles_Circle;

		// Token: 0x040021E2 RID: 8674
		public ParticleSystem DisconnectedParticles_TrainStation;
	}
}
