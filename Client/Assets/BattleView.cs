using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleView : MonoBehaviour {
    public GameObject Partner;
    public GameObject Enemy;
    public GameObject PartnerSkillEffect;
    public GameObject EnemySkillEffect;//<---
        
    //Enemy顯示
    public Text EnemyHP;
    public Text EnemyCD;
    public Text EnemyCrit;
    public Text EnemyDefDrop;
    public Text EnemyOnFire;
    public Text messageEnemyMove;

    //Partner顯示
    public Text PartnerHP;
    public Text PartnerCD;
    public Text PartnerCrit;
    public Text PartnerDefDrop;
    public Text PartnerOnFire;
    public Text messageBoxText;

    public GameObject VictoryPanel;
    public GameObject DefeatPanel;

    private BattlePhase battlePhase;
    private Click click;
    // Use this for initialization
    void Start () {
        //this.gameObject.ac
        JSONObject partnerData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject enemyData = new JSONObject(PlayerPrefs.GetString("enemyAI"));
        EnemyHP.text = "Enemy HP:" + enemyData["stamina"].f.ToString();
        EnemyCD.text = "CD:" + enemyData["skill"]["CD"].f.ToString();
        PartnerHP.text = "Partner HP:" + partnerData["stamina"].f.ToString();
        PartnerCD.text = "CD:" + partnerData["skill"]["CD"].f.ToString();
        //Debug.Log("87878787" + enemyData.ToString());
        battlePhase = new BattlePhase(enemyData, partnerData);
        click = GameObject.Find("BtnManager").GetComponent<Click>();
	}

    public void SetMyMovement(BattlePhase.Movement myMovement)
    {
        battlePhase.SetPartnerMovement(myMovement);
        RoundStart();
    }

    private void RoundStart()
    {
        click.SetBtnsEnabled(false);
        battlePhase.SetEnemyMovement((BattlePhase.Movement)Random.Range(0, 5));
        battlePhase.RoundStart();
        BattleRoundResult result = battlePhase.GetRoundResult();

        //顯示Partner結果
        messageBoxText.text = result.partnerStatusText;
        EnemyHP.text = "Enemy HP:" + result.enemyHp;
        EnemyCD.text = "CD:" + result.enemyRemainingCD.ToString();
        EnemyCrit.text = result.isEnemyNextCritical ? "爆擊" : "";
        EnemyDefDrop.text = result.isEnemyDefenseDropped ? "降防" : "";
        EnemyOnFire.text = result.isEnemyOnfire ? "燃燒" : "";
        if (result.isEnemySkillActivated)
            EnemySkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;

        //顯示Enemy結果
        messageEnemyMove.text = result.enemyStatusText;
        PartnerHP.text = "Partner HP:" + result.partnerHp;
        PartnerCD.text = "CD:" + result.partnerRemainingCD.ToString();
        PartnerCrit.text = result.isPartnerNextCritical ? "爆擊" : "";
        PartnerDefDrop.text = result.isPartnerDefenseDropped ? "降防" : "";
        PartnerOnFire.text = result.isPartnerOnfire ? "燃燒" : "";
        if (result.isPartnerSkillActivated)
            PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;

        StartCoroutine(CheckGameOver());
    }

    private IEnumerator CheckGameOver()
    {
        string isGameOver = battlePhase.CheckIfGameOver();
        switch (isGameOver)
        {
            case "even":
                VictoryPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                yield return battlePhase.WaitForBattleResult(isGameOver);
                SceneManager.LoadScene("BattleResult");
                break;
            case "win":
                VictoryPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                yield return battlePhase.WaitForBattleResult(isGameOver);
                SceneManager.LoadScene("BattleResult");
                break;
            case "lose":
                DefeatPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                yield return battlePhase.WaitForBattleResult(isGameOver);
                SceneManager.LoadScene("BattleResult");
                break;
            default:
                click.SetBtnsEnabled(true);
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetSkillBtnText()
    {
        return battlePhase.GetSkillBtnText();
    }

    public bool IsPartnerSkillReady()
    {
        return battlePhase.IsPartnerSkillReady();
    }
}
