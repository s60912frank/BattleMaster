using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapProcessor : MonoBehaviour {
    public const string API_KEY = "vector-tiles-vxQ7SnN";
    private const string MAP_TYPE = "boundaries,buildings,roads";
    private int xTile;  //儲存現在camera所在的地圖格(大概 不是很準)
    private int yTile;
    public string url;
	public static float latOrigin = 25.0417534f;
	public static float lonOrigin = 121.5339142f;
    private int zoom = 15; //放大倍率，1~19
    private List<MapTile> mapTiles; //儲存現在畫面上的mapTiles
    //private List<T>
    private int mapTileIndex = -1;
    public GameObject loadingPanel;
    //public Text gpsStatus;
	// Use this for initialization
	void Start () {
        //test畫圓
        /*GameObject whee = new GameObject();
        LineRenderer lr = whee.AddComponent<LineRenderer>();
        lr.SetWidth(15f, 15f);
        lr.SetVertexCount((int)((2.0 * Mathf.PI) / 0.02f) + 1);
        int i = 0;
        float radCircle = 0.015f;
        for (float theta = 0f; theta < (2 * Mathf.PI); theta += 0.02f)
        {
            // Calculate position of point
            float x = (radCircle * 100) * Mathf.Cos(theta);
            float y = (radCircle * 100) * Mathf.Sin(theta);

            // Set the position of this point
            Vector3 pos = new Vector3(x, y, 1);
            lr.SetPosition(i, pos);
            i++;
        }*/


        Debug.Log (Application.persistentDataPath);
        mapTiles = new List<MapTile>();
        //loading Start
        loadingPanel.GetComponent<LoadingScript>().Start();
        loadingPanel.GetComponent<LoadingScript>().StartLoading();
	}

    public void requestMap(float lon,float lat)
    {
        mapTiles.Add(new MapTile(lon, lat, zoom));
        mapTileIndex++;
        int xTile2 = mapTiles[mapTileIndex].xTile;
        int yTile2 = mapTiles[mapTileIndex].yTile;
        if (File.Exists(Application.persistentDataPath + "/" + zoom.ToString() + "_" + xTile2.ToString() + "_" + yTile2.ToString() + ".json"))
        {
            JsonProssor(File.ReadAllText(Application.persistentDataPath + "/" + zoom.ToString() + "_" + xTile2.ToString() + "_" + yTile2.ToString() + ".json"));
        }
        //沒的話跟伺服器要
        else
        {
            url = "https://vector.mapzen.com/osm/" + MAP_TYPE + "/" + zoom + "/" + xTile2 + "/" + yTile2 + ".json?api_key=" + API_KEY;
            Debug.Log(url);
            WWW request = new WWW(url);
            StartCoroutine(WaitForRequest(request));
        }
    }

    private void requestMap(int xtile, int ytile) //要地圖塊
    {
        loadingPanel.GetComponent<LoadingScript>().StartLoading();
        mapTiles.Add(new MapTile(xTile, yTile, zoom));
        mapTileIndex++;
        //如果之前有存過此地圖塊那就直接讀檔
        if (File.Exists(Application.persistentDataPath + "/" + zoom.ToString() + "_" + xTile.ToString() + "_" + yTile.ToString() + ".json"))
        {
            JsonProssor(File.ReadAllText(Application.persistentDataPath + "/" + zoom.ToString() + "_" + xTile.ToString() + "_" + yTile.ToString() + ".json"));
        }
        //沒的話跟伺服器要
        else
        {
            url = "https://vector.mapzen.com/osm/" + MAP_TYPE + "/" + zoom + "/" + xtile + "/" + ytile + ".json?api_key=" + API_KEY;
            Debug.Log(url);
            WWW request = new WWW(url);
            StartCoroutine(WaitForRequest(request));
        }
    }

    IEnumerator WaitForRequest(WWW www) //當資料從伺服器回來會執行這個
    {
        yield return www;
        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!");
            File.WriteAllText(Application.persistentDataPath + "/" + zoom.ToString() + "_" + xTile.ToString() + "_" + yTile.ToString() + ".json", www.text);
            JsonProssor(www.text); //有資料就去處理囉
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    private void JsonProssor(string rawData)
    {
        JSONObject data = new JSONObject(rawData);
        JSONObject buildings = data["buildings"]["features"]; //讀取資料中的"building"節點下的"features"節點
        foreach (JSONObject obj in buildings.list) //讀取這個節點的陣列
        {
            List<Vector2> coords = new List<Vector2>();
            string type = obj["geometry"]["type"].ToString().Replace("\"", ""); //因為type讀出來會有兩個雙引號，所以去掉再判斷
            switch (type)
            {
                case "Point":
                    //點不知怎畫，然而好像也不是很重要
                    break;
                case "Polygon":
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list[0].list) //polygon的座標陣列存在陣列[0]的陣列中
                    {
                        coords.Add(new Vector2(float.Parse(co[0].ToString()), float.Parse(co[1].ToString()))); //加到list中          
                    }
                    break;
                default:
                    Debug.Log(type);
                    break;
            }
            //存入該mapTile的點List
            //mapTiles[mapTileIndex].AddMapObj(type, coords);
            //不存了直接畫
            StartCoroutine(DrawMapObj("Polygon", mapTiles[mapTileIndex].CoordTransform(coords)));
        }

        JSONObject roads = data["roads"]["features"]; //讀取資料中的"roads"節點下的"features"節點
        foreach (JSONObject obj in roads.list)
        {
            //List<Vector2> coords = new List<Vector2>();
            string type = obj["geometry"]["type"].ToString().Replace("\"", "");
            switch (type)
            {
                case "LineString":
                    List<Vector2> coords = new List<Vector2>();
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list) //lineString的座標陣列存在陣列中
                    {
                        coords.Add(new Vector2(float.Parse(co[0].ToString()), float.Parse(co[1].ToString()))); 
                    }
                    StartCoroutine(DrawMapObj("LineString", mapTiles[mapTileIndex].CoordTransform(coords)));
                    break;
                case "MultiLineString":
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list) //multiLineString的座標陣列存在陣列中
                    {
                        List<Vector2> smallLine = new List<Vector2>(); //看名稱就知道啦這個一個就有很多條線，所以一條線就丟去畫
                        foreach (JSONObject co2 in co.list)
                        {
                            smallLine.Add(new Vector2(float.Parse(co2[0].ToString()), float.Parse(co2[1].ToString()))); 
                        }
                        //存入該mapTile的點List
                        //mapTiles[mapTileIndex].AddMapObj("LineString", smallLine);
                        StartCoroutine(DrawMapObj("LineString", mapTiles[mapTileIndex].CoordTransform(smallLine)));
                    }
                    break;
                default:
                    Debug.Log(type);
                    break;
            }
            //存入該mapTile的點List
            //mapTiles[mapTileIndex].AddMapObj("LineString", coords);
        }

        JSONObject boundaries = data["boundaries"]["features"]; //讀取資料中的"boundaries"節點下的"features"節點
        foreach (JSONObject obj in boundaries.list) //其實跟上面一樣
        {
            string type = obj["geometry"]["type"].ToString().Replace("\"", "");
            switch (type)
            {
                case "LineString":
                    List<Vector2> coords = new List<Vector2>();
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list)
                    {
                        coords.Add(new Vector2(float.Parse(co[0].ToString()), float.Parse(co[1].ToString()))); 
                    }
                    StartCoroutine(DrawMapObj("LineString", mapTiles[mapTileIndex].CoordTransform(coords)));
                    break;
                case "MultiLineString":
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list)
                    {
                        List<Vector2> smallLine = new List<Vector2>();
                        foreach (JSONObject co2 in co.list)
                        {
                            smallLine.Add(new Vector2(float.Parse(co2[0].ToString()), float.Parse(co2[1].ToString()))); 
                        }
                        //存入該mapTile的點List
                        //mapTiles[mapTileIndex].AddMapObj("LineString", smallLine);
                        StartCoroutine(DrawMapObj("LineString", mapTiles[mapTileIndex].CoordTransform(smallLine)));
                    }
                    break;
                default:
                    Debug.Log(type);
                    break;
            }
        }
        mapTiles[mapTileIndex].SetPlane();
        loadingPanel.GetComponent<LoadingScript>().EndLoading();
        Debug.Log("Done drawing map.");
    }

    private IEnumerator DrawMapObj(string type, Vector2[] vertices2D) //畫地圖物件
    {
        //yield return new WaitForEndOfFrame();
        yield return false;
        GameObject obj = new GameObject(); //創個新物體
        obj.tag = "MapObj"; //方便清掉
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++) //就只是2D轉3D
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, -0.1f); //一樣設0會有問題
        }
        switch (type)
        {
            case "Polygon":
                // Use the triangulator to get indices for creating triangles
                Triangulator tr = new Triangulator(vertices2D);
                int[] indices = tr.Triangulate();
                //Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                // Set up game object with mesh;
                MeshRenderer msgr = obj.AddComponent<MeshRenderer>();
                msgr.material = Resources.Load("building") as Material;
                MeshFilter filter = obj.AddComponent<MeshFilter>() as MeshFilter;
                filter.mesh = msh;
                break;
            case "Point":
                //還是不知怎畫
                break;
            case "LineString":
                //Test
                //HEREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
                /*Debug.Log(vertices.Length);
                Vector2[] newVecs = new Vector2[vertices.Length * 2];
                for (int i = 0; i < vertices.Length; i++)
                {
                    //line.SetPosition(i, vertices[i]);
                    newVecs[2 * i] = new Vector2(vertices[i].x + 0.1f, vertices[i].y + 0.1f);
                    newVecs[2 * i + 1] = new Vector2(vertices[i].x, vertices[i].y);
                }
                Vector3[] newVec3 = new Vector3[newVecs.Length];
                for (int i = 0; i < newVecs.Length; i++)
                {
                    newVec3[i] = newVecs[i];
                }
                Triangulator tr2 = new Triangulator(newVecs);
                int[] indices2 = tr2.Triangulate();
                //Create the mesh
                Mesh msh2 = new Mesh();
                msh2.vertices = newVec3;
                msh2.triangles = indices2;
                msh2.RecalculateNormals();
                msh2.RecalculateBounds();

                // Set up game object with mesh;
                MeshRenderer msgr2 = obj.AddComponent<MeshRenderer>();
                msgr2.material = Resources.Load("building") as Material;
                MeshFilter filter2 = obj.AddComponent<MeshFilter>() as MeshFilter;
                filter2.mesh = msh2;*/


                LineRenderer line = obj.AddComponent<LineRenderer>();
                //set the number of points to the line
                line.SetVertexCount(vertices.Length);
                for (int i = 0; i < vertices.Length; i++)
                {
                    line.SetPosition(i, vertices[i]);
                }
                //set the width
                line.SetWidth(0.1f, 0.1f);
                line.material = Resources.Load("road") as Material;
                //line.transform.parent = this.gameObject.transform;
                line.useWorldSpace = false;
                obj.SetActive(true);
                break;
        }
        //yield return false;
    }

    public void GetNewTileByWordPos(float x, float y)
    {
        //先除times再加原點
        //2^14/10
        const float times = 3276.8f;
        float lon = x / times + lonOrigin;
        float lat = y / times + latOrigin;
        //經緯度轉地圖格
        int xTile2 = (int)Mathf.Floor((lon + 180.0f) / 360.0f * (1 << zoom));
        int yTile2 = (int)Mathf.Floor(((1.0f - Mathf.Log(Mathf.Tan(lat * Mathf.PI / 180.0f) + 1.0f / Mathf.Cos(lat * Mathf.PI / 180.0f)) / Mathf.PI) / 2.0f * (1 << zoom)));
        GetNewTile(xTile2, yTile2);
    }

    public void GetNewTile(int xTile, int yTile) //跟伺服器要新地圖塊
    {
        bool found = false;
        foreach(MapTile mt in mapTiles)
        {
            if(mt.xTile == xTile && mt.yTile == yTile && mt.zoom == zoom)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            this.xTile = xTile; //往右??格
            this.yTile = yTile; //往下??格
            //mapTiles.Add(new MapTile(xTile, yTile, zoom));
            //mapTileIndex++;
            requestMap(xTile, yTile); //沒重複的話就要
        }
        else
        {
            Debug.Log("拒絕!!!!!!!!");
        }
    }

    public void GetNewZoomTile(object[] objs) //要不同縮放層級的地圖
    {
        mapTiles.Clear();  //清掉array
        mapTileIndex = 0; //index設為0
        int zoomDiff = (int)(objs[1] as int?);//zoom+1 or -1
        Vector2 camPos = (Vector2)(objs[0] as Vector2?);//cam的位置
        zoom += zoomDiff;
        float times = Mathf.Pow(2, zoom) / 10;
        //將遊戲座標轉回經緯度
        mapTiles.Add(new MapTile(camPos.x / times + lonOrigin, camPos.y / times + latOrigin, zoom));
        xTile = mapTiles[0].xTile;
        yTile = mapTiles[0].yTile;
        requestMap(mapTiles[0].xTile, mapTiles[0].yTile);
        Debug.Log(mapTiles[0].xTile + "/" + mapTiles[0].yTile);
    }
}
