using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Utility;
using Motorways.Views;
using Motorways.Views.Boats;
using Motorways.Views.Trains;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000459 RID: 1113
	public class LaneCursor : LaneModel.IObserver, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x06001BD3 RID: 7123 RVA: 0x00065984 File Offset: 0x00063B84
		public void MoveToTrain(TrainView train, float stepAlpha, ViewIndex viewIndex, float additionalPrefixLength)
		{
			TrainModel model = train.Model;
			RailTileModel currentRailModel = model.CurrentFrame.tile;
			RailTileModel nextRailModel = model.NextFrame.tile;
			RailTileModel nextRailModelInPath = model.NextFrame.tile.NextRailModel;
			this._currentLineSegment = LineSegment.Null;
			this._distanceAlongCurrentLineSegment = 0f;
			this._currentLineSegmentIndex = -1;
			if (this._previousOriginRailTileModelForPath == currentRailModel && currentRailModel == nextRailModel)
			{
				RailView lastUsedRailView = viewIndex.GetRailView(this._previousOriginRailTileModelForPath);
				this._currentLaneIndex = -1;
				for (int laneIndex = 0; laneIndex < this._path.Count; laneIndex++)
				{
					if (this._path[laneIndex].RepresentsRailView(lastUsedRailView))
					{
						this._currentLaneIndex = laneIndex;
						break;
					}
				}
				if (this._currentLaneIndex != -1)
				{
					this._distanceAlongCurrentLane = Mathf.Lerp((float)model.CurrentFrame.distanceAlongTrack, (float)model.NextFrame.distanceAlongTrack, stepAlpha);
					this._distanceAlongCurrentLane *= this._path[this._currentLaneIndex].LengthRatio;
					return;
				}
			}
			if (this._path.Count > 0)
			{
				this.ClearPath();
			}
			RailTileModel currentModel = currentRailModel;
			while (additionalPrefixLength > 0f && currentModel.PreviousRailModel != null)
			{
				currentModel = currentModel.PreviousRailModel;
				additionalPrefixLength -= (float)currentModel.Length;
				this._path.Insert(0, new LaneCursor.Lane(viewIndex.GetRailView(currentModel)));
			}
			this._distanceAlongCurrentLane = 0f;
			if (currentRailModel == nextRailModel)
			{
				this._currentLaneIndex = this._path.Count;
				this._path.Add(new LaneCursor.Lane(viewIndex.GetRailView(currentRailModel)));
				this._distanceAlongCurrentLane = Mathf.Lerp((float)model.CurrentFrame.distanceAlongTrack, (float)model.NextFrame.distanceAlongTrack, stepAlpha);
				this._distanceAlongCurrentLane *= this._path[this._currentLaneIndex].LengthRatio;
			}
			else
			{
				LaneCursor.Lane currentLane = new LaneCursor.Lane(viewIndex.GetRailView(currentRailModel));
				float currentLaneLengthRatio = currentLane.LengthRatio;
				float distanceAlongCurrentLane = (float)model.CurrentFrame.distanceAlongTrack * currentLaneLengthRatio;
				float currentLaneLength = (float)currentRailModel.Length * currentLaneLengthRatio;
				LaneCursor.Lane nextLane = new LaneCursor.Lane(viewIndex.GetRailView(nextRailModel));
				float nextLaneLengthRatio = nextLane.LengthRatio;
				float distanceAlongNextLane = (float)model.NextFrame.distanceAlongTrack * nextLaneLengthRatio;
				float nextLaneLength = (float)nextRailModel.Length * nextLaneLengthRatio;
				if (currentRailModel.NextRailModel == nextRailModel)
				{
					float distanceTravelledBetweenSteps = distanceAlongNextLane + (currentLaneLength - distanceAlongCurrentLane);
					this._distanceAlongCurrentLane = distanceAlongCurrentLane + distanceTravelledBetweenSteps * stepAlpha;
				}
				else
				{
					float distanceTravelledBetweenSteps2 = distanceAlongCurrentLane + (nextLaneLength - distanceAlongNextLane);
					this._distanceAlongCurrentLane = distanceAlongCurrentLane - distanceTravelledBetweenSteps2 * stepAlpha;
				}
				if (this._distanceAlongCurrentLane > currentLaneLength)
				{
					this._path.Add(currentLane);
					this._currentLaneIndex = this._path.Count;
					this._path.Add(nextLane);
					this._distanceAlongCurrentLane -= currentLaneLength;
				}
				else if (distanceAlongCurrentLane < 0f)
				{
					this._path.Add(currentLane);
					this._currentLaneIndex = this._path.Count;
					this._path.Add(nextLane);
					this._distanceAlongCurrentLane += nextLaneLength;
				}
				else
				{
					this._currentLaneIndex = this._path.Count;
					this._path.Add(currentLane);
					this._path.Add(nextLane);
				}
			}
			if (nextRailModelInPath != null)
			{
				this._path.Add(new LaneCursor.Lane(viewIndex.GetRailView(nextRailModelInPath)));
			}
			this._previousOriginRailTileModelForPath = currentRailModel;
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x00065D24 File Offset: 0x00063F24
		public void MoveToBoat(BoatView boat, ViewIndex viewIndex, float stepAlpha)
		{
			BoatModel model = boat.Model;
			BoatPathTileModel currentModel = model.CurrentFrame.tile;
			BoatPathTileModel nextModel = model.NextFrame.tile;
			BoatPathTileModel nextModelInPath = model.NextFrame.tile.GetNextBoatPathModelInDirection(model.CurrentFrame.direction);
			this._currentLineSegment = LineSegment.Null;
			this._distanceAlongCurrentLineSegment = 0f;
			this._currentLineSegmentIndex = -1;
			if (this._previousOriginBoatPathTileModelForPath == currentModel && currentModel == nextModel)
			{
				this._currentLaneIndex = -1;
				BoatPathView lastUsedBoatPathView = viewIndex.GetBoatPathView(this._previousOriginBoatPathTileModelForPath);
				for (int laneIndex = 0; laneIndex < this._path.Count; laneIndex++)
				{
					if (this._path[laneIndex].RepresentsBoatPathView(lastUsedBoatPathView))
					{
						this._currentLaneIndex = laneIndex;
						break;
					}
				}
				if (this._currentLaneIndex != -1)
				{
					this._distanceAlongCurrentLane = Mathf.Lerp((float)model.CurrentFrame.DistanceAlongPathSegment, (float)model.NextFrame.DistanceAlongPathSegment, stepAlpha);
					this._distanceAlongCurrentLane *= this._path[this._currentLaneIndex].LengthRatio;
					return;
				}
			}
			if (this._path.Count > 0)
			{
				this.ClearPath();
			}
			this._distanceAlongCurrentLane = 0f;
			if (currentModel == nextModel)
			{
				this._currentLaneIndex = this._path.Count;
				this._path.Add(new LaneCursor.Lane(viewIndex.GetBoatPathView(currentModel), model.CurrentFrame.direction));
				this._distanceAlongCurrentLane = Mathf.Lerp((float)model.CurrentFrame.DistanceAlongPathSegment, (float)model.NextFrame.DistanceAlongPathSegment, stepAlpha);
				this._distanceAlongCurrentLane *= this._path[this._currentLaneIndex].LengthRatio;
			}
			else
			{
				LaneCursor.Lane currentLane = new LaneCursor.Lane(viewIndex.GetBoatPathView(currentModel), model.CurrentFrame.direction);
				float currentLaneLengthRatio = currentLane.LengthRatio;
				float distanceAlongCurrentLane = (float)model.CurrentFrame.DistanceAlongPathSegment * currentLaneLengthRatio;
				float currentLaneLength = (float)currentModel.Length * currentLaneLengthRatio;
				LaneCursor.Lane nextLane = new LaneCursor.Lane(viewIndex.GetBoatPathView(nextModel), model.CurrentFrame.direction);
				float nextLaneLengthRatio = nextLane.LengthRatio;
				float distanceAlongNextLane = (float)model.NextFrame.DistanceAlongPathSegment * nextLaneLengthRatio;
				float nextLaneLength = (float)nextModel.Length * nextLaneLengthRatio;
				if (currentModel.GetNextBoatPathModelInDirection(model.CurrentFrame.direction) == nextModel)
				{
					float distanceTravelledBetweenSteps = distanceAlongNextLane + (currentLaneLength - distanceAlongCurrentLane);
					this._distanceAlongCurrentLane = distanceAlongCurrentLane + distanceTravelledBetweenSteps * stepAlpha;
				}
				else
				{
					float distanceTravelledBetweenSteps2 = distanceAlongCurrentLane + (nextLaneLength - distanceAlongNextLane);
					this._distanceAlongCurrentLane = distanceAlongCurrentLane - distanceTravelledBetweenSteps2 * stepAlpha;
				}
				if (this._distanceAlongCurrentLane > currentLaneLength)
				{
					this._path.Add(currentLane);
					this._currentLaneIndex = this._path.Count;
					this._path.Add(nextLane);
					this._distanceAlongCurrentLane -= currentLaneLength;
				}
				else if (distanceAlongCurrentLane < 0f)
				{
					this._path.Add(currentLane);
					this._currentLaneIndex = this._path.Count;
					this._path.Add(nextLane);
					this._distanceAlongCurrentLane += nextLaneLength;
				}
				else
				{
					this._currentLaneIndex = this._path.Count;
					this._path.Add(currentLane);
					this._path.Add(nextLane);
				}
			}
			if (nextModelInPath != null)
			{
				this._path.Add(new LaneCursor.Lane(viewIndex.GetBoatPathView(nextModelInPath), model.CurrentFrame.direction));
			}
			this._previousOriginBoatPathTileModelForPath = currentModel;
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x000660BC File Offset: 0x000642BC
		public void MoveToVehicle(VehicleView vehicle, float stepAlpha)
		{
			VehicleModel vehicleModel = vehicle.Model;
			LaneModel currentLaneModel = vehicleModel.CurrentFrame.lane;
			LaneModel nextLaneModel = vehicleModel.NextFrame.lane;
			LaneModel nextLaneModelInPath = (vehicleModel.path.Count > 0) ? vehicleModel.path[0] : null;
			this._currentLineSegment = LineSegment.Null;
			this._distanceAlongCurrentLineSegment = 0f;
			this._currentLineSegmentIndex = -1;
			if (this._previousOriginLaneModelForPath == currentLaneModel && currentLaneModel == nextLaneModel)
			{
				this._currentLaneIndex = -1;
				for (int laneIndex = 0; laneIndex < this._path.Count; laneIndex++)
				{
					if (this._path[laneIndex].RepresentsLaneModel(this._previousOriginLaneModelForPath))
					{
						this._currentLaneIndex = laneIndex;
						break;
					}
				}
				if (this._currentLaneIndex != -1)
				{
					this._distanceAlongCurrentLane = Mathf.Lerp((float)vehicleModel.CurrentFrame.distanceAlongLane, (float)vehicleModel.NextFrame.distanceAlongLane, stepAlpha);
					this._distanceAlongCurrentLane *= this._path[this._currentLaneIndex].LengthRatio;
					if (this._currentLaneIndex == this._path.Count - 1 && nextLaneModelInPath != null)
					{
						this._path.Add(new LaneCursor.Lane(nextLaneModelInPath, this._tilemapView));
					}
					return;
				}
			}
			if (this._path.Count > 0)
			{
				this.ClearPath();
			}
			if (vehicle.PreviousLaneSegments.Count > 0)
			{
				this._path.Add(new LaneCursor.Lane(vehicle.PreviousLaneSegments));
			}
			this._distanceAlongCurrentLane = 0f;
			if (currentLaneModel == nextLaneModel)
			{
				this._currentLaneIndex = this._path.Count;
				this._path.Add(new LaneCursor.Lane(currentLaneModel, this._tilemapView));
				this._distanceAlongCurrentLane = Mathf.Lerp((float)vehicleModel.CurrentFrame.distanceAlongLane, (float)vehicleModel.NextFrame.distanceAlongLane, stepAlpha);
				this._distanceAlongCurrentLane *= this._path[this._currentLaneIndex].LengthRatio;
			}
			else
			{
				LaneCursor.Lane currentLane = new LaneCursor.Lane(currentLaneModel, this._tilemapView);
				float currentLaneLengthRatio = currentLane.LengthRatio;
				float distanceAlongCurrentLane = (float)vehicleModel.CurrentFrame.distanceAlongLane * currentLaneLengthRatio;
				float currentLaneLength = (float)currentLaneModel.Length * currentLaneLengthRatio;
				LaneCursor.Lane nextLane = new LaneCursor.Lane(nextLaneModel, this._tilemapView);
				float nextLaneLengthRatio = nextLane.LengthRatio;
				float distanceTravelledBetweenSteps = (float)vehicleModel.NextFrame.distanceAlongLane * nextLaneLengthRatio + (currentLaneLength - distanceAlongCurrentLane);
				this._distanceAlongCurrentLane = distanceAlongCurrentLane + distanceTravelledBetweenSteps * stepAlpha;
				if (this._distanceAlongCurrentLane > currentLaneLength)
				{
					this._path.Add(currentLane);
					this._currentLaneIndex = this._path.Count;
					this._path.Add(nextLane);
					this._distanceAlongCurrentLane -= currentLaneLength;
				}
				else
				{
					this._currentLaneIndex = this._path.Count;
					this._path.Add(currentLane);
					this._path.Add(nextLane);
				}
			}
			if (nextLaneModelInPath != null)
			{
				this._path.Add(new LaneCursor.Lane(nextLaneModelInPath, this._tilemapView));
			}
			this._previousOriginLaneModelForPath = currentLaneModel;
			foreach (LaneCursor.Lane lane in this._path)
			{
				LaneModel laneModel = lane.LaneModel;
				if (laneModel != null)
				{
					laneModel.Subscribe(this);
				}
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06001BD6 RID: 7126 RVA: 0x00066444 File Offset: 0x00064644
		public Vector3 Position
		{
			get
			{
				if (this._currentLineSegmentIndex == -1)
				{
					this.FindCurrentLineSegment();
				}
				return this._currentLineSegment.GetPosition(this._distanceAlongCurrentLineSegment);
			}
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x0006646C File Offset: 0x0006466C
		public void Move(float distance)
		{
			this._distanceAlongCurrentLane += distance;
			float laneLength = this._path[this._currentLaneIndex].Length;
			while (this._distanceAlongCurrentLane > laneLength)
			{
				if (this._currentLaneIndex + 1 >= this._path.Count)
				{
					this._distanceAlongCurrentLane = laneLength;
					IL_BB:
					while (this._distanceAlongCurrentLane < 0f)
					{
						if (this._currentLaneIndex <= 0)
						{
							this._distanceAlongCurrentLane = 0f;
							break;
						}
						this._currentLaneIndex--;
						float previousLaneLength = this._path[this._currentLaneIndex].Length;
						this._distanceAlongCurrentLane += previousLaneLength;
					}
					this._currentLineSegmentIndex = -1;
					return;
				}
				this._currentLaneIndex++;
				this._distanceAlongCurrentLane -= laneLength;
			}
			goto IL_BB;
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00066548 File Offset: 0x00064748
		public bool MoveAlongRadius(float distance, out Vector3 position)
		{
			if (!Diagnostics.Verify(distance < 0f, "MoveAlongRadius does not support positive movement yet."))
			{
				position = this.Position;
				return false;
			}
			if (this._currentLaneIndex != -1 && this._path[this._currentLaneIndex].IsStraight)
			{
				float distanceToMoveBackwards = -distance;
				if (this._distanceAlongCurrentLane >= distanceToMoveBackwards)
				{
					this._distanceAlongCurrentLane -= distanceToMoveBackwards;
					this._currentLineSegmentIndex = -1;
					position = this.Position;
					return true;
				}
				if (this._currentLaneIndex >= 1)
				{
					LaneCursor.Lane previousLane = this._path[this._currentLaneIndex - 1];
					if (previousLane.IsStraight && previousLane.Length + this._distanceAlongCurrentLane >= distanceToMoveBackwards)
					{
						this._currentLaneIndex--;
						this._distanceAlongCurrentLane = previousLane.Length - (distanceToMoveBackwards - this._distanceAlongCurrentLane);
						this._currentLineSegmentIndex = -1;
						position = this.Position;
						return true;
					}
				}
			}
			if (this._currentLineSegmentIndex == -1)
			{
				this.FindCurrentLineSegment();
			}
			Circle circle = new Circle(this.Position, Mathf.Abs(distance));
			this.debugLastRadiusMovementCircle = circle;
			bool intersectingWithCursorSegment = true;
			while (!this._currentLineSegment.IsNull)
			{
				this.debugLastRadiusMovementLineSegment = this._currentLineSegment;
				float intersectionCoordinate = this.GetLastIntersectionCoordinate(circle, this._currentLineSegment, intersectingWithCursorSegment ? this._distanceAlongCurrentLineSegment : -1f);
				if (intersectionCoordinate >= 0f)
				{
					float newDistanceAlongCurrentLineSegment = intersectionCoordinate * this._currentLineSegment.Length;
					this._distanceAlongCurrentLane += newDistanceAlongCurrentLineSegment - this._distanceAlongCurrentLineSegment;
					this._distanceAlongCurrentLineSegment = newDistanceAlongCurrentLineSegment;
					position = this.Position;
					return true;
				}
				intersectingWithCursorSegment = false;
				if (!this.MoveToPreviousLineSegment())
				{
					LineSegment extrudedLineSegment = new LineSegment(this._currentLineSegment.Start - this._currentLineSegment.Direction * circle.Radius * 3f, this._currentLineSegment.Start);
					this.debugLastRadiusMovementLineSegment = extrudedLineSegment;
					float extrudedIntersectionCoordinate = this.GetLastIntersectionCoordinate(circle, extrudedLineSegment, -1f);
					if (extrudedIntersectionCoordinate >= 0f)
					{
						position = extrudedLineSegment.GetPosition(extrudedIntersectionCoordinate * extrudedLineSegment.Length);
						return true;
					}
					break;
				}
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x00066791 File Offset: 0x00064991
		public void OnLaneModelReleased(LaneModel laneModel)
		{
			this.ClearPath();
			this._previousOriginLaneModelForPath = null;
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x000667A0 File Offset: 0x000649A0
		private void ClearPath()
		{
			foreach (LaneCursor.Lane lane in this._path)
			{
				LaneModel laneModel = lane.LaneModel;
				if (laneModel != null)
				{
					laneModel.Unsubscribe(this);
				}
			}
			this._path.Clear();
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x0006680C File Offset: 0x00064A0C
		private void FindCurrentLineSegment()
		{
			int lineSegmentCount = this._path[this._currentLaneIndex].LineSegmentCount;
			float distanceTraversed = 0f;
			this._currentLineSegmentIndex = 0;
			while (this._currentLineSegmentIndex < lineSegmentCount)
			{
				this._currentLineSegment = this._path[this._currentLaneIndex].GetLineSegment(this._currentLineSegmentIndex);
				if (distanceTraversed + this._currentLineSegment.Length > this._distanceAlongCurrentLane)
				{
					this._distanceAlongCurrentLineSegment = this._distanceAlongCurrentLane - distanceTraversed;
					break;
				}
				distanceTraversed += this._currentLineSegment.Length;
				this._currentLineSegmentIndex++;
			}
			if (this._currentLineSegmentIndex >= lineSegmentCount)
			{
				this._currentLineSegmentIndex = lineSegmentCount - 1;
				this._distanceAlongCurrentLineSegment = this._currentLineSegment.Length;
			}
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000668D4 File Offset: 0x00064AD4
		private bool MoveToPreviousLineSegment()
		{
			if (this._currentLineSegmentIndex == -1)
			{
				this.FindCurrentLineSegment();
			}
			if (this._currentLineSegmentIndex != 0)
			{
				this._distanceAlongCurrentLane -= this._distanceAlongCurrentLineSegment;
				this._currentLineSegmentIndex--;
				this._currentLineSegment = this._path[this._currentLaneIndex].GetLineSegment(this._currentLineSegmentIndex);
				this._distanceAlongCurrentLineSegment = this._currentLineSegment.Length;
				return true;
			}
			if (this._currentLaneIndex == 0)
			{
				LaneModel unambiguousPreviousLane = this._path[0].UnambiguousPreviousLane;
				if (unambiguousPreviousLane != null)
				{
					bool isPreviousLaneAlreadyInPath = false;
					foreach (LaneCursor.Lane lane in this._path)
					{
						if (lane.RepresentsLaneModel(unambiguousPreviousLane))
						{
							isPreviousLaneAlreadyInPath = true;
							break;
						}
					}
					if (!isPreviousLaneAlreadyInPath)
					{
						unambiguousPreviousLane.Subscribe(this);
						this._path.Insert(0, new LaneCursor.Lane(unambiguousPreviousLane, this._tilemapView));
						this._currentLaneIndex = 1;
					}
				}
			}
			if (this._currentLaneIndex == 0)
			{
				this._distanceAlongCurrentLane = 0f;
				this._distanceAlongCurrentLineSegment = 0f;
				return false;
			}
			this._currentLaneIndex--;
			LaneCursor.Lane newLane = this._path[this._currentLaneIndex];
			this._currentLineSegmentIndex = newLane.LineSegmentCount - 1;
			this._currentLineSegment = newLane.GetLineSegment(this._currentLineSegmentIndex);
			this._distanceAlongCurrentLineSegment = this._currentLineSegment.Length;
			this._distanceAlongCurrentLane = newLane.Length;
			return true;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00066A74 File Offset: 0x00064C74
		private float GetLastIntersectionCoordinate(Circle circle, LineSegment lineSegment, float minimumDistanceAlongLineSegment = -1f)
		{
			Geometry.CircleLineIntersection intersection = Geometry.TryCircleLineSegmentIntersection(circle, lineSegment);
			if (intersection.count > 0)
			{
				float lastIntersectionCoordinate = -1f;
				for (int intersectionIndex = 0; intersectionIndex < intersection.count; intersectionIndex++)
				{
					float intersectionCoordinate = lineSegment.GetParametricCoordinate(intersection.GetIntersection(intersectionIndex));
					if (minimumDistanceAlongLineSegment < 0f || intersectionCoordinate * lineSegment.Length <= minimumDistanceAlongLineSegment)
					{
						lastIntersectionCoordinate = Mathf.Max(lastIntersectionCoordinate, intersectionCoordinate);
					}
				}
				return lastIntersectionCoordinate;
			}
			return -1f;
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00066ADC File Offset: 0x00064CDC
		public void Reset()
		{
			this._currentLaneIndex = 0;
			this._distanceAlongCurrentLane = 0f;
			this._currentLineSegment = LineSegment.Null;
			this._distanceAlongCurrentLineSegment = 0f;
			this._currentLineSegmentIndex = -1;
			this.debugLastRadiusMovementCircle = default(Circle);
			this.debugLastRadiusMovementLineSegment = default(LineSegment);
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x00066B30 File Offset: 0x00064D30
		public void OnReleasedFromScope(IScope scope)
		{
			this.ClearPath();
		}

		// Token: 0x0400172C RID: 5932
		private List<LaneCursor.Lane> _path = new List<LaneCursor.Lane>(4);

		// Token: 0x0400172D RID: 5933
		private int _currentLaneIndex;

		// Token: 0x0400172E RID: 5934
		private float _distanceAlongCurrentLane;

		// Token: 0x0400172F RID: 5935
		private LaneModel _previousOriginLaneModelForPath;

		// Token: 0x04001730 RID: 5936
		private RailTileModel _previousOriginRailTileModelForPath;

		// Token: 0x04001731 RID: 5937
		private BoatPathTileModel _previousOriginBoatPathTileModelForPath;

		// Token: 0x04001732 RID: 5938
		private LineSegment _currentLineSegment;

		// Token: 0x04001733 RID: 5939
		private float _distanceAlongCurrentLineSegment;

		// Token: 0x04001734 RID: 5940
		private int _currentLineSegmentIndex = -1;

		// Token: 0x04001735 RID: 5941
		public Circle debugLastRadiusMovementCircle;

		// Token: 0x04001736 RID: 5942
		public LineSegment debugLastRadiusMovementLineSegment;

		// Token: 0x04001737 RID: 5943
		public List<LineSegment> debugBoatPath = new List<LineSegment>();

		// Token: 0x04001738 RID: 5944
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x0200045A RID: 1114
		private struct Lane
		{
			// Token: 0x06001BE1 RID: 7137 RVA: 0x00066B60 File Offset: 0x00064D60
			public Lane(LaneModel laneModel, TilemapView tilemapView)
			{
				this._laneModel = laneModel;
				this._motorwayView = tilemapView.TryGetMotorwayViewForLane(laneModel);
				this._railView = null;
				this._boatPathView = null;
				this._lineSegments = null;
				this._length = 0f;
				if (this._motorwayView == null)
				{
					this._lineSegments = this._laneModel.GetLineSegments();
					this._length = (float)this._laneModel.Length;
				}
			}

			// Token: 0x06001BE2 RID: 7138 RVA: 0x00066BD8 File Offset: 0x00064DD8
			public Lane(RailView railView)
			{
				this._laneModel = null;
				this._motorwayView = null;
				this._railView = railView;
				this._boatPathView = null;
				this._lineSegments = this._railView.LineSegments;
				this._length = (float)this._railView.Model.Length;
			}

			// Token: 0x06001BE3 RID: 7139 RVA: 0x00066C30 File Offset: 0x00064E30
			public Lane(BoatPathView boatPathView, BoatModel.BoatDirection direction)
			{
				this._laneModel = null;
				this._motorwayView = null;
				this._railView = null;
				this._boatPathView = boatPathView;
				if (direction == BoatModel.BoatDirection.Backwards)
				{
					this._lineSegments = new List<LineSegment>();
					using (List<LineSegment>.Enumerator enumerator = boatPathView.LineSegments.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							LineSegment lineSegment = enumerator.Current;
							this._lineSegments.Insert(0, new LineSegment(lineSegment.End, lineSegment.Start));
						}
						goto IL_85;
					}
				}
				this._lineSegments = boatPathView.LineSegments;
				IL_85:
				this._length = (float)boatPathView.Model.Length;
			}

			// Token: 0x06001BE4 RID: 7140 RVA: 0x00066CEC File Offset: 0x00064EEC
			public Lane(List<LineSegment> lineSegments)
			{
				this._laneModel = null;
				this._motorwayView = null;
				this._railView = null;
				this._boatPathView = null;
				this._lineSegments = lineSegments;
				this._length = 0f;
				foreach (LineSegment segment in this._lineSegments)
				{
					this._length += segment.Length;
				}
			}

			// Token: 0x17000552 RID: 1362
			// (get) Token: 0x06001BE5 RID: 7141 RVA: 0x00066D7C File Offset: 0x00064F7C
			public bool IsStraight
			{
				get
				{
					if (this._railView != null)
					{
						return this._railView.LineSegmentCount == 1;
					}
					if (this._boatPathView != null)
					{
						return this._boatPathView.LineSegmentCount == 1;
					}
					return this._motorwayView == null && this.LineSegmentCount == 1;
				}
			}

			// Token: 0x17000553 RID: 1363
			// (get) Token: 0x06001BE6 RID: 7142 RVA: 0x00066DDC File Offset: 0x00064FDC
			public float Length
			{
				get
				{
					if (this._motorwayView != null)
					{
						return this._motorwayView.GetLaneLength(this._laneModel);
					}
					return this._length;
				}
			}

			// Token: 0x17000554 RID: 1364
			// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x00066E04 File Offset: 0x00065004
			public float LengthRatio
			{
				get
				{
					if (this._laneModel != null)
					{
						return this.Length / (float)this._laneModel.Length;
					}
					return 1f;
				}
			}

			// Token: 0x17000555 RID: 1365
			// (get) Token: 0x06001BE8 RID: 7144 RVA: 0x00066E2C File Offset: 0x0006502C
			public int LineSegmentCount
			{
				get
				{
					if (this._motorwayView != null)
					{
						return this._motorwayView.GetLanePoints(this._laneModel).Count - 1;
					}
					if (this._lineSegments != null)
					{
						return this._lineSegments.Count;
					}
					if (this._railView != null)
					{
						return this._railView.LineSegmentCount;
					}
					if (this._boatPathView != null)
					{
						return this._boatPathView.LineSegmentCount;
					}
					return 0;
				}
			}

			// Token: 0x06001BE9 RID: 7145 RVA: 0x00066EAC File Offset: 0x000650AC
			public LineSegment GetLineSegment(int lineSegmentIndex)
			{
				if (this._lineSegments != null)
				{
					if (!Diagnostics.Verify(lineSegmentIndex < this._lineSegments.Count, "Invalid index for GetLineSegment."))
					{
						lineSegmentIndex = this._lineSegments.Count - 1;
					}
					return this._lineSegments[lineSegmentIndex];
				}
				if (this._motorwayView != null)
				{
					List<Vector2> lanePoints = this._motorwayView.GetLanePoints(this._laneModel);
					if (!Diagnostics.Verify(lineSegmentIndex < lanePoints.Count - 1, "Invalid index for GetLineSegment."))
					{
						lineSegmentIndex = lanePoints.Count - 2;
					}
					return new LineSegment(lanePoints[lineSegmentIndex], lanePoints[lineSegmentIndex + 1]);
				}
				return LineSegment.Null;
			}

			// Token: 0x06001BEA RID: 7146 RVA: 0x00066F53 File Offset: 0x00065153
			public bool RepresentsLaneModel(LaneModel laneModel)
			{
				return this._laneModel == laneModel;
			}

			// Token: 0x06001BEB RID: 7147 RVA: 0x00066F5E File Offset: 0x0006515E
			public bool RepresentsRailView(RailView railView)
			{
				return this._railView == railView;
			}

			// Token: 0x06001BEC RID: 7148 RVA: 0x00066F6C File Offset: 0x0006516C
			public bool RepresentsBoatPathView(BoatPathView boatPathView)
			{
				return this._boatPathView == boatPathView;
			}

			// Token: 0x17000556 RID: 1366
			// (get) Token: 0x06001BED RID: 7149 RVA: 0x00066F7A File Offset: 0x0006517A
			public LaneModel LaneModel
			{
				get
				{
					return this._laneModel;
				}
			}

			// Token: 0x17000557 RID: 1367
			// (get) Token: 0x06001BEE RID: 7150 RVA: 0x00066F82 File Offset: 0x00065182
			public LaneModel UnambiguousPreviousLane
			{
				get
				{
					if (this._laneModel != null && this._laneModel.InboundLanes.Count == 1)
					{
						return this._laneModel.InboundLanes[0];
					}
					return null;
				}
			}

			// Token: 0x04001739 RID: 5945
			private LaneModel _laneModel;

			// Token: 0x0400173A RID: 5946
			private MotorwayView _motorwayView;

			// Token: 0x0400173B RID: 5947
			private RailView _railView;

			// Token: 0x0400173C RID: 5948
			private BoatPathView _boatPathView;

			// Token: 0x0400173D RID: 5949
			private List<LineSegment> _lineSegments;

			// Token: 0x0400173E RID: 5950
			private float _length;
		}
	}
}
