using System;
using Factory;
using Motorways.Commands;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000590 RID: 1424
	[System.Serializable]
	public class CreativeModeEditableDestination : MonoBehaviour, ICreativeModeEditableObject
	{
		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x0600276C RID: 10092 RVA: 0x000A842E File Offset: 0x000A662E
		public bool IsDouble
		{
			get
			{
				return this._isDouble;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x0600276D RID: 10093 RVA: 0x000A8436 File Offset: 0x000A6636
		public bool IsTrainStation
		{
			get
			{
				return this.view.Model.IsTrainStation;
			}
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x0600276E RID: 10094 RVA: 0x000A8448 File Offset: 0x000A6648
		public bool IsBoatTerminal
		{
			get
			{
				return this.view.Model.IsBoatTerminal;
			}
		}

		// Token: 0x0600276F RID: 10095 RVA: 0x000A845A File Offset: 0x000A665A
		public void Initialize(IScope scope, bool isDouble)
		{
			this._scope = scope;
			this._isDouble = isDouble;
		}

		// Token: 0x06002770 RID: 10096 RVA: 0x000A846A File Offset: 0x000A666A
		public Bounds GetBounds()
		{
			return this.view.GetBounds();
		}

		// Token: 0x06002771 RID: 10097 RVA: 0x000A8478 File Offset: 0x000A6678
		public void Delete(bool isReplacement)
		{
			if (!Diagnostics.Verify(this._scope.Get<City>().GameMode == GameMode.Creative, "We shouldn't be deleting destinations out of creative mode!"))
			{
				return;
			}
			if (this.view.Model.Carpark.SupportsTwoDestinations && this.view.Model.Carpark.ActiveDestinationCount > 1)
			{
				this._scope.Get<ISimulation>().ScheduleCommand(RemoveDestinationCommand.Create(this._scope, this.view.Model));
				return;
			}
			this._scope.Get<ISimulation>().ScheduleCommand(RemoveCarparkCommand.Create(this._scope, this.view.Model.Carpark));
		}

		// Token: 0x06002772 RID: 10098 RVA: 0x000A8528 File Offset: 0x000A6728
		public BuildingLayout GetBuildingLayout()
		{
			if (this.view.Model.Carpark.Alignment == TileAlignment.Vertical)
			{
				return BuildingLayout.BuildingToSide;
			}
			return BuildingLayout.BuildingAbove;
		}

		// Token: 0x06002773 RID: 10099 RVA: 0x000020AA File Offset: 0x000002AA
		public bool IsConfirmable()
		{
			return true;
		}

		// Token: 0x06002774 RID: 10100 RVA: 0x000A8545 File Offset: 0x000A6745
		public Vector2 GetWorldPosition()
		{
			return this.view.transform.position;
		}

		// Token: 0x06002775 RID: 10101 RVA: 0x000A855C File Offset: 0x000A675C
		public Vector2Int GetTilePosition()
		{
			return this.view.Model.TileModels[0].Tile.Coordinates;
		}

		// Token: 0x06002776 RID: 10102 RVA: 0x000A857E File Offset: 0x000A677E
		public Vector2 GetCenterForEditMenuPosition()
		{
			return this.GetWorldPosition();
		}

		// Token: 0x06002777 RID: 10103 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CompletelyOutOfPlayArea(City city)
		{
			return false;
		}

		// Token: 0x06002778 RID: 10104 RVA: 0x000A8588 File Offset: 0x000A6788
		public EditMenuButtonType GetEditOptions()
		{
			if (!this.IsBoatTerminal)
			{
				return this.EditOptions;
			}
			CarparkModel carpark = this.view.Model.Carpark;
			if (carpark.SupportsTwoDestinations && carpark.ActiveDestinationCount > 1)
			{
				return this.BoatTerminalEditOptions | EditMenuButtonType.Delete;
			}
			return this.BoatTerminalEditOptions;
		}

		// Token: 0x06002779 RID: 10105 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Confirm()
		{
		}

		// Token: 0x0600277A RID: 10106 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Cancel()
		{
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x000A85D6 File Offset: 0x000A67D6
		public int GetGroupIndex()
		{
			return this.view.Model.GroupIndex;
		}

		// Token: 0x0600277C RID: 10108 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetGroupIndex(int groupIndex, bool isOriginalDeleted)
		{
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x000A85E8 File Offset: 0x000A67E8
		public void Flip(bool isReplacement)
		{
			CreativeModeEditableDestination.Log.Error("Flip called on a CreativeModeEditableDestination, but this should have been diverted to a ghost preview.", Array.Empty<object>());
		}

		// Token: 0x0600277E RID: 10110 RVA: 0x000A85FE File Offset: 0x000A67FE
		public void UpgradeOrDowngrade(bool isReplacement)
		{
			CreativeModeEditableDestination.Log.Error("UpgradeOrDowngrade called on a CreativeModeEditableDestination, but this should have been diverted to a ghost preview.", Array.Empty<object>());
		}

		// Token: 0x0600277F RID: 10111 RVA: 0x000A8614 File Offset: 0x000A6814
		public void Rotate(bool isReplacement)
		{
			CreativeModeEditableDestination.Log.Error("Rotate called on a CreativeModeEditableDestination, but this should have been diverted to a ghost preview.", Array.Empty<object>());
		}

		// Token: 0x06002780 RID: 10112 RVA: 0x000A862C File Offset: 0x000A682C
		public ICreativeModeEditableObject GetGhostPreview(out bool isOriginalDeleted)
		{
			isOriginalDeleted = true;
			DraftDestination draftDestination = this._scope.Get<DraftDestination>();
			draftDestination.InitializeWithExistingView(this._scope, this.view);
			this._scope.Get<ISimulation>().ScheduleCommand(RemoveCarparkCommand.Create(this._scope, this.view.Model.Carpark));
			return draftDestination;
		}

		// Token: 0x04002170 RID: 8560
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("CreativeModeEditableDestination");

		// Token: 0x04002171 RID: 8561
		[SerializeField]
		public DestinationView view;

		// Token: 0x04002172 RID: 8562
		[SerializeField]
		private EditMenuButtonType EditOptions;

		// Token: 0x04002173 RID: 8563
		[SerializeField]
		private EditMenuButtonType BoatTerminalEditOptions;

		// Token: 0x04002174 RID: 8564
		private IScope _scope;

		// Token: 0x04002175 RID: 8565
		private int _groupIndex = -1;

		// Token: 0x04002176 RID: 8566
		private TileDirection drivewayDirection;

		// Token: 0x04002177 RID: 8567
		private bool _isDouble;
	}
}
