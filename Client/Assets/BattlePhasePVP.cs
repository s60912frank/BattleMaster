using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class BattlePhasePVP : MonoBehaviour {

    public GameObject Partner;
    public GameObject Enemy;
    public GameObject PartnerSkillEffect;
    public Text EnemyHP;
    public Text PartnerHP;
    public Text messageBoxText;
    public Text messageEnemyMove;
    public Text EnemyCrit;
    public Text PartnerCrit;

    public EnemyData enemyData;
    public PartnerData partnerData;

    private SocketIOComponent socket;

    private string _yourNextMove = "Attack";
    public string yourNextMove
    {
        get
        {
            return _yourNextMove;
        }
        set
        {
            _yourNextMove = value;
        }
    }

    private int evadeNumber;//to present rate of evasiveness when Partner's atk lands
    private int enemyDefendActivated = 0;
    private int enemyEvadeActivated = 0;
    private int enemyAttackActivated = 0;
    private int enemyCritActivated = 0;
    private int enemysRandomMove;//temp

    private int yourEvadeNumber;//to present rate of evasiveness when Enemy's atk lands
    private int partnerDefendActivated = 0;
    private int partnerEvadeActivated = 0;
    private int partnerCritActivated = 0;

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        //set status from scripts
        enemyData = Object.Instantiate(Enemy.GetComponent<EnemyData>());
        partnerData = Object.Instantiate(Partner.GetComponent<PartnerData>());
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        //socket事件們
        socket.On("enemyMovement", OnEnemyMoveMent);
        socket.On("attackStart", OnAttackStart);
        socket.On("enemyMovementResult", OnEnemyMoveMentResult);
        socket.On("enemyData", OnReceiveEnemyData);
        socket.On("enemyLeave", OnEnemyLeave);
        socket.Emit("battleSceneReady"); //告訴server我準備好了
	}


    private void OnReceiveEnemyData(SocketIOEvent e)
        //取得敵人資訊時存起來
    {
        SetInitinalData(); //先這樣吧
        JSONObject data = e.data;
        Debug.Log("EnemyData:" + data.ToString());
        enemyData.stamina = int.Parse(data["stamina"].ToString());
        enemyData.attack = int.Parse(data["attack"].ToString());
        enemyData.defense = int.Parse(data["defense"].ToString());
        enemyData.evade = int.Parse(data["evade"].ToString());
        //enemyData.skillCD = int.Parse(data["skill"]["CD"].ToString()); //skill still buggy
    }

    private void SetInitinalData()
        //讀取自己partner的資訊
    {
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        JSONObject partner = data["pet"];
        Debug.Log("PartnerData:" + partner.ToString());
        partnerData.stamina = int.Parse(partner["stamina"].ToString());
        partnerData.attack = int.Parse(partner["attack"].ToString());
        partnerData.defense = int.Parse(partner["defense"].ToString());
        partnerData.evade = int.Parse(partner["evade"].ToString());
        //partnerData.skillCD = int.Parse(partner["skill"]["CD"].ToString()); //skill still buggy
    }

    private void OnEnemyMoveMent(SocketIOEvent e)
        //當enemy選好動作時觸發
    {
        JSONObject data = e.data;
        string movement = data["movement"].ToString().Replace("\"", "");
        Debug.Log("Enemy:" + movement + "!");
        switch (movement)
        {
            case "Attack":
                enemyAttackActivated = 1;
                break;
            case "Defend":
                enemyDefendActivated = 1;
                break;
            case "Evade":
                enemyEvadeActivated = 1;
                break;
            case "Charge":
                //enemyEvadeActivated = 1;
                enemyData.charge++;
                break;
            case "Skill":
                //enemyEvadeActivated = 1;
                break;
        }
        enemyCritActivated = int.Parse(data["critical"].ToString().Replace("\"", ""));
        messageEnemyMove.text = "Enemy is ready.";
    }

	// Update is called once per frame
	void Update ()//Show and hide critical hit hint
    {
        if (enemyCritActivated == 1){
            EnemyCrit.text = "Next hit Critical";
        }
        else
        {
            EnemyCrit.text = "";
        }
        if (partnerCritActivated == 1){
            PartnerCrit.text = "Next hit Critical";
        }
        else
        {
            PartnerCrit.text = "";
        }
        EnemyHP.text = "Enemy HP:" + enemyData.stamina;
        PartnerHP.text = "Partner HP:" + partnerData.stamina;
	}

    public void enterBattlePhase()//Press Confirm button to enter battle phase
    {
        //傳送自己的動作
        Dictionary<string, string> movement = new Dictionary<string, string>();
        movement.Add("movement", _yourNextMove);
        movement.Add("critical", partnerCritActivated.ToString());
        socket.Emit("movement", new JSONObject(movement)); //傳送自己的動作
        Debug.Log("YourMovement:" + _yourNextMove);
        switch (_yourNextMove)
        {
            case "Attack":
                //nothing to do
                //有critical的話剛剛已經傳出去了，這裡重置
                partnerCritActivated = 0;
                break;
            case "Defend":
                partnerDefendActivated = 1;
                break;
            case "Evade":
                partnerEvadeActivated = 1;
                break;
            case "Charge":
                partnerData.charge++;
                break;
            case "Skill":
                partnerData.charge = 0;
                PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
                Partner.GetComponent<PartnerData>().Skill();//SOMETHING IS WRONG
                Debug.Log("hey! over here.");
                break;
        }        
    }

    private void OnAttackStart(SocketIOEvent e)
        //當雙方都選好動作就會開始攻擊(?)
    {
        Debug.Log("BATTLE!");
        //you can't charge or launch skill at this point.
        Dictionary<string, string> attackResult = new Dictionary<string, string>();
        //Enemy's attack 為方便移到這邊分開寫
        if (enemyAttackActivated == 1)
        {
            yourEvadeNumber = Random.Range(0, 100);
            if (partnerData.evade * (1 + partnerEvadeActivated) < yourEvadeNumber)
            {
                int damage = enemyData.attack * (1 + enemyCritActivated) - partnerData.defense * (1 + partnerDefendActivated);
                partnerData.stamina = partnerData.stamina - damage;
                messageBoxText.text = "You took " + damage + " damage";
                PartnerHP.text = "Partner HP:" + partnerData.stamina;
                if (partnerDefendActivated == 1)
                {
                    partnerData.charge++;//Defend success --> Skill boost
                    attackResult.Add("skillBoost", "true");
                }
                attackResult.Add("damage", damage.ToString());
                attackResult.Add("critical", "0");
            }
            else
            {
                //Attack dodged
                messageBoxText.text = "You dodged the enemy's attack.";
                if (partnerEvadeActivated == 1)
                {
                    partnerCritActivated = 1;
                    attackResult.Add("critical", "1");
                }
                else
                {
                    attackResult.Add("critical", "0");
                }
                attackResult.Add("damage", "0");
            }
            enemyCritActivated = 0;
        }
        else
        {
            attackResult.Add("critical", "0");
            attackResult.Add("damage", "0");
            attackResult.Add("skillBoost", "false");
        }

        socket.Emit("result", new JSONObject(attackResult));
        //Debug.Log((new JSONObject(attackResult)).ToString());
        Debug.Log("Partner Take " + attackResult["damage"] + " damage!");
    }

    private void OnEnemyMoveMentResult(SocketIOEvent e)
        //當對方計算好自己的傷害時會觸發這個
    {
        Debug.Log("EnemyMovementResult:" + e.data);
        JSONObject data = e.data;
        int damage = int.Parse(data["damage"].ToString().Replace("\"", ""));
        Debug.Log("Enemy Take " + damage.ToString() + "damage!");
        if (damage != 0)
        {
            enemyData.stamina -= damage;
            EnemyHP.text = "Enemy HP:" + enemyData.stamina;
        }
        else
        {
            if(_yourNextMove == "Attack")
                messageEnemyMove.text = "The Enemy evaded your attack...";
        }
        enemyCritActivated = int.Parse(data["critical"].ToString().Replace("\"", ""));
        if (data["skillBoost"] != null)
        {
            enemyData.charge++;
        }

        //set back variables
        partnerDefendActivated = 0;
        partnerEvadeActivated = 0;
        enemyDefendActivated = 0;
        enemyEvadeActivated = 0;
        enemyAttackActivated = 0;
    }

    private void OnEnemyLeave(SocketIOEvent e)
        //敵人離開時觸發
    {
        messageEnemyMove.text = "The Enemy leave the battle...";
    }

    private void OnApplicaionQuit()
    {
        socket.Close();//關閉socket
    }
}
