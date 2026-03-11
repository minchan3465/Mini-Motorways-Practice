using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Motorways.UI {
    using Managers;

	public class TimerVisualManager : MonoBehaviour {
        //--- 버튼
		public TimerBtn pauseButton;
		public TimerBtn playButton;
		public TimerBtn fastForwardButton;

		private TimerBtn currentSelectedButton;

        //--- 시계
        public RawImage clockPip;
        public Color stopColor;

        [SerializeField] private ClockView clockView;

        //-------------------

        private void Start() {
            // 각 버튼에 매니저 연결
            pauseButton.Setup(this);
            playButton.Setup(this);
            fastForwardButton.Setup(this);

            // 시뮬레이션 상태 변경 이벤트 구독
            if (SimulationManager.Instance != null) {
                SimulationManager.Instance.OnSimulationStateChanged += SyncUIWithSimulation;
            }

            // 초기 상태 동기화
            SyncUIWithSimulation();
        }

        private void OnDestroy() {
            if (SimulationManager.Instance != null) {
                SimulationManager.Instance.OnSimulationStateChanged -= SyncUIWithSimulation;
            }
        }

        public void OnButtonClicked(TimerBtn clickedButton, float timeScale) {
            // 버튼 클릭 시 시뮬레이션 매니저 속도 변경
            SimulationManager.Instance.changeTimeScale(timeScale);
        }

        private void SyncUIWithSimulation() {
            if (SimulationManager.Instance == null) return;

            bool isPaused = SimulationManager.Instance.IsPaused;
            float timeScale = SimulationManager.Instance.TimeScale;

            // 기존 선택된 버튼 해제
            if (currentSelectedButton != null) currentSelectedButton.DeselectButton();

            // 현재 상태에 맞는 버튼 선택
            if (isPaused) {
                currentSelectedButton = pauseButton;
            } else if (timeScale >= 2.0f) {
                currentSelectedButton = fastForwardButton;
            } else {
                currentSelectedButton = playButton;
            }

            if (currentSelectedButton != null) currentSelectedButton.SelectButton();
        }

        public Color GetStopColor() => stopColor;
    }
}
