using UnityEngine;
using System.Collections;

public class statsMinigame : MonoBehaviour {
    //guess it's fine to set variables to public?
    private GameObject[] disks;
    private const float DISK_INTERVAL = 0.8f;
    // Use this for initialization
    void Start() {
        //所有disk種類先擺
        disks = new GameObject[] {
            GameObject.Find("staminaStat"),
            GameObject.Find("attackStat"),
            GameObject.Find("defenseStat")
        };
        //每0.8秒一個disk
        StartCoroutine(GenerateDisk(DISK_INTERVAL));
    }

    private IEnumerator GenerateDisk(float interval)
    {
        while (true)
        {
            generateRandomColorDisk();
            //間隔interval的時間產生一個disk
            yield return new WaitForSeconds(interval);
        }
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
