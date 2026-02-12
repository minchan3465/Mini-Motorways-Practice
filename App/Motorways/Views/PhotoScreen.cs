using System;
using Client;
using Factory;
using Motorways.Constants;
using UnityEngine;
using Utils;

namespace Motorways.Views
{
	// Token: 0x02000566 RID: 1382
	public class PhotoScreen : OverlayBaseScreen
	{
		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x0600258A RID: 9610 RVA: 0x0000222C File Offset: 0x0000042C
		protected override OverlayBaseScreen.OverlayScreenType overlayScreenType
		{
			get
			{
				return OverlayBaseScreen.OverlayScreenType.PhotoScreen;
			}
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x0009ED00 File Offset: 0x0009CF00
		public override void Awake()
		{
			base.Awake();
			this._photoModeCopyMaterial = new Material(this._photoModeCopyShader);
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x0009ED1C File Offset: 0x0009CF1C
		public override void InitScreen(IScope gameScope, bool blocksGameInput)
		{
			base.InitScreen(gameScope, blocksGameInput);
			StandaloneLocString localizedFolderString = StandaloneLocString.CreateString(this._appScope, StringId.MiniMotorways);
			this._folderString = localizedFolderString.ToString();
			this._appScope.Release(localizedFolderString);
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x0009ED57 File Offset: 0x0009CF57
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._flash.alpha > 0f)
			{
				this._flash.alpha = Mathf.Clamp01(this._flash.alpha - deltaTime);
			}
		}

		// Token: 0x0600258E RID: 9614 RVA: 0x0009ED90 File Offset: 0x0009CF90
		public void OnTakePhoto()
		{
			this._game.Scope.Get<NotificationView>().KillNotification();
			base.SetFrameLayer(LayerConstants.OverlayLayerId);
			base.nonPhotoLayer.alpha = 0f;
			this._flash.alpha = 0f;
			GameObject captureCameraObject = new GameObject();
			Camera captureCamera = captureCameraObject.AddComponent<Camera>();
			UnityEngine.Object obj = captureCameraObject.AddComponent<MiniMotorwaysRenderFeatureCameraMarker>();
			captureCamera.CopyFrom(this.gameCamera.DefaultCamera);
			this._canvas.worldCamera = captureCamera;
			Camera previousFadeToBlackCamera = this._screenStack.FadeToBlackCanvas.worldCamera;
			this._screenStack.FadeToBlackCanvas.worldCamera = captureCamera;
			Vector2Int screenshotDimensions = this.softwareCapabilities.ScreenshotDimensions;
			if (!Diagnostics.Verify(screenshotDimensions.x > 0 && screenshotDimensions.y > 0, "Screenshot Dimensions are invalid!"))
			{
				screenshotDimensions = new Vector2Int(Screen.width, Screen.height);
			}
			RenderTexture gameRenderTexture = RenderTexture.GetTemporary(screenshotDimensions.x, screenshotDimensions.y, 24, RenderTextureFormat.ARGB32);
			gameRenderTexture.antiAliasing = this._player.AntiAliasingMSAALevelForUniversalRenderPipeline;
			captureCamera.targetTexture = gameRenderTexture;
			captureCamera.Render();
			UnityEngine.Object.DestroyImmediate(obj);
			RenderTexture auxiliaryGameCameraRenderTexture = RenderTexture.GetTemporary(screenshotDimensions.x, screenshotDimensions.y, 24, RenderTextureFormat.ARGB32);
			auxiliaryGameCameraRenderTexture.antiAliasing = this._player.AntiAliasingMSAALevelForUniversalRenderPipeline;
			foreach (AuxiliaryGameCamera auxiliaryGameCamera in this.gameCamera.GetComponentsInChildren<AuxiliaryGameCamera>())
			{
				if (auxiliaryGameCamera.ShouldRenderInPhotosFromPhotoMode)
				{
					Camera auxiliaryCameraComponent = auxiliaryGameCamera.GetComponent<Camera>();
					captureCamera.Reset();
					captureCamera.CopyFrom(auxiliaryCameraComponent);
					captureCamera.backgroundColor = Color.clear;
					captureCamera.clearFlags = CameraClearFlags.Color;
					captureCamera.targetTexture = auxiliaryGameCameraRenderTexture;
					captureCamera.Render();
					Graphics.Blit(auxiliaryGameCameraRenderTexture, gameRenderTexture, this._photoModeCopyMaterial);
				}
			}
			RenderTexture.ReleaseTemporary(auxiliaryGameCameraRenderTexture);
			this._screenStack.FadeToBlackCanvas.worldCamera = previousFadeToBlackCamera;
			this._canvas.worldCamera = this.gameCamera.UICamera;
			RenderTexture backup = RenderTexture.active;
			RenderTexture.active = gameRenderTexture;
			Texture2D image = new Texture2D(gameRenderTexture.width, gameRenderTexture.height, TextureFormat.RGB24, false);
			image.ReadPixels(new Rect(0f, 0f, (float)gameRenderTexture.width, (float)gameRenderTexture.height), 0, 0);
			image.Apply();
			if (!Diagnostics.Verify(!string.IsNullOrEmpty(this._folderString), "Parent folder string isn't set!"))
			{
				this._folderString = "Mini Motorways";
			}
			StringKey key = this._appScope.Get<StringKey>();
			key.InitWithString(this._game.MapDefinition.mapName);
			StandaloneLocString localizedCityName = StandaloneLocString.CreateString(this._appScope, key);
			StringId messageId;
			bool flag = this.softwareCapabilities.SaveScreenshot(image, localizedCityName.ToString(), this._folderString, out messageId);
			RenderTexture.active = backup;
			RenderTexture.ReleaseTemporary(gameRenderTexture);
			base.nonPhotoLayer.alpha = 1f;
			if (flag)
			{
				this._flash.alpha = 1f;
			}
			base.SetFrameLayer(LayerConstants.UILayerId);
			UnityEngine.Object.Destroy(captureCameraObject);
			if (messageId != StringId.None)
			{
				this._game.Scope.Get<NotificationView>().AddNotification(messageId, 0f, null);
			}
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x0009F0AC File Offset: 0x0009D2AC
		public override void OnTransitionedOut()
		{
			base.OnTransitionedOut();
			foreach (VehicleView vehicleView in this._gameScope.Get<ViewClient>().GetViews<VehicleView>())
			{
				vehicleView.SkipHeadlightResponseTime = false;
			}
		}

		// Token: 0x04001FB2 RID: 8114
		[SerializeField]
		private CanvasGroup _flash;

		// Token: 0x04001FB3 RID: 8115
		[SerializeField]
		private Shader _photoModeCopyShader;

		// Token: 0x04001FB4 RID: 8116
		private Material _photoModeCopyMaterial;

		// Token: 0x04001FB5 RID: 8117
		private string _folderString = "";
	}
}
