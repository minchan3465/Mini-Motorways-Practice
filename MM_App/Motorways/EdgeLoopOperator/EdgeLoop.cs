using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils;
using Utils.Geometry;

namespace Motorways.EdgeLoopOperator
{
	// Token: 0x02000525 RID: 1317
	[NullableContext(1)]
	[Nullable(0)]
	public class EdgeLoop
	{
		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x060022CB RID: 8907 RVA: 0x0008CF07 File Offset: 0x0008B107
		public LinkedList<Vertex> DebugVertices
		{
			get
			{
				return this._vertices;
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x060022CC RID: 8908 RVA: 0x0008CF0F File Offset: 0x0008B10F
		public bool IsEmpty
		{
			get
			{
				return this._vertices.Count == 0;
			}
		}

		// Token: 0x060022CD RID: 8909 RVA: 0x0008CF20 File Offset: 0x0008B120
		public EdgeLoop(MapVisualGroupType visualGroupType, MapMeshLayer meshLayer)
		{
			this._visualGroupType = visualGroupType;
			this._meshLayer = meshLayer;
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x0008D024 File Offset: 0x0008B224
		private float GetDiagonalShiftDistance()
		{
			switch (this._meshLayer)
			{
			case MapMeshLayer.Land:
				return 0.3f;
			case MapMeshLayer.MountainA:
				return 0.7f;
			case MapMeshLayer.MountainB:
				return 0.1f;
			case MapMeshLayer.MountainC:
				return -0.2f;
			case MapMeshLayer.Shadow:
				return 0f;
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x0008D088 File Offset: 0x0008B288
		public Vector2[] Get2DPointArray()
		{
			Vector2[] points = new Vector2[this._vertices.Count];
			int vertexIndex = 0;
			foreach (Vertex vertex in this._vertices)
			{
				points[vertexIndex] = vertex.position;
				vertexIndex++;
			}
			return points;
		}

		// Token: 0x060022D0 RID: 8912 RVA: 0x0008D100 File Offset: 0x0008B300
		public void AddPoint(Vector3 position, TopologyType topologyType)
		{
			this._vertices.AddLast(new Vertex(position, topologyType));
		}

		// Token: 0x060022D1 RID: 8913 RVA: 0x0008D118 File Offset: 0x0008B318
		public void Decimate()
		{
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			List<LinkedListNode<Vertex>> vertsForDeletion = new List<LinkedListNode<Vertex>>();
			do
			{
				Vector3 currentPosition = queryOrigin.Value.position;
				if (Mathf.Abs(currentPosition.x % 1f) > 1E-45f || Mathf.Abs(currentPosition.y % 1f) > 1E-45f)
				{
					vertsForDeletion.Add(queryOrigin);
				}
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
			foreach (LinkedListNode<Vertex> node in vertsForDeletion)
			{
				this._vertices.Remove(node);
			}
		}

		// Token: 0x060022D2 RID: 8914 RVA: 0x0008D1D0 File Offset: 0x0008B3D0
		public void DiagonalizeSteppedSections()
		{
			this.MarkComplexCorners();
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			LinkedListNode<Vertex>[] query = new LinkedListNode<Vertex>[3];
			List<EdgeLoop.TopologyQueryResult> queryResults = new List<EdgeLoop.TopologyQueryResult>();
			do
			{
				query[0] = queryOrigin.LoopingPrevious<Vertex>();
				query[1] = queryOrigin;
				query[2] = queryOrigin.LoopingNext<Vertex>();
				foreach (EdgeLoop.TopologyQueryResult queryResult in this.QueryTopology(query))
				{
					if (queryResult.match)
					{
						queryResults.Add(queryResult);
					}
				}
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
			foreach (EdgeLoop.TopologyQueryResult queryResult2 in queryResults)
			{
				if (queryResult2.updateType == EdgeLoop.TopologyQueryResult.UpdateType.Deletion)
				{
					this._vertices.Remove(queryResult2.nodeForUpdate);
				}
				else if (queryResult2.updateType == EdgeLoop.TopologyQueryResult.UpdateType.Topology)
				{
					queryResult2.nodeForUpdate.Value.topologyType = queryResult2.newTopologyType;
				}
			}
			this.DeleteFlatVertices(true);
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x0008D2F4 File Offset: 0x0008B4F4
		private void MarkComplexCorners()
		{
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			LinkedListNode<Vertex>[] query = new LinkedListNode<Vertex>[4];
			EdgeLoop.TopologyPattern problemChild = new EdgeLoop.TopologyPattern(new TopologyType[]
			{
				TopologyType.Convex,
				TopologyType.Concave,
				TopologyType.Concave,
				TopologyType.Convex
			}, false, false, false, TopologyType.None, 1);
			List<LinkedListNode<Vertex>> nodesForFreeze = new List<LinkedListNode<Vertex>>();
			query[0] = startNode;
			for (int i = 1; i < query.Length; i++)
			{
				query[i] = query[i - 1].LoopingNext<Vertex>();
			}
			do
			{
				if (problemChild.Match(query))
				{
					foreach (LinkedListNode<Vertex> node in query)
					{
						node.Value.isComplexCorner = true;
						nodesForFreeze.Add(node);
					}
				}
				for (int j = 0; j < query.Length; j++)
				{
					if (j == query.Length - 1)
					{
						query[j] = query[j].LoopingNext<Vertex>();
					}
					else
					{
						query[j] = query[j + 1];
					}
				}
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
			foreach (LinkedListNode<Vertex> linkedListNode in nodesForFreeze)
			{
				linkedListNode.Value.topologyType = TopologyType.ComplexCorner;
			}
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x0008D420 File Offset: 0x0008B620
		private void UnmarkComplexCorners()
		{
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			do
			{
				if (queryOrigin.Value.isComplexCorner)
				{
					queryOrigin.Value.topologyType = this.CalculateTopology(queryOrigin);
				}
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x0008D468 File Offset: 0x0008B668
		private TopologyType CalculateTopology(LinkedListNode<Vertex> node)
		{
			Vertex prev = node.LoopingPrevious<Vertex>().Value;
			Vertex next = node.LoopingNext<Vertex>().Value;
			return this.CalculateTopology(node.Value.position, next.position, prev.position);
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x0008D4AC File Offset: 0x0008B6AC
		private TopologyType CalculateTopology(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 lhs = b - a;
			Vector3 ac = c - a;
			Vector3 cr = Vector3.Cross(lhs, ac);
			if (cr.z > Mathf.Epsilon)
			{
				return TopologyType.Concave;
			}
			if (cr.z < -Mathf.Epsilon)
			{
				return TopologyType.Convex;
			}
			return TopologyType.Flat;
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x0008D4F0 File Offset: 0x0008B6F0
		public void ShiftDiagonalsInland()
		{
			this.CalculateCornerAngles();
			this.CalculateMoveVectors();
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			do
			{
				Vertex queryVertex = queryOrigin.Value;
				queryVertex.position += queryVertex.cachedMoveVector;
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
			this.PairCloseVertices();
			this.PreparePairedVertsForArcCreation();
			this.DeleteFlatVertices(false);
			this.UnmarkComplexCorners();
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x0008D55C File Offset: 0x0008B75C
		public void SmoothCorners()
		{
			this.CalculateCornerAngles();
			this.CalculateVertexInfoForSmoothCorners();
			LinkedListNode<Vertex> currentNode = this._vertices.First;
			LinkedListNode<Vertex> startNode = currentNode;
			List<EdgeLoop.CornerBuildOrder> cornerBuildOrders = new List<EdgeLoop.CornerBuildOrder>();
			do
			{
				Vertex currentVertex = currentNode.Value;
				LinkedListNode<Vertex> nextNode = currentNode.LoopingNext<Vertex>();
				LinkedListNode<Vertex> prevNode = currentNode.LoopingPrevious<Vertex>();
				if (currentVertex.HasPairedVertex)
				{
					LinkedListNode<Vertex> centerNode;
					LinkedListNode<Vertex> centerNode2;
					if (currentNode.Value.PairedVertex == nextNode.Value)
					{
						centerNode = currentNode;
						centerNode2 = nextNode;
						currentNode = nextNode;
						if (currentNode == startNode)
						{
							break;
						}
					}
					else
					{
						centerNode = prevNode;
						centerNode2 = currentNode;
					}
					Vertex centerVert = centerNode.Value;
					Vertex centerVert2 = centerNode2.Value;
					Vector3 position = centerNode.LoopingPrevious<Vertex>().Value.position;
					Vector3 nextPosition = centerNode2.LoopingNext<Vertex>().Value.position;
					Vector3 prevDirection = (position - centerVert.position).normalized;
					Vector3 nextDirection = (nextPosition - centerVert2.position).normalized;
					cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(new LinkedListNode<Vertex>[]
					{
						centerNode,
						centerNode2
					}, prevDirection, nextDirection, 0.08f, 12, false));
				}
				else
				{
					Vector3 currentPosition = currentVertex.position;
					Vector3 position2 = nextNode.Value.position;
					Vector3 prevDirection2 = (prevNode.Value.position - currentPosition).normalized;
					Vector3 nextDirection2 = (position2 - currentPosition).normalized;
					if (this._visualGroupType == MapVisualGroupType.Land)
					{
						if (currentVertex.isRightAngle)
						{
							if (currentVertex.topologyType == TopologyType.Concave && currentVertex.GetProximity() == Vertex.Proximity.Far)
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 1.18f, 20, false));
							}
							else if (currentVertex.topologyType == TopologyType.Concave && currentVertex.GetProximity() == Vertex.Proximity.Close)
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.2f, 8, false));
							}
							else
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.3f, 8, false));
							}
						}
						else if (currentVertex.GetProximity() == Vertex.Proximity.Close)
						{
							if (currentVertex.topologyType == TopologyType.Convex)
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.2f, 9, false));
							}
							else
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.18f, 7, false));
							}
						}
						else if (currentVertex.GetProximity() == Vertex.Proximity.Medium)
						{
							if (currentVertex.topologyType == TopologyType.Convex)
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.565f, 10, false));
							}
							else
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.45f, 10, false));
							}
						}
						else if (currentVertex.topologyType == TopologyType.Convex)
						{
							cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.64f, 12, false));
						}
						else
						{
							cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.47f, 14, false));
						}
					}
					else if (this._visualGroupType == MapVisualGroupType.Mountains)
					{
						if (currentVertex.isRightAngle)
						{
							if (currentVertex.topologyType == TopologyType.Convex)
							{
								if (currentVertex.GetProximity() == Vertex.Proximity.Far && this._meshLayer == MapMeshLayer.MountainA)
								{
									cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.97f, 13, true));
								}
								else
								{
									cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.3f, 8, false));
								}
							}
							else if (currentVertex.GetProximity() == Vertex.Proximity.Far && this._meshLayer == MapMeshLayer.MountainA)
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.82f, 12, false));
							}
							else
							{
								cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.2f, 8, false));
							}
						}
						else if (this._meshLayer != MapMeshLayer.MountainA)
						{
							cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.1f, 7, false));
						}
						else if (currentVertex.topologyType == TopologyType.Convex)
						{
							cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.27f, 7, false));
						}
						else
						{
							cornerBuildOrders.Add(new EdgeLoop.CornerBuildOrder(currentNode, prevDirection2, nextDirection2, 0.7425f, 11, false));
						}
					}
				}
				currentNode = currentNode.LoopingNext<Vertex>();
			}
			while (currentNode != startNode);
			foreach (EdgeLoop.CornerBuildOrder buildOrder in cornerBuildOrders)
			{
				this.BuildCorner(buildOrder);
			}
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x0008D978 File Offset: 0x0008BB78
		private List<EdgeLoop.TopologyQueryResult> QueryTopology(IReadOnlyList<LinkedListNode<Vertex>> patternQuery)
		{
			List<EdgeLoop.TopologyQueryResult> results = new List<EdgeLoop.TopologyQueryResult>();
			foreach (EdgeLoop.TopologyPattern topologyPattern in this._topologyPatterns)
			{
				if (topologyPattern.Match(patternQuery))
				{
					if (topologyPattern.deletion)
					{
						patternQuery[0].Value.inDiagonalSection = true;
						patternQuery[2].Value.inDiagonalSection = true;
						results.Add(new EdgeLoop.TopologyQueryResult
						{
							match = true,
							updateType = EdgeLoop.TopologyQueryResult.UpdateType.Deletion,
							nodeForUpdate = patternQuery[1]
						});
					}
					if (topologyPattern.topologyUpdate)
					{
						results.Add(new EdgeLoop.TopologyQueryResult
						{
							match = true,
							updateType = EdgeLoop.TopologyQueryResult.UpdateType.Topology,
							nodeForUpdate = patternQuery[topologyPattern.updateIndex],
							newTopologyType = topologyPattern.newTopologyType
						});
					}
					if (topologyPattern.markTerminal)
					{
						patternQuery[topologyPattern.updateIndex].Value.inDiagonalSection = true;
						patternQuery[topologyPattern.updateIndex].Value.diagonalSectionTerminal = true;
					}
				}
			}
			return results;
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x0008DA80 File Offset: 0x0008BC80
		private void DeleteFlatVertices(bool onlyDiagonals)
		{
			List<LinkedListNode<Vertex>> flatVerts = new List<LinkedListNode<Vertex>>();
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			do
			{
				if (onlyDiagonals)
				{
					if (queryOrigin.Value.inDiagonalSection && !queryOrigin.Value.diagonalSectionTerminal)
					{
						flatVerts.Add(queryOrigin);
					}
				}
				else if (queryOrigin.Value.topologyType == TopologyType.Flat || (queryOrigin.Value.inDiagonalSection && !queryOrigin.Value.diagonalSectionTerminal))
				{
					flatVerts.Add(queryOrigin);
				}
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
			foreach (LinkedListNode<Vertex> flatVertNode in flatVerts)
			{
				this._vertices.Remove(flatVertNode);
			}
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x0008DB4C File Offset: 0x0008BD4C
		private void CalculateMoveVectors()
		{
			LinkedListNode<Vertex> targetNode = this._vertices.First;
			LinkedListNode<Vertex> startNode = targetNode;
			List<LinkedListNode<Vertex>> nodesForDeletion = new List<LinkedListNode<Vertex>>();
			for (;;)
			{
				Vertex queryVertex = targetNode.Value;
				if (queryVertex.inDiagonalSection)
				{
					if (!queryVertex.diagonalSectionTerminal)
					{
						goto IL_335;
					}
					LinkedListNode<Vertex> nextNode = targetNode.LoopingNext<Vertex>();
					LinkedListNode<Vertex> prevNode = targetNode.LoopingPrevious<Vertex>();
					Vector3 nextPosition = nextNode.Value.position;
					Vector3 prevPosition = prevNode.Value.position;
					if (!queryVertex.dontRecalculateMoveVector)
					{
						bool isNextCardinal = queryVertex.position.IsCardinal2D(nextPosition);
						bool isPrevCardinal = queryVertex.position.IsCardinal2D(prevPosition);
						Vector3 moveDirection;
						if (queryVertex.topologyType == TopologyType.ComplexCorner)
						{
							LinkedListNode<Vertex> c;
							LinkedListNode<Vertex> c2;
							if (nextNode.Value.topologyType == TopologyType.ComplexCorner && (nextNode.Value.position - targetNode.Value.position).magnitude < 2f)
							{
								c = nextNode;
								c2 = nextNode.LoopingNext<Vertex>();
							}
							else
							{
								c = prevNode;
								c2 = prevNode.LoopingPrevious<Vertex>();
							}
							Vector3 complexCornerBase = c2.Value.position - c.Value.position;
							c.Value.cachedMoveVector = complexCornerBase.normalized * (1f - this.GetDiagonalShiftDistance());
							c.Value.dontRecalculateMoveVector = true;
							if (!c.Value.HasPairedVertex && !c2.Value.HasPairedVertex)
							{
								Vertex.PairVertices(c.Value, c2.Value);
							}
							nodesForDeletion.Add(targetNode);
							moveDirection = Vector3.zero;
						}
						else if (queryVertex.isAcuteAngle)
						{
							Vertex splitVertex = new Vertex(queryVertex)
							{
								diagonalSectionTerminal = false,
								inDiagonalSection = false,
								dontRecalculateMoveVector = true
							};
							Vertex.PairVertices(queryVertex, splitVertex);
							if (isNextCardinal)
							{
								Vector3 tangent = nextPosition - queryVertex.position;
								splitVertex.cachedMoveVector = tangent.normalized * (this.GetDiagonalShiftDistance() * this._splitVertexCardinalEdgeShiftScalar);
								moveDirection = -tangent.RotateCCW2D();
								this._vertices.AddAfter(targetNode, splitVertex);
							}
							else
							{
								Vector3 tangent2 = prevPosition - queryVertex.position;
								splitVertex.cachedMoveVector = tangent2.normalized * (this.GetDiagonalShiftDistance() * this._splitVertexCardinalEdgeShiftScalar);
								moveDirection = -tangent2.RotateCW2D();
								this._vertices.AddBefore(targetNode, splitVertex);
							}
						}
						else if (isNextCardinal)
						{
							moveDirection = nextPosition - queryVertex.position;
						}
						else if (isPrevCardinal)
						{
							moveDirection = prevPosition - queryVertex.position;
						}
						else
						{
							if (queryVertex.topologyType != TopologyType.Concave)
							{
								break;
							}
							Vector3 prevDirection = (prevPosition - queryVertex.position).normalized;
							moveDirection = (nextPosition - queryVertex.position).normalized - prevDirection;
							Vertex splitVertex2 = new Vertex(queryVertex)
							{
								dontRecalculateMoveVector = true,
								cachedMoveVector = moveDirection.normalized * this.GetDiagonalShiftDistance()
							};
							this._vertices.AddAfter(targetNode, splitVertex2);
						}
						if (queryVertex.topologyType == TopologyType.Concave)
						{
							moveDirection = -moveDirection;
						}
						queryVertex.cachedMoveVector = moveDirection.normalized * this.GetDiagonalShiftDistance();
					}
				}
				targetNode = targetNode.LoopingNext<Vertex>();
				if (targetNode == startNode)
				{
					goto Block_15;
				}
			}
			Diagnostics.FailAssert("Unhandled topology! There should never be a case where two diagonal edges meet at a convex vertex.", Array.Empty<object>());
			return;
			IL_335:
			Diagnostics.FailAssert("Unhandled topology! All flat diagonal vertices should have been deleted by now.", Array.Empty<object>());
			return;
			Block_15:
			foreach (LinkedListNode<Vertex> node in nodesForDeletion)
			{
				node.List.Remove(node);
			}
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x0008DEF8 File Offset: 0x0008C0F8
		private void PairCloseVertices()
		{
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			do
			{
				Vertex targetVertex = queryOrigin.Value;
				Vertex nextVertex = queryOrigin.LoopingNext<Vertex>().Value;
				if (!targetVertex.HasPairedVertex && targetVertex.topologyType != TopologyType.Flat && nextVertex.topologyType != TopologyType.Flat && (targetVertex.position - nextVertex.position).magnitude < 0.5f)
				{
					Vertex.PairVertices(targetVertex, nextVertex);
					targetVertex.isComplexCorner = true;
					nextVertex.isComplexCorner = true;
				}
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
			}
			while (queryOrigin != startNode);
		}

		// Token: 0x060022DD RID: 8925 RVA: 0x0008DF84 File Offset: 0x0008C184
		private void PreparePairedVertsForArcCreation()
		{
			LinkedListNode<Vertex> queryOrigin = this._vertices.First;
			LinkedListNode<Vertex> startNode = queryOrigin;
			List<EdgeLoop.IntersectingEdgeReplacementOperation> edgeReplacementOperations = new List<EdgeLoop.IntersectingEdgeReplacementOperation>();
			for (;;)
			{
				LinkedListNode<Vertex> p = queryOrigin;
				if (!p.Value.HasPairedVertex || !p.Value.isComplexCorner)
				{
					goto IL_267;
				}
				Vertex pairedVertex = p.Value.PairedVertex;
				LinkedListNode<Vertex> nextNode = queryOrigin.LoopingNext<Vertex>();
				if (nextNode.Value != pairedVertex)
				{
					queryOrigin = queryOrigin.LoopingNext<Vertex>();
				}
				else
				{
					LinkedListNode<Vertex> p2 = queryOrigin.LoopingPrevious<Vertex>();
					LinkedListNode<Vertex> p3 = nextNode;
					LinkedListNode<Vertex> p4 = nextNode.LoopingNext<Vertex>();
					float pairDistance = (p.Value.position - p3.Value.position).magnitude;
					bool hasCardinalEdge = false;
					if (p.Value.position.IsCardinal2D(p2.Value.position))
					{
						Vector3 nonPairedAdjacentDirection = (p2.Value.position - p.Value.position).normalized;
						p.Value.position += nonPairedAdjacentDirection * (pairDistance * this._splitVertexCardinalEdgeShiftScalar);
						hasCardinalEdge = true;
					}
					if (p3.Value.position.IsCardinal2D(p4.Value.position))
					{
						Vector3 nonPairedAdjacentDirection2 = (p4.Value.position - p3.Value.position).normalized;
						p3.Value.position += nonPairedAdjacentDirection2 * (pairDistance * this._splitVertexCardinalEdgeShiftScalar);
						hasCardinalEdge = true;
					}
					Vector2 intersect;
					if (!hasCardinalEdge && LineIntersection.IntersectLines(p2.Value.position, p.Value.position, p3.Value.position, p4.Value.position, out intersect, LineIntersection.LineIntersectMode.Segments))
					{
						TopologyType topologyType;
						if (Vector3.Cross(p2.Value.position - intersect, p4.Value.position - intersect).z < 0f)
						{
							topologyType = TopologyType.Concave;
						}
						else
						{
							topologyType = TopologyType.Convex;
						}
						Vertex c = new Vertex(intersect, topologyType)
						{
							diagonalSectionTerminal = true,
							inDiagonalSection = true,
							isRightAngle = true
						};
						edgeReplacementOperations.Add(new EdgeLoop.IntersectingEdgeReplacementOperation
						{
							p1 = p,
							p2 = p3,
							c = c
						});
						goto IL_267;
					}
					goto IL_267;
				}
				IL_26E:
				if (queryOrigin == startNode)
				{
					break;
				}
				continue;
				IL_267:
				queryOrigin = queryOrigin.LoopingNext<Vertex>();
				goto IL_26E;
			}
			foreach (EdgeLoop.IntersectingEdgeReplacementOperation edgeReplacementOperation in edgeReplacementOperations)
			{
				this._vertices.AddAfter(edgeReplacementOperation.p1, edgeReplacementOperation.c);
				this._vertices.Remove(edgeReplacementOperation.p1);
				this._vertices.Remove(edgeReplacementOperation.p2);
			}
		}

		// Token: 0x060022DE RID: 8926 RVA: 0x0008E280 File Offset: 0x0008C480
		private void CalculateCornerAngles()
		{
			LinkedListNode<Vertex> targetNode = this._vertices.First;
			LinkedListNode<Vertex> startNode = targetNode;
			do
			{
				Vertex value = targetNode.Value;
				Vector3 a = value.position;
				Vector3 b = targetNode.LoopingNext<Vertex>().Value.position;
				Vector3 position = targetNode.LoopingPrevious<Vertex>().Value.position;
				Vector3 ab = b - a;
				Vector3 ac = position - a;
				float angleToAB = Mathf.Atan2(ab.y, ab.x);
				float angleBetweenACAB = Mathf.Atan2(ac.y, ac.x) - angleToAB;
				if (angleBetweenACAB < 0f)
				{
					angleBetweenACAB += 6.2831855f;
				}
				value.isRightAngle = (Mathf.Approximately(angleBetweenACAB, 1.5707964f) || Mathf.Approximately(angleBetweenACAB, 4.712389f));
				value.isAcuteAngle = Mathf.Approximately(angleBetweenACAB, 0.7853982f);
				targetNode = targetNode.LoopingNext<Vertex>();
			}
			while (targetNode != startNode);
		}

		// Token: 0x060022DF RID: 8927 RVA: 0x0008E35C File Offset: 0x0008C55C
		private void CalculateVertexInfoForSmoothCorners()
		{
			LinkedListNode<Vertex> targetNode = this._vertices.First;
			LinkedListNode<Vertex> startNode = targetNode;
			do
			{
				Vertex value = targetNode.Value;
				Vector3 a = value.position;
				Vertex next = targetNode.LoopingNext<Vertex>().Value;
				Vertex value2 = targetNode.LoopingPrevious<Vertex>().Value;
				Vector3 b = next.position;
				Vector3 position = value2.position;
				Vector3 ab = b - a;
				Vector3 ac = position - a;
				float closestSqrDistance = Mathf.Min(ab.sqrMagnitude, ac.sqrMagnitude);
				value.sqrDistanceToClosestConnectedVertex = closestSqrDistance;
				targetNode = targetNode.LoopingNext<Vertex>();
			}
			while (targetNode != startNode);
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x0008E3E4 File Offset: 0x0008C5E4
		private void BuildCorner(EdgeLoop.CornerBuildOrder buildOrder)
		{
			Vector3 curveBaseStart = buildOrder.targetNodes[0].Value.position;
			Vector3 curveBaseEnd = buildOrder.targetNodes[buildOrder.targetNodes.Length - 1].Value.position;
			ValueTuple<Vector3, Vector3> curveTerminals = this.GetCurveStartPoints(curveBaseStart, curveBaseEnd, buildOrder.prevDirection, buildOrder.nextDirection, buildOrder.cornerDepth);
			foreach (Vector3 newPosition in this.GetSmoothPoints(curveTerminals.Item1, buildOrder.prevDirection, curveTerminals.Item2, buildOrder.nextDirection, buildOrder.numberOfPoints, buildOrder.Concave, buildOrder.dualArcCorner))
			{
				Vertex vertex = new Vertex(newPosition, buildOrder.targetNodes[0].Value.topologyType);
				buildOrder.targetNodes[0].List.AddBefore(buildOrder.targetNodes[0], vertex);
			}
			foreach (LinkedListNode<Vertex> targetNode in buildOrder.targetNodes)
			{
				targetNode.List.Remove(targetNode);
			}
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x0008E50C File Offset: 0x0008C70C
		[NullableContext(0)]
		private ValueTuple<Vector3, Vector3> GetCurveStartPoints(Vector3 curveBaseStart, Vector3 curveBaseEnd, Vector3 prevDirection, Vector3 nextDirection, float cornerDepth)
		{
			Vector3 item = curveBaseStart + prevDirection * cornerDepth;
			Vector3 endPosition = curveBaseEnd + nextDirection * cornerDepth;
			return new ValueTuple<Vector3, Vector3>(item, endPosition);
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x0008E540 File Offset: 0x0008C740
		private List<Vector3> GetSmoothPoints(Vector3 startPosition, Vector3 startDirection, Vector3 endPosition, Vector3 endDirection, int numberOfPoints, bool concave, bool dualArc)
		{
			List<Vector3> result = new List<Vector3>();
			Vector3 normal = startDirection.RotateCW2D();
			Vector3 normal2 = endDirection.RotateCCW2D();
			Vector3 arcCenter = this.GetArcCenterFromNormals(startPosition, normal, endPosition, normal2);
			Vector3[] arcCenters;
			if (dualArc)
			{
				arcCenters = new Vector3[]
				{
					Vector3.Lerp(arcCenter, startPosition, 0.32f),
					Vector3.Lerp(arcCenter, endPosition, 0.32f)
				};
			}
			else
			{
				arcCenters = new Vector3[]
				{
					arcCenter,
					arcCenter
				};
			}
			Vector3 ab = startPosition - arcCenter;
			Vector3 ac = endPosition - arcCenter;
			float r = ab.magnitude;
			if (dualArc)
			{
				r *= 0.68f;
			}
			float angleToAB = Mathf.Atan2(ab.y, ab.x);
			float angleBetweenACAB = Mathf.Atan2(ac.y, ac.x) - angleToAB;
			if (concave)
			{
				if (angleBetweenACAB < 0f)
				{
					angleBetweenACAB += 6.2831855f;
				}
			}
			else if (angleBetweenACAB > 3.1415927f)
			{
				angleBetweenACAB = -(6.2831855f - angleBetweenACAB);
			}
			float startAngle = angleToAB;
			for (int i = 0; i < numberOfPoints; i++)
			{
				float currentRelativeAngle = (float)i / (float)(numberOfPoints - 1) * angleBetweenACAB;
				float currentAngle = startAngle + currentRelativeAngle;
				Vector3 currentArcCenter = (i < numberOfPoints / 2) ? arcCenters[0] : arcCenters[1];
				Vector3 currentPoint = new Vector3(currentArcCenter.x + r * Mathf.Cos(currentAngle), currentArcCenter.y + r * Mathf.Sin(currentAngle));
				result.Add(currentPoint);
			}
			return result;
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x0008E6B8 File Offset: 0x0008C8B8
		private Vector3 GetArcCenterFromNormals(Vector3 point1, Vector3 normal1, Vector3 point2, Vector3 normal2)
		{
			Vector2 intersect;
			if (LineIntersection.IntersectLines(point1, point1 + normal1, point2, point2 + normal2, out intersect, LineIntersection.LineIntersectMode.Lines))
			{
				return intersect;
			}
			Diagnostics.FailAssert("No intersection found!", Array.Empty<object>());
			return default(Vector3);
		}

		// Token: 0x04001CDC RID: 7388
		private readonly LinkedList<Vertex> _vertices = new LinkedList<Vertex>();

		// Token: 0x04001CDD RID: 7389
		private readonly MapVisualGroupType _visualGroupType;

		// Token: 0x04001CDE RID: 7390
		private readonly MapMeshLayer _meshLayer;

		// Token: 0x04001CDF RID: 7391
		private readonly EdgeLoop.TopologyPattern[] _topologyPatterns = new EdgeLoop.TopologyPattern[]
		{
			new EdgeLoop.TopologyPattern(TopologyType.Flat, TopologyType.Concave, TopologyType.Convex, true, true, true, TopologyType.Concave, 0),
			new EdgeLoop.TopologyPattern(TopologyType.Convex, TopologyType.Concave, TopologyType.Flat, true, true, true, TopologyType.Concave, 2),
			new EdgeLoop.TopologyPattern(TopologyType.ComplexCorner, TopologyType.Concave, TopologyType.Convex, true, false, true, TopologyType.None, 0),
			new EdgeLoop.TopologyPattern(TopologyType.Convex, TopologyType.Concave, TopologyType.ComplexCorner, true, false, true, TopologyType.None, 2),
			new EdgeLoop.TopologyPattern(TopologyType.Convex, TopologyType.Concave, TopologyType.Convex, true, false, false, TopologyType.None, 1),
			new EdgeLoop.TopologyPattern(TopologyType.Concave, TopologyType.Convex, TopologyType.Flat | TopologyType.ComplexCorner, false, false, true, TopologyType.None, 1),
			new EdgeLoop.TopologyPattern(TopologyType.Flat | TopologyType.ComplexCorner, TopologyType.Convex, TopologyType.Concave, false, false, true, TopologyType.None, 1),
			new EdgeLoop.TopologyPattern(TopologyType.Convex, TopologyType.Concave, TopologyType.Concave, true, false, true, TopologyType.None, 2),
			new EdgeLoop.TopologyPattern(TopologyType.Concave, TopologyType.Concave, TopologyType.Convex, true, false, true, TopologyType.None, 0),
			new EdgeLoop.TopologyPattern(TopologyType.Concave, TopologyType.Convex, TopologyType.Convex, false, false, true, TopologyType.None, 1),
			new EdgeLoop.TopologyPattern(TopologyType.Convex, TopologyType.Convex, TopologyType.Concave, false, false, true, TopologyType.None, 1)
		};

		// Token: 0x04001CE0 RID: 7392
		private readonly float _splitVertexCardinalEdgeShiftScalar = Mathf.Sqrt(2f) - 1f;

		// Token: 0x02000526 RID: 1318
		[Nullable(0)]
		private class CornerBuildOrder
		{
			// Token: 0x17000639 RID: 1593
			// (get) Token: 0x060022E4 RID: 8932 RVA: 0x0008E713 File Offset: 0x0008C913
			public bool Concave
			{
				get
				{
					return this.targetNodes[0].Value.topologyType == TopologyType.Concave;
				}
			}

			// Token: 0x060022E5 RID: 8933 RVA: 0x0008E72A File Offset: 0x0008C92A
			public CornerBuildOrder(LinkedListNode<Vertex> targetNode, Vector3 prevDirection, Vector3 nextDirection, float cornerDepth, int numberOfPoints, bool dualArcCorner)
			{
				this.targetNodes = new LinkedListNode<Vertex>[]
				{
					targetNode
				};
				this.prevDirection = prevDirection;
				this.nextDirection = nextDirection;
				this.cornerDepth = cornerDepth;
				this.numberOfPoints = numberOfPoints;
				this.dualArcCorner = dualArcCorner;
			}

			// Token: 0x060022E6 RID: 8934 RVA: 0x0008E768 File Offset: 0x0008C968
			public CornerBuildOrder(LinkedListNode<Vertex>[] targetNodes, Vector3 prevDirection, Vector3 nextDirection, float cornerDepth, int numberOfPoints, bool dualArcCorner)
			{
				this.targetNodes = targetNodes;
				this.prevDirection = prevDirection;
				this.nextDirection = nextDirection;
				this.cornerDepth = cornerDepth;
				this.numberOfPoints = numberOfPoints;
				this.dualArcCorner = dualArcCorner;
			}

			// Token: 0x04001CE1 RID: 7393
			public readonly LinkedListNode<Vertex>[] targetNodes;

			// Token: 0x04001CE2 RID: 7394
			public readonly Vector3 prevDirection;

			// Token: 0x04001CE3 RID: 7395
			public readonly Vector3 nextDirection;

			// Token: 0x04001CE4 RID: 7396
			public readonly float cornerDepth;

			// Token: 0x04001CE5 RID: 7397
			public readonly int numberOfPoints;

			// Token: 0x04001CE6 RID: 7398
			public readonly bool dualArcCorner;
		}

		// Token: 0x02000527 RID: 1319
		[Nullable(0)]
		private class IntersectingEdgeReplacementOperation
		{
			// Token: 0x04001CE7 RID: 7399
			public LinkedListNode<Vertex> p1;

			// Token: 0x04001CE8 RID: 7400
			public LinkedListNode<Vertex> p2;

			// Token: 0x04001CE9 RID: 7401
			public Vertex c;
		}

		// Token: 0x02000528 RID: 1320
		[NullableContext(0)]
		private class TopologyQueryResult
		{
			// Token: 0x04001CEA RID: 7402
			public bool match;

			// Token: 0x04001CEB RID: 7403
			public EdgeLoop.TopologyQueryResult.UpdateType updateType;

			// Token: 0x04001CEC RID: 7404
			[Nullable(1)]
			public LinkedListNode<Vertex> nodeForUpdate;

			// Token: 0x04001CED RID: 7405
			public TopologyType newTopologyType;

			// Token: 0x02000529 RID: 1321
			public enum UpdateType
			{
				// Token: 0x04001CEF RID: 7407
				None,
				// Token: 0x04001CF0 RID: 7408
				Deletion,
				// Token: 0x04001CF1 RID: 7409
				Topology
			}
		}

		// Token: 0x0200052A RID: 1322
		[Nullable(0)]
		private class TopologyPattern
		{
			// Token: 0x060022E9 RID: 8937 RVA: 0x0008E7A0 File Offset: 0x0008C9A0
			public TopologyPattern(TopologyType t0, TopologyType t1, TopologyType t2, bool deletion, bool topologyUpdate, bool markTerminal, TopologyType newTopologyType = TopologyType.None, int updateIndex = 1)
			{
				this._pattern = new TopologyType[]
				{
					t0,
					t1,
					t2
				};
				this.deletion = deletion;
				this.topologyUpdate = topologyUpdate;
				this.markTerminal = markTerminal;
				this.newTopologyType = newTopologyType;
				this.updateIndex = updateIndex;
			}

			// Token: 0x060022EA RID: 8938 RVA: 0x0008E7F3 File Offset: 0x0008C9F3
			public TopologyPattern(TopologyType[] pattern, bool deletion, bool topologyUpdate, bool markTerminal, TopologyType newTopologyType = TopologyType.None, int updateIndex = 1)
			{
				this._pattern = pattern;
				this.deletion = deletion;
				this.topologyUpdate = topologyUpdate;
				this.markTerminal = markTerminal;
				this.newTopologyType = newTopologyType;
				this.updateIndex = updateIndex;
			}

			// Token: 0x060022EB RID: 8939 RVA: 0x0008E828 File Offset: 0x0008CA28
			public bool Match(IReadOnlyList<LinkedListNode<Vertex>> query)
			{
				if (query.Count != this._pattern.Length)
				{
					Diagnostics.FailAssert(string.Format("Cannot match with query of length {0} with a pattern of length {1}!", query.Count, this._pattern.Length), Array.Empty<object>());
					return false;
				}
				for (int i = 0; i < this._pattern.Length; i++)
				{
					if ((this._pattern[i] & query[i].Value.topologyType) == TopologyType.None)
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x04001CF2 RID: 7410
			private readonly TopologyType[] _pattern;

			// Token: 0x04001CF3 RID: 7411
			public readonly bool deletion;

			// Token: 0x04001CF4 RID: 7412
			public readonly bool topologyUpdate;

			// Token: 0x04001CF5 RID: 7413
			public readonly bool markTerminal;

			// Token: 0x04001CF6 RID: 7414
			public readonly TopologyType newTopologyType;

			// Token: 0x04001CF7 RID: 7415
			public readonly int updateIndex;
		}
	}
}
