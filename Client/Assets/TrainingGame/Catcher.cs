using UnityEngine;
using System.Collections;

public class Catcher : Pauseable {
    // Use this for initialization
    private UpdateScore updateScore;
    private bool run = true;
    private float deltaTime = 0.0f;
    void Start () {
        updateScore = GameObject.Find("Canvas").GetComponent<UpdateScore>();
        GameObject.Find("GameScript").GetComponent<statsMinigame>().AddPauseableObject(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (run)
        {
            deltaTime += Time.deltaTime;
            transform.position = new Vector3(Mathf.PingPong(deltaTime * 1.5f, 4) - 2, transform.position.y, transform.position.z);
        }
    }

    public override void Pause()
    {
        run = false;
    }

    public override void Resume()
    {
        run = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Disk")
        {
            if(col.GetComponent<diskMovment>().forceGet)
            {
                if (col.name == "staminaStat(Clone)")
                    updateScore.UpdateStaminaScore();
                else if (col.name == "attackStat(Clone)")
                    updateScore.UpdateAttackIncrease();
                else if (col.name == "defenseStat(Clone)")
                    updateScore.UpdateDefenseScore();
                Destroy(col.gameObject);
            }
        }
    }
}
