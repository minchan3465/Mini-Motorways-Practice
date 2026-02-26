using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200052F RID: 1327
	[Serializable]
	public struct NodeConnection
	{
		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x060022FE RID: 8958 RVA: 0x0008ED87 File Offset: 0x0008CF87
		public Vector3 StartPosition
		{
			get
			{
				return this.startNode.transform.position;
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x060022FF RID: 8959 RVA: 0x0008ED99 File Offset: 0x0008CF99
		public Vector3 EndPosition
		{
			get
			{
				return this.endNode.transform.position;
			}
		}

		// Token: 0x04001D11 RID: 7441
		public float duration;

		// Token: 0x04001D12 RID: 7442
		public MenuScreenNode startNode;

		// Token: 0x04001D13 RID: 7443
		public Vector3 entryHandle;

		// Token: 0x04001D14 RID: 7444
		public Vector3 exitHandle;

		// Token: 0x04001D15 RID: 7445
		public MenuScreenNode endNode;

		// Token: 0x04001D16 RID: 7446
		public TransitionCameraControl cameraControl;
	}
}
