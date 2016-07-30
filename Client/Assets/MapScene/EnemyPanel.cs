using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyPanel : MonoBehaviour {
    public RawImage EnemyImage;
    public Text NameText;
    public Text StaminaText;
    public Text AttackText;
    public Text RewardText;
    public Button BattleButton;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetEnemyData(JSONObject data)
    {
        Debug.Log(data.ToString());
        PlayerPrefs.SetString("enemyAI", data.ToString());
        NameText.text = "名稱:" + data["name"].str;
        StaminaText.text = "血量:" + data["stamina"].f.ToString();
        AttackText.text = "攻擊:" + data["attack"].f.ToString();
        RewardText.text = "獎勵:" + data["reward"].f.ToString();
        BattleButton.interactable = data["inRange"].b;
        Show();
    }

    private void Show()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.GetComponent<Animator>().SetTrigger("StartShow");
    }

    public void Hide()
    {
        gameObject.GetComponent<Animator>().SetTrigger("StartHide");
        //gameObject.GetComponent<Animator>().
        //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000, 0);
        //e臭
        //StartCoroutine(Dirty());
    }

    public void HideAndSetMouseRay()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000, 0);
        //臭
        Camera.main.gameObject.GetComponent<CamController>().MouseRay = true;
    }

    private IEnumerator Dirty()
    {
        yield return new WaitForSeconds(0.2f);
        Camera.main.gameObject.GetComponent<CamController>().MouseRay = true;
    }

    public void Battle()
    {
        //.GetComponent<LoadingScript>().EndLoading();
        StartCoroutine(BattleWithAI());
    }

    private IEnumerator BattleWithAI()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入cookie
        WWW w = new WWW(Constant.SERVER_URL + "/battleWithAI", null, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            PlayerPrefs.SetString("userData", w.text);
            SceneManager.LoadScene("Battle2");
        }
        else
        {
            Debug.Log(w.error);
        }
    }
}
