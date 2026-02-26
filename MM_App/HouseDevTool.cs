using System;
using Motorways;
using Motorways.Models;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class HouseDevTool : MotorwaysModelDevTool<HouseModel, HouseDevTool>
{
	// Token: 0x06000225 RID: 549 RVA: 0x00007BD4 File Offset: 0x00005DD4
	public HouseDevTool()
	{
		this._toolModelType = ToolModelType.House;
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00007BE4 File Offset: 0x00005DE4
	protected override bool TryGetModelAtCoordinates(Vector2Int modelCoordinates, out HouseModel foundModel)
	{
		foundModel = null;
		Tile activeTile = this.gameScope.Get<TilemapModel>().GetTile(modelCoordinates);
		if (activeTile != null && activeTile.ContentType == TileContentType.House)
		{
			foundModel = (HouseModel)activeTile.ContentModel;
			return true;
		}
		return false;
	}
}
