using UnityEngine;
using System.Collections;

public class PartnerEntry : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (transform.position.x <= 25.53)
            transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y);
	}
}
