using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowUserInfo : MonoBehaviour {
    public Text UserNameText;
    public Text MileageText;
    public Text CoinText;
    // Use this for initialization
    void Start ()
    {
        //開啟particle system if之前有關
        var particleEmission = GameObject.Find("Particle System").GetComponent<ParticleSystem>().emission;
        particleEmission.enabled = true;

        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        UserNameText.text = data["name"].str;
        //顯示里程
        MileageText.text = string.Format("里程: {0}", data["mileage"].f);
        CoinText.text = string.Format("金幣: {0}", data["coin"].f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateUserInfo()
    {
        Start();
    }
}
