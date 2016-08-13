using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleView : MonoBehaviour {
    //public GameObject PartnerSkillEffect;
    //public GameObject EnemySkillEffect;//<---
        
    //Enemy顯示
    public Text messageEnemyMove;
    public GameObject enemyStatus;
    public GameObject enemyIcon;
    public GameObject enemySkillEffectIcon;

    //Partner顯示
    public Text messageBoxText;
    public GameObject partnerStatus;
    public GameObject partnerIcon;
    public GameObject partnerSkillEffectIcon;

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
        animationController = SpriteMgr.GetComponent<AnimationController>();
        JSONObject partnerData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject enemyData = new JSONObject(PlayerPrefs.GetString("enemyAI"));
        partnerBar.SetMax((int)partnerData["stamina"].f, (int)partnerData["skill"]["CD"].f, (int)partnerData["defense"].f);
        enemyBar.SetMax((int)enemyData["stamina"].f, (int)enemyData["skill"]["CD"].f, (int)enemyData["defense"].f);
        SetIcon(partnerIcon, partnerSkillEffectIcon, partnerData["name"].str);
        SetIcon(enemyIcon, enemySkillEffectIcon, enemyData["name"].str);
        battlePhase = new BattlePhase(enemyData, partnerData);
        click = GameObject.Find("BtnManager").GetComponent<Click>();
	}

    private void SetIcon(GameObject icon, GameObject skill, string name)
    {
        SpriteRenderer renderer = icon.GetComponent<SpriteRenderer>();
        Sprite sprite = Resources.Load<Sprite>(name);
        float scale = (icon.transform.localScale.x * renderer.sprite.texture.width) / sprite.texture.width;
        icon.transform.localScale = Vector3.one * scale;
        renderer.sprite = sprite;
        skill.GetComponent<Image>().sprite = sprite;
    }

    public void SetMyMovement(BattlePhase.Movement myMovement)
    {
        battlePhase.SetPartnerMovement(myMovement);
        StartCoroutine(RoundStart());
    }

    private IEnumerator RoundStart()
    {
        click.SetBtnsEnabled(false);
        battlePhase.SetEnemyMovement((BattlePhase.Movement)Random.Range(0, 4));
        battlePhase.RoundStart();
        BattleRoundResult result = battlePhase.GetRoundResult();

        //這裡要來撥放動畫了
        yield return animationController.BattleAnimation(result);
        StartCoroutine(enemyBar.UpdateStatus(result.enemyHp, result.enemyRemainingCD, result.enemyDefense));
        yield return partnerBar.UpdateStatus(result.partnerHp, result.partnerRemainingCD, result.partnerDefense);

        //顯示Enemy結果
        messageBoxText.text = result.partnerStatusText;

        //顯示Partner結果
        messageEnemyMove.text = result.enemyStatusText;

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
