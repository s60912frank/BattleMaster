using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BtnFunctions : MonoBehaviour {
    public GameObject LoadingPanel;
    public GameObject NotifyPanelObj;
    private NotifyPanel notify;
    // Use this for initialization

    void Awake()
    {
        NotifyPanelObj = GameObject.Find("NotifyPanel");
        notify = NotifyPanelObj.GetComponent<NotifyPanel>();
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
        LoadingPanel.GetComponent<LoadingScript>().StartLoading();
        StartCoroutine(CheckGPS());
    }

    private IEnumerator CheckGPS()
    {
        GPS gps = new GPS();
        yield return gps.GPSInit((loc) => { });
        if (gps.GPSStatus != "GPS OK")
        {
            notify.SetText(gps.GPSStatus);
            notify.Show();
        }
        else
        {
            SceneManager.LoadScene("map");//移到地圖
        }
        LoadingPanel.GetComponent<LoadingScript>().EndLoading();
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
