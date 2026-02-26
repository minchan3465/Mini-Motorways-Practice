using System;
using TMPro;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class ControllerSymbol : MonoBehaviour
{
	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06000A9A RID: 2714 RVA: 0x00023187 File Offset: 0x00021387
	public bool IsUsingDefaultSymbol
	{
		get
		{
			return this.defaultControllerSymbol.activeInHierarchy;
		}
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00023194 File Offset: 0x00021394
	public void Initialize(IControllerButtonToSymbolService controllerButtonToSymbolService)
	{
		if (!this.shouldUseControllerButton || !controllerButtonToSymbolService.HasMappings)
		{
			this.UseDefaultSymbol();
			return;
		}
		string symbolNameForButton = controllerButtonToSymbolService.GetTextMeshProSymbolTextForControllerButton(this.controllerButton);
		if (!string.IsNullOrEmpty(symbolNameForButton))
		{
			this.UseTextSymbol(symbolNameForButton);
			return;
		}
		this.UseDefaultSymbol();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x000231DB File Offset: 0x000213DB
	public void UseDefaultSymbol()
	{
		this.defaultControllerSymbol.SetActive(true);
		if (this._symbol != null)
		{
			this._symbol.SetActive(false);
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00023204 File Offset: 0x00021404
	public void UseTextSymbol(string symbolCharacter)
	{
		if (this._symbol == null)
		{
			this._symbol = UnityEngine.Object.Instantiate<GameObject>(this.textControllerSymbol, this.baseRectTransform);
			this._symbol.transform.SetAsFirstSibling();
			RectTransform component = this._symbol.GetComponent<RectTransform>();
			RectTransform defaultControllerSymbolRectTransform = this.defaultControllerSymbol.GetComponent<RectTransform>();
			component.anchorMin = defaultControllerSymbolRectTransform.anchorMin;
			component.anchorMax = defaultControllerSymbolRectTransform.anchorMax;
			component.anchoredPosition = defaultControllerSymbolRectTransform.anchoredPosition;
			component.sizeDelta = defaultControllerSymbolRectTransform.sizeDelta;
		}
		if (this.defaultControllerSymbol != null)
		{
			this.defaultControllerSymbol.SetActive(false);
		}
		this._symbol.SetActive(true);
		this._symbol.GetComponent<TextMeshProUGUI>().text = symbolCharacter;
	}

	// Token: 0x040005AB RID: 1451
	public bool shouldUseControllerButton;

	// Token: 0x040005AC RID: 1452
	public ControllerButton controllerButton;

	// Token: 0x040005AD RID: 1453
	public RectTransform baseRectTransform;

	// Token: 0x040005AE RID: 1454
	public GameObject textControllerSymbol;

	// Token: 0x040005AF RID: 1455
	public GameObject defaultControllerSymbol;

	// Token: 0x040005B0 RID: 1456
	private GameObject _symbol;
}
