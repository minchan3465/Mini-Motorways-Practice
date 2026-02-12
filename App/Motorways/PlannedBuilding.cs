using System;
using Factory;
using Motorways.Models;
using Motorways.Processes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000370 RID: 880
	[System.Serializable]
	public struct PlannedBuilding
	{
		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x0600157C RID: 5500 RVA: 0x00049D08 File Offset: 0x00047F08
		public bool PrefersDoubleCarpark
		{
			get
			{
				return this.carparkPreference == CarparkPreference.Double || this.carparkPreference == CarparkPreference.ForceDouble;
			}
		}

		// Token: 0x040011FF RID: 4607
		public CityTileType type;

		// Token: 0x04001200 RID: 4608
		public int groupIndex;

		// Token: 0x04001201 RID: 4609
		[Tooltip("How much extra demand should this destination have? Defaults to zero")]
		public float additionalDemandMultiplier;

		// Token: 0x04001202 RID: 4610
		[Tooltip("If Use Fixed Position is true, what position should it be?")]
		public Vector2Int positionOverride;

		// Token: 0x04001203 RID: 4611
		[Tooltip("Should we use a fixed position")]
		public bool useFixedPosition;

		// Token: 0x04001204 RID: 4612
		public bool useFixedParameters;

		// Token: 0x04001205 RID: 4613
		[Tooltip("What is our preference of carpark?")]
		public CarparkPreference carparkPreference;

		// Token: 0x04001206 RID: 4614
		[Tooltip("An identifier that can be used to refer to this building in tutorial code")]
		public TutorialIdentifier tutorialIdentifier;

		// Token: 0x04001207 RID: 4615
		public TileDirection directionOverride;

		// Token: 0x04001208 RID: 4616
		public CarparkEntrance entranceOverride;

		// Token: 0x04001209 RID: 4617
		public GroupingStyle grouping;

		// Token: 0x0400120A RID: 4618
		public TileDirection drivewayDirectionOverride;

		// Token: 0x02000371 RID: 881
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x0600157D RID: 5501 RVA: 0x00049D20 File Offset: 0x00047F20
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is PlannedBuilding)
				{
					PlannedBuilding building = (PlannedBuilding)obj;
					context.Writer.Write((int)building.type);
					context.Writer.Write(building.groupIndex);
					context.Writer.Write(building.additionalDemandMultiplier);
					context.Writer.Write(building.positionOverride.x);
					context.Writer.Write(building.positionOverride.y);
					context.Writer.Write(building.useFixedParameters);
					context.Writer.Write((int)building.carparkPreference);
					context.Writer.Write((int)building.directionOverride);
					context.Writer.Write((int)building.entranceOverride);
					context.Writer.Write((int)building.grouping);
					context.Writer.Write((int)building.drivewayDirectionOverride);
					context.Writer.Write((int)building.tutorialIdentifier);
					return true;
				}
				return false;
			}

			// Token: 0x0600157E RID: 5502 RVA: 0x00049E1C File Offset: 0x0004801C
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return new PlannedBuilding
				{
					type = (CityTileType)context.Reader.ReadInt32(),
					groupIndex = context.Reader.ReadInt32(),
					additionalDemandMultiplier = context.Reader.ReadSingle(),
					positionOverride = new Vector2Int(context.Reader.ReadInt32(), context.Reader.ReadInt32()),
					useFixedParameters = context.Reader.ReadBoolean(),
					carparkPreference = (CarparkPreference)context.Reader.ReadInt32(),
					directionOverride = (TileDirection)context.Reader.ReadInt32(),
					entranceOverride = (CarparkEntrance)context.Reader.ReadInt32(),
					grouping = (GroupingStyle)context.Reader.ReadInt32(),
					drivewayDirectionOverride = (TileDirection)context.Reader.ReadInt32(),
					tutorialIdentifier = (TutorialIdentifier)context.Reader.ReadInt32()
				};
			}
		}
	}
}
