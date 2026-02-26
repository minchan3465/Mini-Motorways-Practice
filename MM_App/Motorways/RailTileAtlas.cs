using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using JetBrains.Annotations;
using Motorways.Utility;

namespace Motorways
{
	// Token: 0x02000418 RID: 1048
	public class RailTileAtlas
	{
		// Token: 0x060019D8 RID: 6616 RVA: 0x0005CF14 File Offset: 0x0005B114
		public void Initialize()
		{
			foreach (TileDirection inputDirection in TileUtilities.Directions)
			{
				for (int rotation = -2; rotation <= 2; rotation++)
				{
					TileDirection outputDirection = TileUtilities.GetOppositeDirection(TileUtilities.GetRotatedDirection(inputDirection, rotation));
					RailTileConnection connection = new RailTileConnection(inputDirection, outputDirection);
					RailTileDefinition definition = this.CreateDefinition(connection);
					this.AddDefinition(connection, definition);
				}
				RailTileConnection deadEndConnection = new RailTileConnection(inputDirection, TileDirection.None);
				this.AddDefinition(deadEndConnection, this.CreateDefinition(deadEndConnection));
				deadEndConnection = new RailTileConnection(TileDirection.None, inputDirection);
				this.AddDefinition(deadEndConnection, this.CreateDefinition(deadEndConnection));
			}
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x0005CFA8 File Offset: 0x0005B1A8
		[CanBeNull]
		public RailTileDefinition GetDefinition(RailTileConnection connection)
		{
			RailTileDefinition definition;
			if (this._connectionToDefinition.TryGetValue(connection, out definition))
			{
				return definition;
			}
			Diagnostics.FailAssert(string.Format("Couldn't find RailTileDefinition for {0}", connection), Array.Empty<object>());
			return null;
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x0005CFE4 File Offset: 0x0005B1E4
		[NotNull]
		private RailTileDefinition CreateDefinition(RailTileConnection connection)
		{
			RailTileDefinition newDefinition = null;
			for (int rotationIndex = 1; rotationIndex <= 3; rotationIndex++)
			{
				RoadTileRotation rotation = (RoadTileRotation)rotationIndex;
				RailTileConnection rotatedConnection = connection.GetRotatedConnection(rotation);
				RailTileDefinition originalDefinition;
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
				Spline.BezierSplineFixed railSpline = new Spline.BezierSplineFixed(input, inputHandle, outputHandle, output);
				path.pathPieces.Add(RoadTilePath.Piece.Create(this._scope, railSpline.Rasterize(10)));
			}
			newDefinition = this._scope.Get<RailTileDefinition>();
			newDefinition.rotation = RoadTileRotation.None;
			newDefinition.path = path;
			return newDefinition;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x0005D12B File Offset: 0x0005B32B
		private void AddDefinition(RailTileConnection connection, RailTileDefinition definition)
		{
			this._connectionToDefinition.Add(connection, definition);
			definition.index = this._indexToDefinition.Count;
			this._indexToDefinition.Add(definition);
		}

		// Token: 0x040015B9 RID: 5561
		private readonly Dictionary<RailTileConnection, RailTileDefinition> _connectionToDefinition = new Dictionary<RailTileConnection, RailTileDefinition>();

		// Token: 0x040015BA RID: 5562
		private readonly List<RailTileDefinition> _indexToDefinition = new List<RailTileDefinition>();

		// Token: 0x040015BB RID: 5563
		[Dependency]
		private IScope _scope;

		// Token: 0x040015BC RID: 5564
		private const int MaxDirectionChange = 2;
	}
}
