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

    public void SearchEnemy()
    {
        SceneManager.LoadScene("PVPRooms");
    }

    public void TrainingClicked()
    {
        //有點臭
        LoadingPanel.GetComponent<LoadingScript>().StartLoading();
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        if(data["mileage"].f > 100)
        {
            StartCoroutine(EnterTraningGame());
        }
        else
        {
            //出去走走好嗎
        }
    }

    private IEnumerator EnterTraningGame()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入cookie
        WWW w = new WWW(Constant.SERVER_URL + "/enterTraning", null, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            PlayerPrefs.SetString("userData", w.text);
        }
        else
        {
            //你4不4偷改數據
            Debug.Log(w.error);
        }
        LoadingPanel.GetComponent<LoadingScript>().EndLoading();
        SceneManager.LoadScene("StatsTraining");
    }
}
