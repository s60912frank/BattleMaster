using UnityEngine;
using System.Collections;

public class PartnerData : MonoBehaviour {
    private int _stamina;
    public int stamina
    {
        get
        {
            return _stamina;
        }
        set
        {
            _stamina = value;
        }
    }
    private int _charge;
    public int charge
    {
        get
        {
            return _charge;
        }
        set
        {
            _charge = value;
        }
    }
    private int _skillCD;
    public int skillCD
    {
        get
        {
            return _skillCD;
        }
        set
        {
            _skillCD = value;
        }
    }
    private int _attack;
    public int attack
    {
        get
        {
            return _attack;
        }
        set
        {
            _attack = value;
        }
    }
    private int _defense;
    public int defense
    {
        get
        {
            return _defense;
        }
        set
        {
            _defense = value;
        }
    }
    private int _evade;
    public int evade
    {
        get
        {
            return _evade;
        }
        set
        {
            _evade = value;
        }
    }
    public GameObject BattleManager;
    private string _textSkillDescription;
    public string textSkillDescription
    {
        get
        {
            return _textSkillDescription;
        }
    }
    // Use this for initialization
    void Start()
    {
        stamina = 50;
        _charge = 0;
        _skillCD = 3;
        _attack = 12;
        _defense = 2;
        _evade = 30;//percent
        _textSkillDescription = "Drain 15 Hp from Enemy and boost 5 attack";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Skill()
    {
        BattleManager.GetComponent<BattlePhase>().partnerData.stamina += 15;
        BattleManager.GetComponent<BattlePhase>().enemyData.stamina -= 15;
        BattleManager.GetComponent<BattlePhase>().partnerData.attack += 5;
    }
}