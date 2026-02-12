using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Themes;
using Server;
using TMPro;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200060D RID: 1549
	public class AnchoredMessageView : MonoBehaviour, IView, IThemeComponent, IReusable, AnchoredMessageModel.IObserver
	{
		// Token: 0x06002B4B RID: 11083 RVA: 0x000BE9C0 File Offset: 0x000BCBC0
		public void Reset()
		{
			this._parentTransform = null;
			this._animationTimer = 0f;
			this._isAppearing = true;
			this._isAnimating = true;
			this._forceTransitionInEases = false;
			this._showingDismissArrow = false;
			this._anchorType = AnchoredMessageAnchorType.Screen;
			this._worldAnchor = default(Vector3);
			this._direction = TileDirection.North;
			this._anchorOffset = default(Vector2);
			this._uiAnchorPivot = default(Vector2);
			base.transform.position = default(Vector3);
			this._localeDatabase.RemoveLocalizedObject(this.text);
			this._isKilled = false;
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x000BEA5C File Offset: 0x000BCC5C
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._isKilled)
			{
				return TickResult.Destroy;
			}
			float cameraSize = this._gameCamera.OrthographicSize;
			float cameraScale = cameraSize / 6f;
			Vector3 textSize = this.textMesh.bounds.size;
			textSize.x = Mathf.Max(textSize.x, 0f);
			textSize.y = Mathf.Max(textSize.y, 0f);
			textSize.x += 1f;
			if (this._showingDismissArrow)
			{
				textSize.x += this.arrowSize;
			}
			textSize.y += 0.6f;
			float aspectRatio = Screen.safeArea.width / Screen.safeArea.height;
			float cameraWidth = cameraSize * aspectRatio;
			float halfTextWidth = textSize.x * 0.5f;
			float x2 = this._gameCamera.transform.position.x;
			float leftEdgeX = x2 - cameraWidth + (halfTextWidth + 0.1f) * cameraScale;
			float rightEdgeX = x2 + cameraWidth - (halfTextWidth + 0.1f) * cameraScale;
			Vector3 postPosition;
			Vector3 postOrigin;
			Vector3 signPosition;
			if (this._anchorType == AnchoredMessageAnchorType.Screen)
			{
				postPosition = Vector3.zero;
				postOrigin = Vector3.zero;
				signPosition = this._gameCamera.transform.position;
				Vector2 scaledCameraOffset = this._anchorOffset * cameraSize;
				signPosition.x += scaledCameraOffset.x;
				signPosition.y += scaledCameraOffset.y;
				signPosition.z = 0f;
			}
			else if (this._anchorType == AnchoredMessageAnchorType.World)
			{
				postPosition = this._worldAnchor;
				signPosition = this._worldAnchor;
				if (this._direction == TileDirection.West || this._direction == TileDirection.East)
				{
					if (this._direction == TileDirection.West)
					{
						float desiredX = this._worldAnchor.x - (3f + halfTextWidth) * cameraScale;
						float onScreenX = Mathf.Max(desiredX, leftEdgeX);
						if (onScreenX < postPosition.x)
						{
							signPosition.x = onScreenX;
						}
						else
						{
							onScreenX = Mathf.Min(this._worldAnchor.x + (3f + halfTextWidth) * cameraScale, rightEdgeX);
							if (onScreenX > postPosition.x)
							{
								signPosition.x = onScreenX;
							}
							else
							{
								signPosition.x = desiredX;
							}
						}
					}
					else
					{
						float desiredX2 = this._worldAnchor.x + (3f + halfTextWidth) * cameraScale;
						float onScreenX2 = Mathf.Min(desiredX2, rightEdgeX);
						if (onScreenX2 > postPosition.x)
						{
							signPosition.x = onScreenX2;
						}
						else
						{
							onScreenX2 = Mathf.Max(this._worldAnchor.x - (3f + halfTextWidth) * cameraScale, leftEdgeX);
							if (onScreenX2 < postPosition.x)
							{
								signPosition.x = onScreenX2;
							}
							else
							{
								signPosition.x = desiredX2;
							}
						}
					}
				}
				else
				{
					float y = this._gameCamera.transform.position.y;
					float topY = y + cameraSize - textSize.y * cameraScale - 0.1f * cameraScale;
					float bottomY = y - cameraSize + textSize.y * cameraScale + 0.1f * cameraScale;
					float desiredY = this._worldAnchor.y + 3f * cameraScale;
					float onScreenY = Mathf.Min(desiredY, topY);
					if (onScreenY > postPosition.y)
					{
						signPosition.y = onScreenY;
					}
					else
					{
						onScreenY = Mathf.Max(this._worldAnchor.y - 3f * cameraScale, bottomY);
						if (onScreenY < postPosition.y)
						{
							signPosition.y = onScreenY;
						}
						else
						{
							signPosition.y = desiredY;
						}
					}
				}
				postOrigin = signPosition;
			}
			else
			{
				Vector3[] worldCorners = new Vector3[4];
				this._parentTransform.GetWorldCorners(worldCorners);
				postPosition = new Vector3(Mathf.Lerp(worldCorners[0].x, worldCorners[2].x, this._uiAnchorPivot.x), Mathf.Lerp(worldCorners[0].y, worldCorners[1].y, this._uiAnchorPivot.y), 0f);
				Vector3 signOffset = Vector3.zero;
				Bounds screenBounds = this._gameCamera.GetScreenBounds(-1f);
				Vector2 normalizedPostScreenPosition = new Vector2((postPosition.x - screenBounds.min.x) / screenBounds.size.x, (postPosition.y - screenBounds.min.y) / screenBounds.size.y);
				Vector3 directionFromPostToSign;
				if (normalizedPostScreenPosition.y > 0.85f)
				{
					directionFromPostToSign = new Vector3(0f, -1f, 0f);
					float clampedPostX = Mathf.Clamp(postPosition.x, leftEdgeX, rightEdgeX);
					signOffset.x = clampedPostX - postPosition.x;
				}
				else if (normalizedPostScreenPosition.x < 0.5f)
				{
					directionFromPostToSign = new Vector3(1f, 0f, 0f);
				}
				else
				{
					directionFromPostToSign = new Vector3(-1f, 0f, 0f);
				}
				float postLength = 3f + Mathf.Abs(directionFromPostToSign.x) * textSize.x * 0.5f;
				postOrigin = postPosition + directionFromPostToSign * (postLength * cameraScale);
				signPosition = postOrigin + signOffset;
			}
			float postScale = 1f;
			float postLengthScale = 1f;
			float messageBoardScale = 1f;
			float textAlpha = 1f;
			if (this._isAnimating)
			{
				if (!this._hasFiredAudioAppear)
				{
					this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.TextMessageShown, this._gameCamera.GetPanFromWorld(signPosition).x, -1f, true, null));
					this._hasFiredAudioAppear = true;
				}
				this._animationTimer += timeInterval.Delta * (1f / (this._isAppearing ? 1.2f : -0.8f));
				if (this._animationTimer >= 1f && this._isAppearing)
				{
					this._isAnimating = false;
				}
				else
				{
					if (this._animationTimer <= 0f && !this._isAppearing)
					{
						this._isAnimating = false;
						return TickResult.Destroy;
					}
					float animationTimer = this._animationTimer;
					float postLengthScaleT = Mathf.Clamp01((animationTimer - 0.1f) / 0.2f);
					float messageBoardScaleT = animationTimer;
					float p = Mathf.Clamp01((animationTimer - 0.3f) / 0.4f);
					postLengthScale = Easings.QuarticEaseIn(postLengthScaleT);
					messageBoardScale = this.MessageBoardEasing(messageBoardScaleT);
					textAlpha = Easings.Linear(p);
					Color arrowColor = this.arrowSprite.color;
					arrowColor.a = textAlpha;
					this.arrowSprite.color = arrowColor;
				}
			}
			else
			{
				Color arrowColor2 = this.arrowSprite.color;
				arrowColor2.a = this.arrowSpriteAlphaCurve.Evaluate(Time.time);
				this.arrowSprite.color = arrowColor2;
			}
			if (this.signPost.enabled)
			{
				this.signPost.SetPosition(0, postOrigin);
				this.signPost.SetPosition(1, Vector3.Lerp(postOrigin, postPosition, postLengthScale));
				this.signPost.startWidth = (this.signPost.endWidth = 0.1f * postScale * cameraScale);
			}
			this.messageBoard.transform.localScale = new Vector3(cameraScale, cameraScale, 1f);
			this.messageBoard.transform.position = signPosition;
			float messageBoardWidth = textSize.x * messageBoardScale;
			float messageBoardHeight = Mathf.Min(messageBoardWidth, textSize.y);
			this.messageBoard.size = new Vector2(messageBoardWidth, messageBoardHeight);
			float x = messageBoardWidth / 2f - this.arrowSize;
			this.arrowSprite.transform.localPosition = new Vector3(x, 0f, 0f);
			Color color = this.textMesh.color;
			color.a = textAlpha;
			this.textMesh.color = color;
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x000BF1F2 File Offset: 0x000BD3F2
		public void Kill()
		{
			this.messageBoard.transform.localScale = Vector3.zero;
			this._isKilled = true;
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06002B4F RID: 11087 RVA: 0x000BF210 File Offset: 0x000BD410
		private Func<float, float> MessageBoardEasing
		{
			get
			{
				if (this._isAppearing || this._forceTransitionInEases)
				{
					return new Func<float, float>(Easings.ElasticEaseOut);
				}
				return new Func<float, float>(Easings.BackEaseOut);
			}
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x000BF23C File Offset: 0x000BD43C
		private void Initialize()
		{
			this.text.HandleParentAllocated(this._scope);
			this.textMesh.alpha = 0f;
			this.messageBoard.size = new Vector2(0f, 0f);
			this._animationTimer = 0f;
			this._isAppearing = true;
			this._isAnimating = true;
			this._forceTransitionInEases = false;
			this._hasFiredAudioAppear = false;
			this._audioSignPosition = new Vector3(0f, 0f, 0f);
		}

		// Token: 0x06002B51 RID: 11089 RVA: 0x000BF2C8 File Offset: 0x000BD4C8
		public void InitializeWithModel(AnchoredMessageModel model)
		{
			int intParameter = 0;
			Dictionary<string, string> parameters = null;
			if (model.IntParameter != null)
			{
				intParameter = model.IntParameter.Value;
				parameters = new Dictionary<string, string>
				{
					{
						"Num",
						model.IntParameter.Value.ToString()
					}
				};
			}
			StandaloneLocString locString;
			if (parameters != null)
			{
				StringKey key = this._scope.Get<StringKey>();
				key.InitWithStringId(model.Message, intParameter, parameters);
				locString = StandaloneLocString.CreateString(this._scope, key);
			}
			else
			{
				locString = StandaloneLocString.CreateString(this._scope, model.Message);
			}
			switch (model.AnchorType)
			{
			case AnchoredMessageAnchorType.Screen:
				this.InitializeWithScreenAnchor(locString, model.Offset, model.CameraLayer);
				break;
			case AnchoredMessageAnchorType.World:
				this.InitializeWithWorldAnchor(locString, model.WorldAnchor, model.Direction);
				break;
			case AnchoredMessageAnchorType.UI:
			{
				RectTransform anchor = null;
				switch (model.UIAnchor)
				{
				case UIMessageAnchor.DrawModeToggle:
					anchor = this._gameUI.drawButtonAnchors.GetComponent<RectTransform>();
					break;
				case UIMessageAnchor.Concrete:
					anchor = this._gameUI.UpgradeBar.GetRectTransformForUpgrade(UpgradeType.Concrete);
					break;
				case UIMessageAnchor.TrafficLight:
					anchor = this._gameUI.UpgradeBar.GetRectTransformForUpgrade(UpgradeType.TrafficLight);
					break;
				case UIMessageAnchor.Motorway:
					anchor = this._gameUI.UpgradeBar.GetRectTransformForUpgrade(UpgradeType.Motorway);
					break;
				case UIMessageAnchor.Score:
					anchor = this._gameUI.ScoreTextAnchor.GetComponent<RectTransform>();
					break;
				case UIMessageAnchor.Clock:
					anchor = this._gameUI.ClockAnchor.GetComponent<RectTransform>();
					break;
				}
				this.InitializeWithUIAnchor(locString, anchor, model.UIAnchorPivot);
				break;
			}
			}
			this._localeDatabase.AddLocalizedObject(this.text);
			this.SetDismissArrowVisibility(model.ShowDismissArrow);
			model.Subscribe(this);
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x000BF48B File Offset: 0x000BD68B
		public void InitializeWithScreenAnchor(StandaloneLocString messageText, Vector2 screenOffset, CameraLayer cameraLayer = CameraLayer.Default)
		{
			this.Initialize();
			this.text.LocString = messageText;
			this._anchorType = AnchoredMessageAnchorType.Screen;
			this._anchorOffset = screenOffset;
			this.signPost.enabled = false;
			this.InitializeCameraLayer(cameraLayer);
			this.SetDismissArrowVisibility(false);
		}

		// Token: 0x06002B53 RID: 11091 RVA: 0x000BF4C8 File Offset: 0x000BD6C8
		public void InitializeWithWorldAnchor(StandaloneLocString messageText, Vector3 worldAnchor, TileDirection direction = TileDirection.North)
		{
			this.Initialize();
			this.text.LocString = messageText;
			this._anchorType = AnchoredMessageAnchorType.World;
			this._worldAnchor = worldAnchor;
			this._direction = direction;
			this.signPost.enabled = true;
			this.InitializeCameraLayer(CameraLayer.Default);
			this.SetDismissArrowVisibility(false);
		}

		// Token: 0x06002B54 RID: 11092 RVA: 0x000BF518 File Offset: 0x000BD718
		public void InitializeWithUIAnchor(StandaloneLocString messageText, RectTransform transform, Vector2 transformPivot)
		{
			this.Initialize();
			this.text.LocString = messageText;
			this._anchorType = AnchoredMessageAnchorType.UI;
			this._parentTransform = transform;
			this._uiAnchorPivot = transformPivot;
			this.signPost.enabled = true;
			this.InitializeCameraLayer(CameraLayer.Default);
			this.SetDismissArrowVisibility(false);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x000BF568 File Offset: 0x000BD768
		public void OnAnimationRelease()
		{
			if (this._isAnimating && this._isAppearing)
			{
				this._forceTransitionInEases = true;
			}
			else
			{
				this._animationTimer = 1f;
			}
			this._isAppearing = false;
			this._isAnimating = true;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.TextMessageShown, this._gameCamera.GetPanFromWorld(this._audioSignPosition).x, -1f, false, null));
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x000BF5E4 File Offset: 0x000BD7E4
		public void SetDismissArrowVisibility(bool visible)
		{
			this.arrowSprite.gameObject.SetActive(visible);
			Vector4 margin = this.textMesh.margin;
			margin.z = (visible ? (this.arrowSize * 2f) : margin.x);
			this.textMesh.margin = margin;
			this._showingDismissArrow = visible;
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x000BF640 File Offset: 0x000BD840
		public void InitializeCameraLayer(CameraLayer cameraLayer)
		{
			int layerId = (cameraLayer == CameraLayer.Default) ? LayerMask.NameToLayer("UI") : LayerMask.NameToLayer("Overlay");
			base.gameObject.layer = layerId;
			this.signPost.gameObject.layer = layerId;
			this.textMesh.gameObject.layer = layerId;
			this.messageBoard.gameObject.layer = layerId;
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x000BF6A6 File Offset: 0x000BD8A6
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this.text.GetComponent<ThemedComponent>().InitializeTheme(themeDatabase);
		}

		// Token: 0x06002B59 RID: 11097 RVA: 0x000BF6B9 File Offset: 0x000BD8B9
		public void ApplyTheme(ITheme theme)
		{
			this.text.GetComponent<ThemedComponent>().ApplyTheme(theme);
		}

		// Token: 0x06002B5A RID: 11098 RVA: 0x000020AA File Offset: 0x000002AA
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06002B5B RID: 11099 RVA: 0x000BF6CC File Offset: 0x000BD8CC
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			this.text.GetComponent<ThemedComponent>().ReleaseTheme(themeDatabase);
		}

		// Token: 0x04002567 RID: 9575
		public LineRenderer signPost;

		// Token: 0x04002568 RID: 9576
		public LocalizedTextUI text;

		// Token: 0x04002569 RID: 9577
		public TMP_Text textMesh;

		// Token: 0x0400256A RID: 9578
		public SpriteRenderer messageBoard;

		// Token: 0x0400256B RID: 9579
		public SpriteRenderer arrowSprite;

		// Token: 0x0400256C RID: 9580
		public float arrowSize = 0.5f;

		// Token: 0x0400256D RID: 9581
		public AnimationCurve arrowSpriteAlphaCurve;

		// Token: 0x0400256E RID: 9582
		private const float TransitionInDuration = 1.2f;

		// Token: 0x0400256F RID: 9583
		private const float TransitionOutDuration = 0.8f;

		// Token: 0x04002570 RID: 9584
		private float _animationTimer;

		// Token: 0x04002571 RID: 9585
		private bool _isAppearing = true;

		// Token: 0x04002572 RID: 9586
		private bool _isAnimating = true;

		// Token: 0x04002573 RID: 9587
		private bool _forceTransitionInEases;

		// Token: 0x04002574 RID: 9588
		private bool _showingDismissArrow;

		// Token: 0x04002575 RID: 9589
		private AnchoredMessageAnchorType _anchorType;

		// Token: 0x04002576 RID: 9590
		private Vector3 _worldAnchor;

		// Token: 0x04002577 RID: 9591
		private TileDirection _direction;

		// Token: 0x04002578 RID: 9592
		private Vector2 _anchorOffset;

		// Token: 0x04002579 RID: 9593
		private Vector2 _uiAnchorPivot;

		// Token: 0x0400257A RID: 9594
		private RectTransform _parentTransform;

		// Token: 0x0400257B RID: 9595
		private bool _isKilled;

		// Token: 0x0400257C RID: 9596
		private const float PostWidth = 0.1f;

		// Token: 0x0400257D RID: 9597
		private const float TextWidthPadding = 0.5f;

		// Token: 0x0400257E RID: 9598
		private const float TextHeightPadding = 0.3f;

		// Token: 0x0400257F RID: 9599
		private const float ScreenPadding = 0.1f;

		// Token: 0x04002580 RID: 9600
		private const float PostLength = 3f;

		// Token: 0x04002581 RID: 9601
		private const float DefaultCameraSize = 6f;

		// Token: 0x04002582 RID: 9602
		[Dependency]
		private IScope _scope;

		// Token: 0x04002583 RID: 9603
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002584 RID: 9604
		[Dependency]
		protected GameUIScreen _gameUI;

		// Token: 0x04002585 RID: 9605
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002586 RID: 9606
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04002587 RID: 9607
		private bool _hasFiredAudioAppear;

		// Token: 0x04002588 RID: 9608
		private Vector3 _audioSignPosition;

		// Token: 0x0200060E RID: 1550
		public class Builder : IViewBuilder
		{
			// Token: 0x06002B5D RID: 11101 RVA: 0x000BF700 File Offset: 0x000BD900
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				AnchoredMessageView message = client.Scope.Get<AnchoredMessageView>();
				message.InitializeWithModel(model as AnchoredMessageModel);
				client.AddView(message);
			}
		}
	}
}
