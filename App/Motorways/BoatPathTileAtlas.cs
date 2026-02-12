using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using JetBrains.Annotations;
using Motorways.Utility;

namespace Motorways
{
	// Token: 0x020003E4 RID: 996
	public class BoatPathTileAtlas
	{
		// Token: 0x0600181F RID: 6175 RVA: 0x000562A4 File Offset: 0x000544A4
		public void Initialize()
		{
			foreach (TileDirection inputDirection in TileUtilities.Directions)
			{
				for (int rotation = -2; rotation <= 2; rotation++)
				{
					TileDirection outputDirection = TileUtilities.GetOppositeDirection(TileUtilities.GetRotatedDirection(inputDirection, rotation));
					BoatPathTileConnection connection = new BoatPathTileConnection(inputDirection, outputDirection);
					BoatPathTileDefinition definition = this.CreateDefinition(connection);
					this.AddDefinition(connection, definition);
				}
				BoatPathTileConnection deadEndConnection = new BoatPathTileConnection(inputDirection, TileDirection.None);
				this.AddDefinition(deadEndConnection, this.CreateDefinition(deadEndConnection));
				deadEndConnection = new BoatPathTileConnection(TileDirection.None, inputDirection);
				this.AddDefinition(deadEndConnection, this.CreateDefinition(deadEndConnection));
			}
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x00056338 File Offset: 0x00054538
		[CanBeNull]
		public BoatPathTileDefinition GetDefinition(BoatPathTileConnection connection)
		{
			BoatPathTileDefinition definition;
			if (this._connectionToDefinition.TryGetValue(connection, out definition))
			{
				return definition;
			}
			Diagnostics.FailAssert(string.Format("Couldn't find BoatPathTileDefinition for {0}", connection), Array.Empty<object>());
			return null;
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x00056374 File Offset: 0x00054574
		[NotNull]
		private BoatPathTileDefinition CreateDefinition(BoatPathTileConnection connection)
		{
			BoatPathTileDefinition newDefinition = null;
			for (int rotationIndex = 1; rotationIndex <= 3; rotationIndex++)
			{
				RoadTileRotation rotation = (RoadTileRotation)rotationIndex;
				BoatPathTileConnection rotatedConnection = connection.GetRotatedConnection(rotation);
				BoatPathTileDefinition originalDefinition;
				if (this._connectionToDefinition.TryGetValue(rotatedConnection, out originalDefinition))
				{
					newDefinition = originalDefinition.CreateRotatedDefinition(this._scope, TileUtilities.SubtractRotation(RoadTileRotation.None, rotation));
				}
				if (newDefinition != null)
				{
					return newDefinition;
				}
			}
			RoadTilePath path = this._scope.Get<RoadTilePath>();
			Vector2Fixed input = TileUtilities.GetTileEdgeForDirection(connection.input);
			Vector2Fixed output = TileUtilities.GetTileEdgeForDirection(connection.output);
			if (connection.IsDeadEnd || connection.output == TileUtilities.GetOppositeDirection(connection.input))
			{
				path.pathPieces.Add(RoadTilePath.Piece.Create(this._scope, new List<Vector2Fixed>
				{
					input,
					output
				}));
			}
			else
			{
				Fix64 handleScale = Fix64Consts.OneHalf;
				Vector2Fixed inputHandle = input - TileUtilities.GetVectorFixedForDirection(connection.input) * handleScale;
				Vector2Fixed outputHandle = output - TileUtilities.GetVectorFixedForDirection(connection.output) * handleScale;
				Spline.BezierSplineFixed boatPathSpline = new Spline.BezierSplineFixed(input, inputHandle, outputHandle, output);
				path.pathPieces.Add(RoadTilePath.Piece.Create(this._scope, boatPathSpline.Rasterize(10)));
			}
			newDefinition = this._scope.Get<BoatPathTileDefinition>();
			newDefinition.rotation = RoadTileRotation.None;
			newDefinition.path = path;
			return newDefinition;
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x000564BB File Offset: 0x000546BB
		private void AddDefinition(BoatPathTileConnection connection, BoatPathTileDefinition definition)
		{
			this._connectionToDefinition.Add(connection, definition);
			definition.index = this._indexToDefinition.Count;
			this._indexToDefinition.Add(definition);
		}

		// Token: 0x040014AB RID: 5291
		private readonly Dictionary<BoatPathTileConnection, BoatPathTileDefinition> _connectionToDefinition = new Dictionary<BoatPathTileConnection, BoatPathTileDefinition>();

		// Token: 0x040014AC RID: 5292
		private readonly List<BoatPathTileDefinition> _indexToDefinition = new List<BoatPathTileDefinition>();

		// Token: 0x040014AD RID: 5293
		[Dependency]
		private IScope _scope;

		// Token: 0x040014AE RID: 5294
		private const int MaxDirectionChange = 2;
	}
}
