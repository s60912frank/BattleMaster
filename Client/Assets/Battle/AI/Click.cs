﻿using UnityEngine;
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

    private Panel exitPanel;
    // Use this for initialization
    void Start ()
	{
        allBtns = new Button[] { SkillBtn, AttackBtn, DefendBtn, EvadeBtn, ConfirmBtn };
        SkillBtnText = SkillBtn.GetComponentInChildren<Text>();
		battleView = GameObject.Find("BattleManager").GetComponent<BattleView>();

        ColorBlock color = AttackBtn.colors;
        color.normalColor = Color.yellow;
        color.highlightedColor = Color.yellow;
        AttackBtn.colors = color;

        exitPanel = GameObject.Find("ConfirmExitPanel").GetComponent<Panel>();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //使用者按了返回鍵
            LeaveBattleClicked();
        }
    }

	public void action_4_btnSkill()
	{
        movement = BattlePhase.Movement.Charge;
        /*if (battleView.IsPartnerSkillReady()) 
		{
			messageBoxText.text = "確認發動";
		}
		else
		{
			messageBoxText.text = "Confirm to Activate Charge.";
		}*/
        SetColor(SkillBtn);
	}

	public void action_4_btnAttack()
	{
        movement = BattlePhase.Movement.Attack;
		//messageBoxText.text = "Confirm to Activate Attack.";
        SetColor(AttackBtn);
	}

	public void action_4_btnDefend()
	{
        movement = BattlePhase.Movement.Defense;
		//messageBoxText.text = "Confirm to Activate Defend.";
        SetColor(DefendBtn);
	}

	public void action_4_btnEvade()
	{
        movement = BattlePhase.Movement.Evade;
		//messageBoxText.text = "Confirm to Activate Evade.";
        SetColor(EvadeBtn);
	}

    public void ConfirmBtnClicked()
    {
        battleView.SetMyMovement(movement);
    }

    private void SetColor(Button current)
    {
        foreach (Button btn in allBtns)
        {
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            btn.colors = cb;
        }
        ColorBlock color = current.colors;
        color.normalColor = Color.yellow;
        color.highlightedColor = Color.yellow;
        current.colors = color;
    }

    public void LeaveBattleClicked()
    {
        exitPanel.SetText("確定要離開嗎?消耗的里程將不會返還。");
        exitPanel.SetConfirmListener(() =>
        {
            SceneManager.LoadScene("Status");
        });
        exitPanel.Show();
    }
}
