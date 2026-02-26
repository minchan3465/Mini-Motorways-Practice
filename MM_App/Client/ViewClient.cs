using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Views;
using Motorways.Views.Trains;
using Server;
using UnityEngine;

namespace Client
{
	// Token: 0x0200079D RID: 1949
	public class ViewClient : IClient, ISimulationObserver, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x060035A8 RID: 13736 RVA: 0x000F9DAD File Offset: 0x000F7FAD
		// (set) Token: 0x060035A9 RID: 13737 RVA: 0x000F9DB5 File Offset: 0x000F7FB5
		[Dependency]
		public Scope Scope { get; private set; }

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x060035AA RID: 13738 RVA: 0x000F9DBE File Offset: 0x000F7FBE
		public bool OnFirstFrame
		{
			get
			{
				return this._onFirstFrame;
			}
		}

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x060035AB RID: 13739 RVA: 0x000F9DC6 File Offset: 0x000F7FC6
		public CameraView CameraView
		{
			get
			{
				return this._cameraView;
			}
		}

		// Token: 0x060035AC RID: 13740 RVA: 0x000F9DCE File Offset: 0x000F7FCE
		public virtual void Start()
		{
			this._onFirstFrame = true;
		}

		// Token: 0x060035AD RID: 13741 RVA: 0x000F9DD8 File Offset: 0x000F7FD8
		public virtual void Tick(TimeInterval timeInterval, float stepAlpha)
		{
			foreach (IView viewToRemove in this._viewsPendingRemoval)
			{
				this.RemoveView(viewToRemove);
			}
			this._viewsPendingRemoval.Clear();
			int tickingViewIndex = 0;
			while (tickingViewIndex < this._tickingViews.Count)
			{
				IView view = this._tickingViews[tickingViewIndex];
				TickResult viewTickResult = view.Tick(timeInterval, stepAlpha);
				if (viewTickResult == TickResult.ContinueTicking)
				{
					tickingViewIndex++;
				}
				else if (viewTickResult == TickResult.StopTicking)
				{
					this._tickingViews.RemoveAt(tickingViewIndex);
					IViewLateTick viewLateTick = view as IViewLateTick;
					if (viewLateTick != null)
					{
						this._lateTickingViews.Remove(viewLateTick);
					}
				}
				else
				{
					this.RemoveView(view);
				}
			}
			foreach (IViewLateTick viewLateTick2 in this._lateTickingViews)
			{
				viewLateTick2.LateTick(timeInterval, stepAlpha);
			}
			this._onFirstFrame = false;
		}

		// Token: 0x060035AE RID: 13742 RVA: 0x000F9EE8 File Offset: 0x000F80E8
		protected void AddThemeComponent(IThemeComponent component)
		{
			this._themeComponents.Add(component);
			component.InitializeTheme(this._themeDatabase);
			if (this._themeDatabase != null && this._themeDatabase.GetTheme() != null)
			{
				component.ApplyTheme(this._themeDatabase.GetTheme());
			}
		}

		// Token: 0x060035AF RID: 13743 RVA: 0x000F9F28 File Offset: 0x000F8128
		public void AddView(IView view)
		{
			this._views.Add(view);
			this._tickingViews.Add(view);
			this._debugRenderSetManager.RegisterView(view);
			IViewLateTick viewLateTick = view as IViewLateTick;
			if (viewLateTick != null)
			{
				this._lateTickingViews.Add(viewLateTick);
			}
			IThemeComponent themeComponent = view as IThemeComponent;
			if (themeComponent != null)
			{
				this.AddThemeComponent(themeComponent);
			}
			foreach (IViewClientObserver viewClientObserver in this._observers)
			{
				viewClientObserver.OnViewAdded(this, view);
			}
		}

