using System;
using System.Collections.Generic;
using Client;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x0200052E RID: 1326
	public class MenuPlacementDefinition : MonoBehaviour, IView
	{
		// Token: 0x060022F3 RID: 8947 RVA: 0x0008EA10 File Offset: 0x0008CC10
		public void SetGridAlpha(float alpha)
		{
			this.grid.color = new Color(1f, 1f, 1f, alpha);
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x0008EA34 File Offset: 0x0008CC34
		public Vector3 GetPositionFor(ScreenStack.MotorwaysScreen screen)
		{
			foreach (MenuScreenNode node in this.menuPositions)
			{
				if (node.screen == screen)
				{
					return node.transform.position;
				}
			}
			Diagnostics.FailAssert("A MenuScreenNode hasn't been set up for type {0}! Please set one up in the MenuDefinition prefab.", new object[]
			{
				screen
			});
			return Vector3.zero;
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x0008EAB8 File Offset: 0x0008CCB8
		public float GetZoomFor(ScreenStack.MotorwaysScreen screen)
		{
			foreach (MenuScreenNode node in this.menuPositions)
			{
				if (node.screen == screen)
				{
					return node.zoom;
				}
			}
			Diagnostics.FailAssert("A MenuScreenNode hasn't been set up for type {0}! Please set one up in the MenuDefinition prefab. Defaulting to 15 zoom.", new object[]
			{
				screen
			});
			return 15f;
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x0008EB38 File Offset: 0x0008CD38
		public bool IsInGameScreen(ScreenStack.MotorwaysScreen screen)
		{
			foreach (MenuScreenNode node in this.menuPositions)
			{
				if (node.screen == screen)
				{
					return node.IsInGameScreen;
				}
			}
			return false;
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x0008EB9C File Offset: 0x0008CD9C
		public Quaternion GetRotationFor(ScreenStack.MotorwaysScreen screen)
		{
			foreach (MenuScreenNode node in this.menuPositions)
			{
				if (node.screen == screen)
				{
					return node.transform.rotation;
				}
			}
			Diagnostics.FailAssert("A MenuScreenNode hasn't been set up for type {0}! Please set one up in the MenuDefinition prefab.", new object[]
			{
				screen
			});
			return Quaternion.identity;
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x0008EC20 File Offset: 0x0008CE20
		public MenuScreenNode GetNodeForScreenType(ScreenStack.MotorwaysScreen screen)
		{
			foreach (MenuScreenNode menuScreenNode in this.menuPositions)
			{
				if (menuScreenNode.screen == screen)
				{
					return menuScreenNode;
				}
			}
			return null;
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x0008EC7C File Offset: 0x0008CE7C
		public bool TransitionExists(ScreenStack.MotorwaysScreen start, ScreenStack.MotorwaysScreen end)
		{
			return this.GetNodeForScreenType(start).GetTransitionFor(end) != null;
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x0008EC90 File Offset: 0x0008CE90
		public NodeConnection GetConnectionFrom(ScreenStack.MotorwaysScreen start, ScreenStack.MotorwaysScreen end)
		{
			MenuScreenNode startNode = this.GetNodeForScreenType(start);
			MenuScreenNode.Transition transition = startNode.GetTransitionFor(end);
			if (transition == null && (start == ScreenStack.MotorwaysScreen.None || end == ScreenStack.MotorwaysScreen.None))
			{
				return new NodeConnection
				{
					startNode = this.GetNodeForScreenType(start),
					endNode = this.GetNodeForScreenType(end)
				};
			}
			if (Diagnostics.Verify(transition != null, "{0} does not have a transition to {1}! Add one to the `{2}` prefab", start, end, base.name))
			{
				return new NodeConnection
				{
					startNode = startNode,
					entryHandle = transition.entryHandle,
					exitHandle = transition.exitHandle,
					endNode = transition.endNode,
					duration = transition.duration,
					cameraControl = transition.cameraControl
				};
			}
			Diagnostics.FailAssert("There is no transitions from {0} to {1}, please add one in the `{2}` prefab.", new object[]
			{
				start,
				end,
				base.name
			});
			return default(NodeConnection);
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x000020AA File Offset: 0x000002AA
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			return TickResult.StopTicking;
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x04001D0E RID: 7438
		public List<MenuScreenNode> menuPositions;

		// Token: 0x04001D0F RID: 7439
		public GameObject background;

		// Token: 0x04001D10 RID: 7440
		public SpriteRenderer grid;
	}
}
