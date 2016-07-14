using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class EnemyPanel : MonoBehaviour {
    public RawImage EnemyImage;
    public Text NameText;
    public Text StaminaText;
    public Text AttackText;
    public Text RewardText;
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
        Show();
    }

    private void Show()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void Hide()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000, 0);
        //e臭
        StartCoroutine(Dirty());
    }

    private IEnumerator Dirty()
    {
        yield return new WaitForSeconds(0.2f);
        Camera.main.gameObject.GetComponent<CamController>().MouseRay = true;
    }

    public void Battle()
    {
        //.GetComponent<LoadingScript>().EndLoading();
        SceneManager.LoadScene("Battle2");
    }
}
