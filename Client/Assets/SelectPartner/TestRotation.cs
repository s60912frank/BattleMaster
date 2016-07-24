using UnityEngine;
using System.Collections;

public class TestRotation : MonoBehaviour {
    private float rotateDelta = 0;
    private bool isRotating = false;
    private Vector3 CylinderAxis;
	// Use this for initialization
	void Start () {
        CylinderAxis = Quaternion.AngleAxis(-15, Vector3.up).eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(CylinderAxis, rotateDelta);
    }

    public void TurnLeft()
    {
        if (!isRotating)
        {
            StartCoroutine(SmoothRotate(-120));
        }
    }

    public void TurnRight()
    {
        if (!isRotating)
        {
            StartCoroutine(SmoothRotate(120));
        }
    }

    private IEnumerator SmoothRotate(int angle)
    {
        isRotating = true;
        int counter = Mathf.Abs(angle);
        while(counter > 0)
        {
            rotateDelta = angle > 0 ? 4 : -4;
            counter -= 4;
            yield return new WaitForEndOfFrame();
        }
        rotateDelta = 0;
        isRotating = false;
        Debug.Log("WHEE");
    }
}
