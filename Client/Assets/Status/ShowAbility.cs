using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowAbility : MonoBehaviour {
    public GameObject StaminaText;
    public GameObject AttackText;
    public GameObject DefenseText;
    public GameObject EvadeText;
    public GameObject SkillText;
    public GameObject SkillCDText;

    private const string STAMINA = "Stamina:";
    private const string ATTACK = "Attack:";
    private const string DEFENSE = "Defense:";
    private const string EVADE = "Evade:";
    private const string SKILL = "Skill:";
    private const string SKILLCD = "SkillCD:";

    // Use this for initialization
    void Start ()
    {
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        JSONObject petData = data["pet"];
        Debug.Log(petData);

        StaminaText.GetComponent<Text>().text = STAMINA + GetString(petData, "stamina");
        AttackText.GetComponent<Text>().text = ATTACK + GetString(petData, "attack");
        DefenseText.GetComponent<Text>().text = DEFENSE + GetString(petData, "defense");
        EvadeText.GetComponent<Text>().text = EVADE + GetString(petData, "evade");
        SkillText.GetComponent<Text>().text = SKILL + GetString(petData["skill"], "SkillDesc");
        SkillCDText.GetComponent<Text>().text = SKILLCD + GetString(petData["skill"], "CD");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private string GetString(JSONObject data, string property)
    {
        return data[property].ToString().Replace("\"", "");
    }
}
