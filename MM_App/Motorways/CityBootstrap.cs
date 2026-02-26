using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Motorways
{
	// Token: 0x02000365 RID: 869
	public class CityBootstrap : MonoBehaviour
	{
		// Token: 0x06001546 RID: 5446 RVA: 0x0004905F File Offset: 0x0004725F
		private void Awake()
		{
			SceneManager.LoadScene(1, LoadSceneMode.Additive);
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x00049068 File Offset: 0x00047268
		private void Update()
		{
			if (!this._hasStartedGame && SceneManager.GetSceneByName("Runtime").IsValid())
			{
				this.EnableRuntime();
				this._hasStartedGame = true;
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x000490A0 File Offset: 0x000472A0
		private void EnableRuntime()
		{
			AppRuntime runtime = Resources.FindObjectsOfTypeAll<AppRuntime>()[0];
			if (!string.IsNullOrEmpty(this._playbackAppJournalPath))
			{
				runtime._playbackAppJournalPath = this._playbackAppJournalPath;
			}
			runtime.gameObject.SetActive(true);
		}

		// Token: 0x040011C9 RID: 4553
		public CityDefinition cityDefinition;

		// Token: 0x040011CA RID: 4554
		private bool _hasStartedGame;

		// Token: 0x040011CB RID: 4555
		[HideInInspector]
		public string _playbackAppJournalPath;

		// Token: 0x040011CC RID: 4556
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("CityBootstrap");
	}
}
