using System;
using Factory;

namespace Motorways
{
	// Token: 0x02000394 RID: 916
	[System.Serializable]
	public struct UpgradePackageDefinition
	{
		// Token: 0x040012AA RID: 4778
		public UpgradeType type;

		// Token: 0x040012AB RID: 4779
		public int amount;

		// Token: 0x040012AC RID: 4780
		public int additionalConcrete;

		// Token: 0x02000395 RID: 917
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x060015ED RID: 5613 RVA: 0x0004B4CC File Offset: 0x000496CC
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is UpgradePackageDefinition)
				{
					UpgradePackageDefinition upgradePackageDefinition = (UpgradePackageDefinition)obj;
					context.Writer.Write((int)upgradePackageDefinition.type);
					context.Writer.Write(upgradePackageDefinition.amount);
					context.Writer.Write(upgradePackageDefinition.additionalConcrete);
					return true;
				}
				return false;
			}

			// Token: 0x060015EE RID: 5614 RVA: 0x0004B520 File Offset: 0x00049720
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return new UpgradePackageDefinition
				{
					type = (UpgradeType)context.Reader.ReadInt32(),
					amount = context.Reader.ReadInt32(),
					additionalConcrete = context.Reader.ReadInt32()
				};
			}
		}
	}
}
