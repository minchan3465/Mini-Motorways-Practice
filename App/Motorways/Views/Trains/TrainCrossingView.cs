using System;
using Client;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Views.Trains
{
	// Token: 0x02000618 RID: 1560
	public class TrainCrossingView : MonoBehaviour, IView, TrainCrossingModel.IObserver, IReusable
	{
		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06002BBD RID: 11197 RVA: 0x000C1524 File Offset: 0x000BF724
		public TrainCrossingModel Model
		{
			get
			{
				return this._trainCrossingModel;
			}
		}

		// Token: 0x06002BBE RID: 11198 RVA: 0x000C152C File Offset: 0x000BF72C
		private void Initialize(TrainCrossingModel trainCrossingModel)
		{
			this._trainCrossingModel = trainCrossingModel;
			base.transform.position = (Vector3)TilemapModel.GetWorldPositionForCoordinates(this._trainCrossingModel.Tile.Coordinates);
		}

		// Token: 0x06002BBF RID: 11199 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval tickTime, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x06002BC0 RID: 11200 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnSignalChanged(TrainSignalState trainSignalState)
		{
		}

		// Token: 0x06002BC2 RID: 11202 RVA: 0x000C155A File Offset: 0x000BF75A
		public void Reset()
		{
			this._trainCrossingModel = null;
			base.transform.position = Vector3.zero;
		}

		// Token: 0x040025E7 RID: 9703
		private TrainCrossingModel _trainCrossingModel;

		// Token: 0x040025E8 RID: 9704
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x02000619 RID: 1561
		public class Builder : IViewBuilder
		{
			// Token: 0x06002BC4 RID: 11204 RVA: 0x000C1574 File Offset: 0x000BF774
			public void CreateView(ViewClient client, ISimulation simulation, IModel model, Fix64 timestamp)
			{
				TrainCrossingView trainCrossingView = client.Scope.Get<TrainCrossingView>();
				trainCrossingView.Initialize(model as TrainCrossingModel);
				client.AddView(trainCrossingView);
			}
		}
	}
}
