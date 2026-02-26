using System;

namespace Motorways
{
	// Token: 0x0200043C RID: 1084
	public static class TileDirectionExtensions
	{
		// Token: 0x06001AE3 RID: 6883 RVA: 0x00062AF0 File Offset: 0x00060CF0
		public static string ToShortString(this TileDirection direction)
		{
			switch (direction)
			{
			case TileDirection.North:
				return "N";
			case TileDirection.NorthEast:
				return "NE";
			case TileDirection.East:
				return "E";
			case TileDirection.SouthEast:
				return "SE";
			case TileDirection.South:
				return "S";
			case TileDirection.SouthWest:
				return "SW";
			case TileDirection.West:
				return "W";
			case TileDirection.NorthWest:
				return "NW";
			default:
				return direction.ToString();
			}
		}
	}
}
