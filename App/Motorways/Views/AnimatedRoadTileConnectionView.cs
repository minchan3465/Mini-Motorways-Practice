using System;
using Easing;
using Factory;
using Factory.Pools;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200057F RID: 1407
	public class AnimatedRoadTileConnectionView : MonoBehaviour, IReusable
	{
		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x060026AA RID: 9898 RVA: 0x000A4A9C File Offset: 0x000A2C9C
		public RoadTileConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x060026AB RID: 9899 RVA: 0x000A4AA4 File Offset: 0x000A2CA4
		// (set) Token: 0x060026AC RID: 9900 RVA: 0x000A4AAC File Offset: 0x000A2CAC
		public RoadAnimationDirection AnimationDirection
		{
			get
			{
				return this._animationDirection;
			}
			set
			{
				if (value == this._animationDirection || value == RoadAnimationDirection.None)
				{
					return;
				}
				this._animationDirection = value;
				if (this._animationDirection == RoadAnimationDirection.AnimatingIn)
				{
					this._outlineWidthTween.Start(this._outlineWidthFactor, 0f, 1f, this._visualConstants.AppearDuration);
					this._roadWidthTween.Start(this._roadWidthFactor, 0f, 1f, this._visualConstants.AppearDuration);
					return;
				}
				this._outlineWidthTween.Start(this._outlineWidthFactor, 1f, 0f, this._visualConstants.DisappearDuration);
				this._roadWidthTween.Start(this._roadWidthFactor, 1f, 0f, this._visualConstants.DisappearDuration);
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060026AD RID: 9901 RVA: 0x000A4B6F File Offset: 0x000A2D6F
		// (set) Token: 0x060026AE RID: 9902 RVA: 0x000A4B7C File Offset: 0x000A2D7C
		public RoadState RoadState
		{
			get
			{
				return this._dynamicRoadMesh.RoadState;
			}
			set
			{
				this._dynamicRoadMesh.RoadState = value;
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x060026AF RID: 9903 RVA: 0x000A4B8A File Offset: 0x000A2D8A
		public float RoadWidthFactor
		{
			get
			{
				return this._roadWidthFactor;
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060026B0 RID: 9904 RVA: 0x000A4B92 File Offset: 0x000A2D92
		public float OutlineWidthFactor
		{
			get
			{
				return this._outlineWidthFactor;
			}
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x000A4B9A File Offset: 0x000A2D9A
		public bool IsConnectedToDirection(TileDirection direction)
		{
			return this._connection.input.direction == direction || this._connection.output.direction == direction;
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x060026B2 RID: 9906 RVA: 0x000A4BC4 File Offset: 0x000A2DC4
		public bool IsComplete
		{
			get
			{
				return !this._roadWidthTween.IsActive;
			}
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x000A4BD4 File Offset: 0x000A2DD4
		public void Tick(TimeInterval tickTime)
		{
			if (this._outlineWidthTween.IsActive)
			{
				this._outlineWidthFactor = this._outlineWidthTween.Tick(tickTime.Delta);
				this._dynamicRoadMesh.OutlineWidthFactor = this._outlineWidthFactor;
			}
			if (this._roadWidthTween.IsActive)
			{
				this._roadWidthFactor = this._roadWidthTween.Tick(tickTime.Delta);
				this._dynamicRoadMesh.RoadWidthFactor = this._roadWidthFactor;
			}
			this._dynamicRoadMesh.UpdatePermanenceShaderValues();
		}

		// Token: 0x060026B4 RID: 9908 RVA: 0x000A4C58 File Offset: 0x000A2E58
		public void Reset()
		{
			base.transform.localPosition = Vector3.zero;
			this._connection = default(RoadTileConnection);
			this._animationDirection = RoadAnimationDirection.None;
			this._outlineWidthFactor = 0f;
			this._outlineWidthTween.Reset();
			this._roadWidthFactor = 0f;
			this._roadWidthTween.Reset();
		}

		// Token: 0x060026B5 RID: 9909 RVA: 0x000A4CB4 File Offset: 0x000A2EB4
		public void SetPermanenceVisibility(bool isPermanenceVisible)
		{
			this._dynamicRoadMesh.SetPermanenceVisibility(isPermanenceVisible);
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x000A4CC4 File Offset: 0x000A2EC4
		private static AnimatedRoadTileConnectionView CreateAnimation(IScope scope, TileView tileView, RoadState state)
		{
			AnimatedRoadTileConnectionView animatedRoadTileConnectionView = scope.Get<AnimatedRoadTileConnectionView>();
			animatedRoadTileConnectionView.transform.position = TilemapView.GetWorldPositionForCoordinates(tileView.Coordinates);
			animatedRoadTileConnectionView.RoadState = state;
			PermanenceZoneTextureLibrary permanenceZoneTextureLibrary = scope.Get<PermanenceZoneTextureLibrary>();
			animatedRoadTileConnectionView._dynamicRoadMesh.Initialize(tileView, permanenceZoneTextureLibrary, scope.Get<City>().Rules.RoadsBecomePermanentOverTime);
			return animatedRoadTileConnectionView;
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x000A4D18 File Offset: 0x000A2F18
		public static AnimatedRoadTileConnectionView CreateAnimationIn(IScope scope, TileView tileView, RoadTileConnection connection, RoadState state, RoadState previousState)
		{
			float initialOutlineWidth = (float)((previousState == RoadState.Mothballed) ? 1 : 0);
			AnimatedRoadTileConnectionView animatedRoadTileConnectionView = AnimatedRoadTileConnectionView.CreateAnimation(scope, tileView, state);
			animatedRoadTileConnectionView.AnimateConnectionIn(connection, initialOutlineWidth, 0f);
			return animatedRoadTileConnectionView;
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x000A4D46 File Offset: 0x000A2F46
		public static AnimatedRoadTileConnectionView CreateAnimationOut(IScope scope, TileView tileView, RoadTileConnection connection, RoadState state = RoadState.Mothballed)
		{
			AnimatedRoadTileConnectionView animatedRoadTileConnectionView = AnimatedRoadTileConnectionView.CreateAnimation(scope, tileView, state);
			animatedRoadTileConnectionView.AnimateConnectionOut(connection);
			return animatedRoadTileConnectionView;
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x000A4D57 File Offset: 0x000A2F57
		public static AnimatedRoadTileConnectionView CreateStaticAnimation(IScope scope, TileView tileView, RoadTileConnection connection, RoadState state)
		{
			AnimatedRoadTileConnectionView animatedRoadTileConnectionView = AnimatedRoadTileConnectionView.CreateAnimation(scope, tileView, state);
			animatedRoadTileConnectionView.AnimateConnectionIn(connection, 1f, 1f);
			return animatedRoadTileConnectionView;
		}

		// Token: 0x060026BA RID: 9914 RVA: 0x000A4D74 File Offset: 0x000A2F74
		private void AnimateConnectionIn(RoadTileConnection connection, float initialOutlineWidthFactor = 0f, float initialRoadWidthFactor = 0f)
		{
			this._connection = connection;
			RoadTileConnectionStrokePath strokePath = this._atlas.GetStrokePathForConnection(connection);
			this._dynamicRoadMesh.SetPathPoints(strokePath.pathPoints);
			this._dynamicRoadMesh.CursorWidthFactor = 1f;
			this._animationDirection = RoadAnimationDirection.AnimatingIn;
			this._outlineWidthFactor = initialOutlineWidthFactor;
			this._outlineWidthTween.Start(initialOutlineWidthFactor, 1f, this._visualConstants.AppearDuration, Easings.Functions.Linear, 0f);
			this._roadWidthFactor = initialRoadWidthFactor;
			this._roadWidthTween.Start(initialRoadWidthFactor, 1f, this._visualConstants.AppearDuration, Easings.Functions.Linear, 0f);
		}

		// Token: 0x060026BB RID: 9915 RVA: 0x000A4E10 File Offset: 0x000A3010
		private void AnimateConnectionOut(RoadTileConnection connection)
		{
			this._connection = connection;
			RoadTileConnectionStrokePath strokePath = this._atlas.GetStrokePathForConnection(connection);
			this._dynamicRoadMesh.SetPathPoints(strokePath.pathPoints);
			this._dynamicRoadMesh.CursorWidthFactor = 0f;
			this._animationDirection = RoadAnimationDirection.AnimatingOut;
			this._outlineWidthFactor = 1f;
			this._outlineWidthTween.Start(1f, 0f, this._visualConstants.DisappearDuration, Easings.Functions.Linear, 0f);
			this._roadWidthFactor = 1f;
			this._roadWidthTween.Start(1f, 0f, this._visualConstants.DisappearDuration, Easings.Functions.Linear, 0f);
		}

		// Token: 0x040020A5 RID: 8357
		private RoadTileConnection _connection;

		// Token: 0x040020A6 RID: 8358
		[ShowNonSerializedField]
		private RoadAnimationDirection _animationDirection;

		// Token: 0x040020A7 RID: 8359
		[ShowNonSerializedField]
		private float _outlineWidthFactor;

		// Token: 0x040020A8 RID: 8360
		[ShowNonSerializedField]
		private readonly TweenFloat _outlineWidthTween = new TweenFloat();

		// Token: 0x040020A9 RID: 8361
		[ShowNonSerializedField]
		private float _roadWidthFactor;

		// Token: 0x040020AA RID: 8362
		[ShowNonSerializedField]
		private readonly TweenFloat _roadWidthTween = new TweenFloat();

		// Token: 0x040020AB RID: 8363
		[SerializeField]
		private DynamicRoadMesh _dynamicRoadMesh;

		// Token: 0x040020AC RID: 8364
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x040020AD RID: 8365
		[Dependency]
		private RoadTileAtlas _atlas;
	}
}
