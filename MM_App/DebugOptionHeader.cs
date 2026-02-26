using System;
using TMPro;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class DebugOptionHeader : MonoBehaviour
{
	// Token: 0x06000D9B RID: 3483 RVA: 0x0002CDC0 File Offset: 0x0002AFC0
	public void Initialize(string newHeaderText)
	{
		this.headerText.text = newHeaderText;
	}

	// Token: 0x040007BA RID: 1978
	public TMP_Text headerText;
}
