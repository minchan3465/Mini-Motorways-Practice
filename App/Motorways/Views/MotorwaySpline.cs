using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Motorways.Models;
using Motorways.Utility;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005D7 RID: 1495
	public class MotorwaySpline
	{
		// Token: 0x060029BC RID: 10684 RVA: 0x000B35B4 File Offset: 0x000B17B4
		public List<float> CalculateEvenlySpacedSamples(float distanceBetweenSamples, int resolution = 100)
		{
			float num = this.spline.Length();
			List<float> evenlySpacedPoints = new List<float>((int)(num / distanceBetweenSamples) + 1)
			{
				0f
			};
			float distanceSinceLastEvenSample = 0f;
			int divisions = Mathf.CeilToInt(num * (float)resolution);
			float tStep = 1f / (float)divisions;
			float t = tStep;
			Vector2 previousPoint = this.spline.segments[0].inPoint;
			while (t <= 1f)
			{
				Vector2 point = this.spline.Evaluate(t);
				float tentativeDistanceSinceLastEvenSample = distanceSinceLastEvenSample + Vector2.Distance(previousPoint, point);
				if (tentativeDistanceSinceLastEvenSample >= distanceBetweenSamples)
				{
					evenlySpacedPoints.Add(t);
					tentativeDistanceSinceLastEvenSample = 0f;
				}
				distanceSinceLastEvenSample = tentativeDistanceSinceLastEvenSample;
				previousPoint = point;
				t += tStep;
			}
			return evenlySpacedPoints;
		}

		// Token: 0x060029BD RID: 10685 RVA: 0x000B365C File Offset: 0x000B185C
		public void CalculateLinearDistanceLookupTable(float[] linearDistanceTable)
		{
			float totalDistance = 0f;
			linearDistanceTable[0] = 0f;
			Vector2 previousSamplePoint = this.spline.segments[0].inPoint;
			for (int sampleIndex = 1; sampleIndex < linearDistanceTable.Length; sampleIndex++)
			{
				float t = (float)sampleIndex / (float)(linearDistanceTable.Length - 1);
				Vector2 samplePoint = this.spline.Evaluate(t);
				float difference = Vector2.Distance(previousSamplePoint, samplePoint);
				totalDistance += difference;
				linearDistanceTable[sampleIndex] = totalDistance;
				previousSamplePoint = samplePoint;
			}
		}

		// Token: 0x060029BE RID: 10686 RVA: 0x000B36C8 File Offset: 0x000B18C8
		public void RebuildSegments(TileDirection startDirection, TileDirection endDirection, Vector2 startCoordinatesWorldSpace, Vector2 endCoordinatesWorldSpace, Vector2 splineMidpoint, Vector2 naturalMidpoint, Vector2 naturalTangent, float naturalStartHandleLength, float naturalEndHandleLength)
		{
			MotorwaySpline.HandleData startHandle = this.CalculateHandleData(startDirection, startCoordinatesWorldSpace, splineMidpoint, naturalMidpoint, naturalStartHandleLength);
			MotorwaySpline.HandleData endHandle = this.CalculateHandleData(endDirection, endCoordinatesWorldSpace, splineMidpoint, naturalMidpoint, naturalEndHandleLength);
			Vector2 midPointHandleStart = -naturalTangent * (naturalStartHandleLength + naturalEndHandleLength) * 0.25f;
			Vector2 midPointHandleEnd = -midPointHandleStart;
			this.spline = new Spline.PiecewiseBezierSpline(new Spline.BezierSpline[]
			{
				new Spline.BezierSpline(new Vector2(startHandle.splinePosition.x, startHandle.splinePosition.y), new Vector2(startHandle.tileHandlePosition.x, startHandle.tileHandlePosition.y), new Vector2((midPointHandleStart + splineMidpoint).x, (midPointHandleStart + splineMidpoint).y), new Vector2(splineMidpoint.x, splineMidpoint.y)),
				new Spline.BezierSpline(new Vector2(splineMidpoint.x, splineMidpoint.y), new Vector2((splineMidpoint + midPointHandleEnd).x, (splineMidpoint + midPointHandleEnd).y), new Vector2(endHandle.tileHandlePosition.x, endHandle.tileHandlePosition.y), new Vector2(endHandle.splinePosition.x, endHandle.splinePosition.y))
			});
		}

		// Token: 0x060029BF RID: 10687 RVA: 0x000B3810 File Offset: 0x000B1A10
		private MotorwaySpline.HandleData CalculateHandleData(TileDirection tileDirection, Vector2 tileCoordinatesWorldspace, Vector2 splineMidpoint, Vector2 naturalMidpoint, float naturalHandleLength)
		{
			float distanceFromIntersectionScale = (float)TilemapModel.HalfTileWidth;
			float diagonalExtraDistance = Mathf.Sqrt(2f * (float)TilemapModel.HalfTileWidth) - distanceFromIntersectionScale;
			MotorwaySpline.HandleData handleData;
			handleData.direction = TileUtilities.GetVectorForDirection(tileDirection);
			handleData.tilePosition = tileCoordinatesWorldspace + handleData.direction * distanceFromIntersectionScale;
			handleData.splinePosition = handleData.tilePosition + (TileUtilities.IsDirectionDiagonal(tileDirection) ? (handleData.direction * diagonalExtraDistance) : Vector2.zero);
			handleData.proxmityToEnd = (handleData.tilePosition - splineMidpoint).magnitude / (handleData.tilePosition - naturalMidpoint).magnitude;
			handleData.tileHandlePosition = handleData.splinePosition + handleData.direction * naturalHandleLength * 0.5f * handleData.proxmityToEnd;
			return handleData;
		}

		// Token: 0x060029C0 RID: 10688 RVA: 0x000B38FC File Offset: 0x000B1AFC
		public Vector4[] GenerateHazardTapeStripeSamples(float distanceBetweenStripes, float stripeRotationDegrees, float motorwayWidth, float maxHazardStripeWidth, int maxSamples, bool shouldCorrectStripes = true)
		{
			List<float> samples = this.CalculateEvenlySpacedSamples(distanceBetweenStripes, 100);
			Vector4[] hazardTapeSamples = new Vector4[maxSamples];
			int sampleCount = Math.Min(maxSamples, samples.Count);
			for (int sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
			{
				float sample = samples[sampleIndex];
				Vector2 point = this.spline.Evaluate(sample);
				Vector2 rotatedTangent = this.spline.EvaluateTangent(sample).Rotated(-stripeRotationDegrees * 0.017453292f).normalized;
				hazardTapeSamples[sampleIndex] = new Vector4(point.x, point.y, rotatedTangent.x, rotatedTangent.y);
			}
			if (shouldCorrectStripes)
			{
				this.CorrectOverrotatedStripes(hazardTapeSamples, sampleCount, motorwayWidth, maxHazardStripeWidth);
			}
			return hazardTapeSamples;
		}

		// Token: 0x060029C1 RID: 10689 RVA: 0x000B39AA File Offset: 0x000B1BAA
		private float StripeEdgeTestLength(float motorwayWidth)
		{
			return 1.5f * motorwayWidth;
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x000B39B4 File Offset: 0x000B1BB4
		public void CorrectOverrotatedStripes(Vector4[] hazardTapeSamples, int sampleCount, float motorwayWidth, float maxHazardStripeWidth)
		{
			float halfMotorwayWidth = 0.5f * motorwayWidth;
			Spline.RasterizedSpline rightSideSpline = this.spline.Offset(halfMotorwayWidth, 10);
			rightSideSpline.ExtendOutAtEnds(2f);
			Spline.RasterizedSpline leftSideSpline = this.spline.Offset(-halfMotorwayWidth, 10);
			leftSideSpline.ExtendOutAtEnds(2f);
			float stripeEdgeTestLength = this.StripeEdgeTestLength(motorwayWidth);
			for (int hazardSampleIndex = 0; hazardSampleIndex < sampleCount; hazardSampleIndex++)
			{
				Vector4 hazardSample = hazardTapeSamples[hazardSampleIndex];
				Vector2 stripeCentre = new Vector2(hazardSample.x, hazardSample.y);
				Vector2 stripeTangent = new Vector2(hazardSample.z, hazardSample.w);
				Vector2 stripeNormal = stripeTangent.GetNormal();
				Vector2 a = stripeCentre - stripeTangent * 0.5f * maxHazardStripeWidth;
				Vector2 leftEdgeStart = a - stripeNormal * stripeEdgeTestLength;
				Vector2 leftEdgeEnd = a + stripeNormal * stripeEdgeTestLength;
				Vector2 rightEdgeStart = leftEdgeStart + stripeTangent * maxHazardStripeWidth;
				Vector2 rightEdgeEnd = leftEdgeEnd + stripeTangent * maxHazardStripeWidth;
				float rotateBy = this.ComputeCorrectionRotationForEdge(leftEdgeStart, leftEdgeEnd, stripeCentre, leftSideSpline, rightSideSpline);
				if ((double)rotateBy == 0.0)
				{
					rotateBy = this.ComputeCorrectionRotationForEdge(rightEdgeStart, rightEdgeEnd, stripeCentre, leftSideSpline, rightSideSpline);
				}
				if ((double)rotateBy != 0.0)
				{
					Vector2 correctedTangent = -stripeTangent.Rotated(rotateBy);
					hazardTapeSamples[hazardSampleIndex].z = correctedTangent.x;
					hazardTapeSamples[hazardSampleIndex].w = correctedTangent.y;
				}
			}
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x000B3B2C File Offset: 0x000B1D2C
		private int FindLineSegmentCircleIntersections(Vector2 center, float radius, Vector2 start, Vector2 end, out Vector2 intersectionA, out Vector2 intersectionB)
		{
			Vector2 d = end - start;
			float A = d.x * d.x + d.y * d.y;
			float B = 2f * (d.x * (start.x - center.x) + d.y * (start.y - center.y));
			float C = (start.x - center.x) * (start.x - center.x) + (start.y - center.y) * (start.y - center.y) - radius * radius;
			float det = B * B - 4f * A * C;
			if ((double)A <= 1E-07 || det < 0f)
			{
				intersectionA.x = (intersectionA.y = float.NaN);
				intersectionB.x = (intersectionB.y = float.NaN);
				return 0;
			}
			float t;
			if (det == 0f)
			{
				t = -B / (2f * A);
				intersectionA.x = start.x + t * d.x;
				intersectionA.y = start.y + t * d.y;
				intersectionB.x = (intersectionB.y = float.NaN);
				return 1;
			}
			t = (float)(((double)(-(double)B) + Math.Sqrt((double)det)) / (double)(2f * A));
			intersectionA.x = start.x + t * d.x;
			intersectionA.y = start.y + t * d.y;
			t = (float)(((double)(-(double)B) - Math.Sqrt((double)det)) / (double)(2f * A));
			intersectionB.x = start.x + t * d.x;
			intersectionB.y = start.y + t * d.y;
			return 2;
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x000B3D0C File Offset: 0x000B1F0C
		private float CalculateStripeCorrectionRotation(Spline.RasterizedSpline spline, Vector2 stripeCenter, Vector2 stripeEdgeStart, Vector2 stripeEdgeEnd)
		{
			MotorwaySpline.ClosestPointsOnSegment closestPointsOnSegment = MotorwaySpline.ClosestPointOnSplineToLineSegment(spline, stripeEdgeStart, stripeEdgeEnd);
			float radius = Vector2.Distance(stripeCenter, closestPointsOnSegment.CD);
			Vector2 intersectionA;
			Vector2 intersectionB;
			int intersectionCount = this.FindLineSegmentCircleIntersections(stripeCenter, radius, stripeEdgeStart, stripeEdgeEnd, out intersectionA, out intersectionB);
			if (intersectionCount >= 1)
			{
				Vector2 intersection = intersectionA;
				if (intersectionCount >= 2)
				{
					float distanceA = (closestPointsOnSegment.CD - intersectionA).sqrMagnitude;
					if ((closestPointsOnSegment.CD - intersectionB).sqrMagnitude < distanceA)
					{
						intersection = intersectionB;
					}
				}
				Vector2 normalized = (intersection - stripeCenter).normalized;
				Vector2 centreToClosestPoint = (closestPointsOnSegment.CD - stripeCenter).normalized;
				return Mathf.Acos(Vector2.Dot(normalized, centreToClosestPoint));
			}
			return 0f;
		}

		// Token: 0x060029C5 RID: 10693 RVA: 0x000B3DBC File Offset: 0x000B1FBC
		private float ComputeCorrectionRotationForEdge(Vector2 edgeStart, Vector2 edgeEnd, Vector2 centre, Spline.RasterizedSpline leftSpline, Spline.RasterizedSpline rightSpline)
		{
			Vector2 edgeDirection = edgeEnd - edgeStart;
			int leftSplineIntersectionCount = leftSpline.ComputeIntersectionCountWithLineSegment(edgeStart, edgeDirection);
			int rightSplineIntersectionCount = rightSpline.ComputeIntersectionCountWithLineSegment(edgeStart, edgeDirection);
			if (leftSplineIntersectionCount == 0 && rightSplineIntersectionCount != 0)
			{
				return this.CalculateStripeCorrectionRotation(leftSpline, centre, edgeStart, edgeEnd);
			}
			if (leftSplineIntersectionCount != 0 && rightSplineIntersectionCount == 0)
			{
				return this.CalculateStripeCorrectionRotation(rightSpline, centre, edgeStart, edgeEnd);
			}
			return 0f;
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x000B3E10 File Offset: 0x000B2010
		[return: TupleElementNames(new string[]
		{
			"closestStartIndex",
			"closestEndIndex"
		})]
		public static ValueTuple<int, int> ClosestEdgeOnSplineToPoint(Spline.RasterizedSpline spline, Vector2 point)
		{
			int closestStartIndex = -1;
			int closestEndIndex = -1;
			float closestDistance = float.MaxValue;
			int current = 1;
			int previous = 0;
			while (current < spline.Positions.Count)
			{
				Vector2 currentPosition = spline.Positions[current];
				float distanceToMidpoint = Vector2.SqrMagnitude((spline.Positions[previous] + currentPosition) / 2f - point);
				if (distanceToMidpoint < closestDistance)
				{
					closestStartIndex = previous;
					closestEndIndex = current;
					closestDistance = distanceToMidpoint;
				}
				previous = current++;
			}
			return new ValueTuple<int, int>(closestStartIndex, closestEndIndex);
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x000B3E90 File Offset: 0x000B2090
		public static Vector2 ClosestPointOnLineSegmentToPoint(Vector2 P1, Vector2 P2, Vector2 P3)
		{
			float u = ((P3.x - P1.x) * (P2.x - P1.x) + (P3.y - P1.y) * (P2.y - P1.y)) / (P2 - P1).sqrMagnitude;
			if (u <= 0f)
			{
				return P1;
			}
			if (u >= 1f)
			{
				return P2;
			}
			return new Vector2(P1.x + u * (P2.x - P1.x), P1.y + u * (P2.y - P1.y));
		}

		// Token: 0x060029C8 RID: 10696 RVA: 0x000B3F2C File Offset: 0x000B212C
		public static MotorwaySpline.ClosestPointsOnSegment ClosestPointOnLineSegmentToLineSegment(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
		{
			Vector2 ACD = MotorwaySpline.ClosestPointOnLineSegmentToPoint(C, D, A);
			Vector2 BCD = MotorwaySpline.ClosestPointOnLineSegmentToPoint(C, D, B);
			Vector2 CAB = MotorwaySpline.ClosestPointOnLineSegmentToPoint(A, B, C);
			Vector2 DAB = MotorwaySpline.ClosestPointOnLineSegmentToPoint(A, B, D);
			float ACDDistance = (A - ACD).sqrMagnitude;
			float BCDDistance = (B - BCD).sqrMagnitude;
			float CABDistance = (C - CAB).sqrMagnitude;
			float DABDistance = (D - DAB).sqrMagnitude;
			if (ACDDistance < BCDDistance && ACDDistance < CABDistance && ACDDistance < DABDistance)
			{
				return new MotorwaySpline.ClosestPointsOnSegment(A, ACD);
			}
			if (BCDDistance < CABDistance && BCDDistance < DABDistance)
			{
				return new MotorwaySpline.ClosestPointsOnSegment(B, BCD);
			}
			if (CABDistance < DABDistance)
			{
				return new MotorwaySpline.ClosestPointsOnSegment(CAB, C);
			}
			return new MotorwaySpline.ClosestPointsOnSegment(DAB, D);
		}

		// Token: 0x060029C9 RID: 10697 RVA: 0x000B3FE8 File Offset: 0x000B21E8
		public static MotorwaySpline.ClosestPointsOnSegment ClosestPointOnSplineToLineSegment(Spline.RasterizedSpline spline, Vector2 A, Vector2 B)
		{
			List<MotorwaySpline.ClosestPointsOnSegment> closestPointsOnSegments = new List<MotorwaySpline.ClosestPointsOnSegment>();
			for (int positionIndex = 0; positionIndex < spline.Positions.Count - 1; positionIndex++)
			{
				Vector2 C = spline.Positions[positionIndex];
				Vector2 D = spline.Positions[positionIndex + 1];
				closestPointsOnSegments.Add(MotorwaySpline.ClosestPointOnLineSegmentToLineSegment(A, B, C, D));
			}
			float minDistance = float.MaxValue;
			int minIndex = -1;
			for (int closestPointIndex = 0; closestPointIndex < closestPointsOnSegments.Count; closestPointIndex++)
			{
				MotorwaySpline.ClosestPointsOnSegment closestPointsOnSegment = closestPointsOnSegments[closestPointIndex];
				float distance = (closestPointsOnSegment.AB - closestPointsOnSegment.CD).sqrMagnitude;
				if (distance < minDistance)
				{
					minIndex = closestPointIndex;
					minDistance = distance;
				}
			}
			return closestPointsOnSegments[minIndex];
		}

		// Token: 0x060029CA RID: 10698 RVA: 0x000B409C File Offset: 0x000B229C
		public Vector4[] PackSplineSegments()
		{
			Spline.BezierSpline[] segments = this.spline.segments;
			return new Vector4[]
			{
				new Vector4(segments[0].inPoint.x, segments[0].inPoint.y, segments[0].inHandle.x, segments[0].inHandle.y),
				new Vector4(segments[0].outHandle.x, segments[0].outHandle.y, segments[0].outPoint.x, segments[0].outPoint.y),
				new Vector4(segments[1].inPoint.x, segments[1].inPoint.y, segments[1].inHandle.x, segments[1].inHandle.y),
				new Vector4(segments[1].outHandle.x, segments[1].outHandle.y, segments[1].outPoint.x, segments[1].outPoint.y)
			};
		}

		// Token: 0x060029CB RID: 10699 RVA: 0x000B41BC File Offset: 0x000B23BC
		public Vector4[] AddShadowOffsetToSplineSegments(Vector4[] splineSegments, float shadowOffset)
		{
			Vector4[] splineShadowSegments = new Vector4[splineSegments.Length];
			Array.Copy(splineSegments, splineShadowSegments, splineSegments.Length);
			Vector4 offsetVector = new Vector4(shadowOffset, -shadowOffset, shadowOffset, -shadowOffset);
			splineShadowSegments[1] += offsetVector;
			splineShadowSegments[2] += offsetVector;
			return splineShadowSegments;
		}

		// Token: 0x060029CC RID: 10700 RVA: 0x000B421C File Offset: 0x000B241C
		public MotorwaySpline Clone()
		{
			Spline.PiecewiseBezierSpline piecewiseBezierSpline = this.spline;
			if (((piecewiseBezierSpline != null) ? piecewiseBezierSpline.segments : null) == null)
			{
				return new MotorwaySpline();
			}
			Spline.BezierSpline[] clonedSegments = new Spline.BezierSpline[this.spline.segments.Length];
			for (int segmentIndex = 0; segmentIndex < this.spline.segments.Length; segmentIndex++)
			{
				Spline.BezierSpline segment = this.spline.segments[segmentIndex];
				clonedSegments[segmentIndex] = new Spline.BezierSpline(segment.inPoint, segment.inHandle, segment.inHandle, segment.outPoint);
			}
			Spline.PiecewiseBezierSpline clonedSpline = new Spline.PiecewiseBezierSpline(clonedSegments);
			return new MotorwaySpline
			{
				spline = clonedSpline
			};
		}

		// Token: 0x04002384 RID: 9092
		public Spline.PiecewiseBezierSpline spline;

		// Token: 0x04002385 RID: 9093
		private const int _rasterizedResolution = 10;

		// Token: 0x04002386 RID: 9094
		private const int _splineExtensionLength = 2;

		// Token: 0x020005D8 RID: 1496
		private struct HandleData
		{
			// Token: 0x04002387 RID: 9095
			public Vector2 tilePosition;

			// Token: 0x04002388 RID: 9096
			public Vector2 splinePosition;

			// Token: 0x04002389 RID: 9097
			public Vector2 tileHandlePosition;

			// Token: 0x0400238A RID: 9098
			public Vector2 direction;

			// Token: 0x0400238B RID: 9099
			public float proxmityToEnd;
		}

		// Token: 0x020005D9 RID: 1497
		public struct ClosestPointsOnSegment
		{
			// Token: 0x060029CE RID: 10702 RVA: 0x000B42AF File Offset: 0x000B24AF
			public ClosestPointsOnSegment(Vector2 ab, Vector2 cd)
			{
				this.AB = ab;
				this.CD = cd;
			}

			// Token: 0x0400238C RID: 9100
			public Vector2 AB;

			// Token: 0x0400238D RID: 9101
			public Vector2 CD;
		}
	}
}
