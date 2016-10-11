using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BtnFunctions : MonoBehaviour {
    public GameObject LoadingPanel;
    public GameObject NotifyPanelObj;
    public GameObject ConfirmExitPanel;

    private NotifyPanel notifyScript;
    private LoadingScript panelScript;
    private Panel confirmExitScript;
    // Use this for initialization

    void Awake()
    {
        NotifyPanelObj = GameObject.Find("NotifyPanel");
        notifyScript = NotifyPanelObj.GetComponent<NotifyPanel>();
        panelScript = LoadingPanel.GetComponent<LoadingScript>();
        confirmExitScript = ConfirmExitPanel.GetComponent<Panel>();
    }

    IEnumerator Start () {
        //換音樂
        AudioSource bgm = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        bgm.clip = Resources.Load<AudioClip>("music/Menu");
        bgm.Play();
        
        confirmExitScript.SetText("確定要離開遊戲?");
        confirmExitScript.SetConfirmListener(() => { Application.Quit(); });
        yield return CheckForUpdateToSend(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //使用者按了返回鍵
            confirmExitScript.Show();
        }
	}

    public void GoToMap()
    {
        //移到地圖
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        if(data["mileage"].f >= 100)
        {
            StartCoroutine(CheckGPS());
        }
        else
        {
            //顯示錯誤訊息
            notifyScript.SetText("需要100里程才能與野怪對戰，出去走走吧!");
            notifyScript.Show();
        }
    }

    private IEnumerator CheckGPS()
    {
        panelScript.StartLoading();
        float startTime = Time.time;
        GPS gps = new GPS();
        yield return gps.GPSInit((loc) => { });
        if (gps.GPSStatus != "GPS OK")
        {
            notifyScript.SetText(gps.GPSStatus);
            panelScript.OnHidedCallback(() =>
            {
                notifyScript.Show();
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
        if(data["coin"].f >= 1000)
        {
            StartCoroutine(EnterTraningGame());
        }
        else
        {
            //出去走走好嗎
            //顯示錯誤訊息
            notifyScript.SetText("需要1000金幣才能訓練，去地圖上與野怪對戰吧!");
            notifyScript.Show();
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
            notifyScript.SetText("你.....偷改數據喔");
            notifyScript.Show();
        }
        panelScript.EndLoading();
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
            notifyScript.SetText(gps.GPSStatus);
        }
        else
        {
            yield return CheckForUpdateToSend(true);
        }
        notifyScript.Show();
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
                notifyScript.SetText("你獲得了" + gain.ToString() + "點里程");
                notifyScript.Show();
            }
            else
            {
                if (showNoGain)
                {
                    notifyScript.SetText("你沒有獲得里程");
                    notifyScript.Show();
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
