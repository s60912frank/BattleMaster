using UnityEngine;
using System.Collections;

public class MonsterData2 {
    public int _stamina;
    private int _charge;
    public int _skillCD;
    private int _attack;
    public int _defense;
    public int _initialDefense;
    public int _evade;
    private bool _nextCritical;
    public int _burnDamage;
    public int SkillDamage;
    public int SkillRecover;
    public int SkillAttIncrease;
    public int SkillBurn;
    public string _textSkillDescription;

    public void NextCricical()
    {
        _nextCritical = true;
    }

    public int Attack
    {
        get
        {
            if (_nextCritical)
            {
                _nextCritical = false;
                return _attack * 2;
            }
            else
            {
                return _attack;
            }
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
        Debug.Log("Data:" + data.ToString());
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
        //int damage = int.Parse(GetString(_skillParams, "damage"));
        //int recover = int.Parse(GetString(_skillParams, "recover"));
        //int attIncrease = int.Parse(GetString(_skillParams, "attIncrease"));
        enemy._burnDamage = SkillBurn;
        _stamina += SkillRecover;
        enemy._stamina -= SkillDamage;
        _attack += SkillAttIncrease;
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
            return _charge >= _skillCD;
        }
    }
}
