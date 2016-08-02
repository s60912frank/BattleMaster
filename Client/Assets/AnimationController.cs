using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
    private SpriteController partner;
    private SpriteController enemy;
    // Use this for initialization
    void Start () {
        partner = transform.FindChild("Partner").GetComponent<SpriteController>();
        enemy = transform.FindChild("Enemy").GetComponent<SpriteController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator BattleAnimation(BattleRoundResult result)
    {
        if(result.enemyMovement == BattlePhase.Movement.Attack && result.partnerMovement == BattlePhase.Movement.Attack)
        {
            partner.SetTrigger("StartAttack");
            if (result.isEnemyEvaded)
                enemy.SetTrigger("StartEvade");
            else
                enemy.SetTrigger("BeingAttack");
            //等待直到結束
            yield return partner.WaitForFinish();
            yield return enemy.WaitForFinish();
            //攻擊後就一定沒有爆擊
            //partner.SetTrigger("SetNotCritical");

            enemy.SetTrigger("StartAttack");
            if (result.isPartnerEvaded)
                partner.SetTrigger("StartEvade");
            else
                partner.SetTrigger("BeingAttack");
            yield return enemy.WaitForFinish();
            yield return partner.WaitForFinish();
            //enemy.SetTrigger("SetNotCritical");
        }
        else if(result.enemyMovement == BattlePhase.Movement.Attack && result.partnerMovement != BattlePhase.Movement.Attack)
        {
            //自己always先
            if(result.partnerMovement == BattlePhase.Movement.Charge)
            {
                partner.SetTrigger("StartCharge");
                yield return partner.WaitForFinish();
            }
            else if(result.partnerMovement == BattlePhase.Movement.Skill)
            {
                //技能動畫 等等補
            }
            //換敵人攻擊
            enemy.SetTrigger("StartAttack");
            if (result.partnerMovement == BattlePhase.Movement.Defense)
            {
                partner.SetTrigger("StartDefend");
            }
            else if (result.partnerMovement == BattlePhase.Movement.Evade)
            {
                if (result.isPartnerEvaded)
                    partner.SetTrigger("StartEvade");
                else
                    partner.SetTrigger("BeingAttack");
            }
            else
            {
                if (result.isPartnerEvaded)
                    partner.SetTrigger("StartEvade");
                else
                    partner.SetTrigger("BeingAttack");
            }
            yield return enemy.WaitForFinish();
            yield return partner.WaitForFinish();
            //enemy.SetTrigger("SetNotCritical");
        }
        else if (result.enemyMovement != BattlePhase.Movement.Attack && result.partnerMovement == BattlePhase.Movement.Attack)
        {
            //我攻擊
            partner.SetTrigger("StartAttack");
            if (result.enemyMovement == BattlePhase.Movement.Defense)
            {
                enemy.SetTrigger("StartDefend");
            }
            else if (result.enemyMovement == BattlePhase.Movement.Evade)
            {
                if (result.isEnemyEvaded)
                    enemy.SetTrigger("StartEvade");
                else
                    enemy.SetTrigger("BeingAttack");
            }
            else
            {
                if (result.isEnemyEvaded)
                    enemy.SetTrigger("StartEvade");
                else
                    enemy.SetTrigger("BeingAttack");
            }
            yield return enemy.WaitForFinish();
            yield return partner.WaitForFinish();
            //partner.SetTrigger("SetNotCritical");

            //換敵人
            if (result.enemyMovement == BattlePhase.Movement.Charge)
            {
                enemy.SetTrigger("StartCharge");
                yield return partner.WaitForFinish();
            }
            else if (result.enemyMovement == BattlePhase.Movement.Skill)
            {
                //技能動畫 等等補
            }
        }
        else
        {
            if (result.enemyMovement == BattlePhase.Movement.Defense)
                enemy.SetTrigger("StartDefend");
            else if (result.enemyMovement == BattlePhase.Movement.Evade)
                enemy.SetTrigger("StartEvade");
            else if (result.enemyMovement == BattlePhase.Movement.Charge)
                enemy.SetTrigger("StartCharge");
            else if (result.enemyMovement == BattlePhase.Movement.Skill)
                Debug.Log("WHEESkill");
            //技能動畫等等補
            else
                Debug.LogError("邏輯錯誤!");

            if (result.partnerMovement == BattlePhase.Movement.Defense)
                partner.SetTrigger("StartDefend");
            else if (result.partnerMovement == BattlePhase.Movement.Evade)
                partner.SetTrigger("StartEvade");
            else if (result.partnerMovement == BattlePhase.Movement.Charge)
                partner.SetTrigger("StartCharge");
            else if (result.partnerMovement == BattlePhase.Movement.Skill)
                Debug.Log("WHEESkill");
            //技能動畫等等補
            else
                Debug.LogError("邏輯錯誤!");

            yield return enemy.WaitForFinish();
            yield return partner.WaitForFinish();
        }

        //enemy.SetBool("IsNextCritical", result.isEnemyNextCritical);
        //partner.SetBool("IsNextCritical", result.isPartnerNextCritical);

        if (result.isEnemyNextCritical)
        {
            enemy.SetTrigger("SetNextCritical");
        }
        else
        {
            enemy.SetTrigger("SetNotCritical");
        }

        if (result.isPartnerNextCritical)
        {
            partner.SetTrigger("SetNextCritical");
        }
        else
        {
            partner.SetTrigger("SetNotCritical");
        }
    }

    /*
     思路:
     if(雙方都攻擊){
        撥放A攻擊動畫
        if(B閃)
            撥放B閃動畫
        else
            撥放B被擊中動畫
        
        (等待上述動畫撥放完畢)
        (換B再次執行以上條件then結束)
     }
     else if(A攻擊B其他){
        撥放A攻擊動畫
        if(B防禦)
            撥放B防禦動畫
        else if(B迴避){
            if(B閃)
                撥放B閃動畫
            else
                撥放B被擊中動畫
        }
        else {
            if(B閃)
                撥放B閃動畫
            else
                撥放B被擊中動畫
        }
        (等待上述動畫撥放完畢)
        if(B集氣or發動技能)
            撥放B集氣動畫orB技能發動
     }
     else if(A其他B攻擊)
        同上述AB調換
     else{ //AB都沒攻擊的情況
        分別撥放對應動畫
     }
     */
}
