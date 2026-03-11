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
		public GameObject CornerUI; // 시간 UI 등 (팝업 시 숨김 처리)


		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			if (RewardPopupUI != null) RewardPopupUI.SetActive(false);
		}

		// 주간 보상 팝업 표시
		public void ShowRewardPopup() {
			// 1. 시뮬레이션 일시정지 (SimulationManager 직접 호출)
			if (SimulationManager.Instance != null && !SimulationManager.Instance.IsPaused) {
				SimulationManager.Instance.TogglePause();
			}

			// 2. UI 활성화
			if (RewardPopupUI != null) RewardPopupUI.SetActive(true);
			if (CornerUI != null) CornerUI.SetActive(false);
		}

		// 보상 선택 (UI 버튼에서 호출)
		public void SelectReward(int option) {
			GiveReward(option);

			if (RewardPopupUI != null) RewardPopupUI.SetActive(false);
			if (CornerUI != null) CornerUI.SetActive(true);
			if (SimulationManager.Instance != null && SimulationManager.Instance.IsPaused) {
				SimulationManager.Instance.TogglePause();
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
