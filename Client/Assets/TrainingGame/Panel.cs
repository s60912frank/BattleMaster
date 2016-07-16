using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Panel : MonoBehaviour {
    private RectTransform rect;
	// Use this for initialization
	void Start () {
        rect = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void Show()
    {
        rect.anchoredPosition = Vector2.zero;
    }

    public void Hide()
    {
        rect.anchoredPosition = new Vector2(1000, 0);
    }
}
