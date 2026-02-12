using System;
using System.IO;
using Factory;
using FixMath;
using Motorways;
using Server;

// Token: 0x020001B4 RID: 436
public class MotorwaysGameJournalSave : BaseGameJournalSave, IMotorwaysGameJournalHeader, IReleasedFromScopeHandler
{
	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06000A2A RID: 2602 RVA: 0x000219DF File Offset: 0x0001FBDF
	public IScope Scope
	{
		get
		{
			return this._scope;
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x000219E8 File Offset: 0x0001FBE8
	public bool InitializeFromSimulation(ISimulation simulation, GameJournalMotive motive)
	{
		this._header = this._scope.Get<MotorwaysGameJournalHeader>();
		if (!this._header.Initialize(simulation, motive))
		{
			BaseGameJournalSave.Log.Warn("Unable to create header from simulation.", Array.Empty<object>());
			return false;
		}
		MemoryStream simulationDataStream = new MemoryStream();
		using (BinaryWriter writer = new BinaryWriter(simulationDataStream))
		{
			if (!simulation.Scope.Export(simulation, writer))
			{
				BaseGameJournalSave.Log.Warn("Failed to export simulation.", Array.Empty<object>());
				return false;
			}
		}
		this._simulationData = simulationDataStream.ToArray();
		base.UtcTimestamp = DateTime.UtcNow;
		return true;
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x00021A98 File Offset: 0x0001FC98
	public override void InitializeWithBytes(byte[] saveDataAsBytes)
	{
		base.InitializeWithBytes(saveDataAsBytes);
		this._simulationData = saveDataAsBytes;
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00021AA8 File Offset: 0x0001FCA8
	public override byte[] GetBytesForSerializing()
	{
		return this._simulationData;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00021AB0 File Offset: 0x0001FCB0
	public override void OnSerializeBeforeData(BinaryWriter binaryWriter)
	{
		base.OnSerializeBeforeData(binaryWriter);
		this._scope.Export(this._header, binaryWriter);
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00021ACC File Offset: 0x0001FCCC
	public override IBinarySerializableSaveData.HeaderValidationResult ValidateHeader(BinaryReader binaryReader)
	{
		if (base.ValidateHeader(binaryReader) != IBinarySerializableSaveData.HeaderValidationResult.Success)
		{
			return IBinarySerializableSaveData.HeaderValidationResult.InvalidHeader;
		}
		this._header = this._scope.Import<MotorwaysGameJournalHeader>(binaryReader);
		if (this._header == null)
		{
			return IBinarySerializableSaveData.HeaderValidationResult.InvalidHeader;
		}
		base.UtcTimestamp = this._header.UtcTimestamp;
		Assembler gameAssembler = this._scope.Assembler.GetAssemblerForType(typeof(Game));
		if (this._header.GameAssemblerSerializerHashCode != gameAssembler.GlobalTypeSerializerHashCode)
		{
			BaseGameJournalSave.Log.Info("Rejecting save due to mismatched serializer hash codes. Theirs is {0}, ours is {1}.", new object[]
			{
				this._header.GameAssemblerSerializerHashCode,
				gameAssembler.GlobalTypeSerializerHashCode
			});
			return IBinarySerializableSaveData.HeaderValidationResult.HashCodesMismatched;
		}
		if (this._header == null)
		{
			return IBinarySerializableSaveData.HeaderValidationResult.InvalidHeader;
		}
		return IBinarySerializableSaveData.HeaderValidationResult.Success;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00021B88 File Offset: 0x0001FD88
	public Game DeserializeGame(CityDefinition cityDefinition)
	{
		if (this._simulationData != null)
		{
			Game game = this._scope.Get<Game>();
			game.Scope.Get<City>().Definition = cityDefinition;
			MotorwaysGame motorwaysGame = game as MotorwaysGame;
			if (motorwaysGame != null)
			{
				motorwaysGame.PausePathfinder();
			}
			using (BinaryReader simulationReader = new BinaryReader(new MemoryStream(this._simulationData)))
			{
				if (game.Scope.Import<ISimulation>(simulationReader) == null)
				{
					this._scope.Release(game);
					return null;
				}
			}
			if (motorwaysGame != null)
			{
				motorwaysGame.ResumePathfinder();
			}
			return game;
		}
		return null;
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06000A31 RID: 2609 RVA: 0x00021C24 File Offset: 0x0001FE24
	public GameJournalMotive Motive
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.Motive;
			}
			return GameJournalMotive.Autosave;
		}
	}

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06000A32 RID: 2610 RVA: 0x00021C43 File Offset: 0x0001FE43
	public string DeviceModel
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.DeviceModel;
			}
			return "";
		}
	}

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06000A33 RID: 2611 RVA: 0x00021C66 File Offset: 0x0001FE66
	public string DeviceName
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.DeviceName;
			}
			return "";
		}
	}

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06000A34 RID: 2612 RVA: 0x00021C89 File Offset: 0x0001FE89
	public int GameAssemblerSerializerHashCode
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.GameAssemblerSerializerHashCode;
			}
			return -1;
		}
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06000A35 RID: 2613 RVA: 0x00021CA8 File Offset: 0x0001FEA8
	public string CityId
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.CityId;
			}
			return "";
		}
	}

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06000A36 RID: 2614 RVA: 0x00021CCB File Offset: 0x0001FECB
	public GameMode Mode
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.Mode;
			}
			return GameMode.Normal;
		}
	}

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06000A37 RID: 2615 RVA: 0x00021CEA File Offset: 0x0001FEEA
	public int TripCount
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.TripCount;
			}
			return 0;
		}
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06000A38 RID: 2616 RVA: 0x00021D09 File Offset: 0x0001FF09
	public Fix64 TimeElapsed
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.TimeElapsed;
			}
			return Fix64.Zero;
		}
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x00021D2C File Offset: 0x0001FF2C
	public MapChallenge.ChallengeType ChallengeType
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.ChallengeType;
			}
			return MapChallenge.ChallengeType.None;
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06000A3A RID: 2618 RVA: 0x00021D4B File Offset: 0x0001FF4B
	public int ChallengeEndTime
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.ChallengeEndTime;
			}
			return 0;
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06000A3B RID: 2619 RVA: 0x00021D6A File Offset: 0x0001FF6A
	public int ChallengeIndex
	{
		get
		{
			if (Diagnostics.Verify(this._header != null))
			{
				return this._header.ChallengeIndex;
			}
			return -1;
		}
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00021D89 File Offset: 0x0001FF89
	public void OnReleasedFromScope(IScope scope)
	{
		if (this._header != null)
		{
			scope.Release(this._header);
		}
	}

	// Token: 0x04000567 RID: 1383
	[Dependency]
	private IScope _scope;

	// Token: 0x04000568 RID: 1384
	private MotorwaysGameJournalHeader _header;

	// Token: 0x04000569 RID: 1385
	private byte[] _simulationData;
}
