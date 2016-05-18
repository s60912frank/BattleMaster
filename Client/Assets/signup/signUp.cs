using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class signUp : MonoBehaviour {
    public GameObject NameInput;
    //private const string SERVER_URL = "http://127.0.0.1:8080";
    private const string SERVER_URL = "http://server-gmin.rhcloud.com";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SignUpCLicked()
    {
        string name = NameInput.GetComponentInChildren<Text>().text;
        if (name != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("token", SystemInfo.deviceUniqueIdentifier);//用deviceID註冊
			//form.AddField("token", "dljfndjgnjdnklndkjndkjndkjnfdjknd");
            form.AddField("name", name);
            WWW w = new WWW(SERVER_URL + "/signup", form);
            StartCoroutine(WaitForSubmit(w));
        }
        else
        {
            Debug.Log("名子不能是空的~");
        }
    }

    private IEnumerator WaitForSubmit(WWW w)
    {
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log(w.text);
            JSONObject response = new JSONObject(w.text);
            string[] data = w.responseHeaders["SET-COOKIE"].Split(";"[0]);
            if (data.Length > 0)
            {
                response.AddField("cookie", data[0]); //取出cookie
            }
            //進入可戰鬥畫面
            Debug.Log("註冊成功");
            PlayerPrefs.SetString("userData", response.ToString()); //存user資料
            SceneManager.LoadScene("WaitForBattle");
        }
    }
}
