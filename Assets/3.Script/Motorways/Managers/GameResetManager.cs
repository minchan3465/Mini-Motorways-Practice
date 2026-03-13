using UnityEngine;
using UnityEngine.SceneManagement;
using Motorways.Models;
using Motorways.Managers;
using DG.Tweening;

namespace Motorways.Managers {
    //게임 재시작, 씬 전환, 프로그램 종료 등 시스템 명령을 전담하는 매니저입니다.
    public class GameResetManager : MonoBehaviour {
        public static GameResetManager Instance { get; private set; }

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        //게임 데이터를 초기화하고 런타임 씬을 다시 시작합니다.
        public void RestartGame() {
            PerformCleanup();
            SceneManager.LoadScene("RunTime");
        }

        //메인 메뉴 씬("Main")으로 이동합니다.
        public void GoToMainMenu() {
            PerformCleanup();
            SceneManager.LoadScene("Main");
        }

        //어플리케이션을 완전히 종료합니다.
        public void ExitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        //씬 전환 전 정적 데이터 및 DOTween 상태를 정리합니다.
        private void PerformCleanup() {
            WhiteoutController.Instance.OffWhiteOut();

            DOTween.KillAll();
            Time.timeScale = 1f;

            //정적 변수 리셋
            Vehicle.ResetId();
            Lane.ResetId();
            CityModel.LatestLaneChangeFrame = -1;
            CityModel.ChangedNodes.Clear();
        }
    }
}
