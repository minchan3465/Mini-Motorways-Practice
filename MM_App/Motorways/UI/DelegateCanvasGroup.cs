using System;
using UnityEngine;
using UnityEngine.Events;

namespace Motorways.UI
{
	// Token: 0x02000721 RID: 1825
	[RequireComponent(typeof(CanvasGroup))]
	public class DelegateCanvasGroup : MonoBehaviour
	{
		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x0600323B RID: 12859 RVA: 0x000ED525 File Offset: 0x000EB725
		public CanvasGroup CanvasGroup
		{
			get
			{
				if (this._canvasGroup == null)
				{
					this._canvasGroup = base.GetComponent<CanvasGroup>();
				}
				return this._canvasGroup;
			}
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x0600323C RID: 12860 RVA: 0x000ED547 File Offset: 0x000EB747
		// (set) Token: 0x0600323D RID: 12861 RVA: 0x000ED554 File Offset: 0x000EB754
		public float Alpha
		{
			get
			{
				return this.CanvasGroup.alpha;
			}
			set
			{
				this.CanvasGroup.alpha = value;
			}
		}

		// Token: 0x0600323E RID: 12862 RVA: 0x000ED562 File Offset: 0x000EB762
		public void SetInteractable(bool isInteractable)
		{
			this.CanvasGroup.interactable = isInteractable;
			if (this._onInteractableToggled != null)
			{
				this._onInteractableToggled.Invoke(isInteractable);
			}
		}

		// Token: 0x0600323F RID: 12863 RVA: 0x000ED584 File Offset: 0x000EB784
		public void SetBlocksRaycasts(bool doesBlockRaycasts)
		{
			this.CanvasGroup.blocksRaycasts = doesBlockRaycasts;
			if (this._onInteractableToggled != null)
			{
				this._onBlocksRaycastsToggled.Invoke(doesBlockRaycasts);
			}
		}

		// Token: 0x04002B13 RID: 11027
		private CanvasGroup _canvasGroup;

		// Token: 0x04002B14 RID: 11028
		[SerializeField]
		private DelegateCanvasGroup.FieldToggledEvent _onInteractableToggled = new DelegateCanvasGroup.FieldToggledEvent();

		// Token: 0x04002B15 RID: 11029
		[SerializeField]
		private DelegateCanvasGroup.FieldToggledEvent _onBlocksRaycastsToggled = new DelegateCanvasGroup.FieldToggledEvent();

		// Token: 0x02000722 RID: 1826
		[Serializable]
		public class FieldToggledEvent : UnityEvent<bool>
		{
		}
	}
}
