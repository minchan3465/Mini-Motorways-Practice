using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Factory;
using JetBrains.Annotations;
using Screens;

namespace Motorways.Views
{
	// Token: 0x020005B3 RID: 1459
	public static class SaveToolUtilities
	{
		// Token: 0x06002896 RID: 10390 RVA: 0x000AD4AC File Offset: 0x000AB6AC
		public static void BookmarkSavedGame(string savedGameName, MotorwaysGameJournalSave savedGame)
		{
			savedGameName = SaveToolUtilities.MakeValidFileName(savedGameName);
			string savedGamePath = Diagnostics.File.GetFullPath(savedGameName + ".gamejournal");
			using (FileStream saveStream = new FileStream(savedGamePath, FileMode.OpenOrCreate, FileAccess.Write))
			{
				using (BinaryWriter saveWriter = new BinaryWriter(saveStream))
				{
					savedGame.OnSerializeBeforeData(saveWriter);
					saveWriter.Write(savedGame.GetBytesForSerializing());
					SaveToolUtilities.BookmarkedSavedGames.Add(new ArchivedSavedGame(savedGamePath, savedGame));
					SaveToolUtilities.AddBookmark(savedGameName);
					SaveToolUtilities.SortBookmarkedSavedGames();
				}
			}
		}

		// Token: 0x06002897 RID: 10391 RVA: 0x000AD544 File Offset: 0x000AB744
		public static void DeleteArchivedSavedGame(ArchivedSavedGame archivedSavedGame)
		{
			if (SaveToolUtilities.BookmarkedSavedGames.Remove(archivedSavedGame) && !string.IsNullOrEmpty(archivedSavedGame.Name))
			{
				SaveToolUtilities.RemoveBookmark(archivedSavedGame.Name);
			}
			SaveToolUtilities.AutomaticSavedGames.Remove(archivedSavedGame);
			archivedSavedGame.Release();
			archivedSavedGame.Delete();
		}

		// Token: 0x06002898 RID: 10392 RVA: 0x000AD583 File Offset: 0x000AB783
		public static void DeleteAllArchivedSavedGames()
		{
			SaveToolUtilities.DeleteAllAutomaticSavedGames();
			SaveToolUtilities.DeleteAllBookmarkedSavedGames();
		}

		// Token: 0x06002899 RID: 10393 RVA: 0x000AD590 File Offset: 0x000AB790
		public static void DeleteAllAutomaticSavedGames()
		{
			foreach (ArchivedSavedGame archivedSavedGame in SaveToolUtilities.AutomaticSavedGames)
			{
				archivedSavedGame.Release();
				archivedSavedGame.Delete();
			}
			SaveToolUtilities.AutomaticSavedGames.Clear();
		}

		// Token: 0x0600289A RID: 10394 RVA: 0x000AD5F0 File Offset: 0x000AB7F0
		private static void DeleteAllBookmarkedSavedGames()
		{
			foreach (ArchivedSavedGame archivedSavedGame in SaveToolUtilities.BookmarkedSavedGames)
			{
				archivedSavedGame.Release();
				archivedSavedGame.Delete();
			}
			SaveToolUtilities.BookmarkedSavedGames.Clear();
			SaveToolUtilities.RemoveAllBookmarks();
		}

		// Token: 0x0600289B RID: 10395 RVA: 0x000AD654 File Offset: 0x000AB854
		public static void LoadSavedGameLibrary(IScope appScope)
		{
			SaveToolUtilities.BookmarkedSavedGames.ForEach(delegate(ArchivedSavedGame savedGame)
			{
				savedGame.Release();
			});
			SaveToolUtilities.BookmarkedSavedGames.Clear();
			SaveToolUtilities.AutomaticSavedGames.ForEach(delegate(ArchivedSavedGame savedGame)
			{
				savedGame.Release();
			});
			SaveToolUtilities.AutomaticSavedGames.Clear();
			HashSet<string> unloadedBookmarkedSavedGameNames = new HashSet<string>(SaveToolUtilities.LoadBookmarks());
			if (Directory.Exists(Diagnostics.File.Path))
			{
				foreach (string gameJournalPath in Directory.EnumerateFiles(Diagnostics.File.Path))
				{
					if (!gameJournalPath.EndsWith(".DS_Store", StringComparison.InvariantCultureIgnoreCase))
					{
						ArchivedSavedGame savedGame2 = ArchivedSavedGame.Load(gameJournalPath, appScope);
						if (savedGame2 != null)
						{
							if (unloadedBookmarkedSavedGameNames.Contains(savedGame2.Name))
							{
								unloadedBookmarkedSavedGameNames.Remove(gameJournalPath);
								SaveToolUtilities.BookmarkedSavedGames.Add(savedGame2);
							}
							else
							{
								SaveToolUtilities.AutomaticSavedGames.Add(savedGame2);
							}
						}
					}
				}
			}
			SaveToolUtilities.SortBookmarkedSavedGames();
			SaveToolUtilities.SortAutomaticSavedGames();
		}

