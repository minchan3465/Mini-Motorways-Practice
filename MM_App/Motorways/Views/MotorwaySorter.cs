using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Motorways.Utility;
using UnityEngine;
using Utils.Geometry;

namespace Motorways.Views
{
	// Token: 0x020005E7 RID: 1511
	public class MotorwaySorter
	{
		// Token: 0x06002A1C RID: 10780 RVA: 0x000B75C0 File Offset: 0x000B57C0
		public void CalculateDepthSegments(Dictionary<int, MotorwayView> motorwayViews, MotorwayGeometryInfo motorwayGeometryInfo)
		{
			this.ComputeMotorwayEdgeOverlaps(motorwayGeometryInfo, motorwayViews);
			Dictionary<int, MotorwaySorter.MotorwayDepth> motorwayDepths = new Dictionary<int, MotorwaySorter.MotorwayDepth>();
			foreach (KeyValuePair<int, MotorwayView> motorwayView in motorwayViews)
			{
				motorwayDepths.Add(motorwayView.Key, new MotorwaySorter.MotorwayDepth(motorwayView.Key));
			}
			foreach (KeyValuePair<int, MotorwayGeometryInfo.MotorwayEndEdges> motorwayEndEdge in motorwayGeometryInfo.EndEdges)
			{
				this.FindDepthSegments(motorwayEndEdge.Key, motorwayEndEdge.Value.start, true, motorwayViews, motorwayGeometryInfo.Polygons, motorwayDepths);
				this.FindDepthSegments(motorwayEndEdge.Key, motorwayEndEdge.Value.end, false, motorwayViews, motorwayGeometryInfo.Polygons, motorwayDepths);
			}
			List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>> depthSegmentGroups = MotorwaySorter.GroupDepthSegments(motorwayDepths);
			MotorwaySorter.SortDepthSegmentGroups(depthSegmentGroups);
			MotorwaySorter.AssignWorldspaceDepths(depthSegmentGroups);
			foreach (KeyValuePair<int, MotorwayView> motorwayView2 in motorwayViews)
			{
				motorwayView2.Value.SetMotorwayDepth(motorwayDepths[motorwayView2.Key]);
			}
		}

		// Token: 0x06002A1D RID: 10781 RVA: 0x000B7710 File Offset: 0x000B5910
		private static void AssignWorldspaceDepths([TupleElementNames(new string[]
		{
			"endEdges",
			"depthSegments"
		})] List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>> depthSegmentGroups)
		{
			foreach (ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>> depthSegmentGroup in depthSegmentGroups)
			{
				for (int depthSegmentGroupIndex = 0; depthSegmentGroupIndex < depthSegmentGroup.Item2.Count; depthSegmentGroupIndex++)
				{
					depthSegmentGroup.Item2[depthSegmentGroupIndex].depth = -4.5f + -1.5f * (float)(depthSegmentGroupIndex + 1) / (float)depthSegmentGroup.Item2.Count;
				}
			}
		}

		// Token: 0x06002A1E RID: 10782 RVA: 0x000B779C File Offset: 0x000B599C
		private static void SortDepthSegmentGroups([TupleElementNames(new string[]
		{
			"endEdges",
			"depthSegments"
		})] List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>> depthSegmentGroups)
		{
			using (List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>>.Enumerator enumerator = depthSegmentGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>> depthSegmentGroup = enumerator.Current;
					depthSegmentGroup.Item2.Sort(delegate(MotorwaySorter.MotorwayDepthSegment depthSegmentA, MotorwaySorter.MotorwayDepthSegment depthSegmentB)
					{
						bool foundALessThanB = false;
						bool foundBLessThanA = false;
						foreach (MotorwaySorter.MotorwayDepthSegment motorwayDepthSegment in depthSegmentGroup.Item2)
						{
							foreach (MotorwaySorter.MotorwayLowerThanConstraint constraint in motorwayDepthSegment.constraints)
							{
								foundALessThanB = (foundALessThanB || (constraint.lowerMotorwayId == depthSegmentA.motorwayId && constraint.higherMotorwayId == depthSegmentB.motorwayId));
								foundBLessThanA = (foundBLessThanA || (constraint.lowerMotorwayId == depthSegmentB.motorwayId && constraint.higherMotorwayId == depthSegmentA.motorwayId));
							}
							if (foundALessThanB && foundBLessThanA)
							{
								break;
							}
						}
						if ((foundALessThanB && foundBLessThanA) || (!foundALessThanB && !foundBLessThanA))
						{
							if (depthSegmentA.motorwayId >= depthSegmentB.motorwayId)
							{
								return 1;
							}
							return -1;
						}
						else
						{
							if (!foundALessThanB)
							{
								return 1;
							}
							return -1;
						}
					});
				}
			}
		}

