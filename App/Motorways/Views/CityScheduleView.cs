using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Themes;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200059C RID: 1436
	public class CityScheduleView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x0600281D RID: 10269 RVA: 0x000AB260 File Offset: 0x000A9460
		public string GetBuildingStateInfo(CityPlanModel.ScheduledBuilding building, Theme theme)
		{
			Color color = theme.GetBuildingColor(building.groupIndex, ThemeComponentGroupTarget.BuildingBase);
			int week = Mathf.FloorToInt((float)building.time / 0.8333333f / 24f / 7f);
			int day = Mathf.FloorToInt((float)building.time / 0.8333333f / 24f) % 7;
			string buildingType;
			if (building.grouping == GroupingStyle.Circle)
			{
				buildingType = "Upgrade ";
			}
			else
			{
				buildingType = building.type.ToString();
				if (building.carparkPreference == CarparkPreference.Double)
				{
					buildingType = "Double " + buildingType;
				}
			}
			string buildingString = string.Format("Week {0}, Day {1}-<color=#{2}>{3}-group:{4}</color>", new object[]
			{
				week,
				day,
				ColorUtility.ToHtmlStringRGB(color),
				buildingType,
				building.groupIndex
			});
			if (building.spawnAttempts > 0)
			{
				buildingString += string.Format(" Retry {0} in {1:F1}", building.spawnAttempts, (float)(building.time - this._clock.ExpansionTime));
				if (building.type == CityTileType.Demand)
				{
					if (building.spawnAttempts > this._constants.MaxFailedBuildingSpawnsBeforeIgnoringWeights)
					{
						if (building.grouping == GroupingStyle.Circle)
						{
							buildingString += " -> to nrml spwn";
						}
						else
						{
							buildingString += " w\\o weights";
						}
					}
				}
				else if (building.spawnAttempts > this._constants.MaxFailedBuildingSpawnsBeforeIgnoringWeights)
				{
					buildingString += " w\\o weights";
				}
			}
			return buildingString;
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x0600281E RID: 10270 RVA: 0x000AB3E8 File Offset: 0x000A95E8
		private bool ShouldShowDebugScheduleView
		{
			get
			{
				return FeatureToggle.IsFeatureEnabled(Feature.ScheduleView);
			}
		}

		// Token: 0x0600281F RID: 10271 RVA: 0x000AB3F6 File Offset: 0x000A95F6
		private void OnEnable()
		{
			this._style.fontSize = 30;
			this._style.normal.textColor = Color.red;
			this._style.richText = true;
		}

		// Token: 0x06002820 RID: 10272 RVA: 0x000AB426 File Offset: 0x000A9626
		public void Reset()
		{
			this._showPendingBuildings = true;
			this._showReallocatedDemand = true;
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x000020AA File Offset: 0x000002AA
		TickResult IView.Tick(TimeInterval timeInterval, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x040021E3 RID: 8675
		[Dependency]
		private ClockModel _clock;

		// Token: 0x040021E4 RID: 8676
		[Dependency]
		private CityPlanModel _cityPlan;

		// Token: 0x040021E5 RID: 8677
		[Dependency]
		private DemandModel _demand;

		// Token: 0x040021E6 RID: 8678
		[Dependency]
		private IThemeDatabase _themeDatabase;

		// Token: 0x040021E7 RID: 8679
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x040021E8 RID: 8680
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040021E9 RID: 8681
		public const string ShouldShowScheduleView = "ShouldShowScheduleView";

		// Token: 0x040021EA RID: 8682
		private bool _showPendingBuildings = true;

		// Token: 0x040021EB RID: 8683
		private bool _showReallocatedDemand = true;

		// Token: 0x040021EC RID: 8684
		private GUIStyle _style = new GUIStyle();
	}
}
