using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C7 RID: 455
public class DelegateScrollRect : ScrollRect
{
	// Token: 0x06000AB2 RID: 2738 RVA: 0x000237AA File Offset: 0x000219AA
	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		if (this._onBeginDrag != null)
		{
			this._onBeginDrag.Invoke(base.normalizedPosition);
		}
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x000237CC File Offset: 0x000219CC
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		if (this._onDrag != null)
		{
			this._onDrag.Invoke(base.normalizedPosition);
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000237EE File Offset: 0x000219EE
	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		if (this._onEndDrag != null)
		{
			this._onEndDrag.Invoke(base.normalizedPosition);
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00023810 File Offset: 0x00021A10
	public override void OnScroll(PointerEventData data)
	{
		base.OnScroll(data);
		if (this._onScroll != null)
		{
			this._onScroll.Invoke(data.scrollDelta);
		}
	}

	// Token: 0x040005C7 RID: 1479
	[SerializeField]
	private ScrollRect.ScrollRectEvent _onBeginDrag = new ScrollRect.ScrollRectEvent();

	// Token: 0x040005C8 RID: 1480
	[SerializeField]
	private ScrollRect.ScrollRectEvent _onDrag = new ScrollRect.ScrollRectEvent();

	// Token: 0x040005C9 RID: 1481
	[SerializeField]
	private ScrollRect.ScrollRectEvent _onEndDrag = new ScrollRect.ScrollRectEvent();

	// Token: 0x040005CA RID: 1482
	[SerializeField]
	private ScrollRect.ScrollRectEvent _onScroll = new ScrollRect.ScrollRectEvent();
}
