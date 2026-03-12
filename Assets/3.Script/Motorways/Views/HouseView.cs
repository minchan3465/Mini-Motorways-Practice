using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Motorways.Views {
	public class HouseView : MonoBehaviour {
		[SerializeField] private GameObject effect_prefab;

		[SerializeField] private MeshRenderer HouseRoof;
		[SerializeField] private MeshRenderer HouseRoof2;

		public void UpdateColor(int groupIndex) {

			SoundEffect houseRnd = Random.Range(0, 2) + SoundEffect.HouseBuild1;
			SoundManager.Instance.PlaySFX(houseRnd);

			Color color = GroupColor.GetGroupColor(groupIndex);
			HouseRoof.material.color = color;
			HouseRoof2.material.color = color;

			transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 0, 1f);

			Vector3 spawnPos = this.transform.position;
			spawnPos += new Vector3(0, 0.5f, 0f);
			GameObject effect = Instantiate(effect_prefab, spawnPos, Quaternion.identity);
			if(effect.TryGetComponent(out BuildingSpawnCircle component)) {
				component.SpawnEffect(groupIndex, isHouse: true);
			}
		}
	}
}
