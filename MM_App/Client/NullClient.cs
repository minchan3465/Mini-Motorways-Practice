using System;
using FixMath;
using Server;

namespace Client
{
	// Token: 0x0200079A RID: 1946
	public class NullClient : IClient, ISimulationObserver
	{
		// Token: 0x060035A1 RID: 13729 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Start()
		{
		}

		// Token: 0x060035A2 RID: 13730 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Tick(TimeInterval timeInterval, float stepAlpha)
		{
		}

		// Token: 0x060035A3 RID: 13731 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnModelAdded(ISimulation simulation, IModel element, Fix64 timestamp)
		{
		}

		// Token: 0x060035A4 RID: 13732 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnModelRemoved(ISimulation simulation, IModel model, Fix64 timestamp)
		{
		}

		// Token: 0x060035A5 RID: 13733 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ApplyTheme(ITheme theme)
		{
		}

		// Token: 0x060035A6 RID: 13734 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
		}
	}
}
