using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Motorways {
    public class WhiteoutController : MonoBehaviour {
        public static WhiteoutController Instance = null;

        [Header("Settings")]
        public Material whiteoutMaterial; //아까 만든 WhiteoutVignetteMat 연결
        public float duration = 0.25f;

        private static readonly int IntensityID = Shader.PropertyToID("_Intensity");

	    //----------

	    private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(this);
	    }

	    public void OnWhiteOut() {
            whiteoutMaterial.DOKill();
            DOTween.To(() => whiteoutMaterial.GetFloat(IntensityID), x => whiteoutMaterial.SetFloat(IntensityID, x), 0.9f, duration)
                   .SetEase(Ease.OutQuad)
                   .SetTarget(whiteoutMaterial)
                   .SetUpdate(true);
        }

        public void OffWhiteOut() {
            whiteoutMaterial.DOKill();
            DOTween.To(() => whiteoutMaterial.GetFloat(IntensityID),x => whiteoutMaterial.SetFloat(IntensityID, x),0f, duration)
                   .SetEase(Ease.OutQuad)
                   .SetTarget(whiteoutMaterial)
                   .SetUpdate(true);
        }
    }
}