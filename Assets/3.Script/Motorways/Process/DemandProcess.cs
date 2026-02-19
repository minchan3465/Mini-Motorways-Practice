using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;
	using Motorways.Managers;

	//목적지에서 핀을 생성하는 프로세스입니다.
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
			foreach (var dest in _activeDestinations) {
				UpdatePinGeneration(dest, dt);
				UpdateOvercrowding(dest, dt);
			}
		}

		private void UpdatePinGeneration(Destination dest, float dt) {
			if(!dest.isOverCrowding) {
				dest.PinSpawnTimer -= dt;

				if (dest.PinSpawnTimer <= 0) {
					dest.UnassignedPins++;
					dest.PinSpawnTimer = 10.0f;	//추후 생산 시간이 줄어들게끔 설정하면 됨!
				}
			}

		}

		private void UpdateOvercrowding(Destination dest, float dt) {
			if(dest.isOverCrowding) {
				dest.OverCrowdingTimer -= dt;

				//TODO : UI 작업 처리 (원형 게이지)

				if(dest.OverCrowdingTimer <= 0) {
					// TODO : 게임 오버 처리
				} else {
					//TODO : 실제 기능은 타이머가 일정 취소됨. 이후 30.0f을 넘으면 다시 핀 6개로 변경 작업. (과부화 해제)
					if(dest.OverCrowdingTimer < 30.0f) {
						dest.OverCrowdingTimer += dt * 2.0f;
					}
				}
			}
		}
	}
}
