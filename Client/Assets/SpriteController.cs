using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {
    //private System.Action callback;
    private Animator animator;
    private int baseLayerIndex;
	// Use this for initialization
	void Start () {
        animator = gameObject.GetComponent<Animator>();
        baseLayerIndex = animator.GetLayerIndex("Base Layer");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HideSprite()
    {
        transform.position = new Vector2(0, -100);
        Debug.Log(gameObject.name + " hide sprite!");
        //isFinished = true;
    }

    public void AnimationFinished()
    {

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
            Debug.Log(gameObject.name + " is waiting for finish!");
            yield return null;
        }
    }

    public void SetTrigger(string trigger)
    {
        Debug.Log(gameObject.name + ":set trigger for " + trigger);
        animator.SetTrigger(trigger);
    }

    public void SetBool(string property, bool value)
    {
        animator.SetBool(property, value);
        //isFinished = false;
    }
}
