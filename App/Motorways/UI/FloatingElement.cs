using System;
using Easing;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200074E RID: 1870
	[ExecuteAlways]
	public class FloatingElement : MonoBehaviour
	{
		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x06003451 RID: 13393 RVA: 0x000F64FB File Offset: 0x000F46FB
		public GameObject InactiveAnchor
		{
			get
			{
				return this._inactiveAnchor.gameObject;
			}
		}

		// Token: 0x06003452 RID: 13394 RVA: 0x000F6508 File Offset: 0x000F4708
		public void SetInactiveAnchor(Transform inactiveAnchor)
		{
			this._inactiveAnchor = inactiveAnchor;
		}

		// Token: 0x06003453 RID: 13395 RVA: 0x000F6511 File Offset: 0x000F4711
		private bool ShouldShowGraphics()
		{
			return !this.getAllChildGraphics && this.hideIfFallingBack;
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x06003454 RID: 13396 RVA: 0x000F6523 File Offset: 0x000F4723
		// (set) Token: 0x06003455 RID: 13397 RVA: 0x000F652B File Offset: 0x000F472B
		public bool IsAnimating { get; private set; }

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x06003456 RID: 13398 RVA: 0x000F6534 File Offset: 0x000F4734
		// (set) Token: 0x06003457 RID: 13399 RVA: 0x000F653C File Offset: 0x000F473C
		public bool IsActive { get; set; }

		// Token: 0x06003458 RID: 13400 RVA: 0x000F6548 File Offset: 0x000F4748
		private void Awake()
		{
			this._canvasGroup = base.GetComponent<CanvasGroup>();
			if (this.hideIfFallingBack)
			{
				if (this.getAllChildGraphics)
				{
					this.graphics = base.GetComponentsInChildren<Graphic>();
				}
				if ((this.graphics != null || this._canvasGroup != null) && !this.baseElement.activeInHierarchy)
				{
					base.transform.position = this.InactivePosition;
					this.SetGraphicsEnabled(false);
				}
			}
			this.ResetTimers();
			this.IsAnimating = false;
			this._isActivePositionUnstable = (this.baseElement.transform.parent.GetComponent<LayoutGroup>() != null);
		}

		// Token: 0x06003459 RID: 13401 RVA: 0x000F65E6 File Offset: 0x000F47E6
		private void OnEnable()
		{
			this.Snap();
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x0600345A RID: 13402 RVA: 0x000F65EE File Offset: 0x000F47EE
		public bool BaseElementActive
		{
			get
			{
				return this.baseElement.activeInHierarchy;
			}
		}

		// Token: 0x0600345B RID: 13403 RVA: 0x000F65FC File Offset: 0x000F47FC
		public void Snap()
		{
			this._wasDisabled = !this.baseElement.activeInHierarchy;
			base.transform.position = (this._wasDisabled ? this.InactivePosition : this.ActivePosition);
			if (this.hideIfFallingBack)
			{
				this.SetGraphicsEnabled(!this._wasDisabled);
			}
			this.ResetTimers();
			this.IsAnimating = false;
		}

		// Token: 0x0600345C RID: 13404 RVA: 0x000F6664 File Offset: 0x000F4864
		private void Update()
		{
			bool isBaseAnchorActive = this.movementControlledByScript ? this.IsActive : this.BaseElementActive;
			Vector3 origin;
			Vector3 destination;
			if (!isBaseAnchorActive)
			{
				origin = this.ActivePosition;
				destination = this.InactivePosition;
				if (!this._wasDisabled)
				{
					if (this._disappearTimer <= 0f || this.IsAnimating)
					{
						this.StartAnimation(this.IsAnimating ? this._interruptingAnimationOrigin : FloatingElement.AnimationOrigin.Canonical, this._inactiveAnimationDuration, this._inactiveAnimationEasing);
						this._wasDisabled = true;
						this.ResetTimers();
						this.onOptionTriggered.Invoke(false);
					}
					else
					{
						this.IsAnimating = false;
						this._disappearTimer -= Time.deltaTime;
					}
				}
				else if (!this.IsAnimating)
				{
					base.transform.position = destination;
				}
			}
			else
			{
				origin = this.InactivePosition;
				destination = this.ActivePosition;
				Vector3 localActivePosition = this.baseElement.transform.localPosition;
				if (this._isActivePositionUnstable)
				{
					this._lastKnownGoodActiveLocalPosition = localActivePosition;
				}
				if (this._wasDisabled)
				{
					if (this._appearTimer <= 0f || this.IsAnimating)
					{
						this.StartAnimation(this.IsAnimating ? this._interruptingAnimationOrigin : FloatingElement.AnimationOrigin.Canonical, this._activeAnimationDuration, this._activeAnimationEasing);
						this.SetGraphicsEnabled(true);
						this._wasDisabled = false;
						this.ResetTimers();
						this._disappearTimer = this._inactiveAnimationDelay;
						this.onOptionTriggered.Invoke(true);
					}
					else
					{
						this._appearTimer -= Time.deltaTime;
						this.IsAnimating = false;
						base.transform.position = origin;
					}
				}
				else if (!this.IsAnimating)
				{
					if (Vector3.SqrMagnitude(this._lastLocalActivePosition - localActivePosition) > 1f)
					{
						this.StartAnimation(FloatingElement.AnimationOrigin.WorldSpace, this._shuffleAnimationDuration, this._shuffleAnimationEasing);
					}
					else
					{
						base.transform.position = destination;
					}
				}
				this._lastLocalActivePosition = localActivePosition;
			}
			if (this.IsAnimating)
			{
				this._animationTime += Time.deltaTime;
				if (this._animationTime >= this._animationDuration)
				{
					base.transform.position = destination;
					this.ResetTimers();
					this.IsAnimating = false;
					if (!isBaseAnchorActive && this.hideIfFallingBack)
					{
						this.SetGraphicsEnabled(false);
						return;
					}
				}
				else
				{
					float t = Easings.Interpolate(this._animationTime / this._animationDuration, this._animationEasing);
					base.transform.position = Vector3.LerpUnclamped((this._animationOrigin == FloatingElement.AnimationOrigin.Canonical) ? origin : this._initialWorldPosition, destination, t);
				}
			}
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x0600345D RID: 13405 RVA: 0x000F68D8 File Offset: 0x000F4AD8
		private Vector3 ActivePosition
		{
			get
			{
				if (this._isActivePositionUnstable && !this.baseElement.activeInHierarchy)
				{
					return this.baseElement.transform.parent.localToWorldMatrix.MultiplyPoint(this._lastKnownGoodActiveLocalPosition);
				}
				return this.baseElement.transform.position;
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x0600345E RID: 13406 RVA: 0x000F692E File Offset: 0x000F4B2E
		private Vector3 InactivePosition
		{
			get
			{
				if (this.UseInactiveAchorForPosition())
				{
					return this._inactiveAnchor.position;
				}
				return this.ActivePosition + base.transform.rotation * this.fallbackOffset;
			}
		}

		// Token: 0x0600345F RID: 13407 RVA: 0x000F6965 File Offset: 0x000F4B65
		public bool UseInactiveAchorForPosition()
		{
			return this._inactiveAnchor != null;
		}

		// Token: 0x06003460 RID: 13408 RVA: 0x000F6973 File Offset: 0x000F4B73
		private void StartAnimation(FloatingElement.AnimationOrigin origin, float duration, Easings.Functions easing)
		{
			this._animationOrigin = origin;
			this._initialWorldPosition = base.transform.position;
			this.IsAnimating = true;
			this._animationTime = 0f;
			this._animationDuration = duration;
			this._animationEasing = easing;
		}

		// Token: 0x06003461 RID: 13409 RVA: 0x000F69AD File Offset: 0x000F4BAD
		private void ResetTimers()
		{
			this._appearTimer = Mathf.Max(0.01f, this.delayBeforeAppearing);
			this._disappearTimer = this._inactiveAnimationDelay;
		}

		// Token: 0x06003462 RID: 13410 RVA: 0x000F69D4 File Offset: 0x000F4BD4
		private void SetGraphicsEnabled(bool isEnabled)
		{
			Graphic[] array = this.graphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = isEnabled;
			}
			if (this._canvasGroup != null)
			{
				this._canvasGroup.alpha = (float)(isEnabled ? 1 : 0);
			}
		}

		// Token: 0x04002C98 RID: 11416
		public GameObject baseElement;

		// Token: 0x04002C99 RID: 11417
		[DisableIf("UseInactiveAchorForPosition")]
		public Vector3 fallbackOffset;

		// Token: 0x04002C9A RID: 11418
		public float delayBeforeAppearing;

		// Token: 0x04002C9B RID: 11419
		[SerializeField]
		private float _activeAnimationDuration = 1f;

		// Token: 0x04002C9C RID: 11420
		[SerializeField]
		private Easings.Functions _activeAnimationEasing;

		// Token: 0x04002C9D RID: 11421
		[SerializeField]
		private Transform _inactiveAnchor;

		// Token: 0x04002C9E RID: 11422
		[EnableIf("UseInactiveAchorForPosition")]
		[SerializeField]
		private float _inactiveAnimationDelay;

		// Token: 0x04002C9F RID: 11423
		[SerializeField]
		[EnableIf("UseInactiveAchorForPosition")]
		private float _inactiveAnimationDuration = 1f;

		// Token: 0x04002CA0 RID: 11424
		[SerializeField]
		[EnableIf("UseInactiveAchorForPosition")]
		private Easings.Functions _inactiveAnimationEasing;

		// Token: 0x04002CA1 RID: 11425
		[Tooltip("The shuffle animation is played when the element is already visible and not animating, and has to move to a new position. It is only used by the upgrade icons.")]
		[SerializeField]
		private float _shuffleAnimationDuration = 0.5f;

		// Token: 0x04002CA2 RID: 11426
		[SerializeField]
		private Easings.Functions _shuffleAnimationEasing;

		// Token: 0x04002CA3 RID: 11427
		[Tooltip("If set, the associated graphic will be hidden when the floating element has completed its deactivation animation.")]
		public bool hideIfFallingBack = true;

		// Token: 0x04002CA4 RID: 11428
		public bool movementControlledByScript;

		// Token: 0x04002CA5 RID: 11429
		[EnableIf("hideIfFallingBack")]
		public bool getAllChildGraphics;

		// Token: 0x04002CA6 RID: 11430
		[EnableIf("ShouldShowGraphics")]
		public Graphic[] graphics;

		// Token: 0x04002CA7 RID: 11431
		[Tooltip("If the active anchor's visibility is toggled while the element is already animating, should it animate from the world-space position, or snap to the canonical position? Canonical is reliable, but world-space looks nicer and avoid snaps if you can guarantee the stability of the anchors.")]
		[SerializeField]
		private FloatingElement.AnimationOrigin _interruptingAnimationOrigin;

		// Token: 0x04002CA8 RID: 11432
		public FloatingElement.HiddenTrigger onOptionTriggered = new FloatingElement.HiddenTrigger();

		// Token: 0x04002CA9 RID: 11433
		private CanvasGroup _canvasGroup;

		// Token: 0x04002CAA RID: 11434
		private bool _wasDisabled = true;

		// Token: 0x04002CAB RID: 11435
		private float _appearTimer;

		// Token: 0x04002CAC RID: 11436
		private float _disappearTimer;

		// Token: 0x04002CAD RID: 11437
		private bool _isActivePositionUnstable;

		// Token: 0x04002CAE RID: 11438
		private Vector3 _lastKnownGoodActiveLocalPosition;

		// Token: 0x04002CAF RID: 11439
		private Vector3 _lastLocalActivePosition;

		// Token: 0x04002CB1 RID: 11441
		private float _animationTime;

		// Token: 0x04002CB2 RID: 11442
		private float _animationDuration;

		// Token: 0x04002CB3 RID: 11443
		private FloatingElement.AnimationOrigin _animationOrigin;

		// Token: 0x04002CB4 RID: 11444
		private Vector3 _initialWorldPosition;

		// Token: 0x04002CB5 RID: 11445
		private Easings.Functions _animationEasing;

		// Token: 0x04002CB7 RID: 11447
		private const float DistanceTolerance = 1f;

		// Token: 0x04002CB8 RID: 11448
		private const float MinAppearDelay = 0.01f;

		// Token: 0x0200074F RID: 1871
		private enum AnimationOrigin
		{
			// Token: 0x04002CBA RID: 11450
			Canonical,
			// Token: 0x04002CBB RID: 11451
			WorldSpace
		}

		// Token: 0x02000750 RID: 1872
		[Serializable]
		public class HiddenTrigger : UnityEvent<bool>
		{
		}
	}
}
