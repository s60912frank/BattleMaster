using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SetResult : MonoBehaviour {
    public Text StaminaText;
    public Text AttackText;
    public Text DefenseText;
    public Text EvadeText;
    // Use this for initialization
    void Start () {
        //開啟particle system if之前有關
        var particleEmission = GameObject.Find("Particle System").GetComponent<ParticleSystem>().emission;
        particleEmission.enabled = true;

        JSONObject petData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject traningResult = new JSONObject(PlayerPrefs.GetString("trainingResult"));
        StaminaText.text = string.Format("血量:{0}(+{1})", petData["stamina"].f, traningResult["staminaIncrease"].f);
        AttackText.text = string.Format("攻擊:{0}(+{1})", petData["attack"].f, traningResult["attackIncrease"].f);
        DefenseText.text = string.Format("防禦:{0}(+{1})", petData["defense"].f, traningResult["defenseIncrease"].f);
        EvadeText.text = string.Format("迴避:{0}(+{1})", petData["evade"].f, traningResult["evadeIncrease"].f);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //使用者按了返回鍵
            BackToStatusClicked();
        }
    }

    public void BackToStatusClicked()
    {
        SceneManager.LoadScene("Status");
    }
}
