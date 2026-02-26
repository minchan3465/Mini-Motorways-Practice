using System;
using Motorways;
using Motorways.Models;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class DestinationDevTool : MotorwaysModelDevTool<DestinationModel, DestinationDevTool>
{
	// Token: 0x06000227 RID: 551 RVA: 0x00007C22 File Offset: 0x00005E22
	public DestinationDevTool()
	{
		this._toolModelType = ToolModelType.Destination;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00007C34 File Offset: 0x00005E34
	protected override bool TryGetModelAtCoordinates(Vector2Int modelCoordinates, out DestinationModel foundModel)
	{
		foundModel = null;
		Tile activeTile = this.gameScope.Get<TilemapModel>().GetTile(modelCoordinates);
		if (activeTile != null && activeTile.ContentType == TileContentType.Destination)
		{
			foundModel = (DestinationModel)activeTile.ContentModel;
			return true;
		}
		if (activeTile != null && activeTile.ContentType == TileContentType.Carpark)
		{
			foundModel = ((CarparkModel)activeTile.ContentModel).destinations[0];
			return true;
		}
		return false;
	}
}
