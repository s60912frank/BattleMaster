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
	public GameObject VictoryPanel;
	public GameObject DefeatPanel;
    public GameObject LoadingPanel;

    public MonsterData enemy;
	public MonsterData partner;

    private SocketIOComponent socket;
    private Dictionary<string, string> attackResult;

	private List<Button> buttons; //把按鈕存在這控制enable/disable

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
        //開始loading畫面
        //LoadingPanel = Resources.Load("LoadingPanel") as GameObject;
        //LoadingPanel.GetComponent<RectTransform>().SetParent(GameObject.Find("Canvas").transform);
        //LoadingPanel.transform.parent = GameObject.Find("Canvas").transform;
        LoadingPanel.GetComponent<LoadingScript>().Start();
        LoadingPanel.GetComponent<LoadingScript>().StartLoading();
		VictoryPanel.SetActive (false);
		DefeatPanel.SetActive (false);
        attackResult = new Dictionary<string, string>();
        attackResult.Add("nextCritical", "false");
        attackResult.Add("damage", "0");
        attackResult.Add("skillRemainingCD", "0");
		attackResult.Add("defenseDropped", "false");
		attackResult.Add("defenseRecovered", "false");
		attackResult.Add("isOnFire", "false");
        Application.runInBackground = true;
        //set status from scripts
		enemy = new MonsterData();
		partner = new MonsterData();
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        //socket事件們
        socket.On("enemyMovement", OnEnemyMoveMent);
        socket.On("attackStart", OnAttackStart);
        socket.On("enemyMovementResult", OnEnemyMoveMentResult);
        socket.On("enemyData", OnReceiveEnemyData);
		socket.On("enemyLeave", OnEnemyLeave);
		buttons = new List<Button> ();
		foreach (GameObject btn in GameObject.FindGameObjectsWithTag("MovementBtn")) 
		{
			buttons.Add(btn.GetComponent<Button>());
		}
        socket.Emit("battleSceneReady"); //告訴server我準備好了
	}

    private void OnReceiveEnemyData(SocketIOEvent e)
        //取得敵人資訊時存起來
    {
        SetInitinalData(); //先這樣吧
        JSONObject data = e.data;
        Debug.Log("EnemyData:" + data.ToString());
		enemy.Initialize (data);
		attackResult["skillRemainingCD"] = data["skill"]["CD"].ToString();
        //結束讀取畫面
        LoadingPanel.GetComponent<LoadingScript>().EndLoading();
    }

    private void SetInitinalData()
        //讀取自己partner的資訊
    {
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        JSONObject monster = data["pet"];
        Debug.Log("PartnerData:" + partner.ToString());
		partner.Initialize (monster);
    }

    private void OnEnemyMoveMent(SocketIOEvent e)
        //當enemy選好動作時觸發
    {
        JSONObject data = e.data;
		int movement = int.Parse(GetString(data, "movement"));
        Debug.Log("Enemy:" + movement + "!");
		enemyMovement = (Movement)movement;
		//enemyData.NextCritical = bool.Parse(data["nextCritical"].ToString().Replace("\"", ""));
        messageEnemyMove.text = "Enemy is ready.";
    }

	// Update is called once per frame
	void Update ()//Show and hide critical hit hint
    {
		if (enemy.NextCritical){
            EnemyCrit.text = "Next hit Critical";
        }
        else
        {
            EnemyCrit.text = "";
        }
		if (partner.NextCritical){
            PartnerCrit.text = "Next hit Critical";
        }
        else
        {
            PartnerCrit.text = "";
        }
        EnemyHP.text = "Enemy HP:" + enemy.stamina;
        PartnerHP.text = "Partner HP:" + partner.stamina;
	}

    public void enterBattlePhase()//Press Confirm button to enter battle phase
	{
		SetBtnsEnable (false);
        //傳送自己的動作
        Dictionary<string, string> movement = new Dictionary<string, string>();
		movement.Add("movement", ((int)partnerMovement).ToString());
		movement.Add("nextCritical", partner.NextCritical.ToString());
        socket.Emit("movement", new JSONObject(movement)); //傳送自己的動作
		Debug.Log("YourMovement:" + partnerMovement);

		if (partnerMovement == Movement.Charge)
			partner.charge++;
		if (partnerMovement == Movement.Attack && partner.NextCritical)
			partner.NextCritical = false;
    }

    private void OnAttackStart(SocketIOEvent e)
        //當雙方都選好動作就會開始攻擊(?)
    {
        Debug.Log("BATTLE!");
        //you can't charge or launch skill at this point.
        //Enemy's attack 為方便移到這邊分開寫
		if (enemyMovement == Movement.Attack)
        {
			int evade = partner.evade * (partnerMovement == Movement.Evade ? 2 : 1);
			int defense = partner.defense * (partnerMovement == Movement.Defense ? 2 : 1);
			int enemyAtt = enemy.attack * (enemy.NextCritical ? 2 : 1);
			//如果攻擊的話crit必歸0
			attackResult["nextCritical"] = "false";
            int yourEvadeNumber = Random.Range(0, 100);
			if (evade < yourEvadeNumber)
            {
				int damage = enemyAtt - defense;
				partner.TakeDamage (damage);
                messageBoxText.text = "You took " + damage + " damage";
				if (partnerMovement == Movement.Defense)
                {
					//Defend success --> Skill boost
                    partner.charge++;
					partner.RecoverDefense();
					attackResult["defenseRecovered"] = "true";
                }
                attackResult["damage"] = damage.ToString();
            }
            else
            {
                //Attack dodged
                messageBoxText.text = "You dodged the enemy's attack.";
				if (partnerMovement == Movement.Evade)
                {
					partner.NextCritical = true;
                    attackResult["nextCritical"] = "true";
                }
                else
                {
					partner.NextCritical = false;
                    attackResult["nextCritical"] = "false";
                }
                attackResult["damage"] = "0";
            }
			enemy.NextCritical = false;
        }
        //
        else
        {
            attackResult["damage"] = "0";
            attackResult["nextCritical"] = "false";
            //attackResult["skillBoost"] = "false";
			if(partnerMovement == Movement.Defense)
			{
				partner.DropDefense ();
				attackResult["defenseDropped"] = "true";
				Debug.Log ("Parter的防禦降到" + partner.defense + "了!");
			}
        }

		if (partnerMovement == Movement.Skill)
        {
			partner.Skill (ref enemy);
			PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry> ().activated = true;
        }
		if (enemyMovement == Movement.Skill)
		{
			enemy.Skill (ref partner);
			EnemySkillEffect.GetComponent<PartnerSkillEffectEntry> ().activated = true;
		}
		attackResult["skillRemainingCD"] = partner.RemainingCD.ToString();
		if(partner.BurnDamage > 0)
		{
			attackResult["isOnFire"] = "true";
			Debug.Log ("我正在燃燒!");
            Color whee = Color.white;
            whee.a = 200;
            Partner.GetComponentInChildren<Image>().color = whee;
            Debug.Log("6666666666:" + Partner.GetComponentInChildren<Image>().color);
		}
        socket.Emit("result", new JSONObject(attackResult));
        Debug.Log("Partner Take " + attackResult["damage"] + " damage!");
		partner.Burn ();

        //clear value
        attackResult["damage"] = "0";
        attackResult["nextCritical"] = "false";
		attackResult["defenseDropped"] = "false";
		attackResult["defenseRecovered"] = "false";
    }

    private void OnEnemyMoveMentResult(SocketIOEvent e)
        //當對方計算好自己的傷害時會觸發這個
    {
        Debug.Log("EnemyMovementResult:" + e.data);
        JSONObject data = e.data;
		int damage = int.Parse(GetString(data, "damage"));
        Debug.Log("Enemy Take " + damage.ToString() + "damage!");
        if (damage != 0)
        {
            enemy.stamina -= damage;
        }
        else
        {
			if(partnerMovement == Movement.Attack)
                messageEnemyMove.text = "The Enemy evaded your attack...";
        }
		enemy.NextCritical = bool.Parse(GetString(data, "nextCritical"));
		if (bool.Parse (GetString(data, "defenseDropped"))) {
			Debug.Log ("敵方的防禦降低了!");
		}
		if (bool.Parse (GetString(data, "defenseRecovered"))) {
			Debug.Log ("敵方的防禦恢復了!");
		}
		if (bool.Parse (GetString(data, "isOnFire"))) {
			Debug.Log ("敵人正在燃燒!");
			enemy.Burn ();
		}
		Debug.Log ("ENEMY剩" + int.Parse (GetString(data, "skillRemainingCD")) + "CD就可以使用技能");
		SetBtnsEnable (true);
		CheckIfGameOver ();
    }

    private void OnEnemyLeave(SocketIOEvent e)
        //敵人離開時觸發
    {
        messageEnemyMove.text = "The Enemy leave the battle...";
        socket.Close();
		Destroy (GameObject.Find ("SocketIO"));
		//SceneManager.LoadScene("waitForBattle");
		VictoryPanel.SetActive(true);
    }

    private void OnApplicaionQuit()
    {
        socket.Close();//關閉socket
    }

	private string GetString(JSONObject data, string property)
	{
		return data[property].ToString().Replace("\"", "");
	}

	private void SetBtnsEnable(bool state)
	{
		foreach (Button btn in buttons) 
		{
			btn.interactable = state;
		}
	}

	private void CheckIfGameOver()
	{
		if (partner.stamina <= 0 && enemy.stamina <= 0) 
		{
			VictoryPanel.SetActive (true);//其實應該要是平手
			socket.Close();
			Destroy (GameObject.Find ("SocketIO"));
		} 
		else 
		{
			if (enemy.stamina <= 0) 
			{
				VictoryPanel.SetActive (true);
				socket.Close();
				Destroy (GameObject.Find ("SocketIO"));
			} 
			else if(partner.stamina <= 0)
			{
				DefeatPanel.SetActive (true);
				socket.Close();
				Destroy (GameObject.Find ("SocketIO"));
			}
		}
	}
}
