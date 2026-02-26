using System;

namespace Motorways.Audio
{
	// Token: 0x020006DA RID: 1754
	public class VehicleInstancer : ImmediateAudioModule
	{
		// Token: 0x06003028 RID: 12328 RVA: 0x000DF017 File Offset: 0x000DD217
		public VehicleInstancer(AudioEventFilter filter) : base(filter, "", 1f, -1f, "", 1f)
		{
		}

		// Token: 0x06003029 RID: 12329 RVA: 0x000E2420 File Offset: 0x000E0620
		protected override void OnAudioEvent(AudioEvent e)
		{
			default(AudioEventFilter).Vehicle = e.Vehicle;
			Playback playback = new Vehicle(e.Vehicle);
			IAudioModule newVehicleModule = PulsedAudioModule.CreateModule("Vehicle " + e.Vehicle.Id.ToString(), playback, null, 1);
			Get.Loadout.AddDynamicModule(newVehicleModule);
		}
	}
}
