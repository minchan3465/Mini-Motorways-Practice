using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using DG.Tweening;

namespace Motorways.Views {
	public class BuildingSpawnCircle : MonoBehaviour {
		[SerializeField] private GameObject Circle_Outline;
		[SerializeField] private GameObject Circle_Fill;

		[SerializeField] private SpriteRenderer Circle_Outline_Mat;
		[SerializeField] private SpriteRenderer Circle_Fill_Mat;

		public void SpawnEffect(int groupIndex, bool isHouse) {
			transform.localScale = Vector3.one;

			float scale_duration = 1.5f;
			float fade_start = 1.25f;
			float fade_duration = scale_duration - fade_start;
			float Outline_max_sacle = isHouse ? 0.75f : 1.25f;
			float Fill_max_sacle = 7.5f;
			Color color = GroupColor.GetGroupColor(groupIndex);

			Circle_Outline_Mat.material.color = color;

			//---두트윈---
			Sequence mySequence = DOTween.Sequence();

			//크기 커지면서 점점 느려짐.
			Circle_Outline.transform.localScale = Vector3.zero;
			Circle_Fill.transform.localScale = Vector3.zero;

			mySequence.Append(Circle_Outline.transform.DOScale(new Vector3(Outline_max_sacle, Outline_max_sacle, 1f), scale_duration).SetEase(Ease.OutCubic));

			if (!isHouse) mySequence.Join(Circle_Fill.transform.DOScale(new Vector3(Fill_max_sacle, Fill_max_sacle, 1f), scale_duration).SetEase(Ease.OutCubic));
			else Circle_Fill.SetActive(false);

			mySequence.Insert(fade_start, Circle_Outline_Mat.material.DOFade(0, fade_duration));
			if (!isHouse) mySequence.Insert(fade_start, Circle_Fill_Mat.material.DOFade(0, fade_duration));

			//투명도 감소.
			mySequence.OnComplete(() => { Destroy(gameObject); });
		}
	}
}
