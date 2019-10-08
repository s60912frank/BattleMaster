using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetRoomButton : MonoBehaviour {
    private Text RoomNameText;
    private Text OwnerText;
    void Awake()
    {
        RoomNameText = transform.Find("RoomNameText").GetComponent<Text>();
        OwnerText = transform.Find("OwnerText").GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetRoomName(string name)
    {
        RoomNameText.text = name;
    }

    public void SetRoomOwner(string uid, string name)
    {
        OwnerText.text = "(" + uid + ")" + name;
    }
}
