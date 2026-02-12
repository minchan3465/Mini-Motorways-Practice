using System;
using UnityEngine;

// Token: 0x020001D9 RID: 473
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
	// Token: 0x06000B4A RID: 2890 RVA: 0x000267D0 File Offset: 0x000249D0
	private void OnEnable()
	{
		this._panel = base.GetComponent<RectTransform>();
		if (this._panel == null)
		{
			ScreenStack.Log.Error("Cannot apply safe area - no RectTransform found on " + base.name, Array.Empty<object>());
			UnityEngine.Object.Destroy(base.gameObject);
		}
		this.Refresh();
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00026827 File Offset: 0x00024A27
	public void Update()
	{
		this.Refresh();
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00026830 File Offset: 0x00024A30
	private void Refresh()
	{
		Rect safeArea = this.GetSafeArea();
		if (safeArea != this._lastSafeArea)
		{
			this.ApplySafeArea(safeArea);
		}
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00026859 File Offset: 0x00024A59
	private Rect GetSafeArea()
	{
		return Screen.safeArea;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00026860 File Offset: 0x00024A60
	private void ApplySafeArea(Rect r)
	{
		this._lastSafeArea = r;
		if (!this._conformX)
		{
			r.x = 0f;
			r.width = (float)Screen.width;
		}
		if (!this._conformY)
		{
			r.y = 0f;
			r.height = (float)Screen.height;
		}
		Vector2 anchorMin = r.position;
		Vector2 anchorMax = r.position + r.size;
		anchorMin.x /= (float)Screen.width;
		anchorMin.y /= (float)Screen.height;
		anchorMax.x /= (float)Screen.width;
		anchorMax.y /= (float)Screen.height;
		this._panel.anchorMin = anchorMin;
		this._panel.anchorMax = anchorMax;
		ScreenStack.Log.Info("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}", new object[]
		{
			base.name,
			r.x,
			r.y,
			r.width,
			r.height,
			Screen.width,
			Screen.height
		});
	}

	// Token: 0x04000672 RID: 1650
	private RectTransform _panel;

	// Token: 0x04000673 RID: 1651
	private Rect _lastSafeArea = new Rect(0f, 0f, 0f, 0f);

	// Token: 0x04000674 RID: 1652
	[SerializeField]
	private bool _conformX = true;

	// Token: 0x04000675 RID: 1653
	[SerializeField]
	private bool _conformY = true;
}
