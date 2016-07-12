using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BtnFunctions : MonoBehaviour {
    public GameObject LoadingPanel;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GoToMap()
    {
        SceneManager.LoadScene("map");//移到地圖
    }

    public void PlayWithAIClicked()
    {
        //SceneManager.LoadScene("Battle2"); //自己玩
        //statusText.text = "讀取敵人資料中..";
        LoadingPanel.GetComponent<LoadingScript>().StartLoading();
        StartCoroutine(GetEnemyData());
    }

    private IEnumerator GetEnemyData()
    {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        headers.Add("Cookie", data["cookie"].str); //加入認證過的cookie就不用重新登入了
        //將來這項資料從地圖取得
        form.AddField("enemyName", "Hammer");
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
            LoadingPanel.GetComponent<LoadingScript>().EndLoading();
            SceneManager.LoadScene("Battle2");
        }
    }

    public void SearchEnemy()
    {
        SceneManager.LoadScene("PVPRooms");
    }
}
