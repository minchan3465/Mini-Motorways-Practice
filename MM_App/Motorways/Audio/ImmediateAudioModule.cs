using System;

namespace Motorways.Audio
{
	// Token: 0x0200068F RID: 1679
	public class ImmediateAudioModule : IAudioModule
	{
		// Token: 0x06002E91 RID: 11921 RVA: 0x000D8B44 File Offset: 0x000D6D44
		public ImmediateAudioModule(AudioEventFilter filter, string sampleName, float gain = 1f, float pan = -1f, string moduleName = "", float pitch = 1f)
		{
			this.Filter = filter;
			this.SampleName = sampleName;
			this.Pan = pan;
			this.Gain = gain;
			this.ModuleName = moduleName;
			this.Pitch = pitch;
		}

		// Token: 0x06002E92 RID: 11922 RVA: 0x000D8B84 File Offset: 0x000D6D84
		public ImmediateAudioModule(AudioEventFilter filter)
		{
			this.Filter = filter;
		}

		// Token: 0x06002E93 RID: 11923 RVA: 0x000D8B9E File Offset: 0x000D6D9E
		public ImmediateAudioModule()
		{
		}

		// Token: 0x06002E94 RID: 11924 RVA: 0x000D8BB1 File Offset: 0x000D6DB1
		public void Activate(AudioEnvironment environment)
		{
			this.Environment = environment;
			this.EventListener.Start(new Action(this.AddEventListeners));
			this.OnActivate();
		}

		// Token: 0x06002E95 RID: 11925 RVA: 0x000D8BD8 File Offset: 0x000D6DD8
		public void Deactivate()
		{
			this.OnDeactivate();
			this.EventListener.Stop();
		}

		// Token: 0x06002E96 RID: 11926 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Release()
		{
		}

		// Token: 0x06002E97 RID: 11927 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void UpdateModule()
		{
		}

		// Token: 0x06002E98 RID: 11928 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnActivate()
		{
		}

		// Token: 0x06002E99 RID: 11929 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnDeactivate()
		{
		}

		// Token: 0x06002E9A RID: 11930 RVA: 0x000D8BEB File Offset: 0x000D6DEB
		protected virtual void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnAudioEventBase), this.Filter);
		}

		// Token: 0x06002E9B RID: 11931 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnAudioEvent(AudioEvent e)
		{
		}

		// Token: 0x06002E9C RID: 11932 RVA: 0x000D8C0A File Offset: 0x000D6E0A
		private void OnAudioEventBase(AudioEvent e)
		{
			this.OnAudioEvent(e);
		}

		// Token: 0x04002868 RID: 10344
		protected AudioEnvironment Environment;

		// Token: 0x04002869 RID: 10345
		protected AudioEventFilter Filter;

		// Token: 0x0400286A RID: 10346
		protected string SampleName;

		// Token: 0x0400286B RID: 10347
		protected string ModuleName;

		// Token: 0x0400286C RID: 10348
		protected float Pan;

		// Token: 0x0400286D RID: 10349
		protected float Gain;

		// Token: 0x0400286E RID: 10350
		protected float Pitch;

		// Token: 0x0400286F RID: 10351
		public AudioEventListener EventListener = new AudioEventListener();
	}
}
