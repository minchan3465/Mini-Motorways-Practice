using System;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005AA RID: 1450
	public class OnScreenDebugToolsActivator : MonoBehaviour, IReusable
	{
		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x0600286E RID: 10350 RVA: 0x000AC543 File Offset: 0x000AA743
		// (set) Token: 0x0600286F RID: 10351 RVA: 0x000AC54B File Offset: 0x000AA74B
		public bool AreToolsActive { get; private set; }

		// Token: 0x06002870 RID: 10352 RVA: 0x000AC554 File Offset: 0x000AA754
		private void Awake()
		{
			if (!FeatureToggle.IsFeatureEnabled(Feature.OnScreenDebugTools))
			{
				base.enabled = false;
			}
		}

		// Token: 0x06002871 RID: 10353 RVA: 0x000AC568 File Offset: 0x000AA768
		private void OnGUI()
		{
			if (!FeatureToggle.IsFeatureEnabled(Feature.OnScreenDebugTools))
			{
				return;
			}
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / (float)OnScreenDebugToolsActivator.BaseResolution.x, (float)Screen.height / (float)OnScreenDebugToolsActivator.BaseResolution.y, 1f));
			if (GUI.Button(new Rect(0f, (float)OnScreenDebugToolsActivator.BaseResolution.y - 200f, 200f, 200f), "", GUIStyle.none))
			{
				if (this._lastHitTime <= -3.4028235E+38f)
				{
					this._hitCount++;
					this._lastHitTime = Time.time;
				}
				else if (Time.time - this._lastHitTime < 0.3f)
				{
					this._hitCount++;
					this._lastHitTime = Time.time;
					if (this._hitCount >= 5)
					{
						this._hitCount = 0;
						this._lastHitTime = float.MinValue;
						this.AreToolsActive = !this.AreToolsActive;
						OnScreenDebugToolsActivator.ActivationStatusChange activationStatusChange = this.onActivationStatusChanged;
						if (activationStatusChange != null)
						{
							activationStatusChange(this.AreToolsActive);
						}
					}
				}
				else
				{
					this._hitCount = 1;
					this._lastHitTime = float.MinValue;
				}
			}
			GUI.matrix = matrix;
		}

		// Token: 0x06002872 RID: 10354 RVA: 0x000AC6B9 File Offset: 0x000AA8B9
		public void Reset()
		{
			this._hitCount = 0;
			this._lastHitTime = float.MinValue;
			this.AreToolsActive = false;
		}

		// Token: 0x0400221F RID: 8735
		private static readonly Vector2Int BaseResolution = new Vector2Int(1920, 1080);

		// Token: 0x04002220 RID: 8736
		private const int HitCountBeforeActivation = 5;

		// Token: 0x04002221 RID: 8737
		private const float ActivationAreaSize = 200f;

		// Token: 0x04002222 RID: 8738
		private const float MaxTimeBetweenHitsInSeconds = 0.3f;

		// Token: 0x04002223 RID: 8739
		private float _lastHitTime = float.MinValue;

		// Token: 0x04002224 RID: 8740
		private int _hitCount;

		// Token: 0x04002226 RID: 8742
		public OnScreenDebugToolsActivator.ActivationStatusChange onActivationStatusChanged;

		// Token: 0x020005AB RID: 1451
		// (Invoke) Token: 0x06002876 RID: 10358
		public delegate void ActivationStatusChange(bool isActive);
	}
}
