using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClickPVP : MonoBehaviour {
    public GameObject BattleManager;
    private BattlePhasePVP battlePhase;
    public Text btnSkillText;
    public Text messageBoxText;
    private int chargeMax;
    private int charge;

	private Button[] buttons;
	// Use this for initialization
	void Start ()
    {
        battlePhase = BattleManager.GetComponent<BattlePhasePVP>();
		btnSkillText.text = battlePhase.partner.ChargeText;
    }
	
	// Update is called once per frame
	void Update () {
		btnSkillText.text = battlePhase.partner.ChargeText;
	}

    public void action_4_btnSkill()
    {
		if (battlePhase.partner.IsSkillReady) 
		{
			battlePhase.partnerMovement = BattlePhasePVP.Movement.Skill;
			messageBoxText.text = "Confirm to Activate Skill.";
		}
		else
		{
			battlePhase.partnerMovement = BattlePhasePVP.Movement.Charge;
			messageBoxText.text = "Confirm to Activate Charge.";
		}
    }

    public void action_4_btnAttack()
    {
		battlePhase.partnerMovement = BattlePhasePVP.Movement.Attack;
        messageBoxText.text = "Confirm to Activate Attack.";
    }

    public void action_4_btnDefend()
    {
		battlePhase.partnerMovement = BattlePhasePVP.Movement.Defense;
        messageBoxText.text = "Confirm to Activate Defend.";
    }

    public void action_4_btnEvade()
    {
		battlePhase.partnerMovement = BattlePhasePVP.Movement.Evade;
        messageBoxText.text = "Confirm to Activate Evade.";
    }

	public void LeaveBattleClicked()
	{
		SceneManager.LoadScene("Status");
	}
}
