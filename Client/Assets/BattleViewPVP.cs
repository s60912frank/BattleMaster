using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class BattleViewPVP : MonoBehaviour {
    private SocketIOComponent socket;
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
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        //socket事件們
        socket.On("enemyData", OnReceiveEnemyData);
        socket.On("enemyMovement", OnEnemyMovement);
        socket.On("attackStart", OnAttackStart);
        socket.On("enemyMovementResult", OnEnemyMoveMentResult);
        socket.On("enemyLeave", OnEnemyLeave);
        socket.On("battleResult2", OnBattleResult);

        
        click = GameObject.Find("BtnManager").GetComponent<Click>();
	}

    private void OnReceiveEnemyData(SocketIOEvent e)
    //取得敵人資訊時存起來
    {
        JSONObject partnerData = new JSONObject(PlayerPrefs.GetString("userData"))["pet"];
        JSONObject enemyData = e.data;
        EnemyHP.text = "Enemy HP:" + enemyData["stamina"].f.ToString();
        EnemyCD.text = "CD:" + enemyData["skill"]["CD"].f.ToString();
        PartnerHP.text = "Partner HP:" + partnerData["stamina"].f.ToString();
        PartnerCD.text = "CD:" + partnerData["skill"]["CD"].f.ToString();
        //Debug.Log("87878787" + enemyData.ToString());
        battlePhase = new BattlePhase(enemyData, partnerData);
    }

    private void OnEnemyMovement(SocketIOEvent e)
    //當enemy選好動作時觸發
    {
        JSONObject data = e.data;
        battlePhase.SetEnemyMovement((BattlePhase.Movement)data.f);
        Debug.Log("Enemy:" + data["movement"].f + "!");
        messageEnemyMove.text = "Enemy is ready.";
    }

    private void OnAttackStart(SocketIOEvent e)
    {
        RoundStart();
    }

    private void OnEnemyMoveMentResult(SocketIOEvent e)
    {
        JSONObject enemyResult = e.data;
        battlePhase.ProcessEnemyResult(enemyResult["isEnemyNextCritical"].b, (int)enemyResult["enemyDamageTake"].f);
        //then display result
        DisplayResult(battlePhase.GetRoundResult());
    }

    private void OnEnemyLeave(SocketIOEvent e)
    //敵人離開時觸發
    {
        messageEnemyMove.text = "The Enemy leave the battle...";
        socket.Close();
        Destroy(GameObject.Find("SocketIO"));
        //SceneManager.LoadScene("waitForBattle");
        VictoryPanel.SetActive(true);
    }

    private void OnBattleResult(SocketIOEvent e)
    {
        //JSONObject result = e.data;
        PlayerPrefs.SetString("BattleResult", e.data.ToString());
        Debug.Log(e.data.ToString());
        socket.Close();
        Destroy(GameObject.Find("SocketIO"));
        SceneManager.LoadScene("BattleResult");
    }

    public void SetMyMovement(BattlePhase.Movement myMovement)
    {
        battlePhase.SetEnemyMovement(myMovement);
        socket.Emit("movement", new JSONObject((int)myMovement)); //傳送自己的動作
    }

    private void RoundStart()
    {
        click.SetBtnsEnabled(false);
        battlePhase.RoundStart();
        BattleRoundResult result = battlePhase.GetRoundResult();
        JSONObject resultToSend = new JSONObject();
        resultToSend.AddField("isEnemyNextCritical", result.isPartnerNextCritical);
        resultToSend.AddField("enemyDamageTake", result.partnerDamageTake);
        socket.Emit("result", resultToSend);
    }

    private void DisplayResult(BattleRoundResult result)
    {
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
                socket.Emit("battleEnd", new JSONObject(isGameOver));
                break;
            case "win":
                VictoryPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                yield return battlePhase.WaitForBattleResult(isGameOver);
                socket.Emit("battleEnd", new JSONObject(isGameOver));
                break;
            case "lose":
                DefeatPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                yield return battlePhase.WaitForBattleResult(isGameOver);
                socket.Emit("battleEnd", new JSONObject(isGameOver));
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
