using System;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x0200044D RID: 1101
	[RequireComponent(typeof(RectTransform))]
	public class RoadCursor : MonoBehaviour
	{
		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06001B60 RID: 7008 RVA: 0x00064222 File Offset: 0x00062422
		// (set) Token: 0x06001B61 RID: 7009 RVA: 0x0006422F File Offset: 0x0006242F
		public bool IsVisible
		{
			get
			{
				return this._sprite.enabled;
			}
			set
			{
				this._sprite.enabled = value;
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06001B62 RID: 7010 RVA: 0x0006423D File Offset: 0x0006243D
		// (set) Token: 0x06001B63 RID: 7011 RVA: 0x0006424A File Offset: 0x0006244A
		public Vector2 Position
		{
			get
			{
				return this._rectTransform.anchoredPosition;
			}
			set
			{
				this._rectTransform.anchoredPosition = value;
			}
		}

		// Token: 0x06001B64 RID: 7012 RVA: 0x00064258 File Offset: 0x00062458
		private void Awake()
		{
			this._sprite = base.GetComponent<Image>();
			this._rectTransform = base.GetComponent<RectTransform>();
			this.IsVisible = false;
		}

		// Token: 0x040016E0 RID: 5856
		private RectTransform _rectTransform;

		// Token: 0x040016E1 RID: 5857
		private Image _sprite;
	}
}
