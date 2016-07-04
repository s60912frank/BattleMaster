using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class waitForBattle : MonoBehaviour {
    public GameObject statusObject;
    private Text statusText;
    private JSONObject userData;
	// Use this for initialization
	void Start () {
        userData = new JSONObject(PlayerPrefs.GetString("userData")); //讀取userData
        statusText = statusObject.GetComponent<Text>();
		statusText.text = "歡迎," + userData["name"].ToString().Replace("\"", "") + "!";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayWithAIClicked()
    {
        //SceneManager.LoadScene("Battle2"); //自己玩
        statusText.text = "讀取敵人資料中..";
        StartCoroutine(GetEnemyData());
    }

    public void SearchEnemy()
    {
		SceneManager.LoadScene("PVPRooms");
    }

    private IEnumerator GetEnemyData()
    {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", userData["cookie"].ToString().Replace("\"", "")); //加入認證過的cookie就不用重新登入了
        form.AddField("whe", "wheee");
        WWW w = new WWW(Constant.SERVER_URL + "/battle", form.data, headers);
        yield return w;
        //就只是看有沒有錯誤而已
        if (!string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log(w.text);
            PlayerPrefs.SetString("enemyAI", w.text);
            SceneManager.LoadScene("Battle2");
        }
    }

    public void GoToMap()
    {
        SceneManager.LoadScene("map");//移到地圖
    }
}
