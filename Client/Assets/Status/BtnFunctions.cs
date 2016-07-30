using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BtnFunctions : MonoBehaviour {
    public GameObject LoadingPanel;
    public GameObject NotifyPanelObj;
    private NotifyPanel notify;
    private LoadingScript panelScript;
    // Use this for initialization

    void Awake()
    {
        NotifyPanelObj = GameObject.Find("NotifyPanel");
        notify = NotifyPanelObj.GetComponent<NotifyPanel>();
        panelScript = LoadingPanel.GetComponent<LoadingScript>();
    }

    IEnumerator Start () {
        yield return CheckForUpdateToSend(false);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void GoToMap()
    {
        //SceneManager.LoadScene("map");//移到地圖
        StartCoroutine(CheckGPS());
    }

    private IEnumerator CheckGPS()
    {
        panelScript.StartLoading();
        float startTime = Time.time;
        GPS gps = new GPS();
        yield return gps.GPSInit((loc) => { });
        if (gps.GPSStatus != "GPS OK")
        {
            notify.SetText(gps.GPSStatus);
            panelScript.OnHidedCallback(() =>
            {
                notify.Show();
            });
        }
        else
        {
            AsyncOperation nextScene = SceneManager.LoadSceneAsync("map");
            nextScene.allowSceneActivation = false;
            while(nextScene.progress < 0.9f)
            {
                yield return null;
            }
            while (Time.time - startTime < 0.8f)
            {
                yield return null;
            }
            panelScript.OnHidedCallback(() =>
            {
                nextScene.allowSceneActivation = true;
            });
        }
        panelScript.EndLoading();
    }

    public void SearchEnemy()
    {
        SceneManager.LoadScene("PVPRooms");
    }

    public void TrainingClicked()
    {
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        if(data["mileage"].f > 100)
        {
            StartCoroutine(EnterTraningGame());
        }
        else
        {
            //出去走走好嗎
            //顯示錯誤訊息
        }
    }

    private IEnumerator EnterTraningGame()
    {
        float startTime = Time.time;
        panelScript.StartLoading();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入cookie
        WWW w = new WWW(Constant.SERVER_URL + "/enterTraning", null, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            PlayerPrefs.SetString("userData", w.text);
            AsyncOperation nextScene = SceneManager.LoadSceneAsync("StatsTraining");
            nextScene.allowSceneActivation = false;
            while (nextScene.progress < 0.9f)
            {
                yield return null;
            }
            while (Time.time - startTime < 0.8f)
            {
                yield return null;
            }
            panelScript.OnHidedCallback(() =>
            {
                nextScene.allowSceneActivation = true;
            });
        }
        else
        {
            //你4不4偷改數據
            Debug.Log(w.error);
            //顯示錯誤訊息
        }
        panelScript.EndLoading();
        //SceneManager.LoadScene("StatsTraining");
    }

    public void LocationUpdateClicked()
    {
        StartCoroutine(GetMileageGain());
    }

    private IEnumerator GetMileageGain()
    {
        LoadingPanel.GetComponent<LoadingScript>().StartLoading();
        GPS gps = new GPS();
        yield return gps.GPSInit((loc) => { });
        if (gps.GPSStatus != "GPS OK")
        {
            notify.SetText(gps.GPSStatus);
        }
        else
        {
            yield return CheckForUpdateToSend(true);
        }
        notify.Show();
        gps.StopGPS();
        LoadingPanel.GetComponent<LoadingScript>().EndLoading();
    }

    private IEnumerator CheckForUpdateToSend(bool showNoGain)
    {
        if (PlayerPrefs.HasKey("MileageGainToUpdate"))
        {
            float gain = PlayerPrefs.GetFloat("MileageGainToUpdate");
            if (gain > 0)
            {
                yield return SendMileageGain((int)gain);
                notify.SetText("你獲得了" + gain.ToString() + "點里程");
                notify.Show();
            }
            else
            {
                if (showNoGain)
                {
                    notify.SetText("你沒有獲得里程");
                    notify.Show();
                }
            }
            PlayerPrefs.SetFloat("MileageGainToUpdate", 0);
        }
    }

    private IEnumerator SendMileageGain(int gain)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入cookie
        WWWForm form = new WWWForm();
        form.AddField("mileageGain", gain);
        WWW w = new WWW(Constant.SERVER_URL + "/mileageGain", form.data, headers);
        yield return w;
        if (!string.IsNullOrEmpty(w.text))
        {
            PlayerPrefs.SetString("userData", w.text);
        }
        else
        {
            Debug.Log(w.error);
        }
        //臭
        GameObject.Find("UserPanel").GetComponent<ShowUserInfo>().UpdateUserInfo();
    }
}
