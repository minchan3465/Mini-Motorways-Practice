using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect {
	BridgeBuild,
	ButtonMain,
	ButtonSub,
	ClockClose,
	ClockOpen,
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
	TimeFast,
	TimeSlow
}


public class SoundManager : MonoBehaviour {
	public static SoundManager Instance = null;

	private void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad(this);
			Instance = this;
		} else { 
			Destroy(gameObject); 
		}
		

	}

	

}