		// Token: 0x060035B0 RID: 13744 RVA: 0x000F9FA8 File Offset: 0x000F81A8
		public void ResumeTickingView(IView view)
		{
			this._tickingViews.Add(view);
			IViewLateTick viewLateTick = view as IViewLateTick;
			if (viewLateTick != null)
			{
				this._lateTickingViews.Add(viewLateTick);
			}
		}

		// Token: 0x060035B1 RID: 13745 RVA: 0x000F9FD8 File Offset: 0x000F81D8
		public void SetAllGameObjectsEnabled(bool enabled)
		{
			foreach (IView view in new List<IView>(this._views))
			{
				view.SetGameobjectActive(enabled);
			}
		}

		// Token: 0x060035B2 RID: 13746 RVA: 0x000FA030 File Offset: 0x000F8230
		private void RemoveView(IView view)
		{
			int viewIndex = this._views.IndexOf(view);
			if (Diagnostics.Verify(viewIndex != -1, "We are trying to remove a view that hasn't been added to this client yet!"))
			{
				foreach (IViewClientObserver viewClientObserver in this._observers)
				{
					viewClientObserver.OnViewRemoved(this, view);
				}
				this.Scope.Release(view);
				this._views.RemoveAt(viewIndex);
				this._tickingViews.Remove(view);
				this._debugRenderSetManager.UnregisterView(view);
				IViewLateTick viewLateTick = view as IViewLateTick;
				if (viewLateTick != null)
				{
					this._lateTickingViews.Remove(viewLateTick);
				}
				IThemeComponent component = view as IThemeComponent;
				if (component != null)
				{
					component.ReleaseTheme(this._themeDatabase);
					this._themeComponents.Remove(component);
				}
			}
		}

		// Token: 0x060035B3 RID: 13747 RVA: 0x000FA0F1 File Offset: 0x000F82F1
		public void MarkViewForRemoval(IView view)
		{
			this._viewsPendingRemoval.Add(view);
		}

		// Token: 0x060035B4 RID: 13748 RVA: 0x000FA100 File Offset: 0x000F8300
		public void RegisterViewBuilder<T>(IViewBuilder builder)
		{
			Type modelType = typeof(T);
			ObserverList<IViewBuilder> builders;
			if (!this._builders.TryGetValue(modelType, out builders))
			{
				builders = new ObserverList<IViewBuilder>(1);
				this._builders[modelType] = builders;
			}
			builders.Subscribe(builder);
		}

		// Token: 0x060035B5 RID: 13749 RVA: 0x000FA144 File Offset: 0x000F8344
		public void OnModelAdded(ISimulation simulation, IModel model, Fix64 timestamp)
		{
			ObserverList<IViewBuilder> builders;
			if (!this._builders.TryGetValue(model.GetType(), out builders))
			{
				return;
			}
			foreach (IViewBuilder viewBuilder in builders)
			{
				viewBuilder.CreateView(this, simulation, model, timestamp);
			}
		}

		// Token: 0x060035B6 RID: 13750 RVA: 0x000FA18C File Offset: 0x000F838C
		public void OnModelRemoved(ISimulation simulation, IModel model, Fix64 timestamp)
		{
			TrainCrossingModel trainCrossingModel = model as TrainCrossingModel;
			if (trainCrossingModel != null)
			{
				foreach (IView view in this._views)
				{
					TrainCrossingView trainCrossingView = view as TrainCrossingView;
					if (trainCrossingView != null && trainCrossingView.Model == trainCrossingModel)
					{
						this.MarkViewForRemoval(view);
					}
				}
			}
		}

		// Token: 0x060035B7 RID: 13751 RVA: 0x000FA1FC File Offset: 0x000F83FC
		public List<T> GetViews<T>() where T : class, IView
		{
			List<T> views = new List<T>();
			foreach (IView view in this._views)
			{
				if (view is T)
				{
					views.Add(view as T);
				}
			}
			return views;
		}

