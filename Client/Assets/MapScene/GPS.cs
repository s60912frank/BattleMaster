using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPS : MonoBehaviour {
    private int zoom = 15; //放大倍率，1~19
    private MapProcessor mp;
    public Text gpsStatus;
    public GameObject Player;
    // Use this for initialization
    void Start () {
        mp = GameObject.Find("Map").GetComponent<MapProcessor>();
        //開始開啟GPS
        StartCoroutine(GPSInit());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator GPSInit()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS沒開");
            gpsStatus.text = "GPS沒開";
            mp.requestMap(MapProcessor.lonOrigin, MapProcessor.latOrigin);
            //mapTiles.Add(new MapTile(lonOrigin, latOrigin, zoom));
            //xTile = mapTiles[mapTileIndex].xTile; //起始地圖格
            //yTile = mapTiles[mapTileIndex].yTile;
            //requestMap(xTile, yTile); //要第一塊地圖格

            //loading End
            //loadingPanel.GetComponent<LoadingScript>().EndLoading();
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("GPS逾時");
            gpsStatus.text = "GPS逾時";
            Input.location.Stop();
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS錯誤");
            gpsStatus.text = "GPS錯誤";
            Input.location.Stop();
            yield break;
        }
        else
        {
            Debug.Log("起始OK");
            MapProcessor.lonOrigin = Input.location.lastData.longitude;
            MapProcessor.latOrigin = Input.location.lastData.latitude;
            gpsStatus.text = "longitude:" + MapProcessor.lonOrigin + "    latitude:" + MapProcessor.latOrigin;
            mp.requestMap(MapProcessor.lonOrigin, MapProcessor.latOrigin);
            //loading End
            //loadingPanel.GetComponent<LoadingScript>().EndLoading();
            yield return UpdateLocation();
        }
    }

    private IEnumerator UpdateLocation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            gpsStatus.text = "longitude:" + Input.location.lastData.longitude + "    latitude:" + Input.location.lastData.latitude;
            const float times = 3276.8f;
            float newX = (Input.location.lastData.longitude - MapProcessor.lonOrigin) * times;
            float newY = (Input.location.lastData.latitude - MapProcessor.latOrigin) * times;
            //PlayerLocation.Set(newX, newY);
            Player.transform.position = new Vector3(newX, newY, Player.transform.position.z);
        }
    }
}
