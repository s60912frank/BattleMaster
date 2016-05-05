using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class entry : MonoBehaviour {
    //private const string SERVER_URL = "http://127.0.0.1:8080";
    private const string SERVER_URL = "http://server-gmin.rhcloud.com";
    private string isLoaded = "";
    public GameObject entryText;
    private int retryCount = 0;
	// Use this for initialization
	void Start () {
        WWWForm form = new WWWForm();
        form.AddField("token", SystemInfo.deviceUniqueIdentifier);
        form.AddField("name", "WHEEEEEEE");
        WWW w = new WWW(SERVER_URL + "/login", form);
        StartCoroutine(WaitForLogin(w));
	}

    private IEnumerator WaitForLogin(WWW w)
    {
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            if (w.error.Contains("refused"))
            {
                if (retryCount > 4)
                {
                    isLoaded = "connectionRefused";
                    retryCount = 0;
                }
                else
                {
                    Debug.Log("無法連接");
                    Start();
                    retryCount++;
                }
            }
            else
            {
                //進入註冊畫面
                Debug.Log("導向註冊畫面");
                isLoaded = "signup";
            }
        }
        else
        {
            Debug.Log(w.text);
            JSONObject response = new JSONObject(w.text);
            string[] data = w.responseHeaders["SET-COOKIE"].Split(";"[0]);
            if (data.Length > 0)
            {
                response.AddField("cookie", data[0]);
            }
            //進入可戰鬥畫面
            Debug.Log("登入成功");
            isLoaded = "login";
            PlayerPrefs.SetString("userData", response.ToString());
        }
    }

	// Update is called once per frame
	void Update () {
        switch (isLoaded)
        {
            case "":
                break;
            case "login":
                if (Input.GetMouseButtonDown(0))
                    SceneManager.LoadScene("WaitForBattle");
                entryText.GetComponentInChildren<Text>().text = "點擊螢幕進入遊戲";
                break;
            case "signup":
                if (Input.GetMouseButtonDown(0))
                    SceneManager.LoadScene("SignUp");
                entryText.GetComponentInChildren<Text>().text = "點擊螢幕進入遊戲";
                break;
            case "connectionRefused":
                if (Input.GetMouseButtonDown(0))
                {
                    entryText.GetComponentInChildren<Text>().text = "載入中...";
                    Start();
                    isLoaded = "";
                }
                else
                {
                    entryText.GetComponentInChildren<Text>().text = "無法連接伺服器,點擊重試";
                }
                break;
            default:
                throw null;
        }
	}
}
