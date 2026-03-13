using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Views {
	[RequireComponent(typeof(LineRenderer))]
	public class RoadPreviewView : MonoBehaviour {
		public static RoadPreviewView Instance;

		private LineRenderer _lineRenderer;

		[Header("Settings")]
		[SerializeField] private float _width = 0.8f;
		[SerializeField] private Color _previewColor = new Color(1, 1, 1, 0.6f); //������ ���
		[SerializeField] private int _roundness = 8;

		private void Awake() {
			if (Instance == null) Instance = this;
			else {
				Destroy(gameObject);
				return;
			}

			if (TryGetComponent(out _lineRenderer)) {
				//�� �ձ۰�...
				_lineRenderer.numCapVertices = _roundness;
				_lineRenderer.numCornerVertices = _roundness;

				_lineRenderer.startWidth = _width;
				_lineRenderer.endWidth = _width;
				_lineRenderer.positionCount = 2;
				_lineRenderer.numCapVertices = 8;
				_lineRenderer.useWorldSpace = true;

				_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
				_lineRenderer.startColor = _previewColor;
				_lineRenderer.endColor = _previewColor;
			}

			gameObject.SetActive(false);
		}

		public void UpdatePreview(Vector3 origin, Vector3 mousePos, Vector2Int snappedDir) {
			if (!gameObject.activeSelf) gameObject.SetActive(true);

			//�ڿ� üũ: ���ΰ� ������ 0.2ĭ�� �þ
			bool hasResource = ResourceManager.Instance.HasResource(ItemType.Road);
			float limit = 1.5f;

			Vector3 dirVec = new Vector3(snappedDir.x, 0, snappedDir.y).normalized;
			float projection = Vector3.Dot(mousePos - origin, dirVec);
			float length = Mathf.Clamp(projection, 0f, limit);

			Vector3 startPos = origin + Vector3.up * 0.1f;
			Vector3 endPos = startPos + (dirVec * length);


			//SetPosition ȣ�� �� �� ���� ��Ȯ�� (���� ��õ ����)
			if (_lineRenderer.positionCount != 2) _lineRenderer.positionCount = 2;
			_lineRenderer.SetPosition(0, startPos);
			_lineRenderer.SetPosition(1, endPos);
		}

		public void Hide() {
			if (this != null && gameObject.activeSelf) gameObject.SetActive(false);
		}
	}
}
