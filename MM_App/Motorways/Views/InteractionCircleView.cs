using System;
using Client;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005CF RID: 1487
	public class InteractionCircleView : MonoBehaviour, IThemeComponent
	{
		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x060029A2 RID: 10658 RVA: 0x000B2C55 File Offset: 0x000B0E55
		public ThemedMaterialType MaterialType
		{
			get
			{
				if (this._typeEnum == ThemedMaterialType.Count && !Diagnostics.Verify(this.type.TryParse(out this._typeEnum), "{0} isn't a valid ThemedMaterialType!", this.type))
				{
					this._typeEnum = ThemedMaterialType.Land;
				}
				return this._typeEnum;
			}
		}

		// Token: 0x060029A3 RID: 10659 RVA: 0x000B2C92 File Offset: 0x000B0E92
		private void Awake()
		{
			this._spritePropertyBlock = new MaterialPropertyBlock();
		}

		// Token: 0x060029A4 RID: 10660 RVA: 0x000B2CA0 File Offset: 0x000B0EA0
		public void SetPermanenceProgress(float modelProgress)
		{
			float progress = modelProgress;
			if (modelProgress >= 1f)
			{
				progress = 1.1f;
			}
			this._sprite.GetPropertyBlock(this._spritePropertyBlock);
			this._spritePropertyBlock.SetFloat(InteractionCircleView.InteractionCircleProgressPropertyId, progress);
			this._sprite.SetPropertyBlock(this._spritePropertyBlock);
			if (this._previousProgress < 1f && modelProgress >= 1f && this._previousProgress != 0f)
			{
				this.PlayPermanenceCompleteAnimation();
			}
			this._previousProgress = modelProgress;
		}

		// Token: 0x060029A5 RID: 10661 RVA: 0x000B2D1F File Offset: 0x000B0F1F
		private void PlayPermanenceCompleteAnimation()
		{
			this._animator.SetTrigger(InteractionCircleView.InteractionCircleProgressCompleteAnimatorTrigger);
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x000B2D34 File Offset: 0x000B0F34
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this._database = (MotorwaysThemeDatabase)themeDatabase;
			this._database.materialCollection.BindRendererToThemeTarget(this._sprite, this.MaterialType);
			this._sprite.GetPropertyBlock(this._spritePropertyBlock);
			this._spritePropertyBlock.SetTexture(InteractionCircleView.MainTexturePropertyId, this._sprite.sprite.texture);
			this._sprite.SetPropertyBlock(this._spritePropertyBlock);
		}

		// Token: 0x060029A7 RID: 10663 RVA: 0x000B2DAB File Offset: 0x000B0FAB
		public void ApplyTheme(ITheme theme)
		{
			this.SetPermanenceProgress(this._previousProgress);
		}

		// Token: 0x060029A8 RID: 10664 RVA: 0x000B2DB9 File Offset: 0x000B0FB9
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			this.SetPermanenceProgress(this._previousProgress);
			return ThemeBlendingResult.ContinueBlending;
		}

		// Token: 0x060029A9 RID: 10665 RVA: 0x000B2DC8 File Offset: 0x000B0FC8
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			((MotorwaysThemeDatabase)themeDatabase).materialCollection.UnbindRendererFromThemeTarget(this._sprite, this.MaterialType);
		}

		// Token: 0x04002363 RID: 9059
		[SerializeField]
		private SpriteRenderer _sprite;

		// Token: 0x04002364 RID: 9060
		[SerializeField]
		private Animator _animator;

		// Token: 0x04002365 RID: 9061
		private static readonly int InteractionCircleProgressPropertyId = Shader.PropertyToID("_Progress");

		// Token: 0x04002366 RID: 9062
		private static readonly int MainTexturePropertyId = Shader.PropertyToID("_MainTex");

		// Token: 0x04002367 RID: 9063
		private static readonly int InteractionCircleProgressCompleteAnimatorTrigger = Animator.StringToHash("Bounce");

		// Token: 0x04002368 RID: 9064
		private float _previousProgress;

		// Token: 0x04002369 RID: 9065
		private MotorwaysThemeDatabase _database;

		// Token: 0x0400236A RID: 9066
		private MaterialPropertyBlock _spritePropertyBlock;

		// Token: 0x0400236B RID: 9067
		[SerializeField]
		[StringEnumSearch(typeof(ThemedMaterialType))]
		private string type;

		// Token: 0x0400236C RID: 9068
		private ThemedMaterialType _typeEnum = ThemedMaterialType.Count;
	}
}
