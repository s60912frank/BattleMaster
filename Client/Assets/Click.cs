using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Click : MonoBehaviour {
	public GameObject BattleManager;
	private BattlePhase battlePhase;
	public Text btnSkillText;
	public Text messageBoxText;
	private int chargeMax;
	private int charge;
	// Use this for initialization
	void Start ()
	{
		battlePhase = BattleManager.GetComponent<BattlePhase>();
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
			battlePhase.partnerMovement = BattlePhase.Movement.Skill;
			messageBoxText.text = "Confirm to Activate Skill.";
		}
		else
		{
			battlePhase.partnerMovement = BattlePhase.Movement.Charge;
			messageBoxText.text = "Confirm to Activate Charge.";
		}
	}

	public void action_4_btnAttack()
	{
		battlePhase.partnerMovement = BattlePhase.Movement.Attack;
		messageBoxText.text = "Confirm to Activate Attack.";
	}

	public void action_4_btnDefend()
	{
		battlePhase.partnerMovement = BattlePhase.Movement.Defense;
		messageBoxText.text = "Confirm to Activate Defend.";
	}

	public void action_4_btnEvade()
	{
		battlePhase.partnerMovement = BattlePhase.Movement.Evade;
		messageBoxText.text = "Confirm to Activate Evade.";
	}
}
