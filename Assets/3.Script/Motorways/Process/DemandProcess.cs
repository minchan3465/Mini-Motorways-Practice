using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Managers;

	public class DemandProcess : MonoBehaviour, ISimulationProcess {
		public static DemandProcess Instance;

		private List<Destination> _activeDestinations = new List<Destination>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		private void OnDestroy() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RemoveProcess(this);
			}
		}

		public void RegisterDestination(Destination destination) {
			if(!_activeDestinations.Contains(destination)) {
				_activeDestinations.Add(destination);
			}
		}

		public void Tick(float dt) {
			//일수(Day)가 지날수록 핀 생성 속도(시간 흐름)가 빨라집니다.
			float spawnScale = 1.0f;
			if (ClockProcess.Instance != null && ClockProcess.Instance.Model != null) {
				int startDay = 7; //예: 7일(1주) 후부터 난이도 증가 시작
				float dailyIncrement = 0.05f; //하루마다 생성 속도 5%씩 증가
				int daysPast = ClockProcess.Instance.Model.ExpansionDay - startDay;

				if (daysPast > 0) {
					spawnScale += daysPast * dailyIncrement;
				}
			}

			for (int i = 0; i < _activeDestinations.Count; i++) {
				var dest = _activeDestinations[i];
				if (!dest.isActive) continue;

				UpdatePinGeneration(dest, dt, spawnScale);
				UpdateOvercrowding(dest, dt);
			}
		}

		private void UpdatePinGeneration(Destination dest, float dt, float spawnScale) {
			//최대 핀 개수를 넘지 않을 때만 생성
			if (dest.TotalDemand < Destination.MAX_PINS) {
				dest.PinSpawnTimer -= (dt * spawnScale);

				if (dest.PinSpawnTimer <= 0) {
					dest.UnassignedPins++;
					dest.PinSpawnTimer = 10.0f; 
				}
			}
		}

		private void UpdateOvercrowding(Destination dest, float dt) {
			if (dest.isOverCrowding) {
				//핀이 6개 이상이면 과밀화 시작
				//핀이 더 많을수록 더 빨리 소모되게 (벌칙 계수)
				float penalty = 1.0f + (dest.TotalDemand - Destination.GAUGE_START_PINS) * 0.2f;
				dest.OverCrowdingTimer -= dt * penalty;

				if (dest.OverCrowdingTimer <= 0) {
					//이미 게임 오버 시퀀스가 진행 중인지 확인하여 중복 호출 차단
					if (GameOverManager.Instance != null && !GameOverManager.Instance.IsSequenceActive) {
						Vector3 focusPoint = new Vector3(
							dest.OriginCoordinate.x * MapSettings.TILE_SIZE,
							0,
							dest.OriginCoordinate.y * MapSettings.TILE_SIZE
						);
						GameOverManager.Instance.TriggerGameOver(focusPoint);
					}
				}
			} else {
				//6개 미만이면 타이머 서서히 회복 (최대 30초)
				if (dest.OverCrowdingTimer < 30.0f) {
					dest.OverCrowdingTimer = Mathf.Min(30.0f, dest.OverCrowdingTimer + dt * 1.5f);
				}
			}
		}
	}
}
