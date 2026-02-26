using System;
using System.Collections.Generic;
using Factory;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200033D RID: 829
	[CreateAssetMenu(fileName = "New Challenge", menuName = "Motorways/Challenges/Challenge", order = 3)]
	public class ChallengeData : ScriptableObject
	{
		// Token: 0x06001483 RID: 5251 RVA: 0x00042EA4 File Offset: 0x000410A4
		public override string ToString()
		{
			string result = base.name;
			if (this.modifiers.Count > 0)
			{
				result += string.Format(" ({0}", this.modifiers[0]);
				for (int modifierIndex = 1; modifierIndex < this.modifiers.Count; modifierIndex++)
				{
					result += string.Format(", {0}", this.modifiers[modifierIndex]);
				}
				result += ")";
			}
			return result;
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00042F24 File Offset: 0x00041124
		public bool IsCompatibleWith(MapDefinition city)
		{
			if (this.incompatibleMaps.Contains(ChallengeSystem.GetCityName(city)))
			{
				return false;
			}
			using (List<CityChallengeCompatibilityGroup>.Enumerator enumerator = this.incompatibleCityGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsMapCompatible(ChallengeSystem.GetCityName(city)))
					{
						return false;
					}
				}
			}
			using (List<ChallengeModifier>.Enumerator enumerator2 = this.modifiers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (!enumerator2.Current.IsCompatibleWithMap(city))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00042FE0 File Offset: 0x000411E0
		public bool IsIncompatibleWith(ChallengeData otherChallenge)
		{
			return this.automaticIncompatibleChallenges.Contains(otherChallenge) || this.manualIncompatibleChallenges.Contains(otherChallenge);
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x00043004 File Offset: 0x00041204
		public bool AreModifiersCompatibleWith(ChallengeData otherChallenge)
		{
			foreach (ChallengeModifier ourModifier in this.modifiers)
			{
				foreach (ChallengeModifier otherModifier in otherChallenge.modifiers)
				{
					if (!ourModifier.IsCompatibleWith(otherModifier))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x000430A0 File Offset: 0x000412A0
		public float GetSelectedModifierLocalizationParameter()
		{
			if (Diagnostics.Verify(this.modifierToUseForLocalization >= 0 && this.modifierToUseForLocalization < this.modifiers.Count, "Incorrect modifier index for localisation parameter! Have {0}, max {1}", this.modifierToUseForLocalization, this.modifiers.Count))
			{
				return this.modifiers[this.modifierToUseForLocalization].GetLocalizationParameter();
			}
			return -1f;
		}

		// Token: 0x040010E1 RID: 4321
		[EnumSearch(typeof(StringId), true)]
		public string challengeName;

		// Token: 0x040010E2 RID: 4322
		[EnumSearch(typeof(StringId), true)]
		public string challengeDescription;

		// Token: 0x040010E3 RID: 4323
		public Sprite icon;

		// Token: 0x040010E4 RID: 4324
		private const string SubIcon = "SubIcon";

		// Token: 0x040010E5 RID: 4325
		[FoldoutGroup("SubIcon")]
		public Sprite subIconBackground;

		// Token: 0x040010E6 RID: 4326
		[FoldoutGroup("SubIcon")]
		public Sprite subIcon;

		// Token: 0x040010E7 RID: 4327
		public List<ChallengeModifier> modifiers = new List<ChallengeModifier>();

		// Token: 0x040010E8 RID: 4328
		public int modifierToUseForLocalization;

		// Token: 0x040010E9 RID: 4329
		private const string Incompatibilities = "Incompatibilities";

		// Token: 0x040010EA RID: 4330
		[FoldoutGroup("Incompatibilities")]
		public List<MapDefinition.CityNames> incompatibleMaps = new List<MapDefinition.CityNames>();

		// Token: 0x040010EB RID: 4331
		[FoldoutGroup("Incompatibilities")]
		public List<CityChallengeCompatibilityGroup> incompatibleCityGroups = new List<CityChallengeCompatibilityGroup>();

		// Token: 0x040010EC RID: 4332
		[FoldoutGroup("Incompatibilities")]
		public List<ChallengeData> automaticIncompatibleChallenges = new List<ChallengeData>();

		// Token: 0x040010ED RID: 4333
		[FoldoutGroup("Incompatibilities")]
		public List<ChallengeData> manualIncompatibleChallenges = new List<ChallengeData>();

		// Token: 0x0200033E RID: 830
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x06001489 RID: 5257 RVA: 0x00043150 File Offset: 0x00041350
			public override bool Serialize(object obj, ExportContext context)
			{
				ChallengeData data = obj as ChallengeData;
				if (data != null)
				{
					context.Writer.Write(data.name);
					return true;
				}
				return false;
			}

			// Token: 0x0600148A RID: 5258 RVA: 0x0004317C File Offset: 0x0004137C
			public override object Deserialize(object existingObj, ImportContext context)
			{
				string name = context.Reader.ReadString();
				ChallengeData challenge;
				if (context.Scope.Get<ChallengeDatabase>().TryGetChallenge(name, out challenge))
				{
					return challenge;
				}
				return null;
			}
		}
	}
}
