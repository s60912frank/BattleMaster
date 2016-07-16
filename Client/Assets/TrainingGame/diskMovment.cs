using UnityEngine;
using System.Collections;

public class diskMovment : MonoBehaviour {

    public bool inPanel = false;
    public bool swipedInPanel = false;
    public bool forceGet = false;
    private Vector2 mouseEnterPos;
    private Vector2 swipeForce;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(!forceGet)
            gameObject.transform.Translate(0, -0.1f, 0);//to make movement more smooth
        else if(swipedInPanel && forceGet)
        {
            if (swipeForce.y < 2f)
                swipeForce.y = 2f;
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(swipeForce);
            swipedInPanel = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Panel")
            inPanel = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Panel")
            inPanel = false;
    }

    void OnMouseEnter()
    {
        if (inPanel)
        {
            swipedInPanel = true;
            mouseEnterPos = Input.mousePosition;
        }
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.MouseUp && swipedInPanel)
        {
            Vector2 mouseEndPosition = Input.mousePosition;
            swipeForce = 3 * (mouseEndPosition - mouseEnterPos);
            forceGet = true;
            Debug.Log(transform.name + "get" + swipeForce);
        }
    }
}
