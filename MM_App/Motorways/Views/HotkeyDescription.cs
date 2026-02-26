using System;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005A1 RID: 1441
	public readonly struct HotkeyDescription
	{
		// Token: 0x0600283B RID: 10299 RVA: 0x000ABA0B File Offset: 0x000A9C0B
		public HotkeyDescription(KeyCode keyCode, KeyCode modifierKeyCode, string description)
		{
			this.keyCode = keyCode;
			this.modifierKeyCode = modifierKeyCode;
			this.description = description;
		}

		// Token: 0x0600283C RID: 10300 RVA: 0x000ABA22 File Offset: 0x000A9C22
		public HotkeyDescription(KeyCode keyCode, string description)
		{
			this.keyCode = keyCode;
			this.modifierKeyCode = KeyCode.None;
			this.description = description;
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x0600283D RID: 10301 RVA: 0x000ABA39 File Offset: 0x000A9C39
		public string KeyCodeDisplayName
		{
			get
			{
				return HotkeyDescription.GetHotkeyCharacter(this.modifierKeyCode) + HotkeyDescription.GetHotkeyCharacter(this.keyCode);
			}
		}

		// Token: 0x0600283E RID: 10302 RVA: 0x000ABA58 File Offset: 0x000A9C58
		public static string GetHotkeyCharacter(KeyCode keyCode)
		{
			if (keyCode <= KeyCode.Slash)
			{
				if (keyCode <= KeyCode.Escape)
				{
					if (keyCode == KeyCode.None)
					{
						return "";
					}
					if (keyCode == KeyCode.Escape)
					{
						return "Esc";
					}
				}
				else
				{
					if (keyCode == KeyCode.Quote)
					{
						return "'";
					}
					switch (keyCode)
					{
					case KeyCode.Comma:
						return ",";
					case KeyCode.Period:
						return ".";
					case KeyCode.Slash:
						return "/";
					}
				}
			}
			else if (keyCode <= KeyCode.Equals)
			{
				if (keyCode == KeyCode.Semicolon)
				{
					return ";";
				}
				if (keyCode == KeyCode.Equals)
				{
					return "=";
				}
			}
			else
			{
				if (keyCode == KeyCode.Backslash)
				{
					return "\\";
				}
				if (keyCode - KeyCode.RightShift <= 1)
				{
					return "⇧";
				}
				if (keyCode - KeyCode.RightControl <= 1)
				{
					return "^";
				}
			}
			return keyCode.ToString();
		}

		// Token: 0x04002200 RID: 8704
		public readonly KeyCode keyCode;

		// Token: 0x04002201 RID: 8705
		public readonly KeyCode modifierKeyCode;

		// Token: 0x04002202 RID: 8706
		public readonly string description;
	}
}
