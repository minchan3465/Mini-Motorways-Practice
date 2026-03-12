using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Motorways.Utils;

public class MainMenuController : MonoBehaviour {
	[Header("Settings")]
	public Camera mainCamera;
	public Transform startPoint;
	public Transform cornerPoint;
	public Transform endPoint;
	public float duration = 0.5f;

	[Header("Material")]
	public Material plus_mat;
	public Material bg_mat;
	public Color origin_Color;
	public Color map_Color;
	public Color mapReturn_Color;

	[Header("Change Scene")]
	public string SceneName = "Runtime";

	private void Awake() {
		if (mainCamera == null) mainCamera = Camera.main;
	}

	private void Start() {
		if(GameStateManager.CameFromRuntime) {
			GameStateManager.CameFromRuntime = false;
			MovementToStartPoint();
		}
	}

	public void MovementToEndPoint() {
		Vector3 startPos = startPoint.position;
		Vector3 cornerPos = cornerPoint.position;
		Vector3 endPos = endPoint.position;

		Sequence seq = DOTween.Sequence();

		SoundManager.Instance.PlaySFX(SoundEffect.SwingCamera);
		seq.Append(
			DOVirtual.Float(0f, 1f, duration, (t) => {
				mainCamera.transform.position = BezierUtils.GetPoint(startPos, cornerPos, endPos, t);
			}).SetEase(Ease.InCubic).OnComplete(() => {
				GameStateManager.CameFromMainMenu = true;
				StartCoroutine(LoadSceneAsync(SceneName));
			})
		);
		seq.Join(plus_mat.DOFade(0, 1f).SetDelay(1f));
		seq.Join(bg_mat.DOColor(map_Color,1f));
	}

	private void MovementToStartPoint() {
		mainCamera.transform.position = endPoint.position;

		Vector3 startPos = endPoint.position;
		Vector3 cornerPos = cornerPoint.position;
		Vector3 endPos = startPoint.position;

		Sequence seq = DOTween.Sequence();
		seq.Append(
			DOVirtual.Float(0f, 1f, duration, (t) => {
				mainCamera.transform.position = BezierUtils.GetPoint(startPos, cornerPos, endPos, t);
			}).SetEase(Ease.OutCubic)
		);
		seq.Join(plus_mat.DOFade(1, 0.5f));
		seq.Join(bg_mat.DOColor(origin_Color, 0.5f));
	}

	//비동기 로딩
	private IEnumerator LoadSceneAsync(string sceneName) {
		DOTween.KillAll();
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		while(!asyncLoad.isDone) {
			yield return null;
		}
	}
}
