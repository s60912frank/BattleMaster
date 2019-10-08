using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GPS {
    private float latOrigin;
    private float lonOrigin;
    public string GPSStatus = "GPS OK";

    private Vector2 Location
    {
        get
        {
            //return new Vector2(lonOrigin, latOrigin);
            return new Vector2(121.533695f, 25.044093f);
        }
    }

    public IEnumerator GPSInit(System.Action<Vector2> location)
    {
        // First, check if user has location service enabled
        /*if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS沒開");
            GPSStatus = "請開啟GPS";
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
            GPSStatus = "GPS逾時，請稍後再試";
            Input.location.Stop();
            location(Location);
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS錯誤");
            GPSStatus = "GPS發生錯誤，請稍後再試";
            Input.location.Stop();
            location(Location);
            yield break;
        }
        else
        {
            Debug.Log("起始OK");
            GPSStatus = "GPS OK";
            //臭爆
            yield return new WaitForSeconds(1);
            lonOrigin = Input.location.lastData.longitude;
            latOrigin = Input.location.lastData.latitude;
            //更新里程
            if (Input.location.lastData.timestamp != 0)
                UpdateMileage(Location);
            location(Location);
            yield break;
        }*/
        location(Location);
        yield break;
    }

    private void UpdateMileage(Vector2 location)
    {
        /*if (PlayerPrefs.HasKey("lastLongitude") && PlayerPrefs.HasKey("lastLatitude"))
        {

            Vector2 lastPos = new Vector2(PlayerPrefs.GetFloat("lastLongitude"), PlayerPrefs.GetFloat("lastLatitude"));
            float distance = HaversineDistance(lastPos, location);
            int mileageGain = Mathf.FloorToInt(distance);
            //先把里程取得存起來 統一到主畫面再向server更新
            if (PlayerPrefs.HasKey("MileageGainToUpdate"))
            {
                float oldGain = PlayerPrefs.GetFloat("MileageGainToUpdate");
                PlayerPrefs.SetFloat("MileageGainToUpdate", oldGain + mileageGain);
            }
            else
            {
                PlayerPrefs.SetFloat("MileageGainToUpdate", mileageGain);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("MileageGainToUpdate", 0);
        }
        PlayerPrefs.SetFloat("lastLongitude", location.x);
        PlayerPrefs.SetFloat("lastLatitude", location.y);*/
        PlayerPrefs.SetFloat("MileageGainToUpdate", 150);
    }

    public void StopGPS()
    {
        /*if(Input.location.lastData.timestamp != 0)
            UpdateMileage(new Vector2(Input.location.lastData.longitude, Input.location.lastData.latitude));
        Input.location.Stop();*/
    }

    public Vector2 PlayerLocation
    {
        get
        {
            return Vector2.zero;
        }
    }

    public float HaversineDistance(Vector2 pos1, Vector2 pos2)
    {
        float R = 6371;
        float lat = (pos2.y - pos1.y) * Mathf.Deg2Rad;
        float lng = (pos2.x - pos1.x) * Mathf.Deg2Rad;
        float h1 = Mathf.Sin(lat / 2) * Mathf.Sin(lat / 2) +
                      Mathf.Cos(pos1.y * Mathf.Deg2Rad) * Mathf.Cos(pos2.y * Mathf.Deg2Rad) *
                      Mathf.Sin(lng / 2) * Mathf.Sin(lng / 2);
        float h2 = 2 * Mathf.Atan2(Mathf.Sqrt(h1), Mathf.Sqrt(1 - h1));
        return R * h2 * 1000;
    }
}
