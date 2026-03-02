# Mini Motorways Clone - Architecture & Development Guidelines

## 1. Road Preview (도로 건설 미리보기)
* **절대 타일맵(Chunk) 갱신으로 미리보기를 구현하지 말 것.** (프레임 드랍 원인)
* 원작의 `NewRoadPreview.cs` 방식을 따라, `RoadPreviewVisual`이라는 독립된 시각적 객체(LineRenderer 또는 별도 동적 Mesh)를 사용할 것.
* 드래그 시 시작점(Vector3)과 현재 마우스 위치(Vector3)를 이어주는 선을 렌더링하고, 건설이 확정될 때만 타일 데이터를 변경할 것.

## 2. Mothballed (임시 폐쇄/철거 대기) 도로 시각화
* Mothballed 상태(`RoadState.Mothballed`)인 도로는 렌더링에서 아예 제외(투명화)하는 것이 아님.
* `RoadChunkVisual.cs`에서 메쉬를 병합할 때, Mothballed 도로는 `_roadBuffer`가 아닌 `_mothballedBuffer`에 담아 처리할 것.
* SubMesh를 3개로 늘리고, 인덱스 2번에 `_mothballedBuffer`의 삼각형 데이터를 할당하여 원작처럼 별도의 머테리얼(`mothballedMaterial`)이 렌더링되도록 구현할 것.

## 3. 코드 최적화 및 SRP (단일 책임 원칙)
* `InteractionController.cs`는 너무 많은 책임을 가지고 있으므로 다음 3가지로 분할할 것:
  1. `PlayerInputManager`: 마우스/터치 Raycast 및 좌표 계산 전담
  2. `RoadBuilderAction`: 도로 건설/철거 및 집 회전 비즈니스 로직 전담
  3. `RoadPreviewVisual`: 드래그 중 시각적 미리보기 전담
* `CalculateSnappedDirection`과 같은 타일 방향 계산은 삼각함수(`Mathf.Atan2`) 대신 x, z 값의 크기와 부호(`Mathf.Sign`)를 직접 비교하여 연산량을 최소화할 것.
