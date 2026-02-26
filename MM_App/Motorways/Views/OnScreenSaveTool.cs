using System;
using System.Collections.Generic;
using System.IO;
using DevTools.OnScreenDebugTools;
using Factory;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005AC RID: 1452
	public class OnScreenSaveTool : IOnScreenTool
	{
		// Token: 0x06002879 RID: 10361 RVA: 0x000AC700 File Offset: 0x000AA900
		public OnScreenSaveTool(IScope scope)
		{
			this._scope = scope;
			this._activePlayer = scope.Get<IActivePlayer>();
			this._debugStorage = scope.Get<OnScreenDebugStorage>();
			this._storableTypeHandlerRegistry = scope.Get<StorableTypeHandlerRegistry>();
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x0600287A RID: 10362 RVA: 0x000AC7CB File Offset: 0x000AA9CB
		public Rect InputBlockingRect
		{
			get
			{
				return this._windowRect;
			}
		}

		// Token: 0x0600287B RID: 10363 RVA: 0x000AC7D4 File Offset: 0x000AA9D4
		public void OnGUI(IScope scope)
		{
			if (this._reportIdLabelStyle == null)
			{
				this._reportIdLabelStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 30
				};
			}
			if (this._numberPadButtonStyle == null)
			{
				this._numberPadButtonStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 30
				};
			}
			if (this._sectionHeaderStyle == null)
			{
				this._sectionHeaderStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 40,
					alignment = TextAnchor.MiddleCenter
				};
			}
			if (this._downloadStatusLabelStyle == null)
			{
				this._downloadStatusLabelStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 30,
					wordWrap = true,
					alignment = TextAnchor.MiddleCenter
				};
			}
			if (this._listButtonStyle == null)
			{
				this._listButtonStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 30,
					padding = new RectOffset(5, 5, 5, 5)
				};
			}
			if (this._disabledListButtonStyle == null)
			{
				this._disabledListButtonStyle = new GUIStyle(this._listButtonStyle);
			}
			this._disabledListButtonStyle.normal.textColor = Color.gray;
			if (this._listLabelStyle == null)
			{
				this._listLabelStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 30,
					margin = new RectOffset(0, 0, 5, 0)
				};
			}
			if (this._actionButtonStyle == null)
			{
				this._actionButtonStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 30,
					padding = new RectOffset(0, 0, 10, 10)
				};
			}
			if (this._toggleStyle == null)
			{
				this._toggleStyle = new GUIStyle(GUI.skin.toggle)
				{
					fontSize = 25
				};
			}
			if (this._savedJournalInfo == null)
			{
				this._savedJournalInfo = this.LoadSavedJournalList();
			}
			this._windowRect = GUI.Window(0, this._windowRect, new GUI.WindowFunction(this.DrawReportDownloadWindow), "Save Tool");
		}

		// Token: 0x0600287C RID: 10364 RVA: 0x000AC9B4 File Offset: 0x000AABB4
		private void DrawReportDownloadWindow(int windowId)
		{
			Rect contentRect = new Rect(60f, 18f, 360f, 684f);
			switch (this._currentView)
			{
			case OnScreenSaveTool.View.SavedGameList:
				this.DrawSaveGameListView(contentRect);
				break;
			case OnScreenSaveTool.View.DownloadReport:
				this.DrawDownloadReportView(contentRect);
				break;
			case OnScreenSaveTool.View.DownloadingReport:
				this.DrawDownloadingReportView(contentRect);
				break;
			}
			GUI.DragWindow(new Rect(0f, 0f, 360f, 684f));
		}

		// Token: 0x0600287D RID: 10365 RVA: 0x000ACA30 File Offset: 0x000AAC30
		private void DrawSaveGameListView(Rect contentRect)
		{
			GUILayout.BeginArea(contentRect);
			GUILayout.Space(0.03f * contentRect.height);
			if (GUILayout.Button("Load Remote Save", this._actionButtonStyle, Array.Empty<GUILayoutOption>()))
			{
				this._currentView = OnScreenSaveTool.View.DownloadReport;
				return;
			}
			GUILayout.Space(0.02f * contentRect.height);
			GUILayout.Label("Downloaded Saves", this._sectionHeaderStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(0.01f * contentRect.height);
			if (this._savedJournalInfo == null || this._savedJournalInfo.Count < 0)
			{
				GUILayout.Label("No saves downloaded.", Array.Empty<GUILayoutOption>());
			}
			else
			{
				this._startGamesPaused = GUILayout.Toggle(this._startGamesPaused, " Start Paused?", this._toggleStyle, Array.Empty<GUILayoutOption>());
				GUILayout.Space(0.03f * contentRect.height);
				foreach (OnScreenSaveTool.SavedJournalInfo savedJournalInfo in this._savedJournalInfo)
				{
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label(savedJournalInfo.name, this._listLabelStyle, Array.Empty<GUILayoutOption>());
					if (GUILayout.Button("Load", this._listButtonStyle, Array.Empty<GUILayoutOption>()) && this._gameStarter == null)
					{
						MotorwaysGameJournalSave newSavedGame = this.LoadJournalSave(savedJournalInfo.filepath) as MotorwaysGameJournalSave;
						if (newSavedGame != null)
						{
							SaveToolUtilities.StartGame(newSavedGame, this._startGamesPaused, this._scope, ref this._gameStarter);
						}
					}
					if (savedJournalInfo.pendingDeleteConfirmation)
					{
						if (GUILayout.Button(" Sure?", this._listButtonStyle, Array.Empty<GUILayoutOption>()))
						{
							this._debugStorage.Delete(savedJournalInfo.filepath);
							this.RefreshSavedJournalList();
						}
					}
					else if (GUILayout.Button("Delete", this._listButtonStyle, Array.Empty<GUILayoutOption>()))
					{
						savedJournalInfo.pendingDeleteConfirmation = true;
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(0.05f * contentRect.height);
				}
			}
			GUILayout.EndArea();
		}

		// Token: 0x0600287E RID: 10366 RVA: 0x000ACC28 File Offset: 0x000AAE28
		private void DrawDownloadReportView(Rect contentRect)
		{
			GUI.BeginGroup(contentRect);
			GUIContent reportIdTextContent = new GUIContent(this._reportIdInput);
			Vector2 reportIdContentSize = this._reportIdLabelStyle.CalcSize(reportIdTextContent);
			float reportIdAreaHeight = 0.1f * contentRect.height;
			GUI.BeginGroup(new Rect(0f, 0f, contentRect.width, reportIdAreaHeight));
			GUI.Label(new Rect(0.5f * (contentRect.width - reportIdContentSize.x), 0.5f * (reportIdAreaHeight - reportIdContentSize.y), reportIdContentSize.x, reportIdContentSize.y), reportIdTextContent, this._reportIdLabelStyle);
			GUI.EndGroup();
			GUILayout.BeginArea(new Rect(0f, reportIdAreaHeight, contentRect.width, 0.9f * contentRect.height));
			this._selectedGridButtonIndex = GUILayout.SelectionGrid(this._selectedGridButtonIndex, this._numberPadButtons, 3, this._numberPadButtonStyle, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.MaxWidth(contentRect.width)
			});
			if (this._selectedGridButtonIndex != -1)
			{
				string button = this._numberPadButtons[this._selectedGridButtonIndex];
				int buttonInteger;
				if (this._reportIdInput.Length < 10 && int.TryParse(button, out buttonInteger))
				{
					this._reportIdInput += buttonInteger.ToString();
				}
				else if (button == "C")
				{
					this._reportIdInput = "";
				}
				else if (button == "<" && this._reportIdInput.Length > 0)
				{
					this._reportIdInput = this._reportIdInput.Remove(this._reportIdInput.Length - 1);
				}
				this._selectedGridButtonIndex = -1;
			}
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			int reportId;
			if (GUILayout.Button("Download", this._actionButtonStyle, Array.Empty<GUILayoutOption>()) && this._reportIdInput.Length > 0 && int.TryParse(this._reportIdInput, out reportId))
			{
				this._isDownloadingReport = true;
				this._remoteReport = Diagnostics.Report.Download(reportId);
				this._currentView = OnScreenSaveTool.View.DownloadingReport;
			}
			if (GUILayout.Button("Back", this._actionButtonStyle, Array.Empty<GUILayoutOption>()))
			{
				this._remoteReport = null;
				this._currentView = OnScreenSaveTool.View.SavedGameList;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
			GUI.EndGroup();
		}

		// Token: 0x0600287F RID: 10367 RVA: 0x000ACE58 File Offset: 0x000AB058
		private void DrawDownloadingReportView(Rect contentRect)
		{
			GUI.BeginGroup(contentRect);
			string text = this._remoteReport.State.ToString();
			float contentHeight = 0.5f * contentRect.height;
			float backButtonHeight = 0.2f * contentRect.height;
			GUIContent statusTextContent = new GUIContent(text);
			float statusTextContentHeight = this._downloadStatusLabelStyle.CalcHeight(statusTextContent, contentRect.width);
			GUILayout.BeginArea(new Rect(0f, 0.5f * (contentRect.height - (statusTextContentHeight + backButtonHeight)), contentRect.width, contentHeight));
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.Label(statusTextContent, this._downloadStatusLabelStyle, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Back", this._actionButtonStyle, Array.Empty<GUILayoutOption>()))
			{
				this._remoteReport = null;
				this._currentView = OnScreenSaveTool.View.SavedGameList;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
			GUI.EndGroup();
		}

		// Token: 0x06002880 RID: 10368 RVA: 0x000ACF34 File Offset: 0x000AB134
		public void Update()
		{
			if (this._gameStarter != null && this._gameStarter.CanStart)
			{
				ScreenStack stack = this._scope.Get<ScreenStack>();
				if (stack.GetTopActiveScreenType() == ScreenStack.MotorwaysScreen.MainMenu)
				{
					this._gameStarter.Start(stack, this._scope);
					this._gameStarter = null;
				}
			}
			if (!this._isDownloadingReport || this._remoteReport == null)
			{
				return;
			}
			if (this._remoteReport.State == Diagnostics.ReportState.Searching || this._remoteReport.State == Diagnostics.ReportState.Downloading)
			{
				return;
			}
			foreach (Diagnostics.ReportAttachment attachment in this._remoteReport.Attachments)
			{
				byte[] journalBytes;
				if (!(attachment.Filename != "simulation.gamejournal") && OnScreenDebugStorage.LoadBytesFromFile(attachment.LocalFilepath, out journalBytes))
				{
					SavedGameStorableTypeHandler handler = this._storableTypeHandlerRegistry.GetHandlerForType(typeof(MotorwaysGameJournalSave)) as SavedGameStorableTypeHandler;
					if (handler != null)
					{
						IStorable storable = handler.Load(journalBytes);
						if (storable != null)
						{
							IGameJournalSave journalSave = storable as IGameJournalSave;
							if (journalSave != null)
							{
								this._debugStorage.Store(this._remoteReport.Id.ToString() + ".gamejournal", journalBytes);
								this.RefreshSavedJournalList();
								this._activePlayer.AddForeignSavedGame(journalSave);
							}
						}
					}
				}
			}
			this._isDownloadingReport = false;
		}

		// Token: 0x06002881 RID: 10369 RVA: 0x000AD09C File Offset: 0x000AB29C
		public void Reset()
		{
			this._reportIdInput = "";
			this._windowRect = OnScreenSaveTool.DefaultWindowRect;
		}

		// Token: 0x06002882 RID: 10370 RVA: 0x000AD0B4 File Offset: 0x000AB2B4
		private void RefreshSavedJournalList()
		{
			this._savedJournalInfo = this.LoadSavedJournalList();
		}

		// Token: 0x06002883 RID: 10371 RVA: 0x000AD0C4 File Offset: 0x000AB2C4
		private IGameJournalSave LoadJournalSave(string filepath)
		{
			IStorableTypeHandler handler = this._storableTypeHandlerRegistry.GetHandlerForType(typeof(MotorwaysGameJournalSave));
			byte[] journalBytes;
			if (OnScreenDebugStorage.LoadBytesFromFile(filepath, out journalBytes))
			{
				return handler.Load(journalBytes) as IGameJournalSave;
			}
			return null;
		}

		// Token: 0x06002884 RID: 10372 RVA: 0x000AD100 File Offset: 0x000AB300
		private IReadOnlyList<OnScreenSaveTool.SavedJournalInfo> LoadSavedJournalList()
		{
			string[] savedFilePaths = this._debugStorage.LoadAll();
			if (savedFilePaths == null)
			{
				return new List<OnScreenSaveTool.SavedJournalInfo>().AsReadOnly();
			}
			List<OnScreenSaveTool.SavedJournalInfo> savedJournalInfo = new List<OnScreenSaveTool.SavedJournalInfo>(savedFilePaths.Length);
			foreach (string savedFile in savedFilePaths)
			{
				if (savedFile.EndsWith(".gamejournal"))
				{
					string fileName = Path.GetFileName(savedFile);
					string saveName = fileName.Substring(0, fileName.Length - ".gamejournal".Length);
					savedJournalInfo.Add(new OnScreenSaveTool.SavedJournalInfo(saveName, savedFile));
				}
			}
			return savedJournalInfo;
		}

		// Token: 0x04002227 RID: 8743
		private readonly IScope _scope;

		// Token: 0x04002228 RID: 8744
		private readonly IActivePlayer _activePlayer;

		// Token: 0x04002229 RID: 8745
		private readonly OnScreenDebugStorage _debugStorage;

		// Token: 0x0400222A RID: 8746
		private readonly StorableTypeHandlerRegistry _storableTypeHandlerRegistry;

		// Token: 0x0400222B RID: 8747
		private OnScreenSaveTool.View _currentView;

		// Token: 0x0400222C RID: 8748
		private GUIStyle _sectionHeaderStyle;

		// Token: 0x0400222D RID: 8749
		private GUIStyle _listButtonStyle;

		// Token: 0x0400222E RID: 8750
		private GUIStyle _disabledListButtonStyle;

		// Token: 0x0400222F RID: 8751
		private GUIStyle _listLabelStyle;

		// Token: 0x04002230 RID: 8752
		private GUIStyle _toggleStyle;

		// Token: 0x04002231 RID: 8753
		private GUIStyle _reportIdLabelStyle;

		// Token: 0x04002232 RID: 8754
		private GUIStyle _numberPadButtonStyle;

		// Token: 0x04002233 RID: 8755
		private GUIStyle _actionButtonStyle;

		// Token: 0x04002234 RID: 8756
		private GUIStyle _downloadStatusLabelStyle;

		// Token: 0x04002235 RID: 8757
		private static readonly Vector2Int BaseResolution = new Vector2Int(1920, 1080);

		// Token: 0x04002236 RID: 8758
		private const int BaseWindowWidth = 480;

		// Token: 0x04002237 RID: 8759
		private const int BaseWindowHeight = 720;

		// Token: 0x04002238 RID: 8760
		private static readonly Rect DefaultWindowRect = new Rect((float)(OnScreenSaveTool.BaseResolution.x - 480), 0.5f * (float)(OnScreenSaveTool.BaseResolution.y - 720), 480f, 720f);

		// Token: 0x04002239 RID: 8761
		private Rect _windowRect = OnScreenSaveTool.DefaultWindowRect;

		// Token: 0x0400223A RID: 8762
		private IReadOnlyList<OnScreenSaveTool.SavedJournalInfo> _savedJournalInfo;

		// Token: 0x0400223B RID: 8763
		private GameStarter _gameStarter;

		// Token: 0x0400223C RID: 8764
		private bool _startGamesPaused;

		// Token: 0x0400223D RID: 8765
		private const int MaxReportIdDigits = 10;

		// Token: 0x0400223E RID: 8766
		private const string DefaultReportId = "";

		// Token: 0x0400223F RID: 8767
		private string _reportIdInput = "";

		// Token: 0x04002240 RID: 8768
		private bool _isDownloadingReport;

		// Token: 0x04002241 RID: 8769
		private Diagnostics.Report _remoteReport;

		// Token: 0x04002242 RID: 8770
		private const string ClearButton = "C";

		// Token: 0x04002243 RID: 8771
		private const string BackspaceButton = "<";

		// Token: 0x04002244 RID: 8772
		private readonly string[] _numberPadButtons = new string[]
		{
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"C",
			"0",
			"<"
		};

		// Token: 0x04002245 RID: 8773
		private const int NoButtonSelected = -1;

		// Token: 0x04002246 RID: 8774
		private int _selectedGridButtonIndex = -1;

		// Token: 0x04002247 RID: 8775
		private const string GameJournalFileExtension = ".gamejournal";

		// Token: 0x020005AD RID: 1453
		private enum View
		{
			// Token: 0x04002249 RID: 8777
			SavedGameList,
			// Token: 0x0400224A RID: 8778
			DownloadReport,
			// Token: 0x0400224B RID: 8779
			DownloadingReport
		}

		// Token: 0x020005AE RID: 1454
		private class SavedJournalInfo
		{
			// Token: 0x06002886 RID: 10374 RVA: 0x000AD1E5 File Offset: 0x000AB3E5
			public SavedJournalInfo(string name, string filepath)
			{
				this.name = name;
				this.filepath = filepath;
			}

			// Token: 0x0400224C RID: 8780
			public readonly string name;

			// Token: 0x0400224D RID: 8781
			public readonly string filepath;

			// Token: 0x0400224E RID: 8782
			public bool pendingDeleteConfirmation;
		}
	}
}
