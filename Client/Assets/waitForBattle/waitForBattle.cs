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
        DontDestroyOnLoad(socketIOObject); //他會一直活著!
        socket = socketIOObject.GetComponent<SocketIOComponent>();
        userData = new JSONObject(PlayerPrefs.GetString("userData"));
        statusText.GetComponentInChildren<Text>().text = "歡迎," + userData["name"].ToString() + "!";
        socket.On("waiting", OnWaiting);
        socket.On("battleStart", OnBattleStart);
	}

    private void OnWaiting(SocketIOEvent e)
    {
        Debug.Log(e.name);
        statusText.GetComponentInChildren<Text>().text = "搜尋對手中";
    }

    private void OnBattleStart(SocketIOEvent e)
    {
        SceneManager.LoadScene("BattlePVP");//???
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayWithAIClicked()
    {
        SceneManager.LoadScene("Battle");
    }

    public void SearchEnemy()
    {
        StartCoroutine(WaitForBattle());
    }

    private IEnumerator WaitForBattle()
    {
        socket.Connect();
        while (socket.sid == null)
        {
            yield return null; // wait until next frame
        }
        Debug.Log(socket.sid);
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        Debug.Log(userData["cookie"].ToString());
        headers.Add("Cookie", userData["cookie"].ToString().Replace("\"", ""));
        form.AddField("sid", "/#" + socket.sid);
        WWW w = new WWW(SERVER_URL + "/waitforbattle", form.data, headers);
        yield return w;
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