		// Token: 0x06002A1F RID: 10783 RVA: 0x000B780C File Offset: 0x000B5A0C
		[return: TupleElementNames(new string[]
		{
			"endEdges",
			"depthSegments"
		})]
		private static List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>> GroupDepthSegments(Dictionary<int, MotorwaySorter.MotorwayDepth> motorwayDepths)
		{
			List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>> groupedConstraints = new List<ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>>();
			foreach (KeyValuePair<int, MotorwaySorter.MotorwayDepth> motorwayDepth in motorwayDepths)
			{
				foreach (MotorwaySorter.MotorwayDepthSegment motorwayDepthSegment in motorwayDepth.Value.DepthSegments)
				{
					ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>? existingGroup = null;
					foreach (ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>> groupedConstraint in groupedConstraints)
					{
						foreach (MotorwaySorter.MotorwayLowerThanConstraint constraint in motorwayDepthSegment.constraints)
						{
							if (groupedConstraint.Item1.Contains(constraint.endEdge))
							{
								existingGroup = new ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>?(groupedConstraint);
								break;
							}
						}
					}
					if (existingGroup == null)
					{
						existingGroup = new ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>?(new ValueTuple<List<MotorwayGeometryInfo.MotorwayEndEdge>, List<MotorwaySorter.MotorwayDepthSegment>>(new List<MotorwayGeometryInfo.MotorwayEndEdge>(), new List<MotorwaySorter.MotorwayDepthSegment>()));
						groupedConstraints.Add(existingGroup.Value);
					}
					foreach (MotorwaySorter.MotorwayLowerThanConstraint constraint2 in motorwayDepthSegment.constraints)
					{
						if (!existingGroup.Value.Item1.Contains(constraint2.endEdge))
						{
							existingGroup.Value.Item1.Add(constraint2.endEdge);
						}
					}
					existingGroup.Value.Item2.Add(motorwayDepthSegment);
				}
			}
			return groupedConstraints;
		}

