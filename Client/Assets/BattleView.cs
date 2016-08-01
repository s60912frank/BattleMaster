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
    public Text EnemyCrit;
    public Text EnemyOnFire;
    public Text messageEnemyMove;
    public GameObject enemyStatus;

    //Partner顯示
    public Text PartnerCrit;
    public Text PartnerOnFire;
    public Text messageBoxText;
    public GameObject partnerStatus;

    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public GameObject SpriteMgr;

    private BattlePhase battlePhase;
    private AnimationController animationController;
    private Click click;
    private StatusScript partnerBar;
    private StatusScript enemyBar;
    void Awake()
    {
        partnerBar = partnerStatus.GetComponent<StatusScript>();
        enemyBar = enemyStatus.GetComponent<StatusScript>();
    }

    // Use this for initialization
    void Start () {
        //this.gameObject.ac
        //partnerBar = partnerStatus.GetComponent<StatusScript>();
        animationController = SpriteMgr.GetComponent<AnimationController>();
        JSONObject partnerData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject enemyData = new JSONObject(PlayerPrefs.GetString("enemyAI"));
        partnerBar.SetMax((int)partnerData["stamina"].f, (int)partnerData["skill"]["CD"].f, (int)partnerData["defense"].f);
        enemyBar.SetMax((int)enemyData["stamina"].f, (int)enemyData["skill"]["CD"].f, (int)enemyData["defense"].f);
        battlePhase = new BattlePhase(enemyData, partnerData);
        click = GameObject.Find("BtnManager").GetComponent<Click>();
	}

    public void SetMyMovement(BattlePhase.Movement myMovement)
    {
        battlePhase.SetPartnerMovement(myMovement);
        StartCoroutine(RoundStart());
    }

    private IEnumerator RoundStart()
    {
        click.SetBtnsEnabled(false);
        battlePhase.SetEnemyMovement((BattlePhase.Movement)Random.Range(0, 5));
        battlePhase.RoundStart();
        BattleRoundResult result = battlePhase.GetRoundResult();

        //這裡要來撥放動畫了
        yield return animationController.BattleAnimation(result);
        StartCoroutine(enemyBar.UpdateStatus(result.enemyHp, result.enemyRemainingCD, result.enemyDefense));
        yield return partnerBar.UpdateStatus(result.partnerHp, result.partnerRemainingCD, result.partnerDefense);

        //顯示Enemy結果
        messageBoxText.text = result.partnerStatusText;
        EnemyCrit.text = result.isEnemyNextCritical ? "爆擊" : "";
        //EnemyDefDrop.text = result.isEnemyDefenseDropped ? "降防" : "";
        EnemyOnFire.text = result.isEnemyOnfire ? "燃燒" : "";
        if (result.isEnemySkillActivated)
            EnemySkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;

        //顯示Partner結果
        messageEnemyMove.text = result.enemyStatusText;
        PartnerCrit.text = result.isPartnerNextCritical ? "爆擊" : "";
        PartnerOnFire.text = result.isPartnerOnfire ? "燃燒" : "";
        if (result.isPartnerSkillActivated)
            PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;

        yield return CheckGameOver();
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
