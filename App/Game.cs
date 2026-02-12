using System;
using Client;
using Factory;
using FixMath;
using Server;

// Token: 0x02000157 RID: 343
public class Game : IControllerConnectionObserver, IReleasedFromScopeHandler
{
	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06000771 RID: 1905 RVA: 0x0001898A File Offset: 0x00016B8A
	// (set) Token: 0x06000772 RID: 1906 RVA: 0x00018992 File Offset: 0x00016B92
	[Dependency]
	public IScope Scope { get; private set; }

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06000773 RID: 1907 RVA: 0x0001899B File Offset: 0x00016B9B
	// (set) Token: 0x06000774 RID: 1908 RVA: 0x000189A3 File Offset: 0x00016BA3
	public GameStartReason StartReason { get; private set; }

	// Token: 0x06000775 RID: 1909 RVA: 0x000189AC File Offset: 0x00016BAC
	public void Start(GameStartReason gameStartReason)
	{
		this.StartReason = gameStartReason;
		this._themeDatabase.AddView(this._view);
		this._view.Start();
		this._accumulatedTime = this._simulation.Timestep;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x000189E2 File Offset: 0x00016BE2
	public virtual void AddArbitraryAccumulatedTime(Fix64 additionalAccumulatedTime)
	{
		this._accumulatedTime += additionalAccumulatedTime;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000189F6 File Offset: 0x00016BF6
	public virtual void OnGameStarted()
	{
		this.Scope.Get<IInputState>().SubscribeToControllerConnectionMessages(this);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00018A09 File Offset: 0x00016C09
	public virtual void OnGameEnd(GameEndReason gameEndReason)
	{
		this.Scope.Get<IInputState>().UnsubscribeFromControllerConnectionMessages(this);
		this.Scope.Get<PlayerActionController>().GameEnded();
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0000222C File Offset: 0x0000042C
	public virtual bool TrySave(GameJournalMotive motive)
	{
		return false;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00018A2C File Offset: 0x00016C2C
	public virtual void Tick(float frameTime)
	{
		this._timeInterval.UnsyncedDelta = frameTime;
		this._timeInterval.Delta = frameTime;
		this.AdjustTimeInterval(this._timeInterval);
		this._accumulatedTime += (Fix64)this._timeInterval.UnpausedScaledDelta;
		while (this._accumulatedTime >= this._simulation.Timestep)
		{
			this._simulation.Step();
			this._accumulatedTime -= this._simulation.Timestep;
		}
		float stepAlpha = (float)(this._accumulatedTime / this._simulation.Timestep);
		this._view.Tick(this._timeInterval, stepAlpha);
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00018AF0 File Offset: 0x00016CF0
	public virtual void SetPaused(bool isPaused)
	{
		Game.Log.Info("{0} the simulation.", new object[]
		{
			isPaused ? "Pausing" : "Resuming"
		});
		this._simulation.ScheduleCommand(SetPausedCommand.Create(this.Scope, isPaused));
		this._timeInterval.IsPaused = isPaused;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void StopAudio()
	{
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x000022F5 File Offset: 0x000004F5
	protected virtual void AdjustTimeInterval(TimeInterval timeInterval)
	{
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00018B48 File Offset: 0x00016D48
	public virtual void OnReleasedFromScope(IScope scope)
	{
		this._themeDatabase.RemoveView(this._view);
		this.StopAudio();
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x000020AA File Offset: 0x000002AA
	public virtual bool CanInteract()
	{
		return true;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00018B61 File Offset: 0x00016D61
	public void OnControllerConnected(IController controller)
	{
		if (this.CanInteract())
		{
			controller.RegisterInputActionsForGame(this.Scope);
			controller.EnsureActionsAreRegistered(this.Scope);
		}
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00018B83 File Offset: 0x00016D83
	public void OnControllerDisconnected(IController controller)
	{
		if (typeof(IScopeObserver).IsAssignableFrom(controller.GetType()))
		{
			this.Scope.Unsubscribe((IScopeObserver)controller);
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06000782 RID: 1922 RVA: 0x00018BAD File Offset: 0x00016DAD
	public ISimulation Simulation
	{
		get
		{
			return this._simulation;
		}
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00018BB5 File Offset: 0x00016DB5
	public void SetTimeScale(TimeScale scale)
	{
		this._timeInterval.Scale = scale;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00018BC3 File Offset: 0x00016DC3
	public TimeScale GetTimeScale()
	{
		return this._timeInterval.Scale;
	}

	// Token: 0x0400037E RID: 894
	[Dependency]
	protected IThemeDatabase _themeDatabase;

	// Token: 0x0400037F RID: 895
	[Dependency]
	protected ISimulation _simulation;

	// Token: 0x04000380 RID: 896
	[Dependency]
	protected IClient _view;

	// Token: 0x04000383 RID: 899
	private Fix64 _accumulatedTime = Fix64.Zero;

	// Token: 0x04000384 RID: 900
	protected TimeInterval _timeInterval = new TimeInterval();

	// Token: 0x04000385 RID: 901
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Game");
}
