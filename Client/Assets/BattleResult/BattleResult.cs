using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleResult : MonoBehaviour {
    public Text ResultText;
    public Text IncreaseText;
	// Use this for initialization
	void Start () {
        //換音樂
        AudioSource bgm = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        JSONObject battleResult = new JSONObject(PlayerPrefs.GetString("BattleResult"));
        PlayerPrefs.DeleteKey("BattleResult");
        if (battleResult["result"].str == "win")
        {
            ResultText.text = "勝利";
            bgm.clip = Resources.Load<AudioClip>("music/win");
            bgm.Play();
        }
        else if(battleResult["result"].str == "lose")
        {
            ResultText.text = "失敗";
        }
        else if (battleResult["result"].str == "even")
        {
            ResultText.text = "平手";
        }
        //update userdata
        JSONObject userData = new JSONObject(PlayerPrefs.GetString("userData"));
        if(battleResult.HasField("mileageIncrease")){
            IncreaseText.text = string.Format("里程: {0}(+{1})", battleResult["mileage"].f, battleResult["mileageIncrease"].f);
            userData["mileage"] = battleResult["mileage"];
        }
        else if(battleResult.HasField("coinIncrease")){
            IncreaseText.text = string.Format("金幣: {0}(+{1})", battleResult["coin"].f, battleResult["coinIncrease"].f);
            userData["coin"] = battleResult["coin"];
        }
        else{
            IncreaseText.text = "出錯啦!";
        }
        PlayerPrefs.SetString("userData", userData.ToString());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StatusButtonClicked()
    {
        SceneManager.LoadScene("Status");
    }
}
