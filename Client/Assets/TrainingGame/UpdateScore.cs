using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UpdateScore : Pauseable {
    public Text TimerText;
    private int attackScore = 0;
    private int defenseScore = 0;
    private int staminaScore = 0;
    private int time = 30;
    private bool run = true;
    // Use this for initialization
    void Start () {
        StartCoroutine(DirtyWay());
        StartCoroutine(CountDownTimer());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator DirtyWay()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("GameScript").GetComponent<statsMinigame>().AddPauseableObject(this);
    }

    public override void Pause()
    {
        run = false;
    }

    public override void Resume()
    {
        run = true;
    }

    private IEnumerator CountDownTimer()
    {
        do
        {
            yield return new WaitForSeconds(1);
            if (run)
            {
                time--;
                TimerText.text = "剩餘時間:" + time.ToString();
            }
        } while (time > 0);
        //結算訓練成果
        yield return TrainingResult();
    }

    private IEnumerator TrainingResult()
    {
        JSONObject trainingResult = new JSONObject();
        WWWForm postData = new WWWForm();
        postData.AddField("staminaIncrease", staminaScore);
        trainingResult.AddField("staminaIncrease", staminaScore);
        postData.AddField("attackIncrease", attackScore);
        trainingResult.AddField("attackIncrease", attackScore);
        postData.AddField("evadeIncrease", 0);
        trainingResult.AddField("evadeIncrease", 0);
        postData.AddField("defenseIncrease", defenseScore);
        trainingResult.AddField("defenseIncrease", defenseScore);
        PlayerPrefs.SetString("trainingResult", trainingResult.ToString());
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入cookie
        WWW w = new WWW(Constant.SERVER_URL + "/traningResult", postData.data, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            PlayerPrefs.SetString("userData", w.text);
            //先直接回到狀態scene 之後改
            SceneManager.LoadScene("TrainingResultScene");
        }
        else{
            Debug.Log(w.error);
        }
    }

    public void UpdateAttackIncrease()
    {
        attackScore++;
    }

    public void UpdateDefenseScore()
    {
        defenseScore++;
    }

    public void UpdateStaminaScore()
    {
        staminaScore++;
    }
}
