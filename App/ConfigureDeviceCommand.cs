using System;
using Factory;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class ConfigureDeviceCommand : AppCommand
{
	// Token: 0x06000126 RID: 294 RVA: 0x00004CA0 File Offset: 0x00002EA0
	public void Initialize()
	{
		this._platform = this._hardwareCapabilities.Platform;
		this._screenWidth = Screen.width;
		this._screenHeight = Screen.height;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00004CC9 File Offset: 0x00002EC9
	public override void Reset()
	{
		this._platform = RuntimePlatform.OSXEditor;
		this._screenWidth = 0;
		this._screenHeight = 0;
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00004CE0 File Offset: 0x00002EE0
	public override bool Execute(IApp receiver)
	{
		RuntimePlatform platform = this._hardwareCapabilities.Platform;
		return true;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00004CEF File Offset: 0x00002EEF
	private static bool IsPlatformStandalone(RuntimePlatform platform)
	{
		return platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.LinuxPlayer;
	}

	// Token: 0x04000066 RID: 102
	private RuntimePlatform _platform;

	// Token: 0x04000067 RID: 103
	private int _screenWidth;

	// Token: 0x04000068 RID: 104
	private int _screenHeight;

	// Token: 0x04000069 RID: 105
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;
}
