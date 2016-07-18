using System.Collections;

public class BattleRoundResult {
    public bool isPartnerOnfire = false;
    public bool isPartnerNextCritical = false;
    public bool isPartnerDefenseDropped = false;
    public bool isPartnerSkillActivated = false;
    public int partnerDamageTake;
    public int partnerRemainingCD;
    public int partnerHp;
    public string partnerStatusText = "";

    public bool isEnemyOnfire = false;
    public bool isEnemyNextCritical = false;
    public bool isEnemyDefenseDropped = false;
    public bool isEnemySkillActivated = false;
    public int enemyDamageTake;
    public int enemyRemainingCD;
    public int enemyHp;
    public string enemyStatusText = "";
}
