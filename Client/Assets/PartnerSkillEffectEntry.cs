using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PartnerSkillEffectEntry : MonoBehaviour {

    public bool activated;
    private RectTransform rt;
    private Vector3 startPos;
    private Vector3 startScale;//save initial states
	// Use this for initialization
	void Start () {
        rt  = gameObject.GetComponent<RectTransform>();
        activated = false;
        startPos = rt.anchoredPosition;
        startScale = rt.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (rt.anchoredPosition.x >= 800 || rt.anchoredPosition.x <= -800)//set activate to false if out of screen range(roughly) and enable the else loop below to run
            activated = false;
        if (activated)//run animate
        {
            if (rt.position.x >= 30){
                rt.Translate(-10, 0.08f, 0);
                rt.localScale += new Vector3(0.1f, 0.1f, 0);
            }
            else if (rt.position.x <= -30){
                rt.Translate(-10, -0.08f, 0);
                rt.localScale += new Vector3(-0.1f, -0.1f, 0);
            }
            else//middle  display
                transform.Translate(-1, 0, 0);
        }
        else//set position & scale to initial state
        {
            rt.anchoredPosition = startPos;
            rt.localScale = startScale;
        }
	}
}
