using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattlePhase : MonoBehaviour {

    public GameObject Partner;
    public GameObject Enemy;
    public GameObject PartnerSkillEffect;
    public Text EnemyHP;
    public Text PartnerHP;
    public Text messageBoxText;
    public Text messageEnemyMove;
    public Text EnemyCrit;
    public Text PartnerCrit;

    public EnemyData enemyData;
    public PartnerData partnerData;

    private string _yourNextMove = "Attack";
    public string yourNextMove
    {
        get
        {
            return _yourNextMove;
        }
        set
        {
            _yourNextMove = value;
        }
    }

    private int evadeNumber;//to present rate of evasiveness when Partner's atk lands
    private int enemyDefendActivated = 0;
    private int enemyEvadeActivated = 0;
    private int enemyAttackActivated = 0;
    private int enemyCritActivated = 0;
    private int enemysRandomMove;//temp

    private int yourEvadeNumber;//to present rate of evasiveness when Enemy's atk lands
    private int partnerDefendActivated = 0;
    private int partnerEvadeActivated = 0;
    private int partnerCritActivated = 0;

	// Use this for initialization
	void Start () {
        //set status from scripts
        enemyData = Object.Instantiate(Enemy.GetComponent<EnemyData>());
        partnerData = Object.Instantiate(Partner.GetComponent<PartnerData>());
        EnemyHP.text = "Enemy HP:" + enemyData.stamina;
        PartnerHP.text = "Partner HP:" + partnerData.stamina;
	}
	
	// Update is called once per frame
	void Update ()//Show and hide critical hit hint
    {
        if (enemyCritActivated == 1){
            EnemyCrit.text = "Next hit Critical";
        }
        else
        {
            EnemyCrit.text = "";
        }
        if (partnerCritActivated == 1){
            PartnerCrit.text = "Next hit Critical";
        }
        else
        {
            PartnerCrit.text = "";
        }
        EnemyHP.text = "Enemy HP:" + enemyData.stamina;
        PartnerHP.text = "Partner HP:" + partnerData.stamina;
	}

    public void enterBattlePhase()//Press Confirm button to enter battle phase
    {
        //enemy won't charge or launch its skill at this point.
        enemysRandomMove = Random.Range(0, 3);
        switch (enemysRandomMove)
        {
            case 0:
                enemyDefendActivated = 1;
                messageEnemyMove.text = "The enemy tried to defend your attack.";
                break;
            case 1:
                enemyEvadeActivated = 1;
                messageEnemyMove.text = "The enemy tried to evade your attack.";
                break;
            case 2:
                enemyAttackActivated = 1;//see Enemy's attack below
                messageEnemyMove.text = "The enemy attacked!";
                yourEvadeNumber = Random.Range(0, 100);
                break;
            //case 3:
            //    enemyChargeActivated = 1;
            //    break;
        }

        //you can't charge or launch skill at this point.
        switch (_yourNextMove)
        {
            case "Attack":
                evadeNumber = Random.Range(0, 100);
                Debug.Log(evadeNumber);
                //迴避的話迴避加倍
                if (enemyData.evade * (1 + enemyEvadeActivated) < evadeNumber)//update messageBox text
                {
                    int damage = partnerData.attack * (1 + partnerCritActivated) - enemyData.defense * (1 + enemyDefendActivated);
                    enemyData.stamina = enemyData.stamina - damage;
                    messageBoxText.text = "Attack hit and dealt " + damage + " damage.";
                    //Enemy HP update
                    EnemyHP.text = "Enemy HP:" + enemyData.stamina;
                }
                else
                {
                    messageBoxText.text = "The Enemy evaded your attack...";
                    if (enemyEvadeActivated == 1)
                        enemyCritActivated = 1;
                }
                partnerCritActivated = 0;
                break;
            case "Defend":
                partnerDefendActivated = 1;
                break;
            case "Evade":
                partnerEvadeActivated = 1;
                break;
            case "Charge":
                partnerData.charge++;
                break;
            case "Skill":
                partnerData.charge = 0;
                PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
                Partner.GetComponent<PartnerData>().Skill();//SOMETHING IS WRONG
                Debug.Log("hey! over here.");
                break;
        }
        
        //Enemy's attack 為方便移到這邊分開寫
        if (enemyAttackActivated == 1)
        {
            if (partnerData.evade * (1 + partnerEvadeActivated) < yourEvadeNumber)
            {
                int damage = enemyData.attack * (1 + enemyCritActivated) - partnerData.defense * (1 + partnerDefendActivated);
                partnerData.stamina = partnerData.stamina - damage;
                messageBoxText.text = "You took " + damage + " damage";
                PartnerHP.text = "Partner HP:" + partnerData.stamina;
                if (partnerDefendActivated == 1)
                {
                    partnerData.charge++;//Defend success --> Skill boost
                    
                }
            }
            else
            {
                //Attack dodged
                messageBoxText.text = "You dodged the enemy's attack.";
                if (partnerEvadeActivated == 1)
                    partnerCritActivated = 1;
            }
            enemyCritActivated = 0;
        }
        
        //set back variables
        partnerDefendActivated = 0;
        partnerEvadeActivated = 0;

        enemyDefendActivated = 0;
        enemyEvadeActivated = 0;
        enemyAttackActivated = 0;
    }
}
