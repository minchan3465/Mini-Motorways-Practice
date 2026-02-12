using System;

// Token: 0x0200015E RID: 350
public class AxisState
{
	// Token: 0x060007B4 RID: 1972 RVA: 0x00018EC8 File Offset: 0x000170C8
	public virtual float GetAxisValue()
	{
		return this._axisValue;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00018ED0 File Offset: 0x000170D0
	public virtual void SetAxisValue(float newValue)
	{
		this._axisValue = newValue;
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Tick(float appTime)
	{
	}

	// Token: 0x0400038A RID: 906
	protected float _axisValue;
}
