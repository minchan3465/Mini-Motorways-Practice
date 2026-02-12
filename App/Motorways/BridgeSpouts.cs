using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000457 RID: 1111
	public class BridgeSpouts : MonoBehaviour
	{
		// Token: 0x06001BC5 RID: 7109 RVA: 0x000655D4 File Offset: 0x000637D4
		public void SetDryingTunnelMesh(RoadTileDefinition deadEndMesh)
		{
			foreach (TileDirection direction in TileUtilities.Directions)
			{
				MeshFilter dryingTunnelMesh = this.GetSpoutInDirection(direction).dryingTunnelMesh;
				dryingTunnelMesh.mesh = deadEndMesh.mesh.roadMesh;
				dryingTunnelMesh.transform.localRotation = Quaternion.Euler(0f, 0f, (float)(-TileUtilities.GetRotationAngle(deadEndMesh.rotation)));
			}
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x00065648 File Offset: 0x00063848
		public void DisableAllSpouts()
		{
			for (int childIndex = 0; childIndex < base.transform.childCount; childIndex++)
			{
				base.transform.GetChild(childIndex).gameObject.SetActive(false);
			}
			this._visibleSpouts.Clear();
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x00065690 File Offset: 0x00063890
		public void SetSpoutActiveInDirection(TileDirection direction, UpgradeType upgradeType)
		{
			PassageSpout spout = this.GetSpoutInDirection(direction);
			if (Diagnostics.Verify(spout != null))
			{
				this._visibleSpouts[direction] = true;
				spout.gameObject.SetActive(true);
				if (upgradeType == UpgradeType.Bridge)
				{
					spout.ShowBridge();
					return;
				}
				if (upgradeType != UpgradeType.Tunnel)
				{
					return;
				}
				spout.ShowTunnel();
			}
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x000656E4 File Offset: 0x000638E4
		public void ShowDryingTunnel(MaterialPropertyBlock propertyBlock)
		{
			foreach (TileDirection direction in this._visibleSpouts)
			{
				this.GetSpoutInDirection(direction).ShowDryingTunnel(propertyBlock);
			}
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x00065720 File Offset: 0x00063920
		public void HideDryingTunnel()
		{
			foreach (TileDirection direction in this._visibleSpouts)
			{
				this.GetSpoutInDirection(direction).HideDryingTunnel();
			}
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x00065758 File Offset: 0x00063958
		private PassageSpout GetSpoutInDirection(TileDirection direction)
		{
			switch (direction)
			{
			case TileDirection.North:
				return this.N;
			case TileDirection.NorthEast:
				return this.NE;
			case TileDirection.East:
				return this.E;
			case TileDirection.SouthEast:
				return this.SE;
			case TileDirection.South:
				return this.S;
			case TileDirection.SouthWest:
				return this.SW;
			case TileDirection.West:
				return this.W;
			case TileDirection.NorthWest:
				return this.NW;
			default:
				Diagnostics.FailAssert("Failed to find the {0} spout.", new object[]
				{
					direction
				});
				return null;
			}
		}

		// Token: 0x0400171F RID: 5919
		public PassageSpout N;

		// Token: 0x04001720 RID: 5920
		public PassageSpout NE;

		// Token: 0x04001721 RID: 5921
		public PassageSpout E;

		// Token: 0x04001722 RID: 5922
		public PassageSpout SE;

		// Token: 0x04001723 RID: 5923
		public PassageSpout S;

		// Token: 0x04001724 RID: 5924
		public PassageSpout SW;

		// Token: 0x04001725 RID: 5925
		public PassageSpout W;

		// Token: 0x04001726 RID: 5926
		public PassageSpout NW;

		// Token: 0x04001727 RID: 5927
		private TileDirectionBitfield _visibleSpouts;
	}
}
