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
    private string type;
    private string token;
    private string fbid;
    private LoadingScript loadingPanel;
    // Use this for initialization
    void Start () {
        //先清空userData
        PlayerPrefs.DeleteKey("userData");
        PlayerPrefs.DeleteKey("Cookie");
        loadingPanel = LoadingPanel.GetComponent<LoadingScript>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LocalLoginClicked()
    {
        loadingPanel.StartLoading();
        //本地登入 使用DeviceID
        type = "local";
        token = SystemInfo.deviceUniqueIdentifier;
        StartCoroutine(WaitForLogin());
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
            //登入成功
            SetUserData(w);
            //進入狀態畫面
            SceneManager.LoadScene("Status");
        }
        else
        {
            //要是出現其他錯誤可能會出錯

            //登入失敗 準備註冊
            //移入輸入暱稱面板
            SignUpPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            Debug.Log(w.error);
        }
        loadingPanel.EndLoading();
    }

    private IEnumerator WaitForSignUp()
    {
        WWWForm form2 = new WWWForm();
        form2.AddField("token", token); //用token登入，deviceID or FB
        form2.AddField("name", NameInput.transform.FindChild("Text").gameObject.GetComponent<Text>().text);
        string requestUrl = Constant.SERVER_URL;
        if (type == "local")
        {
            requestUrl += "/signup";
        }
        else if (type == "facebook")
        {
            form2.AddField("fbid", fbid); //多需要FBID
            requestUrl += "/signupFacebook";
        }
        WWW w2 = new WWW(requestUrl, form2);
        yield return w2;
        if (string.IsNullOrEmpty(w2.error))
        {
            //註冊成功
            SetUserData(w2);
            SceneManager.LoadScene("Status");
        }
        else
        {
            //註冊失敗
            Debug.Log("註冊失敗喔!" + w2.error);
        }
        loadingPanel.EndLoading();
    }

    private void SetUserData(WWW w)
        //將server傳來的使用者資料儲存在本機
    {
        JSONObject response = new JSONObject(w.text);
        string[] data = w.responseHeaders["SET-COOKIE"].Split(";"[0]); //取出登入後的cookie存起來
        if (data.Length > 0)
        {
            //response.AddField("cookie", data[0]);
            PlayerPrefs.SetString("Cookie", data[0]);
            Debug.Log(data[0]);
        }
        Debug.Log("登入成功");
        PlayerPrefs.SetString("userData", response.ToString()); //將使用者資料與cookie存入playerPrefs
    }
}
