using System;
using Easing;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000458 RID: 1112
	public class DestinationLevel : MonoBehaviour
	{
		// Token: 0x06001BCC RID: 7116 RVA: 0x000657DF File Offset: 0x000639DF
		private void Awake()
		{
			this._meshFilter = base.gameObject.GetComponent<MeshFilter>();
			base.gameObject.SetActive(false);
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x00065800 File Offset: 0x00063A00
		public void Tick(float tickTime)
		{
			if (this._transitionTween.IsActive)
			{
				float scale = this._transitionTween.Tick(tickTime);
				base.transform.localScale = new Vector3(scale, scale, 1f);
				if (scale <= 0f && !this._transitionTween.IsActive)
				{
					base.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x00065860 File Offset: 0x00063A60
		public void Show(TransitionStyle transitionStyle)
		{
			base.gameObject.SetActive(true);
			if (transitionStyle == TransitionStyle.Tween)
			{
				base.transform.localScale = new Vector3(0f, 0f, 1f);
				this._transitionTween.Start(0f, 1f, this._tweenInDuration, Easings.Functions.BackEaseOut, 0f);
				return;
			}
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			this._transitionTween.Stop();
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x000658E8 File Offset: 0x00063AE8
		public void Hide(TransitionStyle transitionStyle)
		{
			if (transitionStyle == TransitionStyle.Tween)
			{
				this._transitionTween.Start(base.transform.localScale.x, 0f, this._tweenOutDuration, Easings.Functions.BackEaseIn, 0f);
				return;
			}
			base.gameObject.SetActive(false);
			this._transitionTween.Stop();
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x0006593D File Offset: 0x00063B3D
		public void SetDestinationMesh(Mesh mesh)
		{
			this._meshFilter.mesh = mesh;
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06001BD1 RID: 7121 RVA: 0x0006594B File Offset: 0x00063B4B
		public bool IsTweening
		{
			get
			{
				return this._transitionTween.IsActive;
			}
		}

		// Token: 0x04001728 RID: 5928
		[SerializeField]
		private float _tweenInDuration = 0.45f;

		// Token: 0x04001729 RID: 5929
		[SerializeField]
		private float _tweenOutDuration = 0.45f;

		// Token: 0x0400172A RID: 5930
		private TweenFloat _transitionTween = new TweenFloat();

		// Token: 0x0400172B RID: 5931
		private MeshFilter _meshFilter;
	}
}
