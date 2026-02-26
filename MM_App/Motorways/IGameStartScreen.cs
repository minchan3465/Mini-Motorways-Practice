using System;

namespace Motorways
{
	// Token: 0x020003AC RID: 940
	public interface IGameStartScreen
	{
		// Token: 0x06001653 RID: 5715
		void PrepareForNewGame(CityDefinition newCity, MapDefinition newMapDefinition, MotorwaysGame game, MapChallenge newMapChallenge = null, bool startPaused = false);
	}
}
