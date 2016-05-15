using UnityEngine;
using System.Collections;

public class MonsterData{
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
	private int _initialDefense;
	public int InitialDefense
	{
		get
		{
			return _initialDefense;
		}
		set
		{
			_initialDefense = value;
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
	private bool _nextCritical;
	public bool NextCritical
	{
		get
		{
			return _nextCritical;
		}
		set
		{
			_nextCritical = value;
		}
	}
	private int _burnDamage;
	public int BurnDamage
	{
		get
		{
			return _burnDamage;
		}
		set
		{
			_burnDamage = value;
		}
	}
    private string _textSkillDescription;
    public string textSkillDescription
    {
        get
        {
            return _textSkillDescription;
        }
    }
	private JSONObject _skillParams;
	public JSONObject SkillParams
	{
		get
		{
			return _skillParams;
		}
		set
		{
			_skillParams = value;
		}
	}
    // Use this for initialization
    public void Start()
    {
        _stamina = 50;
        _charge = 0;
        _skillCD = 3;
        _attack = 12;
        _defense = 2;
        _evade = 30;//percent
		_nextCritical = false;
        _textSkillDescription = "Drain 15 Hp from Enemy and boost 5 attack";
    }

	public void Skill(ref MonsterData enemy)
    {
		int damage = int.Parse(GetString(_skillParams, "damage"));
		int recover = int.Parse(GetString(_skillParams, "recover"));
		int attIncrease = int.Parse(GetString(_skillParams,"attIncrease"));
		enemy.BurnDamage = int.Parse(GetString(_skillParams, "burn"));
		_stamina += recover;
		enemy.stamina -= damage;
		_attack += attIncrease;
		//技能特效還要再想辦法
		//PartnerSkillEffect.GetComponent<PartnerSkillEffectEntry>().activated = true;
		_charge = 0;
		Debug.Log("Your skill activated!");
    }

	public int RemainingCD
	{
		get
		{ 
			return _skillCD - _charge;
		}
	}

	public void Initialize(JSONObject data)
	{
		Debug.Log("PartnerData:" + data.ToString());
		_stamina = int.Parse(GetString(data, "stamina"));
		Debug.Log ("WHEEEEEEE:" + data["stamina"].ToString());
		_attack = int.Parse(GetString(data, "attack"));
		_defense = int.Parse(GetString(data, "defense"));
		_initialDefense = _defense;
		_evade = int.Parse(GetString(data, "evade"));
		_skillParams = data["skill"]["params"];
		_textSkillDescription = data["skill"]["SkillDesc"].ToString().Replace("\"", "");
		_charge = 0;
		_skillCD = int.Parse(data["skill"]["CD"].ToString().Replace("\"", ""));
		_nextCritical = false;
	}

	public void TakeDamage(int damage)
	{
		_stamina -= damage;
	}

	public void Burn()
	{
		if (_burnDamage > 0) 
		{
			_stamina -= _burnDamage;
			Debug.Log ("你受到了" + _burnDamage + "點的燃燒傷害!");
		}
	}

	public void DropDefense()
	{
		_defense -= 5;
		if (_defense <= 0)
			_defense = 0;
	}

	public void RecoverDefense()
	{
		_defense = _initialDefense;
	}

	public string ChargeText
	{
		get
		{ 
			if (_charge < _skillCD)
				return "Charge:" + _charge.ToString() + "/" + _skillCD.ToString();
			else
				return "Skill";
		}
	}

	public bool IsSkillReady
	{
		get
		{
			return (_charge < _skillCD) ? false : true;
		}
	}

	private string GetString(JSONObject data, string property)
	{
		return data[property].ToString().Replace("\"", "");
	}
}