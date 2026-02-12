using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200060A RID: 1546
	public class TreeView : MonoBehaviour, IView, TreeModel.IObserver, IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06002B3D RID: 11069 RVA: 0x000BE6CD File Offset: 0x000BC8CD
		public Vector2 Pan
		{
			get
			{
				return this._gameCamera.GetPanFromWorld(base.transform.position);
			}
		}

		// Token: 0x06002B3E RID: 11070 RVA: 0x000BE6E8 File Offset: 0x000BC8E8
		public void Initialize(TreeModel model)
		{
			this._treeModel = model;
			this.tilePosition = this._treeModel.TileModel.Coordinates;
			base.transform.localPosition = new Vector3((float)this.tilePosition.x * (float)TilemapModel.TileWidth, (float)this.tilePosition.y * (float)TilemapModel.TileWidth, 0f);
			for (int treeTypeIndex = 0; treeTypeIndex < this._treeTypes.Length; treeTypeIndex++)
			{
				this._treeTypes[treeTypeIndex].SetActive(this._treeModel.PrefabIndex == treeTypeIndex);
			}
			this._treeModel.Subscribe(this);
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x000BE791 File Offset: 0x000BC991
		public void OnReleasedFromScope(IScope scope)
		{
			TreeModel treeModel = this._treeModel;
			if (treeModel == null)
			{
				return;
			}
			treeModel.Unsubscribe(this);
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x000BE7A5 File Offset: 0x000BC9A5
		public void Reset()
		{
			base.transform.localPosition = Vector3.zero;
			this._treeModel = null;
			this.tilePosition = default(Vector2Int);
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x000BE7CA File Offset: 0x000BC9CA
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._treeModel == null && !this._explosion.isPlaying)
			{
				return TickResult.Destroy;
			}
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x000BE7E4 File Offset: 0x000BC9E4
		public void OnBulldozed()
		{
			for (int treeTypeIndex = 0; treeTypeIndex < this._treeTypes.Length; treeTypeIndex++)
			{
				this._treeTypes[treeTypeIndex].SetActive(false);
			}
			this._explosion.Play();
			this._treeModel.Unsubscribe(this);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(this._audioSystem.DspTime, AudioEventType.TreeBulldozed, this.Pan.x, -1f, true, null));
		}

		// Token: 0x0400255B RID: 9563
		[SerializeField]
		private GameObject[] _treeTypes;

		// Token: 0x0400255C RID: 9564
		private TreeModel _treeModel;

		// Token: 0x0400255D RID: 9565
		public Vector2Int tilePosition;

		// Token: 0x0400255E RID: 9566
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x0400255F RID: 9567
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002560 RID: 9568
		[SerializeField]
		private ParticleSystem _explosion;

		// Token: 0x0200060B RID: 1547
		public class Builder : IViewBuilder
		{
			// Token: 0x06002B45 RID: 11077 RVA: 0x000BE860 File Offset: 0x000BCA60
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				TreeView treeView = client.Scope.Get<TreeView>();
				treeView.Initialize(model as TreeModel);
				client.AddView(treeView);
			}
		}
	}
}
