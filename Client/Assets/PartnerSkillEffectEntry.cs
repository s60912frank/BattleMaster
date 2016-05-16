using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PartnerSkillEffectEntry : MonoBehaviour {

    public bool activated;
    private Vector2 startPos;
    private Vector3 startScale;//save initial states
	private RectTransform rt;
	private float leftBorder;
	private float rightBorder;
	private float speed;
	// Use this for initialization
	void Start () {
		rt = gameObject.GetComponent<RectTransform> ();
		leftBorder = -Screen.width - 350;
		rightBorder = Screen.width + 350;
		speed = Screen.width / 25;
		Debug.Log (leftBorder);
		rt.anchoredPosition = new Vector2 (rightBorder - 1, rt.anchoredPosition.y);
        activated = false;
		startPos = rt.anchoredPosition;
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (rt.anchoredPosition.x >= rightBorder || rt.anchoredPosition.x <= leftBorder)//set activate to false if out of screen range(roughly) and enable the else loop below to run
            activated = false;
        if (activated)//run animate
        {
			if (rt.anchoredPosition.x >= 30){
				rt.Translate(-speed, 0.08f, 0);
				rt.localScale += new Vector3(0.1f, 0.1f, 0);
            }
			else if (rt.anchoredPosition.x <= -30){
				rt.Translate(-speed, -0.08f, 0);
				rt.localScale += new Vector3(-0.1f, -0.1f, 0);
            }
            else//middle  display
				rt.Translate(-speed / 40, 0, 0);
        }
        else//set position & scale to initial state
        {
			rt.anchoredPosition = startPos;
			rt.localScale = startScale;
        }
	}
}
