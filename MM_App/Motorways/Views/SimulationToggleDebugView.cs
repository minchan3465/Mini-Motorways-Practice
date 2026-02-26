using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005B6 RID: 1462
	public class SimulationToggleDebugView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x060028B2 RID: 10418 RVA: 0x0000222C File Offset: 0x0000042C
		private bool ShouldShowView
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060028B3 RID: 10419 RVA: 0x000ADAD0 File Offset: 0x000ABCD0
		private void OnEnable()
		{
			this._boxStyle.fontSize = 18;
			this._boxStyle.alignment = TextAnchor.MiddleLeft;
			this._boxStyle.richText = true;
			this._boxStyle.normal.textColor = Color.white;
			this._boxStyle.normal.background = DebugViewUtils.DebugWindowBackground;
			this._boxStyle.padding = new RectOffset(10, 10, 10, 10);
		}

		// Token: 0x060028B4 RID: 10420 RVA: 0x000ADB44 File Offset: 0x000ABD44
		private Rect CalculateRectSize(string text)
		{
			GUIContent content = new GUIContent(text);
			Vector2 contentSize = this._boxStyle.CalcSize(content);
			return new Rect(10f, (float)Screen.height - contentSize.y - 10f, contentSize.x, contentSize.y);
		}

		// Token: 0x060028B5 RID: 10421 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x060028B6 RID: 10422 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060028B7 RID: 10423 RVA: 0x000ADB8E File Offset: 0x000ABD8E
		public void Reset()
		{
			this._boxStyle = new GUIStyle();
		}

		// Token: 0x04002262 RID: 8802
		[Dependency]
		private ClockModel _clockModel;

		// Token: 0x04002263 RID: 8803
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x04002264 RID: 8804
		[Dependency]
		private Simulation _simulation;

		// Token: 0x04002265 RID: 8805
		[Dependency]
		private BuildingsIndicatorView _indicatorView;

		// Token: 0x04002266 RID: 8806
		[Dependency]
		private GameUIScreen _gameUIScreen;

		// Token: 0x04002267 RID: 8807
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabaseModel;

		// Token: 0x04002268 RID: 8808
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04002269 RID: 8809
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x0400226A RID: 8810
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x0400226B RID: 8811
		private GUIStyle _boxStyle = new GUIStyle();

		// Token: 0x0400226C RID: 8812
		public const string ShouldShowDebugToggleView = "ShouldShowDebugToggleView";

		// Token: 0x0400226D RID: 8813
		private const int Padding = 10;

		// Token: 0x0400226E RID: 8814
		private const int Margins = 10;

		// Token: 0x0400226F RID: 8815
		private bool _isCollapsed;
	}
}
