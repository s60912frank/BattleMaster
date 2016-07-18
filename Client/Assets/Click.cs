using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Click : MonoBehaviour {
    public Button SkillBtn;
    public Button AttackBtn;
    public Button DefendBtn;
    public Button EvadeBtn;
    public Button ConfirmBtn;
    private Button[] allBtns;

	private BattleView battleView;
	public Text messageBoxText;
    private Text SkillBtnText;
    private BattlePhase.Movement movement;
	// Use this for initialization
	void Start ()
	{
        allBtns = new Button[] { SkillBtn, AttackBtn, DefendBtn, EvadeBtn, ConfirmBtn };
        SkillBtnText = SkillBtn.GetComponentInChildren<Text>();
		battleView = GameObject.Find("BattleManager").GetComponent<BattleView>();
	}

    public void SetBtnsEnabled(bool status)
    {
        foreach(Button btn in allBtns)
        {
            btn.interactable = status;
        }
    }

	// Update is called once per frame
	void Update () {
        SkillBtnText.text = battleView.GetSkillBtnText();
	}

	public void action_4_btnSkill()
	{
		if (battleView.IsPartnerSkillReady()) 
		{
			movement = BattlePhase.Movement.Skill;
			messageBoxText.text = "Confirm to Activate Skill.";
		}
		else
		{
            movement = BattlePhase.Movement.Charge;
			messageBoxText.text = "Confirm to Activate Charge.";
		}
	}

	public void action_4_btnAttack()
	{
        movement = BattlePhase.Movement.Attack;
		messageBoxText.text = "Confirm to Activate Attack.";
	}

	public void action_4_btnDefend()
	{
        movement = BattlePhase.Movement.Defense;
		messageBoxText.text = "Confirm to Activate Defend.";
	}

	public void action_4_btnEvade()
	{
        movement = BattlePhase.Movement.Evade;
		messageBoxText.text = "Confirm to Activate Evade.";
	}

    public void ConfirmBtnClicked()
    {
        battleView.RoundStart(movement);
    }

	public void LeaveBattleClicked()
	{
		SceneManager.LoadScene("Status");
	}
}
