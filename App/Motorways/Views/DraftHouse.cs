using System;
using Easing;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Views.MeshGeneration;
using Server;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x020005C1 RID: 1473
	public class DraftHouse : MonoBehaviour, IReusable, IReleasedFromScopeHandler, ICreativeModeEditableObject
	{
		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06002910 RID: 10512 RVA: 0x000B0DF4 File Offset: 0x000AEFF4
		// (set) Token: 0x06002911 RID: 10513 RVA: 0x000B0DFC File Offset: 0x000AEFFC
		public bool HasUnplaceableView { get; private set; }

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06002912 RID: 10514 RVA: 0x000B0E05 File Offset: 0x000AF005
		public bool IsTicking
		{
			get
			{
				return this._isTicking;
			}
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x000B0E10 File Offset: 0x000AF010
		public void Initialize(Vector2Int initialTilePosition, IScope scope, int groupIndex, TileDirection drivewayDirection)
		{
			this._isTicking = true;
			this.HasUnplaceableView = false;
			this.tilePosition = initialTilePosition;
			this._originalPosition = initialTilePosition;
			this._scope = scope;
			this._groupIndex = groupIndex;
			this._originalGroupIndex = groupIndex;
			this._drivewayDirection = drivewayDirection;
			this._originalDrivewayDirection = this._drivewayDirection;
			this.UpdatePosition(this.tilePosition);
			this._transitionTween.Start(0f, 1f, this.transitionInDuration, Easings.Functions.BackEaseOut, 0f);
			base.transform.localScale = new Vector3(0f, 0f, 1f);
			this.UpdateMesh(this._groupIndex);
			this._renderTextureImage.color = new Color(this._renderTextureImage.color.r, this._renderTextureImage.color.g, this._renderTextureImage.color.b, this._ghostPreviewNormalOpacity);
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x000B0F04 File Offset: 0x000AF104
		public void InitializeWithExistingView(IScope scope, HouseView view)
		{
			this.HasUnplaceableView = false;
			this._hasExistingView = true;
			this._originalGroupIndex = view.groupIndex;
			this._originalDrivewayDirection = view.Model.DrivewayDirection;
			this._originalPosition = view.tilePosition;
			this._isTicking = true;
			this.tilePosition = view.tilePosition;
			this._scope = scope;
			this._groupIndex = view.groupIndex;
			this._drivewayDirection = view.Model.DrivewayDirection;
			this.UpdatePosition(this.tilePosition);
			base.transform.localScale = Vector3.one;
			this.UpdateMesh(this._groupIndex);
			this._renderTextureImage.color = new Color(this._renderTextureImage.color.r, this._renderTextureImage.color.g, this._renderTextureImage.color.b, this._ghostPreviewNormalOpacity);
		}

		// Token: 0x06002915 RID: 10517 RVA: 0x000B0FF0 File Offset: 0x000AF1F0
		public void UpdatePosition(Vector2Int newPosition)
		{
			this.tilePosition = newPosition;
			base.transform.localPosition = new Vector3((float)this.tilePosition.x * (float)TilemapModel.TileWidth, (float)this.tilePosition.y * (float)TilemapModel.TileWidth, 0f);
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06002916 RID: 10518 RVA: 0x000B1059 File Offset: 0x000AF259
		public void UpdateDrivewayPosition(TileDirection drivewayDirection)
		{
			this._drivewayDirection = drivewayDirection;
		}

		// Token: 0x06002917 RID: 10519 RVA: 0x000B1062 File Offset: 0x000AF262
		public void OnReleasedFromScope(IScope scope)
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x000B1084 File Offset: 0x000AF284
		public void Reset()
		{
			this.HasUnplaceableView = false;
			this.tilePosition = default(Vector2Int);
			this._originalPosition = default(Vector2Int);
			this._scope = null;
			this._groupIndex = 0;
			this._drivewayDirection = TileDirection.North;
			this._isTicking = false;
			base.transform.localPosition = Vector3.zero;
			this._hasExistingView = false;
			this._originalDrivewayDirection = TileDirection.North;
			this._originalGroupIndex = 0;
		}

		// Token: 0x06002919 RID: 10521 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Tick(float frameTime)
		{
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x0600291A RID: 10522 RVA: 0x000B10F1 File Offset: 0x000AF2F1
		public Vector2 Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(base.transform.position);
			}
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x0600291B RID: 10523 RVA: 0x000B1109 File Offset: 0x000AF309
		public float Attenuation
		{
			get
			{
				return this._gameCamera.GetAttenuationFromWorld(base.transform.position, true, 5f);
			}
		}

		// Token: 0x0600291C RID: 10524 RVA: 0x000B1127 File Offset: 0x000AF327
		public float GetAttenuation(bool zoom, float falloffFactor = 5f)
		{
			return this._gameCamera.GetAttenuationFromWorld(base.transform.position, zoom, falloffFactor);
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x000B1144 File Offset: 0x000AF344
		public Bounds GetBounds()
		{
			float tileWidth = (float)TilemapModel.TileWidth;
			Vector3 halfTileDimensions = new Vector3(tileWidth, tileWidth, 0f) * 0.5f;
			Vector3 tileCenterPosition = new Vector3((float)this.tilePosition.x * tileWidth, (float)this.tilePosition.y * tileWidth, base.transform.position.z);
			return new Bounds
			{
				min = tileCenterPosition - halfTileDimensions,
				max = tileCenterPosition + halfTileDimensions
			};
		}

		// Token: 0x0600291E RID: 10526 RVA: 0x000B11CC File Offset: 0x000AF3CC
		public void StartUnplaceableView()
		{
			DraftHouse.Log.Info("Start unplaceable ghost view for {0}", new object[]
			{
				this.ToString()
			});
			this.HasUnplaceableView = true;
			this._renderTextureImage.color = new Color(this._renderTextureImage.color.r, this._renderTextureImage.color.g, this._renderTextureImage.color.b, this._ghostPreviewInvalidOpacity);
		}

		// Token: 0x0600291F RID: 10527 RVA: 0x000B1244 File Offset: 0x000AF444
		public void EndUnplaceableView()
		{
			DraftHouse.Log.Info("Start unplaceable ghost view for {0}", new object[]
			{
				this.ToString()
			});
			this.HasUnplaceableView = false;
			this._renderTextureImage.color = new Color(this._renderTextureImage.color.r, this._renderTextureImage.color.g, this._renderTextureImage.color.b, this._ghostPreviewNormalOpacity);
		}

		// Token: 0x06002920 RID: 10528 RVA: 0x000B12BC File Offset: 0x000AF4BC
		public void Delete(bool isReplacement)
		{
			this._scope.Release(this);
		}

		// Token: 0x06002921 RID: 10529 RVA: 0x000B12CB File Offset: 0x000AF4CB
		public bool IsConfirmable()
		{
			return !this.HasUnplaceableView;
		}

		// Token: 0x06002922 RID: 10530 RVA: 0x0000222C File Offset: 0x0000042C
		public BuildingLayout GetBuildingLayout()
		{
			return BuildingLayout.BuildingAbove;
		}

		// Token: 0x06002923 RID: 10531 RVA: 0x000B12D6 File Offset: 0x000AF4D6
		public Vector2 GetWorldPosition()
		{
			return base.transform.position;
		}

		// Token: 0x06002924 RID: 10532 RVA: 0x000B12E8 File Offset: 0x000AF4E8
		public Vector2Int GetTilePosition()
		{
			return this.tilePosition;
		}

		// Token: 0x06002925 RID: 10533 RVA: 0x000B12F0 File Offset: 0x000AF4F0
		public Vector2 GetCenterForEditMenuPosition()
		{
			return this.GetWorldPosition();
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x000B12F8 File Offset: 0x000AF4F8
		public bool CompletelyOutOfPlayArea(City city)
		{
			return city != null && !city.IsTileInPlayableArea(this.tilePosition, Fix64.MaxValue);
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x000B1313 File Offset: 0x000AF513
		public EditMenuButtonType GetEditOptions()
		{
			return this._editOptions;
		}

		// Token: 0x06002928 RID: 10536 RVA: 0x000B131B File Offset: 0x000AF51B
		public void Confirm()
		{
			if (!Diagnostics.Verify(!this.HasUnplaceableView && this.IsConfirmable(), "We should only confirm if the house has a valid placement!"))
			{
				return;
			}
			this.SpawnHouse(false);
			this._scope.Release(this);
		}

		// Token: 0x06002929 RID: 10537 RVA: 0x000B1350 File Offset: 0x000AF550
		private void SpawnHouse(bool original = false)
		{
			CityPlanModel.ScheduledBuilding firstHouse = this._scope.Get<CityPlanModel.ScheduledBuilding>();
			firstHouse.type = CityTileType.Supply;
			firstHouse.groupIndex = (original ? this._originalGroupIndex : this._groupIndex);
			firstHouse.useFixedParameters = true;
			firstHouse.positionOverride = (original ? this._originalPosition : this.tilePosition);
			firstHouse.drivewayDirectionOverride = (original ? this._originalDrivewayDirection : this._drivewayDirection);
			firstHouse.time = this._simulation.Timestep;
			this._scope.Get<CityPlanModel>().ScheduleBuilding(firstHouse);
		}

		// Token: 0x0600292A RID: 10538 RVA: 0x000B13DE File Offset: 0x000AF5DE
		public void Cancel()
		{
			if (this._hasExistingView)
			{
				this.UpdatePosition(this._originalPosition);
				this.SpawnHouse(true);
			}
			this._scope.Release(this);
		}

		// Token: 0x0600292B RID: 10539 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600292C RID: 10540 RVA: 0x000B1408 File Offset: 0x000AF608
		public int GetGroupIndex()
		{
			return this._groupIndex;
		}

		// Token: 0x0600292D RID: 10541 RVA: 0x000B1410 File Offset: 0x000AF610
		public void SetGroupIndex(int groupIndex, bool isReplacement)
		{
			if (this._groupIndex != groupIndex)
			{
				this._groupIndex = groupIndex;
				this.UpdateMesh(this._groupIndex);
			}
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x000AE722 File Offset: 0x000AC922
		public ICreativeModeEditableObject GetGhostPreview(out bool isReplacement)
		{
			isReplacement = false;
			return this;
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x000A875F File Offset: 0x000A695F
		public void Flip(bool isReplacement)
		{
			Diagnostics.FailAssert("Flip called on a DraftHouse, but only makes sense on Single Destinations!", Array.Empty<object>());
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x000A8770 File Offset: 0x000A6970
		public void Rotate(bool isReplacement)
		{
			Diagnostics.FailAssert("Rotate called on a DraftHouse, but only makes sense on Destinations!", Array.Empty<object>());
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x000B142E File Offset: 0x000AF62E
		public void UpgradeOrDowngrade(bool isReplacement)
		{
			DraftHouse.Log.Error("UpgradeOrDowngrade called on a DraftHouse, but only makes sense on Destinations!", Array.Empty<object>());
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x000B1444 File Offset: 0x000AF644
		private void UpdateMesh(int groupIndex)
		{
			if (!Diagnostics.Verify(this._houseMesh != null, "HouseMesh cannot be null, set it on prefab"))
			{
				return;
			}
			HouseMeshCombiner combiner = this._scope.Get<HouseMeshCombiner>();
			if (!Diagnostics.Verify(combiner != null, "Cannot find HouseMeshCombiner in scope"))
			{
				return;
			}
			Mesh mesh = combiner.MeshForGroupIndex(groupIndex);
			this._houseMesh.mesh = mesh;
		}

		// Token: 0x040022D0 RID: 8912
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DraftHouse");

		// Token: 0x040022D1 RID: 8913
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040022D2 RID: 8914
		[Dependency]
		private City _city;

		// Token: 0x040022D3 RID: 8915
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x040022D4 RID: 8916
		[SerializeField]
		private EditMenuButtonType _editOptions;

		// Token: 0x040022D5 RID: 8917
		[SerializeField]
		private MeshFilter _houseMesh;

		// Token: 0x040022D6 RID: 8918
		[SerializeField]
		private RawImage _renderTextureImage;

		// Token: 0x040022D7 RID: 8919
		[SerializeField]
		private float _ghostPreviewNormalOpacity = 0.8f;

		// Token: 0x040022D8 RID: 8920
		[SerializeField]
		private float _ghostPreviewInvalidOpacity = 0.5f;

		// Token: 0x040022DA RID: 8922
		public Vector2Int tilePosition;

		// Token: 0x040022DB RID: 8923
		public float transitionInDuration = 0.6f;

		// Token: 0x040022DC RID: 8924
		private readonly TweenFloat _transitionTween = new TweenFloat();

		// Token: 0x040022DD RID: 8925
		private IScope _scope;

		// Token: 0x040022DE RID: 8926
		private float _spawnTime;

		// Token: 0x040022DF RID: 8927
		private bool _isTicking;

		// Token: 0x040022E0 RID: 8928
		private int _groupIndex;

		// Token: 0x040022E1 RID: 8929
		private TileDirection _drivewayDirection;

		// Token: 0x040022E2 RID: 8930
		private bool _hasExistingView;

		// Token: 0x040022E3 RID: 8931
		private Vector2Int _originalPosition;

		// Token: 0x040022E4 RID: 8932
		private int _originalGroupIndex;

		// Token: 0x040022E5 RID: 8933
		private TileDirection _originalDrivewayDirection;
	}
}
