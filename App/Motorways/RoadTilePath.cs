using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200042A RID: 1066
	[Factory.Serializable(1)]
	public class RoadTilePath : IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x06001A3F RID: 6719 RVA: 0x0005F6A6 File Offset: 0x0005D8A6
		public List<Vector2Fixed> GetVisualPoints()
		{
			return this.GetVisualPoints(Vector2Fixed.zero);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x0005F6B4 File Offset: 0x0005D8B4
		public List<Vector2Fixed> GetVisualPoints(Vector2Fixed offset)
		{
			List<Vector2Fixed> points = new List<Vector2Fixed>();
			foreach (RoadTilePath.Piece piece in this.pathPieces)
			{
				if (piece.visualPoints != null)
				{
					foreach (Vector2Fixed point in piece.visualPoints)
					{
						points.Add(point + offset);
					}
				}
			}
			return points;
		}

		// Token: 0x06001A41 RID: 6721 RVA: 0x0005F75C File Offset: 0x0005D95C
		public List<Vector2Fixed> GetLogicalPoints()
		{
			return this.GetLogicalPoints(Vector2Fixed.zero);
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x0005F76C File Offset: 0x0005D96C
		public List<Vector2Fixed> GetLogicalPoints(Vector2Fixed offset)
		{
			List<Vector2Fixed> points = new List<Vector2Fixed>();
			foreach (RoadTilePath.Piece piece in this.pathPieces)
			{
				foreach (Vector2Fixed point in piece.logicalPoints)
				{
					points.Add(point + offset);
				}
			}
			return points;
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0005F808 File Offset: 0x0005DA08
		public Fix64 Length
		{
			get
			{
				if (this._length < Fix64.Zero)
				{
					this._length = Fix64.Zero;
					foreach (RoadTilePath.Piece piece in this.pathPieces)
					{
						List<Vector2Fixed> points = piece.logicalPoints;
						for (int pointIndex = 0; pointIndex < points.Count - 1; pointIndex++)
						{
							this._length += Vector2Fixed.Distance(points[pointIndex], points[pointIndex + 1]);
						}
					}
				}
				return this._length;
			}
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x0005F8B8 File Offset: 0x0005DAB8
		public List<Vector2> ConstructPointsForPolygon(float roadScale, bool roundEnds, float edgePointScale)
		{
			List<Vector2> points = new List<Vector2>();
			List<Vector2Fixed> midPoints = this.GetVisualPoints();
			for (int side = 0; side < 2; side++)
			{
				for (int pointIndex = 0; pointIndex < midPoints.Count; pointIndex++)
				{
					Vector2 midPoint = new Vector2((float)midPoints[pointIndex].x, (float)midPoints[pointIndex].y) * ((pointIndex == 0 || pointIndex == midPoints.Count - 1) ? edgePointScale : 1f);
					Vector2 forward = Vector2.zero;
					if (!roundEnds && pointIndex == 0)
					{
						forward = TileUtilities.GetVectorForDirection(TileUtilities.GetClosestDirection((Vector2)(-midPoints[0])));
					}
					else if (!roundEnds && pointIndex == midPoints.Count - 1)
					{
						forward = TileUtilities.GetVectorForDirection(TileUtilities.GetClosestDirection((Vector2)midPoints[midPoints.Count - 1]));
					}
					else
					{
						if (pointIndex < midPoints.Count - 1)
						{
							Vector2 nextPoint = new Vector2((float)midPoints[pointIndex + 1].x, (float)midPoints[pointIndex + 1].y);
							forward += nextPoint - midPoint;
						}
						if (pointIndex > 0)
						{
							Vector2 prevPoint = new Vector2((float)midPoints[pointIndex - 1].x, (float)midPoints[pointIndex - 1].y);
							forward += midPoint - prevPoint;
						}
					}
					forward.Normalize();
					Vector2 left = new Vector2(-forward.y, forward.x);
					points.Add(midPoint + left * roadScale);
					Vector2 endPos = midPoint;
					if (pointIndex == midPoints.Count - 1 && roundEnds && side == 0)
					{
						float angleStep = 15f;
						for (int capIndex = 1; capIndex <= 12; capIndex++)
						{
							Vector2 pos = Quaternion.Euler(0f, 0f, -angleStep * (float)capIndex) * left;
							points.Add(endPos + pos * roadScale);
						}
					}
				}
				midPoints.Reverse();
			}
			points.Add(points[0]);
			return points;
		}

		// Token: 0x06001A45 RID: 6725 RVA: 0x0005FAE8 File Offset: 0x0005DCE8
		public RoadTilePath CreateRotatedPath(RoadTileRotation rotation)
		{
			RoadTilePath rotatedTilePath = this._scope.Get<RoadTilePath>();
			foreach (RoadTilePath.Piece piece in this.pathPieces)
			{
				RoadTilePath.Piece rotatedPiece = piece.GetRotatedPiece(this._scope, rotation);
				rotatedTilePath.pathPieces.Add(rotatedPiece);
			}
			return rotatedTilePath;
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x0005FB5C File Offset: 0x0005DD5C
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (RoadTilePath.Piece piece in this.pathPieces)
			{
				scope.Release(piece);
			}
			this.pathPieces.Clear();
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x0005FBBC File Offset: 0x0005DDBC
		public void Reset()
		{
			this._length = -Fix64.One;
			this.pathPieces.Clear();
		}

		// Token: 0x040015FA RID: 5626
		public readonly List<RoadTilePath.Piece> pathPieces = new List<RoadTilePath.Piece>();

		// Token: 0x040015FB RID: 5627
		[Serialize(false, null)]
		private Fix64 _length = -Fix64.One;

		// Token: 0x040015FC RID: 5628
		[Dependency]
		private IScope _scope;

		// Token: 0x0200042B RID: 1067
		[Factory.Serializable(1)]
		public class Piece : IReusable
		{
			// Token: 0x06001A49 RID: 6729 RVA: 0x0005FBFC File Offset: 0x0005DDFC
			public static RoadTilePath.Piece Create(IScope scope, List<Vector2Fixed> logicalPoints)
			{
				RoadTilePath.Piece piece = scope.Get<RoadTilePath.Piece>();
				piece.visualPoints = null;
				piece.logicalPoints = logicalPoints;
				return piece;
			}

			// Token: 0x06001A4A RID: 6730 RVA: 0x0005FC12 File Offset: 0x0005DE12
			public static RoadTilePath.Piece Create(IScope scope, List<Vector2Fixed> visualPoints, List<Vector2Fixed> logicalPoints)
			{
				RoadTilePath.Piece piece = scope.Get<RoadTilePath.Piece>();
				piece.visualPoints = visualPoints;
				piece.logicalPoints = logicalPoints;
				return piece;
			}

			// Token: 0x06001A4B RID: 6731 RVA: 0x0005FC28 File Offset: 0x0005DE28
			public void Reset()
			{
				this.visualPoints = null;
				this.logicalPoints = null;
			}

			// Token: 0x06001A4C RID: 6732 RVA: 0x0005FC38 File Offset: 0x0005DE38
			public RoadTilePath.Piece GetRotatedPiece(IScope scope, RoadTileRotation rotation)
			{
				List<Vector2Fixed> rotatedVisualPoints = null;
				if (this.visualPoints != null)
				{
					rotatedVisualPoints = new List<Vector2Fixed>();
					foreach (Vector2Fixed point in this.visualPoints)
					{
						rotatedVisualPoints.Add(TileUtilities.GetRotatedVector(point, rotation));
					}
				}
				List<Vector2Fixed> rotatedLogicalPoints = new List<Vector2Fixed>();
				foreach (Vector2Fixed point2 in this.logicalPoints)
				{
					rotatedLogicalPoints.Add(TileUtilities.GetRotatedVector(point2, rotation));
				}
				return RoadTilePath.Piece.Create(scope, rotatedVisualPoints, rotatedLogicalPoints);
			}

			// Token: 0x040015FD RID: 5629
			public List<Vector2Fixed> visualPoints;

			// Token: 0x040015FE RID: 5630
			public List<Vector2Fixed> logicalPoints;
		}
	}
}
