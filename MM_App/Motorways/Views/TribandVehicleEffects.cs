using System;
using Client;
using Factory.Pools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Motorways.Views
{
	// Token: 0x0200060C RID: 1548
	public class TribandVehicleEffects : MonoBehaviour, IReusable
	{
		// Token: 0x06002B47 RID: 11079 RVA: 0x000BE88C File Offset: 0x000BCA8C
		public TickResult Tick(TimeInterval timeInterval, float stepAlpha)
		{
			if (this._lastPosition == Vector3.zero)
			{
				this._lastPosition = base.transform.position;
			}
			float startRotation = -base.transform.rotation.eulerAngles.z * 0.017453292f;
			for (int footIndex = 0; footIndex < this._feetParticleSystems.Length; footIndex++)
			{
				this._feetParticleSystems[footIndex].main.startRotation = startRotation;
			}
			float distanceTravelled = (base.transform.position - this._lastPosition).magnitude;
			this._distanceSinceSpawn += distanceTravelled;
			if (this._distanceSinceSpawn > this._spawnInterval)
			{
				this._feetParticleSystems[this._nextParticleSystemIndex].Emit(1);
				this._nextParticleSystemIndex = (this._nextParticleSystemIndex + 1) % this._feetParticleSystems.Length;
				this._distanceSinceSpawn = 0f;
			}
			this._lastPosition = base.transform.position;
			return TickResult.ContinueTicking;
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x000BE98E File Offset: 0x000BCB8E
		public void Reset()
		{
			this._lastPosition = default(Vector3);
			this._distanceSinceSpawn = 0f;
			this._nextParticleSystemIndex = 0;
		}

		// Token: 0x04002561 RID: 9569
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("TribandVehicleEffects");

		// Token: 0x04002562 RID: 9570
		[SerializeField]
		private ParticleSystem[] _feetParticleSystems;

		// Token: 0x04002563 RID: 9571
		[FormerlySerializedAs("_spawnIntervalSquared")]
		[Tooltip("The distance travelled between each particle spawns")]
		[SerializeField]
		[FormerlySerializedAs("_spawnDelaySquared")]
		private float _spawnInterval;

		// Token: 0x04002564 RID: 9572
		private Vector3 _lastPosition;

		// Token: 0x04002565 RID: 9573
		private float _distanceSinceSpawn;

		// Token: 0x04002566 RID: 9574
		private int _nextParticleSystemIndex;
	}
}
