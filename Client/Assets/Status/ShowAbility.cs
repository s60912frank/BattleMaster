using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowAbility : MonoBehaviour {
    public Text StaminaText;
    public Text AttackText;
    public Text DefenseText;
    public Text EvadeText;
    public Text SkillText;
    public Text SkillCDText;
    public GameObject PetIcon;
    private const float ICON_MAX_HEIGHT = 600;

    // Use this for initialization
    void Start ()
    { 
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        JSONObject petData = data["pet"];
        SetPartnerIcon(petData["name"].str);
        Debug.Log(petData);

        
        StaminaText.text = string.Format("血量:{0}", petData["stamina"].f);
        AttackText.text = string.Format("攻擊:{0}", petData["attack"].f);
        DefenseText.text = string.Format("防禦:{0}", petData["defense"].f);
        EvadeText.text = string.Format("迴避:{0}", petData["evade"].f);
        SkillText.text = string.Format("技能:{0}", petData["skill"]["SkillDesc"].str);
        SkillCDText.text = string.Format("技能CD:{0}", petData["skill"]["CD"].f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SetPartnerIcon(string name)
    {
        Sprite partnerSprite = Resources.Load<Sprite>(name);
        float ratio = (float)partnerSprite.texture.width / (float)partnerSprite.texture.height;
        float width = ICON_MAX_HEIGHT * ratio;
        PetIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(name);
        PetIcon.GetComponent<Image>().SetNativeSize();
        PetIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(width, ICON_MAX_HEIGHT);
    }
}
