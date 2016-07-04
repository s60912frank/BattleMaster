using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class entry : MonoBehaviour {
    private string isLoaded = "";
    public GameObject entryText;
    private int retryCount = 0;
    private JSONObject userData;
	// Use this for initialization
	void Start () {
        StartCoroutine(CheckServerAlive());
	}

    private IEnumerator CheckServerAlive()
    {
        WWW w = new WWW(Constant.SERVER_URL + "/isAlive");
        yield return w;
        if (!string.IsNullOrEmpty(w.error) && retryCount <= 5)
        {
            //retry
            retryCount++;
            Debug.Log(retryCount);
            Start();
        }
        else if(!string.IsNullOrEmpty(w.error) && retryCount > 5)
        {
            retryCount = 0;
            isLoaded = "connectionRefused";
        }
        else
        {
            CheckUserDataExist();
        }
    }

    private void CheckUserDataExist()
    {
        userData = new JSONObject(PlayerPrefs.GetString("userData"));
        //先找有沒有登入過的資訊
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("userData")))
        {
            //轉向註冊or登入畫面
            //轉向選擇FB OR本機登入
            //StartCoroutine(WaitForLogin());
            //SceneManager.LoadScene("login");
            isLoaded = "login";
        }
        else
        {
            //檢查此cookie O不OK
            StartCoroutine(CheckIfLoggedIn());
        }
    }

    private IEnumerator CheckIfLoggedIn()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", userData["cookie"].ToString().Replace("\"", "")); //加入認證過的cookie就不用重新登入了
        WWW w = new WWW(Constant.SERVER_URL + "/isLoggedIn", null, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            //直接登入了
            //到status畫面
            isLoaded = "status";
        }
        else
        {
            //註冊去
            isLoaded = "login";
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
                    SceneManager.LoadScene("login");
                entryText.GetComponentInChildren<Text>().text = "點擊螢幕進入遊戲";
                break;
            case "status":
                if (Input.GetMouseButtonDown(0))
                    SceneManager.LoadScene("Status");
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
