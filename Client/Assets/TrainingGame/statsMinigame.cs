using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class statsMinigame : MonoBehaviour {
    //guess it's fine to set variables to public?
    private GameObject[] disks;
    private List<Pauseable> pauseables;
    private const float DISK_INTERVAL = 0.8f;
    private bool run = true;
    public Button PauseButton;
    public GameObject ConfirmPanel;
    private Text PauseButtonText;
    // Use this for initialization
    void Start() {
        PauseButtonText = PauseButton.GetComponentInChildren<Text>();
        //所有disk種類先擺
        disks = new GameObject[] {
            GameObject.Find("staminaStat"),
            GameObject.Find("attackStat"),
            GameObject.Find("defenseStat")
        };
        pauseables = new List<Pauseable>();
        //每0.8秒一個disk
        StartCoroutine(GenerateDisk(DISK_INTERVAL));
    }

    private IEnumerator GenerateDisk(float interval)
    {
        while (true)
        {
            if (run)
            {
                generateRandomColorDisk();
                //間隔interval的時間產生一個disk
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void ExitClicked()
    {
        ConfirmPanel.GetComponent<Panel>().Show();
        run = false;
        foreach (Pauseable obj in pauseables)
        {
            obj.Pause();
        }
    }

    public void ConfirmButtonClicked()
    {
        SceneManager.LoadScene("Status");
    }

    public void NoButtonCliked()
    {
        ConfirmPanel.GetComponent<Panel>().Hide();
        PauseButtonText.text = "暫停";
        run = true;
        foreach (Pauseable obj in pauseables)
        {
            obj.Resume();
        }
    }

    public void PauseClicked()
    {
        run = !run;
        Debug.Log(pauseables.Count);
        if (run)
        {
            PauseButtonText.text = "暫停";
            foreach (Pauseable obj in pauseables)
            {
                obj.Resume();
            }
        }
        else
        {
            PauseButtonText.text = "繼續";
            foreach (Pauseable obj in pauseables)
            {
                obj.Pause();
            }
        }
    }

    public void AddPauseableObject(Pauseable obj)
    {
        pauseables.Add(obj);
    }

    public void RemovePauseableObject(Pauseable obj)
    {
        pauseables.Remove(obj);
    }

    // Update is called once per frame
    void Update() {
    }

    void generateRandomColorDisk()//call functions to generate disks of different colors
    {
        int color = Random.Range(0, disks.Length);//currently only three colors
        Instantiate(disks[color], new Vector3(generateRandomTrackPosition(), 5.5f, 0), Quaternion.identity);
    }

    float generateRandomTrackPosition()//create position of random tracks
    {
        int track = Random.Range(1,6);
        switch (track)
        {
            case 1:
                return -2.36f;
            case 2:
                return -1.18f;
            case 3:
                return 0f;
            case 4:
                return 1.18f;
            case 5:
                return 2.36f;
            default:
                return 0f;//for safty
        }
    }
}
