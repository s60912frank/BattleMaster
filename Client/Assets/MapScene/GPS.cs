using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPS {
    private int zoom = 15; //放大倍率，1~19
    private float latOrigin = 25.0417534f;
    private float lonOrigin = 121.5339142f;
    //public Text gpsStatus;
    public string GPSStatus = "";

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
            GPSStatus = "GPS沒開";
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
            GPSStatus = "GPS逾時";
            Input.location.Stop();
            location(Location);
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS錯誤");
            GPSStatus = "GPS錯誤";
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

    public Vector2 PlayerLocation
    {
        get
        {
            if(Input.location.status == LocationServiceStatus.Running)
            {
                const float times = 3276.8f;
                float newX = (Input.location.lastData.longitude - lonOrigin) * times;
                float newY = (Input.location.lastData.latitude - latOrigin) * times;
                GPSStatus = "longitude:" + Input.location.lastData.longitude + "    latitude:" + Input.location.lastData.latitude;
                return new Vector2(newX, newY);
            }
            else
            {
                //Input.location.lastData.
                return new Vector2(lonOrigin, latOrigin);
            }
        }
    }
}
