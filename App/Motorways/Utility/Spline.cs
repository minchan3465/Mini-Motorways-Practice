using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using UnityEngine;
using Utils.Geometry;

namespace Motorways.Utility
{
	// Token: 0x02000467 RID: 1127
	public static class Spline
	{
		// Token: 0x06001C34 RID: 7220 RVA: 0x00068528 File Offset: 0x00066728
		public static Vector2 EvaluateBezier(float time, Vector2 inPoint, Vector2 inHandle, Vector2 outHandle, Vector2 outPoint)
		{
			float u = 1f - time;
			float uu = u * u;
			float num = uu * u;
			float tt = time * time;
			float ttt = tt * time;
			float pX = num * inPoint.x;
			float pY = num * inPoint.y;
			pX += 3f * uu * time * inHandle.x;
			pY += 3f * uu * time * inHandle.y;
			pX += 3f * u * tt * outHandle.x;
			pY += 3f * u * tt * outHandle.y;
			pX += ttt * outPoint.x;
			pY += ttt * outPoint.y;
			return new Vector2(pX, pY);
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000685D8 File Offset: 0x000667D8
		public static Vector2Fixed EvaluateBezier(Fix64 time, Vector2Fixed inPoint, Vector2Fixed inHandle, Vector2Fixed outHandle, Vector2Fixed outPoint)
		{
			Fix64 u = Fix64.One - time;
			Fix64 uu = u * u;
			Fix64 d = uu * u;
			Fix64 tt = time * time;
			Fix64 ttt = tt * time;
			return d * inPoint + (Fix64)3L * uu * time * inHandle + (Fix64)3L * u * tt * outHandle + ttt * outPoint;
		}

		// Token: 0x02000468 RID: 1128
		public class BezierSpline
		{
			// Token: 0x06001C36 RID: 7222 RVA: 0x0006865F File Offset: 0x0006685F
			public BezierSpline(Vector2 inP, Vector2 inH, Vector2 outH, Vector2 outP)
			{
				this.inPoint = inP;
				this.inHandle = inH;
				this.outHandle = outH;
				this.outPoint = outP;
			}

			// Token: 0x06001C37 RID: 7223 RVA: 0x00068684 File Offset: 0x00066884
			public Vector2 Evaluate(float time)
			{
				return Spline.EvaluateBezier(time, this.inPoint, this.inHandle, this.outHandle, this.outPoint);
			}

			// Token: 0x06001C38 RID: 7224 RVA: 0x000686A4 File Offset: 0x000668A4
			public Vector2 EvaluateLinear(float time)
			{
				return Vector2.Lerp(this.inPoint, this.outPoint, time);
			}

			// Token: 0x06001C39 RID: 7225 RVA: 0x000686B8 File Offset: 0x000668B8
			public Vector2 EvaluateTangent(float time)
			{
				float q0X = this.inPoint.x + (this.inHandle.x - this.inPoint.x) * time;
				float q0Y = this.inPoint.y + (this.inHandle.y - this.inPoint.y) * time;
				float q1X = this.inHandle.x + (this.outHandle.x - this.inHandle.x) * time;
				float q1Y = this.inHandle.y + (this.outHandle.y - this.inHandle.y) * time;
				float q2X = this.outHandle.x + (this.outPoint.x - this.outHandle.x) * time;
				float q2Y = this.outHandle.y + (this.outPoint.y - this.outHandle.y) * time;
				float r0X = q0X + (q1X - q0X) * time;
				float r0Y = q0Y + (q1Y - q0Y) * time;
				float num = q1X + (q2X - q1X) * time;
				float r1Y = q1Y + (q2Y - q1Y) * time;
				float x = num - r0X;
				float tangentY = r1Y - r0Y;
				return new Vector2(x, tangentY);
			}

			// Token: 0x06001C3A RID: 7226 RVA: 0x000687E0 File Offset: 0x000669E0
			public float Length(int resolution = 25)
			{
				float length = 0f;
				for (int i = 0; i < resolution; i++)
				{
					float timeA = 1f / (float)resolution * (float)i;
					Vector2 start = this.Evaluate(timeA);
					float timeB = 1f / (float)resolution * (float)(i + 1);
					Vector2 end = this.Evaluate(timeB);
					length += Vector2.Distance(start, end);
				}
				return length;
			}

			// Token: 0x06001C3B RID: 7227 RVA: 0x00068838 File Offset: 0x00066A38
			public Spline.RasterizedSpline Rasterize(int resolution)
			{
				List<Vector2> rasterizedPoints = new List<Vector2>(resolution);
				float resolutionStep = 1f / (float)(resolution - 1);
				for (int splineIndex = 0; splineIndex < resolution; splineIndex++)
				{
					Vector2 newPosition = this.Evaluate(resolutionStep * (float)splineIndex);
					rasterizedPoints.Add(newPosition);
				}
				return new Spline.RasterizedSpline(rasterizedPoints);
			}

			// Token: 0x06001C3C RID: 7228 RVA: 0x0006887C File Offset: 0x00066A7C
			public Spline.RasterizedSpline RasterizeWithTangents(int resolution)
			{
				List<Vector2> rasterizedPositions = new List<Vector2>(resolution);
				List<Vector2> rasterizedTangents = new List<Vector2>(resolution);
				float resolutionStep = 1f / (float)(resolution - 1);
				for (int splineIndex = 0; splineIndex < resolution; splineIndex++)
				{
					float t = resolutionStep * (float)splineIndex;
					Vector2 position = this.Evaluate(t);
					Vector2 tangent = this.EvaluateTangent(t);
					tangent.Normalize();
					rasterizedPositions.Add(position);
					rasterizedTangents.Add(tangent);
				}
				return new Spline.RasterizedSpline(rasterizedPositions, rasterizedTangents);
			}

			// Token: 0x06001C3D RID: 7229 RVA: 0x000688E8 File Offset: 0x00066AE8
			public Spline.RasterizedSpline RasterizeWithOffset(float distance, int resolution)
			{
				Spline.RasterizedSpline rasterizedSpline = this.RasterizeWithTangents(resolution);
				List<Vector2> offsetSamples = new List<Vector2>(resolution);
				for (int sampleIndex = 0; sampleIndex < rasterizedSpline.Resolution; sampleIndex++)
				{
					Vector2 position = rasterizedSpline.Positions[sampleIndex];
					Vector2 normal = rasterizedSpline.Tangents[sampleIndex].GetNormal();
					offsetSamples.Add(position + normal * distance);
				}
				return new Spline.RasterizedSpline(offsetSamples);
			}

			// Token: 0x06001C3E RID: 7230 RVA: 0x00068950 File Offset: 0x00066B50
			public List<Vector2> EvaluateTangents(int resolution)
			{
				List<Vector2> tangents = new List<Vector2>(resolution);
				float resolutionStep = 1f / (float)(resolution - 1);
				for (int splineIndex = 0; splineIndex < resolution; splineIndex++)
				{
					Vector2 newPosition = this.EvaluateTangent(resolutionStep * (float)splineIndex);
					tangents.Add(newPosition);
				}
				return tangents;
			}

			// Token: 0x06001C3F RID: 7231 RVA: 0x00068990 File Offset: 0x00066B90
			public static Spline.BezierSpline Lerp(Spline.BezierSpline a, Spline.BezierSpline b, float t)
			{
				return new Spline.BezierSpline(Vector2.LerpUnclamped(a.inPoint, b.inPoint, t), Vector2.LerpUnclamped(a.inHandle, b.inHandle, t), Vector2.LerpUnclamped(a.outHandle, b.outHandle, t), Vector2.LerpUnclamped(a.outPoint, b.outPoint, t));
			}

			// Token: 0x06001C40 RID: 7232 RVA: 0x000689EC File Offset: 0x00066BEC
			protected bool Equals(Spline.BezierSpline other)
			{
				return this.inPoint.Equals(other.inPoint) && this.inHandle.Equals(other.inHandle) && this.outHandle.Equals(other.outHandle) && this.outPoint.Equals(other.outPoint);
			}

			// Token: 0x040017E5 RID: 6117
			public readonly Vector2 inPoint;

			// Token: 0x040017E6 RID: 6118
			public readonly Vector2 inHandle;

			// Token: 0x040017E7 RID: 6119
			public readonly Vector2 outHandle;

			// Token: 0x040017E8 RID: 6120
			public readonly Vector2 outPoint;

			// Token: 0x02000469 RID: 1129
			public class Serializer : PrimitiveSerializer
			{
				// Token: 0x06001C41 RID: 7233 RVA: 0x00068A54 File Offset: 0x00066C54
				public override bool Serialize(object obj, ExportContext context)
				{
					Spline.BezierSpline spline = obj as Spline.BezierSpline;
					if (spline != null)
					{
						context.Writer.Write(true);
						ISerializer serializer = SerializerLibrary.GetSerializer<Vector2>();
						serializer.Serialize(spline.inPoint, context);
						serializer.Serialize(spline.inHandle, context);
						serializer.Serialize(spline.outHandle, context);
						serializer.Serialize(spline.outPoint, context);
						return true;
					}
					context.Writer.Write(false);
					return true;
				}

				// Token: 0x06001C42 RID: 7234 RVA: 0x00068AD8 File Offset: 0x00066CD8
				public override object Deserialize(object existingObj, ImportContext context)
				{
					if (context.Reader.ReadBoolean())
					{
						ISerializer vector2Serializer = SerializerLibrary.GetSerializer<Vector2>();
						return new Spline.BezierSpline((Vector2)vector2Serializer.Deserialize(null, context), (Vector2)vector2Serializer.Deserialize(null, context), (Vector2)vector2Serializer.Deserialize(null, context), (Vector2)vector2Serializer.Deserialize(null, context));
					}
					return null;
				}
			}
		}

		// Token: 0x0200046A RID: 1130
		public class BezierSplineWithRotation : Spline.BezierSpline
		{
			// Token: 0x06001C44 RID: 7236 RVA: 0x00068B33 File Offset: 0x00066D33
			public BezierSplineWithRotation(Vector2 inP, Vector2 inH, Vector2 outH, Vector2 outP, Quaternion inRot, Quaternion outRot) : base(inP, inH, outH, outP)
			{
				this.startRotation = inRot;
				this.endRotation = outRot;
			}

			// Token: 0x06001C45 RID: 7237 RVA: 0x00068B50 File Offset: 0x00066D50
			public Quaternion EvaluateRotation(float time)
			{
				return Quaternion.Slerp(this.startRotation, this.endRotation, time);
			}

			// Token: 0x040017E9 RID: 6121
			public readonly Quaternion startRotation;

			// Token: 0x040017EA RID: 6122
			public readonly Quaternion endRotation;
		}

		// Token: 0x0200046B RID: 1131
		public class BezierSplineFixed
		{
			// Token: 0x06001C46 RID: 7238 RVA: 0x00068B64 File Offset: 0x00066D64
			public BezierSplineFixed(Vector2Fixed inP, Vector2Fixed inH, Vector2Fixed outH, Vector2Fixed outP)
			{
				this.inPoint = inP;
				this.inHandle = inH;
				this.outHandle = outH;
				this.outPoint = outP;
			}

			// Token: 0x06001C47 RID: 7239 RVA: 0x00068B89 File Offset: 0x00066D89
			public Vector2Fixed Evaluate(Fix64 time)
			{
				return Spline.EvaluateBezier(time, this.inPoint, this.inHandle, this.outHandle, this.outPoint);
			}

			// Token: 0x06001C48 RID: 7240 RVA: 0x00068BAC File Offset: 0x00066DAC
			public Fix64 Length(int resolution = 25)
			{
				Fix64 length = Fix64.Zero;
				Fix64 step = Fix64.One / (Fix64)((long)resolution);
				for (int i = 0; i < resolution; i++)
				{
					Fix64 timeA = step * (Fix64)((long)i);
					Vector2Fixed start = this.Evaluate(timeA);
					Fix64 timeB = step * (Fix64)((long)(i + 1));
					Vector2Fixed end = this.Evaluate(timeB);
					length += Vector2Fixed.Distance(start, end);
				}
				return length;
			}

			// Token: 0x06001C49 RID: 7241 RVA: 0x00068C20 File Offset: 0x00066E20
			public List<Vector2Fixed> Rasterize(int resolution)
			{
				List<Vector2Fixed> rasterizedPoints = new List<Vector2Fixed>(resolution);
				Fix64 resolutionStep = Fix64.One / ((Fix64)((long)resolution) - Fix64.One);
				Fix64 splineIndex = Fix64.Zero;
				while (splineIndex < (Fix64)((long)resolution))
				{
					Vector2Fixed newPosition = this.Evaluate(resolutionStep * splineIndex);
					rasterizedPoints.Add(newPosition);
					splineIndex += Fix64.One;
				}
				return rasterizedPoints;
			}

			// Token: 0x040017EB RID: 6123
			public readonly Vector2Fixed inPoint;

			// Token: 0x040017EC RID: 6124
			public readonly Vector2Fixed inHandle;

			// Token: 0x040017ED RID: 6125
			public readonly Vector2Fixed outHandle;

			// Token: 0x040017EE RID: 6126
			public readonly Vector2Fixed outPoint;

			// Token: 0x0200046C RID: 1132
			public class Serializer : PrimitiveSerializer
			{
				// Token: 0x06001C4A RID: 7242 RVA: 0x00068C8C File Offset: 0x00066E8C
				public override bool Serialize(object obj, ExportContext context)
				{
					Spline.BezierSplineFixed spline = obj as Spline.BezierSplineFixed;
					if (spline != null)
					{
						context.Writer.Write(true);
						ISerializer serializer = SerializerLibrary.GetSerializer<Vector2Fixed>();
						serializer.Serialize(spline.inPoint, context);
						serializer.Serialize(spline.inHandle, context);
						serializer.Serialize(spline.outHandle, context);
						serializer.Serialize(spline.outPoint, context);
						return true;
					}
					context.Writer.Write(false);
					return true;
				}

				// Token: 0x06001C4B RID: 7243 RVA: 0x00068D10 File Offset: 0x00066F10
				public override object Deserialize(object existingObj, ImportContext context)
				{
					if (context.Reader.ReadBoolean())
					{
						ISerializer vector2FixedSerializer = SerializerLibrary.GetSerializer<Vector2Fixed>();
						return new Spline.BezierSplineFixed((Vector2Fixed)vector2FixedSerializer.Deserialize(null, context), (Vector2Fixed)vector2FixedSerializer.Deserialize(null, context), (Vector2Fixed)vector2FixedSerializer.Deserialize(null, context), (Vector2Fixed)vector2FixedSerializer.Deserialize(null, context));
					}
					return null;
				}
			}
		}

		// Token: 0x0200046D RID: 1133
		public class PiecewiseBezierSpline
		{
			// Token: 0x06001C4D RID: 7245 RVA: 0x00068D6B File Offset: 0x00066F6B
			public PiecewiseBezierSpline(Spline.BezierSpline[] segments)
			{
				this.segments = segments;
			}

			// Token: 0x06001C4E RID: 7246 RVA: 0x00068D7C File Offset: 0x00066F7C
			public Vector2 Evaluate(float t)
			{
				Spline.PiecewiseBezierSpline.IndexInfo indexInfo = this.ComputeIndexInfo(t);
				return this.segments[indexInfo.segmentIndex].Evaluate(indexInfo.tValue);
			}

			// Token: 0x06001C4F RID: 7247 RVA: 0x00068DAC File Offset: 0x00066FAC
			public Vector2 EvaluateTangent(float t)
			{
				Spline.PiecewiseBezierSpline.IndexInfo indexInfo = this.ComputeIndexInfo(t);
				return this.segments[indexInfo.segmentIndex].EvaluateTangent(indexInfo.tValue);
			}

			// Token: 0x06001C50 RID: 7248 RVA: 0x00068DDC File Offset: 0x00066FDC
			public float Length()
			{
				float length = 0f;
				foreach (Spline.BezierSpline spline in this.segments)
				{
					length += spline.Length(25);
				}
				return length;
			}

			// Token: 0x06001C51 RID: 7249 RVA: 0x00068E14 File Offset: 0x00067014
			private Spline.PiecewiseBezierSpline.IndexInfo ComputeIndexInfo(float t)
			{
				if ((double)t == 1.0)
				{
					return new Spline.PiecewiseBezierSpline.IndexInfo(this.segments.Length - 1, 1f);
				}
				float num = t * (float)this.segments.Length;
				int segmentIndex = (int)Math.Floor((double)num);
				float rescaledT = num - (float)segmentIndex;
				if (Diagnostics.Verify(segmentIndex >= 0 && segmentIndex <= this.segments.Length))
				{
					return new Spline.PiecewiseBezierSpline.IndexInfo(segmentIndex, rescaledT);
				}
				return new Spline.PiecewiseBezierSpline.IndexInfo(-1, 0f);
			}

			// Token: 0x06001C52 RID: 7250 RVA: 0x00068E8C File Offset: 0x0006708C
			public Spline.RasterizedSpline Offset(float distance, int resolution)
			{
				int resolutionPerSegment = resolution / this.segments.Length;
				int totalResolution = resolutionPerSegment * this.segments.Length;
				int leftOver = resolution - totalResolution;
				Spline.RasterizedSpline offsetSpline = new Spline.RasterizedSpline(totalResolution + leftOver);
				for (int segmentIndex = 0; segmentIndex < this.segments.Length; segmentIndex++)
				{
					Spline.BezierSpline spline = this.segments[segmentIndex];
					int resolutionForSegment = (segmentIndex == 0) ? (resolutionPerSegment + leftOver) : resolutionPerSegment;
					offsetSpline.Append(spline.RasterizeWithOffset(distance, resolutionForSegment));
				}
				return offsetSpline;
			}

			// Token: 0x06001C53 RID: 7251 RVA: 0x00068EFC File Offset: 0x000670FC
			public Spline.RasterizedSpline Rasterize(int resolution)
			{
				int resolutionPerSegment = resolution / this.segments.Length;
				int totalResolution = resolutionPerSegment * this.segments.Length;
				int leftOver = resolution - totalResolution;
				Spline.RasterizedSpline offsetSpline = new Spline.RasterizedSpline(totalResolution + leftOver);
				for (int segmentIndex = 0; segmentIndex < this.segments.Length; segmentIndex++)
				{
					Spline.BezierSpline spline = this.segments[segmentIndex];
					int resolutionForSegment = (segmentIndex == 0) ? (resolutionPerSegment + leftOver) : resolutionPerSegment;
					offsetSpline.Append(spline.Rasterize(resolutionForSegment));
				}
				return offsetSpline;
			}

			// Token: 0x06001C54 RID: 7252 RVA: 0x00068F6C File Offset: 0x0006716C
			public Spline.RasterizedSpline RasterizeWithTangents(int resolution)
			{
				int resolutionPerSegment = resolution / this.segments.Length;
				int totalResolution = resolutionPerSegment * this.segments.Length;
				int leftOver = resolution - totalResolution;
				Spline.RasterizedSpline offsetSpline = new Spline.RasterizedSpline(totalResolution + leftOver);
				for (int segmentIndex = 0; segmentIndex < this.segments.Length; segmentIndex++)
				{
					Spline.BezierSpline spline = this.segments[segmentIndex];
					int resolutionForSegment = (segmentIndex == 0) ? (resolutionPerSegment + leftOver) : resolutionPerSegment;
					offsetSpline.Append(spline.RasterizeWithTangents(resolutionForSegment));
				}
				return offsetSpline;
			}

			// Token: 0x06001C55 RID: 7253 RVA: 0x00068FDC File Offset: 0x000671DC
			public override bool Equals(object obj)
			{
				Spline.PiecewiseBezierSpline piecewiseBezierSpline = obj as Spline.PiecewiseBezierSpline;
				if (piecewiseBezierSpline == null)
				{
					return false;
				}
				if (piecewiseBezierSpline.segments.Length != this.segments.Length)
				{
					return false;
				}
				for (int segmentIndex = 0; segmentIndex < piecewiseBezierSpline.segments.Length; segmentIndex++)
				{
					object obj2 = piecewiseBezierSpline.segments[segmentIndex];
					Spline.BezierSpline thisSegment = this.segments[segmentIndex];
					if (!obj2.Equals(thisSegment))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06001C56 RID: 7254 RVA: 0x00069038 File Offset: 0x00067238
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x040017EF RID: 6127
			public Spline.BezierSpline[] segments;

			// Token: 0x0200046E RID: 1134
			private struct IndexInfo
			{
				// Token: 0x06001C57 RID: 7255 RVA: 0x00069040 File Offset: 0x00067240
				public IndexInfo(int segmentIndex, float tValue)
				{
					this.segmentIndex = segmentIndex;
					this.tValue = tValue;
				}

				// Token: 0x040017F0 RID: 6128
				public int segmentIndex;

				// Token: 0x040017F1 RID: 6129
				public float tValue;
			}
		}

		// Token: 0x0200046F RID: 1135
		public class RasterizedSpline
		{
			// Token: 0x06001C58 RID: 7256 RVA: 0x00069050 File Offset: 0x00067250
			public RasterizedSpline(int sampleCapacity)
			{
				this._positionSamples = new List<Vector2>(sampleCapacity);
				this._tangentSamples = null;
			}

			// Token: 0x06001C59 RID: 7257 RVA: 0x0006906B File Offset: 0x0006726B
			public RasterizedSpline(List<Vector2> positionSamples)
			{
				this._positionSamples = positionSamples;
				this._tangentSamples = null;
			}

			// Token: 0x06001C5A RID: 7258 RVA: 0x00069081 File Offset: 0x00067281
			public RasterizedSpline(List<Vector2> positionSamples, List<Vector2> tangentSamples)
			{
				this._positionSamples = positionSamples;
				this._tangentSamples = tangentSamples;
			}

			// Token: 0x17000566 RID: 1382
			// (get) Token: 0x06001C5B RID: 7259 RVA: 0x00069097 File Offset: 0x00067297
			public List<Vector2> Positions
			{
				get
				{
					return this._positionSamples;
				}
			}

			// Token: 0x17000567 RID: 1383
			// (get) Token: 0x06001C5C RID: 7260 RVA: 0x0006909F File Offset: 0x0006729F
			public List<Vector2> Tangents
			{
				get
				{
					return this._tangentSamples;
				}
			}

			// Token: 0x17000568 RID: 1384
			// (get) Token: 0x06001C5D RID: 7261 RVA: 0x000690A7 File Offset: 0x000672A7
			public int Resolution
			{
				get
				{
					return this._positionSamples.Count;
				}
			}

			// Token: 0x06001C5E RID: 7262 RVA: 0x000690B4 File Offset: 0x000672B4
			public void Append(Spline.RasterizedSpline rasterizedSpline)
			{
				if (this._positionSamples.Count > 0 && rasterizedSpline._positionSamples.Count > 0 && this._positionSamples[(this._positionSamples.Count == 1) ? 0 : (this._positionSamples.Count - 1)] == rasterizedSpline._positionSamples[0])
				{
					this._positionSamples.RemoveAt(this._positionSamples.Count - 1);
					List<Vector2> tangentSamples = this._tangentSamples;
					if (tangentSamples != null)
					{
						tangentSamples.RemoveAt(this._tangentSamples.Count - 1);
					}
				}
				this._positionSamples.AddRange(rasterizedSpline._positionSamples);
				if (rasterizedSpline._tangentSamples != null)
				{
					if (this._tangentSamples == null)
					{
						this._tangentSamples = new List<Vector2>(rasterizedSpline._tangentSamples.Count);
					}
					this._tangentSamples.AddRange(rasterizedSpline._tangentSamples);
				}
			}

			// Token: 0x06001C5F RID: 7263 RVA: 0x0006919C File Offset: 0x0006739C
			public void Truncate(float maxLength)
			{
				int lineSegmentIndex = 0;
				while (lineSegmentIndex < this._positionSamples.Count - 1)
				{
					Vector2 lineSegmentDirection = this._positionSamples[lineSegmentIndex + 1] - this._positionSamples[lineSegmentIndex];
					float lineSegmentLength = lineSegmentDirection.magnitude;
					if (lineSegmentLength >= maxLength)
					{
						this._positionSamples[lineSegmentIndex + 1] = this._positionSamples[lineSegmentIndex] + lineSegmentDirection * (maxLength / lineSegmentLength);
						this._positionSamples.RemoveRange(lineSegmentIndex + 2, this._positionSamples.Count - (lineSegmentIndex + 2));
						if (this._tangentSamples != null)
						{
							this._tangentSamples[lineSegmentIndex + 1] = this._tangentSamples[lineSegmentIndex] + lineSegmentDirection * (maxLength / lineSegmentLength);
							this._tangentSamples.RemoveRange(lineSegmentIndex + 2, this._tangentSamples.Count - (lineSegmentIndex + 2));
							return;
						}
						break;
					}
					else
					{
						maxLength -= lineSegmentLength;
						lineSegmentIndex++;
					}
				}
			}

			// Token: 0x17000569 RID: 1385
			// (get) Token: 0x06001C60 RID: 7264 RVA: 0x00069290 File Offset: 0x00067490
			public float Length
			{
				get
				{
					if (this._positionSamples.Count <= 0)
					{
						return 0f;
					}
					Vector2 previous = this._positionSamples[0];
					float length = 0f;
					for (int positionIndex = 1; positionIndex < this._positionSamples.Count; positionIndex++)
					{
						Vector2 current = this._positionSamples[positionIndex];
						length += Vector2.Distance(current, previous);
						previous = current;
					}
					return length;
				}
			}

			// Token: 0x06001C61 RID: 7265 RVA: 0x000692F4 File Offset: 0x000674F4
			public int ComputeIntersectionCountWithLineSegment(Vector2 origin, Vector2 direction)
			{
				if (this._positionSamples.Count <= 0)
				{
					return 0;
				}
				int intersectionCount = 0;
				Vector2 previousPosition = this._positionSamples[0];
				for (int sampleIndex = 1; sampleIndex < this.Resolution; sampleIndex++)
				{
					Vector2 vector = this._positionSamples[sampleIndex];
					Vector2 previousToCurrentDirection = vector - previousPosition;
					float num;
					float num2;
					if (LineIntersection.LineLineIntersection(previousPosition.x, previousPosition.y, previousToCurrentDirection.x, previousToCurrentDirection.y, origin.x, origin.y, direction.x, direction.y, out num, out num2) == LineIntersection.Point)
					{
						intersectionCount++;
					}
					previousPosition = vector;
				}
				return intersectionCount;
			}

			// Token: 0x06001C62 RID: 7266 RVA: 0x0006938C File Offset: 0x0006758C
			public List<Vector2> ComputeIntersectionsWithLineSegment(Vector2 origin, Vector2 direction)
			{
				List<Vector2> intersectionPositions = new List<Vector2>();
				if (this._positionSamples.Count <= 0)
				{
					return intersectionPositions;
				}
				Vector2 previousPosition = this._positionSamples[0];
				for (int sampleIndex = 1; sampleIndex < this.Resolution; sampleIndex++)
				{
					Vector2 vector = this._positionSamples[sampleIndex];
					Vector2 previousToCurrentDirection = vector - previousPosition;
					LineIntersection.IntersectionInfo intersection = LineIntersection.LineLineIntersection(previousPosition, previousToCurrentDirection, origin, direction);
					if (intersection.type == LineIntersection.IntersectionInfo.IntersectionType.Point)
					{
						intersectionPositions.Add(intersection.intersection);
					}
					previousPosition = vector;
				}
				return intersectionPositions;
			}

			// Token: 0x06001C63 RID: 7267 RVA: 0x00069404 File Offset: 0x00067604
			public void ExtendOutAtEnds(float distance)
			{
				if (this._positionSamples.Count < 2)
				{
					return;
				}
				Vector2 startDirection = (this._positionSamples[0] - this._positionSamples[1]).normalized;
				Vector2 endDirection;
				if (this._positionSamples.Count == 2)
				{
					endDirection = -startDirection;
				}
				else
				{
					endDirection = (this._positionSamples[this._positionSamples.Count - 1] - this._positionSamples[this._positionSamples.Count - 2]).normalized;
				}
				this._positionSamples.Insert(0, this._positionSamples[0] + distance * startDirection);
				List<Vector2> tangentSamples = this._tangentSamples;
				if (tangentSamples != null)
				{
					tangentSamples.Insert(0, this._tangentSamples[0]);
				}
				this._positionSamples.Add(this._positionSamples[this._positionSamples.Count - 1] + distance * endDirection);
				List<Vector2> tangentSamples2 = this._tangentSamples;
				if (tangentSamples2 == null)
				{
					return;
				}
				tangentSamples2.Add(this._tangentSamples[this._tangentSamples.Count - 1]);
			}

			// Token: 0x06001C64 RID: 7268 RVA: 0x00069534 File Offset: 0x00067734
			public Spline.RasterizedSpline Offset(float distance)
			{
				if (this.Tangents == null)
				{
					Diagnostics.FailAssert("Cannot offset RasterizedSpline as it has no tangents.", Array.Empty<object>());
					return null;
				}
				List<Vector2> offsetSamples = new List<Vector2>(this.Resolution);
				for (int sampleIndex = 0; sampleIndex < this.Resolution; sampleIndex++)
				{
					Vector2 position = this.Positions[sampleIndex];
					Vector2 normal = this.Tangents[sampleIndex].GetNormal();
					offsetSamples.Add(position + normal * distance);
				}
				return new Spline.RasterizedSpline(offsetSamples, this.Tangents);
			}

			// Token: 0x040017F2 RID: 6130
			private List<Vector2> _positionSamples;

			// Token: 0x040017F3 RID: 6131
			private List<Vector2> _tangentSamples;
		}
	}
}
