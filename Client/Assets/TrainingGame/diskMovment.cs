using UnityEngine;
using System.Collections;

public class diskMovment : Pauseable {

    public bool inPanel = false;
    public bool swipedInPanel = false;
    public bool forceGet = false;
    private Vector2 mouseEnterPos;
    private Vector2 swipeForce;
    private statsMinigame reference;
    private bool run = true;
    // Use this for initialization
    void Start () {
        reference = GameObject.Find("GameScript").GetComponent<statsMinigame>();
        //reference.AddPauseableObject(this);
        StartCoroutine(DirtyWay());
	}

    private IEnumerator DirtyWay()
    {
        yield return new WaitForSeconds(0.1f);
        reference.AddPauseableObject(this);
    }

    // Update is called once per frame
    void Update () {
        if (run)
        {
            if (!forceGet)
                gameObject.transform.Translate(0, -0.1f, 0);//to make movement more smooth
            else if (swipedInPanel && forceGet)
            {
                if (swipeForce.y < 2f)
                    swipeForce.y = 2f;
                GetComponent<Rigidbody2D>().AddForce(swipeForce);
                swipedInPanel = false;
            }
        }
    }

    public override void Pause()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        run = false;
    }

    public override void Resume()
    {
        GetComponent<Rigidbody2D>().AddForce(swipeForce);
        run = true;
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
        if (inPanel && run)
        {
            swipedInPanel = true;
            mouseEnterPos = Input.mousePosition;
        }
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.MouseUp && swipedInPanel && run)
        {
            Vector2 mouseEndPosition = Input.mousePosition;
            swipeForce = 3 * (mouseEndPosition - mouseEnterPos);
            forceGet = true;
            Debug.Log(transform.name + "get" + swipeForce);
        }
    }

    void OnDestroy()
    {
        reference.RemovePauseableObject(this);
    }
}
