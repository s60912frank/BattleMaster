using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattlePhase : MonoBehaviour {

    public GameObject Partner;
    public GameObject Enemy;
    public GameObject PartnerSkillEffect;
    public GameObject EnemySkillEffect;//<---
    public MonsterData enemy;
	public MonsterData partner;
    public Text EnemyHP;
    public Text PartnerHP;
    public Text messageBoxText;
    public Text messageEnemyMove;
    public Text EnemyCrit;
    public Text PartnerCrit;
	public GameObject VictoryPanel;
	public GameObject DefeatPanel;
    private JSONObject enemyData;
    private JSONObject userData;

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

	private List<Button> buttons; //把按鈕存在這控制enable/disable

	void Start () {
		VictoryPanel.SetActive (false);
		DefeatPanel.SetActive (false);
        enemyData = new JSONObject(PlayerPrefs.GetString("enemyAI"));
        userData = new JSONObject(PlayerPrefs.GetString("userData"));
        //set status from scripts
        enemy = new MonsterData();
        enemy.Initialize(enemyData);
		partner = new MonsterData();
        partner.Initialize(userData["pet"]);

		buttons = new List<Button> ();
		foreach (GameObject btn in GameObject.FindGameObjectsWithTag("MovementBtn")) 
		{
			buttons.Add(btn.GetComponent<Button>());
		}
	}
	
	void Update ()//Update UI
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
        //Enemy Movement
        int enemysRandomMove = Random.Range(0, 3);
        switch (enemysRandomMove)
        {
			case 0:
				enemyMovement = Movement.Attack;
                messageEnemyMove.text = "The enemy attacked!";
                //yourEvadeNumber = Random.Range(0, 100);
                /* See Enemy's attack below */
                break;
            case 1:
				enemyMovement = Movement.Defense;
                messageEnemyMove.text = "The enemy tried to defend your attack.";
                break;
            case 2:
				enemyMovement = Movement.Evade;
                messageEnemyMove.text = "The enemy tried to evade your attack.";
                break;
			case 3:
				if (enemy.IsSkillReady) 
				{
					enemy.Skill (ref partner);
					EnemySkillEffect.GetComponent<PartnerSkillEffectEntry> ().activated = true;
				}
				else
					enemy.charge++;
                break;
        }

        //Partner Movement
		switch (partnerMovement)
        {
			case Movement.Attack:
                int evadeNumber = Random.Range(0, 100);
				int evade = enemy.evade * (enemyMovement == Movement.Evade ? 2 : 1);
				int defense = enemy.defense * (enemyMovement == Movement.Defense ? 2 : 1);
				int attack = partner.attack * (partner.NextCritical ? 2 : 1);
                if (evade < evadeNumber)//update messageBox text
                {
					int damage = attack - defense;
                    enemy.stamina -= damage;
                    messageBoxText.text = "Attack hit and dealt " + damage + " damage.";
                }
                else
                {
                    messageBoxText.text = "The Enemy evaded your attack...";
					if (enemyMovement == Movement.Evade)
						enemy.NextCritical = true;
                }
				partner.NextCritical = false;
                break;
			case Movement.Defense:
				if (enemyMovement != Movement.Attack)
                {
					partner.DropDefense ();
					Debug.Log ("Parter的防禦降到" + partner.defense + "了!");
                }
                break;
			case Movement.Evade:
				partnerMovement = Movement.Evade;
                break;
			case Movement.Charge:
                partner.charge++;
                break;
		case Movement.Skill:
                //partnerData.charge = 0;
                //PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
                //Partner.GetComponent<PartnerData>().Skill();
				partner.Skill (ref enemy);
				PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry> ().activated = true;
                break;
        }
		partner.Burn ();
        
        /* Enemy's attack */
		if (enemyMovement == Movement.Attack)
        {
			int yourEvadeNumber = Random.Range(0, 100);
			int evade = partner.evade * (partnerMovement == Movement.Evade ? 2 : 1);
			int defense = partner.defense * (partnerMovement == Movement.Defense ? 2 : 1);
			int enemyAtt = enemy.attack * (enemy.NextCritical ? 2 : 1);
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
				}
			}
            else
            {
                //Attack dodged
                messageBoxText.text = "You dodged the enemy's attack.";
				if (partnerMovement == Movement.Evade)
					partner.NextCritical = true;
            }
			enemy.NextCritical = false;
        }
		enemy.Burn ();
		SetBtnsEnable (true);
		CheckIfGameOver ();
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
        if(partner.stamina <= 0 || enemy.stamina <= 0)
        {
            SetBtnsEnable(false);
            WWWForm form = new WWWForm();
            form.AddField("enemyName", enemyData["name"].str);
            if (partner.stamina <= 0 && enemy.stamina <= 0)
            {
                VictoryPanel.SetActive(true);//其實應該要是平手
                form.AddField("result", "even");
            }
            else
            {
                if (enemy.stamina <= 0)
                {
                    VictoryPanel.SetActive(true);
                    form.AddField("result", "win");
                }
                else if (partner.stamina <= 0)
                {
                    DefeatPanel.SetActive(true);
                    form.AddField("result", "lose");
                }
            }
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入認證過的cookie就不用重新登入了
            WWW w = new WWW(Constant.SERVER_URL + "/battleAIResult", form.data, headers);
            StartCoroutine(WaitForBattleResult(w));
        }
	}

    private IEnumerator WaitForBattleResult(WWW w)
    {
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.text);
            PlayerPrefs.SetString("BattleResult", w.text);
            SceneManager.LoadScene("BattleResult");
        }
        else
        {
            Debug.Log(w.error);
        }
    }
}
