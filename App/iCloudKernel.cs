using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class iCloudKernel
{
	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000532 RID: 1330 RVA: 0x0001251C File Offset: 0x0001071C
	// (remove) Token: 0x06000533 RID: 1331 RVA: 0x00012554 File Offset: 0x00010754
	private event Action<string> _userChanged;

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06000534 RID: 1332 RVA: 0x0001258C File Offset: 0x0001078C
	// (remove) Token: 0x06000535 RID: 1333 RVA: 0x000125C4 File Offset: 0x000107C4
	private event Action _filesChanged;

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000536 RID: 1334 RVA: 0x000125FC File Offset: 0x000107FC
	// (remove) Token: 0x06000537 RID: 1335 RVA: 0x00012634 File Offset: 0x00010834
	private event Action<bool> _loadCompleted;

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000538 RID: 1336 RVA: 0x0001266C File Offset: 0x0001086C
	// (remove) Token: 0x06000539 RID: 1337 RVA: 0x000126A4 File Offset: 0x000108A4
	private event Action<string> _fileDeleted;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x0600053A RID: 1338 RVA: 0x000126DC File Offset: 0x000108DC
	// (remove) Token: 0x0600053B RID: 1339 RVA: 0x00012714 File Offset: 0x00010914
	private event Action<string> _userMessageChanged;

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x0600053C RID: 1340 RVA: 0x00012749 File Offset: 0x00010949
	public static iCloudKernel Instance
	{
		get
		{
			if (iCloudKernel._instance == null)
			{
				iCloudKernel._instance = new iCloudKernel();
			}
			return iCloudKernel._instance;
		}
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x00012764 File Offset: 0x00010964
	public void Connect()
	{
		if (this._hasConnected)
		{
			return;
		}
		this._hasConnected = true;
		IntPtr onLogMessageDelegate = IntPtr.Zero;
		onLogMessageDelegate = Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudKernel.OnLogMessageDelegate));
		iCloudKernel.iCloudAttemptLogin(Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudKernel.OnUserChangedDelegate)), Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudKernel.OnFileWriteCompletedDelegate)), Marshal.GetFunctionPointerForDelegate<Action>(new Action(iCloudKernel.OnFilesChangedDelegate)), Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudKernel.OnFileDeletedDelegate)), Marshal.GetFunctionPointerForDelegate<Action<bool>>(new Action<bool>(iCloudKernel.OnLoadCompletedDelegate)), onLogMessageDelegate, Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudKernel.OnUserMessageDelegate)), "iCloud.com.dinopoloclub.minimotorways");
		this._connectionStartTime = Time.realtimeSinceStartup;
		iCloudKernel.appDelegateSetNotificationCallback(iCloudKernel.iCloudGetFunctionPointerToNotificationCallback());
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x0600053E RID: 1342 RVA: 0x0001281F File Offset: 0x00010A1F
	public float TimeSinceConnection
	{
		get
		{
			return Time.realtimeSinceStartup - this._connectionStartTime;
		}
	}

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x0600053F RID: 1343 RVA: 0x00012830 File Offset: 0x00010A30
	// (remove) Token: 0x06000540 RID: 1344 RVA: 0x00012880 File Offset: 0x00010A80
	public event Action<string> UserChanged
	{
		add
		{
			if (this._hasUserChanged)
			{
				value(this._userId);
			}
			lock (this)
			{
				this._userChanged += value;
			}
		}
		remove
		{
			lock (this)
			{
				this._userChanged -= value;
			}
		}
	}

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000541 RID: 1345 RVA: 0x000128BC File Offset: 0x00010ABC
	// (remove) Token: 0x06000542 RID: 1346 RVA: 0x00012908 File Offset: 0x00010B08
	public event Action FilesChanged
	{
		add
		{
			if (this._haveFilesChanged)
			{
				value();
			}
			lock (this)
			{
				this._filesChanged += value;
			}
		}
		remove
		{
			lock (this)
			{
				this._filesChanged -= value;
			}
		}
	}

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000543 RID: 1347 RVA: 0x00012944 File Offset: 0x00010B44
	// (remove) Token: 0x06000544 RID: 1348 RVA: 0x00012994 File Offset: 0x00010B94
	public event Action<bool> LoadCompleted
	{
		add
		{
			if (this._hasLoadCompleted)
			{
				value(this._wasLoadSuccessful);
			}
			lock (this)
			{
				this._loadCompleted += value;
			}
		}
		remove
		{
			lock (this)
			{
				this._loadCompleted -= value;
			}
		}
	}

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000545 RID: 1349 RVA: 0x000129D0 File Offset: 0x00010BD0
	// (remove) Token: 0x06000546 RID: 1350 RVA: 0x00012A50 File Offset: 0x00010C50
	public event Action<string> FileDeleted
	{
		add
		{
			foreach (string deletedFile in this._deletedFiles)
			{
				value(deletedFile);
			}
			lock (this)
			{
				this._fileDeleted += value;
			}
		}
		remove
		{
			lock (this)
			{
				this._fileDeleted -= value;
			}
		}
	}

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000547 RID: 1351 RVA: 0x00012A8C File Offset: 0x00010C8C
	// (remove) Token: 0x06000548 RID: 1352 RVA: 0x00012AE4 File Offset: 0x00010CE4
	public event Action<string> UserMessageChanged
	{
		add
		{
			if (!string.IsNullOrEmpty(this._messageStringKey))
			{
				value(this._messageStringKey);
			}
			lock (this)
			{
				this._userMessageChanged += value;
			}
		}
		remove
		{
			lock (this)
			{
				this._userMessageChanged -= value;
			}
		}
	}

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000549 RID: 1353 RVA: 0x00012B20 File Offset: 0x00010D20
	// (remove) Token: 0x0600054A RID: 1354 RVA: 0x00012B58 File Offset: 0x00010D58
	public event Action<string> FileStored;

	// Token: 0x0600054B RID: 1355 RVA: 0x00012B90 File Offset: 0x00010D90
	private void OnUserChanged(string newUserId)
	{
		if (this._userId == newUserId && this._hasUserChanged)
		{
			return;
		}
		this._hasUserChanged = true;
		this._userId = newUserId;
		if (string.IsNullOrEmpty(this._userId))
		{
			iCloudKernel.Log.Info("iCloud user disconnected.", Array.Empty<object>());
		}
		else
		{
			iCloudKernel.Log.Info("iCloud user connected with id {0}.", new object[]
			{
				this._userId
			});
		}
		Action<string> userChanged = this._userChanged;
		if (userChanged == null)
		{
			return;
		}
		userChanged(this._userId);
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00012C19 File Offset: 0x00010E19
	private void OnFilesChanged()
	{
		iCloudKernel.Log.Info("Data changed, processing new files.", Array.Empty<object>());
		this._haveFilesChanged = true;
		Action filesChanged = this._filesChanged;
		if (filesChanged == null)
		{
			return;
		}
		filesChanged();
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00012C48 File Offset: 0x00010E48
	private void OnLoadCompleted(bool didSucceed)
	{
		if (didSucceed)
		{
			iCloudKernel.Log.Info("Load completed with no errors.", Array.Empty<object>());
		}
		else
		{
			iCloudKernel.Log.Info("Load completed with errors.", Array.Empty<object>());
		}
		this._hasLoadCompleted = true;
		this._wasLoadSuccessful = didSucceed;
		Action<bool> loadCompleted = this._loadCompleted;
		if (loadCompleted == null)
		{
			return;
		}
		loadCompleted(this._wasLoadSuccessful);
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00012CA6 File Offset: 0x00010EA6
	private void OnFileDeleted(string deletedFilename)
	{
		iCloudKernel.Log.Info("File {0} has been deleted from the database.", new object[]
		{
			deletedFilename
		});
		this._deletedFiles.Add(deletedFilename);
		Action<string> fileDeleted = this._fileDeleted;
		if (fileDeleted == null)
		{
			return;
		}
		fileDeleted(deletedFilename);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x00012CDE File Offset: 0x00010EDE
	private void OnUserMessage(string messageStringKey)
	{
		iCloudKernel.Log.Info("Received message {0}.", new object[]
		{
			messageStringKey
		});
		this._messageStringKey = messageStringKey;
		Action<string> userMessageChanged = this._userMessageChanged;
		if (userMessageChanged == null)
		{
			return;
		}
		userMessageChanged(messageStringKey);
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x00012D11 File Offset: 0x00010F11
	private void OnFileStored(string filename)
	{
		iCloudKernel.Log.Info("File {0} was stored successfully.", new object[]
		{
			filename
		});
		Action<string> fileStored = this.FileStored;
		if (fileStored == null)
		{
			return;
		}
		fileStored(filename);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x00012D3D File Offset: 0x00010F3D
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnUserChangedDelegate(string userId)
	{
		iCloudKernel instance = iCloudKernel.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnUserChanged(userId);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00012D4F File Offset: 0x00010F4F
	[MonoPInvokeCallback(typeof(Action))]
	private static void OnFilesChangedDelegate()
	{
		iCloudKernel instance = iCloudKernel.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnFilesChanged();
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x00012D60 File Offset: 0x00010F60
	[MonoPInvokeCallback(typeof(Action<bool>))]
	private static void OnLoadCompletedDelegate(bool didSucceed)
	{
		iCloudKernel instance = iCloudKernel.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnLoadCompleted(didSucceed);
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x00012D72 File Offset: 0x00010F72
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnFileDeletedDelegate(string deletedFilename)
	{
		iCloudKernel instance = iCloudKernel.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnFileDeleted(deletedFilename);
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00012D84 File Offset: 0x00010F84
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnUserMessageDelegate(string messageStringKey)
	{
		iCloudKernel instance = iCloudKernel.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnUserMessage(messageStringKey);
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00012D96 File Offset: 0x00010F96
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnLogMessageDelegate(string logMessage)
	{
		iCloudKernel.Log.Info(logMessage, Array.Empty<object>());
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x00012DA8 File Offset: 0x00010FA8
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnFileWriteCompletedDelegate(string filename)
	{
		iCloudKernel instance = iCloudKernel.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnFileStored(filename);
	}

	// Token: 0x06000558 RID: 1368
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern void iCloudAttemptLogin(IntPtr userChangedCallback, IntPtr fileWriteCompletedCallback, IntPtr filesChangedCallback, IntPtr fileDeletedCallback, IntPtr loadCompletedCallback, IntPtr logCallback, IntPtr errorCallback, string containerId);

	// Token: 0x06000559 RID: 1369
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern long iCloudGetFunctionPointerToNotificationCallback();

	// Token: 0x0600055A RID: 1370 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void appDelegateSetNotificationCallback(long functionAddress)
	{
	}

	// Token: 0x0400022E RID: 558
	private bool _hasConnected;

	// Token: 0x0400022F RID: 559
	private float _connectionStartTime;

	// Token: 0x04000230 RID: 560
	private bool _haveFilesChanged;

	// Token: 0x04000231 RID: 561
	private bool _hasLoadCompleted;

	// Token: 0x04000232 RID: 562
	private bool _wasLoadSuccessful;

	// Token: 0x04000233 RID: 563
	private string _userId;

	// Token: 0x04000234 RID: 564
	private bool _hasUserChanged;

	// Token: 0x04000235 RID: 565
	private string _messageStringKey;

	// Token: 0x04000236 RID: 566
	private readonly List<string> _deletedFiles = new List<string>();

	// Token: 0x0400023C RID: 572
	private static iCloudKernel _instance = null;

	// Token: 0x0400023D RID: 573
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("iCloudKernel");
}
