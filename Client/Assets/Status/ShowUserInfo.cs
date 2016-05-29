using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowUserInfo : MonoBehaviour {
    public GameObject UserNameText;
    public GameObject MileageText;
    // Use this for initialization
    void Start ()
    {
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        string userName = data["name"].ToString().Replace("\"", "");
        UserNameText.GetComponent<Text>().text = userName;
        //之後還要顯示里程
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
