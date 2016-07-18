using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlePhase {
    public MonsterData2 enemy;
	public MonsterData2 partner;

	public enum Movement
	{
		Attack,
		Defense,
		Evade,
		Charge,
		Skill
	}

    public BattlePhase(JSONObject enemy, JSONObject partner)
    {
        this.enemy = new MonsterData2(enemy);
        this.partner = new MonsterData2(partner);
    }

    public BattleRoundResult RoundStart(Movement partnerMovement, Movement enemyMovement)//Press Confirm button to enter battle phase
    {
		//SetBtnsEnable (false);
        BattleRoundResult result = new BattleRoundResult();
        //Enemy Movement
        int enemysRandomMove = Random.Range(0, 3);
        switch (enemyMovement)
        {
			case Movement.Attack:
                result.enemyStatusText = "The enemy attacked!";
                //yourEvadeNumber = Random.Range(0, 100);
                /* See Enemy's attack below */
                break;
            case Movement.Defense:
                result.enemyStatusText = "The enemy tried to defend your attack.";
                break;
            case Movement.Evade:
                result.enemyStatusText = "The enemy tried to evade your attack.";
                break;
			case Movement.Charge:
                if (enemy.IsSkillReady)
                {
                    enemy.Skill(ref partner);
                    //EnemySkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
                    result.isEnemySkillActivated = true;
                    enemyMovement = Movement.Skill;
                    result.enemyStatusText = "敵人使用了技能!";
                }
                else
                    enemy.Charge();
                result.enemyStatusText = "敵人正在蓄能!";
                break;
        }

        //Partner Movement
		switch (partnerMovement)
        {
			case Movement.Attack:
                int evadeNumber = Random.Range(0, 100);
				int evade = enemy.Evade * (enemyMovement == Movement.Evade ? 2 : 1);
				int defense = enemy.Defense;
                if(enemyMovement == Movement.Defense)
                {
                    defense *= 2;
                    enemy.RecoverDefense();
                    enemy.Charge();
                    //enemy防禦力回復了 並且charge+1
                }
				int attack = partner.Attack * (partner.IsNextCritical ? 2 : 1);
                if (evade < evadeNumber)//update messageBox text
                {
					int damage = attack - defense;
                    enemy.TakeDamage(damage);
                    result.enemyDamageTake = damage;
                    result.partnerStatusText = "Attack hit and dealt " + damage + " damage.";
                }
                else
                {
                    result.partnerStatusText = "The Enemy evaded your attack...";
					if (enemyMovement == Movement.Evade)
                    {
                        enemy.SetNextCricical(true);
                    }
                }
				partner.SetNextCricical(false);
                break;
			case Movement.Defense:
				if (enemyMovement != Movement.Attack)
                {
					partner.DropDefense ();
                    result.partnerStatusText = "你的防禦降低了!";
                    Debug.Log ("Parter的防禦降到" + partner.Defense + "了!");
                }
                break;
			case Movement.Evade:
				partnerMovement = Movement.Evade;
                break;
			case Movement.Charge:
                partner.Charge();
                break;
		    case Movement.Skill:
                if (partner.IsSkillReady)
                {
                    partner.Skill(ref enemy);
                    //PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
                    result.isPartnerSkillActivated = true;
                }
                break;
        }
		partner.Burn ();
        result.isPartnerOnfire = partner.IsOnFire;
        
        /* Enemy's attack */
		if (enemyMovement == Movement.Attack)
        {
            if(partnerMovement == Movement.Defense)
            {
                //防禦時迴避規0
                partner.Charge();
                partner.RecoverDefense();
            }
            else
            {
                int yourEvadeNumber = Random.Range(0, 100);
                int evade = partner.Evade * (partnerMovement == Movement.Evade ? 2 : 1);
                int enemyAtt = enemy.Attack * (enemy.IsNextCritical ? 2 : 1);
                if (evade < yourEvadeNumber)
                {
                    //迴避失敗
                    int damage = enemyAtt - partner.Defense;
                    result.partnerDamageTake = damage;
                    partner.TakeDamage(damage);
                    result.partnerStatusText = "You took " + damage + " damage";
                }
                else
                {
                    //迴避成功
                    result.partnerStatusText = "You dodged the enemy's attack.";
                    if (partnerMovement == Movement.Evade)
                    {
                        partner.SetNextCricical(true);
                    }
                }
            }
			enemy.SetNextCricical(false);
        }
        else if(enemyMovement == Movement.Defense)
        {
            if (partnerMovement != Movement.Attack)
            {
                enemy.DropDefense();
            }
                
        }
		enemy.Burn ();
        result.isPartnerNextCritical = partner.IsNextCritical;
        result.isEnemyNextCritical = enemy.IsNextCritical;
        result.isPartnerDefenseDropped = partner.IsDenfenseDropped;
        result.isEnemyDefenseDropped = enemy.IsDenfenseDropped;
        result.isEnemyOnfire = enemy.IsOnFire;
        result.enemyRemainingCD = enemy.RemainingCD;
        result.partnerRemainingCD = partner.RemainingCD;
        result.enemyHp = enemy.Stamina;
        result.partnerHp = partner.Stamina;
        //ROUND OVER
        return result;
    }

	public string CheckIfGameOver()
	{
        if(partner.Stamina <= 0 || enemy.Stamina <= 0)
        {
            if (partner.Stamina <= 0 && enemy.Stamina <= 0)
            {
                return "even";
            }
            else
            {
                if (enemy.Stamina <= 0)
                {
                    return "win";
                }
                else if (partner.Stamina <= 0)
                {
                    return "lose";
                }
            }
        }
        return "no";
	}

    public IEnumerator WaitForBattleResult(string result)
    {
        WWWForm form = new WWWForm();
        form.AddField("enemyName", enemy.Name);
        form.AddField("result", result);
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入認證過的cookie就不用重新登入了
        WWW w = new WWW(Constant.SERVER_URL + "/battleAIResult", form.data, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.text);
            PlayerPrefs.SetString("BattleResult", w.text);
            //SceneManager.LoadScene("BattleResult");
        }
        else
        {
            Debug.Log(w.error);
        }
    }

    public string GetSkillBtnText()
    {
        if (partner.IsSkillReady)
        {
            return "Skill";
        }
        else
        {
            return "Charge" + partner.NowCharge + "/" + partner._skillCD;
        }
    }

    public bool IsPartnerSkillReady()
    {
        return partner.IsSkillReady;
    }
}
