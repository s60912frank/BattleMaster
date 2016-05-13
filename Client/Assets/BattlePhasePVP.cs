using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine.SceneManagement;

public class BattlePhasePVP : MonoBehaviour {

    public GameObject Partner;
    public GameObject Enemy;
    public GameObject PartnerSkillEffect;
    public GameObject EnemySkillEffect;
    public Text EnemyHP;
    public Text PartnerHP;
    public Text messageBoxText;
    public Text messageEnemyMove;
    public Text EnemyCrit;
    public Text PartnerCrit;

    public EnemyData enemyData;
    public PartnerData partnerData;

    private SocketIOComponent socket;
    private Dictionary<string, string> attackResult;

	public enum Movement
	{
		Attack,
		Defense,
		Evade,
		Charge,
		Skill
	}
	public Movement partnerMovement = Movement.Attack;
	public Movement enemyMovement = Movement.Attack;

	// Use this for initialization
	void Start () {
        attackResult = new Dictionary<string, string>();
        attackResult.Add("nextCritical", "false");
        attackResult.Add("damage", "0");
        attackResult.Add("skillBoost", "false");
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
        enemyData.skillCD = int.Parse(data["skill"]["CD"].ToString());
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
        partnerData.skillCD = int.Parse(partner["skill"]["CD"].ToString()); //skill still buggy
    }

    private void OnEnemyMoveMent(SocketIOEvent e)
        //當enemy選好動作時觸發
    {
        JSONObject data = e.data;
		int movement = int.Parse(data["movement"].ToString().Replace("\"", ""));
        Debug.Log("Enemy:" + movement + "!");
		enemyMovement = (Movement)movement;
		enemyData.NextCritical = bool.Parse(data["nextCritical"].ToString().Replace("\"", ""));
        messageEnemyMove.text = "Enemy is ready.";
    }

	// Update is called once per frame
	void Update ()//Show and hide critical hit hint
    {
		if (enemyData.NextCritical){
            EnemyCrit.text = "Next hit Critical";
        }
        else
        {
            EnemyCrit.text = "";
        }
		if (partnerData.NextCritical){
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
		movement.Add("movement", ((int)partnerMovement).ToString());
		movement.Add("nextCritical", partnerData.NextCritical.ToString());
        socket.Emit("movement", new JSONObject(movement)); //傳送自己的動作
		Debug.Log("YourMovement:" + partnerMovement);

		if (partnerMovement == Movement.Charge)
			partnerData.charge++;
		if (partnerMovement == Movement.Attack && partnerData.NextCritical)
			partnerData.NextCritical = false;
    }

    private void OnAttackStart(SocketIOEvent e)
        //當雙方都選好動作就會開始攻擊(?)
    {
        Debug.Log("BATTLE!");
        //you can't charge or launch skill at this point.
        //Enemy's attack 為方便移到這邊分開寫
		if (enemyMovement == Movement.Attack)
        {
			int evade = partnerData.evade * (partnerMovement == Movement.Evade ? 2 : 1);
			int defense = partnerData.defense * (partnerMovement == Movement.Defense ? 2 : 1);
			int enemyAtt = enemyData.attack * (enemyData.NextCritical ? 2 : 1);
			//如果攻擊的話crit必歸0
			attackResult["nextCritical"] = "false";
            int yourEvadeNumber = Random.Range(0, 100);
			if (evade < yourEvadeNumber)
            {
				int damage = enemyAtt - defense;
                partnerData.stamina = partnerData.stamina - damage;
                messageBoxText.text = "You took " + damage + " damage";
                PartnerHP.text = "Partner HP:" + partnerData.stamina;
				if (partnerMovement == Movement.Defense)
                {
					//Defend success --> Skill boost
                    partnerData.charge++;
                    attackResult["skillBoost"] = "true";
                }
                attackResult["damage"] = damage.ToString();
            }
            else
            {
                //Attack dodged
                messageBoxText.text = "You dodged the enemy's attack.";
				if (partnerMovement == Movement.Evade)
                {
					partnerData.NextCritical = true;
                    attackResult["nextCritical"] = "true";
                }
                else
                {
					partnerData.NextCritical = false;
                    attackResult["nextCritical"] = "false";
                }
                attackResult["damage"] = "0";
            }
			enemyData.NextCritical = false;
        }
        //
        else
        {
            attackResult["damage"] = "0";
            attackResult["nextCritical"] = "false";
            attackResult["skillBoost"] = "false";
        }

		if (partnerMovement == Movement.Skill)
        {
            Skill();
        }
		if (enemyMovement == Movement.Skill)
		{
			EnemySkill();
		}
        socket.Emit("result", new JSONObject(attackResult));
        Debug.Log("Partner Take " + attackResult["damage"] + " damage!");

        //clear value
        attackResult["damage"] = "0";
        attackResult["nextCritical"] = "false";
        attackResult["skillBoost"] = "false";
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
        }
        else
        {
			if(partnerMovement == Movement.Attack)
                messageEnemyMove.text = "The Enemy evaded your attack...";
        }
		enemyData.NextCritical = bool.Parse(data["critical"].ToString().Replace("\"", ""));
		print ("ENEMY CRIT:" + data["critical"].ToString ().Replace ("\"", ""));
		if (bool.Parse(data["skillBoost"].ToString()))
        {
            enemyData.charge++;
        }
    }

    private void OnEnemyLeave(SocketIOEvent e)
        //敵人離開時觸發
    {
        messageEnemyMove.text = "The Enemy leave the battle...";
        socket.Close();
		Destroy (GameObject.Find ("SocketIO"));
		SceneManager.LoadScene("waitForBattle"); 
    }

    private void OnApplicaionQuit()
    {
        socket.Close();//關閉socket
    }


    private void Skill()
    {
        partnerData.stamina += 15;
        enemyData.stamina -= 15;
        partnerData.attack += 5;
        PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
		partnerData.charge = 0;
        print("Your skill activated!");
		print (partnerData.stamina);
    }

    private void EnemySkill()
    {
        enemyData.stamina += 15;
        partnerData.stamina -= 15;
        enemyData.attack += 5;
        EnemySkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
		enemyData.charge = 0;
        print("Enemy skill activated!");
		print (enemyData.stamina);
    }
}
