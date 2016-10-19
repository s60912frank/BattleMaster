using UnityEngine;
using System.Collections;

public class MonsterData2 {
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
    }
    private int _stamina;
    public int Stamina
    {
        get
        {
            return _stamina;
        }
    }
    private int _charge;
    public int _skillCD;
    private int _attack;
    public int Attack
    {
        get
        {
            return _attack;
        }
    }
    private int _defense;
    public int Defense
    {
        get
        {
            return _defense;
        }
    }
    public int _initialDefense;
    private int _evade;
    public int Evade
    {
        get
        {
            return _evade;
        }
    }
    private bool _nextCritical;
    public int _burnDamage;
    public int SkillDamage;
    public int SkillRecover;
    public int SkillAttIncrease;
    public int SkillBurn;
    private string _textSkillDescription;
    public string SkillDescription
    {
        get
        {
            return _textSkillDescription;
        }
    }

    public void SetNextCricical(bool status)
    {
        _nextCritical = status;
    }

    public bool IsNextCritical
    {
        get
        {
            return _nextCritical;
        }
    }

    public int NowCharge
    {
        get
        {
            return _charge;
        }
    }

    public int DefensingDefend
    {
        get
        {
            return _defense * 2;
        }
    }

    public int EvadingEvade
    {
        get
        {
            return _evade * 2;
        }
    }

    public MonsterData2(JSONObject data)
    {
        //Debug.Log("Data:" + data.ToString());
        _name = data["name"].str;
        _stamina = (int)data["stamina"].f;
        _attack = (int)data["attack"].f;
        _defense = (int)data["defense"].f;
        _initialDefense = _defense;
        _evade = (int)data["evade"].f;
        SkillDamage = (int)data["skill"]["params"]["damage"].f;
        SkillRecover = (int)data["skill"]["params"]["recover"].f;
        SkillBurn = (int)data["skill"]["params"]["burn"].f;
        SkillAttIncrease = (int)(int)data["skill"]["params"]["attIncrease"].f;
        _textSkillDescription = data["skill"]["SkillDesc"].str;
        _charge = 0;
        _skillCD = (int)data["skill"]["CD"].f;
        _nextCritical = false;
    }

    public void Skill(ref MonsterData2 enemy)
    {
        enemy._burnDamage = SkillBurn;
        _stamina += SkillRecover;
        enemy._stamina -= SkillDamage;
        _attack += SkillAttIncrease;
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

    public void TakeDamage(int damage)
    {
        _stamina -= damage;
    }

    public void Burn()
    {
        if (_burnDamage > 0)
        {
            _stamina -= _burnDamage;
            Debug.Log("你受到了" + _burnDamage + "點的燃燒傷害!");
        }
    }

    public void Charge()
    {
        _charge++;
        if (_charge > _skillCD)
            _charge = _skillCD;
    }

    public void DropDefense()
    {
        _defense -= Mathf.FloorToInt((float)_initialDefense * 0.5f);
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
            return _charge >= _skillCD;
        }
    }

    public bool IsOnFire
    {
        get
        {
            return (_burnDamage > 0);
        }
    }

    public bool IsDenfenseDropped
    {
        get
        {
            return _initialDefense != _defense;
        }
    }
}
