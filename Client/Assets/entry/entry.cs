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
        //檢查server是否還活著 嘗試超過五次失敗就顯示無法連線
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
        //檢查有沒有存過使用者資料
    {
        //先找有沒有登入過的資訊
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("userData")))
        {
            //沒有的話轉向註冊&登入畫面
            isLoaded = "login";
        }
        else
        {
            userData = new JSONObject(PlayerPrefs.GetString("userData"));
            StartCoroutine(CheckIfLoggedIn());
        }
    }

    private IEnumerator CheckIfLoggedIn()
        //檢查此cookie O不OK 有沒有過期 存不存在
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", userData["cookie"].str); //加入cookie
        WWW w = new WWW(Constant.SERVER_URL + "/isLoggedIn", null, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            //沒有錯誤就直接登入了
            //到status畫面
            isLoaded = "status";
            JSONObject newUserData = new JSONObject(w.text);
            newUserData.AddField("cookie", userData["cookie"]);
            PlayerPrefs.SetString("userData", newUserData.ToString());
        }
        else
        {
            //收到錯誤就註冊去
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
                    entryText.GetComponentInChildren<Text>().text = "無法連接伺服器,點擊重試";
                break;
            default:
                throw null;
        }
	}
}
