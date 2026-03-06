using UnityEngine;
using UnityEngine.UI;
using Motorways.Models;
using DG.Tweening;

namespace Motorways.Views {
	public class DestinationView : MonoBehaviour {
		[SerializeField] private GameObject effect_prefab;

		[SerializeField] private GameObject West;
		[SerializeField] private GameObject South;
		[SerializeField] private GameObject Plus;
		[SerializeField] private GameObject Minus;

		[SerializeField] private MeshRenderer West_Top_Top;
		[SerializeField] private MeshRenderer West_Top_Side;
		[SerializeField] private MeshRenderer West_Top_Entrance_Top;
		[SerializeField] private MeshRenderer West_Top_Entrance_Side;
		[SerializeField] private MeshRenderer West_Bottom_Top;
		[SerializeField] private MeshRenderer West_Bottom_Side;

		[SerializeField] private MeshRenderer South_Top_Top;
		[SerializeField] private MeshRenderer South_Top_Side;
		[SerializeField] private MeshRenderer South_Top_Entrance_Top;
		[SerializeField] private MeshRenderer South_Top_Entrance_Side;
		[SerializeField] private MeshRenderer South_Bottom_Top;
		[SerializeField] private MeshRenderer South_Bottom_Side;

		private bool _isHorizontal;

		//isHorizontal	: true면 가로형(3x2), false면 세로형(2x3)
		//isPositive	: true면 위/왼쪽 입구, false면 아래/오른쪽 입구
		public void UpdateVisuals(bool isHorizontal, bool isPositive) {
			// TODO: 전달된 상태값에 따라 메쉬나 도어의 활성 상태를 제어하세요.
			if (isHorizontal) {
				West.SetActive(true);
				South.SetActive(false);
				//								펀치세기, 지속시간, 진동횟수, 탄성
				West.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 0, 1f);
			} else {
				West.SetActive(false);
				South.SetActive(true);
				South.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 0, 1f);
			}

			if (isPositive) {
				Plus.SetActive(true);
				Minus.SetActive(false);
			} else {
				Plus.SetActive(false);
				Minus.SetActive(true);
			}
		}

		public void UpdateColor(int groupIndex) {
			GroupColor.ColorSet colorSet = GroupColor.GetGroupColorSet(groupIndex);

			//Base
			West_Bottom_Top.material.color = colorSet.Base;
			South_Bottom_Top.material.color = colorSet.Base;

			//Top
			West_Top_Top.material.color = colorSet.Top;
			West_Top_Entrance_Top.material.color = colorSet.Top;
			South_Top_Top.material.color = colorSet.Top;
			South_Top_Entrance_Top.material.color = colorSet.Top;

			//Side
			West_Top_Side.material.color = colorSet.Side;
			West_Top_Entrance_Side.material.color = colorSet.Side;
			South_Top_Side.material.color = colorSet.Side;
			South_Top_Entrance_Side.material.color = colorSet.Side;

			Vector3 spawnPos;
			if (_isHorizontal) spawnPos = West.transform.position;
			else spawnPos = South.transform.position;
			spawnPos += new Vector3(0, 0.5f, 0f);
			GameObject effect = Instantiate(effect_prefab, spawnPos, Quaternion.identity);

			if (effect.TryGetComponent(out BuildingSpawnCircle component)) {
				component.SpawnEffect(groupIndex, isHouse: false);
			}
		}
	}
}
