using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Panel : MonoBehaviour {
    private RectTransform rect;
    private Animator animator;
    private Text ContentText;
    private Button NoButton;
    private Button ConfirmButton;
    private System.Action callback;
	// Use this for initialization

	void Start () {
        rect = gameObject.GetComponent<RectTransform>();
        animator = gameObject.GetComponent<Animator>();
        Transform innerPanel = transform.Find("ConfirmExit");
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
        animator.SetTrigger("StartShow");
        Debug.Log("SHOWWWWWWWWW");
    }

    public void Hide()
    {
        animator.SetTrigger("StartHide");
        Debug.Log("STARTHIDE");
    }

    public void HidePanel()
    {
        //動畫播完後觸發
        rect.anchoredPosition = new Vector2(1000, 0);
        //這樣寫可能會有點怪w
        if (callback != null) callback();
        Debug.Log("HIDEPANEL");
    }

    public void SetText(string text)
    {
        ContentText.text = text;
    }

    public void SetConfirmListener(System.Action listener)
    {
        ConfirmButton.onClick.AddListener(delegate { listener(); });
    }

    public void SetNoListener(System.Action listener)
    {
        NoButton.onClick.AddListener(delegate { listener(); });
        //this.callback = listener;
    }

    public void OnHideCallback(System.Action callback)
    {
        this.callback = callback;
    }
}
