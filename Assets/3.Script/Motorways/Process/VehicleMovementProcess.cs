using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Process {
	using Models;
	using Managers;
	using Utils;

	public class VehicleMovementProcess : MonoBehaviour, ISimulationProcess {
		public static VehicleMovementProcess Instance;

		public List<Vehicle> _activeVehicle = new List<Vehicle>();	//�����̴� ���� ����.
		private Dictionary<int, Vehicle> _vehicleMap = new Dictionary<int, Vehicle>();	//Vehicle ID�� ������ ã��.

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		public void RegisterVehicle(Vehicle v) {
			if (!_vehicleMap.ContainsKey(v.Id)) {
				_activeVehicle.Add(v);
				_vehicleMap.Add(v.Id, v);
			}
		}
		public Vehicle GetVehicle(int id) {
			_vehicleMap.TryGetValue(id, out Vehicle v);
			return v;
		}

		public void Tick(float dt) {
			Step(dt);
		}

		public void Step(float deltaTime) {
			for(int i = _activeVehicle.Count - 1; i >= 0; i--) {
				Vehicle v = _activeVehicle[i];

				switch (v.State) {
					case VehicleState.Driving:
					case VehicleState.Returning:
						ProcessDriving(v, deltaTime);
						break;
					case VehicleState.Arrived:
						ProcessParking(v, deltaTime);
						break;
				}
			}
		}

		private void ProcessDriving(Vehicle v, float deltaTime) {
			Lane lane = v.GetCurrentLane();
			if (lane == null) return;

			//��ǥ �ӵ� ���� (�⺻�� MaxSpeed)
			float targetSpeed = v.MaxSpeed;
			float distanceToObstacle = float.MaxValue;

			//�������� �Ÿ� üũ (���� ���� ��)
			foreach(int otherId in lane.VehiclesOnLane) {
				if (otherId == v.Id) continue;
				Vehicle other = GetVehicle(otherId);
				if (other == null) continue;

				//������ �տ� �ִ� ������ �Ÿ� ���.
				if (other != null && other.DistanceAlongLane > v.DistanceAlongLane) {
					distanceToObstacle = Mathf.Min(distanceToObstacle, other.DistanceAlongLane - v.DistanceAlongLane);
				}
			}

			//������ ������ ���� ������ ���� Ȯ�� (Lookahead)
			if (distanceToObstacle > 5f && v.CurrentPath.Count > 1) {
			    Lane nextLane = v.CurrentPath.ToArray()[1];
			    foreach (int otherId in nextLane.VehiclesOnLane) {
			        Vehicle other = GetVehicle(otherId);
			        if (other != null) {
			            float gap = (lane.Length - v.DistanceAlongLane) + other.DistanceAlongLane;
			            distanceToObstacle = Mathf.Min(distanceToObstacle, gap);
			        }
			    }
			}

			//�������� �Ÿ��� ���� �ӵ� ���� 
			if (distanceToObstacle < v.MinGap * 3f) {
				//0���� �ִ� �ӵ����� �Ÿ� ���̸�ŭ ������ ����.
				targetSpeed = Mathf.Lerp(0, v.MaxSpeed, (distanceToObstacle - v.MinGap) / (v.MinGap * 2f));
			}

			//����/���� ����.
			if(v.CurrentSpeed < targetSpeed) {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, targetSpeed, v.Acceleration * deltaTime);
			} else {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, targetSpeed, v.Deceleration * deltaTime);
			}
			//v.CurrentSpeed = v.MaxSpeed; �ش� �ܼ� �ӵ� �ڵ尡, ����� ���� �Ÿ� ��� �� ����/������ ������� �þ!

			//��ġ ������Ʈ.
			v.DistanceAlongLane += v.CurrentSpeed * deltaTime;;
			UpdateVisualPosition(v, lane);

			//���� ����
			if(v.DistanceAlongLane >= lane.Length) {
				float overflow = v.DistanceAlongLane - lane.Length;

				lane.Exit(v.Id);
				lane.CancelReservation(v.Id);
				v.CurrentPath.Dequeue();
				v.DistanceAlongLane = overflow;

				Lane nextLane = v.GetCurrentLane();
				if(nextLane != null) {
					nextLane.Enter(v.Id);
				} else {
					HandleArrival(v);
				}
			}
		}
		private void ProcessParking(Vehicle v, float deltaTime) {
			v.ParkingTimer += deltaTime;
			if (v.ParkingTimer >= v.ParkingDuration) {
				StartReturnTrip(v);
			}
		}

		private void HandleArrival(Vehicle v) {
			if(v.State == VehicleState.Driving) {
				//Desination �ǹ����� �˸�. (Event�� ó��)
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;
			} else if (v.State == VehicleState.Returning) {
				//�� �����ߴٰ� �˸�.
				v.State = VehicleState.Ready;

				v.ClearAllReservations();
			}

			// ������ ���� �� ���� ó�� (���� ��)
		}

		private void StartReturnTrip(Vehicle v) {
			if(v.ReturnPath != null && v.ReturnPath.Count > 0) {
				v.CurrentPath = v.ReturnPath;
				v.ReturnPath = new Queue<Lane>();
				v.CurrentLaneIndex = 0;
				v.DistanceAlongLane = 0f;
				v.State = VehicleState.Returning;

				v.GetCurrentLane()?.Enter(v.Id);
				//�� �����ٰ� �˸�.
			}
		}

		private void UpdateVisualPosition(Vehicle v, Lane lane) {
			float t = Mathf.Clamp01(v.DistanceAlongLane / lane.Length);

			// 차선의 시작점과 끝점 (타일 중심)
			Vector3 p0 = new Vector3(lane.StartNode.x + 0.5f, 0, lane.StartNode.y + 0.5f);
			Vector3 p1 = new Vector3(lane.EndNode.x + 0.5f, 0, lane.EndNode.y + 0.5f);

			Vector3 currentPos;
			Vector3 forward;

			// 차선 자체가 곡선인 경우 (현재는 직선 위주지만 구조는 유지)
			if (lane.IsCurved) {
				Vector3 cp = new Vector3(lane.ControlPoint.Value.x, 0, lane.ControlPoint.Value.y);
				currentPos = BezierUtils.GetPoint(p0, cp, p1, t);
				forward = BezierUtils.GetTangent(p0, cp, p1, t);
			} else {
				currentPos = Vector3.Lerp(p0, p1, t);
				forward = (p1 - p0).normalized;

				// --- 원작 스타일 코너 스무딩 ---
				// 차선의 끝부분(80% 이후)에서 다음 차선의 방향을 미리 반영하여 부드럽게 꺾습니다.
				// 단, t=1일 때 위치가 p1과 정확히 일치해야 덜컥거림이 없습니다.
				float smoothStart = 0.8f;
				if (v.CurrentPath.Count > 1 && t > smoothStart) {
					Lane nextLane = v.CurrentPath.ToArray()[1];
					Vector3 p2 = new Vector3(nextLane.EndNode.x + 0.5f, 0, nextLane.EndNode.y + 0.5f);
					Vector3 nextForward = (p2 - p1).normalized;

					if (Vector3.Dot(forward, nextForward) < 0.99f) {
						float localT = (t - smoothStart) / (1f - smoothStart);
						// 위치는 그대로 p0->p1 선상에 두되, 방향(Rotation)만 미리 꺾어주거나
						// 아주 미세한 베지어 보간을 사용하되 t=1에서 p1에 도착하게 설계합니다.

						// 방향 보간
						forward = Vector3.Slerp(forward, nextForward, localT);
					}
				}
			}

			// --- 우측 통행 오프셋 (원작 수치 0.2) ---
			// 진행 방향의 오른쪽으로 오프셋을 줍니다.
			Vector3 right = new Vector3(forward.z, 0, -forward.x).normalized;
			v.transform.position = currentPos + right * 0.2f;

			if (forward != Vector3.zero) {
				v.transform.rotation = Quaternion.LookRotation(forward);
			}
		}
	}
}
