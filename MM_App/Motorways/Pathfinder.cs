using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Pathfinding;

namespace Motorways
{
	// Token: 0x020003BF RID: 959
	public class Pathfinder : ICreatedInScopeHandler, IReleasedFromScopeHandler
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x060016D1 RID: 5841 RVA: 0x00052C60 File Offset: 0x00050E60
		// (set) Token: 0x060016D2 RID: 5842 RVA: 0x00052C68 File Offset: 0x00050E68
		public bool IsActive { get; private set; }

		// Token: 0x060016D3 RID: 5843 RVA: 0x00052C71 File Offset: 0x00050E71
		public void OnCreatedInScope(IScope scope)
		{
			this._externPathfinding = new ExternPathfinding();
			this.IsActive = true;
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x00052C85 File Offset: 0x00050E85
		public void OnReleasedFromScope(IScope scope)
		{
			this.Clear();
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x00052C8D File Offset: 0x00050E8D
		public void Clear()
		{
			if (this.IsActive)
			{
				this._externPathfinding.Clear();
			}
			this.IsActive = false;
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x00052CA9 File Offset: 0x00050EA9
		public void PauseUpdate()
		{
			this._externPathfinding.PauseUpdate();
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x00052CB6 File Offset: 0x00050EB6
		public void ResumeUpdate()
		{
			this._externPathfinding.ResumeUpdate();
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x00052CC4 File Offset: 0x00050EC4
		public List<LaneModel> CreatePath(LaneModel startLane, List<LaneModel> possibleEndLanes, bool allowMothballedLaneUse)
		{
			if (possibleEndLanes.Contains(startLane))
			{
				return new List<LaneModel>();
			}
			LaneModel nearestEndLane;
			if (this._externPathfinding.GetNearestEndpoint(startLane.PathfindingEndNodeId, possibleEndLanes, out nearestEndLane))
			{
				return this.CreatePath(startLane, nearestEndLane, allowMothballedLaneUse);
			}
			return null;
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00052D04 File Offset: 0x00050F04
		public List<LaneModel> CreatePath(LaneModel startLane, LaneModel endLane, bool allowMothballedLaneUse)
		{
			if (!Diagnostics.Verify(startLane != null, "Cannot find path from a null lane."))
			{
				return null;
			}
			if (endLane == startLane)
			{
				Pathfinder.Log.Info("Pathfinding from {0} to {1} which is the same lane!", new object[]
				{
					startLane,
					endLane
				});
				return new List<LaneModel>();
			}
			if (startLane.EndPosition == endLane.StartPosition)
			{
				Pathfinder.Log.Info("Pathfinding from {0} to {1} which are connected, so we just return the endLane as the entire path", new object[]
				{
					startLane,
					endLane
				});
				return new List<LaneModel>
				{
					endLane
				};
			}
			if (!Diagnostics.Verify(this._externPathfinding.IsNodeEndpoint(endLane.PathfindingStartNodeId), "Pathfinder can only find paths to endpoint nodes"))
			{
				return null;
			}
			LaneModel currentLane = startLane;
			if (!allowMothballedLaneUse)
			{
				int pathCost = this._externPathfinding.GetPathCost(currentLane.PathfindingEndNodeId, endLane.PathfindingStartNodeId);
				if (pathCost >= 100000)
				{
					Pathfinder.Log.Info("Returning null path from {0} to {1} as known path costs {2} so must contain mothballed road, which isn't allowed!", new object[]
					{
						currentLane,
						endLane,
						pathCost
					});
					return null;
				}
			}
			List<LaneModel> newPath = new List<LaneModel>();
			while (currentLane.PathfindingEndNodeId != endLane.PathfindingStartNodeId)
			{
				currentLane = this._externPathfinding.GetPathNextLane(currentLane.PathfindingEndNodeId, endLane.PathfindingStartNodeId);
				if (currentLane == null || !Diagnostics.Verify(!newPath.Contains(currentLane), "Path from {0} to {1} already contains lane {2} from {3} to {4}. There must be a loop!", currentLane.PathfindingEndNodeId, endLane.PathfindingStartNodeId, currentLane._id, currentLane.PathfindingStartNodeId, currentLane.PathfindingEndNodeId))
				{
					break;
				}
				newPath.Add(currentLane);
			}
			if (currentLane == null)
			{
				return null;
			}
			if (!Diagnostics.Verify(currentLane.PathfindingEndNodeId == endLane.PathfindingStartNodeId, "currentLane ({0}) was not null but it doesn't end at the endpoint lane. How is that possible?", currentLane))
			{
				return null;
			}
			newPath.Add(endLane);
			return newPath;
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x00052EA4 File Offset: 0x000510A4
		public int AddNode(bool isEndpoint)
		{
			return this._externPathfinding.AddNode(isEndpoint);
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x00052EB2 File Offset: 0x000510B2
		public void AddEdge(LaneModel laneModel, int fromNodeId, int toNodeId, int cost)
		{
			this._externPathfinding.AddEdge(laneModel, fromNodeId, toNodeId, cost);
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00052EC5 File Offset: 0x000510C5
		public void RemoveEdge(int fromNodeId, int toNodeId)
		{
			this._externPathfinding.RemoveEdge(fromNodeId, toNodeId);
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x00052ED5 File Offset: 0x000510D5
		public void ChangeEdgeCost(int fromNode, int toNode, int newCost)
		{
			this._externPathfinding.ChangeEdgeCost(fromNode, toNode, newCost);
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00052EE6 File Offset: 0x000510E6
		public bool MergeNodes(int intoNodeId, int fromNodeId)
		{
			return this._externPathfinding.MergeNodes(intoNodeId, fromNodeId);
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x00052EF5 File Offset: 0x000510F5
		public void MakeEndpoint(int nodeId)
		{
			this._externPathfinding.MakeNodeEndpoint(nodeId);
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x00052F03 File Offset: 0x00051103
		public LaneModel GetPathNextLane(int fromNodeId, int toNodeId)
		{
			return this._externPathfinding.GetPathNextLane(fromNodeId, toNodeId);
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00052F14 File Offset: 0x00051114
		public int GetMinPathCost(LaneModel fromLane, List<LaneModel> toLanes, bool allowMothballedLaneUse)
		{
			int validCostLimit = allowMothballedLaneUse ? int.MaxValue : 100000;
			int minPathCost = validCostLimit;
			foreach (LaneModel toLane in toLanes)
			{
				int cost = this._externPathfinding.GetPathCost(fromLane.PathfindingEndNodeId, toLane.PathfindingStartNodeId);
				if (cost >= 0 && cost < minPathCost)
				{
					minPathCost = cost;
				}
			}
			if (minPathCost < validCostLimit)
			{
				return minPathCost;
			}
			return -1;
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x00052F9C File Offset: 0x0005119C
		public bool AreLanesConnected(LaneModel fromLane, IEnumerable<LaneModel> possibleToLanes, bool allowMothballedLaneUsage)
		{
			foreach (LaneModel toLane in possibleToLanes)
			{
				if (this.AreLanesConnected(fromLane, toLane, allowMothballedLaneUsage))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x00052FF0 File Offset: 0x000511F0
		public bool AreLanesConnected(LaneModel fromLane, LaneModel toLane, bool allowMothballedLaneUsage)
		{
			if (fromLane.PathfindingEndNodeId == -1 || toLane.PathfindingStartNodeId == -1)
			{
				Pathfinder.Log.Warn("Returning false from AreLanesConnected({0}, {1}) as the {2} lane doesn't have an end node ID", new object[]
				{
					fromLane,
					toLane,
					(fromLane.PathfindingEndNodeId == -1) ? "from" : "to"
				});
				return false;
			}
			return this._externPathfinding.AreNodesConnected(fromLane.PathfindingEndNodeId, toLane.PathfindingStartNodeId, allowMothballedLaneUsage);
		}

		// Token: 0x04001371 RID: 4977
		public const int NormalLaneCostMultiplier = 10;

		// Token: 0x04001372 RID: 4978
		public const int MothballLaneCost = 100000;

		// Token: 0x04001373 RID: 4979
		public const int NoPathCost = -1;

		// Token: 0x04001374 RID: 4980
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Pathfinder");

		// Token: 0x04001375 RID: 4981
		public static readonly Fix64 CarparkPenalty = (Fix64)5L;

		// Token: 0x04001376 RID: 4982
		public static readonly Fix64 UTurnPenalty = (Fix64)500L;

		// Token: 0x04001377 RID: 4983
		public static readonly Fix64 MothballedDiscouragedLanePenalty = (Fix64)1.25;

		// Token: 0x04001378 RID: 4984
		public static readonly Fix64 MothballedLastResortLanePenalty = (Fix64)500L;

		// Token: 0x04001379 RID: 4985
		public static readonly Fix64 VehicleSpeedCostMultiplier = (Fix64)2L;

		// Token: 0x0400137B RID: 4987
		private ExternPathfinding _externPathfinding;

		// Token: 0x020003C0 RID: 960
		public enum MothballedLaneUse
		{
			// Token: 0x0400137D RID: 4989
			Discouraged,
			// Token: 0x0400137E RID: 4990
			LastResort,
			// Token: 0x0400137F RID: 4991
			Never
		}
	}
}
