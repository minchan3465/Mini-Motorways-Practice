using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Motorways;
using Motorways.Utils;

public class RuntimeController : MonoBehaviour {
	[Header("Settings")]
	public Camera mainCamera;
	public Transform startPoint;
	public Transform cornerPoint;
	public Transform endPoint;
	public float duration = 0.5f;

	[Header("Hide UI")]
	public GameObject pauseUI;
	public GameObject gameoverUI;

	[Header("Runtime UI Show")]
	public GameObject clockUI;
	public GameObject inventoryUI;
	private Vector3 clockUI_originPos;
	private Vector3 inventoryUI_originPos;

	[Header("Change Scene")]
	public string SceneName = "Main";

	private void Awake() {
		clockUI_originPos = clockUI.transform.position;
		inventoryUI_originPos = inventoryUI.transform.position;

		if (mainCamera == null) mainCamera = Camera.main;
	}

	private void Start() {
		if (GameStateManager.CameFromMainMenu) {
			GameStateManager.CameFromMainMenu = false;
			MovementToStartPoint();
		}
	}

	public void MovementToEndPoint() {
		pauseUI.SetActive(false);
		gameoverUI.SetActive(false);

		Vector3 startPos = startPoint.position;
		Vector3 cornerPos = cornerPoint.position;
		Vector3 endPos = endPoint.position;

		WhiteoutController.Instance.OffWhiteOut();

		SoundManager.Instance.PlaySFX(SoundEffect.SwingCamera);

		DOVirtual.Float(0f, 1f, duration, (t) => {
			mainCamera.transform.position = BezierUtils.GetPoint(startPos, cornerPos, endPos, t);
		}).SetEase(Ease.InCubic).OnComplete(() => {
			GameStateManager.CameFromRuntime = true;
			StartCoroutine(LoadSceneAsync(SceneName));
		});
	}

	private void MovementToStartPoint() {
		mainCamera.transform.position = endPoint.position;
		clockUI.transform.position += Vector3.up * 100f;
		inventoryUI.transform.position += Vector3.down * 100f;

		Vector3 startPos = endPoint.position;
		Vector3 cornerPos = cornerPoint.position;
		Vector3 endPos = startPoint.position;

		DOVirtual.Float(0f, 1f, duration, (t) => {
			mainCamera.transform.position = BezierUtils.GetPoint(startPos, cornerPos, endPos, t);
		}).SetEase(Ease.OutCubic);

		StartCoroutine(UI_move());
	}

	//비동기 로딩
	private IEnumerator LoadSceneAsync(string sceneName) {
		DOTween.KillAll();
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		while (!asyncLoad.isDone) {
			yield return null;
		}
	}

	//급하게 넣은 UI 움직임 ㅎㅎ;
	private IEnumerator UI_move() {
		yield return new WaitForSeconds(2.5f);
		inventoryUI.transform.DOMove(inventoryUI_originPos, 0.25f);
		yield return new WaitForSeconds(0.5f);
		clockUI.transform.DOMove(clockUI_originPos, 0.25f);
	}
}
