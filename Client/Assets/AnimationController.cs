using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
    private SpriteController partner;
    private SpriteController enemy;
    public GameObject PartnerSkillEffect;
    public GameObject EnemySkillEffect;
    private SpriteController partnerSkill;
    private SpriteController enemySkill;
    // Use this for initialization
    void Start () {
        partnerSkill = PartnerSkillEffect.GetComponent<SpriteController>();
        enemySkill = EnemySkillEffect.GetComponent<SpriteController>(); ;
        partner = transform.FindChild("PartnerParent").FindChild("Partner").GetComponent<SpriteController>();
        enemy = transform.FindChild("EnemyParent").FindChild("Enemy").GetComponent<SpriteController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator BattleAnimation(BattleRoundResult result)
    {
        if(result.enemyMovement == BattlePhase.Movement.Attack && result.partnerMovement == BattlePhase.Movement.Attack)
        {
            partner.SetTrigger("PartnerStartAttack");
            if (result.isEnemyEvaded)
                enemy.SetTrigger("EnemyStartEvade");
            else
                enemy.SetTrigger("BeingAttack");
            //等待直到結束
            yield return partner.WaitForFinish();
            yield return enemy.WaitForFinish();
            //攻擊後就一定沒有爆擊
            //partner.SetTrigger("SetNotCritical");

            enemy.SetTrigger("EnemyStartAttack");
            if (result.isPartnerEvaded)
                partner.SetTrigger("PartnerStartEvade");
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
                partnerSkill.SetTrigger("ActiveSkill");
                yield return partnerSkill.WaitForFinish();
            }
            //換敵人攻擊
            enemy.SetTrigger("EnemyStartAttack");
            if (result.partnerMovement == BattlePhase.Movement.Defense)
            {
                partner.SetTrigger("StartDefend");
            }
            else if (result.partnerMovement == BattlePhase.Movement.Evade)
            {
                if (result.isPartnerEvaded)
                    partner.SetTrigger("PartnerStartEvade");
                else
                    partner.SetTrigger("BeingAttack");
            }
            else
            {
                if (result.isPartnerEvaded)
                    partner.SetTrigger("PartnerStartEvade");
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
            partner.SetTrigger("PartnerStartAttack");
            if (result.enemyMovement == BattlePhase.Movement.Defense)
            {
                enemy.SetTrigger("StartDefend");
            }
            else if (result.enemyMovement == BattlePhase.Movement.Evade)
            {
                if (result.isEnemyEvaded)
                    enemy.SetTrigger("EnemyStartEvade");
                else
                    enemy.SetTrigger("BeingAttack");
            }
            else
            {
                if (result.isEnemyEvaded)
                    enemy.SetTrigger("EnemyStartEvade");
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
                enemySkill.SetTrigger("ActiveSkill");
                yield return enemySkill.WaitForFinish();
            }
        }
        else
        {
            if (result.enemyMovement == BattlePhase.Movement.Defense)
                enemy.SetTrigger("StartDefend");
            else if (result.enemyMovement == BattlePhase.Movement.Evade)
                enemy.SetTrigger("EnemyStartEvade");
            else if (result.enemyMovement == BattlePhase.Movement.Charge)
                enemy.SetTrigger("StartCharge");
            else if (result.enemyMovement == BattlePhase.Movement.Skill)
            {
                enemySkill.SetTrigger("ActiveSkill");
                enemySkill.WaitForFinish();
            }
            //技能動畫等等補
            else
                Debug.LogError("邏輯錯誤!");

            if (result.partnerMovement == BattlePhase.Movement.Defense)
                partner.SetTrigger("StartDefend");
            else if (result.partnerMovement == BattlePhase.Movement.Evade)
                partner.SetTrigger("PartnerStartEvade");
            else if (result.partnerMovement == BattlePhase.Movement.Charge)
                partner.SetTrigger("StartCharge");
            else if (result.partnerMovement == BattlePhase.Movement.Skill)
            {
                partnerSkill.SetTrigger("ActiveSkill");
                yield return partnerSkill.WaitForFinish();
            }
            //技能動畫等等補
            else
                Debug.LogError("邏輯錯誤!");

            yield return enemy.WaitForFinish();
            yield return partner.WaitForFinish();
        }

        enemy.SetBool("IsNextCritical", result.isEnemyNextCritical);
        partner.SetBool("IsNextCritical", result.isPartnerNextCritical);

        enemy.SetBool("IsBurning", result.isEnemyOnfire);
        partner.SetBool("IsBurning", result.isPartnerOnfire);
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
