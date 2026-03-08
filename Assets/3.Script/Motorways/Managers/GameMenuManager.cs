using UnityEngine;
using Motorways.Managers;

namespace Motorways.Managers {
    /// <summary>
    /// 게임 메뉴 UI와 관련된 상태 및 참조를 관리하는 매니저입니다.
    /// </summary>
    public class GameMenuManager : MonoBehaviour {
        public static GameMenuManager Instance { get; private set; }

        [Header("Menu UI References")]
        public GameObject PausePanel;
        public GameObject OtherUI1;
        public GameObject OtherUI2;
        public GameObject OtherUI3;

        public bool IsMenuOpen => PausePanel != null && PausePanel.activeSelf;

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (PausePanel != null) PausePanel.SetActive(false);
        }

        /// <summary>
        /// 메뉴 UI를 토글하고 게임 일시정지 상태를 제어합니다.
        /// </summary>
        public void ToggleMenu() {
            if (PausePanel == null) return;

            bool newState = !PausePanel.activeSelf;
            PausePanel.SetActive(newState);

            // 다른 UI들 토글
            if (OtherUI1 != null) OtherUI1.SetActive(!newState);
            if (OtherUI2 != null) OtherUI2.SetActive(!newState);
            if (OtherUI3 != null) OtherUI3.SetActive(!newState);

            // 시뮬레이션 일시정지 제어
            if (SimulationManager.Instance != null) {
                if (newState) {
                    if (!SimulationManager.Instance.IsPaused) SimulationManager.Instance.TogglePause();
                } else {
                    if (SimulationManager.Instance.IsPaused) SimulationManager.Instance.TogglePause();
                }
            }
        }
    }
}
