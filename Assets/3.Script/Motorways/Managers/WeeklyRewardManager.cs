using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Motorways.Models;

namespace Motorways.Managers {
	using DG.Tweening;

	// 원작의 주간 보상 시스템을 관리하는 클래스입니다.
	// 일요일 자정이 되면 게임을 멈추고 보상 선택지를 제공합니다.
	public class WeeklyRewardManager : MonoBehaviour {
		public static WeeklyRewardManager Instance;

		[Header("UI References")]
		public GameObject RewardPopupUI; // 보상 팝업 부모 오브젝트
		public GameObject CornerUI; //시간 안보이게 하기

		[Header("Shader Effect")]
		public Material FrostedMaterial; // Full Screen Pass에 사용 중인 매테리얼
		private float _currentStrength = 0f;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			if (RewardPopupUI != null) RewardPopupUI.SetActive(false);
			
			// 초기화 시 강도 0
			if (FrostedMaterial != null) FrostedMaterial.SetFloat("_Strength", 0f);
		}

		// 주간 보상 팝업 표시
		public void ShowRewardPopup() {
			// 1. 시뮬레이션 일시정지
			if (SimulationManager.Instance != null && !SimulationManager.Instance.IsPaused) {
				SimulationManager.Instance.TogglePause();
			}

			// 2. UI 활성화
			if (RewardPopupUI != null) {
				RewardPopupUI.SetActive(true);
				CornerUI.SetActive(false);
			}

			// 3. 화면 뽀얗게 만들기 (원작 효과)
			if (FrostedMaterial != null) {
				DOTween.To(() => _currentStrength, x => {
					_currentStrength = x;
					FrostedMaterial.SetFloat("_Strength", _currentStrength);
				}, 1.0f, 0.5f).SetUpdate(true); // 일시정지 상태에서도 동작하도록 SetUpdate(true)
			} else {
				Debug.LogWarning("[WeeklyReward] 보상 팝업 UI가 할당되지 않았습니다. 임시로 즉시 보상을 지급합니다.");
				if (RewardPopupUI == null) GiveReward(0);
			}
		}

		// 보상 선택 (UI 버튼에서 호출)
		public void SelectReward(int option) {
			GiveReward(option);

			// 1. 화면 다시 선명하게
			if (FrostedMaterial != null) {
				DOTween.To(() => _currentStrength, x => {
					_currentStrength = x;
					FrostedMaterial.SetFloat("_Strength", _currentStrength);
				}, 0.0f, 0.3f).SetUpdate(true).OnComplete(() => {
					if (RewardPopupUI != null) RewardPopupUI.SetActive(false);
					CornerUI.SetActive(true);

					// 2. 시뮬레이션 재개 (효과가 완전히 끝난 뒤)
					if (SimulationManager.Instance != null && SimulationManager.Instance.IsPaused) {
						SimulationManager.Instance.TogglePause();
					}
				});
			} else {
				if (RewardPopupUI != null) RewardPopupUI.SetActive(false);
				if (SimulationManager.Instance != null && SimulationManager.Instance.IsPaused) {
					SimulationManager.Instance.TogglePause();
					CornerUI.SetActive(true);
				}
			}
		}

		private void GiveReward(int option) {
			if (ResourceManager.Instance == null) return;

			if (option == 0) {
				// 도로 20개 + 다리 1개
				ResourceManager.Instance.AddResource(ItemType.Road, 20);
				ResourceManager.Instance.AddResource(ItemType.Bridge, 1);
				Debug.Log("[WeeklyReward] 보상 지급: 도로 20 + 다리 1");
			} else {
				// 도로 30개
				ResourceManager.Instance.AddResource(ItemType.Road, 30);
				Debug.Log("[WeeklyReward] 보상 지급: 도로 30");
			}
		}
	}
}
