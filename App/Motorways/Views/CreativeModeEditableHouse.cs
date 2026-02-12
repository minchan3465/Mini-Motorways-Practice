using System;
using Factory;
using Motorways.Commands;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000591 RID: 1425
	[System.Serializable]
	public class CreativeModeEditableHouse : MonoBehaviour, ICreativeModeEditableObject
	{
		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06002783 RID: 10115 RVA: 0x000A86A5 File Offset: 0x000A68A5
		public int GroupIndex
		{
			get
			{
				return this._groupIndex;
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06002784 RID: 10116 RVA: 0x000A86AD File Offset: 0x000A68AD
		public TileDirection DrivewayDirection
		{
			get
			{
				return this._drivewayDirection;
			}
		}

		// Token: 0x06002785 RID: 10117 RVA: 0x000A86B5 File Offset: 0x000A68B5
		public void Initialize(IScope scope, int groupIndex, TileDirection drivewayDirection)
		{
			this._scope = scope;
			this._groupIndex = groupIndex;
			this._drivewayDirection = drivewayDirection;
		}

		// Token: 0x06002786 RID: 10118 RVA: 0x000A86CC File Offset: 0x000A68CC
		public Bounds GetBounds()
		{
			return this.view.GetBounds();
		}

		// Token: 0x06002787 RID: 10119 RVA: 0x000A86D9 File Offset: 0x000A68D9
		public void Delete(bool isReplacement)
		{
			this._scope.Get<ISimulation>().ScheduleCommand(RemoveHouseCommand.Create(this._scope, this.view.Model));
		}

		// Token: 0x06002788 RID: 10120 RVA: 0x000020AA File Offset: 0x000002AA
		public bool IsConfirmable()
		{
			return true;
		}

		// Token: 0x06002789 RID: 10121 RVA: 0x0000222C File Offset: 0x0000042C
		public BuildingLayout GetBuildingLayout()
		{
			return BuildingLayout.BuildingAbove;
		}

		// Token: 0x0600278A RID: 10122 RVA: 0x000A8702 File Offset: 0x000A6902
		public Vector2 GetWorldPosition()
		{
			return this.view.transform.position;
		}

		// Token: 0x0600278B RID: 10123 RVA: 0x000A8719 File Offset: 0x000A6919
		public Vector2Int GetTilePosition()
		{
			return this.view.tilePosition;
		}

		// Token: 0x0600278C RID: 10124 RVA: 0x000A8726 File Offset: 0x000A6926
		public Vector2 GetCenterForEditMenuPosition()
		{
			return this.GetWorldPosition();
		}

		// Token: 0x0600278D RID: 10125 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CompletelyOutOfPlayArea(City city)
		{
			return false;
		}

		// Token: 0x0600278E RID: 10126 RVA: 0x000A872E File Offset: 0x000A692E
		public EditMenuButtonType GetEditOptions()
		{
			return this._editOptions;
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Confirm()
		{
		}

		// Token: 0x06002790 RID: 10128 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Cancel()
		{
		}

		// Token: 0x06002791 RID: 10129 RVA: 0x00015E3F File Offset: 0x0001403F
		public int GetGroupIndex()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002792 RID: 10130 RVA: 0x00015E3F File Offset: 0x0001403F
		public void SetGroupIndex(int groupIndex, bool isReplacement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002793 RID: 10131 RVA: 0x000A8736 File Offset: 0x000A6936
		public ICreativeModeEditableObject GetGhostPreview(out bool isOriginalDeleted)
		{
			isOriginalDeleted = true;
			DraftHouse draftHouse = this._scope.Get<DraftHouse>();
			draftHouse.InitializeWithExistingView(this._scope, this.view);
			this.Delete(false);
			return draftHouse;
		}

		// Token: 0x06002794 RID: 10132 RVA: 0x000A875F File Offset: 0x000A695F
		public void Flip(bool isReplacement)
		{
			Diagnostics.FailAssert("Flip called on a DraftHouse, but only makes sense on Single Destinations!", Array.Empty<object>());
		}

		// Token: 0x06002795 RID: 10133 RVA: 0x000A8770 File Offset: 0x000A6970
		public void Rotate(bool isReplacement)
		{
			Diagnostics.FailAssert("Rotate called on a DraftHouse, but only makes sense on Destinations!", Array.Empty<object>());
		}

		// Token: 0x06002796 RID: 10134 RVA: 0x000A8781 File Offset: 0x000A6981
		public void UpgradeOrDowngrade(bool isReplacement)
		{
			CreativeModeEditableHouse.Log.Error("UpgradeOrDowngrade called on a DraftHouse, but only makes sense on Destinations!", Array.Empty<object>());
		}

		// Token: 0x04002178 RID: 8568
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("CreativeModeEditableHouse");

		// Token: 0x04002179 RID: 8569
		[SerializeField]
		public HouseView view;

		// Token: 0x0400217A RID: 8570
		[SerializeField]
		private EditMenuButtonType _editOptions;

		// Token: 0x0400217B RID: 8571
		private IScope _scope;

		// Token: 0x0400217C RID: 8572
		private int _groupIndex = -1;

		// Token: 0x0400217D RID: 8573
		private TileDirection _drivewayDirection;
	}
}
