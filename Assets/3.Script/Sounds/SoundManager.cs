using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect {
	BridgeBuild,
	ButtonMain,
	ButtonSub,
	ClockClose,
	ClockOpen,
	ClockSound,
	DestinationBuild ,
	HouseBuild1,
	HouseBuild2,
	OverCrowdingDown,
	OverCrowdingUp,
	PinCollect1,
	PinCollect2,
	PinCreate,
	RoadBuild,
	RoadRemove,
	SelecteSE1,
	SelecteSE2,
	SelecteSE3,
	SwingCamera,
	TimeFast,
	TimeSlow
}

public class SoundManager : MonoBehaviour {
	public static SoundManager Instance = null;

	[Header("AudioClip")]
	public AudioClip[] SFX;

	private Dictionary<SoundEffect, AudioClip> sfxDictionary;	//Ä³½Ì
	private AudioSource SFXPlayer;

	private void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad(this);
			Instance = this;
		} else { 
			Destroy(gameObject); 
		}

		TryGetComponent(out SFXPlayer);
		Initialize();
	}

	private void Initialize() {
		sfxDictionary = new Dictionary<SoundEffect, AudioClip>();

		for(int i = 0; i < SFX.Length; i++) {
			if (!sfxDictionary.ContainsKey((SoundEffect)i)) {
				sfxDictionary.Add((SoundEffect)i, SFX[i]);
			}
		}
	}

	public void PlaySFX(SoundEffect _SoundEffect) {
		if(sfxDictionary.TryGetValue(_SoundEffect, out AudioClip clip)) {
			if(clip != null) {
				SFXPlayer.PlayOneShot(clip);
			}
		}
	}
}
