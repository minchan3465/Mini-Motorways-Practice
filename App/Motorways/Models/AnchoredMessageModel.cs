using System;
using Factory;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004CA RID: 1226
	public class AnchoredMessageModel : Model<EmptyModelFrame, AnchoredMessageModel.IObserver>
	{
		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001FFC RID: 8188 RVA: 0x0007E23F File Offset: 0x0007C43F
		// (set) Token: 0x06001FFD RID: 8189 RVA: 0x0007E247 File Offset: 0x0007C447
		public StringId Message { get; private set; }

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001FFE RID: 8190 RVA: 0x0007E250 File Offset: 0x0007C450
		// (set) Token: 0x06001FFF RID: 8191 RVA: 0x0007E258 File Offset: 0x0007C458
		public AnchoredMessageAnchorType AnchorType { get; private set; }

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06002000 RID: 8192 RVA: 0x0007E261 File Offset: 0x0007C461
		// (set) Token: 0x06002001 RID: 8193 RVA: 0x0007E269 File Offset: 0x0007C469
		public Vector2 Offset { get; private set; }

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06002002 RID: 8194 RVA: 0x0007E272 File Offset: 0x0007C472
		// (set) Token: 0x06002003 RID: 8195 RVA: 0x0007E27A File Offset: 0x0007C47A
		public Vector3 WorldAnchor { get; private set; }

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06002004 RID: 8196 RVA: 0x0007E283 File Offset: 0x0007C483
		// (set) Token: 0x06002005 RID: 8197 RVA: 0x0007E28B File Offset: 0x0007C48B
		public TileDirection Direction { get; private set; }

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06002006 RID: 8198 RVA: 0x0007E294 File Offset: 0x0007C494
		// (set) Token: 0x06002007 RID: 8199 RVA: 0x0007E29C File Offset: 0x0007C49C
		public UIMessageAnchor UIAnchor { get; private set; }

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06002008 RID: 8200 RVA: 0x0007E2A5 File Offset: 0x0007C4A5
		// (set) Token: 0x06002009 RID: 8201 RVA: 0x0007E2AD File Offset: 0x0007C4AD
		public Vector2 UIAnchorPivot { get; private set; }

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x0600200A RID: 8202 RVA: 0x0007E2B6 File Offset: 0x0007C4B6
		// (set) Token: 0x0600200B RID: 8203 RVA: 0x0007E2BE File Offset: 0x0007C4BE
		public CameraLayer CameraLayer { get; private set; }

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x0600200C RID: 8204 RVA: 0x0007E2C7 File Offset: 0x0007C4C7
		// (set) Token: 0x0600200D RID: 8205 RVA: 0x0007E2CF File Offset: 0x0007C4CF
		public bool ShowDismissArrow { get; set; }

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x0600200E RID: 8206 RVA: 0x0007E2D8 File Offset: 0x0007C4D8
		// (set) Token: 0x0600200F RID: 8207 RVA: 0x0007E2E0 File Offset: 0x0007C4E0
		public int? IntParameter { get; set; }

		// Token: 0x06002010 RID: 8208 RVA: 0x0007E2E9 File Offset: 0x0007C4E9
		public void InitializeWithScreenAnchor(StringId message, Vector2 screenOffset, CameraLayer cameraLayer = CameraLayer.Default, int? intParameter = null)
		{
			this.Message = message;
			this.AnchorType = AnchoredMessageAnchorType.Screen;
			this.Offset = screenOffset;
			this.CameraLayer = cameraLayer;
			this.IntParameter = intParameter;
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x0007E30F File Offset: 0x0007C50F
		public void InitializeWithWorldAnchor(StringId message, Vector3 worldAnchor, TileDirection direction)
		{
			this.Message = message;
			this.AnchorType = AnchoredMessageAnchorType.World;
			this.WorldAnchor = worldAnchor;
			this.Direction = direction;
			this.UIAnchor = UIMessageAnchor.None;
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x0007E334 File Offset: 0x0007C534
		public void InitializeWithUIAnchor(StringId message, UIMessageAnchor uiAnchor, Vector2 uiAnchorPivot)
		{
			this.Message = message;
			this.AnchorType = AnchoredMessageAnchorType.UI;
			this.UIAnchor = uiAnchor;
			this.UIAnchorPivot = uiAnchorPivot;
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x0007E354 File Offset: 0x0007C554
		public override void OnReleasedFromScope(IScope scope)
		{
			foreach (AnchoredMessageModel.IObserver observer in base.Observers)
			{
				observer.OnAnimationRelease();
			}
			base.OnReleasedFromScope(scope);
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x0007E38C File Offset: 0x0007C58C
		public override void Reset()
		{
			base.Reset();
			this.Message = StringId.None;
			this.AnchorType = AnchoredMessageAnchorType.Screen;
			this.Offset = default(Vector2);
			this.WorldAnchor = default(Vector3);
			this.Direction = TileDirection.North;
			this.UIAnchor = UIMessageAnchor.None;
			this.UIAnchorPivot = default(Vector2);
			this.CameraLayer = CameraLayer.Default;
			this.ShowDismissArrow = false;
			this.IntParameter = null;
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x0007E405 File Offset: 0x0007C605
		public AnchoredMessageModel() : base(1)
		{
		}

		// Token: 0x020004CB RID: 1227
		public interface IObserver
		{
			// Token: 0x06002016 RID: 8214
			void OnAnimationRelease();
		}
	}
}
