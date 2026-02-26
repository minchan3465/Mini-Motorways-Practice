using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Factory;
using Factory.Allocators;
using Motorways;
using Motorways.Views;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class AppRuntime : MonoBehaviour
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060000DF RID: 223 RVA: 0x00004602 File Offset: 0x00002802
	public IApp App
	{
		get
		{
			AppContainer container = this._container;
			if (container == null)
			{
				return null;
			}
			return container.App;
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00004615 File Offset: 0x00002815
	private void Awake()
	{
		Diagnostics.IsTrackingExceptions = true;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x0000461D File Offset: 0x0000281D
	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			IApp app = this.App;
			if (app == null)
			{
				return;
			}
			app.GameOpenedNotificationSetup();
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00004632 File Offset: 0x00002832
	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			IApp app = this.App;
			if (app == null)
			{
				return;
			}
			app.Scope.Get<ScreenStack>().OnApplicationPaused();
		}
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00004654 File Offset: 0x00002854
	private void SetupInputOverride()
	{
		this._inputOverride = AppContainer.Environment.AddInputOverrideToGameObject(base.gameObject);
		this._inputModule = base.gameObject.AddComponent<InputModule>();
		this._inputModule.inputOverride = this._inputOverride;
		this._inputModule.horizontalAxis = "";
		this._inputModule.verticalAxis = "";
		this._inputModule.submitButton = "";
		this._inputModule.cancelButton = "";
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x000046DC File Offset: 0x000028DC
	private void Start()
	{
		FeatureToggle.RemoveAllSources();
		if (Application.isEditor)
		{
			FeatureToggle.AddSource(new EditorPrefsConfigSettingSource());
		}
		if (this._container == null)
		{
			this._container = new MotorwaysAppContainer();
		}
		this._container.CreateAssemblers();
		if (FeatureToggle.IsFeatureEnabled(Feature.OptionsDebugMenu))
		{
			FeatureToggle.AddSource(new OptionsMenuSettingSource());
		}
		if (FeatureToggle.IsFeatureEnabled(Feature.RecordLogs))
		{
			Diagnostics.Log.IsRecordingLog = true;
		}
		this.SetupInputOverride();
		bool isPlayingBackAppJournal = !string.IsNullOrEmpty(this._playbackAppJournalPath);
		if (isPlayingBackAppJournal)
		{
			this._container.AppAssembler.Register<IAppCommandSource, JournalAppCommandSource>().Allocator(new HeapAllocator<JournalAppCommandSource>()).Binding(Binding.Scope);
		}
		this._container.CreateScope();
		if (isPlayingBackAppJournal)
		{
			using (BinaryReader journalReader = new BinaryReader(File.Open(this._playbackAppJournalPath, FileMode.Open)))
			{
				this._container.AppScope.Import(journalReader);
				goto IL_141;
			}
		}
		if (Diagnostics.File.CanWrite)
		{
			DateTime now = DateTime.Now;
			this._recordingAppJournalPath = Diagnostics.File.GetFullPath(string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}.appjournal", new object[]
			{
				now.Year,
				now.Month,
				now.Day,
				now.Hour,
				now.Minute
			}));
		}
		IL_141:
		IApp app = this._container.CreateApp();
		this._inputOverride.InputState = app.InputState;
		bool recordJournal = false;
		if (FeatureToggle.IsFeatureEnabled(Feature.RecordAppJournal))
		{
			recordJournal = !isPlayingBackAppJournal;
		}
		if (FeatureToggle.IsFeatureEnabled(Feature.ElevateErrorsForCloudDiagnostics))
		{
			AppRuntime.Log.Error("Cloud diagnostics enabled.", Array.Empty<object>());
		}
		this._container.Start(recordJournal);
		IApp app2 = this.App;
		this._deepLinkProcessor = ((app2 != null) ? app2.Scope.Get<DeepLinkProcessor>() : null);
		IApp app3 = this.App;
		this._screenStack = ((app3 != null) ? app3.Scope.Get<ScreenStack>() : null);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x000048CC File Offset: 0x00002ACC
	private void Update()
	{
		this._container.Tick();
		this.CheckForDeepLinkChallenge();
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060000E6 RID: 230 RVA: 0x000048DF File Offset: 0x00002ADF
	public bool CanExportAppJournal
	{
		get
		{
			return this._container.CommandJournal != null && !string.IsNullOrEmpty(this._recordingAppJournalPath);
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00004900 File Offset: 0x00002B00
	public void ExportAppJournal()
	{
		using (BinaryWriter journalWriter = new BinaryWriter(File.Open(this._recordingAppJournalPath, FileMode.Create)))
		{
			this._container.AppScope.Export(this._container.CommandJournal, journalWriter);
		}
		AppRuntime.Log.Info("Exported journal to {0}.", new object[]
		{
			this._recordingAppJournalPath
		});
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00004978 File Offset: 0x00002B78
	private void CheckForDeepLinkChallenge()
	{
		if (this._deepLinkProcessor == null || this._screenStack == null)
		{
			return;
		}
		if (this._screenStack.ExitingToMainMenu)
		{
			return;
		}
		if (this._deepLinkProcessor.hasChallengeToUse && this._screenStack.HasVisibleScreens() && this._screenStack.IsScreenInStack<MainMenuScreen>())
		{
			this.HandleDeeplinkRequest(this._screenStack, this._deepLinkProcessor);
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x000049E0 File Offset: 0x00002BE0
	private Task HandleDeeplinkRequest(ScreenStack screenStack, DeepLinkProcessor deepLinkProcessor)
	{
		AppRuntime.<HandleDeeplinkRequest>d__21 <HandleDeeplinkRequest>d__;
		<HandleDeeplinkRequest>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<HandleDeeplinkRequest>d__.screenStack = screenStack;
		<HandleDeeplinkRequest>d__.deepLinkProcessor = deepLinkProcessor;
		<HandleDeeplinkRequest>d__.<>1__state = -1;
		<HandleDeeplinkRequest>d__.<>t__builder.Start<AppRuntime.<HandleDeeplinkRequest>d__21>(ref <HandleDeeplinkRequest>d__);
		return <HandleDeeplinkRequest>d__.<>t__builder.Task;
	}

	// Token: 0x04000050 RID: 80
	[HideInInspector]
	public string _playbackAppJournalPath;

	// Token: 0x04000051 RID: 81
	[HideInInspector]
	public string _playbackSimJournalPath;

	// Token: 0x04000052 RID: 82
	[HideInInspector]
	public string _recordingAppJournalPath;

	// Token: 0x04000053 RID: 83
	private BaseInputOverride _inputOverride;

	// Token: 0x04000054 RID: 84
	private InputModule _inputModule;

	// Token: 0x04000055 RID: 85
	private AppContainer _container;

	// Token: 0x04000056 RID: 86
	private DeepLinkProcessor _deepLinkProcessor;

	// Token: 0x04000057 RID: 87
	private ScreenStack _screenStack;

	// Token: 0x04000058 RID: 88
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AppRuntime");
}