		// Token: 0x060035B8 RID: 13752 RVA: 0x000FA268 File Offset: 0x000F8468
		public void Subscribe(IViewClientObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x060035B9 RID: 13753 RVA: 0x000FA276 File Offset: 0x000F8476
		public bool Unsubscribe(IViewClientObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x060035BA RID: 13754 RVA: 0x000FA284 File Offset: 0x000F8484
		public virtual void OnReleasedFromScope(IScope scope)
		{
			while (this._views.Count > 0)
			{
				this.RemoveView(this._views[0]);
			}
		}

		// Token: 0x060035BB RID: 13755 RVA: 0x000FA2A8 File Offset: 0x000F84A8
		public void Reset()
		{
			this._views.Clear();
			this._tickingViews.Clear();
			this._lateTickingViews.Clear();
			this._builders.Clear();
			this._themeComponents.Clear();
		}

		// Token: 0x060035BC RID: 13756 RVA: 0x000FA2E4 File Offset: 0x000F84E4
		public void ApplyTheme(ITheme theme)
		{
			foreach (IThemeComponent themeComponent in this._themeComponents)
			{
				themeComponent.ApplyTheme(theme);
			}
		}

		// Token: 0x060035BD RID: 13757 RVA: 0x000FA338 File Offset: 0x000F8538
		public void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			foreach (IThemeComponent themeComponent in this._themeComponents)
			{
				themeComponent.ApplyBlendedTheme(oldTheme, newTheme, progress);
			}
		}

		// Token: 0x060035BE RID: 13758 RVA: 0x000FA38C File Offset: 0x000F858C
		public CarparkView GetCarparkWithEmptySpace(Vector2 position)
		{
			foreach (CarparkView carparkView in this.GetViews<CarparkView>())
			{
				CarparkModel carparkModel = carparkView.Model;
				if (carparkModel != null && carparkModel.SupportsTwoDestinations && carparkModel.destinationOffsets.Count > carparkModel.destinations.Count)
				{
					Bounds bounds = carparkView.GetEmptyDestinationSlotBounds();
					if (bounds.Contains(new Vector3(position.x, position.y, bounds.min.z)))
					{
						return carparkView;
					}
				}
			}
			return null;
		}

		// Token: 0x060035BF RID: 13759 RVA: 0x000FA438 File Offset: 0x000F8638
		public CarparkView GetCarparkViewFromModel(CarparkModel model)
		{
			foreach (CarparkView carparkView in this.GetViews<CarparkView>())
			{
				if (carparkView.Model == model)
				{
					return carparkView;
				}
			}
			return null;
		}

		// Token: 0x04002D85 RID: 11653
		[Dependency]
		private IThemeDatabase _themeDatabase;

		// Token: 0x04002D86 RID: 11654
		[Dependency]
		private IDebugRenderSetManager _debugRenderSetManager;

		// Token: 0x04002D87 RID: 11655
		private bool _onFirstFrame;

		// Token: 0x04002D88 RID: 11656
		private readonly Dictionary<Type, ObserverList<IViewBuilder>> _builders = new Dictionary<Type, ObserverList<IViewBuilder>>();

		// Token: 0x04002D89 RID: 11657
		private readonly List<IView> _views = new List<IView>();

		// Token: 0x04002D8A RID: 11658
		private readonly List<IView> _tickingViews = new List<IView>();

		// Token: 0x04002D8B RID: 11659
		private readonly List<IViewLateTick> _lateTickingViews = new List<IViewLateTick>();

		// Token: 0x04002D8C RID: 11660
		private readonly List<IView> _viewsPendingRemoval = new List<IView>();

		// Token: 0x04002D8D RID: 11661
		private readonly List<IThemeComponent> _themeComponents = new List<IThemeComponent>();

		// Token: 0x04002D8E RID: 11662
		private readonly ObserverList<IViewClientObserver> _observers = new ObserverList<IViewClientObserver>(1);

		// Token: 0x04002D8F RID: 11663
		[Dependency]
		private CameraView _cameraView;
	}
}
