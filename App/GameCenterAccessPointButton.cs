using System;
using Factory;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CC RID: 460
public class GameCenterAccessPointButton : MonoBehaviour
{
	// Token: 0x06000AD4 RID: 2772 RVA: 0x00023FCA File Offset: 0x000221CA
	protected void Awake()
	{
		this._camera = Camera.main;
		this._rectTransform = base.GetComponent<RectTransform>();
		this._parentRectTransform = base.transform.parent.GetComponent<RectTransform>();
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00023FF9 File Offset: 0x000221F9
	public void Initialise(IScope scope)
	{
		this._gameCenterAccessPoint = scope.Get<IGameCenterAccessPoint>();
		this.RefreshButtonState();
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0002400D File Offset: 0x0002220D
	public void Show()
	{
		if (this._gameCenterAccessPoint.IsAvailable())
		{
			this._gameCenterAccessPoint.Show();
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00024027 File Offset: 0x00022227
	public void Hide()
	{
		if (this._gameCenterAccessPoint.IsAvailable())
		{
			this._gameCenterAccessPoint.Hide();
		}
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00024041 File Offset: 0x00022241
	protected void Update()
	{
		this.RefreshButtonState();
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0002404C File Offset: 0x0002224C
	private void RefreshButtonState()
	{
		if (!this._gameCenterAccessPoint.IsAvailable())
		{
			this._touchButton.gameObject.SetActive(false);
			return;
		}
		Rect accessPointRect = this.GetAccessPointRect();
		if (accessPointRect.size == Vector2.zero)
		{
			this._touchButton.gameObject.SetActive(false);
			return;
		}
		this._touchButton.gameObject.SetActive(true);
		this.ResizeButtonTo(accessPointRect);
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x000240BC File Offset: 0x000222BC
	private Rect GetAccessPointRect()
	{
		Rect accessPointRect = this._gameCenterAccessPoint.GetRect();
		if (accessPointRect.size == Vector2.zero)
		{
			return accessPointRect;
		}
		accessPointRect = new Rect(accessPointRect.x, (float)Screen.height - accessPointRect.yMin - accessPointRect.height, accessPointRect.width, accessPointRect.height);
		return accessPointRect;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00024120 File Offset: 0x00022320
	private void ResizeButtonTo(Rect rect)
	{
		float minDimension = Mathf.Min(rect.width, rect.height);
		Vector2 clampedSize = new Vector2(minDimension, minDimension);
		rect.min = rect.center - clampedSize * 0.5f;
		rect.size = clampedSize;
		Camera canvasCamera = this._camera;
		if (this._parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			canvasCamera = null;
		}
		Vector3 worldBottomLeft;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(this._parentRectTransform, new Vector2(rect.xMin, rect.yMin), canvasCamera, out worldBottomLeft);
		Vector3 worldTopRight;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(this._parentRectTransform, new Vector2(rect.xMax, rect.yMax), canvasCamera, out worldTopRight);
		Vector2 size = (worldTopRight - worldBottomLeft) / this._rectTransform.lossyScale;
		this._rectTransform.position = worldBottomLeft;
		this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00024220 File Offset: 0x00022420
	public void OnAccessPointClick()
	{
		if (Diagnostics.Verify(this._gameCenterAccessPoint != null, this, "No valid AccessPoint. Has this object been initialised?"))
		{
			this._gameCenterAccessPoint.Select();
		}
	}

	// Token: 0x040005ED RID: 1517
	private IGameCenterAccessPoint _gameCenterAccessPoint;

	// Token: 0x040005EE RID: 1518
	private Camera _camera;

	// Token: 0x040005EF RID: 1519
	private RectTransform _rectTransform;

	// Token: 0x040005F0 RID: 1520
	private RectTransform _parentRectTransform;

	// Token: 0x040005F1 RID: 1521
	[SerializeField]
	private Canvas _parentCanvas;

	// Token: 0x040005F2 RID: 1522
	[SerializeField]
	private TouchButton _touchButton;
}
