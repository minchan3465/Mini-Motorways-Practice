using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x02000744 RID: 1860
	public class ToggleButtonGroup : MonoBehaviour
	{
		// Token: 0x060033F8 RID: 13304 RVA: 0x000F5774 File Offset: 0x000F3974
		public void ClearToggles()
		{
			this._toggles.Clear();
		}

		// Token: 0x060033F9 RID: 13305 RVA: 0x000F5781 File Offset: 0x000F3981
		public void RegisterToggle(TouchToggle toggle)
		{
			toggle.Group = this;
			this._toggles.Add(toggle);
		}

		// Token: 0x060033FA RID: 13306 RVA: 0x000F5798 File Offset: 0x000F3998
		public void NotifyToggleOn(TouchToggle toggle)
		{
			foreach (TouchToggle otherToggle in this._toggles)
			{
				if (otherToggle != toggle)
				{
					otherToggle.IsOn = false;
				}
			}
		}

		// Token: 0x060033FB RID: 13307 RVA: 0x000F57F4 File Offset: 0x000F39F4
		public bool AnyTogglesOn()
		{
			using (List<TouchToggle>.Enumerator enumerator = this._toggles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsOn)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060033FC RID: 13308 RVA: 0x000F5850 File Offset: 0x000F3A50
		public void EnsureValidState()
		{
			if (Diagnostics.Verify(this._toggles.Count > 0, "There is no toggles in the {0} group!", base.name))
			{
				bool foundOnToggle = false;
				foreach (TouchToggle toggle in this._toggles)
				{
					if (foundOnToggle)
					{
						toggle.IsOn = false;
					}
					else
					{
						foundOnToggle = toggle.IsOn;
					}
				}
				if (!foundOnToggle && !this.allowSwitchOff)
				{
					this._toggles[0].IsOn = true;
				}
			}
		}

		// Token: 0x04002C64 RID: 11364
		[SerializeField]
		private List<TouchToggle> _toggles = new List<TouchToggle>();

		// Token: 0x04002C65 RID: 11365
		public bool allowSwitchOff;
	}
}
