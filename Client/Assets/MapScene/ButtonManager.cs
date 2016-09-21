using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour {

	// Use this for initialization
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
}