		// Token: 0x0600289C RID: 10396 RVA: 0x000AD76C File Offset: 0x000AB96C
		public static void StartGame(MotorwaysGameJournalSave save, bool startGamePaused, IScope scope, ref GameStarter gameStarter)
		{
			ScreenStack stack = scope.Get<ScreenStack>();
			GameContainerScreen gameContainerScreen = stack.GetActiveScreen<GameContainerScreen>();
			if (gameContainerScreen != null && gameContainerScreen.GetActiveGame() != null)
			{
				gameContainerScreen.GetActiveGame().OnGameEnd(GameEndReason.Exit);
			}
			BaseScalingScreen baseScalingScreen = stack.GetTopVisibleScreen() as BaseScalingScreen;
			if (baseScalingScreen != null)
			{
				baseScalingScreen.SkipNextTransition();
			}
			if (!stack.IsScreenActive<MainMenuScreen>())
			{
				stack.ReplaceScreens<MainMenuScreen>(ScreenStack.MotorwaysScreen.MainMenu, typeof(GameContainerScreen), null, true);
			}
			else if (stack.GetTopActiveScreenType() != ScreenStack.MotorwaysScreen.MainMenu)
			{
				stack.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
			}
			MainMenuScreen mainMenuScreen = stack.GetActiveScreen<MainMenuScreen>();
			if (gameStarter == null)
			{
				gameStarter = new GameStarter(mainMenuScreen);
			}
			MapDatabase mapDatabase = scope.Get<MapDatabase>();
			gameStarter.StartFromSavedGame(mapDatabase.MapLibrary, save, false, true, startGamePaused);
		}

		// Token: 0x0600289D RID: 10397 RVA: 0x000AD81C File Offset: 0x000ABA1C
		private static string MakeValidFileName(string name)
		{
			string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format("([{0}]*\\.+$)|([{0}]+)|(\\,)", invalidChars);
			return Regex.Replace(name, invalidRegStr, "_");
		}

		// Token: 0x0600289E RID: 10398 RVA: 0x000AD851 File Offset: 0x000ABA51
		private static void SortBookmarkedSavedGames()
		{
			SaveToolUtilities.BookmarkedSavedGames.Sort((ArchivedSavedGame a, ArchivedSavedGame b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
		}

		// Token: 0x0600289F RID: 10399 RVA: 0x000AD87C File Offset: 0x000ABA7C
		private static void SortAutomaticSavedGames()
		{
			SaveToolUtilities.AutomaticSavedGames.Sort(delegate(ArchivedSavedGame a, ArchivedSavedGame b)
			{
				int sortResult = string.Compare(a.SavedGame.CityId, b.SavedGame.CityId, StringComparison.Ordinal);
				if (sortResult != 0)
				{
					return sortResult;
				}
				sortResult = a.SavedGame.TimeElapsed.CompareTo(b.SavedGame.TimeElapsed);
				if (sortResult != 0)
				{
					return sortResult;
				}
				return a.SavedGame.TripCount.CompareTo(b.SavedGame.TripCount);
			});
		}

		// Token: 0x060028A0 RID: 10400 RVA: 0x000022F5 File Offset: 0x000004F5
		private static void AddBookmark(string name)
		{
		}

		// Token: 0x060028A1 RID: 10401 RVA: 0x000022F5 File Offset: 0x000004F5
		private static void RemoveBookmark(string name)
		{
		}

		// Token: 0x060028A2 RID: 10402 RVA: 0x000AD8A7 File Offset: 0x000ABAA7
		[NotNull]
		private static string[] LoadBookmarks()
		{
			return new string[0];
		}

		// Token: 0x060028A3 RID: 10403 RVA: 0x000022F5 File Offset: 0x000004F5
		private static void RemoveAllBookmarks()
		{
		}

		// Token: 0x04002256 RID: 8790
		public static readonly List<ArchivedSavedGame> BookmarkedSavedGames = new List<ArchivedSavedGame>();

		// Token: 0x04002257 RID: 8791
		public static readonly List<ArchivedSavedGame> AutomaticSavedGames = new List<ArchivedSavedGame>();

		// Token: 0x04002258 RID: 8792
		private const string BookmarkedSaveStringDelimiter = ",";

		// Token: 0x04002259 RID: 8793
		private const string BookmarkedGamesEditorPrefsId = "SaveGameTool-BookmarkedSaveGames";
	}
}
