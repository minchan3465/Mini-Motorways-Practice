using System;

// Token: 0x0200011A RID: 282
public class InternetConnectionHandle : IDisposable
{
	// Token: 0x0600060C RID: 1548 RVA: 0x00015D88 File Offset: 0x00013F88
	public InternetConnectionHandle(IReachability reachability)
	{
		this._isOpen = true;
		this._reachability = reachability;
		this.Id = InternetConnectionHandle.NextId;
		InternetConnectionHandle.NextId++;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00015DB8 File Offset: 0x00013FB8
	~InternetConnectionHandle()
	{
		this.Close();
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x0600060E RID: 1550 RVA: 0x00015DE4 File Offset: 0x00013FE4
	public int Id { get; }

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x0600060F RID: 1551 RVA: 0x00015DEC File Offset: 0x00013FEC
	public bool IsAvailable
	{
		get
		{
			return this._reachability.Connectivity == InternetConnectivity.Connected;
		}
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00015DFC File Offset: 0x00013FFC
	public void Close()
	{
		if (this._isOpen)
		{
			this._reachability.CloseConnection(this);
			this._isOpen = false;
		}
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00015E19 File Offset: 0x00014019
	public void Dispose()
	{
		this.Close();
	}

	// Token: 0x0400028F RID: 655
	private bool _isOpen;

	// Token: 0x04000290 RID: 656
	private IReachability _reachability;

	// Token: 0x04000291 RID: 657
	private static int NextId = 1;
}
