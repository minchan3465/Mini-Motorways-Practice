using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways.Views
{
	// Token: 0x020005A4 RID: 1444
	public class DebugRenderSetManager : IDebugRenderSetManager
	{
		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06002849 RID: 10313 RVA: 0x000ABB62 File Offset: 0x000A9D62
		public IReadOnlyDictionary<string, DebugRendererSet> RendererSets
		{
			get
			{
				return this._registeredRendererSet;
			}
		}

		// Token: 0x0600284A RID: 10314 RVA: 0x000ABB6C File Offset: 0x000A9D6C
		public void Register(MonoBehaviour monoBehaviour)
		{
			Renderer[] renderers = this.GetRenderers(monoBehaviour);
			if (renderers.Length != 0)
			{
				this.GetOrCreateRenderSet(this.GetName<MonoBehaviour>(monoBehaviour)).AddRenderers(renderers, monoBehaviour);
			}
		}

		// Token: 0x0600284B RID: 10315 RVA: 0x000ABB9C File Offset: 0x000A9D9C
		public void Unregister(MonoBehaviour monoBehaviour)
		{
			Renderer[] renderers = this.GetRenderers(monoBehaviour);
			if (renderers.Length != 0)
			{
				this.GetOrCreateRenderSet(this.GetName<MonoBehaviour>(monoBehaviour)).RemoveRenderers(renderers, monoBehaviour);
			}
		}

		// Token: 0x0600284C RID: 10316 RVA: 0x000ABBCC File Offset: 0x000A9DCC
		public void RegisterView(IView view)
		{
			MonoBehaviour monoBehaviour = view as MonoBehaviour;
			if (monoBehaviour != null)
			{
				this.Register(monoBehaviour);
			}
		}

		// Token: 0x0600284D RID: 10317 RVA: 0x000ABBEC File Offset: 0x000A9DEC
		public void UnregisterView(IView view)
		{
			MonoBehaviour monoBehaviour = view as MonoBehaviour;
			if (monoBehaviour != null)
			{
				this.Unregister(monoBehaviour);
			}
		}

		// Token: 0x0600284E RID: 10318 RVA: 0x000ABC0C File Offset: 0x000A9E0C
		private string GetName<T>(T monoBehaviour) where T : MonoBehaviour
		{
			CityDefinition cityDefinition = monoBehaviour as CityDefinition;
			string name;
			if (cityDefinition != null)
			{
				name = cityDefinition.name;
			}
			else
			{
				name = monoBehaviour.GetType().Name;
			}
			return name;
		}

		// Token: 0x0600284F RID: 10319 RVA: 0x000ABC44 File Offset: 0x000A9E44
		private DebugRendererSet GetOrCreateRenderSet(string id)
		{
			DebugRendererSet rendererSet;
			if (this._registeredRendererSet.TryGetValue(id, out rendererSet))
			{
				return rendererSet;
			}
			rendererSet = new DebugRendererSet(id);
			this._registeredRendererSet.Add(id, rendererSet);
			return rendererSet;
		}

		// Token: 0x06002850 RID: 10320 RVA: 0x000ABC78 File Offset: 0x000A9E78
		private Renderer[] GetRenderers(MonoBehaviour monoBehaviour)
		{
			Renderer[] renderers = monoBehaviour.GetComponentsInChildren<Renderer>();
			if (monoBehaviour is CityDefinition)
			{
				renderers = (from renderer in renderers
				where !(renderer is TilemapRenderer)
				select renderer).ToArray<Renderer>();
			}
			return renderers;
		}

		// Token: 0x0400220A RID: 8714
		private readonly Dictionary<string, DebugRendererSet> _registeredRendererSet = new Dictionary<string, DebugRendererSet>();
	}
}
