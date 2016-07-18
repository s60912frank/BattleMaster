using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleView : MonoBehaviour {
    public GameObject Partner;
    public GameObject Enemy;
    public GameObject PartnerSkillEffect;
    public GameObject EnemySkillEffect;//<---
    public Text EnemyHP;
    public Text PartnerHP;
    public Text messageBoxText;
    public Text messageEnemyMove;

    public Text EnemyCrit;
    public Text EnemyDefDrop;
    public Text EnemyOnFire;

    public Text PartnerCrit;
    public Text PartnerDefDrop;
    public Text PartnerOnFire;

    public GameObject VictoryPanel;
    public GameObject DefeatPanel;

    private BattlePhase battlePhase;
    private Click click;
    // Use this for initialization
    void Start () {
        JSONObject partnerData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject enemyData = new JSONObject(PlayerPrefs.GetString("enemyAI"));
        EnemyHP.text = "Enemy HP:" + enemyData["stamina"].f.ToString();
        PartnerHP.text = "Partner HP:" + partnerData["stamina"].f.ToString();
        Debug.Log("87878787" + enemyData.ToString());
        battlePhase = new BattlePhase(enemyData, partnerData);
        click = GameObject.Find("BtnManager").GetComponent<Click>();
	}

    public void RoundStart(BattlePhase.Movement myMovement)
    {
        click.SetBtnsEnabled(false);
        BattleRoundResult result = battlePhase.RoundStart(myMovement, (BattlePhase.Movement)Random.Range(0,5));
        if(result.isEnemySkillActivated)
            EnemySkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
        if(result.isPartnerSkillActivated)
            PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
        messageBoxText.text = result.partnerStatusText;
        messageEnemyMove.text = result.enemyStatusText;

        EnemyCrit.text = result.isEnemyNextCritical ? "爆擊" : "";
        EnemyDefDrop.text = result.isEnemyDefenseDropped ? "降防" : "";
        EnemyOnFire.text = result.isEnemyOnfire ? "燃燒" : "";

        PartnerCrit.text = result.isPartnerNextCritical ? "爆擊" : "";
        PartnerDefDrop.text = result.isPartnerDefenseDropped ? "降防" : "";
        PartnerOnFire.text = result.isPartnerOnfire ? "燃燒" : "";

        EnemyHP.text = "Enemy HP:" + result.enemyHp;
        PartnerHP.text = "Partner HP:" + result.partnerHp;
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
