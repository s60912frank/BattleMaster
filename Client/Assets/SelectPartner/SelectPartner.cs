using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SelectPartner : MonoBehaviour {
    private float rotateDelta = 0;
    private bool isRotating = false;
    private Vector3 CylinderAxis;
    private GameObject UniformObj;
    public Text InfoText;
    public GameObject ConfirmExitPanel;
    private Panel confirmPanelScript;
    private string[] partner;
    private int selectedIndex = 0;
    // Use this for initialization
    void Start () {
        partner = new string[] { "Augu", "Charmander", "V" };
        confirmPanelScript = ConfirmExitPanel.GetComponent<Panel>();
        confirmPanelScript.SetText("確定是這個夥伴嗎?選擇之後就不能再更改了");
        CylinderAxis = new Quaternion(0, 1, 0, 15).eulerAngles;
        UniformObj = GameObject.Find("UniformObj");
        Debug.Log(CylinderAxis);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(CylinderAxis, rotateDelta);
        UniformObj.transform.Rotate(CylinderAxis, rotateDelta);
    }

    public void TurnLeft()
    {
        if (!isRotating)
        {
            StartCoroutine(SmoothRotate(-120));
        }
    }

    public void TurnRight()
    {
        if (!isRotating)
        {
            StartCoroutine(SmoothRotate(120));
        }
    }

    public void SelectButtonClicked()
    {
        confirmPanelScript.Show();
        confirmPanelScript.SetConfirmListener(() =>
        {
            //這裡要加loading畫面
            StartCoroutine(WaitForSetPartner());            
        });
    }

    private IEnumerator SmoothRotate(int angle)
    {
        isRotating = true;
        int counter = Mathf.Abs(angle);
        while (counter > 0)
        {
            rotateDelta = angle > 0 ? 4 : -4;
            counter -= 4;
            yield return new WaitForEndOfFrame();
        }
        rotateDelta = 0;
        isRotating = false;
        selectedIndex = (selectedIndex + partner.Length + (angle > 0 ? 1 : -1)) % partner.Length;
        Debug.Log(partner[selectedIndex]);
        SetInfoText();
    }

    private void SetInfoText()
    {
        switch (selectedIndex)
        {
            case 0:
                InfoText.text = "Augu-擁有較高的攻擊數值";
                break;
            case 1:
                InfoText.text = "狗屎爛蛋小火龍-擁有較高的血量及防禦";
                break;
            case 2:
                InfoText.text = "V-擁有較高的迴避數值";
                break;
        }
    }

    private IEnumerator WaitForSetPartner()
    {
        string cookie = PlayerPrefs.GetString("Cookie");
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", cookie); //加入cookie
        WWWForm form = new WWWForm();
        form.AddField("type", partner[selectedIndex]); //用token登入，deviceID or FB
        WWW w = new WWW(Constant.SERVER_URL + "/setPartner", form.data, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            PlayerPrefs.SetString("userData", w.text);
            SceneManager.LoadScene("Status");
        }
        else
        {
            //之後看怎處理
            Debug.LogError(w.error);
        }
    }
}
