using System;
using Factory;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004E8 RID: 1256
	public class EasterEggModel : Model<EasterEggModel.Frame, EasterEggModel.IObserver>
	{
		// Token: 0x060020DE RID: 8414 RVA: 0x000823F4 File Offset: 0x000805F4
		public bool ShouldBeEasterEggVehicle(VehicleModel vehicleModel)
		{
			if (base.CurrentFrame.currentEasterEggVehicle != null)
			{
				return base.CurrentFrame.currentEasterEggVehicle == vehicleModel;
			}
			if (this._gameScope.Get<MotorwaysGame>().MapDefinition.CityNameEnum.ToString().Equals("Copenhagen") && vehicleModel.house.GroupIndex == 0 && UnityEngine.Random.value < 1f)
			{
				base.CurrentFrame.currentEasterEggVehicle = vehicleModel;
				base.NextFrame.currentEasterEggVehicle = vehicleModel;
				return true;
			}
			return false;
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x00082480 File Offset: 0x00080680
		public EasterEggModel() : base(1)
		{
		}

		// Token: 0x04001B4A RID: 6986
		private const string EasterEggCityName = "Copenhagen";

		// Token: 0x04001B4B RID: 6987
		private const int EasterEggGroupIndex = 0;

		// Token: 0x04001B4C RID: 6988
		private const float EasterEggSpawnProbability = 1f;

		// Token: 0x04001B4D RID: 6989
		[Dependency]
		private IScope _gameScope;

		// Token: 0x020004E9 RID: 1257
		public class Frame : IFrame
		{
			// Token: 0x060020E0 RID: 8416 RVA: 0x00082489 File Offset: 0x00080689
			public void Reset()
			{
				this.currentEasterEggVehicle = null;
			}

			// Token: 0x060020E1 RID: 8417 RVA: 0x00082492 File Offset: 0x00080692
			public bool CloneInto(IFrame cloneState, IScope scope)
			{
				((EasterEggModel.Frame)cloneState).currentEasterEggVehicle = this.currentEasterEggVehicle;
				return true;
			}

			// Token: 0x04001B4E RID: 6990
			public VehicleModel currentEasterEggVehicle;
		}

		// Token: 0x020004EA RID: 1258
		public interface IObserver
		{
			// Token: 0x060020E3 RID: 8419
			void OnEasterEggVehicleChanged(int oldVehicleId, int newVehicleId);
		}
	}
}
