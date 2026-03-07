using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Motorways.Process;
using Motorways.Models;
using Motorways.Managers;
using DG.Tweening;

namespace Motorways.UI {
	public class ClockView : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] private Transform _clockHand_transform; 
        [SerializeField] private RawImage _clockPip;
        [SerializeField] private RawImage _clockHand;
        [SerializeField] private RawImage _clockFace;
        [SerializeField] private TextMeshProUGUI _dayText; 
        [SerializeField] private Color _dayColor;
        [SerializeField] private Color _nightColor;
        [SerializeField] private TimerVisualManager _timerManager;

        private readonly string[] _dayNames = { "월", "화", "수", "목", "금", "토", "일" };
        private int _lastDayIndex = -1;

        private bool _isDaytime = true;

        private void Start() {
            if (ClockProcess.Instance != null && ClockProcess.Instance.Model != null) {
                CheckDayNight(ClockProcess.Instance.Model, true);
            }
        }

        private void Update() {
            if (ClockProcess.Instance == null || ClockProcess.Instance.Model == null) return;

            ClockModel model = ClockProcess.Instance.Model;

            UpdateClockHandRotation(model);
            UpdateDayText(model);

            CheckDayNight(model);
            RefreshPipColor(); 
        }

        private void RefreshPipColor() {
            if (SimulationManager.Instance == null || _clockPip == null) return;

            if (SimulationManager.Instance.IsPaused) {
                Color stopColor = _timerManager != null ? _timerManager.GetStopColor() : Color.red;
                if (_clockPip.color != stopColor) {
                    TransitionToColor(stopColor);
                }
            } else {
                Color targetColor = _isDaytime ? _dayColor : _nightColor;
                if (_clockPip.color != targetColor) {
                    TransitionToColor(targetColor);
                }
            }
        }

        private void UpdateClockHandRotation(ClockModel model) {
            if (_clockHand == null) return;
            float exactHour = model.Time / ClockModel.SecondsPerHour;
            float angle = exactHour * 30f;
            _clockHand_transform.localRotation = Quaternion.Euler(0, 0, -angle);
        }

        private void UpdateDayText(ClockModel model) {
            if (_dayText == null) return;
            int currentDayIndex = model.Day % 7;
            if (currentDayIndex != _lastDayIndex) {
                _lastDayIndex = currentDayIndex;
                _dayText.text = _dayNames[currentDayIndex];
            }
        }

        private void CheckDayNight(ClockModel model, bool forceUpdate = false) {
            int currentHourOfDay = model.Hour % 24;
            bool isNowDaytime = (currentHourOfDay >= 6 && currentHourOfDay < 18);

            if (isNowDaytime != _isDaytime || forceUpdate) {
                _isDaytime = isNowDaytime;

                if (_isDaytime) {
                    _clockHand.DOColor(_nightColor, 0.3f);
                    _clockFace.DOColor(_nightColor, 0.3f);
                } else {
                    _clockHand.DOColor(_dayColor, 0.3f);
                    _clockFace.DOColor(_dayColor, 0.3f);
                }
            }
        }

        public void TransitionToColor(Color color) {
            _clockPip.DOKill();
            _clockPip.DOColor(color, 0.3f);
        }
    }
}
