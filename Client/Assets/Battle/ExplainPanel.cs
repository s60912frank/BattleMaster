using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExplainPanel : MonoBehaviour {
	private RectTransform rt;
	public Toggle toggle;
	void Awake(){
		rt = gameObject.GetComponent<RectTransform>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show(){
		rt.anchoredPosition = Vector2.zero;
	}

	public void Hide(){
		rt.anchoredPosition = new Vector2(1000, 0);
		PlayerPrefs.SetInt("showTutorial", toggle.isOn ? 0:1);
		print(toggle.isOn);
	}
}
