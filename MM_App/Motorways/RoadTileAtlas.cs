using System;
using System.Collections.Generic;
using System.Linq;
using Factory;
using FixMath;
using Motorways.Utility;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200041D RID: 1053
	[Factory.Serializable(1)]
	public class RoadTileAtlas
	{
		// Token: 0x060019F1 RID: 6641 RVA: 0x0005D3D9 File Offset: 0x0005B5D9
		public void Reset()
		{
			this._signatureToDefinition.Clear();
			this._signatureToCornerDefinition.Clear();
			this._indexToDefinition.Clear();
			this._connectionToStrokePaths.Clear();
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x0005D407 File Offset: 0x0005B607
		public void Initialize()
		{
			RoadTileAtlas.Log.Error("RoadTileAtlas.Initialise should only be called from Editor. Try rebuilding the RoadTileAtlas asset bundle (Assets -> Asset Bundles -> Build RoadTileAtlas", Array.Empty<object>());
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0005D420 File Offset: 0x0005B620
		public RoadTileDefinition GetDefinitionForSignature(RoadTileSignature signature)
		{
			RoadTileDefinition definition;
			if (this._signatureToDefinition.TryGetValue(signature, out definition))
			{
				return definition;
			}
			return null;
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x0005D440 File Offset: 0x0005B640
		public RoadTileDefinition GetCornerDefinitionForSignature(RoadTileSignature signature)
		{
			RoadTileDefinition definition;
			if (this._signatureToCornerDefinition.TryGetValue(signature, out definition))
			{
				return definition;
			}
			return null;
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x0005D460 File Offset: 0x0005B660
		public RoadTileDefinition GetDefinitionForIndex(int index)
		{
			if (Diagnostics.Verify(index < this._indexToDefinition.Count, "Invalid RoadTileDefinition index ({0}, max is {1}).", index, this._indexToDefinition.Count - 1))
			{
				return this._indexToDefinition[index];
			}
			return null;
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x0005D4B0 File Offset: 0x0005B6B0
		public RoadTileConnectionStrokePath GetStrokePathForConnection(RoadTileConnection connection)
		{
			RoadTileConnectionStrokePath mesh;
			if (this._connectionToStrokePaths.TryGetValue(connection, out mesh))
			{
				return mesh;
			}
			return null;
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x0005D4D0 File Offset: 0x0005B6D0
		public void ForEachDefinition(Action<RoadTileSignature, RoadTileDefinition> action)
		{
			foreach (KeyValuePair<RoadTileSignature, RoadTileDefinition> pair in this._signatureToDefinition)
			{
				action(pair.Key, pair.Value);
			}
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x0005D530 File Offset: 0x0005B730
		private List<List<TileDirection>> GetAllTwoLaneCombinations(List<TileDirection> inputDirections)
		{
			List<List<TileDirection>> result = new List<List<TileDirection>>();
			result.Add(new List<TileDirection>());
			result.Last<List<TileDirection>>().Add(inputDirections[0]);
			if (inputDirections.Count == 1)
			{
				return result;
			}
			this.GetAllTwoLaneCombinations(inputDirections.Skip(1).ToList<TileDirection>()).ForEach(delegate(List<TileDirection> combo)
			{
				result.Add(new List<TileDirection>(combo));
				combo.Add(inputDirections[0]);
				result.Add(new List<TileDirection>(combo));
			});
			return result;
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x0005D5C4 File Offset: 0x0005B7C4
		private bool ContainsDefinition(RoadTileSignature signature)
		{
			return this._signatureToDefinition.ContainsKey(signature);
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x0005D5D4 File Offset: 0x0005B7D4
		private void AddDefinitionForSignature(RoadTileSignature signature, RoadTileDefinition definition)
		{
			RoadTileDefinition existingDefinition;
			if (Diagnostics.Verify(!this._signatureToDefinition.TryGetValue(signature, out existingDefinition), "Tried to add definition {0} for signature {1} but _signatureToDefinition already contains a definition {2}", definition, signature, existingDefinition))
			{
				this._signatureToDefinition.Add(signature, definition);
				this.AddDefinitionToIndex(definition);
			}
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x0005D618 File Offset: 0x0005B818
		private void AddDefinitionForCornerSignature(RoadTileSignature cornerSignature, RoadTileDefinition definition)
		{
			RoadTileDefinition existingDefinition;
			if (Diagnostics.Verify(!this._signatureToCornerDefinition.TryGetValue(cornerSignature, out existingDefinition), "Tried to add definition {0} for corner signature {1} but _signatureToCornerDefinition already contains a definition {2}!", definition, cornerSignature, existingDefinition))
			{
				this._signatureToCornerDefinition.Add(cornerSignature, definition);
				this.AddDefinitionToIndex(definition);
			}
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x0005D659 File Offset: 0x0005B859
		private void AddDefinitionToIndex(RoadTileDefinition definition)
		{
			if (Diagnostics.Verify(definition.index == -1, "Tried to add RoadTileDefinition {0} to index, but it already has index {1}", definition, definition.index))
			{
				definition.index = this._indexToDefinition.Count;
				this._indexToDefinition.Add(definition);
			}
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x0005D69C File Offset: 0x0005B89C
		private void GenerateStrokePathForConnection(RoadTileConnection connection)
		{
			if (this._connectionToStrokePaths.ContainsKey(connection))
			{
				return;
			}
			Spline.BezierSplineFixed pathSpline;
			RoadTilePath connectionPath;
			if (connection.IsUTurn)
			{
				connectionPath = this.ConstructStubFromConnection(connection, out pathSpline, RoadTileAtlas.DiagonalPathLength.Extend);
			}
			else
			{
				connectionPath = this.ConstructPathFromConnection(connection, out pathSpline, RoadTileAtlas.DiagonalPathLength.Extend, RoadTileAtlas.PathLocationOnConnection.ThroughMedian, RoadTileAtlas.PathContainerType.Tile, false);
			}
			RoadTileConnectionStrokePath strokePath = this._scope.Get<RoadTileConnectionStrokePath>();
			strokePath.pathPoints.AddRange(from point in connectionPath.GetVisualPoints(Vector2Fixed.zero)
			select (Vector2)point);
			if (pathSpline != null)
			{
				strokePath.pathSpline = new Spline.BezierSpline((Vector2)pathSpline.inPoint, (Vector2)pathSpline.inHandle, (Vector2)pathSpline.outHandle, (Vector2)pathSpline.outPoint);
			}
			this._connectionToStrokePaths.Add(connection, strokePath);
			this._scope.Release(connectionPath);
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x0005D778 File Offset: 0x0005B978
		public RoadTileDefinition ConstructDefinitionFromSignature(RoadTileSignature signature)
		{
			if (this.ContainsDefinition(signature))
			{
				return this.GetDefinitionForSignature(signature);
			}
			for (int rotatedIndex = 1; rotatedIndex <= 3; rotatedIndex++)
			{
				RoadTileRotation testRotation = (RoadTileRotation)rotatedIndex;
				RoadTileSignature rotatedSignature = signature.CreateRotatedSignature(testRotation, this._scope);
				RoadTileDefinition newDefinition = null;
				if (this.ContainsDefinition(rotatedSignature))
				{
					RoadTileDefinition definitionForSignature = this.GetDefinitionForSignature(rotatedSignature);
					RoadTileRotation newDefinitionRotation = TileUtilities.SubtractRotation(definitionForSignature.rotation, testRotation);
					newDefinition = definitionForSignature.CreateRotatedDefinition(this._scope, newDefinitionRotation);
				}
				this._scope.Release(rotatedSignature);
				if (newDefinition != null)
				{
					return newDefinition;
				}
			}
			RoadTileDefinition definition = this._scope.Get<RoadTileDefinition>();
			definition.rotation = RoadTileRotation.None;
			foreach (RoadTileConnection connection in signature.Connections)
			{
				Spline.BezierSplineFixed bezierSplineFixed;
				RoadTilePath path = this.ConstructPathFromConnection(connection, out bezierSplineFixed, RoadTileAtlas.DiagonalPathLength.Truncate, RoadTileAtlas.PathLocationOnConnection.AlongsideMedian, RoadTileAtlas.PathContainerType.Tile, signature.IsRoundaboutCorner);
				definition.connectionToPath.Add(connection, path);
			}
			RoadTileMesh roadMesh = this.ConstructMeshFromDefinition(definition, signature.IsRoundaboutCorner);
			definition.mesh = roadMesh;
			return definition;
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x0005D888 File Offset: 0x0005BA88
		public RoadTileDefinition ConstructCornerDefinitionFromSignature(RoadTileSignature signature)
		{
			if (this._signatureToCornerDefinition.ContainsKey(signature))
			{
				return this._signatureToCornerDefinition[signature];
			}
			RoadTileDefinition definition = this._scope.Get<RoadTileDefinition>();
			definition.rotation = RoadTileRotation.None;
			foreach (RoadTileConnection connection in signature.Connections)
			{
				Spline.BezierSplineFixed bezierSplineFixed;
				RoadTilePath path = this.ConstructPathFromConnection(connection, out bezierSplineFixed, RoadTileAtlas.DiagonalPathLength.Truncate, RoadTileAtlas.PathLocationOnConnection.AlongsideMedian, RoadTileAtlas.PathContainerType.Corner, false);
				definition.connectionToPath.Add(connection, path);
			}
			definition.mesh = null;
			return definition;
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x0005D920 File Offset: 0x0005BB20
		public RoadTilePath ConstructPathFromConnection(RoadTileConnection connection, out Spline.BezierSplineFixed pathSpline, RoadTileAtlas.DiagonalPathLength diagonalPathLength = RoadTileAtlas.DiagonalPathLength.Truncate, RoadTileAtlas.PathLocationOnConnection pathLocation = RoadTileAtlas.PathLocationOnConnection.AlongsideMedian, RoadTileAtlas.PathContainerType containerType = RoadTileAtlas.PathContainerType.Tile, bool isRoundaboutCorner = false)
		{
			pathSpline = null;
			Fix64 pathLengthScale = Fix64Consts.One;
			if (containerType == RoadTileAtlas.PathContainerType.Corner)
			{
				pathLengthScale = Fix64.Sqrt(Fix64Consts.Two) - Fix64Consts.One;
			}
			TileDirection inDirection = connection.input.direction;
			TileDirection outDirection = connection.output.direction;
			RoadTilePath path = this._scope.Get<RoadTilePath>();
			Fix64 medianOffset = (pathLocation == RoadTileAtlas.PathLocationOnConnection.ThroughMedian && inDirection != outDirection) ? Fix64.Zero : RoadTileAtlas.LaneOffsetScale;
			Fix64 inMedianOffset = (connection.input.type == RoadType.Roundabout) ? Fix64.Zero : medianOffset;
			Fix64 outMedianOffset = (connection.output.type == RoadType.Roundabout) ? Fix64.Zero : medianOffset;
			-Fix64Consts.Two + Fix64.Sqrt((Fix64)3L);
			Fix64.Sqrt((Fix64)3L) * Fix64Consts.OneHalf;
			Vector2Fixed tileInBase = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)inDirection]) * pathLengthScale;
			Vector2Fixed tileInNormal = new Vector2Fixed(tileInBase.y, -tileInBase.x).normalized;
			Vector2Fixed tileInPos = tileInBase - tileInNormal * inMedianOffset;
			Vector2Fixed intersectionInBase = tileInBase.normalized * pathLengthScale;
			Vector2Fixed intersectionInPos = intersectionInBase - new Vector2Fixed(intersectionInBase.y, -intersectionInBase.x).normalized * inMedianOffset;
			Vector2Fixed tileOutBase = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)outDirection]) * pathLengthScale;
			Vector2Fixed tileOutNormal = new Vector2Fixed(tileOutBase.y, -tileOutBase.x).normalized;
			Vector2Fixed tileOutPos = tileOutBase + tileOutNormal * outMedianOffset;
			Vector2Fixed intersectionOutBase = tileOutBase.normalized * pathLengthScale;
			Vector2Fixed intersectionOutPos = intersectionOutBase + new Vector2Fixed(intersectionOutBase.y, -intersectionOutBase.x).normalized * outMedianOffset;
			bool isRoundaboutMerge = connection.input.type == RoadType.Roundabout ^ connection.output.type == RoadType.Roundabout;
			int distanceFromPerpendicular = TileUtilities.GetDistanceBetweenDirections(connection.output.direction, TileUtilities.GetRotatedDirection(connection.input.direction, 5));
			bool isPerpendicularRoundaboutMerge = isRoundaboutMerge && distanceFromPerpendicular <= 1;
			if (diagonalPathLength == RoadTileAtlas.DiagonalPathLength.Extend && TileUtilities.IsDirectionDiagonal(inDirection) && (!isRoundaboutMerge || connection.input.type != RoadType.Roundabout))
			{
				List<Vector2Fixed> pathInPoints = new List<Vector2Fixed>();
				if (connection.input.type == RoadType.Roundabout)
				{
					Vector2Fixed circleCentre = RoadTileAtlas.GetRoundaboutCenterForConnection(connection, containerType);
					Vector2Fixed centreToOut = intersectionInPos - circleCentre;
					Vector2Fixed centreToIn = (tileInPos - circleCentre).normalized * centreToOut.magnitude;
					Fix64 angleStep = Vector2Fixed.Angle(centreToIn, centreToOut) / RoadTileAtlas.PathInOutOfOctagonResolution;
					for (int pathPointIndex = 0; pathPointIndex <= (int)((long)RoadTileAtlas.PathInOutOfOctagonResolution); pathPointIndex++)
					{
						Vector2 centreToPathPoint = Vector3.RotateTowards((Vector3)centreToIn, (Vector3)centreToOut, (float)pathPointIndex * (float)angleStep, 0f);
						Vector2Fixed pathPoint = circleCentre + new Vector2Fixed(centreToPathPoint);
						pathInPoints.Add(pathPoint);
					}
				}
				else if (connection.input.type != RoadType.Roundabout)
				{
					pathInPoints.Add(tileInPos);
					pathInPoints.Add(intersectionInPos);
				}
				if (pathInPoints.Count > 0)
				{
					RoadTilePath.Piece pathPieceIn = RoadTilePath.Piece.Create(this._scope, pathInPoints, pathInPoints);
					path.pathPieces.Add(pathPieceIn);
				}
			}
			List<Vector2Fixed> visualPathThroughPoints = new List<Vector2Fixed>();
			List<Vector2Fixed> logicalPathThroughPoints = new List<Vector2Fixed>();
			if (inDirection != outDirection)
			{
				Fix64 inHandleScale = RoadTileAtlas.CornerHandleScale;
				Fix64 outHandleScale = RoadTileAtlas.CornerHandleScale;
				if (TileUtilities.GetDistanceBetweenDirections(inDirection, outDirection) == 1)
				{
					if (Vector2Fixed.Dot(tileOutPos - tileInPos, tileInNormal) < Fix64.Zero && diagonalPathLength == RoadTileAtlas.DiagonalPathLength.Truncate)
					{
						inHandleScale = RoadTileAtlas.TightCornerHandleScale;
						outHandleScale = RoadTileAtlas.TightCornerHandleScale;
					}
					if (isRoundaboutMerge)
					{
						if (connection.input.type == RoadType.Roundabout)
						{
							inHandleScale *= (Fix64)1.5f;
						}
						else
						{
							outHandleScale *= (Fix64)1.5f;
						}
					}
				}
				if (isRoundaboutMerge && containerType == RoadTileAtlas.PathContainerType.Corner)
				{
					if (connection.input.type != RoadType.Roundabout)
					{
						inHandleScale *= (Fix64)0.4f;
					}
					else
					{
						outHandleScale *= (Fix64)0.4f;
					}
				}
				Vector2Fixed inPosition = intersectionInPos;
				Vector2Fixed outPosition = intersectionOutPos;
				Vector2Fixed handlePositionA = intersectionInPos - intersectionInBase * inHandleScale;
				Vector2Fixed handlePositionB = intersectionOutPos - intersectionOutBase * outHandleScale;
				if (isRoundaboutMerge)
				{
					if (isPerpendicularRoundaboutMerge)
					{
						if (distanceFromPerpendicular == 0)
						{
							Fix64 handleAdjustment = (Fix64)0.3f;
							if (outDirection == TileDirection.North)
							{
								handlePositionA = new Vector2Fixed(intersectionOutPos.x, intersectionInPos.y + handleAdjustment);
							}
							else if (inDirection == TileDirection.North)
							{
								handlePositionA = new Vector2Fixed(intersectionInPos.x, intersectionOutPos.y + handleAdjustment);
							}
							else if (outDirection == TileDirection.South)
							{
								handlePositionA = new Vector2Fixed(intersectionOutPos.x, intersectionInPos.y - handleAdjustment);
							}
							else if (inDirection == TileDirection.South)
							{
								handlePositionA = new Vector2Fixed(intersectionInPos.x, intersectionOutPos.y - handleAdjustment);
							}
							else if (outDirection == TileDirection.East)
							{
								handlePositionA = new Vector2Fixed(intersectionInPos.x + handleAdjustment, intersectionOutPos.y);
							}
							else if (inDirection == TileDirection.East)
							{
								handlePositionA = new Vector2Fixed(intersectionOutPos.x + handleAdjustment, intersectionInPos.y);
							}
							else if (outDirection == TileDirection.West)
							{
								handlePositionA = new Vector2Fixed(intersectionInPos.x - handleAdjustment, intersectionOutPos.y);
							}
							else if (inDirection == TileDirection.West)
							{
								handlePositionA = new Vector2Fixed(intersectionOutPos.x - handleAdjustment, intersectionInPos.y);
							}
							handlePositionB = handlePositionA;
						}
						else if (containerType == RoadTileAtlas.PathContainerType.Tile)
						{
							Vector2Fixed roundaboutCenter = RoadTileAtlas.GetRoundaboutCenterForConnection(connection, containerType);
							if (connection.input.type == RoadType.Roundabout)
							{
								Vector2Fixed tangent = (intersectionInPos - roundaboutCenter).normalized.tangent;
								handlePositionA = intersectionInPos + tangent * inHandleScale;
							}
							else
							{
								Vector2Fixed tangent2 = (intersectionOutPos - roundaboutCenter).normalized.tangent;
								handlePositionB = intersectionOutPos - tangent2 * inHandleScale;
							}
						}
					}
					else if (diagonalPathLength == RoadTileAtlas.DiagonalPathLength.Extend)
					{
						Vector2Fixed circleCentre2 = RoadTileAtlas.GetRoundaboutCenterForConnection(connection, containerType);
						if (connection.input.type == RoadType.Roundabout)
						{
							Vector2Fixed centreToIntersectionIn = intersectionInPos - circleCentre2;
							Vector2Fixed centreToTileIn = (tileInPos - circleCentre2).normalized * centreToIntersectionIn.magnitude;
							inPosition = circleCentre2 + centreToTileIn;
							handlePositionA = inPosition - intersectionInBase * inHandleScale;
						}
						else
						{
							Vector2Fixed centreToIntersectionOut = intersectionOutPos - circleCentre2;
							Vector2Fixed centreToTileOut = (tileOutPos - circleCentre2).normalized * centreToIntersectionOut.magnitude;
							outPosition = circleCentre2 + centreToTileOut;
							handlePositionB = outPosition - intersectionOutBase * outHandleScale;
						}
					}
				}
				if (connection.IsRoundabout)
				{
					Vector2Fixed circleCentre3 = RoadTileAtlas.GetRoundaboutCenterForConnection(connection, containerType);
					Vector2Fixed centreToIn2 = inPosition - circleCentre3;
					Vector2Fixed centreToOut2 = outPosition - circleCentre3;
					Fix64 x = Vector2Fixed.Angle(centreToIn2, centreToOut2);
					Fix64 visualAngleStep = x / RoadTileAtlas.RoadPointResolution;
					Fix64 logicalAngleStep = x / RoadTileAtlas.LanePointResolution;
					for (int pathPointIndex2 = 0; pathPointIndex2 <= (int)((long)RoadTileAtlas.RoadPointResolution); pathPointIndex2++)
					{
						Vector2Fixed centreToPathPoint2 = (Vector2Fixed)Vector3.RotateTowards((Vector3)centreToIn2, (Vector3)centreToOut2, (float)pathPointIndex2 * (float)visualAngleStep, 0f);
						Vector2Fixed pathPoint2 = circleCentre3 + centreToPathPoint2;
						visualPathThroughPoints.Add(pathPoint2);
					}
					for (int pathPointIndex3 = 0; pathPointIndex3 <= (int)((long)RoadTileAtlas.LanePointResolution); pathPointIndex3++)
					{
						Vector2 centreToPathPoint3 = Vector3.RotateTowards((Vector3)centreToIn2, (Vector3)centreToOut2, (float)pathPointIndex3 * (float)logicalAngleStep, 0f);
						Vector2Fixed pathPoint3 = circleCentre3 + new Vector2Fixed(centreToPathPoint3);
						logicalPathThroughPoints.Add(new Vector2Fixed(pathPoint3));
					}
				}
				else
				{
					if (isPerpendicularRoundaboutMerge && containerType == RoadTileAtlas.PathContainerType.Tile)
					{
						Vector2Fixed circleCenter = RoadTileAtlas.GetRoundaboutCenterForConnection(connection, containerType);
						Fix64 circleRadius = (connection.input.type == RoadType.Roundabout) ? (circleCenter - inPosition).magnitude : (circleCenter - outPosition).magnitude;
						Vector2Fixed directionFromTileToCircleCenter = circleCenter.normalized;
						Vector2Fixed roundaboutIntersectionPoint = circleCenter - directionFromTileToCircleCenter * circleRadius;
						Vector2Fixed roundaboutMergeDirection = -directionFromTileToCircleCenter;
						Vector2Fixed mergeStart;
						Vector2Fixed mergeStartHandle;
						Vector2Fixed mergeEnd;
						Vector2Fixed mergeEndHandle;
						if (connection.input.type == RoadType.Roundabout)
						{
							mergeStart = roundaboutIntersectionPoint;
							mergeStartHandle = roundaboutIntersectionPoint + roundaboutMergeDirection * RoadTileAtlas.RoundaboutMergeHandleScale;
							mergeEnd = outPosition;
							mergeEndHandle = outPosition - intersectionOutBase * outHandleScale;
						}
						else
						{
							mergeStart = inPosition;
							mergeStartHandle = inPosition - intersectionInBase * inHandleScale;
							mergeEnd = roundaboutIntersectionPoint;
							mergeEndHandle = roundaboutIntersectionPoint + roundaboutMergeDirection * RoadTileAtlas.RoundaboutMergeHandleScale;
						}
						Fix64 pathPointIndex4 = Fix64.Zero;
						while (pathPointIndex4 <= RoadTileAtlas.RoadPointResolution)
						{
							Vector2Fixed thisPosition = Spline.EvaluateBezier(pathPointIndex4 / RoadTileAtlas.RoadPointResolution, mergeStart, mergeStartHandle, mergeEndHandle, mergeEnd);
							visualPathThroughPoints.Add(thisPosition);
							pathPointIndex4 += Fix64Consts.One;
						}
						pathSpline = new Spline.BezierSplineFixed(mergeStart, mergeStartHandle, mergeEndHandle, mergeEnd);
					}
					else
					{
						Fix64 pathPointIndex5 = Fix64.Zero;
						while (pathPointIndex5 <= RoadTileAtlas.RoadPointResolution)
						{
							Vector2Fixed thisPosition2 = Spline.EvaluateBezier(pathPointIndex5 / RoadTileAtlas.RoadPointResolution, inPosition, handlePositionA, handlePositionB, outPosition);
							visualPathThroughPoints.Add(thisPosition2);
							pathPointIndex5 += Fix64Consts.One;
						}
						pathSpline = new Spline.BezierSplineFixed(inPosition, handlePositionA, handlePositionB, outPosition);
					}
					Fix64 pathPointIndex6 = Fix64.Zero;
					while (pathPointIndex6 <= RoadTileAtlas.LanePointResolution)
					{
						Vector2Fixed thisPosition3 = Spline.EvaluateBezier(pathPointIndex6 / RoadTileAtlas.LanePointResolution, inPosition, handlePositionA, handlePositionB, outPosition);
						logicalPathThroughPoints.Add(thisPosition3);
						pathPointIndex6 += Fix64Consts.One;
					}
				}
			}
			else
			{
				visualPathThroughPoints.Add(new Vector2Fixed(intersectionInPos));
				logicalPathThroughPoints.Add(new Vector2Fixed(intersectionInPos));
				Vector2Fixed uTurnCenter = (intersectionInPos + intersectionOutPos) / Fix64Consts.Two;
				Fix64 uTurnDisplacementFromEdge = medianOffset;
				if (connection.input.type == RoadType.Driveway)
				{
					uTurnDisplacementFromEdge += RoadTileAtlas.DrivewayLength;
				}
				uTurnCenter -= intersectionInBase.normalized * uTurnDisplacementFromEdge;
				Fix64 angleStep2 = Fix64.Pi / RoadTileAtlas.EndCapMeshResolution;
				Vector2Fixed uTurnInitialPoint = new Vector2Fixed(intersectionInBase.y, -intersectionInBase.x).normalized * -medianOffset;
				Fix64 capIndex = Fix64.Zero;
				while (capIndex <= RoadTileAtlas.EndCapMeshResolution)
				{
					Fix64 cosAngle = Fix64.Cos(angleStep2 * capIndex);
					Fix64 sinAngle = Fix64.Sin(angleStep2 * capIndex);
					Fix64 x2 = uTurnInitialPoint.x * cosAngle - uTurnInitialPoint.y * sinAngle;
					Fix64 newY = uTurnInitialPoint.x * sinAngle + uTurnInitialPoint.y * cosAngle;
					Vector2Fixed pos = new Vector2Fixed(x2, newY) + uTurnCenter;
					visualPathThroughPoints.Add(pos);
					capIndex += Fix64Consts.One;
				}
				angleStep2 = Fix64.Pi / RoadTileAtlas.EndCapLaneResolution;
				Fix64 capIndex2 = Fix64.Zero;
				while (capIndex2 <= RoadTileAtlas.EndCapLaneResolution)
				{
					Fix64 cosAngle2 = Fix64.Cos(angleStep2 * capIndex2);
					Fix64 sinAngle2 = Fix64.Sin(angleStep2 * capIndex2);
					Fix64 x3 = uTurnInitialPoint.x * cosAngle2 - uTurnInitialPoint.y * sinAngle2;
					Fix64 newY2 = uTurnInitialPoint.x * sinAngle2 + uTurnInitialPoint.y * cosAngle2;
					Vector2Fixed pos2 = new Vector2Fixed(x3, newY2) + uTurnCenter;
					logicalPathThroughPoints.Add(pos2);
					capIndex2 += Fix64Consts.One;
				}
				visualPathThroughPoints.Add(new Vector2Fixed(intersectionOutPos));
				logicalPathThroughPoints.Add(new Vector2Fixed(intersectionOutPos));
				pathSpline = new Spline.BezierSplineFixed(intersectionInPos, Vector2Fixed.Lerp(intersectionInPos, intersectionOutPos, Fix64.One / (Fix64)3L), Vector2Fixed.Lerp(intersectionInPos, intersectionOutPos, Fix64Consts.Two / (Fix64)3L), intersectionOutPos);
			}
			RoadTilePath.Piece pathPieceThrough = RoadTilePath.Piece.Create(this._scope, visualPathThroughPoints, logicalPathThroughPoints);
			path.pathPieces.Add(pathPieceThrough);
			if (diagonalPathLength == RoadTileAtlas.DiagonalPathLength.Extend && TileUtilities.IsDirectionDiagonal(outDirection) && (!isRoundaboutMerge || connection.output.type != RoadType.Roundabout))
			{
				List<Vector2Fixed> pathOutPoints = new List<Vector2Fixed>();
				if (connection.output.type == RoadType.Roundabout)
				{
					Vector2Fixed circleCentre4 = RoadTileAtlas.GetRoundaboutCenterForConnection(connection, containerType);
					Vector2Fixed centreToIn3 = intersectionOutPos - circleCentre4;
					Vector2Fixed centreToOut3 = (tileOutPos - circleCentre4).normalized * centreToIn3.magnitude;
					Fix64 angleStep3 = Vector2Fixed.Angle(centreToIn3, centreToOut3) / RoadTileAtlas.PathInOutOfOctagonResolution;
					for (int pathPointIndex7 = 0; pathPointIndex7 <= (int)((long)RoadTileAtlas.PathInOutOfOctagonResolution); pathPointIndex7++)
					{
						Vector2 centreToPathPoint4 = Vector3.RotateTowards((Vector3)centreToIn3, (Vector3)centreToOut3, (float)pathPointIndex7 * (float)angleStep3, 0f);
						Vector2Fixed pathPoint4 = circleCentre4 + new Vector2Fixed(centreToPathPoint4);
						pathOutPoints.Add(pathPoint4);
					}
				}
				else if (connection.output.type != RoadType.Roundabout)
				{
					pathOutPoints.Add(intersectionOutPos);
					pathOutPoints.Add(tileOutPos);
				}
				if (pathOutPoints.Count > 0)
				{
					RoadTilePath.Piece pathPieceOut = RoadTilePath.Piece.Create(this._scope, pathOutPoints, pathOutPoints);
					path.pathPieces.Add(pathPieceOut);
				}
			}
			return path;
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x0005E6A8 File Offset: 0x0005C8A8
		private static Vector2Fixed GetRoundaboutCenterForConnection(RoadTileConnection connection, RoadTileAtlas.PathContainerType containerType)
		{
			if (containerType == RoadTileAtlas.PathContainerType.Tile)
			{
				if (connection.input.type == RoadType.Roundabout)
				{
					switch (connection.input.direction)
					{
					case TileDirection.NorthEast:
						return new Vector2Fixed(2f, 0f);
					case TileDirection.SouthEast:
						return new Vector2Fixed(0f, -2f);
					case TileDirection.SouthWest:
						return new Vector2Fixed(-2f, 0f);
					case TileDirection.NorthWest:
						return new Vector2Fixed(0f, 2f);
					}
				}
				else if (connection.output.type == RoadType.Roundabout)
				{
					switch (connection.output.direction)
					{
					case TileDirection.NorthEast:
						return new Vector2Fixed(0f, 2f);
					case TileDirection.SouthEast:
						return new Vector2Fixed(2f, 0f);
					case TileDirection.SouthWest:
						return new Vector2Fixed(0f, -2f);
					case TileDirection.NorthWest:
						return new Vector2Fixed(-2f, 0f);
					}
				}
			}
			else if (containerType == RoadTileAtlas.PathContainerType.Corner)
			{
				if (connection.input.type == RoadType.Roundabout)
				{
					switch (connection.input.direction)
					{
					case TileDirection.NorthEast:
						return new Vector2Fixed(1f, -1f);
					case TileDirection.SouthEast:
						return new Vector2Fixed(-1f, -1f);
					case TileDirection.SouthWest:
						return new Vector2Fixed(-1f, 1f);
					case TileDirection.NorthWest:
						return new Vector2Fixed(1f, 1f);
					}
				}
				else if (connection.input.type == RoadType.Roundabout)
				{
					switch (connection.output.direction)
					{
					case TileDirection.NorthEast:
						return new Vector2Fixed(-1f, 1f);
					case TileDirection.SouthEast:
						return new Vector2Fixed(1f, 1f);
					case TileDirection.SouthWest:
						return new Vector2Fixed(1f, -1f);
					case TileDirection.NorthWest:
						return new Vector2Fixed(-1f, -1f);
					}
				}
			}
			return Vector2Fixed.zero;
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x0005E8D4 File Offset: 0x0005CAD4
		private RoadTilePath ConstructStubFromConnection(RoadTileConnection connection, out Spline.BezierSplineFixed stubSpline, RoadTileAtlas.DiagonalPathLength diagonalPathLength = RoadTileAtlas.DiagonalPathLength.Truncate)
		{
			if (connection.input.direction != connection.output.direction)
			{
				return this.ConstructPathFromConnection(connection, out stubSpline, diagonalPathLength, RoadTileAtlas.PathLocationOnConnection.ThroughMedian, RoadTileAtlas.PathContainerType.Tile, false);
			}
			TileDirection direction = connection.input.direction;
			RoadTilePath path = this._scope.Get<RoadTilePath>();
			Vector2Fixed tileInBase = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)direction]);
			Vector2Fixed intersectionInBase = tileInBase.normalized;
			Vector2Fixed endPos = Vector2Fixed.zero;
			if (diagonalPathLength == RoadTileAtlas.DiagonalPathLength.Extend && TileUtilities.IsDirectionDiagonal(direction))
			{
				List<Vector2Fixed> pathInPoints = new List<Vector2Fixed>();
				pathInPoints.Add(tileInBase);
				pathInPoints.Add(intersectionInBase);
				RoadTilePath.Piece pathPieceIn = RoadTilePath.Piece.Create(this._scope, pathInPoints, pathInPoints);
				path.pathPieces.Add(pathPieceIn);
			}
			List<Vector2Fixed> pathThroughPoints = new List<Vector2Fixed>();
			pathThroughPoints.Add(intersectionInBase);
			pathThroughPoints.Add(endPos);
			RoadTilePath.Piece pathPieceThrough = RoadTilePath.Piece.Create(this._scope, pathThroughPoints, pathThroughPoints);
			path.pathPieces.Add(pathPieceThrough);
			Vector2Fixed stubDirection = endPos - intersectionInBase;
			stubSpline = new Spline.BezierSplineFixed(intersectionInBase, intersectionInBase + stubDirection * (Fix64.One / (Fix64)3L), intersectionInBase + stubDirection * (Fix64Consts.Two / (Fix64)3L), endPos);
			return path;
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x00004BD9 File Offset: 0x00002DD9
		private RoadTileMesh ConstructMeshFromDefinition(RoadTileDefinition definition, bool isRoundaboutCorner)
		{
			return null;
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x0005EA08 File Offset: 0x0005CC08
		public void ApplyMeshOverrides(RoadTileMeshOverride meshOverride)
		{
			foreach (RoadTileMeshOverrideDefinition overrideDefinition in meshOverride.meshOverrides)
			{
				RoadTileSignature signature = this._scope.Get<RoadTileSignature>();
				TileDirectionBitfield directions = new TileDirectionBitfield(overrideDefinition.directions);
				foreach (TileDirection direction in directions)
				{
					signature.AddNode(new RoadTileNode(direction, RoadType.TwoLane, -1));
				}
				this._signatureToDefinition[signature].mesh.ApplyMeshOverrides(overrideDefinition.meshes);
			}
		}

		// Token: 0x040015CC RID: 5580
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("RoadTileAtlas");

		// Token: 0x040015CD RID: 5581
		public static readonly Fix64 RoadScale = (Fix64)0.4f;

		// Token: 0x040015CE RID: 5582
		private static readonly Fix64 OutlineScale = (Fix64)0.6f;

		// Token: 0x040015CF RID: 5583
		private static readonly Fix64 EdgeExtrusion = (Fix64)1.002f;

		// Token: 0x040015D0 RID: 5584
		private static readonly Fix64 CornerHandleScale = (Fix64)0.6f;

		// Token: 0x040015D1 RID: 5585
		private static readonly Fix64 TightCornerHandleScale = (Fix64)0.2f;

		// Token: 0x040015D2 RID: 5586
		private static readonly Fix64 RoundaboutMergeHandleScale = (Fix64)0.6f;

		// Token: 0x040015D3 RID: 5587
		private static readonly Fix64 DrivewayLength = (Fix64)0.5f;

		// Token: 0x040015D4 RID: 5588
		private static readonly Fix64 RoadPointResolution = (Fix64)24L;

		// Token: 0x040015D5 RID: 5589
		private static readonly Fix64 LanePointResolution = (Fix64)10L;

		// Token: 0x040015D6 RID: 5590
		private static readonly Fix64 PathInOutOfOctagonResolution = (Fix64)5L;

		// Token: 0x040015D7 RID: 5591
		public static readonly Fix64 LaneOffsetScale = (Fix64)0.2f;

		// Token: 0x040015D8 RID: 5592
		private static readonly Fix64 EndCapMeshResolution = (Fix64)13L;

		// Token: 0x040015D9 RID: 5593
		private static readonly Fix64 EndCapLaneResolution = (Fix64)9L;

		// Token: 0x040015DA RID: 5594
		private readonly Dictionary<RoadTileSignature, RoadTileDefinition> _signatureToDefinition = new Dictionary<RoadTileSignature, RoadTileDefinition>(new RoadTileSignature.MotorwayAgnosticEqualityComparer());

		// Token: 0x040015DB RID: 5595
		private readonly Dictionary<RoadTileSignature, RoadTileDefinition> _signatureToCornerDefinition = new Dictionary<RoadTileSignature, RoadTileDefinition>();

		// Token: 0x040015DC RID: 5596
		private readonly List<RoadTileDefinition> _indexToDefinition = new List<RoadTileDefinition>();

		// Token: 0x040015DD RID: 5597
		private readonly Dictionary<RoadTileConnection, RoadTileConnectionStrokePath> _connectionToStrokePaths = new Dictionary<RoadTileConnection, RoadTileConnectionStrokePath>(new RoadTileConnection.MotorwayAgnosticEqualityComparer());

		// Token: 0x040015DE RID: 5598
		[Dependency]
		private IScope _scope;

		// Token: 0x0200041E RID: 1054
		public enum DiagonalPathLength
		{
			// Token: 0x040015E0 RID: 5600
			Extend,
			// Token: 0x040015E1 RID: 5601
			Truncate
		}

		// Token: 0x0200041F RID: 1055
		public enum PathLocationOnConnection
		{
			// Token: 0x040015E3 RID: 5603
			ThroughMedian,
			// Token: 0x040015E4 RID: 5604
			AlongsideMedian
		}

		// Token: 0x02000420 RID: 1056
		public enum PathContainerType
		{
			// Token: 0x040015E6 RID: 5606
			Tile,
			// Token: 0x040015E7 RID: 5607
			Corner
		}
	}
}
