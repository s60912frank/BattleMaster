using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class login : MonoBehaviour {
    public GameObject SignUpPanel;
    public GameObject NameInput;
    private string type;
    private string token;
    private string fbid;
    // Use this for initialization
    void Start () {
        PlayerPrefs.SetString("userData", "");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LocalLoginClicked()
    {
        type = "local";
        token = SystemInfo.deviceUniqueIdentifier;
        StartCoroutine(WaitForLogin());
    }

    public void FBLoginCLicked()
    {
        FB.Init(OnInitComplete, OnHideUnity);
    }

    public void SignUpClicked()
    {
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
                Debug.Log("WHEEEE!!!!!" + result.AccessToken.TokenString);
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
            form.AddField("fbid", fbid);
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
            form2.AddField("fbid", fbid);
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
    }

    private void SetUserData(WWW w)
    {
        JSONObject response = new JSONObject(w.text);
        string[] data = w.responseHeaders["SET-COOKIE"].Split(";"[0]); //取出登入後的cookie就不用一直登入了
        if (data.Length > 0)
        {
            response.AddField("cookie", data[0]);
            Debug.Log(data[0]);
        }
        Debug.Log("登入成功");
        PlayerPrefs.SetString("userData", response.ToString()); //將使用者資料與cookie存入playerPrefs
    }
}
