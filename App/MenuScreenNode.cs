using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class MenuScreenNode : MonoBehaviour
{
	// Token: 0x06000B2E RID: 2862 RVA: 0x00025DA4 File Offset: 0x00023FA4
	[Button(null)]
	public void AddMatchingRecipicalConnections()
	{
		foreach (MenuScreenNode.Transition transition in this.transitions)
		{
			if (!transition.endNode.HasConnectionFor(this))
			{
				transition.endNode.transitions.Add(new MenuScreenNode.Transition
				{
					entryHandle = transition.exitHandle,
					exitHandle = transition.entryHandle,
					endNode = this,
					duration = transition.duration
				});
			}
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00025E40 File Offset: 0x00024040
	public void UpdateMatchingConnection(int index)
	{
		if (this.transitions.Count > index)
		{
			MenuScreenNode.Transition transition = this.transitions[index];
			foreach (MenuScreenNode.Transition otherTransition in transition.endNode.transitions)
			{
				if (otherTransition.endNode == this)
				{
					otherTransition.entryHandle = transition.exitHandle;
					otherTransition.exitHandle = transition.entryHandle;
					otherTransition.duration = transition.duration;
				}
			}
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00025EE0 File Offset: 0x000240E0
	public bool HasConnectionFor(MenuScreenNode node)
	{
		using (List<MenuScreenNode.Transition>.Enumerator enumerator = this.transitions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.endNode == node)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x00025F40 File Offset: 0x00024140
	public MenuScreenNode.Transition GetTransitionFor(ScreenStack.MotorwaysScreen screen)
	{
		foreach (MenuScreenNode.Transition transition in this.transitions)
		{
			if (transition.endNode.screen == screen)
			{
				return transition;
			}
		}
		return null;
	}

	// Token: 0x0400064D RID: 1613
	public ScreenStack.MotorwaysScreen screen;

	// Token: 0x0400064E RID: 1614
	public bool IsInGameScreen;

	// Token: 0x0400064F RID: 1615
	public float zoom = 15f;

	// Token: 0x04000650 RID: 1616
	public List<MenuScreenNode.Transition> transitions;

	// Token: 0x020001D5 RID: 469
	[Serializable]
	public class Transition
	{
		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x00025FB7 File Offset: 0x000241B7
		public Vector3 EndPosition
		{
			get
			{
				return this.endNode.transform.position;
			}
		}

		// Token: 0x04000651 RID: 1617
		public float duration = 1f;

		// Token: 0x04000652 RID: 1618
		public Vector3 entryHandle;

		// Token: 0x04000653 RID: 1619
		public Vector3 exitHandle;

		// Token: 0x04000654 RID: 1620
		public MenuScreenNode endNode;

		// Token: 0x04000655 RID: 1621
		public TransitionCameraControl cameraControl = TransitionCameraControl.Transform;
	}
}
