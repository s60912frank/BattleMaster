using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class BattleViewPVP : MonoBehaviour {
    private SocketIOComponent socket;
    private GameObject SocketIOObj;
        
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
    public GameObject TutorialPanel;

    private BattlePhase battlePhase;
    private AnimationController animationController;
    private ClickPVP click;
    private StatusScript partnerBar;
    private StatusScript enemyBar;
    private ResultPanelController victoryPanel;
    private ResultPanelController defeatPanel;
    private ExplainPanel tutPanel;

    void Awake()
    {
        partnerBar = partnerStatus.GetComponent<StatusScript>();
        enemyBar = enemyStatus.GetComponent<StatusScript>();
        victoryPanel = VictoryPanel.GetComponent<ResultPanelController>();
        defeatPanel = DefeatPanel.GetComponent<ResultPanelController>();
        tutPanel = TutorialPanel.GetComponent<ExplainPanel>();
    }

    // Use this for initialization
    void Start () {
        //換音樂
        AudioSource bgm = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        bgm.clip = Resources.Load<AudioClip>("music/BattlePVP");
        bgm.Play();

        if(!PlayerPrefs.HasKey("showBattleTutorial") || PlayerPrefs.GetInt("showBattleTutorial") == 1){
            tutPanel.Show();
        }

        animationController = SpriteMgr.GetComponent<AnimationController>();
        SocketIOObj = GameObject.Find("SocketIO");
        socket = SocketIOObj.GetComponent<SocketIOComponent>();
        //socket事件們
        socket.On("enemyData", OnReceiveEnemyData);
        socket.On("enemyMovement", OnEnemyMovement);
        socket.On("attackStart", OnAttackStart);
        socket.On("enemyMovementResult", OnEnemyMoveMentResult);
        socket.On("enemyLeave", OnEnemyLeave);
        socket.On("battleResult2", OnBattleResult);

        
        click = GameObject.Find("BtnManager").GetComponent<ClickPVP>();
        //click.SetBtnsEnabled(false);
        socket.Emit("battleSceneReady");
	}

    private void OnReceiveEnemyData(SocketIOEvent e)
    //取得敵人資訊時存起來
    {
        JSONObject partnerData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject enemyData = e.data;
        partnerBar.SetMax((int)partnerData["stamina"].f, (int)partnerData["skill"]["CD"].f, (int)partnerData["defense"].f);
        enemyBar.SetMax((int)enemyData["stamina"].f, (int)enemyData["skill"]["CD"].f, (int)enemyData["defense"].f);
        //將圖片改成他們所選的夥伴
        SetIcon(partnerIcon, partnerSkillEffectIcon, partnerData["name"].str);
        SetIcon(enemyIcon, enemySkillEffectIcon, enemyData["name"].str);

        Debug.Log("87878787" + enemyData.ToString());
        click.SetBtnsEnabled(true);
        battlePhase = new BattlePhase(enemyData, partnerData);
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

    private void OnEnemyMovement(SocketIOEvent e)
    //當enemy選好動作時觸發
    {
        JSONObject data = e.data;
        battlePhase.SetEnemyMovement((BattlePhase.Movement)(int.Parse(data["movement"].str)));
        Debug.Log("Enemy:" + data["movement"].str + "!");
        messageEnemyMove.text = "敵人已經準備好了";
    }

    private void OnAttackStart(SocketIOEvent e)
    {
        RoundStart();
    }

    private void OnEnemyMoveMentResult(SocketIOEvent e)
    {
        JSONObject enemyResult = e.data;
        battlePhase.ProcessEnemyResult(enemyResult["isEnemyNextCritical"].b, (int)enemyResult["enemyDamageTake"].f, enemyResult["isEnemyEvaded"].b);
        Debug.Log("ENEMYDAMAGETAKE:" + (int)enemyResult["enemyDamageTake"].f);
        //then display result
        StartCoroutine(DisplayResult(battlePhase.GetRoundResult()));
    }

    private void OnEnemyLeave(SocketIOEvent e)
    //敵人離開時觸發
    {
        victoryPanel.Show();
    }


    //戰鬥整個結束的時候
    private void OnBattleResult(SocketIOEvent e)
    {
        PlayerPrefs.SetString("BattleResult", e.data.ToString());
        Debug.Log(e.data.ToString());
        socket.Close();
        Destroy(GameObject.Find("SocketIO"));
        victoryPanel.SetButtonInteractable(true);
        defeatPanel.SetButtonInteractable(true);
    }

    public void SetMyMovement(BattlePhase.Movement myMovement)
    {
        click.SetBtnsEnabled(false);
        battlePhase.SetPartnerMovement(myMovement);
        socket.Emit("movement", new JSONObject(new Dictionary<string, string>() { { "movement", ((int)myMovement).ToString() } })); //傳送自己的動作
    }

    private void RoundStart()
    {
        battlePhase.RoundStart();
        BattleRoundResult result = battlePhase.GetRoundResult();
        JSONObject resultToSend = new JSONObject();
        resultToSend.AddField("isEnemyNextCritical", result.isPartnerNextCritical);
        resultToSend.AddField("enemyDamageTake", result.partnerDamageTake);
        resultToSend.AddField("isEnemyEvaded", result.isPartnerEvaded);
        Debug.Log("PARTNERDMGTAKE:" + result.partnerDamageTake);
        socket.Emit("result", resultToSend);
    }

    private IEnumerator DisplayResult(BattleRoundResult result)
    {
        //這裡要來撥放動畫了
        yield return animationController.BattleAnimation(result);
        StartCoroutine(enemyBar.UpdateStatus(result.enemyHp, result.enemyRemainingCD, result.enemyDefense));
        yield return partnerBar.UpdateStatus(result.partnerHp, result.partnerRemainingCD, result.partnerDefense);

        //顯示Partner結果
        messageBoxText.text = result.partnerStatusText;

        //顯示Enemy結果
        messageEnemyMove.text = result.enemyStatusText;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        string isGameOver = battlePhase.CheckIfGameOver();
        JSONObject result = new JSONObject();
        result.AddField("result", isGameOver);
        switch (isGameOver)
        {
            case "even":
                victoryPanel.Show();
                socket.Emit("battleEnd", result);
                break;
            case "win":
                victoryPanel.Show();
                socket.Emit("battleEnd", result);
                break;
            case "lose":
                defeatPanel.Show();
                socket.Emit("battleEnd", result);
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
        try
        {
            return battlePhase.GetSkillBtnText();
        }
        catch
        {
            return "";
        }
        
    }

    public bool IsPartnerSkillReady()
    {
        try
        {
            return battlePhase.IsPartnerSkillReady();
        }
        catch
        {
            return false;
        }
    }

    public void LeaveBattle()
    {
        socket.Close();
        Destroy(SocketIOObj);
    }
}
