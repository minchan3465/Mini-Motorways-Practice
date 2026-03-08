using UnityEngine;
using UnityEngine.SceneManagement;
using Motorways.Models;
using Motorways.Managers;
using DG.Tweening;

namespace Motorways.Managers {
    /// <summary>
    /// 게임 재시작, 씬 전환, 프로그램 종료 등 시스템 명령을 전담하는 매니저입니다.
    /// </summary>
    public class GameResetManager : MonoBehaviour {
        public static GameResetManager Instance { get; private set; }

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// 게임 데이터를 초기화하고 런타임 씬("RunTime")을 다시 시작합니다. (PlayBtn 연동)
        /// </summary>
        public void RestartGame() {
            Debug.Log("[GameResetManager] Restarting Game (RunTime Scene)...");
            PerformCleanup();
            SceneManager.LoadScene("RunTime");
        }

        /// <summary>
        /// 메인 메뉴 씬("Main")으로 이동합니다. (QuitBtn 연동)
        /// </summary>
        public void GoToMainMenu() {
            Debug.Log("[GameResetManager] Returning to Main Menu...");
            PerformCleanup();
            SceneManager.LoadScene("Main");
        }

        /// <summary>
        /// 어플리케이션을 완전히 종료합니다. (TerminatedBtn 연동)
        /// </summary>
        public void ExitGame() {
            Debug.Log("[GameResetManager] Exiting Application...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 씬 전환 전 정적 데이터 및 DOTween 상태를 정리합니다.
        /// </summary>
        private void PerformCleanup() {
            DOTween.KillAll();
            Time.timeScale = 1f;

            // 정적 변수 리셋 (원작 방식)
            Vehicle.ResetId();
            Lane.ResetId();
            CityModel.LatestLaneChangeFrame = -1;
            CityModel.ChangedNodes.Clear();
        }
    }
}
