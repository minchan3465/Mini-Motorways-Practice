using System;
using System.Collections.Generic;
using FixMath;
using Motorways;
using Motorways.Models;

namespace Server
{
	// Token: 0x02000286 RID: 646
	public class IdleVehicleChecker : ISimulationObserver
	{
		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x00035BCB File Offset: 0x00033DCB
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x00035BD3 File Offset: 0x00033DD3
		private bool HasSentReport { get; set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x00035BDC File Offset: 0x00033DDC
		public IReadOnlyList<IdleVehicleChecker.VehicleInformation> IdleVehicles
		{
			get
			{
				return this._idleVehicles.AsReadOnly();
			}
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x00035BE9 File Offset: 0x00033DE9
		public void Initialize(MotorwaysGame motorwaysGame)
		{
			this._motorwaysGame = motorwaysGame;
			this._simulation = motorwaysGame.Simulation;
			this._simulation.Subscribe(this);
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00035C0C File Offset: 0x00033E0C
		public void RunCheck()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.IdleVehicleCheckerDiagnosticReport) || FeatureToggle.IsFeatureEnabled(Feature.IdleVehicleCheckerGUI))
			{
				ClockModel clockModel = this._simulation.GetModel<ClockModel>();
				if (clockModel == null)
				{
					return;
				}
				foreach (IdleVehicleChecker.VehicleInformation vehicleInformation in this._vehicleInformation)
				{
					Fix64? vehicleStoppedTimestamp = vehicleInformation.vehicleStoppedTimestamp;
					VehicleModel vehicleModel = vehicleInformation.vehicleModel;
					if (vehicleStoppedTimestamp != null)
					{
						if (!IdleVehicleChecker.HasStoppedOnJourney(vehicleModel))
						{
							vehicleInformation.vehicleStoppedTimestamp = null;
							if (this._idleVehicles.Contains(vehicleInformation))
							{
								this._idleVehicles.Remove(vehicleInformation);
							}
						}
						else if (!this._idleVehicles.Contains(vehicleInformation))
						{
							Fix64 idleTime = clockModel.Time - vehicleStoppedTimestamp.Value;
							if (idleTime > IdleVehicleChecker.MaxVehicleIdleTimeBeforeWarning)
							{
								IdleVehicleChecker.Log.Warn("Vehicle ({0}) has been idle while driving to destination/home for {1} seconds", new object[]
								{
									vehicleModel.id,
									idleTime
								});
								this._idleVehicles.Add(vehicleInformation);
								this.SubmitReportIfNotSubmittedAlready(vehicleInformation, idleTime);
							}
						}
					}
					else if (IdleVehicleChecker.HasStoppedOnJourney(vehicleModel))
					{
						vehicleInformation.vehicleStoppedTimestamp = new Fix64?(clockModel.Time);
					}
				}
			}
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00035D74 File Offset: 0x00033F74
		private void SubmitReportIfNotSubmittedAlready(IdleVehicleChecker.VehicleInformation vehicleInformation, Fix64 idleTime)
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.IdleVehicleCheckerDiagnosticReport))
			{
				if (this.HasSentReport)
				{
					return;
				}
				if (FeatureToggle.IsFeatureEnabled(Feature.DiagnosticReports))
				{
					Diagnostics.Report report = this._motorwaysGame.GenerateDiagnosticReport("VehicleIdle-Test", DiagnosticReportAttachments.SimCommandJournal | DiagnosticReportAttachments.Screenshot);
					report.SetMetadata("idleVehicleId", vehicleInformation.vehicleModel.id.ToString(), false);
					report.SetMetadata("idleTime", idleTime.ToString(), false);
					report.Upload();
					this.HasSentReport = true;
				}
			}
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x00035DEF File Offset: 0x00033FEF
		private static bool HasStoppedOnJourney(VehicleModel vehicleModel)
		{
			return vehicleModel.behaviorState != VehicleModel.BehaviorState.WaitingForDestination && vehicleModel.CurrentFrame.speed - Fix64.Zero < IdleVehicleChecker.SpeedEpsilon;
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x00035E1C File Offset: 0x0003401C
		public void OnModelAdded(ISimulation simulation, IModel model, Fix64 timestamp)
		{
			VehicleModel vehicleModel = model as VehicleModel;
			if (vehicleModel != null)
			{
				this._vehicleInformation.Add(new IdleVehicleChecker.VehicleInformation(vehicleModel));
			}
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnModelRemoved(ISimulation simulation, IModel model, Fix64 timestamp)
		{
		}

		// Token: 0x04000E38 RID: 3640
		public static readonly Fix64 MaxVehicleIdleTimeBeforeWarning = (Fix64)60f;

		// Token: 0x04000E39 RID: 3641
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("IdleVehicleChecker");

		// Token: 0x04000E3A RID: 3642
		private readonly List<IdleVehicleChecker.VehicleInformation> _idleVehicles = new List<IdleVehicleChecker.VehicleInformation>();

		// Token: 0x04000E3B RID: 3643
		private readonly List<IdleVehicleChecker.VehicleInformation> _vehicleInformation = new List<IdleVehicleChecker.VehicleInformation>();

		// Token: 0x04000E3D RID: 3645
		private MotorwaysGame _motorwaysGame;

		// Token: 0x04000E3E RID: 3646
		private ISimulation _simulation;

		// Token: 0x04000E3F RID: 3647
		private static readonly Fix64 SpeedEpsilon = (Fix64)0.01f;

		// Token: 0x02000287 RID: 647
		public class VehicleInformation
		{
			// Token: 0x06000FF5 RID: 4085 RVA: 0x00035E91 File Offset: 0x00034091
			public VehicleInformation(VehicleModel vehicleModel)
			{
				this.vehicleModel = vehicleModel;
			}

			// Token: 0x06000FF6 RID: 4086 RVA: 0x00035EA0 File Offset: 0x000340A0
			public override bool Equals(object obj)
			{
				IdleVehicleChecker.VehicleInformation vehicleInformation = obj as IdleVehicleChecker.VehicleInformation;
				return vehicleInformation != null && this.vehicleModel != null && vehicleInformation.vehicleModel != null && this.vehicleModel.id == vehicleInformation.vehicleModel.id;
			}

			// Token: 0x06000FF7 RID: 4087 RVA: 0x00035EE3 File Offset: 0x000340E3
			public override int GetHashCode()
			{
				return this.vehicleModel.id;
			}

			// Token: 0x04000E40 RID: 3648
			public readonly VehicleModel vehicleModel;

			// Token: 0x04000E41 RID: 3649
			public Fix64? vehicleStoppedTimestamp;
		}
	}
}
