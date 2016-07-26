using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Panel : MonoBehaviour {
    private RectTransform rect;
    private Text ContentText;
    private Button NoButton;
    private Button ConfirmButton;
	// Use this for initialization

	void Start () {
        rect = gameObject.GetComponent<RectTransform>();
        Transform innerPanel = transform.Find("ConfirmExit");
        Debug.Log(innerPanel.parent.name + "66666666666");
        ContentText = innerPanel.Find("ContentText").GetComponent<Text>();
        NoButton = innerPanel.Find("NoButton").GetComponent<Button>();
        NoButton.onClick.AddListener(delegate { Hide(); });
        ConfirmButton = innerPanel.Find("ConfirmButton").GetComponent<Button>();
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

    public void SetText(string text)
    {
        ContentText.text = text;
    }

    public void SetConfirmListener(System.Action listener)
    {
        ConfirmButton.onClick.AddListener(delegate { listener(); });
    }
}
