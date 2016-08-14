using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignUpPanelController : MonoBehaviour {
    private RectTransform rt;
	// Use this for initialization
	void Start () {
        rt = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show()
    {
        rt.anchoredPosition = Vector2.zero;
    }

    public void Hide()
    {
        rt.anchoredPosition = new Vector2(1000, 0);
    }
}