		// Token: 0x06002A20 RID: 10784 RVA: 0x000B7A2C File Offset: 0x000B5C2C
		public bool CanCalculateDepthSegments(Dictionary<int, MotorwayView> motorwayViews)
		{
			if (motorwayViews.Count <= 0)
			{
				return false;
			}
			using (Dictionary<int, MotorwayView>.ValueCollection.Enumerator enumerator = motorwayViews.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Spline.spline == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002A21 RID: 10785 RVA: 0x000B7A98 File Offset: 0x000B5C98
		private ValueTuple<int, int> MotorwayIntersectionCacheKey(int motorwayIdA, int motorwayIdB)
		{
			return new ValueTuple<int, int>(Math.Min(motorwayIdA, motorwayIdB), Math.Max(motorwayIdA, motorwayIdB));
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x000B7AB0 File Offset: 0x000B5CB0
		private Dictionary<ValueTuple<int, int>, List<MotorwayIntersectionUtil.MotorwayIntersection>> ComputeIntersectionPoints(Dictionary<int, MotorwayGeometryInfo.MotorwayEndEdges> endEdges, Dictionary<int, MotorwayPolygon> motorwayPolygons)
		{
			Dictionary<ValueTuple<int, int>, List<MotorwayIntersectionUtil.MotorwayIntersection>> allIntersections = new Dictionary<ValueTuple<int, int>, List<MotorwayIntersectionUtil.MotorwayIntersection>>();
			foreach (KeyValuePair<int, MotorwayGeometryInfo.MotorwayEndEdges> motorwayEndEdges in endEdges)
			{
				int lowerMotorwayId = motorwayEndEdges.Key;
				MotorwayPolygon lowerMotorwayPolygon = motorwayPolygons[lowerMotorwayId];
				List<int> list = new List<int>(motorwayEndEdges.Value.start.overlappingMotorwayIds);
				list.AddRange(motorwayEndEdges.Value.end.overlappingMotorwayIds);
				foreach (int overlappingMotorwayId in list)
				{
					ValueTuple<int, int> cacheKey = this.MotorwayIntersectionCacheKey(lowerMotorwayId, overlappingMotorwayId);
					if (!allIntersections.ContainsKey(cacheKey))
					{
						MotorwayPolygon higherMotorwayPolygon = motorwayPolygons[overlappingMotorwayId];
						List<MotorwayIntersectionUtil.MotorwayIntersection> intersections;
						MotorwayIntersectionUtil.PolygonIntersectsPolygon(lowerMotorwayPolygon, higherMotorwayPolygon, out intersections);
						allIntersections.Add(cacheKey, intersections);
					}
				}
			}
			return allIntersections;
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x000B7BB0 File Offset: 0x000B5DB0
		private void FindDepthSegments(int endEdgeMotorwayId, MotorwayGeometryInfo.MotorwayEndEdge motorwayEndEdge, bool isStartEdge, Dictionary<int, MotorwayView> motorwayViews, Dictionary<int, MotorwayPolygon> motorwayPolygons, Dictionary<int, MotorwaySorter.MotorwayDepth> motorwayDepth)
		{
			MotorwayPolygon lowerMotorwayPolygon = motorwayPolygons[endEdgeMotorwayId];
			Spline.RasterizedSpline lowerMotorway = motorwayViews[endEdgeMotorwayId].Spline.spline.Rasterize(10);
			foreach (int higherMotorwayId in motorwayEndEdge.overlappingMotorwayIds)
			{
				Spline.RasterizedSpline higherMotorway = motorwayViews[higherMotorwayId].Spline.spline.Rasterize(10);
				MotorwayPolygon higherMotorwayPolygon = motorwayPolygons[higherMotorwayId];
				List<MotorwayIntersectionUtil.MotorwayIntersection> intersections;
				MotorwayIntersectionUtil.PolygonIntersectsPolygon(lowerMotorwayPolygon, higherMotorwayPolygon, out intersections);
				if (intersections.Count == 0)
				{
					Diagnostics.FailAssert("There should be intersections as the motorways end-edges were flagged as 'overlapping'", Array.Empty<object>());
				}
				else if (intersections.Count != 1)
				{
					List<ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2>> sortedIntersections = MotorwaySorter.SortIntersectionsAlongSpline(lowerMotorway, intersections, isStartEdge);
					ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2> endIntersection = sortedIntersections[sortedIntersections.Count - 1];
					foreach (ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2> sortedIntersection in sortedIntersections)
					{
						if (sortedIntersection.Item1.isSeparatingOrJoining)
						{
							endIntersection = sortedIntersection;
							break;
						}
					}
					MotorwaySorter.MotorwayLowerThanConstraint motorwayLowerThanConstraint = new MotorwaySorter.MotorwayLowerThanConstraint(motorwayEndEdge, endEdgeMotorwayId, higherMotorwayId);
					float startDistance = isStartEdge ? 0f : lowerMotorway.Length;
					Vector2 startPosition = isStartEdge ? lowerMotorway.Positions[0] : lowerMotorway.Positions[lowerMotorway.Positions.Count - 1];
					MotorwaySorter.MotorwayDepthSegment lowerMotorwaySegment = new MotorwaySorter.MotorwayDepthSegment(endEdgeMotorwayId, startDistance, startPosition, endIntersection.Item2, endIntersection.Item3);
					lowerMotorwaySegment.constraints.Add(motorwayLowerThanConstraint);
					motorwayDepth[endEdgeMotorwayId].Add(lowerMotorwaySegment);
					List<ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2>> sortedIntersectionsOverlapping = MotorwaySorter.SortIntersectionsAlongSpline(higherMotorway, intersections, true);
					int firstSeparationPointIndex = 0;
					for (int sortedIntersectionOverlappingIndex = 0; sortedIntersectionOverlappingIndex < sortedIntersectionsOverlapping.Count; sortedIntersectionOverlappingIndex++)
					{
						if (sortedIntersectionsOverlapping[sortedIntersectionOverlappingIndex].Item1.Equals(endIntersection.Item1))
						{
							firstSeparationPointIndex = sortedIntersectionOverlappingIndex;
							break;
						}
					}
					int leftOfFirstSeparationPoint = firstSeparationPointIndex - 1;
					int rightOfFirstSeparationPoint = firstSeparationPointIndex + 1;
					bool takeClosestToStart = (leftOfFirstSeparationPoint >= 0 && rightOfFirstSeparationPoint >= sortedIntersectionsOverlapping.Count) || ((leftOfFirstSeparationPoint >= 0 || rightOfFirstSeparationPoint >= sortedIntersections.Count) && (leftOfFirstSeparationPoint < 0 || rightOfFirstSeparationPoint >= sortedIntersectionsOverlapping.Count || sortedIntersectionsOverlapping[rightOfFirstSeparationPoint].Item1.isSeparatingOrJoining));
					ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2> firstSeparationPoint = sortedIntersectionsOverlapping[firstSeparationPointIndex];
					MotorwaySorter.MotorwayDepthSegment higherMotorwaySegment;
					if (takeClosestToStart)
					{
						float segmentStartDistance = sortedIntersectionsOverlapping[0].Item2;
						Vector2 segmentStartPoint = sortedIntersectionsOverlapping[0].Item3;
						higherMotorwaySegment = new MotorwaySorter.MotorwayDepthSegment(higherMotorwayId, segmentStartDistance, segmentStartPoint, firstSeparationPoint.Item2, firstSeparationPoint.Item3);
					}
					else
					{
						float segmentEndDistance = sortedIntersectionsOverlapping[sortedIntersectionsOverlapping.Count - 1].Item2;
						Vector2 segmentEndPoint = sortedIntersectionsOverlapping[sortedIntersectionsOverlapping.Count - 1].Item3;
						higherMotorwaySegment = new MotorwaySorter.MotorwayDepthSegment(higherMotorwayId, firstSeparationPoint.Item2, firstSeparationPoint.Item3, segmentEndDistance, segmentEndPoint);
					}
					higherMotorwaySegment.constraints.Add(motorwayLowerThanConstraint);
					motorwayDepth[higherMotorwayId].Add(higherMotorwaySegment);
				}
			}
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x000B7EE0 File Offset: 0x000B60E0
		[return: TupleElementNames(new string[]
		{
			"point",
			"distance",
			"pointOnSpline"
		})]
		private static List<ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2>> SortIntersectionsAlongSpline(Spline.RasterizedSpline motorwaySpline, List<MotorwayIntersectionUtil.MotorwayIntersection> intersections, bool fromStart)
		{
			List<ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2>> sortedIntersections = new List<ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2>>(intersections.Count);
			foreach (MotorwayIntersectionUtil.MotorwayIntersection motorwayIntersection in intersections)
			{
				ValueTuple<int, int> valueTuple = MotorwaySpline.ClosestEdgeOnSplineToPoint(motorwaySpline, motorwayIntersection.point);
				int closestStartIndex = valueTuple.Item1;
				int closestEndIndex = valueTuple.Item2;
				Vector2 pointClosestToEdge = motorwaySpline.Positions[closestStartIndex];
				Vector2 segmentDirection = motorwaySpline.Positions[closestEndIndex] - pointClosestToEdge;
				float projectionMagnitude = Vector2.Dot(segmentDirection, motorwayIntersection.point - pointClosestToEdge) / segmentDirection.magnitude;
				Vector2 pointOnSegment = pointClosestToEdge + segmentDirection.normalized * projectionMagnitude;
				Vector2 vector = pointClosestToEdge;
				float distanceAlongSpline = Vector2.Distance(vector, pointOnSegment);
				Vector2 previous = vector;
				for (int i = closestStartIndex - 1; i >= 0; i--)
				{
					Vector2 current = motorwaySpline.Positions[i];
					distanceAlongSpline += Vector2.Distance(current, previous);
					previous = current;
				}
				sortedIntersections.Add(new ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2>(motorwayIntersection, distanceAlongSpline, pointOnSegment));
			}
			sortedIntersections.Sort(delegate([TupleElementNames(new string[]
			{
				"point",
				"distance",
				"pointOnSpline"
			})] ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2> intersectionA, [TupleElementNames(new string[]
			{
				"point",
				"distance",
				"pointOnSpline"
			})] ValueTuple<MotorwayIntersectionUtil.MotorwayIntersection, float, Vector2> intersectionB)
			{
				if (intersectionA.Item2 < intersectionB.Item2)
				{
					if (!fromStart)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (intersectionA.Item2 <= intersectionB.Item2)
					{
						return 0;
					}
					if (!fromStart)
					{
						return -1;
					}
					return 1;
				}
			});
			return sortedIntersections;
		}

		// Token: 0x06002A25 RID: 10789 RVA: 0x000B8020 File Offset: 0x000B6220
		private void ComputeMotorwayEdgeOverlaps(MotorwayGeometryInfo motorwayGeometryInfo, Dictionary<int, MotorwayView> motorwayViews)
		{
			foreach (KeyValuePair<int, AxisAlignedBoundingBox> motorwayBoundingBox in motorwayGeometryInfo.Bounds)
			{
				if ((motorwayViews[motorwayBoundingBox.Key].Motorway.State & RoadState.VisiblyActive) > RoadState.None)
				{
					MotorwayPolygon motorwayPolygon = motorwayGeometryInfo.Polygons[motorwayBoundingBox.Key];
					foreach (KeyValuePair<int, MotorwayGeometryInfo.MotorwayEndEdges> motorwayEndEdgesWithId in motorwayGeometryInfo.EndEdges)
					{
						if (motorwayBoundingBox.Key != motorwayEndEdgesWithId.Key && (motorwayViews[motorwayEndEdgesWithId.Key].Motorway.State & RoadState.VisiblyActive) > RoadState.None)
						{
							MotorwayGeometryInfo.MotorwayEndEdges motorwayEndEdges = motorwayEndEdgesWithId.Value;
							if (MotorwayIntersectionUtil.EndEdgeIntersectsPolygon(motorwayPolygon, motorwayEndEdges.start, motorwayBoundingBox.Value))
							{
								motorwayEndEdges.start.overlappingMotorwayIds.Add(motorwayPolygon.motorwayId);
							}
							if (MotorwayIntersectionUtil.EndEdgeIntersectsPolygon(motorwayPolygon, motorwayEndEdges.end, motorwayBoundingBox.Value))
							{
								motorwayEndEdges.end.overlappingMotorwayIds.Add(motorwayPolygon.motorwayId);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002A26 RID: 10790 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x04002421 RID: 9249
		public const float MaxMotorwayWorldHeight = -6f;

		// Token: 0x04002422 RID: 9250
		public const float MinMotorwayWorldHeight = -3f;

		// Token: 0x04002423 RID: 9251
		private const float MotorwayWorldHeightRange = -3f;

		// Token: 0x04002424 RID: 9252
		private const float GapBetweenSortedAndDefault = -0.3f;

		// Token: 0x04002425 RID: 9253
		public const float SortedMinMotorwayWorldHeight = -4.5f;

		// Token: 0x04002426 RID: 9254
		public const float SortedMaxMotorwayWorldHeight = -6f;

		// Token: 0x04002427 RID: 9255
		public const float SortedMotorwayWorldHeightRange = -1.5f;

		// Token: 0x04002428 RID: 9256
		public const float DefaultMinMotorwayWorldHeight = -3f;

		// Token: 0x04002429 RID: 9257
		public const float DefaultMaxMotorwayWorldHeight = -4.2f;

		// Token: 0x0400242A RID: 9258
		public const float DefaultMotorwayWorldHeightRange = -1.1999998f;

		// Token: 0x020005E8 RID: 1512
		public readonly struct MotorwayLowerThanConstraint
		{
			// Token: 0x06002A28 RID: 10792 RVA: 0x000B8198 File Offset: 0x000B6398
			public MotorwayLowerThanConstraint(MotorwayGeometryInfo.MotorwayEndEdge endEdge, int lowerMotorwayId, int higherMotorwayId)
			{
				this.endEdge = endEdge;
				this.lowerMotorwayId = lowerMotorwayId;
				this.higherMotorwayId = higherMotorwayId;
			}

			// Token: 0x0400242B RID: 9259
			public readonly MotorwayGeometryInfo.MotorwayEndEdge endEdge;

			// Token: 0x0400242C RID: 9260
			public readonly int lowerMotorwayId;

			// Token: 0x0400242D RID: 9261
			public readonly int higherMotorwayId;
		}

		// Token: 0x020005E9 RID: 1513
		public class MotorwayDepthSegment
		{
			// Token: 0x06002A29 RID: 10793 RVA: 0x000B81B0 File Offset: 0x000B63B0
			public MotorwayDepthSegment(int motorwayId, float startDistance, Vector2 startPosition, float endDistance, Vector2 endPosition)
			{
				this.motorwayId = motorwayId;
				if (startDistance <= endDistance)
				{
					this.startDistance = startDistance;
					this.startPosition = startPosition;
					this.endDistance = endDistance;
					this.endPosition = endPosition;
					return;
				}
				this.startDistance = endDistance;
				this.startPosition = endPosition;
				this.endDistance = startDistance;
				this.endPosition = startPosition;
			}

			// Token: 0x0400242E RID: 9262
			public readonly int motorwayId;

			// Token: 0x0400242F RID: 9263
			public float startDistance;

			// Token: 0x04002430 RID: 9264
			public Vector2 startPosition;

			// Token: 0x04002431 RID: 9265
			public float endDistance;

			// Token: 0x04002432 RID: 9266
			public Vector2 endPosition;

			// Token: 0x04002433 RID: 9267
			public readonly List<MotorwaySorter.MotorwayLowerThanConstraint> constraints = new List<MotorwaySorter.MotorwayLowerThanConstraint>();

			// Token: 0x04002434 RID: 9268
			public float depth;
		}

		// Token: 0x020005EA RID: 1514
		public class MotorwayDepth
		{
			// Token: 0x17000721 RID: 1825
			// (get) Token: 0x06002A2A RID: 10794 RVA: 0x000B8217 File Offset: 0x000B6417
			public IReadOnlyList<MotorwaySorter.MotorwayDepthSegment> DepthSegments
			{
				get
				{
					return this._depthSegments.AsReadOnly();
				}
			}

			// Token: 0x06002A2B RID: 10795 RVA: 0x000B8224 File Offset: 0x000B6424
			public MotorwayDepth(int motorwayId)
			{
				this.motorwayId = motorwayId;
			}

			// Token: 0x06002A2C RID: 10796 RVA: 0x000B8240 File Offset: 0x000B6440
			public void Add(MotorwaySorter.MotorwayDepthSegment newDepthSegment)
			{
				if (this._depthSegments.Count == 0)
				{
					this._depthSegments.Add(newDepthSegment);
					return;
				}
				int depthSegmentIndex = 0;
				while (depthSegmentIndex < this._depthSegments.Count)
				{
					MotorwaySorter.MotorwayDepthSegment depthSegment = this._depthSegments[depthSegmentIndex];
					if (newDepthSegment.startDistance > depthSegment.endDistance)
					{
						if (depthSegmentIndex == this._depthSegments.Count - 1)
						{
							this._depthSegments.Add(newDepthSegment);
							return;
						}
						depthSegmentIndex++;
					}
					else
					{
						if (depthSegment.startDistance > newDepthSegment.endDistance)
						{
							this._depthSegments.Insert(depthSegmentIndex, newDepthSegment);
							return;
						}
						float newStartDistance;
						Vector2 newStartPosition;
						if (depthSegment.startDistance < newDepthSegment.startDistance)
						{
							newStartDistance = depthSegment.startDistance;
							newStartPosition = depthSegment.startPosition;
						}
						else
						{
							newStartDistance = newDepthSegment.startDistance;
							newStartPosition = newDepthSegment.startPosition;
						}
						float newEndDistance;
						Vector2 newEndPosition;
						if (depthSegment.endDistance > newDepthSegment.endDistance)
						{
							newEndDistance = depthSegment.endDistance;
							newEndPosition = depthSegment.endPosition;
						}
						else
						{
							newEndDistance = newDepthSegment.endDistance;
							newEndPosition = newDepthSegment.endPosition;
						}
						depthSegment.startDistance = newStartDistance;
						depthSegment.startPosition = newStartPosition;
						depthSegment.endDistance = newEndDistance;
						depthSegment.endPosition = newEndPosition;
						depthSegment.constraints.AddRange(newDepthSegment.constraints);
						return;
					}
				}
			}

			// Token: 0x04002435 RID: 9269
			public readonly int motorwayId;

			// Token: 0x04002436 RID: 9270
			private readonly List<MotorwaySorter.MotorwayDepthSegment> _depthSegments = new List<MotorwaySorter.MotorwayDepthSegment>();
		}
	}
}
