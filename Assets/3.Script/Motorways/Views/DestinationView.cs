using UnityEngine;
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

		[Header("Pins & Gauge")]
		[SerializeField] private GameObject Pins;
		[SerializeField] private PinView[] _normalPins;   // 6개 (3x2)
		[SerializeField] private PinView[] _overflowPins; // 4개 (맨 아래)
		[SerializeField] private GameObject _timerPinGroup;  // 큰 게이지 부모
		[SerializeField] private Renderer _timerGaugeRenderer; // 쉐이더로 제어할 렌더러 (SpriteRenderer or MeshRenderer)

		private Destination _model;
		private bool _isHorizontal;

		private bool _isOverCrowdingDownSound = false;
		private bool _isOverCrowdingUpSound = true;
		// 갑작스러운 게이지 감소 시 회색 잔상(Ghost)을 보여주기 위한 변수
		private float _ghostRatio = 0f;

		public void Initialize(Destination model) {
			_model = model;
		}

		//isHorizontal	: true면 가로형(3x2), false면 세로형(2x3)
		//isPositive	: true면 위/왼쪽 입구, false면 아래/오른쪽 입구
		public void UpdateVisuals(bool isHorizontal, bool isPositive) {
			SoundManager.Instance.PlaySFX(SoundEffect.DestinationBuild);

			_isHorizontal = isHorizontal;
			
			if (isHorizontal) {
				West.SetActive(true);
				South.SetActive(false);
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
			SetPinsPos();
		}

		private void Update() {
			if (_model == null) return;
			RefreshPins();
		}

		private void RefreshPins() {
			int demand = _model.TotalDemand;
			
			// 실제 타이머 위험도 (0: 안전, 1: 게임오버)
			float ratio = 1.0f - (_model.OverCrowdingTimer / 30.0f);
			
			// 고스트 비율 업데이트 로직:
			// 실제 위험도가 증가하면 고스트도 즉시 따라가고,
			// 실제 위험도가 깎이면(차량 도착) 고스트는 천천히 줄어듭니다.
			if (ratio > _ghostRatio) {
				_ghostRatio = ratio;
			} else {
				_ghostRatio = Mathf.MoveTowards(_ghostRatio, ratio, Time.deltaTime * 0.5f); // 1초에 50%씩 부드럽게 감소
			}

			// 게이지 표시 조건: 수요가 6개 이상이거나, 잔상(Ghost)이 아직 남아있을 때
			bool showGauge = demand >= Destination.GAUGE_START_PINS || _ghostRatio > 0.01f;

			if (!showGauge) {
				// 게이지 숨김, 일반 핀 표시
				if (_timerPinGroup != null && _isOverCrowdingDownSound == true) {
					SoundManager.Instance.PlaySFX(SoundEffect.OverCrowdingDown);
					_isOverCrowdingDownSound = false;
					_timerPinGroup.SetActive(false);
					_isOverCrowdingUpSound = true;
				}
				for (int i = 0; i < _normalPins.Length; i++) {					
					if (_normalPins[i] != null) _normalPins[i].SetVisibility(i < demand);
				}
				for (int i = 0; i < _overflowPins.Length; i++) {
					if (_overflowPins[i] != null) _overflowPins[i].SetVisibility(false);
				}
			} else {
				// 게이지 표시, 일반 핀 6개는 게이지로 합체되었으므로 숨김
				for (int i = 0; i < _normalPins.Length; i++) {
					if (_normalPins[i] != null) _normalPins[i].SetVisibility(false);
				}
				if (_timerPinGroup != null && _isOverCrowdingUpSound == true) {
					SoundManager.Instance.PlaySFX(SoundEffect.OverCrowdingUp);
					_isOverCrowdingUpSound = false;
					_timerPinGroup.SetActive(true); 
					_isOverCrowdingDownSound = true;
				}

				if (_timerGaugeRenderer != null) {
					// 쉐이더로 값 전달
					// _FillAmount: 꼬리로 남는 전체 길이 (회색 부분 포함)
					// _PreviewAmount: 진짜 칠해져야 할 빨간색 영역
					_timerGaugeRenderer.material.SetFloat("_FillAmount", _ghostRatio);
					_timerGaugeRenderer.material.SetFloat("_PreviewAmount", ratio);

					// [수정] 후반부에 더 급격하게 빨간색으로 변하도록 곡선(Pow) 적용
					float colorCurve = Mathf.Pow(ratio, 1.5f);
					Color targetColor = Color.Lerp(new Color(0.15f, 0.15f, 0.15f), Color.red, colorCurve);
					_timerGaugeRenderer.material.SetColor("_Color", targetColor);
				}

				// 추가 핀(7~10번째) 표시. (수요가 떨어져도 0 이하로 내려가지 않도록 처리)
				int overflowCount = Mathf.Max(0, demand - Destination.GAUGE_START_PINS);
				for (int i = 0; i < _overflowPins.Length; i++) {
					if (_overflowPins[i] != null) _overflowPins[i].SetVisibility(i < overflowCount);
				}
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

			// 핀 색상 동기화
			if (_normalPins != null) {
				foreach (var pin in _normalPins) {
					if (pin != null) pin.SetColor(colorSet.Base);
				}
			}
			if (_overflowPins != null) {
				foreach (var pin in _overflowPins) {
					if (pin != null) pin.SetColor(colorSet.Base);
				}
			}

			Vector3 spawnPos;
			if (_isHorizontal) spawnPos = West.transform.position;
			else spawnPos = South.transform.position;
			spawnPos += new Vector3(0, 0.5f, 0f);
			GameObject effect = Instantiate(effect_prefab, spawnPos, Quaternion.identity);

			if (effect.TryGetComponent(out BuildingSpawnCircle component)) {
				component.SpawnEffect(groupIndex, isHouse: false);
			}
		}

		private void SetPinsPos() {
			if (Pins == null) return;
			
			if(_isHorizontal) {
				Pins.transform.localPosition = Vector3.zero;
				Pins.transform.localRotation = Quaternion.identity;
			} else {
				// 월드 좌표가 아닌 지역(Local) 좌표와 회전을 사용해야 건물을 따라갑니다.
				Pins.transform.localPosition = new Vector3(1f, 0, 1f); // 사용자님이 설정한 오프셋 유지
				Pins.transform.localRotation = Quaternion.Euler(0, 90f, 0);
			}
		}
	}
}
