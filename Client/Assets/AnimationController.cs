using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
    private SpriteController fireball;
    private SpriteController shield;
    private SpriteController partner;
    private SpriteController enemy;
    // Use this for initialization
    void Start () {
        fireball = transform.FindChild("FireBall").GetComponent<SpriteController>();
        shield = transform.FindChild("Shield").GetComponent<SpriteController>();
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
            fireball.SetTrigger("StartPartnerAttack");
            if (result.isEnemyEvaded)
                enemy.SetTrigger("StartEvade");
            else
                enemy.SetTrigger("BeingAttack");
            //等待直到結束
            yield return fireball.WaitForFinish();
            yield return enemy.WaitForFinish();
            fireball.SetTrigger("StartEnemyAttack");
            if(result.isPartnerEvaded)
                partner.SetTrigger("StartEvade");
            else
                partner.SetTrigger("BeingAttack");
            yield return fireball.WaitForFinish();
            yield return partner.WaitForFinish();
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
            fireball.SetTrigger("StartEnemyAttack");
            if (result.partnerMovement == BattlePhase.Movement.Defense)
            {
                shield.SetTrigger("StartPartnerShield");
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
            yield return fireball.WaitForFinish();
            yield return partner.WaitForFinish();
            yield return shield.WaitForFinish();
        }
        else if (result.enemyMovement != BattlePhase.Movement.Attack && result.partnerMovement == BattlePhase.Movement.Attack)
        {
            //我攻擊
            fireball.SetTrigger("StartPartnerAttack");
            if (result.enemyMovement == BattlePhase.Movement.Defense)
            {
                shield.SetTrigger("StartEnemyShield");
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
            yield return fireball.WaitForFinish();
            yield return partner.WaitForFinish();
            yield return shield.WaitForFinish();
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
                shield.SetTrigger("StartEnemyShield");
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
                shield.SetTrigger("StartPartnerShield");
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
            yield return shield.WaitForFinish();
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
