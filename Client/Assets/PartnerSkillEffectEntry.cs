using UnityEngine;
using System.Collections;

public class PartnerSkillEffectEntry : MonoBehaviour {

    public bool activated;
    private Vector3 startPos;
    private Vector3 startScale;//save initial states
	// Use this for initialization
	void Start () {
        activated = false;
        startPos = transform.position;
        startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x >= 20 || transform.position.x <= -20)//set activate to false if out of screen range(roughly) and enable the else loop below to run
            activated = false;
        if (activated)//run animate
        {
            if (transform.position.x >= 1){
                transform.Translate(-1, 0.08f, 0);
                transform.localScale += new Vector3(0.1f, 0.1f, 0);
            }
            else if (transform.position.x <= -1){
                transform.Translate(-1, -0.08f, 0);
                transform.localScale += new Vector3(-0.1f, -0.1f, 0);
            }
            else//middle  display
                transform.Translate(-0.01f, 0, 0);
        }
        else//set position & scale to initial state
        {
            transform.position = startPos;
            transform.localScale = startScale;
        }
	}
}
