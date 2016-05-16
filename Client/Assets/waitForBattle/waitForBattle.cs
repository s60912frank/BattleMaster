using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class waitForBattle : MonoBehaviour {
    public GameObject statusText;
    public GameObject socketIOObject;
    private SocketIOComponent socket;
    //private const string SERVER_URL = "http://127.0.0.1:8080";
    private const string SERVER_URL = "http://server-gmin.rhcloud.com";
    private JSONObject userData;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(socketIOObject); //他會一直活著!因為下一個scene還要用
        socket = socketIOObject.GetComponent<SocketIOComponent>(); //把socket存起來
        userData = new JSONObject(PlayerPrefs.GetString("userData")); //讀取userData
        statusText.GetComponentInChildren<Text>().text = "歡迎," + userData["name"].ToString() + "!";
        socket.On("waiting", OnWaiting); //等待中觸發
        socket.On("battleStart", OnBattleStart); //進入戰鬥觸發
	}

    private void OnWaiting(SocketIOEvent e)
    {
        Debug.Log(e.name);
        statusText.GetComponentInChildren<Text>().text = "搜尋對手中";
    }

    private void OnBattleStart(SocketIOEvent e)
    {
        SceneManager.LoadScene("BattlePVP");//移到戰鬥畫面
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayWithAIClicked()
    {
        SceneManager.LoadScene("Battle2"); //自己玩
    }

    public void SearchEnemy()
    {
        StartCoroutine(WaitForBattle());
    }

    private IEnumerator WaitForBattle()
    {
        socket.Connect(); //加入等待前先連socket
        while (socket.sid == null)
        {
            yield return null; // wait until sid != null
        }
        Debug.Log(socket.sid);
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", userData["cookie"].ToString().Replace("\"", "")); //加入認證過的cookie就不用重新登入了
        form.AddField("sid", "/#" + socket.sid); //加入連上的sid
        WWW w = new WWW(SERVER_URL + "/waitforbattle", form.data, headers);
        yield return w;
        //就只是看有沒有錯誤而已
        if (!string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log(w.text);
        }
    }
}
