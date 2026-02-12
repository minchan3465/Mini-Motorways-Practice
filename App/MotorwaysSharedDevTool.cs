using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using Motorways.Commands;
using UnityEngine;

// Token: 0x02000095 RID: 149
public abstract class MotorwaysSharedDevTool<DevToolType, CommandType> : BaseInGameDevTool<DevToolType, CommandType> where DevToolType : MotorwaysSharedDevTool<DevToolType, CommandType> where CommandType : BaseInGameDevToolCommand<CommandType>
{
	// Token: 0x0600022E RID: 558 RVA: 0x000082B8 File Offset: 0x000064B8
	public void RemoveRoadsAndUpgradesAtTileCoordinate(Vector2Int tileCoordinate)
	{
		List<TileEdit> tileEditsToQueue = new List<TileEdit>();
		TileEditResult trafficLightEditResult = this._tileEditor.ClearTileExplicit(this._tilemapView, tileCoordinate, TileEditor.ClearTileOfType.TrafficLight, Tile.TileChangePermissions.Full);
		if (trafficLightEditResult.IsSuccessful)
		{
			tileEditsToQueue.Add(trafficLightEditResult.edit);
		}
		TileEditResult unbuiltMotorwayEditResult = this._tileEditor.ClearTileExplicit(this._tilemapView, tileCoordinate, TileEditor.ClearTileOfType.UnbuiltMotorway, Tile.TileChangePermissions.Full);
		if (unbuiltMotorwayEditResult.IsSuccessful)
		{
			tileEditsToQueue.Add(unbuiltMotorwayEditResult.edit);
		}
		TileEditResult builtMotorwaysEditResult = this._tileEditor.ClearTileExplicit(this._tilemapView, tileCoordinate, TileEditor.ClearTileOfType.BuiltMotorways, Tile.TileChangePermissions.Full);
		if (builtMotorwaysEditResult.IsSuccessful)
		{
			tileEditsToQueue.Add(builtMotorwaysEditResult.edit);
		}
		TileEditResult roundaboutsEditResult = this._tileEditor.ClearTileExplicit(this._tilemapView, tileCoordinate, TileEditor.ClearTileOfType.Roundabout, Tile.TileChangePermissions.Full);
		if (roundaboutsEditResult.IsSuccessful)
		{
			tileEditsToQueue.Add(roundaboutsEditResult.edit);
		}
		TileEditResult passagesEditResult = this._tileEditor.ClearTileExplicit(this._tilemapView, tileCoordinate, TileEditor.ClearTileOfType.Passages, Tile.TileChangePermissions.Full);
		if (passagesEditResult.IsSuccessful)
		{
			tileEditsToQueue.Add(passagesEditResult.edit);
		}
		TileEditResult roadsEditResult = this._tileEditor.ClearTileExplicit(this._tilemapView, tileCoordinate, TileEditor.ClearTileOfType.Roads, Tile.TileChangePermissions.Full);
		if (roadsEditResult.IsSuccessful)
		{
			tileEditsToQueue.Add(roadsEditResult.edit);
		}
		foreach (TileEdit tileEdit in tileEditsToQueue)
		{
			this.AddTileEdit(tileEdit);
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00008414 File Offset: 0x00006614
	public void AddTileEdit(TileEdit edit)
	{
		if (edit == null)
		{
			return;
		}
		ClientTileEdit clientTileEdit = this._tilemapView.GenerateClientTileEditAndAddEditToViews(edit, false);
		this.ScheduleClientTileEdit(clientTileEdit);
		this._clientUpgradeDatabase.AddTileEdit(clientTileEdit);
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00008448 File Offset: 0x00006648
	public void ScheduleClientTileEdit(ClientTileEdit clientTileEdit)
	{
		EditTileCommand editCommand = EditTileCommand.Create(this.gameScope, clientTileEdit.edit);
		this._simulation.ScheduleCommand(editCommand);
		clientTileEdit.isScheduledOnSimulation = true;
	}

	// Token: 0x040000D1 RID: 209
	[Dependency]
	protected TileEditor _tileEditor;

	// Token: 0x040000D2 RID: 210
	[Dependency]
	protected ClientUpgradeDatabase _clientUpgradeDatabase;
}
