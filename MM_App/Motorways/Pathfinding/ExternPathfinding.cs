using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Pathfinding
{
	// Token: 0x020004C4 RID: 1220
	public class ExternPathfinding
	{
		// Token: 0x06001FB2 RID: 8114 RVA: 0x0007D923 File Offset: 0x0007BB23
		public void Clear()
		{
			this._edgeToLaneMap.Clear();
			ExternPathfinding.NativeClear(this._pathfinderId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FB3 RID: 8115 RVA: 0x0007D941 File Offset: 0x0007BB41
		public void PauseUpdate()
		{
			ExternPathfinding.NativePauseUpdate(this._pathfinderId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FB4 RID: 8116 RVA: 0x0007D954 File Offset: 0x0007BB54
		public void ResumeUpdate()
		{
			ExternPathfinding.NativeResumeUpdate(this._pathfinderId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FB5 RID: 8117 RVA: 0x0007D967 File Offset: 0x0007BB67
		public int AddNode(bool isEndpoint = false)
		{
			return ExternPathfinding.NativeAddNode(this._pathfinderId, isEndpoint, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FB6 RID: 8118 RVA: 0x0007D97A File Offset: 0x0007BB7A
		public bool MergeNodes(int intoNodeId, int fromNodeId)
		{
			return ExternPathfinding.NativeMergeNodes(this._pathfinderId, intoNodeId, fromNodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x0007D98E File Offset: 0x0007BB8E
		[MonoPInvokeCallback(typeof(ExternPathfinding.NativeReportError))]
		private static void ReportError(string errorMessage)
		{
			Diagnostics.FailAssert(errorMessage, Array.Empty<object>());
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x0007D99B File Offset: 0x0007BB9B
		public int AddEdge(int nodeId1, int nodeId2, int cost)
		{
			return this.AddEdge(null, nodeId1, nodeId2, cost);
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x0007D9A8 File Offset: 0x0007BBA8
		public int AddEdge(LaneModel laneModel, int nodeId1, int nodeId2, int cost)
		{
			int edgeId = ExternPathfinding.NativeAddEdge(this._pathfinderId, nodeId1, nodeId2, cost, ExternPathfinding.ReportErrorMarshalled);
			if (Diagnostics.Verify(edgeId >= 0))
			{
				this._edgeToLaneMap[edgeId] = laneModel;
			}
			return edgeId;
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x0007D9E6 File Offset: 0x0007BBE6
		public bool AddEdgesBetweenNodes(int nodeA, int nodeB, int cost)
		{
			return this.AddEdge(null, nodeA, nodeB, cost) != -1 && this.AddEdge(null, nodeB, nodeA, cost) != -1;
		}

		// Token: 0x06001FBB RID: 8123 RVA: 0x0007DA07 File Offset: 0x0007BC07
		public bool RemoveEdge(int nodeId1, int nodeId2)
		{
			return ExternPathfinding.NativeRemoveEdge(this._pathfinderId, nodeId1, nodeId2, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x0007DA1B File Offset: 0x0007BC1B
		public bool ChangeEdgeCost(int node1Id, int node2Id, int newCost)
		{
			return ExternPathfinding.NativeChangeEdgeCost(this._pathfinderId, node1Id, node2Id, newCost, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FBD RID: 8125 RVA: 0x0007DA30 File Offset: 0x0007BC30
		public int GetPathCost(int fromNodeId, int endpointNodeId)
		{
			int pathCost = ExternPathfinding.NativeGetPathCost(this._pathfinderId, fromNodeId, endpointNodeId, ExternPathfinding.ReportErrorMarshalled);
			if (pathCost < ExternPathfinding.NativeGetNoPathCostConstant())
			{
				return pathCost;
			}
			return -1;
		}

		// Token: 0x06001FBE RID: 8126 RVA: 0x0007DA5B File Offset: 0x0007BC5B
		public int GetPathNextEdge(int fromNodeId, int endpointNodeId)
		{
			return ExternPathfinding.NativeGetPathNextEdge(this._pathfinderId, fromNodeId, endpointNodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x0007DA6F File Offset: 0x0007BC6F
		public int GetPathNextNode(int fromNodeId, int endpointNodeId)
		{
			return ExternPathfinding.NativeGetPathNextNode(this._pathfinderId, fromNodeId, endpointNodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x0007DA84 File Offset: 0x0007BC84
		public LaneModel GetPathNextLane(int fromNodeId, int endpointNodeId)
		{
			int nextEdgeId = ExternPathfinding.NativeGetPathNextEdge(this._pathfinderId, fromNodeId, endpointNodeId, ExternPathfinding.ReportErrorMarshalled);
			LaneModel nextLaneModel;
			if (this._edgeToLaneMap.TryGetValue(nextEdgeId, out nextLaneModel))
			{
				return nextLaneModel;
			}
			return null;
		}

		// Token: 0x06001FC1 RID: 8129 RVA: 0x0007DAB8 File Offset: 0x0007BCB8
		public bool GetNearestEndpoint(int fromNodeId, List<LaneModel> endpointLaneModels, out LaneModel nearestEndpoint)
		{
			int lowestCost = ExternPathfinding.NativeGetNoPathCostConstant();
			nearestEndpoint = null;
			foreach (LaneModel endpointLaneModel in endpointLaneModels)
			{
				int destCost = ExternPathfinding.NativeGetPathCost(this._pathfinderId, fromNodeId, endpointLaneModel.PathfindingStartNodeId, ExternPathfinding.ReportErrorMarshalled);
				if (destCost < lowestCost)
				{
					lowestCost = destCost;
					nearestEndpoint = endpointLaneModel;
				}
			}
			return nearestEndpoint != null;
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x0007DB30 File Offset: 0x0007BD30
		public int GetEdgeCost(int fromNodeId, int toNodeId)
		{
			return ExternPathfinding.NativeGetEdgeCost(this._pathfinderId, fromNodeId, toNodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC3 RID: 8131 RVA: 0x0007DB44 File Offset: 0x0007BD44
		public int GetNeighbourCount(int fromNodeId)
		{
			return ExternPathfinding.NativeGetNeighbourCount(this._pathfinderId, fromNodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC4 RID: 8132 RVA: 0x0007DB57 File Offset: 0x0007BD57
		public int GetNeighbour(int fromNodeId, int neighbourIndex)
		{
			return ExternPathfinding.NativeGetNeighbour(this._pathfinderId, fromNodeId, neighbourIndex, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC5 RID: 8133 RVA: 0x0007DB6B File Offset: 0x0007BD6B
		public bool IsNodeEndpoint(int nodeId)
		{
			return ExternPathfinding.NativeIsNodeEndpoint(this._pathfinderId, nodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC6 RID: 8134 RVA: 0x0007DB7E File Offset: 0x0007BD7E
		public void MakeNodeEndpoint(int nodeId)
		{
			ExternPathfinding.NativeMakeNodeEndpoint(this._pathfinderId, nodeId, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC7 RID: 8135 RVA: 0x0007DB91 File Offset: 0x0007BD91
		public bool AreNodesConnected(int nodeId1, int nodeId2, bool allowMothballedLaneUsage)
		{
			return ExternPathfinding.NativeAreNodesConnected(this._pathfinderId, nodeId1, nodeId2, allowMothballedLaneUsage ? ExternPathfinding.NativeGetNoPathCostConstant() : 100000, ExternPathfinding.ReportErrorMarshalled);
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x0007DBB4 File Offset: 0x0007BDB4
		public static void DebugPrintPathfindingGraphs()
		{
			Debug.LogWarning(ExternPathfinding.NativeDebugPrintPathfindingGraphs(ExternPathfinding.ReportErrorMarshalled));
		}

		// Token: 0x06001FC9 RID: 8137
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddNode")]
		private static extern int NativeAddNode(int pathfinderId, bool isEndpoint, IntPtr reportError);

		// Token: 0x06001FCA RID: 8138
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MergeNodes")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool NativeMergeNodes(int pathfinderId, int intoNodeId, int fromNodeId, IntPtr reportError);

		// Token: 0x06001FCB RID: 8139
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddEdge")]
		private static extern int NativeAddEdge(int pathfinderId, int nodeId1, int nodeId2, int cost, IntPtr reportError);

		// Token: 0x06001FCC RID: 8140
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "RemoveEdge")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool NativeRemoveEdge(int pathfinderId, int nodeId1, int nodeId2, IntPtr reportError);

		// Token: 0x06001FCD RID: 8141
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ChangeEdgeCost")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool NativeChangeEdgeCost(int pathfinderId, int node1Id, int node2Id, int newCost, IntPtr reportError);

		// Token: 0x06001FCE RID: 8142
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPathCost")]
		private static extern int NativeGetPathCost(int pathfinderId, int fromNodeId, int endpointNodeId, IntPtr reportError);

		// Token: 0x06001FCF RID: 8143
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPathNextEdge")]
		private static extern int NativeGetPathNextEdge(int pathfinderId, int fromNodeId, int endpointNodeId, IntPtr reportError);

		// Token: 0x06001FD0 RID: 8144
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPathNextNode")]
		private static extern int NativeGetPathNextNode(int pathfinderId, int fromNodeId, int endpointNodeId, IntPtr reportError);

		// Token: 0x06001FD1 RID: 8145
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEdgeCost")]
		private static extern int NativeGetEdgeCost(int pathfinderId, int fromNodeId, int toNodeId, IntPtr reportError);

		// Token: 0x06001FD2 RID: 8146
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetNeighbourCount")]
		private static extern int NativeGetNeighbourCount(int pathfinderId, int fromNodeId, IntPtr reportError);

		// Token: 0x06001FD3 RID: 8147
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetNeighbour")]
		private static extern int NativeGetNeighbour(int pathfinderId, int fromNodeId, int neighbourIndex, IntPtr reportError);

		// Token: 0x06001FD4 RID: 8148
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsNodeEndpoint")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool NativeIsNodeEndpoint(int pathfinderId, int nodeId, IntPtr reportError);

		// Token: 0x06001FD5 RID: 8149
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MakeNodeEndpoint")]
		private static extern void NativeMakeNodeEndpoint(int pathfinderId, int nodeId, IntPtr reportError);

		// Token: 0x06001FD6 RID: 8150
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AreNodesConnected")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool NativeAreNodesConnected(int pathfinderId, int nodeId1, int nodeId2, int costUpperLimit, IntPtr reportError);

		// Token: 0x06001FD7 RID: 8151
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetNoPathCostConstant")]
		private static extern int NativeGetNoPathCostConstant();

		// Token: 0x06001FD8 RID: 8152
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Clear")]
		private static extern int NativeClear(int pathfinderId, IntPtr reportError);

		// Token: 0x06001FD9 RID: 8153
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PauseUpdate")]
		private static extern int NativePauseUpdate(int pathfinderId, IntPtr reportError);

		// Token: 0x06001FDA RID: 8154
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ResumeUpdate")]
		private static extern int NativeResumeUpdate(int pathfinderId, IntPtr reportError);

		// Token: 0x06001FDB RID: 8155
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Setup")]
		private static extern int NativeSetup();

		// Token: 0x06001FDC RID: 8156
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DebugPrintPathfindingGraphs")]
		private static extern string NativeDebugPrintPathfindingGraphs(IntPtr reportError);

		// Token: 0x04001A73 RID: 6771
		private readonly int _pathfinderId = ExternPathfinding.NativeSetup();

		// Token: 0x04001A74 RID: 6772
		private readonly Dictionary<int, LaneModel> _edgeToLaneMap = new Dictionary<int, LaneModel>();

		// Token: 0x04001A75 RID: 6773
		private static readonly IntPtr ReportErrorMarshalled = Marshal.GetFunctionPointerForDelegate<ExternPathfinding.NativeReportError>(new ExternPathfinding.NativeReportError(ExternPathfinding.ReportError));

		// Token: 0x020004C5 RID: 1221
		// (Invoke) Token: 0x06001FE0 RID: 8160
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NativeReportError(string errorMessage);
	}
}
