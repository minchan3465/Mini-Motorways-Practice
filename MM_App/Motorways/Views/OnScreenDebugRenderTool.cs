using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005A8 RID: 1448
	public class OnScreenDebugRenderTool : IOnScreenTool
	{
		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06002866 RID: 10342 RVA: 0x000AC024 File Offset: 0x000AA224
		public Rect InputBlockingRect
		{
			get
			{
				return this._windowRect;
			}
		}

		// Token: 0x06002867 RID: 10343 RVA: 0x000AC02C File Offset: 0x000AA22C
		public void OnGUI(IScope scope)
		{
			this._debugRenderSetManager = scope.Get<IDebugRenderSetManager>();
			if (this._headerStyle == null)
			{
				this._headerStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 35,
					alignment = TextAnchor.MiddleCenter,
					margin = new RectOffset(20, 20, 30, 5),
					wordWrap = false
				};
			}
			if (this._rootStyle == null)
			{
				this._rootStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 30,
					alignment = TextAnchor.MiddleLeft,
					wordWrap = false,
					margin = new RectOffset(0, 0, 10, 0)
				};
			}
			if (this._levelOneStyle == null)
			{
				this._levelOneStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 25,
					alignment = TextAnchor.MiddleLeft,
					margin = new RectOffset(20, 0, 10, 0),
					wordWrap = false
				};
			}
			if (this._leftButtonStyle == null)
			{
				this._leftButtonStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 50,
					alignment = TextAnchor.MiddleLeft,
					margin = new RectOffset(20, 20, 0, 20)
				};
			}
			GUI.skin.verticalScrollbar.fixedWidth = 30f;
			GUI.skin.verticalScrollbarThumb.fixedWidth = 30f;
			this._windowRect = GUI.Window(1, this._windowRect, new GUI.WindowFunction(this.DrawDebugRenderSetWindow), "Render Set Tool");
		}

		// Token: 0x06002868 RID: 10344 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Update()
		{
		}

		// Token: 0x06002869 RID: 10345 RVA: 0x000AC19C File Offset: 0x000AA39C
		private void DrawDebugRenderSetWindow(int windowId)
		{
			GUILayout.BeginArea(new Rect(12.5f, 18f, 475f, 684f));
			GUILayout.Label("Views", this._headerStyle, Array.Empty<GUILayoutOption>());
			this._scrollPosition = GUILayout.BeginScrollView(this._scrollPosition, false, true, Array.Empty<GUILayoutOption>());
			if (this._debugRenderSetManager.RendererSets == null)
			{
				return;
			}
			foreach (KeyValuePair<string, DebugRendererSet> debugRendererSetWithName in this._debugRenderSetManager.RendererSets)
			{
				string rendererSetName = debugRendererSetWithName.Key;
				if (!this._rendererSetViewInfos.ContainsKey(rendererSetName))
				{
					this._rendererSetViewInfos.Add(rendererSetName, new OnScreenDebugRenderTool.RendererSetViewInfo());
				}
				OnScreenDebugRenderTool.RendererSetViewInfo rendererSetViewInfo = this._rendererSetViewInfos[rendererSetName];
				DebugRendererSet debugRendererSet = debugRendererSetWithName.Value;
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button(rendererSetViewInfo.isCollapsed ? ">" : "v", this._leftButtonStyle, Array.Empty<GUILayoutOption>()))
				{
					rendererSetViewInfo.isCollapsed = !rendererSetViewInfo.isCollapsed;
				}
				GUILayout.Label(this.Truncate(rendererSetName, 20, "..."), this._rootStyle, Array.Empty<GUILayoutOption>());
				GUILayout.FlexibleSpace();
				bool allRenderersMuted = debugRendererSet.AllRenderersMuted;
				Color backgroundColor = GUI.backgroundColor;
				if (allRenderersMuted)
				{
					GUI.backgroundColor = this._mutedColor;
				}
				if (GUILayout.Button("M", this._leftButtonStyle, Array.Empty<GUILayoutOption>()))
				{
					debugRendererSet.SetAllRenderersMuted(!allRenderersMuted);
				}
				GUI.backgroundColor = backgroundColor;
				GUILayout.EndHorizontal();
				if (!rendererSetViewInfo.isCollapsed)
				{
					rendererSetViewInfo.scrollPosition = GUILayout.BeginScrollView(rendererSetViewInfo.scrollPosition, false, true, Array.Empty<GUILayoutOption>());
					foreach (string rendererName in debugRendererSet.RendererNames)
					{
						GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
						bool areRendersWithNameMuted = debugRendererSet.AreRenderersWithNameMuted(rendererName);
						GUILayout.Label(this.Truncate(rendererName, 20, "..."), this._levelOneStyle, Array.Empty<GUILayoutOption>());
						Color backgroundColor2 = GUI.backgroundColor;
						if (areRendersWithNameMuted)
						{
							GUI.backgroundColor = this._mutedColor;
						}
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("M", this._leftButtonStyle, Array.Empty<GUILayoutOption>()))
						{
							debugRendererSet.SetRendersWithNameMuted(rendererName, !areRendersWithNameMuted);
						}
						GUI.backgroundColor = backgroundColor2;
						GUILayout.EndHorizontal();
					}
					GUILayout.EndScrollView();
					break;
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			GUI.DragWindow(new Rect(0f, 0f, 475f, 684f));
		}

		// Token: 0x0600286A RID: 10346 RVA: 0x000AC458 File Offset: 0x000AA658
		private string Truncate(string input, int maxCharacters, string truncationString = "...")
		{
			if (input.Length > maxCharacters)
			{
				return input.Substring(0, maxCharacters - truncationString.Length) + truncationString;
			}
			return input;
		}

		// Token: 0x04002210 RID: 8720
		private IDebugRenderSetManager _debugRenderSetManager;

		// Token: 0x04002211 RID: 8721
		private static readonly Vector2Int BaseResolution = new Vector2Int(1920, 1080);

		// Token: 0x04002212 RID: 8722
		private const int BaseWindowWidth = 500;

		// Token: 0x04002213 RID: 8723
		private const int BaseWindowHeight = 720;

		// Token: 0x04002214 RID: 8724
		private static readonly Rect DefaultWindowRect = new Rect((float)(OnScreenDebugRenderTool.BaseResolution.x - 500), 0.5f * (float)(OnScreenDebugRenderTool.BaseResolution.y - 720), 500f, 720f);

		// Token: 0x04002215 RID: 8725
		private Rect _windowRect = OnScreenDebugRenderTool.DefaultWindowRect;

		// Token: 0x04002216 RID: 8726
		private GUIStyle _headerStyle;

		// Token: 0x04002217 RID: 8727
		private GUIStyle _rootStyle;

		// Token: 0x04002218 RID: 8728
		private GUIStyle _levelOneStyle;

		// Token: 0x04002219 RID: 8729
		private GUIStyle _leftButtonStyle;

		// Token: 0x0400221A RID: 8730
		private readonly Color _mutedColor = new Color(0.8f, 0f, 0f);

		// Token: 0x0400221B RID: 8731
		private Vector2 _scrollPosition = Vector2.zero;

		// Token: 0x0400221C RID: 8732
		private readonly Dictionary<string, OnScreenDebugRenderTool.RendererSetViewInfo> _rendererSetViewInfos = new Dictionary<string, OnScreenDebugRenderTool.RendererSetViewInfo>();

		// Token: 0x020005A9 RID: 1449
		private class RendererSetViewInfo
		{
			// Token: 0x0400221D RID: 8733
			public bool isCollapsed = true;

			// Token: 0x0400221E RID: 8734
			public Vector2 scrollPosition = Vector2.zero;
		}
	}
}
