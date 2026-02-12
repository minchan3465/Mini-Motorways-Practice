using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
public interface IGameCenterAccessPoint
{
	// Token: 0x06000421 RID: 1057
	bool IsAvailable();

	// Token: 0x06000422 RID: 1058
	void Show();

	// Token: 0x06000423 RID: 1059
	void Hide();

	// Token: 0x06000424 RID: 1060
	Rect GetRect();

	// Token: 0x06000425 RID: 1061
	void Select();
}
