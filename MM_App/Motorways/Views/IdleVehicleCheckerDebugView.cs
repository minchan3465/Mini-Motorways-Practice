using System;
using Client;
using Factory;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005A2 RID: 1442
	public class IdleVehicleCheckerDebugView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x0600283F RID: 10303 RVA: 0x000ABB17 File Offset: 0x000A9D17
		private void Awake()
		{
			this._textStyle = new GUIStyle
			{
				fontSize = 25
			};
			this._idleCircleStyle = new GUIStyle
			{
				fontSize = 50,
				fontStyle = FontStyle.Bold
			};
		}

		// Token: 0x06002840 RID: 10304 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002841 RID: 10305 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002842 RID: 10306 RVA: 0x000ABB46 File Offset: 0x000A9D46
		public void Reset()
		{
			this._idleCircleStyle = new GUIStyle
			{
				fontSize = 50,
				fontStyle = FontStyle.Bold
			};
		}

		// Token: 0x04002203 RID: 8707
		private const int IdleIndicatorFontSize = 50;

		// Token: 0x04002204 RID: 8708
		private const int IdleIndicatorFontInitialSize = 400;

		// Token: 0x04002205 RID: 8709
		[Dependency]
		private MotorwaysGame _motorwaysGame;

		// Token: 0x04002206 RID: 8710
		[Dependency]
		private GameCamera _camera;

		// Token: 0x04002207 RID: 8711
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x04002208 RID: 8712
		private GUIStyle _textStyle;

		// Token: 0x04002209 RID: 8713
		private GUIStyle _idleCircleStyle;
	}
}
