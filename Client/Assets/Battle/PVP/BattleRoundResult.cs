using System.Collections;

public class BattleRoundResult {
    public bool isPartnerOnfire = false;
    public bool isPartnerNextCritical = false;
    //public bool isPartnerDefenseDropped = false;
    public bool isPartnerSkillActivated = false;
    //為了判定動畫多存這個
    public BattlePhase.Movement partnerMovement;
    public bool isPartnerEvaded = false;
    public int partnerDamageTake;
    public int partnerRemainingCD;
    public int partnerHp;
    public int partnerDefense;
    public string partnerStatusText = "";

    public bool isEnemyOnfire = false;
    public bool isEnemyNextCritical = false;
    //public bool isEnemyDefenseDropped = false;
    public bool isEnemySkillActivated = false;
    public bool isEnemyEvaded = false;
    //為了判定動畫多存這個
    public BattlePhase.Movement enemyMovement;
    public int enemyDamageTake;
    public int enemyRemainingCD;
    public int enemyHp;
    public int enemyDefense;
    public string enemyStatusText = "";
}