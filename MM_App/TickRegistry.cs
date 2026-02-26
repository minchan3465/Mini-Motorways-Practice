using System;
using System.Collections.Generic;

// Token: 0x0200024A RID: 586
public class TickRegistry
{
	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06000E02 RID: 3586 RVA: 0x0002F707 File Offset: 0x0002D907
	// (remove) Token: 0x06000E03 RID: 3587 RVA: 0x0002F715 File Offset: 0x0002D915
	public event TickRegistry.TickDelegate AppTicking
	{
		add
		{
			this._tickDelegates.Add(value);
		}
		remove
		{
			this._tickDelegates.Remove(value);
		}
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0002F724 File Offset: 0x0002D924
	public void Tick(float deltaTime)
	{
		this._currentlyTickingDelegates.AddRange(this._tickDelegates);
		foreach (TickRegistry.TickDelegate tickDelegate in this._currentlyTickingDelegates)
		{
			tickDelegate(deltaTime);
		}
		this._currentlyTickingDelegates.Clear();
	}

	// Token: 0x04000831 RID: 2097
	private List<TickRegistry.TickDelegate> _tickDelegates = new List<TickRegistry.TickDelegate>();

	// Token: 0x04000832 RID: 2098
	private List<TickRegistry.TickDelegate> _currentlyTickingDelegates = new List<TickRegistry.TickDelegate>();

	// Token: 0x0200024B RID: 587
	// (Invoke) Token: 0x06000E07 RID: 3591
	public delegate void TickDelegate(float deltaTime);
}
