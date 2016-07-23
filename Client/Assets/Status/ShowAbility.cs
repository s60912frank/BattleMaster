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

        StaminaText.GetComponent<Text>().text = STAMINA + petData["stamina"].str;
        AttackText.GetComponent<Text>().text = ATTACK + petData["attack"].str;
        DefenseText.GetComponent<Text>().text = DEFENSE + petData["defense"].str;
        EvadeText.GetComponent<Text>().text = EVADE + petData["evade"].str;
        SkillText.GetComponent<Text>().text = SKILL + petData["skill"]["SkillDesc"].str;
        SkillCDText.GetComponent<Text>().text = SKILLCD + petData["skill"]["CD"].str;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
