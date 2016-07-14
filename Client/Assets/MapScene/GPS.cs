using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPS {
    private int zoom = 15; //放大倍率，1~19
    public static float latOrigin = 25.0417534f;
    public static float lonOrigin = 121.5339142f;
    //public Text gpsStatus;
    public GameObject Player;

    private Vector2 Location
    {
        get
        {
            return new Vector2(lonOrigin, latOrigin);
        }
    }

    public IEnumerator GPSInit(System.Action<Vector2> location)
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS沒開");
            location(Location);
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
            //gpsStatus.text = "GPS逾時";
            Input.location.Stop();
            location(Location);
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS錯誤");
            //gpsStatus.text = "GPS錯誤";
            Input.location.Stop();
            location(Location);
            yield break;
        }
        else
        {
            Debug.Log("起始OK");
            lonOrigin = Input.location.lastData.longitude;
            latOrigin = Input.location.lastData.latitude;
            //gpsStatus.text = "longitude:" + MapProcessor.lonOrigin + "    latitude:" + MapProcessor.latOrigin;
            //mp.requestMap(MapProcessor.lonOrigin, MapProcessor.latOrigin);
            //loading End
            //loadingPanel.GetComponent<LoadingScript>().EndLoading();
            //yield return UpdateLocation();
            location(Location);
            yield break;
        }
    }

    public void UpdateLocation()
    {
        //yield return new WaitForSeconds(0.5f);
        //gpsStatus.text = "longitude:" + Input.location.lastData.longitude + "    latitude:" + Input.location.lastData.latitude;
        const float times = 3276.8f;
        float newX = (Input.location.lastData.longitude - lonOrigin) * times;
        float newY = (Input.location.lastData.latitude - latOrigin) * times;
        //PlayerLocation.Set(newX, newY);
        Player.transform.position = new Vector3(newX, newY, Player.transform.position.z);
    }
}
