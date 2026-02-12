using System;
using Easing;
using Motorways;
using Motorways.Views;
using TMPro;
using UnityEngine;

// Token: 0x020001D6 RID: 470
public class NewsletterModal : MonoBehaviour
{
	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06000B35 RID: 2869 RVA: 0x00025FE3 File Offset: 0x000241E3
	private Vector3 HiddenPosition
	{
		get
		{
			return this._activePosition + Vector3.up * this._rect.sizeDelta.y * 1f;
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00026014 File Offset: 0x00024214
	private void Update()
	{
		if (this.positionTween.IsActive)
		{
			this.positionTween.Tick(Time.deltaTime);
			this._rect.anchoredPosition = this.positionTween.Value;
		}
		else if (!this._isShowing)
		{
			this._canvas.alpha = 0f;
			this._canvas.interactable = false;
			this._canvas.blocksRaycasts = false;
		}
		if (this._isShowing)
		{
			this._timeActive += Time.deltaTime;
		}
		if (Input.anyKey || Input.touchCount > 0)
		{
			this._timeActive = 0f;
		}
		if (this._timeActive > 114f)
		{
			this._timeActive = 0f;
			this.HideModal();
		}
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x000260DE File Offset: 0x000242DE
	public void OnEmailEntered(string email)
	{
		this._emailToAdd = email;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x000260E7 File Offset: 0x000242E7
	public void OnConfirmSubscribe()
	{
		this.HideModal();
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnPrintSubscriptions()
	{
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x000260F0 File Offset: 0x000242F0
	public void ShowModal()
	{
		if (Vector3.Distance(this._rect.anchoredPosition, this.HiddenPosition) < 1f)
		{
			this.positionTween.Start(this._rect.anchoredPosition, this._activePosition, 0.5f, Easings.Functions.BackEaseOut, 0f);
			this._isShowing = true;
			this._timeActive = 0f;
			TMP_InputField emailInputField = this._emailInputField;
			if (emailInputField != null)
			{
				emailInputField.ActivateInputField();
			}
			this._canvas.alpha = 1f;
			this._canvas.interactable = true;
			this._canvas.blocksRaycasts = true;
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x00026198 File Offset: 0x00024398
	public void HideModal()
	{
		if (Vector3.Distance(this._rect.anchoredPosition, this._activePosition) < 1f)
		{
			this.positionTween.Start(this._rect.anchoredPosition, this.HiddenPosition, 0.5f, Easings.Functions.BackEaseIn, 0.2f);
			if (this._emailInputField != null)
			{
				this._emailInputField.text = "";
			}
			this._emailToAdd = "";
			this._isShowing = false;
		}
		TMP_InputField emailInputField = this._emailInputField;
		if (emailInputField == null)
		{
			return;
		}
		emailInputField.DeactivateInputField(false);
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00026238 File Offset: 0x00024438
	public void Awake()
	{
		this._rect = base.GetComponent<RectTransform>();
		this._activePosition = this._rect.anchoredPosition;
		this._rect.anchoredPosition = this.HiddenPosition;
		this._canvas = (this._canvas ?? base.GetComponent<CanvasGroup>());
	}

	// Token: 0x04000656 RID: 1622
	[SerializeField]
	private TMP_InputField _emailInputField;

	// Token: 0x04000657 RID: 1623
	[SerializeField]
	private GameOverScreen _gameOverScreen;

	// Token: 0x04000658 RID: 1624
	[SerializeField]
	private MainMenuScreen _mainMenuScreen;

	// Token: 0x04000659 RID: 1625
	private bool _isShowing;

	// Token: 0x0400065A RID: 1626
	[SerializeField]
	private CanvasGroup _canvas;

	// Token: 0x0400065B RID: 1627
	private Vector3 _activePosition;

	// Token: 0x0400065C RID: 1628
	private RectTransform _rect;

	// Token: 0x0400065D RID: 1629
	private string _emailToAdd = "";

	// Token: 0x0400065E RID: 1630
	private TweenVector3 positionTween = new TweenVector3();

	// Token: 0x0400065F RID: 1631
	private float _timeActive;
}
