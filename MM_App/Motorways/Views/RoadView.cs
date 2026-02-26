using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Constants;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005F7 RID: 1527
	[SelectionBase]
	public class RoadView : MonoBehaviour, IView, IReusable, ICreatedInScopeHandler
	{
		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06002A72 RID: 10866 RVA: 0x000B9E3D File Offset: 0x000B803D
		// (set) Token: 0x06002A73 RID: 10867 RVA: 0x000B9E48 File Offset: 0x000B8048
		public TileView tileView
		{
			get
			{
				return this._tileView;
			}
			set
			{
				this._tileView = value;
				this._permanenceProgressRoadView = ((this._tileView == null) ? null : new PermanenceProgressRoadView(this._materialPropertyBlock, this.baseRenderer, this._tileView, this._permanenceZoneTextureLibrary, this._visualConstants, this._city.Rules.RoadsBecomePermanentOverTime));
			}
		}

		// Token: 0x06002A74 RID: 10868 RVA: 0x000B9EA6 File Offset: 0x000B80A6
		public void Awake()
		{
			base.GetComponent<MeshRenderer>().sharedMaterial = this.activeMaterial;
			this._roadOutlineSortingLayerId = SortingLayer.NameToID("RoadOutline");
			this._motorwayRoadConnectionOutlineSortingLayerId = SortingLayer.NameToID("MotorwayRoadConnectionOutline");
			this._materialPropertyBlock = new MaterialPropertyBlock();
		}

		// Token: 0x06002A75 RID: 10869 RVA: 0x000B9EE4 File Offset: 0x000B80E4
		public void OnCreatedInScope(IScope scope)
		{
			using (RoadTileSignature deadEndSignature = scope.Get<RoadTileSignature>())
			{
				deadEndSignature.AddConnection(new RoadTileConnection(new RoadTileNode(TileDirection.South, RoadType.TwoLane, -1), new RoadTileNode(TileDirection.South, RoadType.TwoLane, -1)));
				this.bridgeSpouts.SetDryingTunnelMesh(this._roadTileAtlas.GetDefinitionForSignature(deadEndSignature));
			}
		}

		// Token: 0x06002A76 RID: 10870 RVA: 0x000B9F48 File Offset: 0x000B8148
		public void Reset()
		{
			this._tileView = null;
			Transform transform = base.transform;
			transform.localPosition = default(Vector3);
			transform.localRotation = default(Quaternion);
		}

		// Token: 0x06002A77 RID: 10871 RVA: 0x000B9F7F File Offset: 0x000B817F
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._city.Rules.RoadsBecomePermanentOverTime && this._tileView != null)
			{
				this._permanenceProgressRoadView.UpdatePermanenceValues();
				return TickResult.ContinueTicking;
			}
			return TickResult.StopTicking;
		}

		// Token: 0x06002A78 RID: 10872 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002A79 RID: 10873 RVA: 0x000B9FB0 File Offset: 0x000B81B0
		public void SetSignature(RoadTileSignature newSignature)
		{
			RoadTileDefinition newDefinition = this._roadTileAtlas.GetDefinitionForSignature(newSignature);
			if (!Diagnostics.Verify(newDefinition != null, "Tile at {0} has invalid visual signature {1}.", (this._tileView != null) ? this._tileView.Coordinates : default(Vector2Int), newSignature))
			{
				return;
			}
			this.baseMesh.mesh = newDefinition.mesh.roadMesh;
			this.outlineMesh.mesh = newDefinition.mesh.outlineMesh;
			this.bridgeSpouts.DisableAllSpouts();
			this.baseRenderer.enabled = true;
			this.dashedOutlineRenderer.enabled = false;
			this.dryingTunnelRenderer.enabled = false;
			bool connectedToMotorway = false;
			foreach (RoadTileConnection connection in newSignature.Connections)
			{
				connectedToMotorway = (connection.input.type == RoadType.Motorway || connection.output.type == RoadType.Motorway);
				if (connectedToMotorway)
				{
					break;
				}
			}
			this.outlineRenderer.sortingLayerID = (connectedToMotorway ? this._motorwayRoadConnectionOutlineSortingLayerId : this._roadOutlineSortingLayerId);
			if (this._tileView != null && this._city.Definition.TileIsOverWater(this._tileView.Coordinates))
			{
				this.baseRenderer.material = this.bridgeMaterial;
				this.outlineRenderer.material = this.bridgeOutlineMaterial;
				using (IEnumerator<RoadTileConnection> enumerator = newSignature.Connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RoadTileConnection roadTileConnection = enumerator.Current;
						TileDirection directionIn = roadTileConnection.input.direction;
						TileDirection directionOut = roadTileConnection.output.direction;
						if (!this._city.Definition.TileIsOverWater(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, directionIn)))
						{
							this.bridgeSpouts.SetSpoutActiveInDirection(directionIn, UpgradeType.Bridge);
						}
						if (!this._city.Definition.TileIsOverWater(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, directionOut)))
						{
							this.bridgeSpouts.SetSpoutActiveInDirection(directionOut, UpgradeType.Bridge);
						}
					}
					goto IL_478;
				}
			}
			if (this._tileView != null && this._city.Definition.TileIsUnderAMountain(this._tileView.Coordinates))
			{
				this.outlineRenderer.material = this.outlineMaterial;
				this.dashedOutlineRenderer.material = this.tunnelOutlineMaterial;
				this.dashedOutlineMesh.mesh = ((newDefinition.mesh.dashedOutlineMesh != null) ? newDefinition.mesh.dashedOutlineMesh : newDefinition.mesh.outlineMesh);
				this.dashedOutlineRenderer.enabled = true;
				float tunnelPermanenceAlpha = 0f;
				if (this._city.Rules.RoadsBecomePermanentOverTime)
				{
					TileDirectionBitfield tileNodes = this.tileView.Tile.GetTwoLaneRoads(RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Ignore);
					if (tileNodes.Count > 0)
					{
						Fix64 tunnelPermanence = this.tileView.Tile.GetNodePermanenceProgress(tileNodes[0]);
						if (tunnelPermanence < Fix64.One)
						{
							tunnelPermanenceAlpha = (1f - this._visualConstants.DryingTunnelFalloff.Evaluate((float)tunnelPermanence)) * this._visualConstants.MaxDryingTunnelOpacity;
						}
					}
				}
				foreach (RoadTileConnection roadTileConnection2 in newSignature.Connections)
				{
					TileDirection directionIn2 = roadTileConnection2.input.direction;
					TileDirection directionOut2 = roadTileConnection2.output.direction;
					if (!this._city.Definition.TileIsUnderAMountain(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, directionIn2)))
					{
						this.bridgeSpouts.SetSpoutActiveInDirection(directionIn2, UpgradeType.Tunnel);
					}
					if (!this._city.Definition.TileIsUnderAMountain(TileUtilities.GetAdjacentCoordinates(this._tileView.Coordinates, directionOut2)))
					{
						this.bridgeSpouts.SetSpoutActiveInDirection(directionOut2, UpgradeType.Tunnel);
					}
				}
				if (tunnelPermanenceAlpha > 0f)
				{
					this.dryingTunnelMesh.mesh = newDefinition.mesh.roadMesh;
					this.dryingTunnelRenderer.enabled = true;
					if (this._dryingTunnelPropertyBlock == null)
					{
						this._dryingTunnelPropertyBlock = new MaterialPropertyBlock();
					}
					this._dryingTunnelPropertyBlock.SetFloat(ShaderConstants.Alpha, tunnelPermanenceAlpha);
					this.dryingTunnelRenderer.SetPropertyBlock(this._dryingTunnelPropertyBlock);
					this.bridgeSpouts.ShowDryingTunnel(this._dryingTunnelPropertyBlock);
				}
			}
			else
			{
				this.baseRenderer.material = this.activeMaterial;
				this.outlineRenderer.material = this.outlineMaterial;
			}
			IL_478:
			base.transform.localRotation = Quaternion.Euler(0f, 0f, (float)(-TileUtilities.GetRotationAngle(newDefinition.rotation)));
			this.bridgeSpouts.transform.localRotation = Quaternion.Euler(0f, 0f, (float)TileUtilities.GetRotationAngle(newDefinition.rotation));
		}

		// Token: 0x06002A7A RID: 10874 RVA: 0x000BA4C0 File Offset: 0x000B86C0
		public void ReconfigurePermanenceVisibility()
		{
			bool isPermanenceVisible = this._city.Rules.RoadsBecomePermanentOverTime;
			PermanenceProgressRoadView permanenceProgressRoadView = this._permanenceProgressRoadView;
			if (permanenceProgressRoadView != null)
			{
				permanenceProgressRoadView.SetPermanenceVisibility(isPermanenceVisible);
			}
			if (!isPermanenceVisible)
			{
				this.dryingTunnelRenderer.enabled = false;
				this.bridgeSpouts.HideDryingTunnel();
			}
		}

		// Token: 0x04002488 RID: 9352
		[Dependency]
		private City _city;

		// Token: 0x04002489 RID: 9353
		[Dependency]
		private RoadTileAtlas _roadTileAtlas;

		// Token: 0x0400248A RID: 9354
		[Dependency]
		private PermanenceZoneTextureLibrary _permanenceZoneTextureLibrary;

		// Token: 0x0400248B RID: 9355
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x0400248C RID: 9356
		public Material activeMaterial;

		// Token: 0x0400248D RID: 9357
		public Material mothballedMaterial;

		// Token: 0x0400248E RID: 9358
		public Material outlineMaterial;

		// Token: 0x0400248F RID: 9359
		public Material bridgeMaterial;

		// Token: 0x04002490 RID: 9360
		public Material bridgeOutlineMaterial;

		// Token: 0x04002491 RID: 9361
		public Material tunnelOutlineMaterial;

		// Token: 0x04002492 RID: 9362
		public MeshFilter baseMesh;

		// Token: 0x04002493 RID: 9363
		public MeshFilter outlineMesh;

		// Token: 0x04002494 RID: 9364
		public MeshFilter dashedOutlineMesh;

		// Token: 0x04002495 RID: 9365
		public MeshFilter dryingTunnelMesh;

		// Token: 0x04002496 RID: 9366
		public MeshRenderer baseRenderer;

		// Token: 0x04002497 RID: 9367
		public MeshRenderer outlineRenderer;

		// Token: 0x04002498 RID: 9368
		public MeshRenderer dashedOutlineRenderer;

		// Token: 0x04002499 RID: 9369
		public MeshRenderer dryingTunnelRenderer;

		// Token: 0x0400249A RID: 9370
		public BridgeSpouts bridgeSpouts;

		// Token: 0x0400249B RID: 9371
		[Serialize(false, null)]
		private TileView _tileView;

		// Token: 0x0400249C RID: 9372
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x0400249D RID: 9373
		private MaterialPropertyBlock _dryingTunnelPropertyBlock;

		// Token: 0x0400249E RID: 9374
		private PermanenceProgressRoadView _permanenceProgressRoadView;

		// Token: 0x0400249F RID: 9375
		public const string DrawLaneLinesMode = "DrawLaneLinesMode";

		// Token: 0x040024A0 RID: 9376
		public const string ShouldDrawPathfindingLaneCosts = "ShouldDrawPathfindingLaneCosts";

		// Token: 0x040024A1 RID: 9377
		public const string ShouldDrawPathfindingNodeIds = "ShouldDrawPathfindingNodeIds";

		// Token: 0x040024A2 RID: 9378
		public const string ShouldDrawLinesFromVehiclesToAllChunksInTheirPath = "ShouldDrawLinesFromVehiclesToAllChunksInTheirPath";

		// Token: 0x040024A3 RID: 9379
		private const string RoadOutlineSortingLayerName = "RoadOutline";

		// Token: 0x040024A4 RID: 9380
		private int _roadOutlineSortingLayerId;

		// Token: 0x040024A5 RID: 9381
		private const string MotorwayRoadConnectionOutlineLayerName = "MotorwayRoadConnectionOutline";

		// Token: 0x040024A6 RID: 9382
		private int _motorwayRoadConnectionOutlineSortingLayerId;

		// Token: 0x020005F8 RID: 1528
		public enum LaneLineDebugMode
		{
			// Token: 0x040024A8 RID: 9384
			None,
			// Token: 0x040024A9 RID: 9385
			Speed,
			// Token: 0x040024AA RID: 9386
			Hotswap
		}
	}
}
