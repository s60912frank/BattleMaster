using UnityEngine;
using System.Collections;

public class Catcher : MonoBehaviour {
    // Use this for initialization
    private UpdateScore updateScore;
	void Start () {
        updateScore = GameObject.Find("Canvas").GetComponent<UpdateScore>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Mathf.PingPong(Time.time * 1.5f, 4) - 2, transform.position.y, transform.position.z);
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
