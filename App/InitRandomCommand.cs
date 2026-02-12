using System;

// Token: 0x02000078 RID: 120
public class InitRandomCommand : AppCommand
{
	// Token: 0x0600012B RID: 299 RVA: 0x00004D00 File Offset: 0x00002F00
	public bool Configure(uint seed)
	{
		this._seed = seed;
		return true;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00004D0A File Offset: 0x00002F0A
	public override void Reset()
	{
		this._seed = 0U;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00004D13 File Offset: 0x00002F13
	public override bool Execute(IApp receiver)
	{
		global::Random.SetSimulationSeed(this._seed, receiver.Scope);
		return true;
	}

	// Token: 0x0400006A RID: 106
	private uint _seed;
}
