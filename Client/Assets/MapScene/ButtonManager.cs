using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void GoBackScene()
    {
        SceneManager.LoadScene("Status");
    }
}
