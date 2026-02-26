using System;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x0200072F RID: 1839
	[RequireComponent(typeof(DelegateCanvasGroup))]
	public class MapButtonCard : MonoBehaviour
	{
		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x060032FA RID: 13050 RVA: 0x000F1F13 File Offset: 0x000F0113
		// (set) Token: 0x060032FB RID: 13051 RVA: 0x000F1F20 File Offset: 0x000F0120
		public float Alpha
		{
			get
			{
				return this._delegateCanvasGroup.Alpha;
			}
			set
			{
				this._delegateCanvasGroup.Alpha = value;
			}
		}

		// Token: 0x060032FC RID: 13052 RVA: 0x000F1F2E File Offset: 0x000F012E
		private void Awake()
		{
			this._delegateCanvasGroup = base.GetComponent<DelegateCanvasGroup>();
		}

		// Token: 0x060032FD RID: 13053 RVA: 0x000F1F3C File Offset: 0x000F013C
		public virtual void SetVisible(bool isVisible)
		{
			base.gameObject.SetActive(isVisible);
			this.SetSelected(isVisible);
		}

		// Token: 0x060032FE RID: 13054 RVA: 0x000F1F51 File Offset: 0x000F0151
		public virtual void SetSelected(bool isSelected)
		{
			this._delegateCanvasGroup.SetInteractable(isSelected);
			this._delegateCanvasGroup.SetBlocksRaycasts(isSelected);
		}

		// Token: 0x060032FF RID: 13055 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnMapButtonSelected(bool isMapButtonSelected)
		{
		}

		// Token: 0x04002B96 RID: 11158
		private DelegateCanvasGroup _delegateCanvasGroup;
	}
}
