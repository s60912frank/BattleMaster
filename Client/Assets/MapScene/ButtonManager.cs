using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour {
    public GameObject NPanel;
	private NotifyPanel notifyScript;

    void Awake(){
        notifyScript = NPanel.GetComponent<NotifyPanel>();
    }

	void Start () {
        
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //使用者按了返回鍵
            GoBackScene();
        }
    }

    public void GoBackScene()
    {
        GameObject.Find("Map").GetComponent<MapView>().StopGps();
        SceneManager.LoadScene("Status");
    }

    public void ShowExplain(){
        notifyScript.Show();
    }
}
