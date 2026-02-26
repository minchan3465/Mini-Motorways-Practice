using System;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005C7 RID: 1479
	public interface ICreativeModeEditableObject
	{
		// Token: 0x0600296A RID: 10602
		Bounds GetBounds();

		// Token: 0x0600296B RID: 10603
		bool IsConfirmable();

		// Token: 0x0600296C RID: 10604
		BuildingLayout GetBuildingLayout();

		// Token: 0x0600296D RID: 10605
		void Delete(bool isReplacement);

		// Token: 0x0600296E RID: 10606
		Vector2 GetWorldPosition();

		// Token: 0x0600296F RID: 10607
		Vector2Int GetTilePosition();

		// Token: 0x06002970 RID: 10608
		Vector2 GetCenterForEditMenuPosition();

		// Token: 0x06002971 RID: 10609
		bool CompletelyOutOfPlayArea(City city);

		// Token: 0x06002972 RID: 10610
		EditMenuButtonType GetEditOptions();

		// Token: 0x06002973 RID: 10611
		void Confirm();

		// Token: 0x06002974 RID: 10612
		void Cancel();

		// Token: 0x06002975 RID: 10613
		void SetGroupIndex(int groupIndex, bool isReplacement);

		// Token: 0x06002976 RID: 10614
		int GetGroupIndex();

		// Token: 0x06002977 RID: 10615
		ICreativeModeEditableObject GetGhostPreview(out bool isOriginalDeleted);

		// Token: 0x06002978 RID: 10616
		void Flip(bool isReplacement);

		// Token: 0x06002979 RID: 10617
		void UpgradeOrDowngrade(bool isReplacement);

		// Token: 0x0600297A RID: 10618
		void Rotate(bool isReplacement);
	}
}
