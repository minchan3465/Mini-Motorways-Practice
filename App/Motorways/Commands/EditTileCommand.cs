using System;
using Factory;
using Motorways.Models;
using Server;

namespace Motorways.Commands
{
	// Token: 0x0200051D RID: 1309
	public class EditTileCommand : Command, IReleasedFromScopeHandler
	{
		// Token: 0x060022A2 RID: 8866 RVA: 0x0008C170 File Offset: 0x0008A370
		public override void Execute(ISimulation simulation)
		{
			Command.Log.Info("Executing EditTileCommand with {0}.", new object[]
			{
				this._edit
			});
			if (Diagnostics.Verify(this._upgradeDatabase.ApplyEdit(this._edit, this._tilemap), "Failed to apply edit {0} to the upgrade database.", this._edit))
			{
				Diagnostics.Verify(this._edit.ApplyToTilemap(this._tilemap), "Failed to apply edit {0} to the tilemap.", this._edit);
			}
			this._edit.ApplyToSimulation(simulation);
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x0008C1F2 File Offset: 0x0008A3F2
		public static EditTileCommand Create(IScope scope, TileEdit edit)
		{
			EditTileCommand editTileCommand = scope.Get<EditTileCommand>();
			editTileCommand._edit = edit;
			return editTileCommand;
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x0008C201 File Offset: 0x0008A401
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._edit != null)
			{
				scope.Release(this._edit);
				this._edit = null;
			}
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x0008C21F File Offset: 0x0008A41F
		public override string ToString()
		{
			return string.Format("[EditTileCommand Edit={0}]", this._edit);
		}

		// Token: 0x04001CBA RID: 7354
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001CBB RID: 7355
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabase;

		// Token: 0x04001CBC RID: 7356
		private TileEdit _edit;
	}
}
