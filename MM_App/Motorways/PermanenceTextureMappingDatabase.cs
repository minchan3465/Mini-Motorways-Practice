using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Motorways
{
	// Token: 0x020003C3 RID: 963
	[CreateAssetMenu(menuName = "Motorways/PermanenceTextureMappingDatabase")]
	public class PermanenceTextureMappingDatabase : ScriptableObject
	{
		// Token: 0x1400003A RID: 58
		// (add) Token: 0x060016EB RID: 5867 RVA: 0x000534DC File Offset: 0x000516DC
		// (remove) Token: 0x060016EC RID: 5868 RVA: 0x00053514 File Offset: 0x00051714
		public event Action OnTextureMappingsUpdated;

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x060016ED RID: 5869 RVA: 0x00053549 File Offset: 0x00051749
		public int ShaderSolidZoneCount
		{
			get
			{
				return this.solidZoneShaderIndices.Length;
			}
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x00053554 File Offset: 0x00051754
		public void RefreshBakedData()
		{
			for (int generationIndex = 0; generationIndex < this.generationIndexToZoneIdMappings.Count; generationIndex++)
			{
				PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping generationIndexToZoneIdMapping = this.generationIndexToZoneIdMappings[generationIndex];
				generationIndexToZoneIdMapping.zoneId = generationIndex;
				this.generationIndexToZoneIdMappings[generationIndex] = generationIndexToZoneIdMapping;
			}
			this.FillShaderIndexToZoneIndexArray(this.generationIndexToZoneIdMappings.Count);
			this.CalculateSolidZoneShaderIndices();
			for (int generationIndex2 = 0; generationIndex2 < this.generationIndexToZoneIdMappings.Count; generationIndex2++)
			{
				PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping generationIndexToZoneIdMapping2 = this.generationIndexToZoneIdMappings[generationIndex2];
				generationIndexToZoneIdMapping2.fadeFromIndexB = -1;
				generationIndexToZoneIdMapping2.fadeToIndexB = -1;
				if (generationIndexToZoneIdMapping2.zoneType == PermanenceTextureMappingDatabase.ZoneType.Solid)
				{
					PermanenceTextureMappingDatabase.ZoneAddress solidZoneAddress = new PermanenceTextureMappingDatabase.ZoneAddress(TileDirection.None, generationIndexToZoneIdMapping2.sectionDirection, generationIndexToZoneIdMapping2.insideSectionDirection, generationIndexToZoneIdMapping2.SharingStatus);
					int solidZoneShaderIndex = this.FindShaderSolidZoneIndex(solidZoneAddress);
					generationIndexToZoneIdMapping2.fadeFromIndexA = solidZoneShaderIndex;
					generationIndexToZoneIdMapping2.fadeToIndexA = solidZoneShaderIndex;
				}
				else
				{
					generationIndexToZoneIdMapping2.fadeFromIndexA = this.FindShaderSolidZoneIndex(generationIndexToZoneIdMapping2.fadeFromZoneA);
					generationIndexToZoneIdMapping2.fadeToIndexA = this.FindShaderSolidZoneIndex(generationIndexToZoneIdMapping2.fadeToZoneA);
					if (generationIndexToZoneIdMapping2.zoneType == PermanenceTextureMappingDatabase.ZoneType.Quadrant)
					{
						generationIndexToZoneIdMapping2.fadeFromIndexB = this.FindShaderSolidZoneIndex(generationIndexToZoneIdMapping2.fadeFromZoneB);
						generationIndexToZoneIdMapping2.fadeToIndexB = this.FindShaderSolidZoneIndex(generationIndexToZoneIdMapping2.fadeToZoneB);
					}
				}
				this.generationIndexToZoneIdMappings[generationIndex2] = generationIndexToZoneIdMapping2;
			}
			this.zoneIndexToFadeIndices = new Vector4[this.generationIndexToZoneIdMappings.Count];
			foreach (PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping shaderToZoneIdMapping in this.generationIndexToZoneIdMappings)
			{
				this.zoneIndexToFadeIndices[shaderToZoneIdMapping.zoneId] = new Vector4((float)shaderToZoneIdMapping.fadeFromIndexA, (float)shaderToZoneIdMapping.fadeToIndexA, (float)shaderToZoneIdMapping.fadeFromIndexB, (float)shaderToZoneIdMapping.fadeToIndexB);
			}
			Action onTextureMappingsUpdated = this.OnTextureMappingsUpdated;
			if (onTextureMappingsUpdated == null)
			{
				return;
			}
			onTextureMappingsUpdated();
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x0005372C File Offset: 0x0005192C
		private void CalculateSolidZoneShaderIndices()
		{
			List<PermanenceTextureMappingDatabase.ZoneAddress> usedSolidZones = new List<PermanenceTextureMappingDatabase.ZoneAddress>();
			usedSolidZones.Clear();
			foreach (PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping shaderToZoneIdMapping in this.generationIndexToZoneIdMappings)
			{
				if (shaderToZoneIdMapping.zoneType != PermanenceTextureMappingDatabase.ZoneType.Solid)
				{
					usedSolidZones.Add(shaderToZoneIdMapping.fadeFromZoneA);
					usedSolidZones.Add(shaderToZoneIdMapping.fadeToZoneA);
					if (shaderToZoneIdMapping.zoneType == PermanenceTextureMappingDatabase.ZoneType.Quadrant)
					{
						usedSolidZones.Add(shaderToZoneIdMapping.fadeFromZoneB);
						usedSolidZones.Add(shaderToZoneIdMapping.fadeToZoneB);
					}
				}
			}
			usedSolidZones = usedSolidZones.Distinct<PermanenceTextureMappingDatabase.ZoneAddress>().ToList<PermanenceTextureMappingDatabase.ZoneAddress>();
			usedSolidZones.Sort(delegate(PermanenceTextureMappingDatabase.ZoneAddress a, PermanenceTextureMappingDatabase.ZoneAddress b)
			{
				if (a.tile != b.tile)
				{
					return a.tile - b.tile;
				}
				if (a.sharingStatus == b.sharingStatus)
				{
					if (a.section == b.section)
					{
						return 0;
					}
					return a.section - b.section;
				}
				else
				{
					if (a.sharingStatus != PermanenceTextureMappingDatabase.ZoneSharing.Local)
					{
						return 1;
					}
					return -1;
				}
			});
			this.solidZoneShaderIndices = usedSolidZones.ToArray();
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x00053804 File Offset: 0x00051A04
		public int FindShaderSolidZoneIndex(PermanenceTextureMappingDatabase.ZoneAddress zoneAddress)
		{
			for (int shaderId = 0; shaderId < this.solidZoneShaderIndices.Length; shaderId++)
			{
				PermanenceTextureMappingDatabase.ZoneAddress otherZoneAddress = this.solidZoneShaderIndices[shaderId];
				if (zoneAddress == otherZoneAddress)
				{
					return shaderId;
				}
			}
			return -1;
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x00053840 File Offset: 0x00051A40
		private void FillShaderIndexToZoneIndexArray(int newArraySize)
		{
			this.shaderIndexToZoneIndex = new float[newArraySize];
			for (int i = 0; i < this.shaderIndexToZoneIndex.Length; i++)
			{
				this.shaderIndexToZoneIndex[i] = 1000f;
			}
			foreach (PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping shaderToIdMapping in this.generationIndexToZoneIdMappings)
			{
				this.shaderIndexToZoneIndex[shaderToIdMapping.shaderIndex] = (float)shaderToIdMapping.zoneId;
			}
		}

		// Token: 0x04001390 RID: 5008
		public VisualConstantsData visualConstantsData;

		// Token: 0x04001391 RID: 5009
		[FormerlySerializedAs("shaderToIdMappings")]
		public List<PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping> generationIndexToZoneIdMappings = new List<PermanenceTextureMappingDatabase.GenerationIndexToZoneIdMapping>();

		// Token: 0x04001392 RID: 5010
		public float[] shaderIndexToZoneIndex;

		// Token: 0x04001393 RID: 5011
		public Vector4[] zoneIndexToFadeIndices;

		// Token: 0x04001394 RID: 5012
		[FormerlySerializedAs("solidZoneShaderIds")]
		public PermanenceTextureMappingDatabase.ZoneAddress[] solidZoneShaderIndices;

		// Token: 0x020003C4 RID: 964
		[Serializable]
		public struct ZoneAddress
		{
			// Token: 0x060016F3 RID: 5875 RVA: 0x000538DF File Offset: 0x00051ADF
			public static PermanenceTextureMappingDatabase.ZoneAddress LocalDirection(TileDirection direction)
			{
				return new PermanenceTextureMappingDatabase.ZoneAddress(TileDirection.None, direction, TileDirection.None, PermanenceTextureMappingDatabase.ZoneSharing.Local);
			}

			// Token: 0x060016F4 RID: 5876 RVA: 0x000538EA File Offset: 0x00051AEA
			public ZoneAddress(TileDirection tile, TileDirection section, TileDirection insideSection, PermanenceTextureMappingDatabase.ZoneSharing sharingStatus)
			{
				this.tile = tile;
				this.section = section;
				this.insideSection = insideSection;
				this.sharingStatus = sharingStatus;
			}

			// Token: 0x060016F5 RID: 5877 RVA: 0x00053909 File Offset: 0x00051B09
			public bool Equals(PermanenceTextureMappingDatabase.ZoneAddress other)
			{
				return this.tile == other.tile && this.section == other.section && this.insideSection == other.insideSection && this.sharingStatus == other.sharingStatus;
			}

			// Token: 0x060016F6 RID: 5878 RVA: 0x00053948 File Offset: 0x00051B48
			public override bool Equals(object obj)
			{
				if (obj is PermanenceTextureMappingDatabase.ZoneAddress)
				{
					PermanenceTextureMappingDatabase.ZoneAddress other = (PermanenceTextureMappingDatabase.ZoneAddress)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x060016F7 RID: 5879 RVA: 0x0005396D File Offset: 0x00051B6D
			public override int GetHashCode()
			{
				return (int)(((this.tile * (TileDirection)397 ^ this.section) * (TileDirection)397 ^ this.insideSection) * (TileDirection)397 ^ (TileDirection)this.sharingStatus);
			}

			// Token: 0x060016F8 RID: 5880 RVA: 0x0005399C File Offset: 0x00051B9C
			public static bool operator ==(PermanenceTextureMappingDatabase.ZoneAddress a, PermanenceTextureMappingDatabase.ZoneAddress b)
			{
				return a.Equals(b);
			}

			// Token: 0x060016F9 RID: 5881 RVA: 0x000539A6 File Offset: 0x00051BA6
			public static bool operator !=(PermanenceTextureMappingDatabase.ZoneAddress a, PermanenceTextureMappingDatabase.ZoneAddress b)
			{
				return !a.Equals(b);
			}

			// Token: 0x060016FA RID: 5882 RVA: 0x000539B4 File Offset: 0x00051BB4
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.tile.ToShortString(),
					", ",
					this.section.ToShortString(),
					", ",
					this.insideSection.ToShortString(),
					", ",
					this.sharingStatus.ToString()
				});
			}

			// Token: 0x04001395 RID: 5013
			public static readonly PermanenceTextureMappingDatabase.ZoneAddress Center = new PermanenceTextureMappingDatabase.ZoneAddress(TileDirection.None, TileDirection.None, TileDirection.None, PermanenceTextureMappingDatabase.ZoneSharing.Local);

			// Token: 0x04001396 RID: 5014
			public TileDirection tile;

			// Token: 0x04001397 RID: 5015
			public TileDirection section;

			// Token: 0x04001398 RID: 5016
			public TileDirection insideSection;

			// Token: 0x04001399 RID: 5017
			public PermanenceTextureMappingDatabase.ZoneSharing sharingStatus;
		}

		// Token: 0x020003C5 RID: 965
		public enum ZoneType
		{
			// Token: 0x0400139B RID: 5019
			Solid,
			// Token: 0x0400139C RID: 5020
			Fade,
			// Token: 0x0400139D RID: 5021
			Quadrant
		}

		// Token: 0x020003C6 RID: 966
		public enum ZoneSharing
		{
			// Token: 0x0400139F RID: 5023
			Local,
			// Token: 0x040013A0 RID: 5024
			Shared,
			// Token: 0x040013A1 RID: 5025
			Phantom
		}

		// Token: 0x020003C7 RID: 967
		[Serializable]
		public struct GenerationIndexToZoneIdMapping
		{
			// Token: 0x17000458 RID: 1112
			// (get) Token: 0x060016FC RID: 5884 RVA: 0x00053A32 File Offset: 0x00051C32
			public PermanenceTextureMappingDatabase.ZoneSharing SharingStatus
			{
				get
				{
					if (this.shaderIndex < 49)
					{
						return PermanenceTextureMappingDatabase.ZoneSharing.Local;
					}
					if (this.shaderIndex < 85)
					{
						return PermanenceTextureMappingDatabase.ZoneSharing.Shared;
					}
					return PermanenceTextureMappingDatabase.ZoneSharing.Phantom;
				}
			}

			// Token: 0x060016FD RID: 5885 RVA: 0x00053A50 File Offset: 0x00051C50
			public string GetDisplayString(int index)
			{
				return string.Format("{0}: ({1}) -> ({2})  | ({3}, {4}, {5}, {6})", new object[]
				{
					index,
					this.shaderIndex,
					this.zoneId,
					this.fadeFromIndexA,
					this.fadeToIndexA,
					this.fadeFromIndexB,
					this.fadeToIndexB
				});
			}

			// Token: 0x040013A2 RID: 5026
			[FormerlySerializedAs("zoneStartId")]
			public int zoneId;

			// Token: 0x040013A3 RID: 5027
			public int fadeFromIndexA;

			// Token: 0x040013A4 RID: 5028
			public int fadeToIndexA;

			// Token: 0x040013A5 RID: 5029
			public int fadeFromIndexB;

			// Token: 0x040013A6 RID: 5030
			public int fadeToIndexB;

			// Token: 0x040013A7 RID: 5031
			[FormerlySerializedAs("shaderStartIndex")]
			public int shaderIndex;

			// Token: 0x040013A8 RID: 5032
			public PermanenceTextureMappingDatabase.ZoneType zoneType;

			// Token: 0x040013A9 RID: 5033
			public TileDirection sectionDirection;

			// Token: 0x040013AA RID: 5034
			public TileDirection insideSectionDirection;

			// Token: 0x040013AB RID: 5035
			public PermanenceTextureMappingDatabase.ZoneAddress fadeFromZoneA;

			// Token: 0x040013AC RID: 5036
			public PermanenceTextureMappingDatabase.ZoneAddress fadeToZoneA;

			// Token: 0x040013AD RID: 5037
			public PermanenceTextureMappingDatabase.ZoneAddress fadeFromZoneB;

			// Token: 0x040013AE RID: 5038
			public PermanenceTextureMappingDatabase.ZoneAddress fadeToZoneB;
		}
	}
}
