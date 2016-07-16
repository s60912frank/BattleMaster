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
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        if(data["mileage"].f > 100)
        {
            SceneManager.LoadScene("StatsTraining");
        }
    }
}
