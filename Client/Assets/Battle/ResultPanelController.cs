using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultPanelController : MonoBehaviour {
    private RectTransform rt;
    private Button confirmButton;
	// Use this for initialization
	void Start () {
        rt = gameObject.GetComponent<RectTransform>();
        confirmButton = transform.Find("Panel").Find("ConfirmButton").GetComponent<Button>();
        //可能有些臭
        confirmButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("BattleResult");
        });
        confirmButton.interactable = false;
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
        rt.anchoredPosition = new Vector2(-1000, 0);
    }

    public void SetButtonInteractable(bool state)
    {
        confirmButton.interactable = state;
    }
}
