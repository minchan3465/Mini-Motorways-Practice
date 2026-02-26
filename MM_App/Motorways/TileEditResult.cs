using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200040F RID: 1039
	public struct TileEditResult
	{
		// Token: 0x06001975 RID: 6517 RVA: 0x0005AA30 File Offset: 0x00058C30
		public static TileEditResult InvalidTileCoordinate(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.InvalidTileCoordinate,
				errorPosition = position
			};
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x0005AA58 File Offset: 0x00058C58
		public static TileEditResult CannotConnectToCarpark(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotConnectToCarpark,
				errorPosition = position
			};
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x0005AA80 File Offset: 0x00058C80
		public static TileEditResult CannotConnectHouseToBridge(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotConnectHouseToBridge,
				errorPosition = position
			};
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x0005AAA8 File Offset: 0x00058CA8
		public static TileEditResult CannotConnectHouseToTunnel(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotConnectHouseToTunnel,
				errorPosition = position
			};
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x0005AAD0 File Offset: 0x00058CD0
		public static TileEditResult CannotConnectHouseToRail(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotConnectHouseToRail,
				errorPosition = position
			};
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x0005AAF8 File Offset: 0x00058CF8
		public static TileEditResult CannotCreateBridge(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotCreateBridge,
				errorPosition = position
			};
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x0005AB20 File Offset: 0x00058D20
		public static TileEditResult CannotCreateTunnel(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotCreateTunnel,
				errorPosition = position
			};
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x0005AB48 File Offset: 0x00058D48
		public static TileEditResult CannotCreateCrossing(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.CannotCreateCrossing,
				errorPosition = position
			};
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x0005AB70 File Offset: 0x00058D70
		public static TileEditResult NotEnoughConcrete(Vector2Int position)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.NotEnoughConcrete,
				errorPosition = position
			};
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x0600197E RID: 6526 RVA: 0x0005AB96 File Offset: 0x00058D96
		public bool IsSuccessful
		{
			get
			{
				return this.resultCode == TileEditResultCode.Success;
			}
		}

		// Token: 0x04001576 RID: 5494
		public TileEditResultCode resultCode;

		// Token: 0x04001577 RID: 5495
		public TileEdit edit;

		// Token: 0x04001578 RID: 5496
		public Vector2Int errorPosition;

		// Token: 0x04001579 RID: 5497
		public static TileEditResult Success = new TileEditResult
		{
			resultCode = TileEditResultCode.Success
		};

		// Token: 0x0400157A RID: 5498
		public static TileEditResult NotEnoughUpgrades = new TileEditResult
		{
			resultCode = TileEditResultCode.NotEnoughUpgrades
		};

		// Token: 0x0400157B RID: 5499
		public static TileEditResult MotorwayTooShort = new TileEditResult
		{
			resultCode = TileEditResultCode.MotorwayTooShort
		};
	}
}
