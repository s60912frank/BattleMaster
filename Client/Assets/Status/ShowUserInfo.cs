using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowUserInfo : MonoBehaviour {
    public Text UserNameText;
    public Text MileageText;
    // Use this for initialization
    void Start ()
    {
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        UserNameText.text = data["name"].str;
        //顯示里程
        MileageText.text = string.Format("里程:{0}", data["mileage"].f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateUserInfo()
    {
        Start();
    }
}
