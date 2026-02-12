using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200059F RID: 1439
	public class HotkeyDebugView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x0600282D RID: 10285 RVA: 0x000AB5F4 File Offset: 0x000A97F4
		private void OnEnable()
		{
			this._messageStyle.fontSize = 50;
			this._messageStyle.alignment = TextAnchor.MiddleLeft;
			this._messageStyle.richText = true;
			this._messageStyle.normal.textColor = Color.magenta;
			this._tableRowStyle.fontSize = 30;
			this._tableRowStyle.normal.textColor = Color.gray;
			this._tableRowStyle.alignment = TextAnchor.MiddleLeft;
			this._tableRowStyle.padding = new RectOffset(5, 5, 5, 5);
		}

		// Token: 0x0600282E RID: 10286 RVA: 0x000AB67D File Offset: 0x000A987D
		public void ShowMessage(string message)
		{
			this._currentMessage = message;
			this._currentMessageAlpha = 1f;
		}

		// Token: 0x0600282F RID: 10287 RVA: 0x000AB694 File Offset: 0x000A9894
		public void ShowHotkeyDescriptions(List<HotkeyDescription> hotkeyDescriptions)
		{
			List<List<string>> table = new List<List<string>>
			{
				new List<string>(),
				new List<string>()
			};
			foreach (HotkeyDescription hotkeyDescription in hotkeyDescriptions)
			{
				table[0].Add(hotkeyDescription.description);
				table[1].Add(hotkeyDescription.KeyCodeDisplayName);
			}
			this._nextTabulatedMessageColumnsToDisplay = table;
		}

		// Token: 0x06002830 RID: 10288 RVA: 0x000AB724 File Offset: 0x000A9924
		public void HideHotkeyDescriptions()
		{
			this._nextTabulatedMessageColumnsToDisplay = null;
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06002831 RID: 10289 RVA: 0x000AB72D File Offset: 0x000A992D
		public bool IsShowingHotkeyDescriptions
		{
			get
			{
				return this._nextTabulatedMessageColumnsToDisplay != null;
			}
		}

		// Token: 0x06002832 RID: 10290 RVA: 0x000AB738 File Offset: 0x000A9938
		private void ShowTabulatedMessage(int startRow, int endRow, int horizontalOffsetIndex)
		{
			List<Vector2> maxBoundsForColumn = new List<Vector2>();
			float tableHeight = 0f;
			for (int columnIndex = 0; columnIndex < this._nextTabulatedMessageColumnsToDisplay.Count; columnIndex++)
			{
				maxBoundsForColumn.Add(Vector2.zero);
				for (int rowIndex = startRow; rowIndex < endRow; rowIndex++)
				{
					GUIContent rowTextContent = new GUIContent(this._nextTabulatedMessageColumnsToDisplay[columnIndex][rowIndex]);
					Vector2 rowTextSize = this._tableRowStyle.CalcSize(rowTextContent);
					Vector2 maxBoundsForThisColumn = maxBoundsForColumn[columnIndex];
					if (rowTextSize.x > maxBoundsForThisColumn.x)
					{
						maxBoundsForThisColumn.x = rowTextSize.x;
					}
					if (rowTextSize.y > maxBoundsForThisColumn.y)
					{
						maxBoundsForThisColumn.y = rowTextSize.y;
					}
					maxBoundsForColumn[columnIndex] = maxBoundsForThisColumn;
					if (columnIndex == 0)
					{
						tableHeight += rowTextSize.y;
					}
				}
			}
			float tableWidth = (from bounds in maxBoundsForColumn
			select bounds.x).Sum();
			Rect tableRect = new Rect(0.5f * (HotkeyDebugView.BaseResolution.x - tableWidth) + (float)horizontalOffsetIndex * tableWidth * 0.5f, 0.5f * (HotkeyDebugView.BaseResolution.y - tableHeight), tableWidth, tableHeight);
			GUI.Box(tableRect, "");
			float widthOffset = 0f;
			for (int columnIndex2 = 0; columnIndex2 < this._nextTabulatedMessageColumnsToDisplay.Count; columnIndex2++)
			{
				for (int rowIndex2 = startRow; rowIndex2 < endRow; rowIndex2++)
				{
					string rowText = this._nextTabulatedMessageColumnsToDisplay[columnIndex2][rowIndex2];
					GUIContent rowTextContent2 = new GUIContent(rowText);
					Vector2 rowTextSize2 = this._tableRowStyle.CalcSize(rowTextContent2);
					this._tableRowStyle.normal.textColor = ((columnIndex2 == 0) ? Color.green : Color.white);
					GUI.Label(new Rect(tableRect.x + widthOffset, tableRect.y + (float)(rowIndex2 - startRow) * rowTextSize2.y, maxBoundsForColumn[columnIndex2].x, rowTextSize2.y), rowText, this._tableRowStyle);
				}
				widthOffset += maxBoundsForColumn[columnIndex2].x;
			}
		}

		// Token: 0x06002833 RID: 10291 RVA: 0x000AB96C File Offset: 0x000A9B6C
		public void Reset()
		{
			this._messageStyle = new GUIStyle();
			this._tableRowStyle = new GUIStyle();
			this._currentMessage = "";
			this._nextTabulatedMessageColumnsToDisplay = null;
			this._currentMessageAlpha = 0f;
			this._hotkeyViewTransformationMatrix = default(Matrix4x4);
		}

		// Token: 0x06002834 RID: 10292 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002835 RID: 10293 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x040021F5 RID: 8693
		private GUIStyle _messageStyle = new GUIStyle();

		// Token: 0x040021F6 RID: 8694
		private GUIStyle _tableRowStyle = new GUIStyle();

		// Token: 0x040021F7 RID: 8695
		private static readonly Vector2 BaseResolution = new Vector2(1920f, 1080f);

		// Token: 0x040021F8 RID: 8696
		private Vector2Int _screenSize;

		// Token: 0x040021F9 RID: 8697
		private Matrix4x4 _hotkeyViewTransformationMatrix;

		// Token: 0x040021FA RID: 8698
		private const float MessageFadeSpeed = 0.15f;

		// Token: 0x040021FB RID: 8699
		private string _currentMessage = "";

		// Token: 0x040021FC RID: 8700
		private float _currentMessageAlpha;

		// Token: 0x040021FD RID: 8701
		private List<List<string>> _nextTabulatedMessageColumnsToDisplay;
	}
}
