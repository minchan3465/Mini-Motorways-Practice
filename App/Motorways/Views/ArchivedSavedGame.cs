using System;
using System.IO;
using System.Linq;
using Factory;
using JetBrains.Annotations;

namespace Motorways.Views
{
	// Token: 0x020005B5 RID: 1461
	public class ArchivedSavedGame
	{
		// Token: 0x060028AB RID: 10411 RVA: 0x000AD964 File Offset: 0x000ABB64
		public ArchivedSavedGame(string path, MotorwaysGameJournalSave savedGame)
		{
			this._path = path;
			this.SavedGame = savedGame;
			string filename = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.None).Last<string>();
			this.Name = (filename.Contains('.') ? filename.Substring(0, filename.LastIndexOf('.')) : "");
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x060028AC RID: 10412 RVA: 0x000AD9BD File Offset: 0x000ABBBD
		public string Name { get; }

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x060028AD RID: 10413 RVA: 0x000AD9C5 File Offset: 0x000ABBC5
		// (set) Token: 0x060028AE RID: 10414 RVA: 0x000AD9CD File Offset: 0x000ABBCD
		public MotorwaysGameJournalSave SavedGame { get; private set; }

		// Token: 0x060028AF RID: 10415 RVA: 0x000AD9D6 File Offset: 0x000ABBD6
		public void Release()
		{
			if (this.SavedGame != null)
			{
				this.SavedGame.Scope.Release(this.SavedGame);
				this.SavedGame = null;
			}
		}

		// Token: 0x060028B0 RID: 10416 RVA: 0x000AD9FE File Offset: 0x000ABBFE
		public void Delete()
		{
			File.Delete(this._path);
		}

		// Token: 0x060028B1 RID: 10417 RVA: 0x000ADA0C File Offset: 0x000ABC0C
		[CanBeNull]
		public static ArchivedSavedGame Load(string path, IScope scope)
		{
			MotorwaysGameJournalSave gameJournalSave = scope.Get<MotorwaysGameJournalSave>();
			byte[] rawData;
			try
			{
				rawData = File.ReadAllBytes(path);
			}
			catch (Exception)
			{
				scope.Release(gameJournalSave);
				return null;
			}
			ArchivedSavedGame result;
			using (MemoryStream memoryStream = new MemoryStream(rawData))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					if (gameJournalSave.ValidateHeader(binaryReader) == IBinarySerializableSaveData.HeaderValidationResult.Success)
					{
						byte[] saveDataBytes = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
						gameJournalSave.InitializeWithBytes(saveDataBytes);
						result = new ArchivedSavedGame(path, gameJournalSave);
					}
					else
					{
						scope.Release(gameJournalSave);
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x04002261 RID: 8801
		private readonly string _path;
	}
}
