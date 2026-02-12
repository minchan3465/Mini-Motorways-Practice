using System;
using System.Collections.Generic;
using Motorways.Utility;
using UnityEngine;
using Utils.Geometry;

namespace Motorways.Views
{
	// Token: 0x020005D2 RID: 1490
	public class MotorwayGeometryInfo
	{
		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x060029AD RID: 10669 RVA: 0x000B2E26 File Offset: 0x000B1026
		public Dictionary<int, MotorwayGeometryInfo.MotorwayEndEdges> EndEdges
		{
			get
			{
				return this._motorwayEndEdges;
			}
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x060029AE RID: 10670 RVA: 0x000B2E2E File Offset: 0x000B102E
		public Dictionary<int, MotorwayPolygon> Polygons
		{
			get
			{
				return this._motorwayPolygons;
			}
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x060029AF RID: 10671 RVA: 0x000B2E36 File Offset: 0x000B1036
		public Dictionary<int, AxisAlignedBoundingBox> Bounds
		{
			get
			{
				return this._motorwayBounds;
			}
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x000B2E3E File Offset: 0x000B103E
		public MotorwayGeometryInfo(MotorwayVisualParameters visualParameters)
		{
			this._visualParameters = visualParameters;
		}

		// Token: 0x060029B1 RID: 10673 RVA: 0x000B2E50 File Offset: 0x000B1050
		public void ComputeGeometryInfo(Dictionary<int, MotorwayView> motorwayViews)
		{
			if (motorwayViews.Count <= 0)
			{
				return;
			}
			float halfRoadWidth = 0.5f * this._visualParameters.roadWidth + this._visualParameters.roadOutlineWidth;
			this._motorwayEndEdges = this.ComputeMotorwayEdges(motorwayViews, halfRoadWidth);
			this._motorwayPolygons = this.ComputeMotorwayPolygons(motorwayViews, halfRoadWidth);
			this._motorwayBounds = this.ComputeBounds(this._motorwayPolygons);
		}

		// Token: 0x060029B2 RID: 10674 RVA: 0x000B2EB4 File Offset: 0x000B10B4
		private Dictionary<int, MotorwayGeometryInfo.MotorwayEndEdges> ComputeMotorwayEdges(Dictionary<int, MotorwayView> motorwayViews, float halfRoadWidth)
		{
			Dictionary<int, MotorwayGeometryInfo.MotorwayEndEdges> motorwayEndEdges = new Dictionary<int, MotorwayGeometryInfo.MotorwayEndEdges>();
			foreach (KeyValuePair<int, MotorwayView> motorwayView in motorwayViews)
			{
				Vector2 a = motorwayView.Value.Spline.spline.Evaluate(0f);
				Vector2 startNormal = motorwayView.Value.Spline.spline.EvaluateTangent(0f).GetNormal().normalized;
				Vector2 startEdgeA = a + halfRoadWidth * startNormal;
				Vector2 startEdgeB = a + halfRoadWidth * -startNormal;
				MotorwayGeometryInfo.MotorwayEndEdge startEdge = new MotorwayGeometryInfo.MotorwayEndEdge(motorwayView.Key, MotorwayGeometryInfo.MotorwayEndEdge.Type.Start, startEdgeA, startEdgeB);
				Vector2 a2 = motorwayView.Value.Spline.spline.Evaluate(1f);
				Vector2 endNormal = motorwayView.Value.Spline.spline.EvaluateTangent(1f).GetNormal().normalized;
				Vector2 endEdgeA = a2 + halfRoadWidth * endNormal;
				Vector2 endEdgeB = a2 + halfRoadWidth * -endNormal;
				MotorwayGeometryInfo.MotorwayEndEdge endEdge = new MotorwayGeometryInfo.MotorwayEndEdge(motorwayView.Key, MotorwayGeometryInfo.MotorwayEndEdge.Type.End, endEdgeA, endEdgeB);
				motorwayEndEdges.Add(motorwayView.Key, new MotorwayGeometryInfo.MotorwayEndEdges(motorwayView.Key, startEdge, endEdge));
			}
			return motorwayEndEdges;
		}

		// Token: 0x060029B3 RID: 10675 RVA: 0x000B3028 File Offset: 0x000B1228
		private Dictionary<int, MotorwayPolygon> ComputeMotorwayPolygons(Dictionary<int, MotorwayView> motorwayViews, float halfRoadWidth)
		{
			Dictionary<int, MotorwayPolygon> motorwayPolygons = new Dictionary<int, MotorwayPolygon>();
			foreach (KeyValuePair<int, MotorwayView> motorwayView in motorwayViews)
			{
				Spline.RasterizedSpline rightSideSpline = motorwayView.Value.Spline.spline.Offset(halfRoadWidth, 10);
				Spline.RasterizedSpline leftSideSpline = motorwayView.Value.Spline.spline.Offset(-halfRoadWidth, 10);
				Vector2Int start = motorwayView.Value.Motorway.StartCoordinates;
				Vector2Int end = motorwayView.Value.Motorway.EndCoordinates;
				if (start.x > end.x || (start.x == end.x && start.y > end.y))
				{
					Spline.RasterizedSpline rasterizedSpline = leftSideSpline;
					Spline.RasterizedSpline rasterizedSpline2 = rightSideSpline;
					rightSideSpline = rasterizedSpline;
					leftSideSpline = rasterizedSpline2;
				}
				List<MotorwayPoint> motorwayPolygonPoints = new List<MotorwayPoint>();
				if (leftSideSpline.Positions.Count > 0)
				{
					motorwayPolygonPoints.Add(new MotorwayPoint(leftSideSpline.Positions[0], MotorwayPointType.LeftEnd));
					for (int positionIndex = 1; positionIndex < leftSideSpline.Positions.Count - 1; positionIndex++)
					{
						new MotorwayPoint(leftSideSpline.Positions[positionIndex], MotorwayPointType.Left);
						motorwayPolygonPoints.Add(new MotorwayPoint(leftSideSpline.Positions[positionIndex], MotorwayPointType.Left));
					}
					motorwayPolygonPoints.Add(new MotorwayPoint(leftSideSpline.Positions[leftSideSpline.Positions.Count - 1], MotorwayPointType.LeftEnd));
				}
				motorwayPolygonPoints.Reverse();
				if (rightSideSpline.Positions.Count > 0)
				{
					motorwayPolygonPoints.Add(new MotorwayPoint(rightSideSpline.Positions[0], MotorwayPointType.RightEnd));
					for (int positionIndex2 = 1; positionIndex2 < rightSideSpline.Positions.Count - 1; positionIndex2++)
					{
						new MotorwayPoint(rightSideSpline.Positions[positionIndex2], MotorwayPointType.Right);
						motorwayPolygonPoints.Add(new MotorwayPoint(rightSideSpline.Positions[positionIndex2], MotorwayPointType.Right));
					}
					motorwayPolygonPoints.Add(new MotorwayPoint(rightSideSpline.Positions[rightSideSpline.Positions.Count - 1], MotorwayPointType.RightEnd));
				}
				motorwayPolygons.Add(motorwayView.Key, new MotorwayPolygon(motorwayView.Key, motorwayPolygonPoints));
			}
			return motorwayPolygons;
		}

		// Token: 0x060029B4 RID: 10676 RVA: 0x000B3280 File Offset: 0x000B1480
		private Dictionary<int, AxisAlignedBoundingBox> ComputeBounds(Dictionary<int, MotorwayPolygon> motorwayPolygons)
		{
			Dictionary<int, AxisAlignedBoundingBox> motorwayBounds = new Dictionary<int, AxisAlignedBoundingBox>();
			foreach (KeyValuePair<int, MotorwayPolygon> motorwayPolygon in motorwayPolygons)
			{
				Vector2 min = Vector2.positiveInfinity;
				Vector2 max = Vector2.negativeInfinity;
				foreach (MotorwayPoint motorwayPoint in motorwayPolygon.Value.points)
				{
					Vector2 position = motorwayPoint.position;
					min.x = ((position.x < min.x) ? position.x : min.x);
					min.y = ((position.y < min.y) ? position.y : min.y);
					max.x = ((position.x > max.x) ? position.x : max.x);
					max.y = ((position.y > max.y) ? position.y : max.y);
				}
				AxisAlignedBoundingBox motorwayBoundingBox = new AxisAlignedBoundingBox(min, max);
				motorwayBounds.Add(motorwayPolygon.Key, motorwayBoundingBox);
			}
			return motorwayBounds;
		}

		// Token: 0x04002370 RID: 9072
		public const int RasterizedResolution = 10;

		// Token: 0x04002371 RID: 9073
		private Dictionary<int, MotorwayGeometryInfo.MotorwayEndEdges> _motorwayEndEdges;

		// Token: 0x04002372 RID: 9074
		private Dictionary<int, MotorwayPolygon> _motorwayPolygons;

		// Token: 0x04002373 RID: 9075
		private Dictionary<int, AxisAlignedBoundingBox> _motorwayBounds;

		// Token: 0x04002374 RID: 9076
		private readonly MotorwayVisualParameters _visualParameters;

		// Token: 0x020005D3 RID: 1491
		public class MotorwayEndEdge
		{
			// Token: 0x060029B5 RID: 10677 RVA: 0x000B33F8 File Offset: 0x000B15F8
			public MotorwayEndEdge(int motorwayId, MotorwayGeometryInfo.MotorwayEndEdge.Type type, Vector2 from, Vector2 to)
			{
				this.motorwayId = motorwayId;
				this.type = type;
				this.from = from;
				this.to = to;
			}

			// Token: 0x04002375 RID: 9077
			public int motorwayId;

			// Token: 0x04002376 RID: 9078
			public MotorwayGeometryInfo.MotorwayEndEdge.Type type;

			// Token: 0x04002377 RID: 9079
			public Vector2 from;

			// Token: 0x04002378 RID: 9080
			public Vector2 to;

			// Token: 0x04002379 RID: 9081
			public readonly List<int> overlappingMotorwayIds = new List<int>();

			// Token: 0x020005D4 RID: 1492
			public enum Type
			{
				// Token: 0x0400237B RID: 9083
				Start,
				// Token: 0x0400237C RID: 9084
				End
			}
		}

		// Token: 0x020005D5 RID: 1493
		public readonly struct MotorwayEndEdges
		{
			// Token: 0x060029B6 RID: 10678 RVA: 0x000B3428 File Offset: 0x000B1628
			public MotorwayEndEdges(int motorwayId, MotorwayGeometryInfo.MotorwayEndEdge startEdge, MotorwayGeometryInfo.MotorwayEndEdge endEdge)
			{
				this.motorwayId = motorwayId;
				this.start = startEdge;
				this.end = endEdge;
			}

			// Token: 0x0400237D RID: 9085
			public readonly int motorwayId;

			// Token: 0x0400237E RID: 9086
			public readonly MotorwayGeometryInfo.MotorwayEndEdge start;

			// Token: 0x0400237F RID: 9087
			public readonly MotorwayGeometryInfo.MotorwayEndEdge end;
		}
	}
}
