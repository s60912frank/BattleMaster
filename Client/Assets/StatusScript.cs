using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusScript : MonoBehaviour {
    private RectTransform healthBar;
    private int maxHp;
    private float zeroHealthPos;
    private RectTransform energyBar;
    private float zeroEnergyPos;
    private int maxCD;
    private RectTransform shieldBar;
    private float zeroDefensePos;
    private int maxDefense;
	// Use this for initialization
	void Start () {
        healthBar = transform.FindChild("HealthBar").FindChild("InnerBackground").FindChild("Bar").GetComponent<RectTransform>();
        zeroHealthPos = healthBar.anchoredPosition.x - 0.5f * healthBar.rect.width;
        energyBar = transform.FindChild("EnergyBar").FindChild("InnerBackground").FindChild("Bar").GetComponent<RectTransform>();
        //+是因為一開始是0
        zeroEnergyPos = energyBar.anchoredPosition.x + 0.5f * energyBar.rect.width;
        shieldBar = transform.FindChild("ShieldBar").FindChild("InnerBackground").FindChild("Bar").GetComponent<RectTransform>();
        zeroDefensePos = shieldBar.anchoredPosition.x - 0.5f * shieldBar.rect.width;
    }

    public void SetMax(int maxHp, int maxCD, int maxDefense)
    {
        this.maxHp = maxHp;
        Debug.Log("MAXHP:" + maxHp);
        this.maxCD = maxCD;
        this.maxDefense = maxDefense;
        Debug.Log("MAXDEF:" + maxDefense);
    }

    public IEnumerator UpdateStatus(int currentHp, int currentCD, int currentDefense)
    {
        //blah blah
        float healthTranslateAmount = GetHealthTranslateAmount(currentHp);
        float energyTranslateAmount = GetEnergyTranslateAmount(currentCD);
        float shieldTranslateAmount = GetDefenseTranslateAmount(currentDefense);

        for (int i = 0; i < 30; i++)
        {
            healthBar.anchoredPosition = new Vector2(healthBar.anchoredPosition.x - healthTranslateAmount, healthBar.anchoredPosition.y);
            energyBar.anchoredPosition = new Vector2(energyBar.anchoredPosition.x - energyTranslateAmount, energyBar.anchoredPosition.y);
            shieldBar.anchoredPosition = new Vector2(shieldBar.anchoredPosition.x - shieldTranslateAmount, shieldBar.anchoredPosition.y);
            yield return null;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private float GetHealthTranslateAmount(int health)
    {
        if(health > maxHp)
        {
            maxHp = health;
        }
        float maxWidth = healthBar.rect.width;
        float currentHealthPercent = (float)health / (float)maxHp;
        float xCoord = zeroHealthPos + maxWidth * currentHealthPercent - 0.5f * maxWidth;
        float translateAmount = (healthBar.anchoredPosition.x - xCoord) / 30.0f;
        return translateAmount;
    }

    private float GetEnergyTranslateAmount(int currentCD)
    {
        float maxWidth = energyBar.rect.width;
        float currentEnergyPercent = (float)(maxCD - currentCD) / (float)maxCD;
        Debug.Log("ENERGYPAERSENT:" + currentEnergyPercent);
        float xCoord = zeroEnergyPos + maxWidth * currentEnergyPercent - 0.5f * maxWidth;
        float translateAmount = (energyBar.anchoredPosition.x - xCoord) / 30.0f;
        return translateAmount;
    }

    private float GetDefenseTranslateAmount(int defense)
    {
        Debug.Log("DEFENSE:" + defense);
        float maxWidth = shieldBar.rect.width;
        float currentShieldPercent = (float)defense / (float)maxDefense;
        Debug.Log("DEFENSEPERCENT:" + currentShieldPercent);
        float xCoord = zeroDefensePos + maxWidth * currentShieldPercent - 0.5f * maxWidth;
        float translateAmount = (shieldBar.anchoredPosition.x - xCoord) / 30.0f;
        return translateAmount;
    }
}
