using UnityEngine;
using System.Collections;
using Motorways.Actions;
using Motorways.Managers;
using Motorways.Process;
using DG.Tweening;
using TMPro;

namespace Motorways.Managers {
    /// <summary>
    /// 게임 오버 시퀀스를 총괄하는 매니저입니다.
    /// 카메라 회전 연출과 결과 텍스트 업데이트 기능이 추가되었습니다.
    /// </summary>
    public class GameOverManager : MonoBehaviour {
        public static GameOverManager Instance { get; private set; }

        [Header("UI References")]
        public GameObject GameOverPanel;
        public TextMeshProUGUI ResultText; // 결과 문구 TMP
        private CanvasGroup _canvasGroup;

        [Header("Camera Settings")]
        [SerializeField] private float _zoomSize = 10f;
        [SerializeField] private float _moveDuration = 0.8f;
        [SerializeField] private float _waitBeforeUI = 1.5f;
        [SerializeField] private float _tiltAngle = 35f; // 원작 스타일 기울기 각도

        public bool IsSequenceActive { get; private set; } = false;

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (GameOverPanel != null) {
                _canvasGroup = GameOverPanel.GetComponent<CanvasGroup>();
                if (_canvasGroup == null) _canvasGroup = GameOverPanel.AddComponent<CanvasGroup>();
                GameOverPanel.SetActive(false);
                _canvasGroup.alpha = 0f;
            }
        }

        public void TriggerGameOver(Vector3 focusPoint) {
            if (IsSequenceActive) return;
            IsSequenceActive = true;
            StartCoroutine(GameOverRoutine(focusPoint));
        }

        private IEnumerator GameOverRoutine(Vector3 focusPoint) {
            // 1. 정지 및 입력 차단
            Time.timeScale = 0f;
            if (InteractionController.Instance != null) {
                InteractionController.Instance.enabled = false;
            }

            // 2. 텍스트 업데이트
            UpdateResultText();

            // 3. 카메라 연출: 이동 + 줌 + 회전 (원작 스타일)
            Camera mainCam = InteractionController.Instance != null ? InteractionController.Instance.MainCamera : Camera.main;
            if (mainCam != null) {
                mainCam.transform.DOKill();
                
                // 중앙 기준 위치에 따른 회전 방향 결정
                float mapCenterX = InteractionController.Instance.GetMapCenter().x;
                float finalTilt = focusPoint.x < mapCenterX ? -_tiltAngle : _tiltAngle;

                Vector3 targetPos = new Vector3(focusPoint.x, mainCam.transform.position.y, focusPoint.z);
                
                // 위치와 회전 동시 진행
                mainCam.transform.DOMove(targetPos, _moveDuration).SetEase(Ease.OutExpo).SetUpdate(true);
                mainCam.transform.DORotate(new Vector3(mainCam.transform.eulerAngles.x, mainCam.transform.eulerAngles.y, finalTilt), _moveDuration).SetEase(Ease.OutExpo).SetUpdate(true);
                
                // 줌인
                DOTween.To(() => mainCam.orthographicSize, x => mainCam.orthographicSize = x, _zoomSize, _moveDuration)
                    .SetEase(Ease.OutExpo)
                    .SetUpdate(true);
            }

            // 4. 쉐이더 효과 적용 (WeeklyRewardManager의 로직 공유)
            if (WeeklyRewardManager.Instance != null) {
                // _Strength 값을 1.0으로 올림
                DOTween.To(() => 0f, x => {
                    if(WeeklyRewardManager.Instance.FrostedMaterial != null)
                        WeeklyRewardManager.Instance.FrostedMaterial.SetFloat("_Strength", x);
                }, 1.0f, _moveDuration).SetUpdate(true);
            }

            yield return new WaitForSecondsRealtime(_waitBeforeUI);

            // 5. UI 표시
            if (GameOverPanel != null) {
                GameOverPanel.SetActive(true);
                _canvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
            }
        }

        private void UpdateResultText() {
            if (ResultText == null) return;

            // 데이터 수집
            int days = 0;
            if (ClockProcess.Instance != null && ClockProcess.Instance.Model != null) {
                days = ClockProcess.Instance.Model.ExpansionDay;
            }

            int score = 0;
            if (ScoreManager.Instance != null) {
                score = ScoreManager.Instance.CurrentScore;
            }

            // 사용자 요청 문구 적용 (Bold 처리)
            ResultText.text = $"이 목적지에 제시간에 도달할 수 있는 차가 적습니다. 도시가 꽉 막혔습니다.\n" +
                              $"<b>{days}일</b> 동안 <b>{score}명</b>의 통근자가 도로를 이용했습니다.";
        }
    }
}
