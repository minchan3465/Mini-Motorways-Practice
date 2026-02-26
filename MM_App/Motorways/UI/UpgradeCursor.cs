using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200074B RID: 1867
	[RequireComponent(typeof(RectTransform))]
	public class UpgradeCursor : MonoBehaviour, IView, IReusable
	{
		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06003421 RID: 13345 RVA: 0x000F5CF8 File Offset: 0x000F3EF8
		public Vector2 Position
		{
			get
			{
				return this._cursorIconTransform.anchoredPosition;
			}
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x000F5D08 File Offset: 0x000F3F08
		public void Initialize(Sprite sprite, RectTransform parentTransform)
		{
			this._assetPlaced = false;
			this._assetActionCancelled = false;
			this._upgradeSprite.sprite = sprite;
			this._rectTransform = base.GetComponent<RectTransform>();
			this._rectTransform.SetParent(parentTransform);
			this._rectTransform.localPosition = Vector3.zero;
			this._rectTransform.localScale = Vector3.one;
			this._viewClient.AddView(this);
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x000F5D74 File Offset: 0x000F3F74
		public void SetPosition(Vector2 screenPosition, UpgradeCursor.UpgradeCursorOffsetType offsetType = UpgradeCursor.UpgradeCursorOffsetType.TopLeft)
		{
			this._rectTransform.anchoredPosition = screenPosition;
			switch (offsetType)
			{
			case UpgradeCursor.UpgradeCursorOffsetType.OnPointer:
				this._cursorIconTransform.anchoredPosition = Vector2.zero;
				return;
			case UpgradeCursor.UpgradeCursorOffsetType.TopLeft:
				this._cursorIconTransform.anchoredPosition = new Vector2(-this.horizontalOffset, this.verticalOffset);
				return;
			case UpgradeCursor.UpgradeCursorOffsetType.TopRight:
				this._cursorIconTransform.anchoredPosition = new Vector2(this.horizontalOffset, this.verticalOffset);
				return;
			default:
				return;
			}
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x000F5DEB File Offset: 0x000F3FEB
		public Vector2Int GetTileCoordinates()
		{
			return this._tilemapView.GetTileCoordinatesFromWorldPosition(this._cursorIconTransform.position);
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x000F5E08 File Offset: 0x000F4008
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._assetActionCancelled)
			{
				base.gameObject.SetActive(false);
				return TickResult.Destroy;
			}
			if (this._assetPlaced)
			{
				base.gameObject.SetActive(false);
				return TickResult.Destroy;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x000F5E37 File Offset: 0x000F4037
		public void PlaceAssetAtPosition(Vector2Int tilePosition)
		{
			this._assetPlaced = true;
		}

		// Token: 0x06003428 RID: 13352 RVA: 0x000F5E40 File Offset: 0x000F4040
		public void CancelUpgradeCursor()
		{
			this._assetActionCancelled = true;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06003429 RID: 13353 RVA: 0x000F5E55 File Offset: 0x000F4055
		public void Reset()
		{
			this._assetPlaced = false;
			this._assetActionCancelled = false;
			base.transform.localPosition = Vector3.zero;
		}

		// Token: 0x04002C7A RID: 11386
		public static Diagnostics.Log.Channel Log = new Diagnostics.Log.Channel("UpgradeCursor");

		// Token: 0x04002C7B RID: 11387
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002C7C RID: 11388
		[Dependency]
		private TilemapView _tilemapView;

		// Token: 0x04002C7D RID: 11389
		private RectTransform _rectTransform;

		// Token: 0x04002C7E RID: 11390
		[SerializeField]
		private RectTransform _cursorIconTransform;

		// Token: 0x04002C7F RID: 11391
		private bool _assetPlaced;

		// Token: 0x04002C80 RID: 11392
		private bool _assetActionCancelled;

		// Token: 0x04002C81 RID: 11393
		[SerializeField]
		private Image _upgradeSprite;

		// Token: 0x04002C82 RID: 11394
		public float verticalOffset = 20f;

		// Token: 0x04002C83 RID: 11395
		public float horizontalOffset = 20f;

		// Token: 0x0200074C RID: 1868
		public enum UpgradeCursorOffsetType
		{
			// Token: 0x04002C85 RID: 11397
			OnPointer,
			// Token: 0x04002C86 RID: 11398
			TopLeft,
			// Token: 0x04002C87 RID: 11399
			TopRight
		}
	}
}
