using System;
using System.Collections.Generic;
using Easing;
using Factory.Pools;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005C2 RID: 1474
	public class DynamicRoadMesh : MonoBehaviour, IReusable
	{
		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06002935 RID: 10549 RVA: 0x000B14E0 File Offset: 0x000AF6E0
		// (set) Token: 0x06002936 RID: 10550 RVA: 0x000B14F0 File Offset: 0x000AF6F0
		public bool HasEndCap
		{
			get
			{
				return this._outlineRenderer.numCapVertices > 0;
			}
			set
			{
				this._outlineRenderer.numCapVertices = (value ? this.EndCapResolution : 0);
				this._roadRenderer.numCapVertices = (value ? this.EndCapResolution : 0);
				this._cursorRenderer.numCapVertices = (value ? this.EndCapResolution : 0);
			}
		}

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06002937 RID: 10551 RVA: 0x000B1542 File Offset: 0x000AF742
		// (set) Token: 0x06002938 RID: 10552 RVA: 0x000B154C File Offset: 0x000AF74C
		public float CursorWidthFactor
		{
			get
			{
				return this._cursorWidthFactor;
			}
			set
			{
				this._cursorWidthFactor = value;
				float cursorWidth = value * 1.2f;
				this._cursorRenderer.startWidth = cursorWidth;
				this._cursorRenderer.endWidth = cursorWidth;
			}
		}

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06002939 RID: 10553 RVA: 0x000B1580 File Offset: 0x000AF780
		// (set) Token: 0x0600293A RID: 10554 RVA: 0x000B1588 File Offset: 0x000AF788
		public float OutlineWidthFactor
		{
			get
			{
				return this._outlineWidthFactor;
			}
			set
			{
				this._outlineWidthFactor = value;
				float outlineWidth = this._visualConstants.OutlineAppearCurve.Evaluate(value) * 1.2f;
				this._outlineRenderer.startWidth = outlineWidth;
				this._outlineRenderer.endWidth = outlineWidth;
			}
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x0600293B RID: 10555 RVA: 0x000B15CC File Offset: 0x000AF7CC
		// (set) Token: 0x0600293C RID: 10556 RVA: 0x000B15D4 File Offset: 0x000AF7D4
		public float RoadWidthFactor
		{
			get
			{
				return this._roadWidthFactor;
			}
			set
			{
				this._roadWidthFactor = value;
				float innerWidth = this._visualConstants.InnerAppearCurve.Evaluate(value) * 0.8f;
				this._roadRenderer.startWidth = innerWidth;
				this._roadRenderer.endWidth = innerWidth;
			}
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x0600293D RID: 10557 RVA: 0x000B1618 File Offset: 0x000AF818
		// (set) Token: 0x0600293E RID: 10558 RVA: 0x000B1620 File Offset: 0x000AF820
		public RoadState RoadState
		{
			get
			{
				return this._roadState;
			}
			set
			{
				this._roadState = value;
				if (this._roadState != RoadState.None)
				{
					this._roadRenderer.sharedMaterial = (((this._roadState & RoadState.VisiblyActive) != RoadState.None) ? this._activeMaterial : this._mothballedMaterial);
				}
			}
		}

		// Token: 0x0600293F RID: 10559 RVA: 0x000B1655 File Offset: 0x000AF855
		private void Awake()
		{
			this._materialPropertyBlock = new MaterialPropertyBlock();
		}

		// Token: 0x06002940 RID: 10560 RVA: 0x000B1662 File Offset: 0x000AF862
		public void Initialize(TileView tileView, PermanenceZoneTextureLibrary permanenceZoneTextureLibrary, bool hasPermanenceProgressView)
		{
			this._tileView = tileView;
			this._permanenceProgressRoadView = new PermanenceProgressRoadView(this._materialPropertyBlock, this._roadRenderer, this._tileView, permanenceZoneTextureLibrary, this._visualConstants, hasPermanenceProgressView);
			this.UpdatePermanenceShaderValues();
		}

		// Token: 0x06002941 RID: 10561 RVA: 0x000B1696 File Offset: 0x000AF896
		public void UpdatePermanenceShaderValues()
		{
			this._permanenceProgressRoadView.UpdatePermanenceValues();
		}

		// Token: 0x06002942 RID: 10562 RVA: 0x000B16A3 File Offset: 0x000AF8A3
		public void SetPermanenceVisibility(bool hasPermanenceProgressView)
		{
			PermanenceProgressRoadView permanenceProgressRoadView = this._permanenceProgressRoadView;
			if (permanenceProgressRoadView == null)
			{
				return;
			}
			permanenceProgressRoadView.SetPermanenceVisibility(hasPermanenceProgressView);
		}

		// Token: 0x06002943 RID: 10563 RVA: 0x000B16B8 File Offset: 0x000AF8B8
		public void SetPathPoints(List<Vector2> path)
		{
			this._cursorRenderer.positionCount = path.Count;
			this._outlineRenderer.positionCount = path.Count;
			this._roadRenderer.positionCount = path.Count;
			for (int pointIndex = 0; pointIndex < path.Count; pointIndex++)
			{
				this._cursorRenderer.SetPosition(pointIndex, path[pointIndex]);
				this._outlineRenderer.SetPosition(pointIndex, path[pointIndex]);
				this._roadRenderer.SetPosition(pointIndex, path[pointIndex]);
			}
		}

		// Token: 0x06002944 RID: 10564 RVA: 0x000B1751 File Offset: 0x000AF951
		public void SetCursorRendererHazardStripesAngle(float angle = 0f)
		{
			this._cursorRenderer.material.SetFloat(DynamicRoadMesh.ShaderRotationId, angle);
		}

		// Token: 0x06002945 RID: 10565 RVA: 0x000B176C File Offset: 0x000AF96C
		public void SetCursorRendererHazardStripesEnabled(bool stripesEnabled, bool tween)
		{
			float endAlpha = stripesEnabled ? 0f : 1f;
			if (tween)
			{
				float startAlpha = this._cursorRenderer.material.GetFloat(DynamicRoadMesh.MinimumAlphaId);
				this._alphaTween.Start(startAlpha, endAlpha, 0.3f, Easings.Functions.CubicEaseOut, 0f);
				return;
			}
			this._cursorRenderer.material.SetFloat(DynamicRoadMesh.MinimumAlphaId, endAlpha);
		}

		// Token: 0x06002946 RID: 10566 RVA: 0x000B17D1 File Offset: 0x000AF9D1
		private void Update()
		{
			if (this._alphaTween.IsActive)
			{
				this._cursorRenderer.material.SetFloat(DynamicRoadMesh.MinimumAlphaId, this._alphaTween.Tick(Time.deltaTime));
			}
		}

		// Token: 0x06002947 RID: 10567 RVA: 0x000B1808 File Offset: 0x000AFA08
		public void SetCursorRendererFadeout(float startFade, float endFade, float endAlpha)
		{
			Gradient gradient = new Gradient();
			gradient.SetKeys(this._cursorRenderer.colorGradient.colorKeys, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, startFade),
				new GradientAlphaKey(endAlpha, endFade)
			});
			this._cursorRenderer.colorGradient = gradient;
		}

		// Token: 0x06002948 RID: 10568 RVA: 0x000B187C File Offset: 0x000AFA7C
		public void Reset()
		{
			this._cursorWidthFactor = -1f;
			this._outlineWidthFactor = -1f;
			this._roadWidthFactor = -1f;
			this._roadState = RoadState.Active;
			this._roadRenderer.sharedMaterial = this._activeMaterial;
			this._permanenceProgressRoadView = null;
		}

		// Token: 0x040022E6 RID: 8934
		[SerializeField]
		private LineRenderer _outlineRenderer;

		// Token: 0x040022E7 RID: 8935
		[SerializeField]
		private LineRenderer _roadRenderer;

		// Token: 0x040022E8 RID: 8936
		[SerializeField]
		private LineRenderer _cursorRenderer;

		// Token: 0x040022E9 RID: 8937
		[SerializeField]
		private Material _activeMaterial;

		// Token: 0x040022EA RID: 8938
		[SerializeField]
		private Material _mothballedMaterial;

		// Token: 0x040022EB RID: 8939
		[SerializeField]
		private int EndCapResolution = 16;

		// Token: 0x040022EC RID: 8940
		[SerializeField]
		private VisualConstantsData _visualConstants;

		// Token: 0x040022ED RID: 8941
		private float _cursorWidthFactor = -1f;

		// Token: 0x040022EE RID: 8942
		private float _outlineWidthFactor = -1f;

		// Token: 0x040022EF RID: 8943
		private float _roadWidthFactor = -1f;

		// Token: 0x040022F0 RID: 8944
		private RoadState _roadState = RoadState.Active;

		// Token: 0x040022F1 RID: 8945
		private const float OutlineWidth = 1.2f;

		// Token: 0x040022F2 RID: 8946
		private const float InnerWidth = 0.8f;

		// Token: 0x040022F3 RID: 8947
		private static readonly int ShaderRotationId = Shader.PropertyToID("_Rotation");

		// Token: 0x040022F4 RID: 8948
		private static readonly int MinimumAlphaId = Shader.PropertyToID("_MinimumAlpha");

		// Token: 0x040022F5 RID: 8949
		private readonly TweenFloat _alphaTween = new TweenFloat();

		// Token: 0x040022F6 RID: 8950
		private PermanenceProgressRoadView _permanenceProgressRoadView;

		// Token: 0x040022F7 RID: 8951
		private TileView _tileView;

		// Token: 0x040022F8 RID: 8952
		private MaterialPropertyBlock _materialPropertyBlock;
	}
}
