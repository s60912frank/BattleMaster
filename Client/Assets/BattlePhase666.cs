using UnityEngine;
using System.Collections;

public class BattlePhase666{
    /*private MonsterData2 enemyData;
    private MonsterData2 userData;
    public enum Movement
    {
        Attack,
        Defense,
        Evade,
        Charge,
        Skill
    }

    public BattlePhase666(JSONObject user, JSONObject enemy)
    {
        userData = new MonsterData2(user);
        enemyData = new MonsterData2(enemy);
    }

    public void RoundStart(Movement enemyMovement, Movement userMovement)
    {
        int enemyEvadeNum = Random.Range(0, 100);
        int userEvadeNum = Random.Range(0, 100);
        //降防情況
        if(IsInDefenseOrEvade(enemyMovement) && IsInDefenseOrEvade(userMovement))
        {
            if (enemyMovement == Movement.Defense)
                enemyData.DropDefense();
            if (userMovement == Movement.Defense)
                userData.DropDefense();
        }
        //A防禦B攻擊的情況
        else if (enemyMovement == Movement.Defense && userMovement == Movement.Attack)
        {
            ADefenseBAttack(enemyData, userData);
        }
        else if (userMovement == Movement.Defense && enemyMovement == Movement.Attack)
        {
            ADefenseBAttack(userData, userData);
        }
        //A迴避B攻擊的情況
        else if (enemyMovement == Movement.Evade && userMovement == Movement.Attack)
        {
            AEvadeBAttack(enemyData, userData);
        }
        else if (userMovement == Movement.Evade && enemyMovement == Movement.Attack)
        {
            AEvadeBAttack(userData, userData);
        }
        //A充能B防禦的情況
        else if(enemyMovement == Movement.Charge && userMovement == Movement.Defense)
        {
            AChargeBDefend(enemyData, userData);
        }
        else if (userMovement == Movement.Charge && enemyMovement == Movement.Defense)
        {
            AChargeBDefend(userData, enemyData);
        }
        //A充能B迴避的情況
        else if(userMovement == Movement.Charge && enemyMovement == Movement.Evade)
        {
            userData.Charge();
        }
        else if(enemyMovement == Movement.Charge && userMovement == Movement.Evade)
        {
            enemyData.Charge();
        }
        //雙方都充能的情況
        else if (enemyMovement == Movement.Charge && userMovement == Movement.Charge)
        {
            userData.Charge();
            enemyData.Charge();
        }
        //A充能B攻擊的情況
        else if (userMovement == Movement.Charge && enemyMovement == Movement.Attack)
        {
            userData.Charge();
            //迴避失敗的情況
            if (!(userData._evade > userEvadeNum))
                userData.TakeDamage(enemyData.Attack - userData._defense);
        }
        else if (enemyMovement == Movement.Charge && userMovement == Movement.Attack)
        {
            enemyData.Charge();
            //迴避失敗的情況
            if (!(enemyData._evade > enemyEvadeNum))
                enemyData.TakeDamage(userData.Attack - enemyData._defense);
        }
        //有一方發動技能的情況
        else if (enemyMovement == Movement.Skill || userMovement == Movement.Skill)
        {

        }
    }

    private bool IsInDefenseOrEvade(Movement move)
        //這動作是防禦或迴避?
    {
        return (move == Movement.Evade) || (move == Movement.Defense);
    }

    private void ADefenseBAttack(MonsterData2 A, MonsterData2 B)
    {
        A.Charge();
        A.TakeDamage(B.Attack - A.DefensingDefend);
        A.RecoverDefense();
    }

    private bool AEvadeBAttack(MonsterData2 A, MonsterData2 B)
    {
        int evadeNum = Random.Range(0, 100);
        if(A.EvadingEvade > evadeNum)
        {
            //迴避成功
            A.NextCricical();
            return true;
        }
        else
        {
            //迴避失敗
            A.TakeDamage(B.Attack - A._defense);
            return false;
        }
    }

    private void AChargeBDefend(MonsterData2 A, MonsterData2 B)
    {
        A.Charge();
        B.DropDefense();
    }*/
}
