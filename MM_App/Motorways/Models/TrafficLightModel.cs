using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x02000504 RID: 1284
	public class TrafficLightModel : Model<EmptyModelFrame, TrafficLightModel.IObserver>
	{
		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06002223 RID: 8739 RVA: 0x00089E14 File Offset: 0x00088014
		public TileDirectionBitfield ActivePair
		{
			get
			{
				if (this._currentPairIndex < 0 || this._currentPairIndex >= this.greenLightPairs.Count)
				{
					return default(TileDirectionBitfield);
				}
				return this.greenLightPairs[this._currentPairIndex];
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06002224 RID: 8740 RVA: 0x00089E58 File Offset: 0x00088058
		public TileDirectionBitfield BlockedLanes
		{
			get
			{
				return ~this.ActivePair;
			}
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x00089E65 File Offset: 0x00088065
		public virtual void Initialize(RoadChunkModel roadChunk)
		{
			this._owningChunk = roadChunk;
			this.greenLightPairs.Clear();
			this.CalculatePairs();
			this.RotateLights();
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x00089E88 File Offset: 0x00088088
		public void CalculatePairs()
		{
			this.greenLightPairs.Clear();
			TileDirectionBitfield inputs = this._owningChunk.GetInboundDirections();
			for (int tileDirectionIndex = 0; tileDirectionIndex < 4; tileDirectionIndex++)
			{
				TileDirection direction = (TileDirection)tileDirectionIndex;
				TileDirection opposite = TileUtilities.GetOppositeDirection(direction);
				if (inputs[direction] && inputs[opposite])
				{
					this.greenLightPairs.Add(new TileDirectionBitfield(new TileDirection[]
					{
						direction,
						opposite
					}));
					inputs[direction] = false;
					inputs[opposite] = false;
				}
			}
			for (int tileDirectionIndex2 = 0; tileDirectionIndex2 < 8; tileDirectionIndex2++)
			{
				TileDirection direction2 = (TileDirection)tileDirectionIndex2;
				TileDirection oppositeLeft = TileUtilities.GetRotatedDirection(direction2, 3);
				TileDirection oppositeRight = TileUtilities.GetRotatedDirection(direction2, -3);
				if (!inputs[oppositeLeft] || !inputs[oppositeRight])
				{
					if (inputs[direction2] && inputs[oppositeLeft])
					{
						this.greenLightPairs.Add(new TileDirectionBitfield(new TileDirection[]
						{
							direction2,
							oppositeLeft
						}));
						inputs[direction2] = false;
						inputs[oppositeLeft] = false;
					}
					else if (inputs[direction2] && inputs[oppositeRight])
					{
						this.greenLightPairs.Add(new TileDirectionBitfield(new TileDirection[]
						{
							direction2,
							oppositeRight
						}));
						inputs[direction2] = false;
						inputs[oppositeRight] = false;
					}
				}
			}
			foreach (TileDirection direction3 in inputs)
			{
				this.greenLightPairs.Add(new TileDirectionBitfield(new TileDirection[]
				{
					direction3
				}));
				inputs[direction3] = false;
			}
			this.requiresPairCalculation = false;
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x0008A02C File Offset: 0x0008822C
		public bool SetActivePair(TileDirectionBitfield pair)
		{
			bool result = true;
			int index = this.greenLightPairs.IndexOf(pair);
			if (index == -1)
			{
				index = -1;
				result = false;
			}
			this._currentPairIndex = index;
			return result;
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x0008A058 File Offset: 0x00088258
		public void OnLanesChanged()
		{
			this.requiresPairCalculation = true;
			foreach (TrafficLightModel.IObserver observer in base.Observers)
			{
				observer.OnLanesChanged();
			}
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x0008A090 File Offset: 0x00088290
		public void ChangeGreenToAmber()
		{
			this.amberLightsOn = true;
			foreach (TrafficLightModel.IObserver observer in base.Observers)
			{
				observer.OnTrafficLightAmber(this);
			}
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x0008A0C8 File Offset: 0x000882C8
		public void RotateLights()
		{
			if (this.greenLightPairs.Count == 0)
			{
				return;
			}
			this._currentPairIndex = this.NextValidPairIndex();
			this.amberLightsOn = false;
			if (this._currentPairIndex != -1)
			{
				foreach (TrafficLightModel.IObserver observer in base.Observers)
				{
					observer.OnTrafficLightGreen(this, this.greenLightPairs[this._currentPairIndex]);
				}
			}
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x0008A134 File Offset: 0x00088334
		public bool RequiresRotation()
		{
			if (this.requiresPairCalculation)
			{
				TileDirectionBitfield cachedActivePair = this.ActivePair;
				this.CalculatePairs();
				this.SetActivePair(cachedActivePair);
			}
			if (this.greenLightPairs.Count == 0)
			{
				return false;
			}
			if (this._owningChunk.inboundVehicles.Count > 0)
			{
				if (this._constants.distanceToCountForNearbyCars > Fix64.Zero)
				{
					int numberOfCarsForCurrentPair = this.NumberOfCarsForPair(this._currentPairIndex, true, true);
					if (numberOfCarsForCurrentPair < this._constants.minimumNearbyCarsBeforeSwapping && numberOfCarsForCurrentPair > 0)
					{
						return false;
					}
				}
				int highestWeightedIndex = this.HighestWeightedGreenLightPair(false);
				return this._currentPairIndex != highestWeightedIndex && this._currentPairIndex != this.NextValidPairIndex();
			}
			return false;
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x0008A1E0 File Offset: 0x000883E0
		private int NextValidPairIndex()
		{
			if (this.greenLightPairs.Count == 0)
			{
				return -1;
			}
			if (this._owningChunk.inboundVehicles.Count > 0)
			{
				return this.HighestWeightedGreenLightPair(true);
			}
			return (this._currentPairIndex + 1) % this.greenLightPairs.Count;
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x0008A220 File Offset: 0x00088420
		private int HighestWeightedGreenLightPair(bool ignoreCurrentPair = false)
		{
			Fix64 mostCars = Fix64.Zero;
			int mostCarsIndex = 0;
			for (int pairIndex = 0; pairIndex < this.greenLightPairs.Count; pairIndex++)
			{
				if (!ignoreCurrentPair || pairIndex != this._currentPairIndex)
				{
					Fix64 weightForPair = this.WeightForPair(pairIndex, false);
					if (weightForPair > mostCars)
					{
						mostCarsIndex = pairIndex;
						mostCars = weightForPair;
					}
				}
			}
			if (mostCars == Fix64.Zero)
			{
				mostCarsIndex = Mathf.Max(this._currentPairIndex, 0);
			}
			return mostCarsIndex;
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x0008A288 File Offset: 0x00088488
		public Fix64 WeightForPair(int index, bool onlyNearbyCars = false)
		{
			Fix64 weight = Fix64.Zero;
			foreach (TileDirection direction in this.greenLightPairs[index])
			{
				bool carsCanTurnRight = true;
				foreach (RoadChunkModel.InboundVehicle vehicle in this._owningChunk.InboundVehiclesEnteringFromDirection(direction, onlyNearbyCars ? this._constants.distanceToCountForNearbyCars : (-Fix64.One)))
				{
					if (carsCanTurnRight && this._owningChunk.ConnectionCrossesLane(vehicle.chosenLane.connection.input.direction, vehicle.chosenLane.connection.output.direction))
					{
						carsCanTurnRight = false;
					}
					Fix64 extraWeight = Fix64.Clamp01((this._clock.Time - vehicle.committedTimestamp) / this._constants.MaximumIdleTimeAtTrafficLightBeforeMaxWeight);
					extraWeight = extraWeight * extraWeight * this._constants.IdleTimeAtTrafficLightWeightMultiplier;
					if (vehicle.chosenLane.state == RoadState.Mothballed)
					{
						extraWeight *= this._constants.IdleTimeAtTrafficLightWeightMultiplierOnMothballedLane;
					}
					if (carsCanTurnRight)
					{
						extraWeight *= this._constants.CanTurnRightWeightModifier;
					}
					if (vehicle.vehicle.CurrentFrame.lane.connection.input.type == RoadType.Carpark || vehicle.vehicle.CurrentFrame.lane.connection.input.type == RoadType.ParkingSpace)
					{
						extraWeight *= this._constants.CarparkPriorityModifier;
					}
					extraWeight += Fix64.One;
					LaneModel blockingLane = vehicle.vehicle.CurrentFrame.blockingLane;
					if (((blockingLane != null) ? blockingLane.roadChunk : null) == this._owningChunk && this._currentPairIndex == index)
					{
						extraWeight *= this._constants.BlockedCarWeightModifier;
					}
					weight += extraWeight;
				}
			}
			return weight;
		}

		// Token: 0x0600222F RID: 8751 RVA: 0x0008A4B8 File Offset: 0x000886B8
		private int NumberOfCarsForPair(int index, bool ignoreBlockedVehicles = false, bool onlyNearbyCars = false)
		{
			if (index < 0 || index >= this.greenLightPairs.Count)
			{
				return 0;
			}
			int carCounter = 0;
			foreach (TileDirection direction in this.greenLightPairs[index])
			{
				carCounter += this._owningChunk.NumberOfCarsEnteringFromDirection(direction, ignoreBlockedVehicles, onlyNearbyCars ? this._constants.distanceToCountForNearbyCars : (-Fix64.One));
			}
			return carCounter;
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x0008A530 File Offset: 0x00088730
		public override void Reset()
		{
			base.Reset();
			this.greenLightPairs.Clear();
			this.isInOvertime = false;
			this.durationOnCurrentPair = Fix64.Zero;
			this._owningChunk = null;
			this.amberLightsOn = false;
			this.isInOvertime = false;
			this._currentPairIndex = -1;
			this.requiresPairCalculation = true;
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x0008A583 File Offset: 0x00088783
		public TrafficLightModel() : base(1)
		{
		}

		// Token: 0x04001BF9 RID: 7161
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001BFA RID: 7162
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001BFB RID: 7163
		public Fix64 durationOnCurrentPair = Fix64.Zero;

		// Token: 0x04001BFC RID: 7164
		private int _currentPairIndex = -1;

		// Token: 0x04001BFD RID: 7165
		public readonly List<TileDirectionBitfield> greenLightPairs = new List<TileDirectionBitfield>();

		// Token: 0x04001BFE RID: 7166
		private RoadChunkModel _owningChunk;

		// Token: 0x04001BFF RID: 7167
		public bool requiresPairCalculation = true;

		// Token: 0x04001C00 RID: 7168
		public bool isInOvertime;

		// Token: 0x04001C01 RID: 7169
		public bool amberLightsOn;

		// Token: 0x04001C02 RID: 7170
		private const int InvalidIndex = -1;

		// Token: 0x04001C03 RID: 7171
		public static readonly TileDirectionBitfield AllBlockedDirectionBitfield = new TileDirectionBitfield(-1);

		// Token: 0x02000505 RID: 1285
		public interface IObserver
		{
			// Token: 0x06002233 RID: 8755
			void OnTrafficLightGreen(TrafficLightModel model, TileDirectionBitfield rightOfWay);

			// Token: 0x06002234 RID: 8756
			void OnTrafficLightAmber(TrafficLightModel model);

			// Token: 0x06002235 RID: 8757
			void OnLanesChanged();
		}
	}
}
