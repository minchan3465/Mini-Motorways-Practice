using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005AF RID: 1455
	public class OnScreenToolManager : MonoBehaviour, IOnScreenToolManager, ICreatedInScopeHandler
	{
		// Token: 0x06002887 RID: 10375 RVA: 0x000AC554 File Offset: 0x000AA754
		private void Awake()
		{
			if (!FeatureToggle.IsFeatureEnabled(Feature.OnScreenDebugTools))
			{
				base.enabled = false;
			}
		}

		// Token: 0x06002888 RID: 10376 RVA: 0x000AD1FC File Offset: 0x000AB3FC
		public void Initialize(OnScreenDebugToolsActivator debugToolsActivator)
		{
			this._debugToolsActivator = debugToolsActivator;
			base.enabled = this._debugToolsActivator.AreToolsActive;
			OnScreenDebugToolsActivator debugToolsActivator2 = this._debugToolsActivator;
			debugToolsActivator2.onActivationStatusChanged = (OnScreenDebugToolsActivator.ActivationStatusChange)Delegate.Combine(debugToolsActivator2.onActivationStatusChanged, new OnScreenDebugToolsActivator.ActivationStatusChange(delegate(bool isEnabled)
			{
				base.enabled = isEnabled;
			}));
		}

		// Token: 0x06002889 RID: 10377 RVA: 0x000AD248 File Offset: 0x000AB448
		public void OnCreatedInScope(IScope scope)
		{
			this._activeTools.Add(new OnScreenDebugRenderTool());
			this._activeTools.Add(new OnScreenSaveTool(scope));
		}

		// Token: 0x0600288A RID: 10378 RVA: 0x000AD26C File Offset: 0x000AB46C
		private void OnGUI()
		{
			if (this._resolution.x != Screen.width || this._resolution.y != Screen.height)
			{
				this._resolution = new Vector2Int(Screen.width, Screen.height);
				Vector3 scalingFactors = new Vector3((float)Screen.width / (float)OnScreenToolManager.BaseResolution.x, (float)Screen.height / (float)OnScreenToolManager.BaseResolution.y, 1f);
				this._scalingMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scalingFactors);
				this._inverseScalingMatrix = this._scalingMatrix.inverse;
			}
			Matrix4x4 oldMatrix = GUI.matrix;
			GUI.matrix = this._scalingMatrix;
			foreach (IOnScreenTool onScreenTool in this._activeTools)
			{
				onScreenTool.OnGUI(this._scope);
			}
			GUI.matrix = oldMatrix;
		}

		// Token: 0x0600288B RID: 10379 RVA: 0x000AD370 File Offset: 0x000AB570
		private void Update()
		{
			foreach (IOnScreenTool onScreenTool in this._activeTools)
			{
				onScreenTool.Update();
			}
		}

		// Token: 0x0600288C RID: 10380 RVA: 0x000AD3C0 File Offset: 0x000AB5C0
		public bool IsPointInsideTool(Vector2 coordinates)
		{
			if (!base.enabled)
			{
				return false;
			}
			coordinates.y = (float)Screen.height - coordinates.y;
			coordinates = this._inverseScalingMatrix * coordinates;
			using (List<IOnScreenTool>.Enumerator enumerator = this._activeTools.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.InputBlockingRect.Contains(coordinates))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0400224F RID: 8783
		[Dependency]
		private IScope _scope;

		// Token: 0x04002250 RID: 8784
		private static readonly Vector2Int BaseResolution = new Vector2Int(1920, 1080);

		// Token: 0x04002251 RID: 8785
		private readonly List<IOnScreenTool> _activeTools = new List<IOnScreenTool>();

		// Token: 0x04002252 RID: 8786
		private OnScreenDebugToolsActivator _debugToolsActivator;

		// Token: 0x04002253 RID: 8787
		private Matrix4x4 _scalingMatrix = Matrix4x4.identity;

		// Token: 0x04002254 RID: 8788
		private Matrix4x4 _inverseScalingMatrix = Matrix4x4.identity;

		// Token: 0x04002255 RID: 8789
		private Vector2Int _resolution = Vector2Int.zero;
	}
}
