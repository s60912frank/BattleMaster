using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlePhase {
    public MonsterData2 enemy;
	public MonsterData2 partner;
    private Movement enemyMovement = Movement.Attack;
    private Movement partnerMovement = Movement.Attack;
    private BattleRoundResult roundResult;

    public void SetEnemyMovement(Movement movement)
    {
        enemyMovement = movement;
    }

    public void SetPartnerMovement(Movement movement)
    {
        partnerMovement = movement;
    }

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

    public void RoundStart()//Press Confirm button to enter battle phase
    {
        //SetBtnsEnable (false);
        roundResult = null;
        roundResult = new BattleRoundResult();
        //Enemy Movement
        switch (enemyMovement)
        {
			case Movement.Attack:
                roundResult.enemyStatusText = "敵人發動攻擊！";
                /* See Enemy's attack below */
                break;
            case Movement.Defense:
                roundResult.enemyStatusText = "敵人擺出防禦架式！";
                break;
            case Movement.Evade:
                roundResult.enemyStatusText = "敵人嘗試迴避！";
                break;
			case Movement.Charge:
                if (enemy.IsSkillReady)
                {
                    enemy.Skill(ref partner);
                    roundResult.isEnemySkillActivated = true;
                    enemyMovement = Movement.Skill;
                    roundResult.enemyStatusText = "敵人發動了技能！" + enemy.SkillDescription;
                }
                else{
                    enemy.Charge();
                    roundResult.enemyStatusText = "敵人正在蓄能！";
                }
                break;
        }

        //Partner Movement
		switch (partnerMovement)
        {
			case Movement.Attack:
                int attack = partner.Attack * (partner.IsNextCritical ? 2 : 1);
                int defense = enemy.Defense;
                if(enemyMovement == Movement.Defense)
                {
                    defense *= 2;
                    int damage = attack - defense;
                    if (damage <= 0)
                        damage = 1;
                    enemy.TakeDamage(damage);
                    roundResult.enemyDamageTake = damage;
                    roundResult.partnerStatusText = "我方攻擊造成了 " + damage + " 點傷害！";
                    enemy.RecoverDefense();
                    enemy.Charge();
                    //enemy防禦力回復了 並且charge+1
                }
                else
                {
                    int evadeNumber = Random.Range(0, 100);
                    int evade = enemy.Evade * (enemyMovement == Movement.Evade ? 2 : 1);
                    if (evade < evadeNumber)//update messageBox text
                    {
                        int damage = attack - defense;
                        if (damage <= 0)
                            damage = 1;
                        enemy.TakeDamage(damage);
                        roundResult.enemyDamageTake = damage;
                        roundResult.partnerStatusText = "我方攻擊造成了 " + damage + " 點傷害！";
                    }
                    else
                    {
                        roundResult.partnerStatusText = "敵人迴避了我方攻擊...";
                        roundResult.isEnemyEvaded = true;
                        if (enemyMovement == Movement.Evade)
                        {
                            enemy.SetNextCricical(true);
                        }
                    }
                }
				partner.SetNextCricical(false);
                break;
			case Movement.Defense:
				if (enemyMovement != Movement.Attack)
                {
					partner.DropDefense ();
                    roundResult.partnerStatusText = "你的防禦降低了!";
                    //Debug.Log ("Parter的防禦降到" + partner.Defense + "了!");
                }
                break;
			case Movement.Evade:
                //nothing happened
                break;
			case Movement.Charge:
                if (partner.IsSkillReady)
                {
                    partner.Skill(ref enemy);
                    //PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
                    roundResult.isPartnerSkillActivated = true;
                    partnerMovement = Movement.Skill;
                }
                else
                {
                    partner.Charge();
                }
                break;
        }
		partner.Burn ();
        roundResult.isPartnerOnfire = partner.IsOnFire;
        
        /* Enemy's attack */
		if (enemyMovement == Movement.Attack)
        {
            int attack = enemy.Attack * (enemy.IsNextCritical ? 2 : 1);
            int defense = partner.Defense;
            if (partnerMovement == Movement.Defense)
            {
                defense *= 2;
                int damage = attack - defense;
                if (damage <= 0)
                    damage = 1;
                partner.TakeDamage(damage);
                roundResult.partnerDamageTake = damage;
                roundResult.enemyStatusText = "我方攻擊造成了 " + damage + " 點傷害！";
                partner.RecoverDefense();
                partner.Charge();
            }
            else
            {
                int yourEvadeNumber = Random.Range(0, 100);
                int evade = partner.Evade * (partnerMovement == Movement.Evade ? 2 : 1);
                if (evade < yourEvadeNumber)
                {
                    //迴避失敗
                    int damage = attack - partner.Defense;
                    if (damage <= 0)
                        damage = 1;
                    roundResult.partnerDamageTake = damage;
                    partner.TakeDamage(damage);
                    roundResult.partnerStatusText = "我方承受了 " + damage + " 點傷害！";
                }
                else
                {
                    //迴避成功
                    roundResult.partnerStatusText = "成功迴避敵方攻擊！";
                    roundResult.isPartnerEvaded = true;
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
        roundResult.isPartnerNextCritical = partner.IsNextCritical;
        roundResult.isEnemyNextCritical = enemy.IsNextCritical;
        roundResult.enemyDefense = enemy.Defense;
        roundResult.partnerDefense = partner.Defense;
        roundResult.isEnemyOnfire = enemy.IsOnFire;
        roundResult.isPartnerOnfire = partner.IsOnFire;
        roundResult.enemyRemainingCD = enemy.RemainingCD;
        roundResult.partnerRemainingCD = partner.RemainingCD;
        roundResult.enemyHp = enemy.Stamina;
        roundResult.partnerHp = partner.Stamina;
        roundResult.partnerMovement = partnerMovement;
        roundResult.enemyMovement = enemyMovement;
        //ROUND OVER
    }

    public BattleRoundResult GetRoundResult()
    {
        return roundResult;
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

    public void ProcessEnemyResult(bool isNextCrit, int damageTake, bool isEnemyEvaded)
    {
        roundResult.isEnemyNextCritical = isNextCrit;
        enemy.SetNextCricical(isNextCrit);
        enemy.TakeDamage(-roundResult.enemyDamageTake);
        roundResult.enemyDamageTake = damageTake;
        enemy.TakeDamage(damageTake);
        roundResult.enemyHp = enemy.Stamina;
        roundResult.isEnemyEvaded = isEnemyEvaded;
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
        }
        else
        {
            Debug.Log(w.error);
        }
    }

    public string GetSkillBtnText()
    {
        try
        {
            if (partner.IsSkillReady)
            {
                return "技能";
            }
            else
            {
                return "蓄能(" + partner.NowCharge + "/" + partner._skillCD + ")";
            }
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
            return partner.IsSkillReady;
        }
        catch
        {
            return false;
        }
    }
}
