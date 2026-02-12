using System;
using System.Collections.Generic;
using Factory.Pools;
using Motorways.Models;
using Motorways.Views;
using Motorways.Views.Boats;
using Motorways.Views.Trains;

namespace Motorways
{
	// Token: 0x02000456 RID: 1110
	public class ViewIndex : IReusable
	{
		// Token: 0x06001BAC RID: 7084 RVA: 0x00065284 File Offset: 0x00063484
		public void Reset()
		{
			this._houseModelToViewIndex.Clear();
			this._destinationModelToViewIndex.Clear();
			this._vehicleModelToViewIndex.Clear();
			this._railModelToViewIndex.Clear();
			this._trainModelToViewIndex.Clear();
			this._boatModelToViewIndex.Clear();
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x000652D3 File Offset: 0x000634D3
		public void AddHouseView(HouseView view)
		{
			this._houseModelToViewIndex[view.Model] = view;
		}

		// Token: 0x06001BAE RID: 7086 RVA: 0x000652E7 File Offset: 0x000634E7
		public void RemoveHouseView(HouseView view)
		{
			this._houseModelToViewIndex.Remove(view.Model);
		}

		// Token: 0x06001BAF RID: 7087 RVA: 0x000652FC File Offset: 0x000634FC
		public HouseView GetHouseView(HouseModel model)
		{
			HouseView view;
			if (model != null && Diagnostics.Verify(this._houseModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for house {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x0006532A File Offset: 0x0006352A
		public IEnumerable<HouseView> HouseViews
		{
			get
			{
				return this._houseModelToViewIndex.Values;
			}
		}

		// Token: 0x06001BB1 RID: 7089 RVA: 0x00065337 File Offset: 0x00063537
		public void AddDestinationView(DestinationView view)
		{
			if (Diagnostics.Verify(view.Model != null))
			{
				this._destinationModelToViewIndex[view.Model] = view;
			}
		}

		// Token: 0x06001BB2 RID: 7090 RVA: 0x0006535B File Offset: 0x0006355B
		public void RemoveDestinationView(DestinationView view)
		{
			if (Diagnostics.Verify(view.Model != null))
			{
				this._destinationModelToViewIndex.Remove(view.Model);
			}
		}

		// Token: 0x06001BB3 RID: 7091 RVA: 0x00065380 File Offset: 0x00063580
		public DestinationView GetDestinationView(DestinationModel model)
		{
			DestinationView view;
			if (model != null && Diagnostics.Verify(this._destinationModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for destination {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06001BB4 RID: 7092 RVA: 0x000653AE File Offset: 0x000635AE
		public IEnumerable<DestinationView> DestinationViews
		{
			get
			{
				return this._destinationModelToViewIndex.Values;
			}
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x000653BB File Offset: 0x000635BB
		public void AddVehicleView(VehicleView view)
		{
			this._vehicleModelToViewIndex[view.Model] = view;
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x000653CF File Offset: 0x000635CF
		public void RemoveVehicleView(VehicleView view)
		{
			this._vehicleModelToViewIndex.Remove(view.Model);
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x000653E4 File Offset: 0x000635E4
		public VehicleView GetVehicleView(VehicleModel model)
		{
			VehicleView view;
			if (model != null && Diagnostics.Verify(this._vehicleModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for vehicle {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x00065412 File Offset: 0x00063612
		public void AddRailView(RailView view)
		{
			this._railModelToViewIndex[view.Model] = view;
		}

		// Token: 0x06001BB9 RID: 7097 RVA: 0x00065426 File Offset: 0x00063626
		public void RemoveRailView(RailView view)
		{
			this._railModelToViewIndex.Remove(view.Model);
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x0006543C File Offset: 0x0006363C
		public RailView GetRailView(RailTileModel model)
		{
			RailView view;
			if (model != null && Diagnostics.Verify(this._railModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for rail {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x0006546A File Offset: 0x0006366A
		public void AddTrainView(TrainView view)
		{
			this._trainModelToViewIndex[view.Model] = view;
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x0006547E File Offset: 0x0006367E
		public void RemoveTrainView(TrainView view)
		{
			this._trainModelToViewIndex.Remove(view.Model);
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x00065494 File Offset: 0x00063694
		public TrainView GetTrainView(TrainModel model)
		{
			TrainView view;
			if (model != null && Diagnostics.Verify(this._trainModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for train {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x06001BBE RID: 7102 RVA: 0x000654C4 File Offset: 0x000636C4
		public BoatView GetBoatView(BoatModel model)
		{
			BoatView view;
			if (model != null && Diagnostics.Verify(this._boatModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for boat {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x06001BBF RID: 7103 RVA: 0x000654F2 File Offset: 0x000636F2
		public void AddBoatView(BoatView view)
		{
			this._boatModelToViewIndex[view.Model] = view;
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x00065506 File Offset: 0x00063706
		public void RemoveBoatView(BoatView view)
		{
			this._boatModelToViewIndex.Remove(view.Model);
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x0006551A File Offset: 0x0006371A
		public void AddBoatPathView(BoatPathView view)
		{
			this._boatPathModelToViewIndex[view.Model] = view;
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x0006552E File Offset: 0x0006372E
		public void RemoveBoatPathView(BoatPathView view)
		{
			this._boatPathModelToViewIndex.Remove(view.Model);
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x00065544 File Offset: 0x00063744
		public BoatPathView GetBoatPathView(BoatPathTileModel model)
		{
			BoatPathView view;
			if (model != null && Diagnostics.Verify(this._boatPathModelToViewIndex.TryGetValue(model, out view), "Could not find matching view for rail {0}.", model))
			{
				return view;
			}
			return null;
		}

		// Token: 0x04001718 RID: 5912
		private readonly Dictionary<HouseModel, HouseView> _houseModelToViewIndex = new Dictionary<HouseModel, HouseView>();

		// Token: 0x04001719 RID: 5913
		private readonly Dictionary<DestinationModel, DestinationView> _destinationModelToViewIndex = new Dictionary<DestinationModel, DestinationView>();

		// Token: 0x0400171A RID: 5914
		private readonly Dictionary<VehicleModel, VehicleView> _vehicleModelToViewIndex = new Dictionary<VehicleModel, VehicleView>();

		// Token: 0x0400171B RID: 5915
		private readonly Dictionary<RailTileModel, RailView> _railModelToViewIndex = new Dictionary<RailTileModel, RailView>();

		// Token: 0x0400171C RID: 5916
		private readonly Dictionary<BoatPathTileModel, BoatPathView> _boatPathModelToViewIndex = new Dictionary<BoatPathTileModel, BoatPathView>();

		// Token: 0x0400171D RID: 5917
		private readonly Dictionary<TrainModel, TrainView> _trainModelToViewIndex = new Dictionary<TrainModel, TrainView>();

		// Token: 0x0400171E RID: 5918
		private readonly Dictionary<BoatModel, BoatView> _boatModelToViewIndex = new Dictionary<BoatModel, BoatView>();
	}
}
