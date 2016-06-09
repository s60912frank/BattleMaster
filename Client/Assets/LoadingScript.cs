using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScript : MonoBehaviour {
    private RectTransform rt;
	// Use this for initialization
	public void Start () {
        rt = gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(2000, 0);
	}
	

	public void StartLoading()
    {
        rt.anchoredPosition = new Vector2(0, 0);
        Debug.Log("LOADING!");
    }

    public void EndLoading()
    {
        rt.anchoredPosition = new Vector2(2000, 0);
        Debug.Log("FINISH LOADING!");
    }
}
