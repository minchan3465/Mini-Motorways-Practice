using System;
using Factory;
using FixMath;
using Motorways.Audio;
using Rendering.RenderFeatures;
using UnityEngine;

// Token: 0x0200025D RID: 605
[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour, IReleasedFromScopeHandler
{
	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0003088D File Offset: 0x0002EA8D
	public Camera DefaultCamera
	{
		get
		{
			return this._defaultCamera;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06000E48 RID: 3656 RVA: 0x00030895 File Offset: 0x0002EA95
	public Camera UICamera
	{
		get
		{
			return this._uiCamera;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06000E49 RID: 3657 RVA: 0x0003089D File Offset: 0x0002EA9D
	// (set) Token: 0x06000E4A RID: 3658 RVA: 0x000308A5 File Offset: 0x0002EAA5
	public bool PostProcessingEnabled { get; set; } = true;

	// Token: 0x06000E4B RID: 3659 RVA: 0x000308B0 File Offset: 0x0002EAB0
	public void Awake()
	{
		this._defaultCamera = base.GetComponent<Camera>();
		for (int bitIndex = 0; bitIndex < 32; bitIndex++)
		{
			if ((this._overlayCamera.cullingMask & 1 << bitIndex) != 0)
			{
				this._overlayLayerIndex = bitIndex;
				return;
			}
		}
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x000308F2 File Offset: 0x0002EAF2
	public void SetPosition(Vector3 position)
	{
		position.z = base.transform.position.z;
		base.transform.position = position;
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x00030918 File Offset: 0x0002EB18
	public void AttachCameraToCanvas(Canvas canvas, CameraLayer layer)
	{
		Camera cameraForLayer;
		switch (layer)
		{
		case CameraLayer.UI:
			cameraForLayer = this._uiCamera;
			goto IL_2D;
		case CameraLayer.Overlay:
			cameraForLayer = this._overlayCamera;
			goto IL_2D;
		}
		cameraForLayer = this._defaultCamera;
		IL_2D:
		canvas.worldCamera = cameraForLayer;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x0003095C File Offset: 0x0002EB5C
	public void LateUpdate()
	{
		this._overlayCamera.orthographicSize = this._defaultCamera.orthographicSize;
		this._overlayCamera.nearClipPlane = this._defaultCamera.nearClipPlane;
		this._overlayCamera.farClipPlane = this._defaultCamera.farClipPlane;
		this._overlayCamera.rect = this._defaultCamera.rect;
		this._uiCamera.orthographicSize = this._defaultCamera.orthographicSize;
		this._uiCamera.nearClipPlane = this._defaultCamera.nearClipPlane;
		this._uiCamera.farClipPlane = this._defaultCamera.farClipPlane;
		this._uiCamera.rect = this._defaultCamera.rect;
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06000E4F RID: 3663 RVA: 0x00030A19 File Offset: 0x0002EC19
	// (set) Token: 0x06000E50 RID: 3664 RVA: 0x00030A26 File Offset: 0x0002EC26
	public float OrthographicSize
	{
		get
		{
			return this._defaultCamera.orthographicSize;
		}
		set
		{
			this._defaultCamera.orthographicSize = value;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06000E51 RID: 3665 RVA: 0x00030A34 File Offset: 0x0002EC34
	public float Width
	{
		get
		{
			return (float)this._defaultCamera.pixelWidth;
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06000E52 RID: 3666 RVA: 0x00030A42 File Offset: 0x0002EC42
	public float Height
	{
		get
		{
			return (float)this._defaultCamera.pixelHeight;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06000E53 RID: 3667 RVA: 0x00030A50 File Offset: 0x0002EC50
	public Vector2 Dimensions
	{
		get
		{
			return this._defaultCamera.pixelRect.size;
		}
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06000E54 RID: 3668 RVA: 0x00030A70 File Offset: 0x0002EC70
	public float AspectRatio
	{
		get
		{
			return this._defaultCamera.aspect;
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06000E55 RID: 3669 RVA: 0x00030A7D File Offset: 0x0002EC7D
	public int OverlayLayerIndex
	{
		get
		{
			return this._overlayLayerIndex;
		}
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00030A88 File Offset: 0x0002EC88
	public Vector3 GetWorldFromScreen(Vector2 screenPos)
	{
		Vector3 worldPosition = this._defaultCamera.ScreenToWorldPoint(screenPos);
		worldPosition.z = 0f;
		return worldPosition;
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00030AB4 File Offset: 0x0002ECB4
	public Vector2 GetScreenFromWorld(Vector3 worldPos)
	{
		return this._defaultCamera.WorldToScreenPoint(worldPos);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00030AC7 File Offset: 0x0002ECC7
	public Vector2 GetScreenFromWorld(Vector3Fixed worldPos)
	{
		return this.GetScreenFromWorld((Vector3)worldPos);
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00030AD8 File Offset: 0x0002ECD8
	public Bounds GetScreenBounds(float aspectRatio = -1f)
	{
		float screenAspect = (aspectRatio > 0f) ? aspectRatio : ((float)Screen.width / (float)Screen.height);
		float cameraHeight = this._defaultCamera.orthographicSize * 2f;
		return new Bounds(base.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0f));
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00030B2E File Offset: 0x0002ED2E
	public Vector2 GetPanFromWorld(Vector3 worldPos)
	{
		return Get.Pan(this.GetScreenFromWorld(worldPos));
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00030B3C File Offset: 0x0002ED3C
	public float GetAttenuationFromWorld(Vector3 worldPos, bool zoom = true, float falloffFactor = 5f)
	{
		return Get.Attenuation(this.GetScreenFromWorld(worldPos), zoom, falloffFactor);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00030B4C File Offset: 0x0002ED4C
	public Vector2 GetPanFromScreen(Vector2 screenPos)
	{
		return Get.Pan(screenPos);
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x00030B54 File Offset: 0x0002ED54
	public float GetAttenuationFromScreen(Vector2 screenPos, bool zoom = true, float falloffFactor = 5f)
	{
		return Get.Attenuation(screenPos, zoom, falloffFactor);
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x00030B5E File Offset: 0x0002ED5E
	public void OnReleasedFromScope(IScope scope)
	{
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x04000876 RID: 2166
	public RectFixed debugPlayableArea;

	// Token: 0x04000877 RID: 2167
	public bool debugDisplayPlayableArea;

	// Token: 0x04000878 RID: 2168
	public Vector2 debugPlayerOffset;

	// Token: 0x04000879 RID: 2169
	public CustomBlurData customBlur;

	// Token: 0x0400087A RID: 2170
	[SerializeField]
	private Camera _defaultCamera;

	// Token: 0x0400087B RID: 2171
	[SerializeField]
	private Camera _overlayCamera;

	// Token: 0x0400087C RID: 2172
	[SerializeField]
	private Camera _uiCamera;

	// Token: 0x0400087D RID: 2173
	private int _overlayLayerIndex;
}
