using System;
using System.Collections.Generic;
using System.Reflection;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Themes;
using Motorways.Views;
using Server;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class InGameDevToolsRegistry : IInGameDevToolsRegistry, IReusable, IReleasedFromScopeHandler
{
	// Token: 0x06000259 RID: 601 RVA: 0x00008493 File Offset: 0x00006693
	public List<IInGameDevTool> GetAllGenericDevTools()
	{
		if (this._allTools.Count == 0)
		{
			this.RegisterTools();
		}
		return this._allTools;
	}

	// Token: 0x0600025A RID: 602 RVA: 0x000084AE File Offset: 0x000066AE
	public List<ToolModelType> GetModelDevToolTypes()
	{
		if (this._allTools.Count == 0)
		{
			this.RegisterTools();
		}
		return new List<ToolModelType>(this._modelContainerTools.Keys);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x000084D3 File Offset: 0x000066D3
	public List<IInGameModelDevTool> GetAllModelToolsForModelType(ToolModelType toolModelType)
	{
		if (this._allTools.Count == 0)
		{
			this.RegisterTools();
		}
		return this._modelContainerTools[toolModelType].GetToolsForModel();
	}

	// Token: 0x0600025C RID: 604 RVA: 0x000084F9 File Offset: 0x000066F9
	public void OnToolsChanged(Action<string> newOnToolsChangedCallback)
	{
		this._onToolsChanged.Add(newOnToolsChangedCallback);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00008507 File Offset: 0x00006707
	public void UpdateEditorIfPresent()
	{
		this.UpdateObservers();
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00008510 File Offset: 0x00006710
	protected void UpdateObservers()
	{
		string activeToolName = "";
		foreach (Action<string> action in this._onToolsChanged)
		{
			action(activeToolName);
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x00008568 File Offset: 0x00006768
	public IInGameDevTool GetDevToolByCommandSerializationName(string commandSerializationName)
	{
		foreach (IInGameDevTool currentTool in this._allTools)
		{
			if (currentTool.GetCommandSerializationName() == commandSerializationName)
			{
				return currentTool;
			}
		}
		return null;
	}

	// Token: 0x06000260 RID: 608 RVA: 0x000085CC File Offset: 0x000067CC
	public IInGameModelDevTool GetModelDevToolByCommandSerializationName(string commandSerializationName)
	{
		foreach (KeyValuePair<ToolModelType, MotorwaysModelContainerTool> currentType in this._modelContainerTools)
		{
			foreach (IInGameModelDevTool currentTool in currentType.Value.GetToolsForModel())
			{
				if (currentTool.GetCommandSerializationName() == commandSerializationName)
				{
					return currentTool;
				}
			}
		}
		return null;
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00008670 File Offset: 0x00006870
	private DevToolType CreateDevToolWithName<DevToolType>(string commandSerializationName) where DevToolType : class, IInGameDevTool
	{
		IInGameDevTool existingTool = this.GetDevToolByCommandSerializationName(commandSerializationName);
		if (existingTool != null)
		{
			this._allTools.Remove(existingTool);
		}
		DevToolType newDevTool = this._scope.Get<DevToolType>();
		newDevTool.SetCommandSerializationName(commandSerializationName);
		newDevTool.PrepareTool();
		this._allTools.Add(newDevTool);
		this.UpdateObservers();
		return newDevTool;
	}

	// Token: 0x06000262 RID: 610 RVA: 0x000086D4 File Offset: 0x000068D4
	private DevToolType CreateModelDevToolWithName<DevToolType>(string commandSerializationName) where DevToolType : IInGameModelDevTool, new()
	{
		IInGameModelDevTool existingTool = this.GetModelDevToolByCommandSerializationName(commandSerializationName);
		if (existingTool != null)
		{
			this._modelContainerTools[existingTool.GetToolModelType()].RemoveTool(existingTool);
		}
		DevToolType newDevTool = (DevToolType)((object)this._scope.Get(typeof(DevToolType)));
		newDevTool.SetCommandSerializationName(commandSerializationName);
		newDevTool.PrepareTool();
		ToolModelType toolModelType = newDevTool.GetToolModelType();
		if (!this._modelContainerTools.ContainsKey(toolModelType))
		{
			MotorwaysModelContainerTool newContainer = this.CreateDevToolWithName<MotorwaysModelContainerTool>("GroupInspect" + toolModelType.ToString()).SetModelType(toolModelType);
			this._modelContainerTools.Add(toolModelType, newContainer);
		}
		this._modelContainerTools[toolModelType].RegisterNewTool(newDevTool);
		this.UpdateObservers();
		return newDevTool;
	}

	// Token: 0x06000263 RID: 611 RVA: 0x000087A8 File Offset: 0x000069A8
	public void RespondToInGameToolUse()
	{
		bool anyModifierKeysActive = false;
		foreach (KeyCode keyCode in this._modifierKeys)
		{
			anyModifierKeysActive |= Input.GetKey(keyCode);
		}
		foreach (IInGameDevTool inGameDevTool in this._allTools)
		{
			bool hotkeyActive = inGameDevTool.InGameHotkeyActivated();
			bool hasModifierKey = inGameDevTool.GetModifierHotKey() > KeyCode.None;
			bool parameterHotKeyActive = inGameDevTool.InGameParameterHotKeyActivated();
			if ((hotkeyActive && (hasModifierKey || !anyModifierKeysActive)) || parameterHotKeyActive)
			{
				inGameDevTool.OnHotkeyActivated(hotkeyActive);
			}
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000887C File Offset: 0x00006A7C
	public void OnReleasedFromScope(IScope scope)
	{
		foreach (IInGameDevTool tool in this._allTools)
		{
			scope.Release(tool);
		}
		this._allTools.Clear();
		this._modifierKeys.Clear();
		foreach (MotorwaysModelContainerTool tool2 in this._modelContainerTools.Values)
		{
			scope.Release(tool2);
		}
		this._modelContainerTools.Clear();
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0000893C File Offset: 0x00006B3C
	public void Reset()
	{
		this._allTools.Clear();
		this._modifierKeys.Clear();
		this._modelContainerTools.Clear();
		this._onToolsChanged.Clear();
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000896C File Offset: 0x00006B6C
	public void RegisterTools()
	{
		if (FeatureToggle.IsFeatureEnabled(Feature.InGameDevTools))
		{
			this.CreateDevToolWithName<MotorwaysDevTool>("NoneTool").SetEditorDisplayName("None").SetEditorIconPath("Assets/Art/UI/Menus/Options/SPR_UI_MenuX.png");
			this.CreateDevToolWithName<MotorwaysDevTool>("AddDestination").SetEditorDisplayName("Add Destination").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().DefaultToResettingToNoneAfterUse().ShowGridWhenActive().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int groupIndex;
				bool isDouble;
				BuildingLayout buildingLayout;
				bool upgrade;
				bool isStation;
				if (command.TryGetIntParameter("groupIndex", out groupIndex) && command.TryGetBoolParameter("isDouble", out isDouble) && command.TryGetEnumParameter<BuildingLayout>("buildingLayout", out buildingLayout) && command.TryGetBoolParameter("upgrade", out upgrade) && command.TryGetBoolParameter("isStation", out isStation))
				{
					bool foundAllParameters = true;
					TileDirection carparkSide = TileDirection.None;
					DrivewayDirection desiredDirection;
					if (isDouble)
					{
						if (isStation)
						{
							if (buildingLayout == BuildingLayout.BuildingAbove)
							{
								foundAllParameters &= command.TryGetEnumParameter<TileDirection>("stationDestinationHorizontalCarparkSide", out carparkSide);
							}
							else
							{
								foundAllParameters &= command.TryGetEnumParameter<TileDirection>("stationDestinationVerticalCarparkSide", out carparkSide);
							}
						}
						desiredDirection = DrivewayDirection.Both;
					}
					else if (buildingLayout == BuildingLayout.BuildingAbove)
					{
						foundAllParameters &= command.TryGetEnumParameter<DrivewayDirection>("singleDestinationAboveDrivewayDirections", out desiredDirection);
					}
					else
					{
						foundAllParameters &= command.TryGetEnumParameter<DrivewayDirection>("singleDestinationToSideDrivewayDirections", out desiredDirection);
					}
					if (foundAllParameters)
					{
						CarparkEntrance carparkEntrance = CarparkEntrance.TopLeft;
						TileDirection drivewayDirection = (buildingLayout == BuildingLayout.BuildingAbove) ? TileDirection.East : TileDirection.South;
						switch (desiredDirection)
						{
						case DrivewayDirection.West:
							carparkEntrance = CarparkEntrance.TopLeft;
							break;
						case DrivewayDirection.East:
							carparkEntrance = CarparkEntrance.BottomRight;
							break;
						case DrivewayDirection.North:
							carparkEntrance = CarparkEntrance.TopLeft;
							break;
						case DrivewayDirection.South:
							carparkEntrance = CarparkEntrance.BottomRight;
							break;
						case DrivewayDirection.Both:
							carparkEntrance = CarparkEntrance.TopLeftAndBottomRight;
							break;
						}
						CarparkPreference carparkPreference;
						if (isStation && isDouble)
						{
							carparkPreference = CarparkPreference.Station;
						}
						else
						{
							carparkPreference = (isDouble ? CarparkPreference.ForceDouble : CarparkPreference.Solo);
						}
						int secondGroupIndex;
						if (!command.TryGetIntParameter("secondGroupIndex", out secondGroupIndex))
						{
							secondGroupIndex = -1;
						}
						bool secondUpgrade;
						if (!command.TryGetBoolParameter("secondUpgrade", out secondUpgrade))
						{
							secondUpgrade = false;
						}
						command.SpawnDestinationAtCursorPosition(carparkEntrance, carparkPreference, drivewayDirection, carparkSide, groupIndex, upgrade, secondGroupIndex, secondUpgrade);
					}
				}
			}).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("isDouble").SetEditorDisplayName("Is a double destination?").SetEditorTooltip("If true, the destination will have two buildings.").SetValue(false).SetDefaultValueForHotkey(false)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("isStation").SetEditorDisplayName("Is a station?").SetEditorTooltip("If true, the destination will be a train station.").ShowConditionallyOnBool("isDouble", true).SetValue(false).SetDefaultValueForHotkey(false)).WithEnumParam(IngameDevToolEnumParameter<BuildingLayout>.DefineEnumParameter("buildingLayout").SetEditorDisplayName("Building Layout").SetEditorTooltip("Where should the building be relative to the carpark?").SetValue(BuildingLayout.BuildingAbove).SetDefaultValueForHotkey(BuildingLayout.BuildingAbove)).WithEnumParam(IngameDevToolEnumParameter<DrivewayDirection>.DefineEnumParameter("singleDestinationAboveDrivewayDirections").SetEditorDisplayName("Carpark Entrance").SetEditorTooltip("Which side of the carpark is the entrance on?").ShowConditionallyOnBool("isDouble", false).ShowConditionallyOnBool("isStation", false).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingAbove).SetAllowedValues(new List<DrivewayDirection>
			{
				DrivewayDirection.West,
				DrivewayDirection.East
			}).SetValue(DrivewayDirection.West).SetDefaultValueForHotkey(DrivewayDirection.West)).WithEnumParam(IngameDevToolEnumParameter<TileDirection>.DefineEnumParameter("stationDestinationVerticalCarparkSide").SetEditorDisplayName("Carpark Side").SetEditorTooltip("Which side of the entire destination are the carpark tiles on?").ShowConditionallyOnBool("isDouble", true).ShowConditionallyOnBool("isStation", true).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingToSide).SetAllowedValues(new List<TileDirection>
			{
				TileDirection.West,
				TileDirection.East
			}).SetValue(TileDirection.West).SetDefaultValueForHotkey(TileDirection.West)).WithEnumParam(IngameDevToolEnumParameter<TileDirection>.DefineEnumParameter("stationDestinationHorizontalCarparkSide").SetEditorDisplayName("Carpark Side").SetEditorTooltip("Which side of the entire destination are the carpark tiles on?").ShowConditionallyOnBool("isDouble", true).ShowConditionallyOnBool("isStation", true).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingAbove).SetAllowedValues(new List<TileDirection>
			{
				TileDirection.South,
				TileDirection.North
			}).SetValue(TileDirection.South).SetDefaultValueForHotkey(TileDirection.South)).WithEnumParam(IngameDevToolEnumParameter<DrivewayDirection>.DefineEnumParameter("singleDestinationToSideDrivewayDirections").SetEditorDisplayName("Carpark Entrance").SetEditorTooltip("Which side of the carpark is the entrance on?").ShowConditionallyOnBool("isDouble", false).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingToSide).SetAllowedValues(new List<DrivewayDirection>
			{
				DrivewayDirection.North,
				DrivewayDirection.South
			}).SetValue(DrivewayDirection.North).SetDefaultValueForHotkey(DrivewayDirection.North)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("Which group should the destination belong to?").SetMinimumValue(0).SetMaximumValue(5)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("upgrade").SetEditorDisplayName("Upgrade First Destination").SetEditorTooltip("The first (or only) destination will start as a circle").SetValue(false).SetDefaultValueForHotkey(false)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("secondGroupIndex").SetEditorDisplayName("Second Destination Group Index").SetEditorTooltip("Which group should the destination belong to? (-1 indicates that it should be empty)").SetMinimumValue(-1).SetMaximumValue(5).SetValue(-1).SetDefaultValueForHotkey(-1).ShowConditionallyOnBool("isDouble", true)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("secondUpgrade").SetEditorDisplayName("Upgrade Second Destination").SetEditorTooltip("The second destination will start as a circle").SetValue(false).SetDefaultValueForHotkey(false).ShowConditionallyOnBool("isDouble", true)).DrawOnTilesUnderCursor(delegate(MotorwaysDevTool devTool, Vector2Int newHoveredTile, DebugTileDataViewer tileDataView)
			{
				int groupIndex = devTool.GetIntParameter("groupIndex").ParameterValue;
				MotorwaysThemeDatabase themeDatabase = devTool.gameScope.Get<IThemeDatabase>() as MotorwaysThemeDatabase;
				Color groupColor = (themeDatabase.GetTheme() as Theme).GetBuildingColor(groupIndex, ThemeComponentGroupTarget.BuildingBase);
				bool isDouble = devTool.GetBoolParameter("isDouble").ParameterValue;
				BuildingLayout buildingLayout = devTool.GetEnumParameter<BuildingLayout>("buildingLayout").ParameterValue;
				Color secondGroupColor = Color.black;
				if (isDouble)
				{
					int secondGroupIndex = devTool.GetIntParameter("secondGroupIndex").ParameterValue;
					if (secondGroupIndex != -1)
					{
						secondGroupColor = (themeDatabase.GetTheme() as Theme).GetBuildingColor(secondGroupIndex, ThemeComponentGroupTarget.BuildingBase);
					}
				}
				int specificLayoutDrivewayIndex = 0;
				DrivewayDirection desiredDirection;
				BuildingPlacer.Layout desiredLayout;
				if (isDouble)
				{
					desiredDirection = DrivewayDirection.Both;
					if (buildingLayout == BuildingLayout.BuildingAbove)
					{
						desiredLayout = BuildingSpawningProcess.DoubleCarparkLayouts[1];
					}
					else
					{
						desiredLayout = BuildingSpawningProcess.DoubleCarparkLayouts[0];
					}
				}
				else if (buildingLayout == BuildingLayout.BuildingAbove)
				{
					desiredDirection = devTool.GetEnumParameter<DrivewayDirection>("singleDestinationAboveDrivewayDirections").ParameterValue;
					desiredLayout = ((desiredDirection == DrivewayDirection.West) ? BuildingSpawningProcess.SingleCarparkLayouts[0] : BuildingSpawningProcess.SingleCarparkLayouts[1]);
				}
				else
				{
					desiredDirection = devTool.GetEnumParameter<DrivewayDirection>("singleDestinationToSideDrivewayDirections").ParameterValue;
					desiredLayout = ((desiredDirection == DrivewayDirection.North) ? BuildingSpawningProcess.SingleCarparkLayouts[2] : BuildingSpawningProcess.SingleCarparkLayouts[3]);
				}
				tileDataView.squareTileData.Clear();
				tileDataView.checkerSquareTileData.Clear();
				Vector2Int carparkEntrancePosition = desiredLayout.driveways[specificLayoutDrivewayIndex].coordinatesOffset;
				for (int x = 0; x < desiredLayout.footprint.x; x++)
				{
					for (int y = 0; y < desiredLayout.footprint.y; y++)
					{
						Vector2Int relativeTilePosition = new Vector2Int(x, y);
						Vector2Int tilePosition = relativeTilePosition + newHoveredTile;
						bool isCarpark;
						if (buildingLayout == BuildingLayout.BuildingAbove)
						{
							isCarpark = (relativeTilePosition.y == carparkEntrancePosition.y);
						}
						else
						{
							isCarpark = (relativeTilePosition.x == carparkEntrancePosition.x);
						}
						if (isCarpark)
						{
							tileDataView.squareTileData.Add(tilePosition, Color.grey);
						}
						else
						{
							Color desiredColor = groupColor;
							if (isDouble)
							{
								if (buildingLayout == BuildingLayout.BuildingAbove && x > 1)
								{
									desiredColor = secondGroupColor;
								}
								else if (buildingLayout == BuildingLayout.BuildingToSide && y < 2)
								{
									desiredColor = secondGroupColor;
								}
							}
							tileDataView.squareTileData.Add(tilePosition, desiredColor);
						}
					}
				}
				Vector2Int drivewayPosition = newHoveredTile + carparkEntrancePosition + TileUtilities.GetAdjacentCoordinates(Vector2Int.zero, desiredLayout.driveways[specificLayoutDrivewayIndex].direction);
				tileDataView.squareTileData.Add(drivewayPosition, Color.grey);
				if (isDouble && desiredDirection == DrivewayDirection.Both)
				{
					Vector2Int secondDrivewayPosition = newHoveredTile + desiredLayout.driveways[specificLayoutDrivewayIndex + 1].coordinatesOffset + TileUtilities.GetAdjacentCoordinates(Vector2Int.zero, desiredLayout.driveways[specificLayoutDrivewayIndex + 1].direction);
					tileDataView.squareTileData.Add(secondDrivewayPosition, Color.grey);
				}
			}).ActivateOnInGameHotkey(KeyCode.T, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("AddDoubleDestination").SetEditorDisplayName("Add Double Destination").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().DefaultToResettingToNoneAfterUse().ShowGridWhenActive().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int groupIndex;
				bool isDouble;
				BuildingLayout buildingLayout;
				bool upgrade;
				bool isStation;
				if (command.TryGetIntParameter("groupIndex", out groupIndex) && command.TryGetBoolParameter("isDouble", out isDouble) && command.TryGetEnumParameter<BuildingLayout>("buildingLayout", out buildingLayout) && command.TryGetBoolParameter("upgrade", out upgrade) && command.TryGetBoolParameter("isStation", out isStation))
				{
					bool foundAllParameters = true;
					TileDirection carparkSide = TileDirection.None;
					DrivewayDirection desiredDirection;
					if (isDouble)
					{
						if (isStation)
						{
							if (buildingLayout == BuildingLayout.BuildingAbove)
							{
								foundAllParameters &= command.TryGetEnumParameter<TileDirection>("stationDestinationHorizontalCarparkSide", out carparkSide);
							}
							else
							{
								foundAllParameters &= command.TryGetEnumParameter<TileDirection>("stationDestinationVerticalCarparkSide", out carparkSide);
							}
						}
						desiredDirection = DrivewayDirection.Both;
					}
					else if (buildingLayout == BuildingLayout.BuildingAbove)
					{
						foundAllParameters &= command.TryGetEnumParameter<DrivewayDirection>("singleDestinationAboveDrivewayDirections", out desiredDirection);
					}
					else
					{
						foundAllParameters &= command.TryGetEnumParameter<DrivewayDirection>("singleDestinationToSideDrivewayDirections", out desiredDirection);
					}
					if (foundAllParameters)
					{
						CarparkEntrance carparkEntrance = CarparkEntrance.TopLeft;
						TileDirection drivewayDirection = (buildingLayout == BuildingLayout.BuildingAbove) ? TileDirection.East : TileDirection.South;
						switch (desiredDirection)
						{
						case DrivewayDirection.West:
							carparkEntrance = CarparkEntrance.TopLeft;
							break;
						case DrivewayDirection.East:
							carparkEntrance = CarparkEntrance.BottomRight;
							break;
						case DrivewayDirection.North:
							carparkEntrance = CarparkEntrance.TopLeft;
							break;
						case DrivewayDirection.South:
							carparkEntrance = CarparkEntrance.BottomRight;
							break;
						case DrivewayDirection.Both:
							carparkEntrance = CarparkEntrance.TopLeftAndBottomRight;
							break;
						}
						CarparkPreference carparkPreference;
						if (isStation && isDouble)
						{
							carparkPreference = CarparkPreference.Station;
						}
						else
						{
							carparkPreference = (isDouble ? CarparkPreference.ForceDouble : CarparkPreference.Solo);
						}
						int secondGroupIndex;
						if (!command.TryGetIntParameter("secondGroupIndex", out secondGroupIndex))
						{
							secondGroupIndex = -1;
						}
						bool secondUpgrade;
						if (!command.TryGetBoolParameter("secondUpgrade", out secondUpgrade))
						{
							secondUpgrade = false;
						}
						command.SpawnDestinationAtCursorPosition(carparkEntrance, carparkPreference, drivewayDirection, carparkSide, groupIndex, upgrade, secondGroupIndex, secondUpgrade);
					}
				}
			}).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("isDouble").SetEditorDisplayName("Is a double destination?").SetEditorTooltip("If true, the destination will have two buildings.").SetValue(true).SetDefaultValueForHotkey(true)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("isStation").SetEditorDisplayName("Is a station?").SetEditorTooltip("If true, the destination will be a train station.").ShowConditionallyOnBool("isDouble", true).SetValue(false).SetDefaultValueForHotkey(false)).WithEnumParam(IngameDevToolEnumParameter<BuildingLayout>.DefineEnumParameter("buildingLayout").SetEditorDisplayName("Building Layout").SetEditorTooltip("Where should the building be relative to the carpark?").SetValue(BuildingLayout.BuildingAbove).SetDefaultValueForHotkey(BuildingLayout.BuildingAbove)).WithEnumParam(IngameDevToolEnumParameter<DrivewayDirection>.DefineEnumParameter("singleDestinationAboveDrivewayDirections").SetEditorDisplayName("Carpark Entrance").SetEditorTooltip("Which side of the carpark is the entrance on?").ShowConditionallyOnBool("isDouble", true).ShowConditionallyOnBool("isStation", false).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingAbove).SetAllowedValues(new List<DrivewayDirection>
			{
				DrivewayDirection.West,
				DrivewayDirection.East
			}).SetValue(DrivewayDirection.West).SetDefaultValueForHotkey(DrivewayDirection.West)).WithEnumParam(IngameDevToolEnumParameter<TileDirection>.DefineEnumParameter("stationDestinationVerticalCarparkSide").SetEditorDisplayName("Carpark Side").SetEditorTooltip("Which side of the entire destination are the carpark tiles on?").ShowConditionallyOnBool("isDouble", true).ShowConditionallyOnBool("isStation", true).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingToSide).SetAllowedValues(new List<TileDirection>
			{
				TileDirection.West,
				TileDirection.East
			}).SetValue(TileDirection.West).SetDefaultValueForHotkey(TileDirection.West)).WithEnumParam(IngameDevToolEnumParameter<TileDirection>.DefineEnumParameter("stationDestinationHorizontalCarparkSide").SetEditorDisplayName("Carpark Side").SetEditorTooltip("Which side of the entire destination are the carpark tiles on?").ShowConditionallyOnBool("isDouble", true).ShowConditionallyOnBool("isStation", true).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingAbove).SetAllowedValues(new List<TileDirection>
			{
				TileDirection.South,
				TileDirection.North
			}).SetValue(TileDirection.South).SetDefaultValueForHotkey(TileDirection.South)).WithEnumParam(IngameDevToolEnumParameter<DrivewayDirection>.DefineEnumParameter("singleDestinationToSideDrivewayDirections").SetEditorDisplayName("Carpark Entrance").SetEditorTooltip("Which side of the carpark is the entrance on?").ShowConditionallyOnBool("isDouble", false).ShowConditionallyOnEnum("buildingLayout", BuildingLayout.BuildingToSide).SetAllowedValues(new List<DrivewayDirection>
			{
				DrivewayDirection.North,
				DrivewayDirection.South
			}).SetValue(DrivewayDirection.North).SetDefaultValueForHotkey(DrivewayDirection.North)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("Which group should the destination belong to?").SetMinimumValue(0).SetMaximumValue(5)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("upgrade").SetEditorDisplayName("Upgrade First Destination").SetEditorTooltip("The first (or only) destination will start as a circle").SetValue(false).SetDefaultValueForHotkey(false)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("secondGroupIndex").SetEditorDisplayName("Second Destination Group Index").SetEditorTooltip("Which group should the destination belong to? (-1 indicates that it should be empty)").SetMinimumValue(-1).SetMaximumValue(5).SetValue(-1).SetDefaultValueForHotkey(-1).ShowConditionallyOnBool("isDouble", true)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("secondUpgrade").SetEditorDisplayName("Upgrade Second Destination").SetEditorTooltip("The second destination will start as a circle").SetValue(false).SetDefaultValueForHotkey(false).ShowConditionallyOnBool("isDouble", true)).DrawOnTilesUnderCursor(delegate(MotorwaysDevTool devTool, Vector2Int newHoveredTile, DebugTileDataViewer tileDataView)
			{
				int groupIndex = devTool.GetIntParameter("groupIndex").ParameterValue;
				MotorwaysThemeDatabase themeDatabase = devTool.gameScope.Get<IThemeDatabase>() as MotorwaysThemeDatabase;
				Color groupColor = (themeDatabase.GetTheme() as Theme).GetBuildingColor(groupIndex, ThemeComponentGroupTarget.BuildingBase);
				bool isDouble = devTool.GetBoolParameter("isDouble").ParameterValue;
				BuildingLayout buildingLayout = devTool.GetEnumParameter<BuildingLayout>("buildingLayout").ParameterValue;
				Color secondGroupColor = Color.black;
				if (isDouble)
				{
					int secondGroupIndex = devTool.GetIntParameter("secondGroupIndex").ParameterValue;
					if (secondGroupIndex != -1)
					{
						secondGroupColor = (themeDatabase.GetTheme() as Theme).GetBuildingColor(secondGroupIndex, ThemeComponentGroupTarget.BuildingBase);
					}
				}
				int specificLayoutDrivewayIndex = 0;
				DrivewayDirection desiredDirection;
				BuildingPlacer.Layout desiredLayout;
				if (isDouble)
				{
					desiredDirection = DrivewayDirection.Both;
					if (buildingLayout == BuildingLayout.BuildingAbove)
					{
						desiredLayout = BuildingSpawningProcess.DoubleCarparkLayouts[1];
					}
					else
					{
						desiredLayout = BuildingSpawningProcess.DoubleCarparkLayouts[0];
					}
				}
				else if (buildingLayout == BuildingLayout.BuildingAbove)
				{
					desiredDirection = devTool.GetEnumParameter<DrivewayDirection>("singleDestinationAboveDrivewayDirections").ParameterValue;
					desiredLayout = ((desiredDirection == DrivewayDirection.West) ? BuildingSpawningProcess.SingleCarparkLayouts[0] : BuildingSpawningProcess.SingleCarparkLayouts[1]);
				}
				else
				{
					desiredDirection = devTool.GetEnumParameter<DrivewayDirection>("singleDestinationToSideDrivewayDirections").ParameterValue;
					desiredLayout = ((desiredDirection == DrivewayDirection.North) ? BuildingSpawningProcess.SingleCarparkLayouts[2] : BuildingSpawningProcess.SingleCarparkLayouts[3]);
				}
				tileDataView.squareTileData.Clear();
				tileDataView.checkerSquareTileData.Clear();
				Vector2Int carparkEntrancePosition = desiredLayout.driveways[specificLayoutDrivewayIndex].coordinatesOffset;
				for (int x = 0; x < desiredLayout.footprint.x; x++)
				{
					for (int y = 0; y < desiredLayout.footprint.y; y++)
					{
						Vector2Int relativeTilePosition = new Vector2Int(x, y);
						Vector2Int tilePosition = relativeTilePosition + newHoveredTile;
						bool isCarpark;
						if (buildingLayout == BuildingLayout.BuildingAbove)
						{
							isCarpark = (relativeTilePosition.y == carparkEntrancePosition.y);
						}
						else
						{
							isCarpark = (relativeTilePosition.x == carparkEntrancePosition.x);
						}
						if (isCarpark)
						{
							tileDataView.squareTileData.Add(tilePosition, Color.grey);
						}
						else
						{
							Color desiredColor = groupColor;
							if (isDouble)
							{
								if (buildingLayout == BuildingLayout.BuildingAbove && x > 1)
								{
									desiredColor = secondGroupColor;
								}
								else if (buildingLayout == BuildingLayout.BuildingToSide && y < 2)
								{
									desiredColor = secondGroupColor;
								}
							}
							tileDataView.squareTileData.Add(tilePosition, desiredColor);
						}
					}
				}
				Vector2Int drivewayPosition = newHoveredTile + carparkEntrancePosition + TileUtilities.GetAdjacentCoordinates(Vector2Int.zero, desiredLayout.driveways[specificLayoutDrivewayIndex].direction);
				tileDataView.squareTileData.Add(drivewayPosition, Color.grey);
				if (isDouble && desiredDirection == DrivewayDirection.Both)
				{
					Vector2Int secondDrivewayPosition = newHoveredTile + desiredLayout.driveways[specificLayoutDrivewayIndex + 1].coordinatesOffset + TileUtilities.GetAdjacentCoordinates(Vector2Int.zero, desiredLayout.driveways[specificLayoutDrivewayIndex + 1].direction);
					tileDataView.squareTileData.Add(secondDrivewayPosition, Color.grey);
				}
			}).ActivateOnInGameHotkey(KeyCode.Y, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("AddHouse").SetEditorDisplayName("Add House").SetEditorIconPath("Assets/Art/UI/SPR_UI_Button_home.png").ActivateOnDefaultActionInput().DefaultToResettingToNoneAfterUse().ShowGridWhenActive().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				TileDirection drivewayDirection;
				command.TryGetEnumParameter<TileDirection>("drivewayDirection", out drivewayDirection);
				int groupIndex;
				command.TryGetIntParameter("groupIndex", out groupIndex);
				command.SpawnHouse(drivewayDirection, groupIndex);
			}).WithEnumParam(IngameDevToolEnumParameter<TileDirection>.DefineEnumParameter("drivewayDirection").SetEditorDisplayName("Driveway Direction").SetEditorTooltip("Which direction should the driveway face? Set to `None` for a random direction.").SetValue(TileDirection.East).SetDefaultValueForHotkey(TileDirection.East)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("Which group should we the house belong to? Set to `-1` for random colour.").SetMinimumValue(-1).SetMaximumValue(5).SetValue(0)).ActivateOnInGameHotkey(KeyCode.H, KeyCode.None).DrawOnTilesUnderCursor(delegate(MotorwaysDevTool devTool, Vector2Int newHoveredTile, DebugTileDataViewer tileDataView)
			{
				tileDataView.Clear();
				BuildingPlacer placer = this._scope.Get<BuildingPlacer>();
				placer.StartPlacing(TileContentType.House, 0, GroupingStyle.Normal, BuildingPlacer.WeightEvaluationLevel.IgnoreWeights, BuildingPlacer.WeightSource.Default);
				BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				RectInt placeableArea = (typeof(BuildingPlacer).GetField("_placeableArea", bindFlags).GetValue(placer) as RectInt?) ?? new RectInt(0, 0, 0, 0);
				List<Fix64> weights = typeof(BuildingPlacer).GetField("_placeableTileWeights", bindFlags).GetValue(placer) as List<Fix64>;
				foreach (Vector2Int tilePosition in placeableArea.allPositionsWithin)
				{
					Vector2Int relativePosition = tilePosition - placeableArea.min;
					int index = relativePosition.x + placeableArea.width * relativePosition.y;
					if (weights[index] <= Fix64.Zero)
					{
						tileDataView.checkerSquareTileData.Add(tilePosition, Color.red);
					}
				}
				Color borderColor = Color.Lerp(Color.grey, Color.clear, 0.8f);
				for (int x = placeableArea.xMin - 1; x <= placeableArea.xMax; x++)
				{
					tileDataView.squareTileData.Add(new Vector2Int(x, placeableArea.yMin - 1), borderColor);
					tileDataView.squareTileData.Add(new Vector2Int(x, placeableArea.yMax), borderColor);
				}
				for (int y = placeableArea.yMin - 1; y <= placeableArea.yMax; y++)
				{
					tileDataView.squareTileData[new Vector2Int(placeableArea.xMin - 1, y)] = borderColor;
					tileDataView.squareTileData[new Vector2Int(placeableArea.xMax, y)] = borderColor;
				}
				int groupIndex = devTool.GetIntParameter("groupIndex").ParameterValue;
				MotorwaysThemeDatabase themeDatabase = devTool.gameScope.Get<IThemeDatabase>() as MotorwaysThemeDatabase;
				Color groupColor;
				if (groupIndex >= 0)
				{
					groupColor = (themeDatabase.GetTheme() as Theme).GetBuildingColor(groupIndex, ThemeComponentGroupTarget.BuildingBase);
				}
				else
				{
					groupColor = Color.white;
				}
				tileDataView.squareTileData[newHoveredTile] = groupColor;
				TileDirection direction = devTool.GetEnumParameter<TileDirection>("drivewayDirection").ParameterValue;
				Vector2Int drivewayTile = newHoveredTile + TileUtilities.GetAdjacencyOffsetForDirection(direction);
				tileDataView.squareTileData[drivewayTile] = Color.grey;
				tileDataView.checkerTilesOn = true;
				tileDataView.squareTilesOn = true;
			});
			this.CreateDevToolWithName<MotorwaysDevTool>("RemoveDestination").SetEditorDisplayName("Remove Destination Or House").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().DefaultToResettingToNoneAfterUse().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				command.RemoveAnyBuilding();
			}).ActivateOnInGameHotkey(KeyCode.X, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ChangeGroupIndex").SetEditorDisplayName("Change Group Index").SetEditorIconPath("Assets/Art/UI/SPR_UI_Button_home.png").ActivateOnDefaultActionInput().DefaultToResettingToNoneAfterUse().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int groupIndex;
				if (command.TryGetIntParameter("groupIndex", out groupIndex))
				{
					command.ChangeGroupIndex(groupIndex);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("Which group should we change to?").SetMinimumValue(0).SetMaximumValue(5));
			this.CreateDevToolWithName<MotorwaysDevTool>("RotateDestination").SetEditorDisplayName("Rotate Destination").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_Roundabout.png").ActivateOnDefaultActionInput().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				command.RotateBuilding();
			});
			this.CreateDevToolWithName<MotorwaysDevTool>("FlipDestination").SetEditorDisplayName("Flip Destination").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				command.FlipDestination();
			});
			this.CreateDevToolWithName<MotorwaysDevTool>("UpgradeDestination").SetEditorDisplayName("Upgrade Destination").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				command.UpgradeDestination();
			}).ActivateOnInGameHotkey(KeyCode.Period, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("DowngradeDestination").SetEditorDisplayName("Downgrade Destination").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				command.DowngradeDestinations();
			}).ActivateOnInGameHotkey(KeyCode.Comma, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("GrantUpgrade").SetEditorDisplayName("Grant Upgrade").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_RoadStack.png").ActivateOnEditorButton("Grant Upgrades").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				UpgradeDatabaseModel upgradeDatabase = simulation.GetModel<UpgradeDatabaseModel>();
				int concreteCount;
				command.TryGetIntParameter("concreteCount", out concreteCount);
				if (concreteCount > 0)
				{
					upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.Concrete,
						amount = concreteCount
					}, true);
				}
				int bridgeCount;
				command.TryGetIntParameter("bridgeCount", out bridgeCount);
				if (bridgeCount > 0)
				{
					upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.Bridge,
						amount = bridgeCount
					}, true);
				}
				int tunnelCount;
				command.TryGetIntParameter("tunnelCount", out tunnelCount);
				if (tunnelCount > 0)
				{
					upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.Tunnel,
						amount = tunnelCount
					}, true);
				}
				int motorwayCount;
				command.TryGetIntParameter("motorwayCount", out motorwayCount);
				if (motorwayCount > 0)
				{
					upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.Motorway,
						amount = motorwayCount
					}, true);
				}
				int trafficLightCount;
				command.TryGetIntParameter("trafficLightCount", out trafficLightCount);
				if (trafficLightCount > 0)
				{
					upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.TrafficLight,
						amount = trafficLightCount
					}, true);
				}
				int roundaboutCount;
				command.TryGetIntParameter("roundaboutCount", out roundaboutCount);
				if (roundaboutCount > 0)
				{
					upgradeDatabase.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.Roundabout,
						amount = roundaboutCount
					}, true);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("concreteCount").SetEditorDisplayName("Concrete Count").SetEditorTooltip("How much concrete to grant.").SetDefaultValueForHotkey(20)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("bridgeCount").SetEditorDisplayName("Bridge Count").SetEditorTooltip("How many bridges to grant.").SetDefaultValueForHotkey(1)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("tunnelCount").SetEditorDisplayName("Tunnel Count").SetEditorTooltip("How many tunnels to grant.").SetDefaultValueForHotkey(1)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("motorwayCount").SetEditorDisplayName("Motorway Count").SetEditorTooltip("How many motorways to grant.").SetDefaultValueForHotkey(1)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("trafficLightCount").SetEditorDisplayName("Traffic Light Count").SetEditorTooltip("How many traffic lights to grant.").SetDefaultValueForHotkey(1)).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("roundaboutCount").SetEditorDisplayName("Roundabout Count").SetEditorTooltip("How many roundabouts to grant.").SetDefaultValueForHotkey(1)).ActivateOnInGameHotkey(KeyCode.S, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("RemoveUpgrade").SetEditorDisplayName("Remove Upgrade").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_RoadStack.png").ActivateOnEditorButton("Remove Upgrades").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				UpgradeDatabaseModel upgradeDatabase = simulation.GetModel<UpgradeDatabaseModel>();
				int concreteCount;
				command.TryGetIntParameter("concreteCount", out concreteCount);
				if (concreteCount > 0)
				{
					concreteCount = Math.Min(concreteCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Concrete));
					Diagnostics.Verify(upgradeDatabase.ConsumeUpgrade(UpgradeType.Concrete, concreteCount), "We tried to remove more {0} ({1}) than we had ({2}).", "Concrete", concreteCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Concrete));
				}
				int bridgeCount;
				command.TryGetIntParameter("bridgeCount", out bridgeCount);
				if (bridgeCount > 0)
				{
					bridgeCount = Math.Min(bridgeCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Bridge));
					Diagnostics.Verify(upgradeDatabase.ConsumeUpgrade(UpgradeType.Bridge, bridgeCount), "We tried to remove more {0} ({1}) than we had ({2}).", "Bridge", bridgeCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Bridge));
				}
				int tunnelCount;
				command.TryGetIntParameter("tunnelCount", out tunnelCount);
				if (tunnelCount > 0)
				{
					tunnelCount = Math.Min(tunnelCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Tunnel));
					Diagnostics.Verify(upgradeDatabase.ConsumeUpgrade(UpgradeType.Tunnel, tunnelCount), "We tried to remove more {0} ({1}) than we had ({2}).", "Tunnel", tunnelCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Tunnel));
				}
				int motorwayCount;
				command.TryGetIntParameter("motorwayCount", out motorwayCount);
				if (motorwayCount > 0)
				{
					motorwayCount = Math.Min(motorwayCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Motorway));
					Diagnostics.Verify(upgradeDatabase.ConsumeUpgrade(UpgradeType.Motorway, motorwayCount), "We tried to remove more {0} ({1}) than we had ({2}).", "Motorway", motorwayCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Motorway));
				}
				int trafficLightCount;
				command.TryGetIntParameter("trafficLightCount", out trafficLightCount);
				if (trafficLightCount > 0)
				{
					trafficLightCount = Math.Min(trafficLightCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.TrafficLight));
					Diagnostics.Verify(upgradeDatabase.ConsumeUpgrade(UpgradeType.TrafficLight, trafficLightCount), "We tried to remove more {0} ({1}) than we had ({2}).", "TrafficLight", trafficLightCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.TrafficLight));
				}
				int roundaboutCount;
				command.TryGetIntParameter("roundaboutCount", out roundaboutCount);
				if (roundaboutCount > 0)
				{
					roundaboutCount = Math.Min(roundaboutCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Roundabout));
					Diagnostics.Verify(upgradeDatabase.ConsumeUpgrade(UpgradeType.Roundabout, roundaboutCount), "We tried to remove more {0} ({1}) than we had ({2}).", "Roundabout", roundaboutCount, upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Roundabout));
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("concreteCount").SetEditorDisplayName("Concrete Count").SetEditorTooltip("How much concrete to remove.")).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("bridgeCount").SetEditorDisplayName("Bridge Count").SetEditorTooltip("How many bridges to remove.")).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("tunnelCount").SetEditorDisplayName("Tunnel Count").SetEditorTooltip("How many tunnels to remove.")).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("motorwayCount").SetEditorDisplayName("Motorway Count").SetEditorTooltip("How many motorways to remove.")).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("trafficLightCount").SetEditorDisplayName("Traffic Light Count").SetEditorTooltip("How many traffic lights to remove.")).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("roundaboutCount").SetEditorDisplayName("Roundabout Count").SetEditorTooltip("How many roundabouts to remove."));
			this.CreateDevToolWithName<MotorwaysDevTool>("SetCitySpawnMode").SetEditorDisplayName("Set City SpawnMode").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnEditorButton("Next Mode").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				CityPlanModel.BuildingSpawningMode spawningMode = command.GetSpawningMode();
				switch (spawningMode)
				{
				case CityPlanModel.BuildingSpawningMode.None:
					spawningMode = CityPlanModel.BuildingSpawningMode.Houses;
					break;
				case CityPlanModel.BuildingSpawningMode.Houses:
					spawningMode = CityPlanModel.BuildingSpawningMode.Destinations;
					break;
				case CityPlanModel.BuildingSpawningMode.Destinations:
					spawningMode = CityPlanModel.BuildingSpawningMode.All;
					break;
				case CityPlanModel.BuildingSpawningMode.All:
					spawningMode = CityPlanModel.BuildingSpawningMode.None;
					break;
				}
				command.SetSpawningMode(spawningMode);
				this._hotkeyDebugView.ShowMessage(string.Format("Spawning Mode is: {0}", spawningMode));
			}).ActivateOnInGameHotkey(KeyCode.L, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleClockPaused").SetEditorDisplayName("Toggle Clock Paused").ActivateOnEditorButton("Toggle").SetEditorIconPath("Assets/Art/UI/Clock/SPR_UI_Clock_Face.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool isCurrentlyPaused = simulation.GetModel<ClockModel>().isPaused;
				command.SetClockPaused(!isCurrentlyPaused);
				string state = (!isCurrentlyPaused) ? "paused" : "unpaused";
				this._hotkeyDebugView.ShowMessage("Clock " + state);
			}).ActivateOnInGameHotkey(KeyCode.K, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ClearAll").SetEditorDisplayName("Clear All").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_Trashcan.png").ActivateOnEditorButton("Clear Selected On All Tiles").SetClientCodeToExecute(delegate(MotorwaysDevTool tool, ISimulation simulation)
			{
				IngameDevToolBoolParameter roadsParam = tool.GetBoolParameter("roads");
				IngameDevToolBoolParameter destinationsParam = tool.GetBoolParameter("destinations");
				InGameDevToolParameter<bool, IngameDevToolBoolParameter> boolParameter = tool.GetBoolParameter("houses");
				bool clearRoads = roadsParam.ParameterValue;
				bool parameterValue = destinationsParam.ParameterValue;
				bool parameterValue2 = boolParameter.ParameterValue;
				TilemapModel tilemapModel = this._scope.Get<TilemapModel>();
				List<Vector2Int> tilesToDelete = new List<Vector2Int>();
				List<Vector2Int> destinationTiles = new List<Vector2Int>();
				List<Vector2Int> houseTiles = new List<Vector2Int>();
				foreach (DestinationModel destinationModel in simulation.GetModels<DestinationModel>())
				{
					foreach (TileModel destinationTile in destinationModel.TileModels)
					{
						destinationTiles.Add(destinationTile.Coordinates);
					}
				}
				foreach (HouseModel houseModel in simulation.GetModels<HouseModel>())
				{
					houseTiles.Add(houseModel.tileModel.Coordinates);
				}
				foreach (Vector2Int currentTileCoordinate in tilemapModel.GetAllTileCoordinates())
				{
					if (!destinationTiles.Contains(currentTileCoordinate) && !houseTiles.Contains(currentTileCoordinate))
					{
						Tile currentTile = tilemapModel.GetTile(currentTileCoordinate);
						if (!currentTile.IsEmpty() && clearRoads && currentTile.GetTwoLaneRoadCount(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Include) > 0)
						{
							tilesToDelete.Add(currentTileCoordinate);
						}
					}
				}
				foreach (Vector2Int coordinateToDelete in tilesToDelete)
				{
					if (clearRoads)
					{
						tool.RemoveRoadsAndUpgradesAtTileCoordinate(coordinateToDelete);
					}
				}
			}).SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool clearRoads;
				command.TryGetBoolParameter("roads", out clearRoads);
				bool clearDestinations;
				command.TryGetBoolParameter("destinations", out clearDestinations);
				bool clearHouses;
				command.TryGetBoolParameter("houses", out clearHouses);
				this._scope.Get<TilemapModel>();
				List<Vector2Int> destinationTiles = new List<Vector2Int>();
				List<Vector2Int> houseTiles = new List<Vector2Int>();
				foreach (DestinationModel destinationModel in simulation.GetModels<DestinationModel>())
				{
					foreach (TileModel destinationTile in destinationModel.TileModels)
					{
						destinationTiles.Add(destinationTile.Coordinates);
					}
				}
				foreach (HouseModel houseModel in simulation.GetModels<HouseModel>())
				{
					houseTiles.Add(houseModel.tileModel.Coordinates);
				}
				foreach (Vector2Int destinationCoordinate in destinationTiles)
				{
					if (clearDestinations)
					{
						command.RemoveSpecificBuildingAtTileCoordinate(destinationCoordinate, TileContentType.Destination);
						command.RemoveSpecificBuildingAtTileCoordinate(destinationCoordinate, TileContentType.Carpark);
					}
				}
				foreach (Vector2Int houseCoordinate in houseTiles)
				{
					if (clearHouses)
					{
						command.RemoveSpecificBuildingAtTileCoordinate(houseCoordinate, TileContentType.House);
					}
				}
			}).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("roads").SetEditorDisplayName("Clear Roads").SetEditorTooltip("This clears all road types that aren't driveways (includes bridges, tunnels, roundabouts, motorways, etc).").SetValue(true).SetDefaultValueForHotkey(true)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("destinations").SetEditorDisplayName("Clear Destinations").SetEditorTooltip("This clears destinations and their entry roads.").SetValue(true).SetDefaultValueForHotkey(true)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("houses").SetEditorDisplayName("Clear Houses").SetEditorTooltip("This clears houses and their driveways.").SetValue(true).SetDefaultValueForHotkey(true));
			this.CreateDevToolWithName<MotorwaysDevTool>("ChangePeepCountByColorGroup").SetEditorDisplayName("Change Peep Count By Color Group").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnEditorButton("Apply").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int targetGroupIndex;
				int deltaPeepCount;
				if (command.TryGetIntParameter("groupIndex", out targetGroupIndex) && command.TryGetIntParameter("deltaPeepCount", out deltaPeepCount))
				{
					command.ChangePeepCount(deltaPeepCount, targetGroupIndex);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("deltaPeepCount").SetEditorDisplayName("Change In Peep Count").SetEditorTooltip("This increase or decreases the unassigned peeps on all destinations of a given color.  It does not allow removing ones that are already assigned to cars!").DontSetValueOnApply()).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("Which group should we change to?").SetMinimumValue(0).SetMaximumValue(5));
			this.CreateDevToolWithName<MotorwaysDevTool>("SetPinCountOnDestination").SetEditorDisplayName("Set Pin Count on Destination").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnDefaultActionInput().SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int pinCount;
				if (command.TryGetIntParameter("pinCount", out pinCount))
				{
					command.SetPinCountOnDestination(pinCount);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("pinCount").SetEditorDisplayName("Pin Count").SetEditorTooltip("The number of pins to set on a destination. If it removes pins, it does not allow removing ones that are already assigned to cars!").DontSetValueOnApply().SetMinimumValue(0).SetMaximumValue(15));
			this.CreateDevToolWithName<MotorwaysDevTool>("IncreaseGlobalPeepCount").SetEditorDisplayName("Increase Global Peep Count").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnEditorButton("Apply").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int targetGroupIndex = -1;
				int deltaPeepCount;
				if (command.TryGetIntParameter("deltaPeepCount", out deltaPeepCount))
				{
					command.ChangePeepCount(deltaPeepCount, targetGroupIndex);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("deltaPeepCount").SetEditorDisplayName("Number Of Peeps To Add").SetEditorTooltip("This increases the unassigned peeps on all destinations of all colors.").SetDefaultValueForHotkey(1)).ActivateOnInGameHotkey(KeyCode.D, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("DecreaseGlobalPeepCount").SetEditorDisplayName("Decrease Global Peep Count").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").ActivateOnEditorButton("Apply").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int targetGroupIndex = -1;
				int deltaPeepCount;
				if (command.TryGetIntParameter("deltaPeepCount", out deltaPeepCount))
				{
					command.ChangePeepCount(-Mathf.Abs(deltaPeepCount), targetGroupIndex);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("deltaPeepCount").SetEditorDisplayName("Number Of Peeps To Remove").SetEditorTooltip("This decreases the unassigned peeps on all destinations of all colors.  It does not allow removing ones that are already assigned to cars!").SetDefaultValueForHotkey(-1)).ActivateOnInGameHotkey(KeyCode.C, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("AddScore").SetEditorDisplayName("Add Score").SetEditorIconPath("Assets/Art/Pin/Small/SPR_Pin_Small_BakedCol.png").ActivateOnEditorButton("Add").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int scoreDelta;
				if (command.TryGetIntParameter("scoreDelta", out scoreDelta))
				{
					ScoreModel scoreModel = simulation.GetModel<ScoreModel>();
					for (int score = 0; score < scoreDelta; score++)
					{
						scoreModel.AddScore();
					}
					this._hotkeyDebugView.ShowMessage(string.Format("Added {0} points ", scoreDelta));
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("scoreDelta").SetEditorDisplayName("Score to add").SetDefaultValueForHotkey(100)).ActivateOnInGameHotkey(KeyCode.Equals, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ShowUpgradeScreen").SetEditorDisplayName("Show Upgrade Screen").SetEditorIconPath("Assets/Art/Pin/Small/SPR_Pin_Small_BakedCol.png").ActivateOnEditorButton("Show").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				simulation.Scope.Get<UpgradeAwardingProcess>().GrantUpgradeChoice(1);
			}).ActivateOnInGameHotkey(KeyCode.Backslash, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("EndGame").SetEditorDisplayName("Force Game Over").SetEditorIconPath("Assets/Art/Pin/Small/SPR_Pin_Small_BakedCol.png").ActivateOnEditorButton("Game Over").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				simulation.GetModel<DestinationModel>().OnOvercrowded();
			}).ActivateOnInGameHotkey(KeyCode.R, KeyCode.None);
			this.CreateModelDevToolWithName<DestinationDevTool>("DestinationPeepCountChange").SetEditorDisplayName("Destination Change Peep Count").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").SetModelCommandToExecute(delegate(MotorwaysModelDevToolCommand command, DestinationModel destinationModel, ISimulation simulation)
			{
				if (!destinationModel.isActive)
				{
					return;
				}
				int totalPeepCount;
				if (command.TryGetIntParameter("totalPeepCount", out totalPeepCount))
				{
					if (totalPeepCount > destinationModel.TotalDemand)
					{
						int demand = Mathf.Min(totalPeepCount, command.Scope.Get<City>().Rules.GetMaximumDemandForDestination(destinationModel));
						while (demand > destinationModel.TotalDemand)
						{
							destinationModel.unassignedDemand.Add(destinationModel.GroupIndex);
						}
						return;
					}
					int demand2 = Mathf.Max(totalPeepCount, 0);
					while (demand2 < destinationModel.unassignedDemand.Count)
					{
						destinationModel.unassignedDemand.RemoveAt(destinationModel.unassignedDemand.Count - 1);
					}
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("totalPeepCount", "TotalDemand").SetEditorDisplayName("Peep Count").SetEditorTooltip("This increase or decreases the unassigned peeps on a destination.  It does not allow removing ones that are already assigned to cars!").DontSetValueOnApply());
			this.CreateModelDevToolWithName<DestinationDevTool>("DestinationAddSecondDestination").SetEditorDisplayName("Add Second Destination (Only Works On Double Destinations)").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").SetModelCommandToExecute(delegate(MotorwaysModelDevToolCommand command, DestinationModel destinationModel, ISimulation simulation)
			{
				if (!Diagnostics.Verify(destinationModel.isActive) || !Diagnostics.Verify(destinationModel.Carpark.SupportsTwoDestinations, "The selected destination is a single destination!  Pick a double destination.") || !Diagnostics.Verify(destinationModel.Carpark.ActiveDestinationCount < 2, "This double destination already has two destinations on it!"))
				{
					return;
				}
				BuildingSpawningProcess buildingSpawningProcess = command.Scope.Get<BuildingSpawningProcess>();
				CityPlanModel.ScheduledBuilding newBuilding = command.Scope.Get<CityPlanModel.ScheduledBuilding>();
				if (command.TryGetIntParameter("groupIndex", out newBuilding.groupIndex))
				{
					newBuilding.time = command.Scope.Get<ClockModel>().ExpansionTime;
					newBuilding.type = CityTileType.Demand;
					newBuilding.grouping = GroupingStyle.Normal;
					newBuilding.demandMultiplier = Fix64.One;
					buildingSpawningProcess.AddBuildingToDoubleCarpark(simulation, newBuilding, destinationModel.Carpark, DestinationModel.DestinationType.Destination);
				}
			}).WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("The group index you want the new building to be set to!").SetMinimumValue(0).SetMaximumValue(5));
			this.CreateModelDevToolWithName<DestinationDevTool>("DestinationInspectData").SetEditorDisplayName("Destination Data Inspector").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").OverrideDrawEditorToolFunction(delegate(DestinationDevTool tool)
			{
				tool.DrawBaseEditorTool(tool);
			}).DrawOnTilesUnderCursor(delegate(DestinationDevTool tool, Vector2Int position, DebugTileDataViewer debugTileDataViewer)
			{
				if (tool.SelectedModel != null)
				{
					debugTileDataViewer.onlyDrawWhenSelected = false;
					debugTileDataViewer.textSize = 20;
					debugTileDataViewer.stringData.Clear();
					float requirement = (float)tool.SelectedModel.RequiredSupply;
					string data = string.Format("{0:F2}", requirement);
					if (debugTileDataViewer.stringData.ContainsKey(tool.SelectedModel.Carpark.TopLeftWorldCoordinate))
					{
						debugTileDataViewer.stringData[tool.SelectedModel.Carpark.TopLeftWorldCoordinate] = data;
					}
					else
					{
						debugTileDataViewer.stringData.Add(tool.SelectedModel.Carpark.TopLeftWorldCoordinate, data);
					}
					int relevantDestinationCount = 0;
					ModelListEnumerator<DestinationModel> enumerator2 = tool.gameScope.Get<Simulation>().GetModels<DestinationModel>().GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.GroupIndex == tool.SelectedModel.GroupIndex)
						{
							relevantDestinationCount++;
						}
					}
					DemandModel demand = tool.gameScope.Get<DemandModel>();
					foreach (HouseModel house in tool.gameScope.Get<Simulation>().GetModels<HouseModel>())
					{
						if (house.GroupIndex == tool.SelectedModel.GroupIndex)
						{
							float value = (float)demand.CalculateSupplyContributionFromHouseToDestination(house, tool.SelectedModel) / (float)relevantDestinationCount;
							data = string.Format("{0:F2}", value);
							if (!debugTileDataViewer.stringData.ContainsKey(house.tileModel.Coordinates))
							{
								debugTileDataViewer.stringData.Add(house.tileModel.Coordinates, data);
							}
							else
							{
								debugTileDataViewer.stringData[house.tileModel.Coordinates] = data;
							}
						}
					}
				}
			}).SetOnModelSelectedCommandToExecute(delegate(MotorwaysModelDevTool<DestinationModel, DestinationDevTool> tool, DestinationModel model)
			{
				tool.OnToolDeselected();
			});
			this.CreateModelDevToolWithName<HouseDevTool>("HouseInspectData").SetEditorDisplayName("House Data Inspector").SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png").OverrideDrawEditorToolFunction(delegate(HouseDevTool tool)
			{
				tool.DrawBaseEditorTool(tool);
			}).DrawOnTilesUnderCursor(delegate(HouseDevTool tool, Vector2Int position, DebugTileDataViewer debugTileDataViewer)
			{
				if (tool.SelectedModel != null)
				{
					debugTileDataViewer.onlyDrawWhenSelected = false;
					debugTileDataViewer.textSize = 20;
					debugTileDataViewer.stringData.Clear();
					int relevantDestinationCount = 0;
					float totalContributions = 0f;
					ModelListEnumerator<DestinationModel> enumerator2 = tool.gameScope.Get<Simulation>().GetModels<DestinationModel>().GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.GroupIndex == tool.SelectedModel.GroupIndex)
						{
							relevantDestinationCount++;
						}
					}
					string data;
					foreach (DestinationModel destination in tool.gameScope.Get<Simulation>().GetModels<DestinationModel>())
					{
						if (destination.GroupIndex == tool.SelectedModel.GroupIndex)
						{
							DemandModel demandModel = tool.gameScope.Get<DemandModel>();
							float value = (float)(demandModel.CalculateSupplyContributionFromHouseToDestination(tool.SelectedModel, destination) * demandModel.GetSupplyScale(destination.GroupIndex));
							float requirement = (float)destination.RequiredSupply;
							totalContributions += value;
							data = string.Format("Add\n{0:F2} of\n{1:F2}", value, requirement);
							if (debugTileDataViewer.stringData.ContainsKey(destination.Carpark.TopLeftWorldCoordinate))
							{
								debugTileDataViewer.stringData[destination.Carpark.TopLeftWorldCoordinate] = data;
							}
							else
							{
								debugTileDataViewer.stringData.Add(destination.Carpark.TopLeftWorldCoordinate, data);
							}
						}
					}
					data = string.Format("Total\n{0:F2}", totalContributions);
					if (debugTileDataViewer.stringData.ContainsKey(tool.SelectedModel.tileModel.Coordinates))
					{
						debugTileDataViewer.stringData[tool.SelectedModel.tileModel.Coordinates] = data;
						return;
					}
					debugTileDataViewer.stringData.Add(tool.SelectedModel.tileModel.Coordinates, data);
				}
			}).SetOnModelSelectedCommandToExecute(delegate(MotorwaysModelDevTool<HouseModel, HouseDevTool> tool, HouseModel model)
			{
				tool.OnToolDeselected();
			});
			this.CreateDevToolWithName<MotorwaysDevTool>("SkipAheadTime").SetEditorDisplayName("Skip Ahead Time").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_FastForward.png").ActivateOnEditorButton("Skip!").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				Fix64 skipAheadDurationHours;
				bool atLeastOne = command.TryGetFloatParameter("skipAheadDurationHours", out skipAheadDurationHours);
				Fix64 skipAheadDurationDays;
				atLeastOne = (command.TryGetFloatParameter("skipAheadDurationDays", out skipAheadDurationDays) || atLeastOne);
				Fix64 skipAheadDurationWeeks;
				atLeastOne = (command.TryGetFloatParameter("skipAheadDurationWeeks", out skipAheadDurationWeeks) || atLeastOne);
				if (atLeastOne)
				{
					Fix64 multiplier = (Fix64)0.8333333333333334;
					Fix64 skipAheadTotal = skipAheadDurationHours * multiplier;
					multiplier *= (Fix64)24f;
					skipAheadTotal += skipAheadDurationDays * multiplier;
					multiplier *= (Fix64)7f;
					skipAheadTotal += skipAheadDurationWeeks * multiplier;
					if (Diagnostics.Verify(skipAheadTotal >= Fix64.Zero))
					{
						Game gameInstance = command.Scope.Get<Game>();
						bool shouldUnpause;
						if (gameInstance != null && command.TryGetBoolParameter("unpauseGame", out shouldUnpause))
						{
							if (simulation.IsPaused && shouldUnpause)
							{
								simulation.IsPaused = false;
							}
							gameInstance.AddArbitraryAccumulatedTime(skipAheadTotal);
						}
					}
				}
			}).WithFloatParam(IngameDevToolFloatParameter.DefineFloatParameter("skipAheadDurationHours").SetEditorDisplayName("Hours To Skip Ahead").SetEditorTooltip("This is in hours.").SetDefaultValueForHotkey(Fix64Consts.Zero)).WithFloatParam(IngameDevToolFloatParameter.DefineFloatParameter("skipAheadDurationDays").SetEditorDisplayName("Days To Skip Ahead").SetEditorTooltip("This is in days.").SetDefaultValueForHotkey(Fix64Consts.One)).WithFloatParam(IngameDevToolFloatParameter.DefineFloatParameter("skipAheadDurationWeeks").SetEditorDisplayName("Weeks To Skip Ahead").SetEditorTooltip("This is in weeks.").SetDefaultValueForHotkey(Fix64Consts.Zero)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("unpauseGame").SetEditorDisplayName("Unpause Game If Needed").SetEditorTooltip("This tool only works if the game is unpaused.  When this is checked the game will automatically be unpaused immediately before skipping ahead.").SetValue(true).SetDefaultValueForHotkey(true)).ActivateOnInGameHotkey(KeyCode.F, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("SkipAheadExpansionTime").SetEditorDisplayName("Skip Ahead Expansion Time").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_FastForward.png").ActivateOnEditorButton("Skip!").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				Fix64 skipAheadDurationHours;
				bool atLeastOneAvailable = command.TryGetFloatParameter("skipAheadDurationHours", out skipAheadDurationHours);
				Fix64 skipAheadDurationDays;
				atLeastOneAvailable = (command.TryGetFloatParameter("skipAheadDurationDays", out skipAheadDurationDays) || atLeastOneAvailable);
				Fix64 skipAheadDurationWeeks;
				atLeastOneAvailable = (command.TryGetFloatParameter("skipAheadDurationWeeks", out skipAheadDurationWeeks) || atLeastOneAvailable);
				if (atLeastOneAvailable)
				{
					Fix64 multiplier = (Fix64)0.8333333333333334;
					Fix64 skipAheadTotal = skipAheadDurationHours * multiplier;
					multiplier *= (Fix64)24f;
					skipAheadTotal += skipAheadDurationDays * multiplier;
					multiplier *= (Fix64)7f;
					skipAheadTotal += skipAheadDurationWeeks * multiplier;
					if (Diagnostics.Verify(skipAheadTotal >= Fix64.Zero))
					{
						simulation.GetModel<ClockModel>().CurrentFrame.expansionTime += skipAheadTotal;
						simulation.GetModel<ClockModel>().NextFrame.expansionTime += skipAheadTotal;
						UpgradeDatabaseModel upgradeDatabaseModel = command.Scope.Get<UpgradeDatabaseModel>();
						if (upgradeDatabaseModel.upgradeSchedulePaused)
						{
							upgradeDatabaseModel.accumulatedUpgradeScheduleDelayTime += skipAheadTotal;
						}
						CityPlanModel cityPlanModel = command.Scope.Get<CityPlanModel>();
						if (cityPlanModel != null && cityPlanModel.scheduledBuildings.Count > 0)
						{
							cityPlanModel.scheduledBuildings[0].time += skipAheadTotal;
						}
					}
				}
			}).WithFloatParam(IngameDevToolFloatParameter.DefineFloatParameter("skipAheadDurationHours").SetEditorDisplayName("Hours To Skip Ahead").SetEditorTooltip("This is in hours.").SetDefaultValueForHotkey(Fix64Consts.Zero)).WithFloatParam(IngameDevToolFloatParameter.DefineFloatParameter("skipAheadDurationDays").SetEditorDisplayName("Days To Skip Ahead").SetEditorTooltip("This is in days.").SetDefaultValueForHotkey(Fix64Consts.Zero)).WithFloatParam(IngameDevToolFloatParameter.DefineFloatParameter("skipAheadDurationWeeks").SetEditorDisplayName("Weeks To Skip Ahead").SetEditorTooltip("This is in weeks.").SetDefaultValueForHotkey((Fix64)3L)).ActivateOnInGameHotkey(KeyCode.E, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleUpgrades").SetEditorDisplayName("Toggle Recurring Upgrades").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_Pause.png").ActivateOnEditorButton("Toggle Recurring Upgrades (e.g. Weekly/Milestones)").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				UpgradeDatabaseModel upgradeDatabaseModel = command.Scope.Get<UpgradeDatabaseModel>();
				upgradeDatabaseModel.upgradeSchedulePaused = !upgradeDatabaseModel.upgradeSchedulePaused;
				this._hotkeyDebugView.ShowMessage("Recurring Upgrades: " + (upgradeDatabaseModel.upgradeSchedulePaused ? "OFF" : "ON"));
			}).ActivateOnInGameHotkey(KeyCode.U, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ResetAllCars").SetEditorDisplayName("Reset All Cars To Houses").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_Car.png").ActivateOnEditorButton("Reset").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				foreach (VehicleModel vehicleModel in simulation.GetModels<VehicleModel>())
				{
					vehicleModel.ResetToHouse();
				}
			});
			this.CreateDevToolWithName<MotorwaysDevTool>("TogglePinVisibility").SetEditorDisplayName("Toggle pin visibility").SetEditorIconPath("Assets/Art/Pin/Small/SPR_Pin_Small_BakedCol.png").ActivateOnEditorButton("Toggle").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool isShowingPins = false;
				bool hasFlippedDecision = false;
				foreach (DestinationView destinationView in simulation.Scope.Get<ViewClient>().GetViews<DestinationView>())
				{
					if (!hasFlippedDecision && !destinationView.IsShowingPins)
					{
						isShowingPins = true;
						hasFlippedDecision = true;
					}
					destinationView.SetPinViewVisible(isShowingPins);
				}
				this._hotkeyDebugView.ShowMessage("Pin Visibility: " + (isShowingPins ? "ON" : "OFF"));
			}).ActivateOnInGameHotkey(KeyCode.Semicolon, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("SetDrawToggleVisiblity").SetEditorDisplayName("Set Draw Toggle Visibility").SetEditorIconPath("Assets/Art/PlatformResources/AppleMFIController/SPR_ControllerDraw.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool drawButtonsHidden;
				if (command.TryGetBoolParameter("drawButtonsHidden", out drawButtonsHidden))
				{
					GameUIScreen gameUIScreen = UnityEngine.Object.FindObjectOfType<GameUIScreen>();
					gameUIScreen.SetDrawButtonsHiddenByTutorial(drawButtonsHidden);
					gameUIScreen.SetDrawButtonsVisible(!drawButtonsHidden);
				}
			}).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("drawButtonsHidden").SetEditorTooltip("Hide the draw toggle buttons.").SetEditorDisplayName("Hidden").SetValue(false)).ActivateOnEditorButton("Apply");
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleHUDVisibility").SetEditorDisplayName("Toggle HUD Visibility").SetEditorIconPath("Assets/Art/PlatformResources/AppleMFIController/SPR_ControllerDraw.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				GameUIScreen screen = UnityEngine.Object.FindObjectOfType<GameUIScreen>();
				this._hotkeyDebugView.ShowMessage("HUD: " + ((!screen.DebugToolsHideUI) ? "OFF" : "ON"));
				screen.DebugToolsHideUI = !screen.DebugToolsHideUI;
			}).ActivateOnEditorButton("Toggle HUD Visibility").ActivateOnInGameHotkey(KeyCode.I, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleWorldGridVisibility").SetEditorDisplayName("Toggle WorldGrid Visibility").SetEditorIconPath("Assets/Art/PlatformResources/AppleMFIController/SPR_ControllerDraw.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				GameUIScreen screen = UnityEngine.Object.FindObjectOfType<GameUIScreen>();
				this._hotkeyDebugView.ShowMessage("World Grid: " + ((!screen.DebugToolsHideWorldGrid) ? "OFF" : "ON"));
				screen.DebugToolsHideWorldGrid = !screen.DebugToolsHideWorldGrid;
			}).ActivateOnEditorButton("Toggle WorldGrid Visibility").ActivateOnInGameHotkey(KeyCode.O, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("SetHudAndWorldGridVisibility").SetEditorDisplayName("Set HUD & World Grid Visibility").SetEditorIconPath("Assets/Art/PlatformResources/AppleMFIController/SPR_ControllerDraw.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool hudHidden;
				bool worldGridHidden;
				if (command.TryGetBoolParameter("hudHidden", out hudHidden) && command.TryGetBoolParameter("worldGridHidden", out worldGridHidden))
				{
					GameUIScreen gameUIScreen = UnityEngine.Object.FindObjectOfType<GameUIScreen>();
					gameUIScreen.DebugToolsHideWorldGrid = worldGridHidden;
					gameUIScreen.DebugToolsHideUI = hudHidden;
				}
			}).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("hudHidden").SetEditorTooltip("Set the HUD hidden.").SetEditorDisplayName("HUD Hidden").SetDefaultValueForHotkey(true).SetValue(true)).WithBoolParam(IngameDevToolBoolParameter.DefineBoolParameter("worldGridHidden").SetEditorTooltip("Set the world grid hidden.").SetEditorDisplayName("World Grid Hidden").SetDefaultValueForHotkey(true).SetValue(true)).ActivateOnEditorButton("Apply");
			this.CreateDevToolWithName<MotorwaysDevTool>("ForceUpdateTheme").SetEditorDisplayName("Force Update Theme").SetEditorIconPath("Assets/Art/UI/Menus/Pause/SPR_PauseUI_NightOff.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				this._hotkeyDebugView.ShowMessage("Force Update Theme");
			}).ActivateOnEditorButton("Update").ActivateOnInGameHotkey(KeyCode.Slash, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ChangeLocaleForward").SetEditorDisplayName("Change Locale Forward").SetEditorIconPath("Assets/Art/UI/Menus/Pause/SPR_PauseUI_NightOff.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				LocaleDatabase db = simulation.Scope.Get<LocaleDatabase>();
				int nextLocaleIndex = (db.GetIndex(db.CurrentLocale) + 1) % db.LocaleCount;
				LocaleDatabase.LocaleId id = db.GetLocale(nextLocaleIndex).Id;
				this._hotkeyDebugView.ShowMessage(string.Format("Setting Locale: {0}", id));
				simulation.Scope.Get<IActivePlayer>().LocaleId = id;
			}).ActivateOnEditorButton("Previous Locale").ActivateOnInGameHotkey(KeyCode.Period, KeyCode.LeftShift);
			this.CreateDevToolWithName<MotorwaysDevTool>("ChangeLocaleBackward").SetEditorDisplayName("Change Locale Backward").SetEditorIconPath("Assets/Art/UI/Menus/Pause/SPR_PauseUI_NightOff.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				LocaleDatabase db = simulation.Scope.Get<LocaleDatabase>();
				int nextLocaleIndex = db.GetIndex(db.CurrentLocale) - 1;
				if (nextLocaleIndex == -1)
				{
					nextLocaleIndex = db.LocaleCount - 1;
				}
				LocaleDatabase.LocaleId id = db.GetLocale(nextLocaleIndex).Id;
				this._hotkeyDebugView.ShowMessage(string.Format("Setting Locale: {0}", id));
				simulation.Scope.Get<IActivePlayer>().LocaleId = id;
			}).ActivateOnEditorButton("Previous Locale").ActivateOnInGameHotkey(KeyCode.Comma, KeyCode.LeftShift);
			this.CreateDevToolWithName<MotorwaysDevTool>("PauseSimulation").SetEditorDisplayName("Toggle Pause Simulation").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_Pause.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				simulation.Scope.Get<Game>().SetPaused(!simulation.IsPaused);
				string state = simulation.IsPaused ? "paused" : "unpaused";
				this._hotkeyDebugView.ShowMessage("Simulation " + state);
			}).ActivateOnEditorButton("Toggle Paused").ActivateOnInGameHotkey(KeyCode.P, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleGodMode").SetEditorDisplayName("Toggle God Mode").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Sub_Double_In.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				GameBehaviourModel behaviourModel = simulation.GetModel<GameBehaviourModel>();
				behaviourModel.CanGameOver = !behaviourModel.CanGameOver;
				this._hotkeyDebugView.ShowMessage("God mode: " + (behaviourModel.CanGameOver ? "OFF" : "ON"));
				BuildingsIndicatorView disconnectedBuildingsView = simulation.Scope.Get<BuildingsIndicatorView>();
				bool godModeOn = !behaviourModel.CanGameOver;
				if (godModeOn)
				{
					disconnectedBuildingsView.StopPulsing();
				}
				else
				{
					disconnectedBuildingsView.StartPulsing();
				}
				disconnectedBuildingsView.AlertsEnabled = !godModeOn;
			}).ActivateOnEditorButton("Toggle God Mode").ActivateOnInGameHotkey(KeyCode.G, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleDisconnectedBuildingPulsing").SetEditorDisplayName("Toggle Disconnected Building Pulse").SetEditorIconPath("Assets/Art/UI/SPR_UI_Button_home.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				BuildingsIndicatorView disconnectedBuildingsView = simulation.Scope.Get<BuildingsIndicatorView>();
				if (!disconnectedBuildingsView.PulsingEnabled)
				{
					disconnectedBuildingsView.StartPulsing();
					this._hotkeyDebugView.ShowMessage("Building Pulsing: ON");
					return;
				}
				disconnectedBuildingsView.StopPulsing();
				this._hotkeyDebugView.ShowMessage("Building Pulsing: OFF");
			}).ActivateOnEditorButton("Apply").ActivateOnInGameHotkey(KeyCode.Quote, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleVideoCaptureMode").SetEditorDisplayName("Toggle Video Capture Mode").SetEditorIconPath("Assets/Art/UI/Icons/SPR_UI_Camera.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool videoModeOn = !InGameDevToolsRegistry.VideoCaptureModeOn;
				BuildingsIndicatorView disconnectedBuildingsView = simulation.Scope.Get<BuildingsIndicatorView>();
				if (videoModeOn)
				{
					disconnectedBuildingsView.StopPulsing();
				}
				else
				{
					disconnectedBuildingsView.StartPulsing();
				}
				disconnectedBuildingsView.AlertsEnabled = !videoModeOn;
				command.Scope.Get<UpgradeDatabaseModel>().upgradeSchedulePaused = videoModeOn;
				command.SetSpawningMode(videoModeOn ? CityPlanModel.BuildingSpawningMode.None : CityPlanModel.BuildingSpawningMode.All);
				UnityEngine.Object.FindObjectOfType<GameUIScreen>().DebugToolsHideUI = videoModeOn;
				simulation.Scope.Get<NotificationView>().NotificationsEnabled = !videoModeOn;
				InGameDevToolsRegistry.VideoCaptureModeOn = videoModeOn;
			}).ActivateOnEditorButton("Toggle Video Capture Mode").ActivateOnInGameHotkey(KeyCode.M, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("SpeedUp").SetEditorDisplayName("Debug Increase Speed").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Sub_Double_In.png").SetClientCodeToExecute(delegate(MotorwaysDevTool command, ISimulation simulation)
			{
				MotorwaysGame game = simulation.Scope.Get<Game>() as MotorwaysGame;
				if (game.DebugTimescale < 1f)
				{
					game.DebugTimescale = 1f;
				}
				else
				{
					game.DebugTimescale += 1f;
				}
				this._hotkeyDebugView.ShowMessage("Increase Timescale: " + game.DebugTimescale.ToString());
			}).ActivateOnEditorButton("Speed Up").ActivateOnInGameHotkey(KeyCode.Q, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("SlowDown").SetEditorDisplayName("Debug Decrease Speed").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Sub_Half_In.png").SetClientCodeToExecute(delegate(MotorwaysDevTool command, ISimulation simulation)
			{
				MotorwaysGame game = simulation.Scope.Get<Game>() as MotorwaysGame;
				float newTimescale = game.DebugTimescale + -1f;
				if (newTimescale < 1f)
				{
					newTimescale = game.DebugTimescale * 0.75f;
				}
				game.DebugTimescale = newTimescale;
				this._hotkeyDebugView.ShowMessage("Decrease Timescale: " + game.DebugTimescale.ToString());
			}).ActivateOnEditorButton("Slow Down").ActivateOnInGameHotkey(KeyCode.A, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("SetBuildingGroupIndex").SetEditorDisplayName("Set Building Group Index").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Main_DestinationCircle.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				int groupIndex;
				if (command.TryGetIntParameter("groupIndex", out groupIndex))
				{
					command.SetGroupIndex(groupIndex);
				}
			}).ActivateOnDefaultActionInput().WithIntParam(IngameDevToolIntParameter.DefineIntParameter("groupIndex").SetEditorDisplayName("Group Index").SetEditorTooltip("Which group should the destination belong to?").SetMinimumValue(0).SetMaximumValue(5)).ActivateOnInGameHotkeyWithIntParameter(KeyCode.Alpha4, "groupIndex", 0).ActivateOnInGameHotkeyWithIntParameter(KeyCode.Alpha5, "groupIndex", 1).ActivateOnInGameHotkeyWithIntParameter(KeyCode.Alpha6, "groupIndex", 2).ActivateOnInGameHotkeyWithIntParameter(KeyCode.Alpha7, "groupIndex", 3).ActivateOnInGameHotkeyWithIntParameter(KeyCode.Alpha8, "groupIndex", 4).ActivateOnInGameHotkeyWithIntParameter(KeyCode.Alpha9, "groupIndex", 5);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleSandboxMode").SetEditorDisplayName("Toggle Sandbox Mode").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Sub_Star_In.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				bool sandboxMode = !InGameDevToolsRegistry.SandboxModeOn;
				command.Scope.Get<UpgradeDatabaseModel>().upgradeSchedulePaused = sandboxMode;
				command.SetSpawningMode(sandboxMode ? CityPlanModel.BuildingSpawningMode.None : CityPlanModel.BuildingSpawningMode.All);
				simulation.GetModel<GameBehaviourModel>().CanGameOver = !sandboxMode;
				InGameDevToolsRegistry.SandboxModeOn = sandboxMode;
				this._hotkeyDebugView.ShowMessage("Sandbox mode: " + (InGameDevToolsRegistry.SandboxModeOn ? "ON" : "OFF"));
			}).ActivateOnEditorButton("Toggle Sandbox Mode").ActivateOnInGameHotkey(KeyCode.N, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleDebugCameraControls").SetEditorDisplayName("Toggle Debug Camera Controls").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Sub_Star_In.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				CameraView view = command.Scope.Get<CameraView>();
				view.HasControlOverriden = !view.HasControlOverriden;
				this._hotkeyDebugView.ShowMessage("Debug Camera: " + (view.HasControlOverriden ? "ON" : "OFF"));
			}).ActivateOnEditorButton("Toggle Debug Camera Controls").ActivateOnInGameHotkey(KeyCode.B, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("CinematicMode").SetEditorDisplayName("Toggle Cinematic Mode").SetEditorIconPath("Assets/Art/UI/GifMode/SPR_GifUI_Gif.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				CameraView view = command.Scope.Get<CameraView>();
				if (view.IsInCinematicMode)
				{
					view.ExitCinematicMode();
					return;
				}
				view.EnterCinematicMode();
				view.GoToNextAgentInCinematicMode();
			}).ActivateOnEditorButton("Toggle Cinematic Mode").ActivateOnInGameHotkey(KeyCode.J, KeyCode.None);
			this.CreateDevToolWithName<MotorwaysDevTool>("CinematicModeNextAgent").SetEditorDisplayName("Cinematic Mode Next Agent").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Main_FFWD.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				command.Scope.Get<CameraView>().GoToNextAgentInCinematicMode();
				this._hotkeyDebugView.ShowMessage("Cinematic Mode - Next Agent");
			}).ActivateOnEditorButton("Next Agent").ActivateOnInGameHotkey(KeyCode.J, KeyCode.LeftShift);
			List<HotkeyDescription> hotkeyDescriptions = new List<HotkeyDescription>();
			foreach (IInGameDevTool inGameDevTool in this._allTools)
			{
				KeyCode keyCode = inGameDevTool.GetHotKey();
				string editorToolDisplayName = inGameDevTool.GetEditorToolDisplayNameWithoutHotkeyCode();
				if (keyCode != KeyCode.None && !editorToolDisplayName.Contains("Toggle Sandbox Mode"))
				{
					hotkeyDescriptions.Add(new HotkeyDescription(keyCode, inGameDevTool.GetModifierHotKey(), editorToolDisplayName));
				}
			}
			hotkeyDescriptions.Add(new HotkeyDescription(KeyCode.V, "Toggle Hotkey Help"));
			hotkeyDescriptions.Sort((HotkeyDescription descriptionA, HotkeyDescription descriptionB) => string.Compare(descriptionA.description, descriptionB.description, StringComparison.Ordinal));
			this.CreateDevToolWithName<MotorwaysDevTool>("ToggleHotkeyHelp").SetEditorDisplayName("Toggle Hotkey Help").SetEditorIconPath("Assets/Art/UI/ChallengeMode/ChallengeIcons/MO_Challenge_Sub_Star_In.png").SetCommandToExecute(delegate(MotorwaysDevToolCommand command, ISimulation simulation)
			{
				if (this._hotkeyDebugView.IsShowingHotkeyDescriptions)
				{
					this._hotkeyDebugView.HideHotkeyDescriptions();
					return;
				}
				this._hotkeyDebugView.ShowHotkeyDescriptions(hotkeyDescriptions);
			}).ActivateOnEditorButton("Toggle Hotkey Help").ActivateOnInGameHotkey(KeyCode.V, KeyCode.None);
			foreach (IInGameDevTool newTool in this._allTools)
			{
				if (newTool.GetModifierHotKey() != KeyCode.None)
				{
					this._modifierKeys.Add(newTool.GetModifierHotKey());
				}
			}
		}
	}

	// Token: 0x040000D3 RID: 211
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("InGameDevTools");

	// Token: 0x040000D4 RID: 212
	private const int MaxGroupIndex = 5;

	// Token: 0x040000D5 RID: 213
	private const KeyCode ClearToolsToNoneKeyCode = KeyCode.Z;

	// Token: 0x040000D6 RID: 214
	[Dependency]
	private IScope _scope;

	// Token: 0x040000D7 RID: 215
	[Dependency]
	private HotkeyDebugView _hotkeyDebugView;

	// Token: 0x040000D8 RID: 216
	private List<IInGameDevTool> _allTools = new List<IInGameDevTool>();

	// Token: 0x040000D9 RID: 217
	private HashSet<KeyCode> _modifierKeys = new HashSet<KeyCode>();

	// Token: 0x040000DA RID: 218
	private Dictionary<ToolModelType, MotorwaysModelContainerTool> _modelContainerTools = new Dictionary<ToolModelType, MotorwaysModelContainerTool>();

	// Token: 0x040000DB RID: 219
	private List<Action<string>> _onToolsChanged = new List<Action<string>>();

	// Token: 0x040000DC RID: 220
	private static bool VideoCaptureModeOn = false;

	// Token: 0x040000DD RID: 221
	public static bool SandboxModeOn = false;
}
