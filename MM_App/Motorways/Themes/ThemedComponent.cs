using System;
using Client;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Themes
{
	// Token: 0x02000478 RID: 1144
	[DisallowMultipleComponent]
	public class ThemedComponent : MonoBehaviour, IThemeComponent
	{
		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001C79 RID: 7289 RVA: 0x00069DBB File Offset: 0x00067FBB
		// (set) Token: 0x06001C7A RID: 7290 RVA: 0x00069DF8 File Offset: 0x00067FF8
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
			set
			{
				this._typeEnum = value;
			}
		}

		// Token: 0x06001C7B RID: 7291 RVA: 0x00069E01 File Offset: 0x00068001
		private void Awake()
		{
			this.AssignComponents();
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x00069E0C File Offset: 0x0006800C
		private bool AssignComponents()
		{
			if (this.componentType == ThemedComponent.ComponentType.Image)
			{
				if (this._image == null)
				{
					this._image = base.GetComponent<Image>();
				}
				this._areComponentsAssigned = (this._image != null);
				if (!this._areComponentsAssigned)
				{
					Diagnostics.FailAssert("Image hasn't been set on " + base.name + "!", Array.Empty<object>());
					return false;
				}
			}
			else if (this.componentType == ThemedComponent.ComponentType.Text)
			{
				if (this._textField == null)
				{
					this._textField = base.GetComponent<TMP_Text>();
				}
				this._areComponentsAssigned = (this._textField != null);
				if (!this._areComponentsAssigned)
				{
					Diagnostics.FailAssert("Text field hasn't been set on " + base.name + "!", Array.Empty<object>());
					return false;
				}
			}
			else if (this.componentType == ThemedComponent.ComponentType.ParticleSystem)
			{
				ParticleSystem particleSystem = base.GetComponent<ParticleSystem>();
				if (particleSystem != null && this._particleSystemRenderer == null)
				{
					this._particleSystemRenderer = particleSystem.GetComponent<Renderer>();
				}
				this._areComponentsAssigned = (this._particleSystemRenderer != null);
				if (!this._areComponentsAssigned)
				{
					Diagnostics.FailAssert("Particle system hasn't been set on " + base.name + "!", Array.Empty<object>());
					return false;
				}
			}
			else if (this.componentType == ThemedComponent.ComponentType.SpriteRenderer)
			{
				if (this._sprite == null)
				{
					this._sprite = base.GetComponent<SpriteRenderer>();
				}
				this._areComponentsAssigned = (this._sprite != null);
				if (!this._areComponentsAssigned)
				{
					Diagnostics.FailAssert("SpriteRenderer hasn't been set on " + base.name + "!", Array.Empty<object>());
					return false;
				}
			}
			else
			{
				this._areComponentsAssigned = true;
			}
			return true;
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x00069F9C File Offset: 0x0006819C
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this._database = (MotorwaysThemeDatabase)themeDatabase;
			if (this.componentType == ThemedComponent.ComponentType.Material)
			{
				foreach (Renderer r in base.GetComponentsInChildren<Renderer>())
				{
					this._database.materialCollection.BindRendererToThemeTarget(r, this.MaterialType);
				}
			}
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x00069FF0 File Offset: 0x000681F0
		public void ApplyTheme(ITheme theme)
		{
			if (this.componentType == ThemedComponent.ComponentType.Material)
			{
				return;
			}
			Color newColor = (this._database != null && this._database.ApplyThemeOverrides) ? (theme as Theme).GetDeleteModeColor(this.MaterialType, "_Color") : (theme as Theme).GetColor(this.MaterialType, "_Color");
			this.SetColor(newColor);
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x0006A054 File Offset: 0x00068254
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			if (this.componentType == ThemedComponent.ComponentType.Material)
			{
				return ThemeBlendingResult.StopBlending;
			}
			object obj = (this._database != null && this._database.IsBlendingOverrides && !this._database.ApplyThemeOverrides) ? (oldTheme as Theme).GetDeleteModeColor(this.MaterialType, "_Color") : (oldTheme as Theme).GetColor(this.MaterialType, "_Color");
			Color newColor = (this._database != null && this._database.ApplyThemeOverrides) ? (newTheme as Theme).GetDeleteModeColor(this.MaterialType, "_Color") : (newTheme as Theme).GetColor(this.MaterialType, "_Color");
			object obj2 = obj;
			Color color = Color.Lerp(obj2, newColor, progress);
			this.SetColor(color);
			if (!(obj2 == newColor))
			{
				return ThemeBlendingResult.ContinueBlending;
			}
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x0006A11C File Offset: 0x0006831C
		public void SetColor(Color color)
		{
			if (!this._areComponentsAssigned && !this.AssignComponents())
			{
				return;
			}
			if (this.componentType == ThemedComponent.ComponentType.Image)
			{
				if (this.maintainAlpha)
				{
					color.a = this._image.color.a;
				}
				this._image.color = color;
				return;
			}
			if (this.componentType == ThemedComponent.ComponentType.Text)
			{
				this._textField.color = color;
				return;
			}
			if (this.componentType == ThemedComponent.ComponentType.ParticleSystem)
			{
				this._particleSystemRenderer.sharedMaterial.color = color;
				return;
			}
			if (this.componentType == ThemedComponent.ComponentType.SpriteRenderer)
			{
				this._sprite.color = color;
			}
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x0006A1B4 File Offset: 0x000683B4
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
			MotorwaysThemeDatabase motorwaysThemeDatabase = (MotorwaysThemeDatabase)themeDatabase;
			if (this.componentType == ThemedComponent.ComponentType.Material)
			{
				foreach (Renderer r in base.GetComponentsInChildren<Renderer>())
				{
					motorwaysThemeDatabase.materialCollection.UnbindRendererFromThemeTarget(r, this.MaterialType);
				}
			}
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x0006A1FD File Offset: 0x000683FD
		public bool IsImage()
		{
			return this.componentType == ThemedComponent.ComponentType.Image;
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x0006A208 File Offset: 0x00068408
		public void SetMaterialType(ThemedMaterialType materialType)
		{
			this.type = materialType.ToString();
		}

		// Token: 0x0400181A RID: 6170
		private MotorwaysThemeDatabase _database;

		// Token: 0x0400181B RID: 6171
		[SerializeField]
		[StringEnumSearch(typeof(ThemedMaterialType))]
		private string type;

		// Token: 0x0400181C RID: 6172
		private ThemedMaterialType _typeEnum = ThemedMaterialType.Count;

		// Token: 0x0400181D RID: 6173
		public ThemedComponent.ComponentType componentType;

		// Token: 0x0400181E RID: 6174
		private Image _image;

		// Token: 0x0400181F RID: 6175
		private TMP_Text _textField;

		// Token: 0x04001820 RID: 6176
		private Renderer _particleSystemRenderer;

		// Token: 0x04001821 RID: 6177
		private SpriteRenderer _sprite;

		// Token: 0x04001822 RID: 6178
		private bool _areComponentsAssigned;

		// Token: 0x04001823 RID: 6179
		[ShowIf("IsImage")]
		public bool maintainAlpha;

		// Token: 0x02000479 RID: 1145
		public enum ComponentType
		{
			// Token: 0x04001825 RID: 6181
			Image,
			// Token: 0x04001826 RID: 6182
			Material,
			// Token: 0x04001827 RID: 6183
			Text,
			// Token: 0x04001828 RID: 6184
			ParticleSystem,
			// Token: 0x04001829 RID: 6185
			SpriteRenderer
		}
	}
}
