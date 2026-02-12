using System;
using FixMath;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003D8 RID: 984
	[CreateAssetMenu(menuName = "Motorways/SimulationConstants")]
	public class SimulationConstantsData : ScriptableObject
	{
		// Token: 0x06001795 RID: 6037 RVA: 0x00054137 File Offset: 0x00052337
		public Fix64 EvaluateHouseContributionFromDistance(Fix64 distance)
		{
			return (Fix64)this._houseDistanceToContributionCurve.Evaluate((float)distance) * this._houseContributionMultiplier;
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0005415B File Offset: 0x0005235B
		public Fix64 EvaluateDestinationCountHouseValueMultiplier(int numberOfDestinationsInGroupIndex)
		{
			return (Fix64)this.GroupDestinationCountToHouseValueMultiplier.Evaluate((float)numberOfDestinationsInGroupIndex);
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x0005416F File Offset: 0x0005236F
		public Fix64 GetOvercrowdTimerSpeedMultiplierForExtraDemand(Fix64 currentDemandMultiplier)
		{
			return (Fix64)this.OvercrowdTimerSpeedMultiplierByDemand.Evaluate((float)currentDemandMultiplier);
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x00054188 File Offset: 0x00052388
		public Fix64 GetCarArrivalPinReductionMultiplierOverTime(Fix64 gameTime)
		{
			return (Fix64)this.CarArrivalPinReductionMultiplierOverTime.Evaluate((float)gameTime);
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x000541A1 File Offset: 0x000523A1
		public Fix64 GetEfficiencyScoreForVehiclePathLength(Fix64 length)
		{
			return (Fix64)this.PathDistanceToEfficiencyScore.Evaluate((float)length);
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x000541BA File Offset: 0x000523BA
		public Fix64 GetPercentageOfMilestoneToLoseFromProgress(Fix64 milestoneProgress)
		{
			return (Fix64)this.PercentageOfMilestoneToLoseMultiplier.Evaluate((float)milestoneProgress) * this.PercentageOfMilestoneToLose;
		}

		// Token: 0x040013E1 RID: 5089
		private const string TrainConstants = "Train Constants";

		// Token: 0x040013E2 RID: 5090
		[FoldoutGroup("Train Constants")]
		public Fix64 trainSpeed = (Fix64)2.6f;

		// Token: 0x040013E3 RID: 5091
		[FoldoutGroup("Train Constants")]
		public Fix64 trainAcceleration = (Fix64)1.1f;

		// Token: 0x040013E4 RID: 5092
		[FoldoutGroup("Train Constants")]
		public Fix64 trainDeceleration = (Fix64)2.2f;

		// Token: 0x040013E5 RID: 5093
		[FoldoutGroup("Train Constants")]
		public Fix64 trainMinimumSpeedDuringDeceleration = (Fix64)0.1f;

		// Token: 0x040013E6 RID: 5094
		[FoldoutGroup("Train Constants")]
		public Fix64 trainStoppingDistanceFromBuffer = (Fix64)0.5f;

		// Token: 0x040013E7 RID: 5095
		[FoldoutGroup("Train Constants")]
		[Tooltip("The distance from a crossing a train has to be before the lights go/cars will stop")]
		public Fix64 trainCrossingSignalDistance = (Fix64)5f;

		// Token: 0x040013E8 RID: 5096
		[FoldoutGroup("Train Constants")]
		public int maxDemandFromTrain = 6;

		// Token: 0x040013E9 RID: 5097
		[FoldoutGroup("Train Constants")]
		public int minDemandFromTrain = 1;

		// Token: 0x040013EA RID: 5098
		[FoldoutGroup("Train Constants")]
		public Fix64 demandPerHouse = (Fix64)0.5f;

		// Token: 0x040013EB RID: 5099
		[FoldoutGroup("Train Constants")]
		[Tooltip("The max rate of acceleration for vehicles on a crossing")]
		public Fix64 maxAccelerationOnCrossings = (Fix64)0.3f;

		// Token: 0x040013EC RID: 5100
		[FoldoutGroup("Train Constants")]
		[Tooltip("The relative speed vehicles want to be at when arriving at a crossing")]
		public Fix64 targetSpeedTowardsCrossings = (Fix64)0.5f;

		// Token: 0x040013ED RID: 5101
		[Tooltip("How smoothly it decelerates towards targetSpeedTowardsCrossings")]
		[FoldoutGroup("Train Constants")]
		public Fix64 decelerationExponentTowardsCrossings = Fix64.One;

		// Token: 0x040013EE RID: 5102
		[Tooltip("How far away from the crossing the traffic slows down, measured in vehicle lengths")]
		[FoldoutGroup("Train Constants")]
		public Fix64 crossingSlowDistance = (Fix64)3f;

		// Token: 0x040013EF RID: 5103
		[FoldoutGroup("Train Constants")]
		[Tooltip("How far away from the edge of a crossing tile the traffic stops, measured in vehicle lengths")]
		public Fix64 crossingStopDistance = (Fix64)2f;

		// Token: 0x040013F0 RID: 5104
		[FoldoutGroup("Train Constants")]
		[Tooltip("How long cars will wait at a crossing after a train has passed")]
		public Fix64 crossingWaitTime = (Fix64)0.5f;

		// Token: 0x040013F1 RID: 5105
		[FoldoutGroup("Train Constants")]
		public Fix64 trainStationWaitTime = (Fix64)1.1f;

		// Token: 0x040013F2 RID: 5106
		[FoldoutGroup("Train Constants")]
		public Fix64 trainCenterToWheelDistance = (Fix64)0.95f;

		// Token: 0x040013F3 RID: 5107
		[FoldoutGroup("Train Constants")]
		public Fix64 trainCarriageSeparationDistance = (Fix64)1.1f;

		// Token: 0x040013F4 RID: 5108
		private const string BoatConstants = "Boat Constants";

		// Token: 0x040013F5 RID: 5109
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatSpeed = (Fix64)2.6f;

		// Token: 0x040013F6 RID: 5110
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatAcceleration = (Fix64)1.1f;

		// Token: 0x040013F7 RID: 5111
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatUndockingAcceleration = (Fix64)0.5f;

		// Token: 0x040013F8 RID: 5112
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatDeceleration = (Fix64)2.2f;

		// Token: 0x040013F9 RID: 5113
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatMinimumSpeedDuringDeceleration = (Fix64)0.1f;

		// Token: 0x040013FA RID: 5114
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatStoppingDistanceFromBuffer = (Fix64)0.5f;

		// Token: 0x040013FB RID: 5115
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatCenterToPivotDistance = (Fix64)1f;

		// Token: 0x040013FC RID: 5116
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatCenterToBowDistance = (Fix64)0.95f;

		// Token: 0x040013FD RID: 5117
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatTerminalWaitTime = (Fix64)1.1f;

		// Token: 0x040013FE RID: 5118
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatDockingSpeedThreshold = Fix64.One;

		// Token: 0x040013FF RID: 5119
		[FoldoutGroup("Boat Constants")]
		public Fix64 boatUndockingSpeedThreshold = (Fix64)2f;

		// Token: 0x04001400 RID: 5120
		[FoldoutGroup("Boat Constants")]
		public Vector2 boatUndockingMidpointOffset = new Vector2(0.5f, 0f);

		// Token: 0x04001401 RID: 5121
		private const string VehicleConstants = "Vehicles";

		// Token: 0x04001402 RID: 5122
		[FoldoutGroup("Vehicles")]
		public Fix64 maxAcceleration = (Fix64)0.6f;

		// Token: 0x04001403 RID: 5123
		[FoldoutGroup("Vehicles")]
		public Fix64 controlledIntersectionAcceleration = (Fix64)0.6f;

		// Token: 0x04001404 RID: 5124
		[FoldoutGroup("Vehicles")]
		public Fix64 roundaboutAcceleration = (Fix64)1f;

		// Token: 0x04001405 RID: 5125
		[FoldoutGroup("Vehicles")]
		[Tooltip("How smooth acceleration is.")]
		public Fix64 accelerationExponent = (Fix64)4L;

		// Token: 0x04001406 RID: 5126
		[FoldoutGroup("Vehicles")]
		public Fix64 maxDeceleration = (Fix64)1.5f;

		// Token: 0x04001407 RID: 5127
		[FoldoutGroup("Vehicles")]
		[Tooltip("How smooth deceleration is.")]
		public Fix64 decelerationExponent = (Fix64)4L;

		// Token: 0x04001408 RID: 5128
		[Tooltip("Speed cars will try hit when driving towards intersections.")]
		[FoldoutGroup("Vehicles")]
		public Fix64 targetSpeedTowardsIntersections = Fix64.One;

		// Token: 0x04001409 RID: 5129
		[Tooltip("How smoothly it decelerates towards targetSpeedTowardsIntersections")]
		[FoldoutGroup("Vehicles")]
		public Fix64 decelerationExponentTowardsIntersections = Fix64.One;

		// Token: 0x0400140A RID: 5130
		[FoldoutGroup("Vehicles")]
		public Fix64 speedMultiplier = Fix64.One;

		// Token: 0x0400140B RID: 5131
		[Tooltip("How long to wait before just going")]
		[FoldoutGroup("Vehicles")]
		public Fix64 MaximumTimeToWaitAtIntersection = (Fix64)45L;

		// Token: 0x0400140C RID: 5132
		[FoldoutGroup("Vehicles")]
		[Tooltip("How many roads do we count as an intersection to slow down for?")]
		public int NumberOfRoadsAtIntersectionToSlowDownFor = 3;

		// Token: 0x0400140D RID: 5133
		[FoldoutGroup("Vehicles")]
		[Tooltip("Do we include houses connected to roads as intersections?")]
		public bool IgnoreHousesForIntersectionSlowDown;

		// Token: 0x0400140E RID: 5134
		[FoldoutGroup("Vehicles")]
		[Tooltip("Do we include destinations connected to roads as intersections?")]
		public bool IgnoreDestinationsForIntersectionSlowDown;

		// Token: 0x0400140F RID: 5135
		[FoldoutGroup("Vehicles")]
		[Tooltip("How much of a straight lane into a roundabout do we count as not part of the roundabout?")]
		public Fix64 PercentageOfStraightLanesIntoRoundaboutsToCountOutside = Fix64Consts.OneHalf;

		// Token: 0x04001410 RID: 5136
		[FoldoutGroup("Vehicles")]
		[Tooltip("Do we want to enable special behaviour for straight lines?")]
		public bool TreatStraightRoundaboutEntrancesAsNotRoundabouts;

		// Token: 0x04001411 RID: 5137
		[Tooltip("How far to look ahead for average speed and intersections to slow at.")]
		[FoldoutGroup("Vehicles")]
		public Fix64 LookaheadDistance = (Fix64)5L;

		// Token: 0x04001412 RID: 5138
		[FoldoutGroup("Vehicles")]
		public bool useAverageLaneSpeedRatherThanMin = true;

		// Token: 0x04001413 RID: 5139
		[FoldoutGroup("Vehicles")]
		public bool useAverageLaneSpeedRatherThanMinOnMotorways;

		// Token: 0x04001414 RID: 5140
		[Tooltip("How slow all vehicles in a cycle must be travelling before they're pushed forward to break the cycle.")]
		[FoldoutGroup("Vehicles")]
		public Fix64 minSpeedBeforePushingCycle = Fix64.One / (Fix64)10L;

		// Token: 0x04001415 RID: 5141
		private const string LaneConstants = "Lanes";

		// Token: 0x04001416 RID: 5142
		[FoldoutGroup("Lanes")]
		[Tooltip("Base lane speed.")]
		public Fix64 defaultLaneSpeed = Fix64.One;

		// Token: 0x04001417 RID: 5143
		[Tooltip("How slow do cars go on a hairpin turn?")]
		[FoldoutGroup("Lanes")]
		public Fix64 sharpTurnSpeedMultiplier = (Fix64)0.3333333333333333;

		// Token: 0x04001418 RID: 5144
		[FoldoutGroup("Lanes")]
		[Tooltip("How slow do cars go on a right hand turn?")]
		public Fix64 rightAngleTurnSpeedMultiplier = (Fix64)0.6666666666666666;

		// Token: 0x04001419 RID: 5145
		[FoldoutGroup("Lanes")]
		[Tooltip("How slow do cars go when heading towards an intersection?")]
		public Fix64 intersectionSpeedMultiplier = Fix64Consts.OneHalf;

		// Token: 0x0400141A RID: 5146
		[FoldoutGroup("Lanes")]
		[Tooltip("How fast do cars go when heading towards a roundabout?")]
		public Fix64 roundaboutSpeedMultiplier = Fix64Consts.Two;

		// Token: 0x0400141B RID: 5147
		[FoldoutGroup("Lanes")]
		public Fix64 maxSpeedOnMotorways = (Fix64)3L;

		// Token: 0x0400141C RID: 5148
		private const string TrafficLightConstants = "Traffic Lights";

		// Token: 0x0400141D RID: 5149
		[Tooltip("The minimum delay before traffic lights changing (seconds).")]
		[FoldoutGroup("Traffic Lights")]
		public Fix64 changeDelay = (Fix64)10L;

		// Token: 0x0400141E RID: 5150
		[FoldoutGroup("Traffic Lights")]
		[Tooltip("The delay before changing traffic lights if in overtime (seconds).")]
		public Fix64 overtimeChangeDelay = (Fix64)5L;

		// Token: 0x0400141F RID: 5151
		[FoldoutGroup("Traffic Lights")]
		[Tooltip("The duration of amber lights (seconds).")]
		public Fix64 amberDelay = (Fix64)2L;

		// Token: 0x04001420 RID: 5152
		[FoldoutGroup("Traffic Lights")]
		[Tooltip("If there are fewer than this many cars nearby, don't swap just yet.")]
		public int minimumNearbyCarsBeforeSwapping = 2;

		// Token: 0x04001421 RID: 5153
		[FoldoutGroup("Traffic Lights")]
		public Fix64 distanceToCountForNearbyCars = (Fix64)2L;

		// Token: 0x04001422 RID: 5154
		[FoldoutGroup("Traffic Lights")]
		public Fix64 MaximumIdleTimeAtTrafficLightBeforeMaxWeight = (Fix64)30L;

		// Token: 0x04001423 RID: 5155
		[FoldoutGroup("Traffic Lights")]
		public Fix64 IdleTimeAtTrafficLightWeightMultiplier = Fix64.One;

		// Token: 0x04001424 RID: 5156
		[FoldoutGroup("Traffic Lights")]
		public Fix64 IdleTimeAtTrafficLightWeightMultiplierOnMothballedLane = Fix64Consts.Two;

		// Token: 0x04001425 RID: 5157
		[Tooltip("If cars can turn right on red, we weigh them less when deciding what green lanes to swap to.This ensures we don't uncessarily swap")]
		[FoldoutGroup("Traffic Lights")]
		public Fix64 CanTurnRightWeightModifier = Fix64.One;

		// Token: 0x04001426 RID: 5158
		[Tooltip("If a car is in a carpark, how much extra priority do we give it?")]
		[FoldoutGroup("Traffic Lights")]
		public Fix64 CarparkPriorityModifier = Fix64Consts.Two;

		// Token: 0x04001427 RID: 5159
		[FoldoutGroup("Traffic Lights")]
		[Tooltip("If this car is blocked by the same intersection that has the traffic light, how much do we drop the resulting weight by?")]
		public Fix64 BlockedCarWeightModifier = (Fix64)0.1f;

		// Token: 0x04001428 RID: 5160
		[FoldoutGroup("Traffic Lights")]
		public bool americanRedLightRules;

		// Token: 0x04001429 RID: 5161
		[FoldoutGroup("Traffic Lights")]
		public bool greenLightsIgnoreCollisions;

		// Token: 0x0400142A RID: 5162
		private const string BuildingSpawningConstants = "Building Spawning";

		// Token: 0x0400142B RID: 5163
		[FoldoutGroup("Building Spawning")]
		public Fix64 FailedHouseSpawnCooldown = (Fix64)2L;

		// Token: 0x0400142C RID: 5164
		[FoldoutGroup("Building Spawning")]
		public Fix64 FailedDestinationRetryDelay = (Fix64)20L;

		// Token: 0x0400142D RID: 5165
		[FoldoutGroup("Building Spawning")]
		[MinValue(0)]
		[Tooltip("Max Failed Building Spawns Before Ignoring Tile Weights")]
		public int MaxFailedBuildingSpawnsBeforeIgnoringWeights = 5;

		// Token: 0x0400142E RID: 5166
		[Tooltip("Max Failed Double Carpark Spawns Before Converting To a Single Carpark")]
		[MinValue(0)]
		[FoldoutGroup("Building Spawning")]
		public int MaxFailedDoubleCarparkSpawnsBeforeConvertingToSingle = 10;

		// Token: 0x0400142F RID: 5167
		[FoldoutGroup("Building Spawning")]
		[Tooltip("How many times a destination has to fail before converting to a destination upgrade instead.")]
		[MinValue(0)]
		public int MaxFailedDestinationSpawnsBeforeConvertingToUpgrade = 15;

		// Token: 0x04001430 RID: 5168
		[FoldoutGroup("Building Spawning")]
		public Fix64 MinimumTimeBetweenDestinationSpawns = (Fix64)10L;

		// Token: 0x04001431 RID: 5169
		[FoldoutGroup("Building Spawning")]
		public Fix64 DelayBetweenSameGroupHouseSpawns = (Fix64)10L;

		// Token: 0x04001432 RID: 5170
		[FoldoutGroup("Building Spawning")]
		[SerializeField]
		private AnimationCurve _houseDistanceToContributionCurve = new AnimationCurve();

		// Token: 0x04001433 RID: 5171
		[FoldoutGroup("Building Spawning")]
		[SerializeField]
		private Fix64 _houseContributionMultiplier = Fix64.One;

		// Token: 0x04001434 RID: 5172
		[FoldoutGroup("Building Spawning")]
		[SerializeField]
		private AnimationCurve GroupDestinationCountToHouseValueMultiplier = new AnimationCurve();

		// Token: 0x04001435 RID: 5173
		private const string SuburbConstants = "Suburb Spawning";

		// Token: 0x04001436 RID: 5174
		[FoldoutGroup("Suburb Spawning")]
		public Fix64 MinimumSuburbCountScale = (Fix64)0.7f;

		// Token: 0x04001437 RID: 5175
		[FoldoutGroup("Suburb Spawning")]
		public Fix64 MinimumSuburbCountExponent = (Fix64)1.2f;

		// Token: 0x04001438 RID: 5176
		[FoldoutGroup("Suburb Spawning")]
		public Fix64 MaximumSuburbCountScale = (Fix64)0.4f;

		// Token: 0x04001439 RID: 5177
		[FoldoutGroup("Suburb Spawning")]
		public Fix64 MaximumSuburbCountExponent = (Fix64)1.4f;

		// Token: 0x0400143A RID: 5178
		[FoldoutGroup("Suburb Spawning")]
		[MinValue(0)]
		public int MinimumSpawnAttemptsForSuburbMultiplier = 5;

		// Token: 0x0400143B RID: 5179
		[MinValue(0)]
		[FoldoutGroup("Suburb Spawning")]
		public int MaximumSpawnAttemptsForSuburbMultiplier = 10;

		// Token: 0x0400143C RID: 5180
		[FoldoutGroup("Suburb Spawning")]
		public Fix64 MaximumDelayedBuildingSuburbCountMultiplier = (Fix64)4L;

		// Token: 0x0400143D RID: 5181
		private const string BigPinConstants = "Big Pins";

		// Token: 0x0400143E RID: 5182
		[FoldoutGroup("Big Pins")]
		public Fix64 MaxOvercrowdTime = (Fix64)90L;

		// Token: 0x0400143F RID: 5183
		[Tooltip("The chunk of time at the end of the overcrowd timer that is not displayed.")]
		[FoldoutGroup("Big Pins")]
		public Fix64 GracePeriodTime = (Fix64)2L;

		// Token: 0x04001440 RID: 5184
		[FoldoutGroup("Big Pins")]
		public Fix64 OvercrowdTimerAcceleration = (Fix64)0.02;

		// Token: 0x04001441 RID: 5185
		[FoldoutGroup("Big Pins")]
		public Fix64 OvercrowdTimerCarArrivalDeceleration = (Fix64)0.5;

		// Token: 0x04001442 RID: 5186
		[FoldoutGroup("Big Pins")]
		public Fix64 OvercrowdTimerReturnSpeed = Fix64Consts.Two;

		// Token: 0x04001443 RID: 5187
		[FoldoutGroup("Big Pins")]
		[SerializeField]
		private AnimationCurve OvercrowdTimerSpeedMultiplierByDemand = AnimationCurve.Linear(0f, 1f, 0f, 1f);

		// Token: 0x04001444 RID: 5188
		[FoldoutGroup("Big Pins")]
		[SerializeField]
		private AnimationCurve CarArrivalPinReductionMultiplierOverTime = AnimationCurve.Linear(0f, 1f, 0f, 1f);

		// Token: 0x04001445 RID: 5189
		[Tooltip("The percentage to reduce the overcrowding timer at the instant a vehicle picks up a pin. Note this is actually a percentage and goes from 0 to 100.")]
		[FoldoutGroup("Big Pins")]
		public Fix64 PercentageToReduceTimerOnCarArrival = (Fix64)10L;

		// Token: 0x04001446 RID: 5190
		[Tooltip("The minimum amount to reduce the timer, in virtualized seconds, when a vehicle picks up a pin.")]
		[FoldoutGroup("Big Pins")]
		public Fix64 MinimumAmountToReduceTimerOnCarArrival = (Fix64)0L;

		// Token: 0x04001447 RID: 5191
		[FoldoutGroup("Big Pins")]
		[Tooltip("The maximum amount to reduce the timer, in virtualized seconds, when a vehicle picks up a pin.")]
		public Fix64 MaximumAmountToReduceTimerOnCarArrival = (Fix64)3L;

		// Token: 0x04001448 RID: 5192
		[FoldoutGroup("Big Pins")]
		public Fix64 MinimumOvercrowdTimerSpeed = (Fix64)0L;

		// Token: 0x04001449 RID: 5193
		[FoldoutGroup("Big Pins")]
		public Fix64 MaximumOvercrowdTimerSpeed = (Fix64)1L;

		// Token: 0x0400144A RID: 5194
		private const string DemandConstants = "Demand";

		// Token: 0x0400144B RID: 5195
		[FoldoutGroup("Demand")]
		public Fix64 DemandMultiplierForBuildings = (Fix64)0.8;

		// Token: 0x0400144C RID: 5196
		[FoldoutGroup("Demand")]
		public Fix64 DemandMultiplierForUpgradedBuildings = (Fix64)1.6;

		// Token: 0x0400144D RID: 5197
		[FoldoutGroup("Demand")]
		public Fix64 AverageCarsPerDay = (Fix64)1.55;

		// Token: 0x0400144E RID: 5198
		[FoldoutGroup("Demand")]
		public Fix64 DelayBeforeFirstPinOfDestination = (Fix64)4L;

		// Token: 0x0400144F RID: 5199
		[FoldoutGroup("Demand")]
		public Fix64 DemandMultiplierForBoatTerminals = Fix64.One;

		// Token: 0x04001450 RID: 5200
		[FoldoutGroup("Demand")]
		public Fix64 DemandMultiplierForUpgradedBoatTerminals = (Fix64)2L;

		// Token: 0x04001451 RID: 5201
		private const string EndlessConstants = "Endless";

		// Token: 0x04001452 RID: 5202
		[SerializeField]
		[FoldoutGroup("Endless")]
		private AnimationCurve PathDistanceToEfficiencyScore = AnimationCurve.Linear(0f, 0f, 100f, 100f);

		// Token: 0x04001453 RID: 5203
		[FoldoutGroup("Endless")]
		public Fix64 EndlessDemandMultiplier = (Fix64)0.5;

		// Token: 0x04001454 RID: 5204
		[FoldoutGroup("Endless")]
		public int MilestoneIncreaseAfterPrecalculatedIntervals = 50;

		// Token: 0x04001455 RID: 5205
		[FoldoutGroup("Endless")]
		public Fix64 PercentageOfMilestoneToLose = (Fix64)0.1f;

		// Token: 0x04001456 RID: 5206
		[FoldoutGroup("Endless")]
		[SerializeField]
		private AnimationCurve PercentageOfMilestoneToLoseMultiplier = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);

		// Token: 0x04001457 RID: 5207
		[FoldoutGroup("Endless")]
		public Fix64 EndlessSpawnRampMultiplier = Fix64.One;

		// Token: 0x04001458 RID: 5208
		[FoldoutGroup("Endless")]
		public Fix64 ExpansionTimePerMilestone = (Fix64)140L;

		// Token: 0x04001459 RID: 5209
		[FoldoutGroup("Endless")]
		[Tooltip("How much extra expansion time to we grant in Endless mode?")]
		public Fix64 BonusExpansionTime = (Fix64)140L;

		// Token: 0x0400145A RID: 5210
		[FoldoutGroup("Endless")]
		public int AdditionalHousesPerGroup = 1;

		// Token: 0x0400145B RID: 5211
		[FoldoutGroup("Endless")]
		public Fix64 TimeAtHouseBeforeCarIsAvailable = (Fix64)5L;

		// Token: 0x0400145C RID: 5212
		private const string ExpertConstants = "Expert";

		// Token: 0x0400145D RID: 5213
		[FoldoutGroup("Expert")]
		public Fix64 DurationTillRoadPermanence = (Fix64)40L;

		// Token: 0x0400145E RID: 5214
		[FoldoutGroup("Expert")]
		public Fix64 PercentageOfPermanenceTimerWhereRoadsCannotBeDemolished = (Fix64)0.25;

		// Token: 0x0400145F RID: 5215
		[FoldoutGroup("Expert")]
		public int MaxUpgradeChoicesAwardedInExpertMode = 8;

		// Token: 0x04001460 RID: 5216
		[FoldoutGroup("Expert")]
		public Fix64 DryingRoadSpeedMultiplier = (Fix64)0.8f;

		// Token: 0x04001461 RID: 5217
		private const string CreativeConstants = "Creative";

		// Token: 0x04001462 RID: 5218
		[FoldoutGroup("Creative")]
		public Fix64 CreativeDemandMultiplier = (Fix64)1.5;
	}
}
