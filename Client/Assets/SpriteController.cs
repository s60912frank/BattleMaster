using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {
    //private System.Action callback;
    private bool isFinished;
    private Animator animator;
	// Use this for initialization
	void Start () {
        isFinished = true;
        animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HideSprite()
    {
        transform.position = new Vector2(0, -100);
        //if (callback != null) callback();
        Debug.Log(gameObject.name + " hide sprite!");
        isFinished = true;
    }

    public void AnimationFinished()
    {
        isFinished = true;
    }

    //this might not be a good idea
    /*public void SetFinishCallback(System.Action callback)
    {
        this.callback = callback;
    }

    public void ClearCallback()
    {
        callback = null;
    }*/

    public IEnumerator WaitForFinish()
    {
        while (!isFinished)
        {
            Debug.Log(gameObject.name + " is waiting for finish!");
            yield return null;
        }
    }

    public void SetTrigger(string trigger)
    {
        Debug.Log(gameObject.name + ":set trigger for " + trigger);
        animator.SetTrigger(trigger);
        isFinished = false;
    }
}
