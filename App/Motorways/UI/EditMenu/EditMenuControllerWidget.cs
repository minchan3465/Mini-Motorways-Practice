using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Motorways.UI.EditMenu
{
	// Token: 0x02000759 RID: 1881
	public class EditMenuControllerWidget : MonoBehaviour
	{
		// Token: 0x06003481 RID: 13441 RVA: 0x000F6DB1 File Offset: 0x000F4FB1
		public void Open()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x06003482 RID: 13442 RVA: 0x000F28B2 File Offset: 0x000F0AB2
		public void Close()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06003483 RID: 13443 RVA: 0x000F6DC0 File Offset: 0x000F4FC0
		public void TurnToFace(Vector3 position, bool animate = true)
		{
			Vector3 direction = position - base.transform.position;
			float angle = Mathf.Atan2(direction.y, direction.x) * 57.29578f - 90f;
			if (animate)
			{
				this.AnimateToRotation(angle);
				return;
			}
			base.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		}

		// Token: 0x06003484 RID: 13444 RVA: 0x000F6E24 File Offset: 0x000F5024
		private Task AnimateToRotation(float angle)
		{
			EditMenuControllerWidget.<AnimateToRotation>d__3 <AnimateToRotation>d__;
			<AnimateToRotation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AnimateToRotation>d__.<>4__this = this;
			<AnimateToRotation>d__.angle = angle;
			<AnimateToRotation>d__.<>1__state = -1;
			<AnimateToRotation>d__.<>t__builder.Start<EditMenuControllerWidget.<AnimateToRotation>d__3>(ref <AnimateToRotation>d__);
			return <AnimateToRotation>d__.<>t__builder.Task;
		}
	}
}
