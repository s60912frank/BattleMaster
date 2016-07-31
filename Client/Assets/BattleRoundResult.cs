using System.Collections;

public class BattleRoundResult {
    public bool isPartnerOnfire = false;
    public bool isPartnerNextCritical = false;
    public bool isPartnerDefenseDropped = false;
    public bool isPartnerSkillActivated = false;
    //public ResultType partnerResultType;
    //為了判定動畫多存這個
    public BattlePhase.Movement partnerMovement;
    public bool isPartnerEvaded = false;
    public int partnerDamageTake;
    public int partnerRemainingCD;
    public int partnerHp;
    public string partnerStatusText = "";

    public bool isEnemyOnfire = false;
    public bool isEnemyNextCritical = false;
    public bool isEnemyDefenseDropped = false;
    public bool isEnemySkillActivated = false;
    public bool isEnemyEvaded = false;
    //public ResultType enemyResultType;
    //為了判定動畫多存這個
    public BattlePhase.Movement enemyMovement;
    public int enemyDamageTake;
    public int enemyRemainingCD;
    public int enemyHp;
    public string enemyStatusText = "";
}


/*public enum ResultType
{
    AttackAndBeingAttacked,
    AttackButEvaded,
    SuccessfullyEvaded,
    EvadeButBeingAttacked,
    Defensed,
    SkillChargedWithoutBeingAttacked,
    SkillChargedAndBeingAttacked,
    SkillActivatedWithoutBeingAttacked,
    SkillActivatedAndBeingAttacked
}*/