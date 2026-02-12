using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200015B RID: 347
public class BaseInputOverride : BaseInput
{
	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000793 RID: 1939 RVA: 0x00018BFF File Offset: 0x00016DFF
	// (set) Token: 0x06000794 RID: 1940 RVA: 0x00018C07 File Offset: 0x00016E07
	public IInputState InputState { get; set; }

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000795 RID: 1941 RVA: 0x00018C10 File Offset: 0x00016E10
	public override string compositionString
	{
		get
		{
			return Input.compositionString;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000796 RID: 1942 RVA: 0x00018C17 File Offset: 0x00016E17
	// (set) Token: 0x06000797 RID: 1943 RVA: 0x00018C1E File Offset: 0x00016E1E
	public override IMECompositionMode imeCompositionMode
	{
		get
		{
			return Input.imeCompositionMode;
		}
		set
		{
			Input.imeCompositionMode = value;
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000798 RID: 1944 RVA: 0x00018C26 File Offset: 0x00016E26
	// (set) Token: 0x06000799 RID: 1945 RVA: 0x00018C2D File Offset: 0x00016E2D
	public override Vector2 compositionCursorPos
	{
		get
		{
			return Input.compositionCursorPos;
		}
		set
		{
			Input.compositionCursorPos = value;
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x0600079A RID: 1946 RVA: 0x00018C35 File Offset: 0x00016E35
	public override bool mousePresent
	{
		get
		{
			return this.InputState.MousePresent;
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x00018C44 File Offset: 0x00016E44
	public override bool GetMouseButtonDown(int buttonIndex)
	{
		int rewiredInputAction = BaseInputOverride.GetRewiredActionForMouseButtonIndex(buttonIndex);
		return rewiredInputAction >= 0 && this.InputState.Mouse.GetButtonState(rewiredInputAction).CurrentState == InputEventButtonState.JustDown;
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00018C78 File Offset: 0x00016E78
	public override bool GetMouseButtonUp(int buttonIndex)
	{
		int rewiredInputAction = BaseInputOverride.GetRewiredActionForMouseButtonIndex(buttonIndex);
		return rewiredInputAction >= 0 && this.InputState.Mouse.GetButtonState(rewiredInputAction).CurrentState == InputEventButtonState.JustUp;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00018CAC File Offset: 0x00016EAC
	public override bool GetMouseButton(int buttonIndex)
	{
		int rewiredInputAction = BaseInputOverride.GetRewiredActionForMouseButtonIndex(buttonIndex);
		return rewiredInputAction >= 0 && this.InputState.Mouse.GetButtonState(rewiredInputAction).IsDown;
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00018CDC File Offset: 0x00016EDC
	public static int GetRewiredActionForMouseButtonIndex(int buttonIndex)
	{
		switch (buttonIndex)
		{
		case 0:
			return 19;
		case 1:
			return 20;
		case 2:
			return 30;
		default:
			return -1;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x0600079F RID: 1951 RVA: 0x00018CFC File Offset: 0x00016EFC
	public override Vector2 mousePosition
	{
		get
		{
			if (Diagnostics.Verify(this.InputState != null, "InputState is null!") && this.InputState.MousePresent)
			{
				return this.InputState.Mouse.Position;
			}
			return Vector2.zero;
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060007A0 RID: 1952 RVA: 0x00018D36 File Offset: 0x00016F36
	public override Vector2 mouseScrollDelta
	{
		get
		{
			return Input.mouseScrollDelta;
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060007A1 RID: 1953 RVA: 0x00018D3D File Offset: 0x00016F3D
	public override bool touchSupported
	{
		get
		{
			return Input.touchSupported;
		}
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060007A2 RID: 1954 RVA: 0x00018D44 File Offset: 0x00016F44
	public override int touchCount
	{
		get
		{
			return this.InputState.TouchCount;
		}
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x00018D54 File Offset: 0x00016F54
	public override Touch GetTouch(int index)
	{
		IPointerState pointer;
		if (this.InputState.TryGetTouch(index, out pointer))
		{
			return pointer.ToUnityTouch();
		}
		return new Touch
		{
			phase = TouchPhase.Canceled
		};
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00018D8C File Offset: 0x00016F8C
	public override float GetAxisRaw(string axisName)
	{
		if (string.IsNullOrEmpty(axisName))
		{
			return 0f;
		}
		int axisInt = this.GetRewiredInputActionForAxisName(axisName);
		if (axisInt >= 0)
		{
			return this.InputState.GetAxis(axisInt);
		}
		return 0f;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00018DC5 File Offset: 0x00016FC5
	private int GetRewiredInputActionForAxisName(string axisName)
	{
		if (axisName == "MoveHorizontal")
		{
			return 0;
		}
		if (!(axisName == "MoveVertical"))
		{
			Diagnostics.FailAssert("Failed to find an axis for '" + axisName + "'", Array.Empty<object>());
			return -1;
		}
		return 1;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0000222C File Offset: 0x0000042C
	public override bool GetButtonDown(string buttonName)
	{
		return false;
	}
}
