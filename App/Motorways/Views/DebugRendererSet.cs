using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005A6 RID: 1446
	public class DebugRendererSet
	{
		// Token: 0x06002855 RID: 10325 RVA: 0x000ABCED File Offset: 0x000A9EED
		public DebugRendererSet(string id)
		{
			this.id = id;
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06002856 RID: 10326 RVA: 0x000ABD12 File Offset: 0x000A9F12
		public IReadOnlyCollection<string> RendererNames
		{
			get
			{
				return this._isMutedStatus.Keys.ToList<string>();
			}
		}

		// Token: 0x06002857 RID: 10327 RVA: 0x000ABD24 File Offset: 0x000A9F24
		public bool AreRenderersWithNameMuted(string name)
		{
			bool isActive;
			return this._isMutedStatus.TryGetValue(name, out isActive) && isActive;
		}

		// Token: 0x06002858 RID: 10328 RVA: 0x000ABD44 File Offset: 0x000A9F44
		private void AddRenderer(Renderer renderer, MonoBehaviour source)
		{
			string rendererName = this.GetRendererName(renderer, source);
			List<Renderer> renderers;
			if (this._registeredRenderers.TryGetValue(rendererName, out renderers))
			{
				if (!renderers.Contains(renderer))
				{
					renderers.Add(renderer);
					return;
				}
			}
			else
			{
				this._isMutedStatus.Add(rendererName, !renderer.enabled);
				this._registeredRenderers.Add(rendererName, new List<Renderer>
				{
					renderer
				});
			}
		}

		// Token: 0x06002859 RID: 10329 RVA: 0x000ABDA8 File Offset: 0x000A9FA8
		private string GetRendererName(Renderer renderer, MonoBehaviour source)
		{
			string rendererName = renderer.name;
			if (source is RoadView && rendererName.Contains("Road #"))
			{
				rendererName = "Road";
			}
			return rendererName;
		}

		// Token: 0x0600285A RID: 10330 RVA: 0x000ABDD8 File Offset: 0x000A9FD8
		public void RemoveRenderer(Renderer renderer, MonoBehaviour source)
		{
			string rendererName = this.GetRendererName(renderer, source);
			List<Renderer> renderers;
			if (this._registeredRenderers.TryGetValue(rendererName, out renderers))
			{
				renderers.Remove(renderer);
			}
		}

		// Token: 0x0600285B RID: 10331 RVA: 0x000ABE08 File Offset: 0x000AA008
		public void RemoveRenderers(ICollection<Renderer> renderers, MonoBehaviour source)
		{
			foreach (Renderer renderer in renderers)
			{
				this.RemoveRenderer(renderer, source);
			}
		}

		// Token: 0x0600285C RID: 10332 RVA: 0x000ABE54 File Offset: 0x000AA054
		public void AddRenderers(ICollection<Renderer> renderers, MonoBehaviour source)
		{
			foreach (Renderer renderer in renderers)
			{
				this.AddRenderer(renderer, source);
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x0600285D RID: 10333 RVA: 0x000ABEA0 File Offset: 0x000AA0A0
		public bool AllRenderersMuted
		{
			get
			{
				using (Dictionary<string, bool>.ValueCollection.Enumerator enumerator = this._isMutedStatus.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x0600285E RID: 10334 RVA: 0x000ABEFC File Offset: 0x000AA0FC
		public void SetAllRenderersMuted(bool isMuted)
		{
			foreach (string name in this._isMutedStatus.Keys)
			{
				this._isMutedStatus[name] = isMuted;
				List<Renderer> renderers;
				if (this._registeredRenderers.TryGetValue(name, out renderers))
				{
					foreach (Renderer renderer in renderers)
					{
						renderer.enabled = !isMuted;
					}
				}
			}
		}

		// Token: 0x0600285F RID: 10335 RVA: 0x000ABFA8 File Offset: 0x000AA1A8
		public void SetRendersWithNameMuted(string name, bool isMuted)
		{
			if (!this._isMutedStatus.ContainsKey(name))
			{
				return;
			}
			this._isMutedStatus[name] = isMuted;
			List<Renderer> renderers;
			if (this._registeredRenderers.TryGetValue(name, out renderers))
			{
				foreach (Renderer renderer in renderers)
				{
					renderer.enabled = !isMuted;
				}
			}
		}

		// Token: 0x0400220D RID: 8717
		public readonly string id;

		// Token: 0x0400220E RID: 8718
		private readonly Dictionary<string, List<Renderer>> _registeredRenderers = new Dictionary<string, List<Renderer>>();

		// Token: 0x0400220F RID: 8719
		private readonly Dictionary<string, bool> _isMutedStatus = new Dictionary<string, bool>();
	}
}
