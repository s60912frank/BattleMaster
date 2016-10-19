using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {
    //private System.Action callback;
    private Animator animator;
    private AudioSource audio;
    private int baseLayerIndex;
	// Use this for initialization
	void Start () {
        animator = gameObject.GetComponent<Animator>();
        audio = gameObject.GetComponent<AudioSource>();
        baseLayerIndex = animator.GetLayerIndex("Base Layer");

        try{
            if(PlayerPrefs.GetInt("SoundEffectOn") == 1){
                audio.volume = 0.75f;
            }
            else{
                audio.volume = 0f;
            }
        }
        catch(System.Exception e){}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HideSprite()
    {
        transform.position = new Vector2(0, -100);
    }

    public IEnumerator WaitForFinish()
    {
        //BAD IDEA!
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }
        while (!animator.GetCurrentAnimatorStateInfo(baseLayerIndex).IsName("Idle"))
        {
            yield return null;
        }
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void SetBool(string property, bool value)
    {
        animator.SetBool(property, value);
    }

    public void PlaySoundEffect(AudioClip sound){
		audio.clip = sound;
		audio.Play();
	}
}
