﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsScript : MonoBehaviour {
	private RectTransform rt;
	public Toggle SoundEffectToggle;
	public Toggle BgmToggle;
	public Toggle BattleTutToggle;
	public Toggle MiniGameTutToggle;
	// Use this for initialization
	void Awake(){
		rt = gameObject.GetComponent<RectTransform>();
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show(){
		SoundEffectToggle.isOn = (PlayerPrefs.GetInt("SoundEffectOn") == 1);
		BgmToggle.isOn = (PlayerPrefs.GetInt("BgmOn") == 1);
		BattleTutToggle.isOn = (PlayerPrefs.GetInt("showBattleTutorial") == 1);
		MiniGameTutToggle.isOn = (PlayerPrefs.GetInt("showMiniGameTutorial") == 1);
		rt.anchoredPosition = Vector2.zero;
	}

	public void ExitClicked(){
		PlayerPrefs.SetInt("SoundEffectOn", SoundEffectToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("BgmOn", BgmToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("showBattleTutorial", BattleTutToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("showMiniGameTutorial", MiniGameTutToggle.isOn ? 1 : 0);
		rt.anchoredPosition = new Vector2(1000, 0);

		AudioSource bgm = GameObject.Find("Audio Source").GetComponent<AudioSource>();
		if(BgmToggle.isOn){
            bgm.volume = 0.75f;
        }
        else{
            bgm.volume = 0f;
        }
	}
}
