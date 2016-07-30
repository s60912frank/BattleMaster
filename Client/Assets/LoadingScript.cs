using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScript : MonoBehaviour {
    private RectTransform rt;
    private Animator animator;
    private System.Action callback;
    private Text content;
	// Use this for initialization
	public void Start () {
        content = transform.Find("InnerPanel").Find("Text").GetComponent<Text>();
        rt = gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(2000, 0);
        animator = gameObject.GetComponent<Animator>();
	}
	
    public void OnHidedCallback(System.Action callback)
    {
        this.callback = callback;
    }

    public void SetText(string text)
    {
        content.text = text;
    }

	public void StartLoading()
    {
        rt.anchoredPosition = new Vector2(0, 0);
        animator.SetTrigger("StartShow");
        Debug.Log("LOADING!");
    }

    public void EndLoading()
    {
        animator.SetTrigger("StartHide");
    }

    public void HidePanel()
    {
        rt.anchoredPosition = new Vector2(2000, 0);
        Debug.Log("FINISH LOADING!");
        if (callback != null) callback();
    }
}
