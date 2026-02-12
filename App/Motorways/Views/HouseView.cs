using System;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Themes;
using Motorways.Views.MeshGeneration;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005C5 RID: 1477
	public class HouseView : MonoBehaviour, IView, IAudioView, HouseModel.IObserver, IReusable, IReleasedFromScopeHandler, ICreatedInScopeHandler
	{
		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06002950 RID: 10576 RVA: 0x000B1A07 File Offset: 0x000AFC07
		// (set) Token: 0x06002951 RID: 10577 RVA: 0x000B1A0F File Offset: 0x000AFC0F
		[Dependency]
		public City City { get; private set; }

		// Token: 0x06002952 RID: 10578 RVA: 0x000B1A18 File Offset: 0x000AFC18
		public void OnCreatedInScope(IScope scope)
		{
			this._scope = scope;
		}

		// Token: 0x06002953 RID: 10579 RVA: 0x000B1A24 File Offset: 0x000AFC24
		private void Initialize(HouseModel model)
		{
			this._isTicking = true;
			this._houseModel = model;
			this.Model.Subscribe(this);
			this._connectivity = NetworkConnectivity.Disconnected;
			this._lastNotifiedConnectivity = NetworkConnectivity.Disconnected;
			this.tilePosition = this._houseModel.tileModel.Coordinates;
			base.transform.localPosition = new Vector3((float)this.tilePosition.x * (float)TilemapModel.TileWidth, (float)this.tilePosition.y * (float)TilemapModel.TileWidth, 0f);
			this._viewIndex.AddHouseView(this);
			this.groupIndex = this._houseModel.GroupIndex;
			this._theme.RegisterGameObjectToThemeByGroupIndex(base.gameObject, this.groupIndex);
			this.combinedHouseMeshFilter.sharedMesh = this._houseMeshCombiner.MeshForGroupIndex(this.groupIndex);
			base.gameObject.GetComponent<CreativeModeEditableHouse>().Initialize(this._scope, this.groupIndex, TileDirection.East);
			if (this._viewClient.OnFirstFrame)
			{
				this.combinedHouseMeshFilter.gameObject.SetActive(false);
				this._transitionTween.Stop();
				base.transform.localScale = Vector3.one;
				this.AddToCombinedMesh();
				return;
			}
			this.combinedHouseMeshFilter.gameObject.SetActive(true);
			this._transitionTween.Start(0f, 1f, this.transitionInDuration, Easings.Functions.BackEaseOut, 0f);
			base.transform.localScale = new Vector3(0f, 0f, 1f);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateHouseEvent(AudioEventType.HouseSpawned, this, true));
		}

		// Token: 0x06002954 RID: 10580 RVA: 0x000B1BCB File Offset: 0x000AFDCB
		private void AddToCombinedMesh()
		{
			this._combinedMeshHandle = this._combinedMeshView.AddMesh(CombinedMeshView.CombinedMeshType.House, this._houseMeshCombiner.MeshForGroupIndex(this.groupIndex), base.transform.localToWorldMatrix);
		}

		// Token: 0x06002955 RID: 10581 RVA: 0x000B1BFC File Offset: 0x000AFDFC
		public void OnReleasedFromScope(IScope scope)
		{
			MotorwaysThemeDatabase.Log.Info("Releasing {0} with group index {1}", new object[]
			{
				base.gameObject.name,
				this.groupIndex
			});
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(base.gameObject, this.groupIndex);
			if (this._houseModel != null)
			{
				this._viewIndex.RemoveHouseView(this);
				this._houseModel.Unsubscribe(this);
				this._houseModel = null;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06002956 RID: 10582 RVA: 0x000B1C9C File Offset: 0x000AFE9C
		public void Reset()
		{
			this.tilePosition = default(Vector2Int);
			this.groupIndex = -1;
			this._isPendingDeletion = false;
			this._connectivity = NetworkConnectivity.Unknown;
			this._lastNotifiedConnectivity = NetworkConnectivity.Unknown;
			this._isTicking = false;
			base.transform.localPosition = Vector3.zero;
			this.combinedHouseMeshFilter.sharedMesh = null;
		}

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06002957 RID: 10583 RVA: 0x000B1CF4 File Offset: 0x000AFEF4
		public HouseModel Model
		{
			get
			{
				return this._houseModel;
			}
		}

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06002958 RID: 10584 RVA: 0x000B1CFC File Offset: 0x000AFEFC
		public VehicleView VehicleA
		{
			get
			{
				if (this._houseModel.ownedVehicles.Count <= 0)
				{
					return null;
				}
				return this._viewIndex.GetVehicleView(this._houseModel.ownedVehicles[0]);
			}
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06002959 RID: 10585 RVA: 0x000B1D2F File Offset: 0x000AFF2F
		public VehicleView VehicleB
		{
			get
			{
				if (this._houseModel.ownedVehicles.Count <= 1)
				{
					return null;
				}
				return this._viewIndex.GetVehicleView(this._houseModel.ownedVehicles[1]);
			}
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x000B1D64 File Offset: 0x000AFF64
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._transitionTween.IsActive)
			{
				float scale = this._transitionTween.Tick(timeInterval.Delta);
				base.transform.localScale = new Vector3(scale, scale, 1f);
				if (!this._transitionTween.IsActive)
				{
					this.combinedHouseMeshFilter.gameObject.SetActive(false);
					this.AddToCombinedMesh();
				}
				return TickResult.ContinueTicking;
			}
			if (this._isPendingDeletion)
			{
				this._combinedMeshView.RemoveMesh(this._combinedMeshHandle);
				return TickResult.Destroy;
			}
			this._isTicking = false;
			return TickResult.StopTicking;
		}

		// Token: 0x0600295B RID: 10587 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x0600295C RID: 10588 RVA: 0x000B1DF0 File Offset: 0x000AFFF0
		public Vector2 Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(base.transform.position);
			}
		}

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x0600295D RID: 10589 RVA: 0x000B1E08 File Offset: 0x000B0008
		public float Attenuation
		{
			get
			{
				return this._gameCamera.GetAttenuationFromWorld(base.transform.position, true, 5f);
			}
		}

		// Token: 0x0600295E RID: 10590 RVA: 0x000B1E26 File Offset: 0x000B0026
		public float GetAttenuation(bool zoom, float falloffFactor = 5f)
		{
			return this._gameCamera.GetAttenuationFromWorld(base.transform.position, zoom, falloffFactor);
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x0600295F RID: 10591 RVA: 0x000B1E40 File Offset: 0x000B0040
		// (set) Token: 0x06002960 RID: 10592 RVA: 0x000B1E48 File Offset: 0x000B0048
		public NetworkConnectivity NetworkConnectivity
		{
			get
			{
				return this._connectivity;
			}
			set
			{
				this._connectivity = value;
				if (this._connectivity != NetworkConnectivity.Unknown && this._lastNotifiedConnectivity != this._connectivity)
				{
					this._lastNotifiedConnectivity = this._connectivity;
					this._audioSystem.ScheduleEvent(AudioEvent.CreateHouseEvent(AudioEventType.HouseConnectedToNetwork, this, this._connectivity == NetworkConnectivity.Connected));
				}
			}
		}

		// Token: 0x06002961 RID: 10593 RVA: 0x000B1EA5 File Offset: 0x000B00A5
		public Color GetBuildingColor(ThemeComponentGroupTarget groupTarget)
		{
			return (this._theme.GetTheme() as Theme).GetBuildingColor(this.groupIndex, groupTarget);
		}

		// Token: 0x06002962 RID: 10594 RVA: 0x000B1EC4 File Offset: 0x000B00C4
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

		// Token: 0x06002963 RID: 10595 RVA: 0x000B1F4C File Offset: 0x000B014C
		public void OnDrawGizmosSelected()
		{
			if (this._houseModel != null && this._houseModel.tileModel != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(new Vector3((float)this._houseModel.tileModel.Coordinates.x * (float)TilemapModel.TileWidth, (float)this._houseModel.tileModel.Coordinates.y * (float)TilemapModel.TileWidth), Vector3.one * 0.2f);
			}
		}

		// Token: 0x06002964 RID: 10596 RVA: 0x000B1FDC File Offset: 0x000B01DC
		public void OnHouseChangedGroup(HouseModel house, int oldGroupIndex, int newGroupIndex)
		{
			VehicleView vehicleA = this.VehicleA;
			if (vehicleA != null)
			{
				vehicleA.SetNewGroupIndex(newGroupIndex);
			}
			VehicleView vehicleB = this.VehicleB;
			if (vehicleB != null)
			{
				vehicleB.SetNewGroupIndex(newGroupIndex);
			}
			this._theme.UnregisterGameObjectFromThemeByGroupIndex(base.gameObject, this.groupIndex);
			this.groupIndex = newGroupIndex;
			this._theme.RegisterGameObjectToThemeByGroupIndex(base.gameObject, this.groupIndex);
			this._combinedMeshView.RemoveMesh(this._combinedMeshHandle);
			this.AddToCombinedMesh();
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x000B205A File Offset: 0x000B025A
		public void OnHouseRemoved(HouseModel house)
		{
			this._isPendingDeletion = true;
			if (!this._isTicking)
			{
				this._viewClient.ResumeTickingView(this);
			}
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x000AB22A File Offset: 0x000A942A
		Transform IAudioView.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0400230B RID: 8971
		[Dependency]
		private ViewIndex _viewIndex;

		// Token: 0x0400230C RID: 8972
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x0400230D RID: 8973
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x0400230E RID: 8974
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x0400230F RID: 8975
		[Dependency]
		private MotorwaysThemeDatabase _theme;

		// Token: 0x04002310 RID: 8976
		[Dependency]
		private CombinedMeshView _combinedMeshView;

		// Token: 0x04002311 RID: 8977
		[Dependency]
		private HouseMeshCombiner _houseMeshCombiner;

		// Token: 0x04002312 RID: 8978
		[Dependency]
		private IScope _scope;

		// Token: 0x04002313 RID: 8979
		private CombinedMeshView.Handle _combinedMeshHandle;

		// Token: 0x04002314 RID: 8980
		public MeshFilter combinedHouseMeshFilter;

		// Token: 0x04002315 RID: 8981
		private HouseModel _houseModel;

		// Token: 0x04002316 RID: 8982
		public Vector2Int tilePosition;

		// Token: 0x04002317 RID: 8983
		public int groupIndex = -1;

		// Token: 0x04002318 RID: 8984
		public float transitionInDuration = 0.6f;

		// Token: 0x04002319 RID: 8985
		private readonly TweenFloat _transitionTween = new TweenFloat();

		// Token: 0x0400231A RID: 8986
		private bool _isPendingDeletion;

		// Token: 0x0400231B RID: 8987
		private NetworkConnectivity _connectivity;

		// Token: 0x0400231C RID: 8988
		private NetworkConnectivity _lastNotifiedConnectivity;

		// Token: 0x0400231D RID: 8989
		private float _spawnTime;

		// Token: 0x0400231E RID: 8990
		private bool _isTicking;

		// Token: 0x020005C6 RID: 1478
		public class Builder : IViewBuilder
		{
			// Token: 0x06002968 RID: 10600 RVA: 0x000B209C File Offset: 0x000B029C
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				HouseView buildingView = client.Scope.Get<HouseView>();
				buildingView.Initialize(model as HouseModel);
				client.AddView(buildingView);
			}
		}
	}
}
