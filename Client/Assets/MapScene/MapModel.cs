using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapModel {
    private const string API_KEY = "vector-tiles-vxQ7SnN";
    private const string MAP_TYPE = "boundaries,buildings,roads";
    private const string MAPZEN_URL = "https://vector.mapzen.com/osm/";
    private const int ZOOM = 15;
    private const float TIMES = 3276.8f;
    public Rect mapBoundry;
    public Vector2 TileNum;
    public float latOrigin = 25.0417534f;
    public float lonOrigin = 121.5339142f;
    private JSONObject EnemyData;
    private MapProcessor2 process;
    private GPS gps;
	// Use this for initialization
    public MapModel()
    {
        mapBoundry = new Rect(0, 0, 0, 0);
    }

    public IEnumerator MapInit()
    {
        gps = new GPS();
        yield return gps.GPSInit((location) => {
            this.lonOrigin = location.x;
            this.latOrigin = location.y;
        });
        process = new MapProcessor2(lonOrigin, latOrigin);
        yield return RequestMap(lonOrigin, latOrigin);
        yield return GetAllEnemyData();
        yield break;
    }

    private string MapUrl(int x, int y)
    {
        return MAPZEN_URL + MAP_TYPE + "/" + ZOOM + "/" + x + "/" + y + ".json?api_key=" + API_KEY;
    }

    public IEnumerator GetAllEnemyData()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie")); //加入cookie
        WWW w = new WWW(Constant.SERVER_URL + "/allEnemyData", null, headers);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            EnemyData = new JSONObject(w.text);
        }
        else
        {
            Debug.Log(w.error);
        }
    }

    public IEnumerator RequestMap(float lon, float lat)
    {
        Debug.Log(lon + "/" + lat);
        TileNum = WorldToTilePos(lon, lat);
        string filePath = Application.persistentDataPath + "/" + ZOOM.ToString() + "_" + TileNum.x.ToString() + "_" + TileNum.y.ToString() + ".json";
        if (File.Exists(filePath))
        {
            process.JsonProcessor(File.ReadAllText(filePath));
        }
        //沒的話跟伺服器要
        else
        {
            Debug.Log(MapUrl((int)TileNum.x, (int)TileNum.y));
            yield return WaitForRequest((int)TileNum.x, (int)TileNum.y);
        }
    }

    public IEnumerator RequestMap(int xTile, int yTile) //要地圖塊
    {
        Debug.Log(xTile + "/" + yTile);
        TileNum = new Vector2(xTile, yTile);
        string filePath = Application.persistentDataPath + "/" + ZOOM.ToString() + "_" + xTile.ToString() + "_" + yTile.ToString() + ".json";
        //如果之前有存過此地圖塊那就直接讀檔
        if (File.Exists(filePath))
        {
            //JsonProssor(File.ReadAllText(Application.persistentDataPath + "/" + ZOOM.ToString() + "_" + xTile.ToString() + "_" + yTile.ToString() + ".json"));
            process.JsonProcessor(File.ReadAllText(filePath));
        }
        //沒的話跟伺服器要
        else
        {
            Debug.Log(MapUrl(xTile, yTile));
            yield return WaitForRequest(xTile, yTile);
        }
    }

    private IEnumerator WaitForRequest(int xTile, int yTile) //當資料從伺服器回來會執行這個
    {
        WWW w = new WWW(MapUrl(xTile, yTile));
        yield return w;
        // check for errors
        if (w.error == null)
        {
            Debug.Log("WWW Ok!");
            File.WriteAllText(Application.persistentDataPath + "/" + ZOOM.ToString() + "_" + xTile.ToString() + "_" + yTile.ToString() + ".json", w.text);
            process.JsonProcessor(w.text); //有資料就去處理囉
            //return www.text;
        }
        else
        {
            Debug.Log("WWW Error: " + w.error);
        }
    }

    public MapProcessor2 GetMapResult()
    {
        MapProcessor2 temp = process;
        process = null;
        process = new MapProcessor2(lonOrigin, latOrigin);
        return temp;
    }

    public Vector2 WorldToTilePos(float longtitude, float latitude) //經緯度轉地圖格編號
    {
        Vector2 tile = new Vector2();
        tile.x = (int)Mathf.Floor((longtitude + 180.0f) / 360.0f * (1 << ZOOM));
        tile.y = (int)Mathf.Floor(((1.0f - Mathf.Log(Mathf.Tan(latitude * Mathf.PI / 180.0f) + 1.0f / Mathf.Cos(latitude * Mathf.PI / 180.0f)) / Mathf.PI) / 2.0f * (1 << ZOOM)));
        return tile;
    }

    public string[] EnemyNames
    {
        get
        {
            int count = EnemyData.list.Count;
            string[] enemies = new string[count];
            for(int i = 0;i < count; i++)
            {
                enemies[i] = EnemyData.list[i]["name"].str;
            }
            return enemies;
        }
    }

    public JSONObject GetEnemyData(string name)
    {
        foreach(JSONObject enemy in EnemyData.list)
        {
            if(enemy["name"].str == name)
            {
                return enemy;
            }
        }
        return null;
    }

    public Vector2 PlayerLocation
    {
        get
        {
            return gps.PlayerLocation;
        }
    }

    public string GPSStatus
    {
        get
        {
            return gps.GPSStatus;
        }
    }

    public void StopGps()
    {
        gps.StopGPS();
    }
}
