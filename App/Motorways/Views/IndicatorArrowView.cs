using System;
using Client;
using Factory;
using Factory.Pools;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005CB RID: 1483
	public class IndicatorArrowView : MonoBehaviour, IView, IReusable
	{
		// Token: 0x0600298A RID: 10634 RVA: 0x000B23CC File Offset: 0x000B05CC
		private void Initialise(DestinationView destinationView, IndicatorArrowView.IndicatorType indicatorType, RectTransform safeAreaTransform, int knockNumber, float knockDelay, float exitDelay)
		{
			this._iconPlus.SetActive(indicatorType == IndicatorArrowView.IndicatorType.NewBuilding);
			this._iconCircleOutline.SetActive(indicatorType == IndicatorArrowView.IndicatorType.DestinationUpgrade);
			this._iconCircleFill.SetActive(indicatorType == IndicatorArrowView.IndicatorType.DestinationBigPin);
			this._iconAlert.SetActive(indicatorType == IndicatorArrowView.IndicatorType.DestinationImminentFail);
			if (indicatorType == IndicatorArrowView.IndicatorType.DestinationImminentFail)
			{
				this._pinInside.color = Color.black;
			}
			else
			{
				this._pinInside.color = destinationView.GetBuildingColor(ThemeComponentGroupTarget.BuildingBase);
			}
			this._safeAreaRect = safeAreaTransform;
			this._targetBounds = destinationView.GetBounds();
			this._targetPositionOnBounds = this._targetBounds.center;
			this._state = IndicatorArrowView.State.Intro;
			this._knockDelay = knockDelay;
			this._knockNumber = knockNumber;
			this._exitDelay = exitDelay;
			this.ClampPosition();
		}

		// Token: 0x0600298B RID: 10635 RVA: 0x000B2488 File Offset: 0x000B0688
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			this.ClampPosition();
			if (this._state != IndicatorArrowView.State.Exit && !this._cameraView.playerZoomedIn)
			{
				this.SetState(IndicatorArrowView.State.Exit);
			}
			if (this._state == IndicatorArrowView.State.Intro)
			{
				this.TickIntro();
			}
			else if (this._state == IndicatorArrowView.State.Idle)
			{
				this.TickIdle(timeInterval.Delta);
			}
			else if (this._state == IndicatorArrowView.State.Exit)
			{
				return this.TickExit();
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x0600298C RID: 10636 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600298D RID: 10637 RVA: 0x000B24F0 File Offset: 0x000B06F0
		private void ClampPosition()
		{
			Camera defaultCamera = this._gameCamera.DefaultCamera;
			this._safeAreaRect.GetWorldCorners(this._safeAreaWorldCorners);
			Vector3 minV = defaultCamera.WorldToScreenPoint(this._safeAreaWorldCorners[0]);
			Vector3 maxV = defaultCamera.WorldToScreenPoint(this._safeAreaWorldCorners[2]);
			Rect screenRect = Rect.MinMaxRect(minV.x, minV.y, maxV.x, maxV.y);
			Vector3 screenTargetMin = defaultCamera.WorldToScreenPoint(this._targetBounds.min);
			Vector3 screenTargetMax = defaultCamera.WorldToScreenPoint(this._targetBounds.max);
			Rect screenTargetRect = Rect.MinMaxRect(screenTargetMin.x, screenTargetMin.y, screenTargetMax.x, screenTargetMax.y);
			Rect screenOuterBoundary = this.GetBoundaryRect(ref screenRect, 0.12f);
			bool flag = !screenTargetRect.Overlaps(screenRect);
			Vector3 screenTargetPositionOnBounds;
			if (flag)
			{
				screenTargetPositionOnBounds = IndicatorArrowView.GetTargetScreenPositionOnBounds(screenTargetRect, screenRect, screenTargetMin.z);
				this._targetPositionOnBounds = defaultCamera.ScreenToWorldPoint(screenTargetPositionOnBounds);
			}
			else
			{
				screenTargetPositionOnBounds = defaultCamera.WorldToScreenPoint(this._targetPositionOnBounds);
			}
			Vector3 clampedScreenPosition = new Vector3(Mathf.Clamp(screenTargetPositionOnBounds.x, screenOuterBoundary.xMin, screenOuterBoundary.xMax), Mathf.Clamp(screenTargetPositionOnBounds.y, screenOuterBoundary.yMin, screenOuterBoundary.yMax), screenTargetPositionOnBounds.z);
			Vector3 clampedWorldPos = defaultCamera.ScreenToWorldPoint(clampedScreenPosition);
			base.transform.position = clampedWorldPos;
			if (flag)
			{
				Vector3 toTarget = this._targetPositionOnBounds - clampedWorldPos;
				float degrees = Mathf.Atan2(toTarget.y, toTarget.x) * 57.29578f + 90f;
				Vector3 mainRotation = new Vector3(0f, 0f, degrees);
				base.transform.rotation = Quaternion.Euler(mainRotation);
				this._iconsTransform.rotation = Quaternion.identity;
				return;
			}
			if (this._state != IndicatorArrowView.State.Exit && this.GetBoundaryRect(ref screenRect, 0.17f).Contains(clampedScreenPosition))
			{
				this.SetState(IndicatorArrowView.State.Exit);
			}
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x000B26E0 File Offset: 0x000B08E0
		private static Vector3 GetTargetScreenPositionOnBounds(Rect screenTargetRect, Rect screenRect, float zDepth)
		{
			Vector2 screenTargetCenter = screenTargetRect.center;
			Vector3 targetPosition = new Vector3(screenTargetCenter.x, screenTargetCenter.y, zDepth);
			if (screenTargetRect.min.x > screenRect.xMax)
			{
				targetPosition.x = screenTargetRect.min.x;
			}
			else if (screenTargetRect.max.x < screenRect.xMin)
			{
				targetPosition.x = screenTargetRect.max.x;
			}
			if (screenTargetRect.min.y > screenRect.yMax)
			{
				targetPosition.y = screenTargetRect.min.y;
			}
			else if (screenTargetRect.max.y < screenRect.yMin)
			{
				targetPosition.y = screenTargetRect.max.y;
			}
			return targetPosition;
		}

		// Token: 0x0600298F RID: 10639 RVA: 0x000B27B0 File Offset: 0x000B09B0
		private Rect GetBoundaryRect(ref Rect screenRect, float gapPercent)
		{
			float innerBoundaryGapWidth = screenRect.width * gapPercent;
			float innerBoundaryGapHeight = screenRect.height * gapPercent;
			return Rect.MinMaxRect(screenRect.xMin + innerBoundaryGapWidth, screenRect.yMin + innerBoundaryGapHeight, screenRect.xMax - innerBoundaryGapWidth, screenRect.yMax - innerBoundaryGapHeight);
		}

		// Token: 0x06002990 RID: 10640 RVA: 0x000B27F4 File Offset: 0x000B09F4
		private void SetState(IndicatorArrowView.State newState)
		{
			if (newState == this._state)
			{
				return;
			}
			this._state = newState;
			if (this._state != IndicatorArrowView.State.Intro)
			{
				if (this._state == IndicatorArrowView.State.Idle)
				{
					this._timeUntilKnock = this._knockDelay;
					return;
				}
				if (this._state == IndicatorArrowView.State.Exit)
				{
					this._animator.SetTrigger(IndicatorArrowView.AnimatorExitHash);
				}
			}
		}

		// Token: 0x06002991 RID: 10641 RVA: 0x000B2849 File Offset: 0x000B0A49
		private void TickIntro()
		{
			if (this.IsInAnimState(IndicatorArrowView.AnimatorIdleHash))
			{
				this.SetState(IndicatorArrowView.State.Idle);
			}
		}

		// Token: 0x06002992 RID: 10642 RVA: 0x000B2860 File Offset: 0x000B0A60
		private void TickIdle(float tickTime)
		{
			if (!this.IsInAnimState(IndicatorArrowView.AnimatorKnockHash) && this._timeUntilKnock >= 0f)
			{
				this._timeUntilKnock -= tickTime;
				if (this._timeUntilKnock < 0f)
				{
					this._animator.SetTrigger(IndicatorArrowView.AnimatorKnockHash);
					this._knockNumber--;
					if (this._knockNumber > 0)
					{
						this._timeUntilKnock += this._knockDelay;
					}
				}
			}
			if (this._exitDelay >= 0f)
			{
				this._exitDelay -= tickTime;
				if (this._exitDelay < 0f)
				{
					this.SetState(IndicatorArrowView.State.Exit);
				}
			}
		}

		// Token: 0x06002993 RID: 10643 RVA: 0x000B290C File Offset: 0x000B0B0C
		private TickResult TickExit()
		{
			AnimatorStateInfo stateInfo = this._animator.GetCurrentAnimatorStateInfo(0);
			if (stateInfo.shortNameHash == IndicatorArrowView.AnimatorExitHash && stateInfo.normalizedTime >= 1f)
			{
				return TickResult.Destroy;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002994 RID: 10644 RVA: 0x000B2950 File Offset: 0x000B0B50
		private bool IsInAnimState(int stateHash)
		{
			return this._animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
		}

		// Token: 0x06002995 RID: 10645 RVA: 0x000B2974 File Offset: 0x000B0B74
		public void Reset()
		{
			this.ClearIcon();
			this._targetBounds = default(Bounds);
			this._targetPositionOnBounds = Vector3.zero;
			base.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			this.SetState(IndicatorArrowView.State.Intro);
			this._timeUntilKnock = 0f;
			this._knockDelay = 0f;
			this._knockNumber = 0;
			this._exitDelay = 0f;
		}

		// Token: 0x06002996 RID: 10646 RVA: 0x000B29E2 File Offset: 0x000B0BE2
		private void ClearIcon()
		{
			this._iconPlus.SetActive(false);
			this._iconCircleOutline.SetActive(false);
			this._iconCircleFill.SetActive(false);
			this._iconAlert.SetActive(false);
		}

		// Token: 0x06002997 RID: 10647 RVA: 0x000B2A14 File Offset: 0x000B0C14
		public static IndicatorArrowView Create(ViewClient client, DestinationView destinationView, IndicatorArrowView.IndicatorType type, RectTransform safeAreaRect, int knockNumber, float knockDelay, float exitDelay)
		{
			IndicatorArrowView newArrow = client.Scope.Get<IndicatorArrowView>();
			newArrow.Initialise(destinationView, type, safeAreaRect, knockNumber, knockDelay, exitDelay);
			client.AddView(newArrow);
			return newArrow;
		}

		// Token: 0x04002338 RID: 9016
		[SerializeField]
		private Animator _animator;

		// Token: 0x04002339 RID: 9017
		[SerializeField]
		private SpriteRenderer _pinInside;

		// Token: 0x0400233A RID: 9018
		[SerializeField]
		private Transform _iconsTransform;

		// Token: 0x0400233B RID: 9019
		[SerializeField]
		private GameObject _iconPlus;

		// Token: 0x0400233C RID: 9020
		[SerializeField]
		private GameObject _iconCircleOutline;

		// Token: 0x0400233D RID: 9021
		[SerializeField]
		private GameObject _iconCircleFill;

		// Token: 0x0400233E RID: 9022
		[SerializeField]
		private GameObject _iconAlert;

		// Token: 0x0400233F RID: 9023
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002340 RID: 9024
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002341 RID: 9025
		private RectTransform _safeAreaRect;

		// Token: 0x04002342 RID: 9026
		private Bounds _targetBounds;

		// Token: 0x04002343 RID: 9027
		private Vector3 _targetPositionOnBounds;

		// Token: 0x04002344 RID: 9028
		private Vector3[] _safeAreaWorldCorners = new Vector3[4];

		// Token: 0x04002345 RID: 9029
		private IndicatorArrowView.State _state;

		// Token: 0x04002346 RID: 9030
		private float _timeUntilKnock;

		// Token: 0x04002347 RID: 9031
		private float _knockDelay;

		// Token: 0x04002348 RID: 9032
		private int _knockNumber;

		// Token: 0x04002349 RID: 9033
		private float _exitDelay;

		// Token: 0x0400234A RID: 9034
		private static readonly int AnimatorKnockHash = Animator.StringToHash("Knock");

		// Token: 0x0400234B RID: 9035
		private static readonly int AnimatorExitHash = Animator.StringToHash("Exit");

		// Token: 0x0400234C RID: 9036
		private static readonly int AnimatorIdleHash = Animator.StringToHash("Idle");

		// Token: 0x0400234D RID: 9037
		private const float InnerBoundaryPercent = 0.17f;

		// Token: 0x0400234E RID: 9038
		private const float OuterBoundaryPercent = 0.12f;

		// Token: 0x020005CC RID: 1484
		private enum State
		{
			// Token: 0x04002350 RID: 9040
			Intro,
			// Token: 0x04002351 RID: 9041
			Idle,
			// Token: 0x04002352 RID: 9042
			Exit
		}

		// Token: 0x020005CD RID: 1485
		public enum IndicatorType
		{
			// Token: 0x04002354 RID: 9044
			NewBuilding,
			// Token: 0x04002355 RID: 9045
			DestinationUpgrade,
			// Token: 0x04002356 RID: 9046
			DestinationBigPin,
			// Token: 0x04002357 RID: 9047
			DestinationImminentFail
		}
	}
}
