using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayFireBallSoundEffect(){
		AudioSource blast = new AudioSource();
		blast.clip = Resources.Load<AudioClip>("music/SoundEffect/fireball");
		blast.Play();
		print("WHEEEEEEEEEEEEEEEEEEEE");
	}
}
