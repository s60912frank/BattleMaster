using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickPVP : MonoBehaviour {
    public GameObject Partner;
    public GameObject BattleManager;
    public BattlePhasePVP battlePhase;
    public Text btnSkillText;
    public Text messageBoxText;
    private int chargeMax;
    private int charge;
	// Use this for initialization
	void Start ()
    {
        battlePhase = BattleManager.GetComponent<BattlePhasePVP>();
        btnSkillText.text = "Charge:" + battlePhase.partnerData.charge + "/" + battlePhase.partnerData.skillCD;
    }
	
	// Update is called once per frame
	void Update () {
        btnSkillText.text = "Charge:" + battlePhase.partnerData.charge + "/" + battlePhase.partnerData.skillCD;
        if (battlePhase.partnerData.charge>= battlePhase.partnerData.skillCD)
        {
            battlePhase.partnerData.charge = battlePhase.partnerData.skillCD;
            btnSkillText.text = "Skill";
        }
	}

    public void action_4_btnSkill()
    {
        if (battlePhase.partnerData.charge < battlePhase.partnerData.skillCD)
			battlePhase.partnerMovement = BattlePhasePVP.Movement.Charge;
        if (battlePhase.partnerData.charge >= battlePhase.partnerData.skillCD)
			battlePhase.partnerMovement = BattlePhasePVP.Movement.Skill;
        if (battlePhase.partnerData.charge == battlePhase.partnerData.skillCD)
            messageBoxText.text = "Charge MAX!";
        messageBoxText.text = "Confirm to Activate Charge.";
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
}
