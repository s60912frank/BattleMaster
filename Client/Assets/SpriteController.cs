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
        Debug.Log(gameObject.name + " hide sprite!");
        isFinished = true;
    }

    public void AnimationFinished()
    {
        isFinished = true;
    }

    public IEnumerator WaitForFinish()
    {
        //failsafe,finding bugs....
        int counter = 60;
        while (!isFinished && counter > 0)
        {
            Debug.Log(gameObject.name + " is waiting for finish!");
            counter--;
            yield return null;
        }
    }

    public void SetTrigger(string trigger)
    {
        Debug.Log(gameObject.name + ":set trigger for " + trigger);
        animator.SetTrigger(trigger);
        isFinished = false;
    }

    public void SetBool(string property, bool value)
    {
        animator.SetBool(property, value);
        //isFinished = false;
    }
}
