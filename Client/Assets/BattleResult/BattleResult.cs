using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleResult : MonoBehaviour {
    public Text ResultText;
    public Text MileageText;
	// Use this for initialization
	void Start () {
        JSONObject battleResult = new JSONObject(PlayerPrefs.GetString("BattleResult"));
        PlayerPrefs.DeleteKey("BattleResult");
        if (battleResult["result"].str == "win")
        {
            ResultText.text = "勝利";
        }
        else if(battleResult["result"].str == "lose")
        {
            ResultText.text = "失敗";
        }
        else if (battleResult["result"].str == "even")
        {
            ResultText.text = "平手";
        }
        MileageText.text = "里程: " + battleResult["mileage"].f.ToString() + "(+" + battleResult["mileageIncrease"].f.ToString() + ")";
        //update userdata
        JSONObject userData = new JSONObject(PlayerPrefs.GetString("userData"));
        userData["mileage"] = battleResult["mileage"];
        PlayerPrefs.SetString("userData", userData.ToString());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PVPButtonClicked()
    {
        SceneManager.LoadScene("PVPRooms");
    }

    public void StatusButtonClicked()
    {
        SceneManager.LoadScene("Status");
    }
}
