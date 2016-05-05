using System.Collections;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
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
	// Use this for initialization
	void Start() 
    {
        _stamina = 50;
        _charge = 0;
        _skillCD = 3;
        _attack = 12;
        _defense = 2;
        _evade = 30;//percent 測試用才用這麼高
	}
}
