using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotifyPanel : MonoBehaviour {
    private RectTransform rt;
    private Animator animator;
    private Text ContentText;
    private Button ConfirmButton;

    public void Awake()
    {
        Debug.Log("AWAKE!");
        animator = gameObject.GetComponent<Animator>();
        rt = gameObject.GetComponent<RectTransform>();
        ContentText = transform.FindChild("Panel").FindChild("ContentText").GetComponent<Text>();
        ConfirmButton = transform.FindChild("Panel").FindChild("ConfirmButton").GetComponent<Button>();
        
    }

    // Use this for initialization
    void Start () {
        Debug.Log("START!");
        //rt = gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(2000, 0);
        ConfirmButton.onClick.AddListener(delegate { Hide(); });
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetText(string text)
    {
        Debug.Log("SET:" + text);
        ContentText.text = text;
    }

    public void Show()
    {
        Debug.Log("SHOW!");
        rt.anchoredPosition = new Vector2(0, 0);
        animator.SetTrigger("StartShow");
    }

    public void Hide()
    {
        animator.SetTrigger("StartHide");
    }

    public void HidePanel()
    {
        rt.anchoredPosition = new Vector2(2000, 0);
    }
}
