using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}

    public void GoBackScene()
    {
        GameObject.Find("Map").GetComponent<MapView>().StopGps();
        SceneManager.LoadScene("Status");
    }
}
