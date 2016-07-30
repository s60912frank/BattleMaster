using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScript : MonoBehaviour {
    private RectTransform rt;
    private Animator animator;
	// Use this for initialization
	public void Start () {
        rt = gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(2000, 0);
        animator = gameObject.GetComponent<Animator>();
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
    }
}
