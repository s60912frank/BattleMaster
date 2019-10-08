using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class login : MonoBehaviour {
    public GameObject SignUpPanel;
    public GameObject NameInput;
    public GameObject LoadingPanel;
    public GameObject NotifyPanel;

    private string type;
    private string token;
    private string fbid;
    private LoadingScript loadingPanel;
    private NotifyPanel notifyScript;
    private SignUpPanelController signUpPanel;

    // Use this for initialization
    void Start () {
        //先清空userData
        PlayerPrefs.DeleteKey("userData");
        PlayerPrefs.DeleteKey("Cookie");
        loadingPanel = LoadingPanel.GetComponent<LoadingScript>();
        notifyScript = NotifyPanel.GetComponent<NotifyPanel>();
        signUpPanel = SignUpPanel.GetComponent<SignUpPanelController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LocalLoginClicked()
    {
        //本地登入 使用DeviceID
        type = "local";
        var txt = Resources.Load<TextAsset>("demo_id");
        Debug.Log(txt.text);
        token = txt.text;
        StartCoroutine(WaitForLogin());
    }

    public void CancelButtonClicked()
    {
        signUpPanel.Hide();
    }

    public void FBLoginCLicked()
    {
        loadingPanel.StartLoading();
        //使用FB API登入
        FB.Init(OnInitComplete, OnHideUnity);
    }

    public void SignUpClicked()
    {
        //註冊按鈕被按時觸發
        loadingPanel.StartLoading();
        StartCoroutine(WaitForSignUp());
    }

    private void OnInitComplete()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            CallFBLogin();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        FB.ActivateApp();
    }

    private void CallFBLogin()
    {
        //We first tell Facebook what permissions we want and then tell it todo GameSparksLogin when done
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, AuthCallBack);
    }

    private void AuthCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            if (string.IsNullOrEmpty(result.Error))
            {
                //使用FB登入
                type = "facebook";
                fbid = result.AccessToken.UserId;
                token = result.AccessToken.TokenString;
                StartCoroutine(WaitForLogin());
            }
            else
            {
                Debug.Log("ERROR!" + result.Error);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private IEnumerator WaitForLogin()
    {
        loadingPanel.StartLoading();
        WWWForm form = new WWWForm();
        form.AddField("token", token); //用token登入，deviceID or FB
        string requestUrl = Constant.SERVER_URL;
        if (type == "local")
        {
            requestUrl += "/login";
        }
        else if (type == "facebook")
        {
            requestUrl += "/loginFacebook";
            form.AddField("fbid", fbid); //fb登入還要多付加一項fbid
        }
        WWW w = new WWW(requestUrl, form);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            JSONObject response = new JSONObject(w.text);
            if (response["ok"].b)
            {
                //登入成功
                SetUserData(w);
                //進入選status畫面
                SceneManager.LoadScene("Status");
            }
            else
            {
                string message = response["message"].str;
                if(message == "找不到此帳號，請先註冊")
                {
                    //登入失敗 準備註冊
                    //移入輸入暱稱面板
                    signUpPanel.Show();
                }
                else
                {
                    //show特殊錯誤訊息
                    Debug.LogError(message);
                    notifyScript.SetText(message);
                    notifyScript.Show();
                }
            }
        }
        else
        {
            //server 錯誤
            //要顯示讓使用者知道
            Debug.Log(w.error);
            notifyScript.SetText("伺服器出現錯誤，請稍後再試");
            notifyScript.Show();
        }
        loadingPanel.EndLoading();
    }

    private IEnumerator WaitForSignUp()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token); //用token登入，deviceID or FB
        form.AddField("name", NameInput.transform.Find("Text").gameObject.GetComponent<Text>().text);
        string requestUrl = Constant.SERVER_URL;
        if (type == "local")
        {
            requestUrl += "/signup";
        }
        else if (type == "facebook")
        {
            form.AddField("fbid", fbid); //多需要FBID
            requestUrl += "/signupFacebook";
        }
        WWW w = new WWW(requestUrl, form);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            JSONObject response = new JSONObject(w.text);
            if (response["ok"].b)
            {
                //註冊成功
                SetUserData(w);
                //進入選partner畫面
                SceneManager.LoadScene("SelectScene");
            }
            else
            {
                signUpPanel.Hide();
                string message = response["message"].str;
                //show特殊錯誤訊息
                Debug.LogError(message);
                notifyScript.SetText(message);
                notifyScript.Show();
            }
        }
        else
        {
            //註冊失敗
            //server錯誤
            Debug.LogError(w.error);
        }
        loadingPanel.EndLoading();
    }

    private void SetUserData(WWW w)
        //將server傳來的使用者資料儲存在本機
    {
        JSONObject response = new JSONObject(w.text);
        foreach (string key in w.responseHeaders.Keys)
        {
            Debug.Log(key);
        }
        if (w.responseHeaders.ContainsKey("SET-COOKIE")) 
        {
            string[] data = w.responseHeaders["SET-COOKIE"].Split(";"[0]); //取出登入後的cookie存起來
            if (data.Length > 0)
            {
                //response.AddField("cookie", data[0]);
                PlayerPrefs.SetString("Cookie", data[0]);
                Debug.Log(data[0]);
            }
        }
        
        Debug.Log("登入成功");
        PlayerPrefs.SetString("userData", response["data"].ToString()); //將使用者資料與cookie存入playerPrefs
    }
}
